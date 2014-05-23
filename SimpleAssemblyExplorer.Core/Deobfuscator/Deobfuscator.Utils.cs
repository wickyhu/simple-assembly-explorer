using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Security;
using System.Diagnostics;
using System.Xml;
using SimpleAssemblyExplorer;
using SimpleAssemblyExplorer.Plugin;
using SimpleAssemblyExplorer.LutzReflector;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public partial class Deobfuscator
    {
        public int DeobfFlowDirectCall(MethodDefinition searchInMethod)
        {
            int count = 0;
            if (searchInMethod == null || !searchInMethod.HasBody)
                return count;

            Mono.Cecil.Cil.MethodBody body = searchInMethod.Body;
            Collection<Instruction> instructions = body.Instructions;

            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction ins = instructions[i];
                if (ins.OpCode != OpCodes.Call && ins.OpCode != OpCodes.Callvirt)
                    continue;

                MethodReference operand = ins.Operand as MethodReference;
                MethodDefinition operandMethod = Resolve(operand);
                if (operandMethod == null || !operandMethod.HasBody) 
                    continue;
                if (operandMethod.DeclaringType != null)
                {
                    if (Utils.IsSystemType(operandMethod.DeclaringType.FullName))
                        continue;
                    if (searchInMethod.DeclaringType != null && operandMethod.DeclaringType.Module.Assembly.FullName != searchInMethod.DeclaringType.Module.Assembly.FullName)
                        continue;
                }

                Collection<Instruction> operandMethodInstructions = operandMethod.Body.Instructions;
                if (operandMethodInstructions.Count < 2)
                    continue;
                int index = operandMethodInstructions.Count - 1;
                Instruction targetIns = operandMethodInstructions[index];
                if (targetIns.OpCode.Code != Code.Ret)
                    continue;                

                index--;
                targetIns = operandMethodInstructions[index];
                while (targetIns.OpCode.Code == Code.Nop)
                {
                    index--;
                    if (index < 0) break;
                    targetIns = operandMethodInstructions[index];
                }
                if (index < 0)
                    continue;

                if (targetIns.OpCode != OpCodes.Call && targetIns.OpCode != OpCodes.Callvirt)
                    continue;

                MethodReference targetOperand = targetIns.Operand as MethodReference;
                MethodDefinition targetOperandMethod = Resolve(targetOperand);
                if (targetOperandMethod == null)
                    continue;

                if (operand.Parameters.Count != targetOperand.Parameters.Count)
                    continue;
                
                bool isOK = true;
                for (int j = 0; j < operand.Parameters.Count; j++)
                {
                    if (operand.Parameters[j].ParameterType.FullName != targetOperand.Parameters[j].ParameterType.FullName)
                    {
                        isOK = false;
                        break;
                    }
                }
                if (!isOK) continue;
                
                for (int j = 0; j < index; j++)
                {
                    if (!operandMethodInstructions[j].OpCode.Name.StartsWith("ldarg") && operandMethodInstructions[j].OpCode.Code != Code.Nop)
                    {
                        isOK = false;
                        break;
                    }
                }
                if (!isOK) continue;

                if (operandMethod.IsStatic) //static method
                {
                    if (targetOperandMethod.IsStatic)
                    {
                        ins.OpCode = targetIns.OpCode;
                        ins.Operand = targetIns.Operand;
                        count++;
                    }
                    //else
                    //{
                    //    //possible?
                    //}
                }
                else // instance method
                {
                    if (targetOperandMethod.IsStatic)
                    {
                        Instruction insPrev = ins.Previous;
                        if (insPrev != null && insPrev.OpCode.Code == Code.Ldarg_0)
                        {
                            insPrev.OpCode = OpCodes.Nop;
                            ins.OpCode = targetIns.OpCode;
                            ins.Operand = targetIns.Operand;
                            count++;                            
                        }
                    }
                    else
                    {
                        if (DeobfUtils.IsSameType(operandMethod.DeclaringType, targetOperandMethod.DeclaringType))
                        {
                            ins.OpCode = targetIns.OpCode;
                            ins.Operand = targetIns.Operand;
                            count++;
                        }
                    }
                }

                continue;
            }

            if (count > 0)
            {
                InsUtils.ComputeOffsets(instructions);
            }
            return count;
        }

        public int DeobfFlowDelegateCall(string assemblyFile, MethodDefinition searchInMethod)
        {
            int count = 0;
            if (searchInMethod == null || !searchInMethod.HasBody) 
                return count;

            Assembly a = AssemblyUtils.LoadAssemblyFile(assemblyFile);
            if (a == null) 
                return count;

            Mono.Cecil.Cil.MethodBody body = searchInMethod.Body;
            Collection<Instruction> instructions = body.Instructions;
            //bool hasDeobf = false;

            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction ins = instructions[i];
                if (ins.OpCode != OpCodes.Call && ins.OpCode != OpCodes.Callvirt)
                    continue;

                MethodReference operand = ins.Operand as MethodReference;
                if (operand == null || operand.Name != "Invoke")
                    continue;

                int paramCount = operand.Parameters.Count;

                int ldsfldIndex = -1;
                FieldDefinition fd = null;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (instructions[j].OpCode.Code == Code.Ldsfld)
                    {
                        fd = instructions[j].Operand as FieldDefinition;                        
                        if (fd != null && fd.IsStatic)
                        {
                            TypeDefinition fdType = Resolve(fd.FieldType);
                            TypeDefinition mdType = Resolve(operand.DeclaringType);
                            if (fdType != null && mdType != null &&
                                DeobfUtils.IsSameType(fdType, mdType) &&
                                DeobfUtils.IsDelegate(fdType))
                            {
                                ldsfldIndex = j;
                                break;
                            }
                        }

                    }
                }
                if (ldsfldIndex < 0 || fd == null)
                    continue;
                Instruction insLdsfld = instructions[ldsfldIndex];               

                Delegate dele;
                try
                {
                    FieldInfo fi = AssemblyUtils.ResolveField(a, fd);
                    dele = fi.GetValue(null) as Delegate;
                }
                catch//(Exception ex)
                {
                    dele = null;
                }
                if (dele == null)
                    continue;

                MethodBase mb = dele.Method;
                if (!mb.IsStatic)
                    continue;

                MethodBase targetMethod = null;
                OpCode targetOpCode = OpCodes.Call;
                List<Mono.Reflection.Instruction> list;

                if (Mono.Reflection.DynamicMethodHelper.IsDynamicOrRTDynamicMethod(mb))
                {
                    list = Mono.Reflection.MethodBodyReader.GetInstructions(mb);
                    Mono.Reflection.Instruction dynamicIns = null;
                    for (int j = list.Count - 1; j >= 0; j--)
                    {
                        dynamicIns = list[j];
                        if (dynamicIns.OpCode == System.Reflection.Emit.OpCodes.Call ||
                            dynamicIns.OpCode == System.Reflection.Emit.OpCodes.Callvirt)
                        {
                            targetMethod = dynamicIns.Operand as MethodBase;
                            break;
                        }
                    }
                    if (targetMethod != null)
                    {
                        if (dynamicIns.OpCode == System.Reflection.Emit.OpCodes.Callvirt)
                        {
                            targetOpCode = OpCodes.Callvirt;
                        }
                    }
                }
                else
                {
                    list = null;
                    targetMethod = mb;
                }

                if (targetMethod == null)
                    continue;

                MethodReference targetMr = Resolve(targetMethod);
                if (targetMr == null)
                {
                    targetMr = searchInMethod.Module.Import(targetMethod);
                }
                ILProcessor ilp = searchInMethod.Body.GetILProcessor();

                //TODO: 
                //for dynamic methods, there may be instructions between call and ldarg, 
                //they should be inserted into method body
                //but we need to know what should be copied and what shouldn't
                //so it's hard to guarantee the copied instructions are legal,
                //except simple cases like ldc.i4, sub, etc. 
                //now we only handle the known case

                //Smart Assembly 5 string functions pattern:
                //ldarg.0
                //ldc.i4 #
                //sub
                //call
                //ret                
                if (list != null)
                {
                    ParameterInfo[] targetParam = targetMethod.GetParameters();
                    if (targetParam != null && targetParam.Length == 1 && 
                        targetParam[0].ParameterType == typeof(int) &&
                        list.Count == 5 &&
                        list[0].OpCode == System.Reflection.Emit.OpCodes.Ldarg_0 &&
                        list[1].OpCode == System.Reflection.Emit.OpCodes.Ldc_I4 &&
                        list[2].OpCode == System.Reflection.Emit.OpCodes.Sub &&
                        list[4].OpCode == System.Reflection.Emit.OpCodes.Ret &&
                        ins.Previous != null &&
                        ins.Previous.OpCode.Code == Code.Ldc_I4
                        )
                    {
                        ins.Previous.Operand = (int)ins.Previous.Operand - (int)list[1].Operand;
                    }
                }

                //Smart Assembly 5 delegate call pattern:
                //ldarg.0
                //ldarg.1
                //...
                //ldarg #
                //callvirt or call
                //ret

                ins.OpCode = targetOpCode;
                ins.Operand = targetMr;
                InsUtils.ToNop(ilp, instructions, ldsfldIndex, false);

                count++;
                continue;
            }

            if (count>0)
            {
                InsUtils.ComputeOffsets(instructions);
            }
            return count;

        }

        public int DeobfString(string searchInFile, MethodDefinition searchInMethod, MethodDefinition searchForMethod,
            MethodDefinition calledMethod)
        {
            string assemblyFile = Path.Combine(_options.SourceDir, searchInFile);

            MethodBase _mb = null;
            if (calledMethod == null)
            {
                calledMethod = searchForMethod;
            }
            if (calledMethod != null)
            {
                try
                {
                    _mb = AssemblyUtils.ResolveMethod(calledMethod);
                }
                catch (Exception ex)
                {
                    _options.AppendTextInfoLine(ex.Message);
                }
                if (_mb == null) return 0;
            }

            int deobfCount = 0;
            try
            {
                if (searchForMethod == null) // autoString
                {
                    deobfCount += DeobfStringAutoStatic(assemblyFile, searchInMethod);
                    deobfCount += DeobfStringAutoDelegate(assemblyFile, searchInMethod);
                }
                else
                {
                    deobfCount += DeobfString(searchInMethod, searchForMethod, _mb, 0);
                }
            }
            catch (Exception ex)
            {
                _options.AppendTextInfoLine(ex.Message);
            }

            return deobfCount;
        }

        /// <summary>
        /// For auto string by delegate and static field
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="searchInMethod"></param>
        private int DeobfStringAutoDelegate(string assemblyFile, MethodDefinition searchInMethod)
        {
            if (searchInMethod == null || !searchInMethod.HasBody) return 0;
            if (_options.IgnoredMethodFile.IsExceedMaxInstructionCount(searchInMethod))
                return 0;

            Assembly a = AssemblyUtils.LoadAssemblyFile(assemblyFile);
            if (a == null) return 0;

            Mono.Cecil.Cil.MethodBody body = searchInMethod.Body;
            Collection<Instruction> instructions = body.Instructions;
            int deobfCount = 0;
            
            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction ins = instructions[i];
                if (ins.OpCode != OpCodes.Call && ins.OpCode != OpCodes.Callvirt) 
                    continue;

                MethodReference operand = ins.Operand as MethodReference;
                if (operand == null || operand.Name != "Invoke" ||
                    operand.ReturnType.FullName != "System.String")
                    continue;

                int startIndex;
                object[] p = SearchParameters(operand, i, searchInMethod, out startIndex);
                if (p == null)
                    continue;

                Instruction insStart = instructions[startIndex];
                Instruction insLdsfld = insStart.Previous;
                if (insLdsfld == null || insLdsfld.OpCode.Code != Code.Ldsfld)
                    continue;
             
                FieldDefinition fd = insLdsfld.Operand as FieldDefinition;
                if (fd == null)
                    continue;

                Delegate dele; 
                try
                {
                    FieldInfo fi = AssemblyUtils.ResolveField(a, fd);
                    dele = (Delegate)fi.GetValue(null);
                }
                catch (Exception ex)
                {
                    dele = null;
                    _options.AppendTextInfoLine(ex.Message);
                }
                if (dele == null)
                    continue;

                //Squall...@gmail.com.patch.start: handle ignored methods for delegate option
                if (_options.IgnoredMethodFile.IsMatch(dele.Method))
                    continue;
                //Squall...@gmail.com.patch.end

                object o;
                try
                {
                    o = dele.DynamicInvoke(p);
                }
                catch//(Exception ex)
                {
                    o = null;                                        
                }

                if (o != null)
                {
                    ILProcessor ilp = searchInMethod.Body.GetILProcessor();
                    //startIndex - 1 to include ldsfld
                    for (int j = startIndex - 1; j <= i; j++)
                    {
                        //here we don't want to insert nop, otherwise index will be wrong
                        InsUtils.ToNop(ilp, instructions, j, false);
                    }

                    //set the last instruction so br reference is ok
                    instructions[i].OpCode = OpCodes.Ldstr;
                    instructions[i].Operand = (string)o;

                    deobfCount++;
                }

            }

            if (deobfCount > 0)
            {
                InsUtils.ComputeOffsets(instructions);
            }

            return deobfCount;
        }

        /// <summary>
        /// For auto string by static method
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="searchInMethod"></param>
        private int DeobfStringAutoStatic(string assemblyFile, MethodDefinition searchInMethod)
        {
            if (searchInMethod == null || !searchInMethod.HasBody) return 0;
            if (_options.IgnoredMethodFile.IsExceedMaxInstructionCount(searchInMethod))
                return 0;

            Assembly a = AssemblyUtils.LoadAssemblyFile(assemblyFile);
            if (a == null) return 0;

            Mono.Cecil.Cil.MethodBody body = searchInMethod.Body;
            Collection<Instruction> instructions = body.Instructions;
            int deobfCount = 0;
            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction ins = instructions[i];
                if (ins.OpCode != OpCodes.Call) continue;

                MethodReference operand = ins.Operand as MethodReference;
                MethodDefinition md = Resolve(operand);

                if (md == null || !md.IsStatic || md.MetadataToken == searchInMethod.MetadataToken ||
                    _options.IgnoredMethodFile.IsMatch(md) ||
                    (md.ReturnType.FullName != "System.String" && md.ReturnType.FullName != "System.Object")
                    )
                    continue;

                MethodBase mb;
                try
                {
                    mb = AssemblyUtils.ResolveMethod(a, md);
                }
                catch// (Exception ex)
                {
                    mb = null;
                    //_options.AppendTextInfoLine(ex.Message);
                }
                if (mb == null) continue;

                deobfCount += DeobfString(searchInMethod, md, mb, i);
            }
            return deobfCount;
        }

        private object[] SearchParameters(MethodReference searchForMethod, int insIndex, MethodDefinition searchInMethod, out int startIndex)
        {
            Collection<Instruction> instructions = searchInMethod.Body.Instructions;
            int paramCount = searchForMethod.Parameters.Count;
            object[] p = new object[paramCount];
            startIndex = insIndex;

            int foundCount = 0;
            Dictionary<int, int> paramIndexes = new Dictionary<int, int>();
            Collection<ParameterDefinition> paramCollection = searchForMethod.Parameters;

            for (int j = paramCount - 1; j >= 0; j--)
            {
                bool paramFound = false;
                string paramTypeName = paramCollection[j].ParameterType.FullName;
                
                #region set opcode
                string[] opcode = null;
                switch (paramTypeName)
                {
                    case "System.String":
                        opcode = new string[] { "ldstr" };
                        break;
                    case "System.Int32":
                        opcode = new string[] { "ldloc", "ldc.i4" };
                        break;
                    case "System.Byte[]":
                        opcode = new string[] { "ldloc" };
                        break;
                    default:
                        break;
                }
                if (opcode == null)
                    break;
                #endregion 

                //search opcode
                for (int k = insIndex - 1; k >= insIndex - 4 && k >= 0; k--)
                {
                    //start from previous instruction, find first match
                    for (int l = 0; l < opcode.Length; l++)
                    {
                        if (instructions[k].OpCode.Name.StartsWith(opcode[l]) && !paramIndexes.ContainsKey(k))
                        {
                            paramFound = true;
                            paramIndexes.Add(k, k);
                            break;
                        }
                    }
                    if (!paramFound) continue;

                    int varIndex = InsUtils.GetVariableIndex(instructions[k]);
                    if (varIndex < 0)
                    {
                        p[j] = InsUtils.GetInstructionOperand(instructions[k]);
                    }
                    else
                    {
                        #region get variable value
                        //VariableReference vr = instructions[k].Operand as VariableReference;
                        VariableReference vr = searchInMethod.Body.Variables[varIndex];
                        //for (int l = 1; l < k; l++)
                        for (int l = k - 1; l > 0; l--)
                        {
                            //search matched stloc
                            if (!instructions[l].OpCode.Name.StartsWith("stloc"))
                                continue;
                            int varStIndex = InsUtils.GetVariableIndex(instructions[l]);
                            if (varStIndex < 0 || varStIndex != varIndex)
                                continue;

                            switch (paramTypeName)
                            {
                                case "System.Byte[]":
                                    Instruction insPrev = instructions[l].Previous;
                                    if (insPrev == null || insPrev.Operand == null) continue;
                                    MethodReference mr = insPrev.Operand as MethodReference;
                                    if (mr == null) continue;
                                    if (mr.Name == "InitializeArray")
                                    {
                                        insPrev = insPrev.Previous;
                                        if (insPrev == null || insPrev.Operand == null || insPrev.OpCode.Code != Code.Ldtoken)
                                            continue;
                                        FieldDefinition fd = insPrev.Operand as FieldDefinition;
                                        if (fd == null || fd.InitialValue == null) continue;
                                        p[j] = fd.InitialValue;
                                    }
                                    else if(mr is MethodDefinition)
                                    {
                                        MethodDefinition md = (MethodDefinition)mr;
                                        if (md.HasBody)
                                        {
                                            Collection<Instruction> ic = md.Body.Instructions;
                                            insPrev = ic[ic.Count - 1];
                                            if (insPrev == null || insPrev.OpCode.Code != Code.Ret) continue;
                                            insPrev = insPrev.Previous;
                                            if (insPrev == null || !insPrev.OpCode.Name.StartsWith("ldloc")) continue;
                                            insPrev = insPrev.Previous;
                                            if (insPrev == null || !insPrev.OpCode.Name.StartsWith("stloc")) continue;
                                            if (InsUtils.GetVariableIndex(insPrev) != InsUtils.GetVariableIndex(insPrev.Next)) continue;
                                            insPrev = insPrev.Previous;

                                            if (insPrev == null || insPrev.Operand == null) continue;
                                            MethodReference mr2 = insPrev.Operand as MethodReference;
                                            if (mr2 == null) continue;
                                            if (mr2.Name == "InitializeArray")
                                            {
                                                insPrev = insPrev.Previous;
                                                if (insPrev == null || insPrev.Operand == null || insPrev.OpCode.Code != Code.Ldtoken)
                                                    continue;
                                                FieldDefinition fd = insPrev.Operand as FieldDefinition;
                                                if (fd == null || fd.InitialValue == null) continue;
                                                p[j] = fd.InitialValue;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    //get value form ld before stloc
                                    p[j] = instructions[l - 1].Operand;
                                    if (p[j] != null && p[j].GetType().FullName != paramTypeName)
                                    {
                                        p[j] = null;
                                        paramFound = false;
                                    }
                                    break;
                            }
                            break;
                        }
                        #endregion get variable value
                    }

                    if(paramFound)
                        startIndex = k;

                    break;
                }

                if (paramFound)
                {
                    foundCount++;
                }
                else
                {
                    break;
                }
            }//end for each parameter

            //now startIndex is first instruction, insIndex is last instruction
            if (paramCount > 0)
            {
                if (foundCount != paramCount)
                    return null;

                // there still other instructions between startIndex and insIndex, we aren't sure about it...
                if (insIndex - startIndex != paramCount)
                    return null;
            }

            return p;
        }

        private int DeobfString(MethodDefinition searchInMethod, MethodDefinition searchForMethod, 
            MethodBase calledMethod, int fromIndex)
        {
            if (_options.IgnoredMethodFile.IsMatch(searchForMethod)) return 0;
            if (searchInMethod == null || !searchInMethod.HasBody) return 0;
            if (searchInMethod.MetadataToken == searchForMethod.MetadataToken) return 0;
            if (searchForMethod.ReturnType.FullName != "System.String" && searchForMethod.ReturnType.FullName != "System.Object") return 0;

            Mono.Cecil.Cil.MethodBody body = searchInMethod.Body;
            Collection<Instruction> instructions = body.Instructions;

            int deobfCount = 0;
            int errorCount = 0;

            for (int i = fromIndex; i < instructions.Count; i++)
            {
                Instruction ins = instructions[i];
                if (ins.OpCode != OpCodes.Call) continue;
                MethodDefinition operand = Resolve(ins.Operand as MethodReference);
                if (operand == null) continue;
                if (operand.MetadataToken != searchForMethod.MetadataToken) continue;

                int startIndex;
                object[] p = SearchParameters(searchForMethod, i, searchInMethod, out startIndex);
                if (p == null) continue;

                #region invoke method
                object o;
                try
                {
                    o = calledMethod.Invoke(null, p);
                }
                catch(Exception ex)
                {                    
                    o = null;
                    errorCount++;

                    while (ex.InnerException != null)
                        ex = ex.InnerException;
                    string errMsg = String.Format("Error when calling 0x{0:x08} ({1}): {2}",
                        calledMethod.MetadataToken,
                        calledMethod.ToString(),
                        ex.Message
                        );
                    _options.AppendTextInfoLine(errMsg, true);
                }

                if (o != null)
                {
                    if (searchForMethod.ReturnType.FullName != "System.String" && o.GetType().FullName != "System.String")
                        continue;

                    string oStr = (string)o;
                    if (_options.IgnoredMethodFile.IgnoreMethodsReturnPath && (Directory.Exists(oStr) || File.Exists(oStr)))
                        continue;

                    ILProcessor ilp = searchInMethod.Body.GetILProcessor();
                    //int size = 0;
                    for (int j = startIndex; j <= i; j++)
                    {
                        //size += instructions[i_1].GetSize();

                        //there is problem for remove operation, not consider br reference
                        //ilp.Remove(instructions[i_1]);//the instruction move up, so always is startIndex

                        //here we don't want to insert nop, otherwise index will be wrong
                        InsUtils.ToNop(ilp, instructions, j, false);
                    }

                    //insert may break br reference
                    //Instruction insertIns = ilp.Create(OpCodes.Ldstr, (string)o);
                    //int insertSize = insertIns.GetSize();
                    //ilp.InsertBefore(instructions[i_1], insertIns);

                    //set the last instruction so br reference is ok
                    instructions[i].OpCode = OpCodes.Ldstr;
                    instructions[i].Operand = oStr;

                    deobfCount++;
                }

                if (errorCount > this.Options.IgnoredMethodFile.MaxCallError)
                {
                    this.Options.IgnoredMethodFile.Ignore(searchForMethod.ToString());
                    break;
                }

                #endregion invoke method
            }

            if (deobfCount > 0)
            {
                InsUtils.ComputeOffsets(instructions);
            }
            return deobfCount;
        }

        public void WalkThroughMethod(MethodDefinition method, 
            Dictionary<Instruction, int> reachableInsts,
            List<Instruction> directJumpInsts,
            List<Instruction> conditionalJumpInsts)
        {
            Collection<Instruction> ic = method.Body.Instructions;

            if (reachableInsts == null)
            {
                reachableInsts = new Dictionary<Instruction, int>();
            }
            List<Instruction> toBeProcessed = new List<Instruction>();
            Instruction currentInstr;
            bool fallThrough = false;

            //add first instruction of all EH sections to be processed
            foreach (ExceptionHandler eh in method.Body.ExceptionHandlers)
            {
                if(eh.TryStart != null)
                    toBeProcessed.Add(eh.TryStart);
                if (eh.HandlerStart != null)
                    toBeProcessed.Add(eh.HandlerStart);
                if (eh.FilterStart != null)
                    toBeProcessed.Add(eh.FilterStart);
            }

            //add method entry point instruction to be processed, if it exists
            toBeProcessed.Add(method.Body.Instructions[0]);

            while (toBeProcessed.Count > 0)
            {
                //currentInstr = (Instruction)toBeProcessed[0];
                //toBeProcessed.RemoveAt(0);
                currentInstr = toBeProcessed[toBeProcessed.Count - 1];
                toBeProcessed.RemoveAt(toBeProcessed.Count - 1);
                
                if (currentInstr == null) 
                    continue;
                if (reachableInsts.ContainsKey(currentInstr))
                {
                    //don't reprocess places we've already been
                    continue;
                }

                do
                {
                    fallThrough = true;

                    if (reachableInsts.ContainsKey(currentInstr))
                    {
                        break;
                    }
                    else
                    {
                        reachableInsts.Add(currentInstr, currentInstr.Offset);

                        if (directJumpInsts != null && DeobfUtils.IsDirectJumpInstruction(currentInstr))
                        {
                            directJumpInsts.Add(currentInstr);
                        }

                        if (conditionalJumpInsts != null && DeobfUtils.IsConditionalJumpInstruction(currentInstr))
                        {
                            conditionalJumpInsts.Add(currentInstr);
                        }
                    }

                    //for branch and switch instructions, add targets to be processed
                    switch (currentInstr.OpCode.OperandType)
                    {
                        case OperandType.InlineSwitch:
                            foreach (Instruction t in (Instruction[])currentInstr.Operand)
                            {
                                if (t == null)
                                    continue;
                                toBeProcessed.Add(t);
                            }
                            break;
                        case OperandType.InlineBrTarget:
                        case OperandType.ShortInlineBrTarget:
                            toBeProcessed.Add((Instruction)currentInstr.Operand);
                            break;

                    }

                    //stop iterating if the next instruction can't be reached from this one
                    switch (currentInstr.OpCode.FlowControl)
                    {
                        case FlowControl.Branch:
                        case FlowControl.Return:
                        case FlowControl.Throw:
                            fallThrough = false;
                            break;

                    }

                    if (currentInstr.Next == null)
                        fallThrough = false;
                    else
                        currentInstr = currentInstr.Next;

                }
                while (fallThrough);
            }
        }

        /// <summary>
        /// DeobfFlowUnreachable: Patch from Pedgis S'igdep
        /// This function borrowed inspiration from CodeWriter.cs in Mono.Cecil.Cil
        /// </summary>
        /// <param name="method"></param>
        public int DeobfFlowUnreachable(MethodDefinition method)
        {
            if (method == null || !method.HasBody || method.Body.Instructions.Count < 1)
                return 0;

            Collection<Instruction> ic = method.Body.Instructions;

            Dictionary<Instruction, int> reachableInsts = new Dictionary<Instruction, int>();
            WalkThroughMethod(method, reachableInsts, null, null);

            //change the unreachable instructions to nops
            //bool hasUnreachable = false;
            int count = 0;
            ILProcessor ilp = method.Body.GetILProcessor();
            for (int i = 0; i < ic.Count; i++)
            {
                Instruction ins = ic[i];
                if (!reachableInsts.ContainsKey(ins) && !DeobfUtils.IsNopInstruction(ins))
                {
                    InsUtils.ToNop(ilp, ins, false);
                    //hasUnreachable = true;
                    count++;
                }

                //don't remove ins here, it will broken exception handler
            }

            InsUtils.ComputeOffsets(ic);
            FixExceptionHandler(method.Body.ExceptionHandlers);

            //remove unnecessary nop
            List<Instruction> nopToBeRemoved = new List<Instruction>();
            List<JumpInstruction> jumpInsList = null;
            for (int i = 0; i < ic.Count; i++)
            {
                Instruction ins = ic[i];
                if (ins.OpCode.Code == Code.Nop)
                {
                    if (jumpInsList == null)
                    {
                        jumpInsList = JumpInstruction.Find(method, -1, -1);
                    }
                    List<int> list = DeobfUtils.FindReferences(method, i, 1, jumpInsList, true);
                    if (list.Count == 0)
                    {
                        nopToBeRemoved.Add(ins);
                        count++;
                    }
                }
            }
            foreach (Instruction ins in nopToBeRemoved)
            {
                ilp.Remove(ins);
            }

            if (count > 0)
            {
                InsUtils.ComputeOffsets(ic);
            }
            return count;
        }

        public int DeobfFlowRemoveInvalidInstruction(MethodDefinition method)
        {
            if (method == null || !method.HasBody || method.Body.Instructions.Count < 1)
                return 0;

            int count = 0;
            Collection<Instruction> ic = method.Body.Instructions;
            ILProcessor ilp = method.Body.GetILProcessor();
            for (int i = 0; i < ic.Count; i++)
            {
                Instruction ins = ic[i];
                if (!InsUtils.IsValidInstruction(ins, method))
                {
                    InsUtils.ToNop(ilp, ins, false);
                    count++;
                }
            }
            if (count > 0)
            {
                InsUtils.ComputeOffsets(ic);
                FixExceptionHandler(method.Body.ExceptionHandlers);
            }
            return count;
        }

        public int DeobfFlowSwitch(MethodDefinition method)
        {
            if (method == null || !method.HasBody) return 0;
            return DeobfFlowSwitch(method, 0, method.Body.Instructions.Count - 1);
        }

        public int DeobfFlowSwitch(MethodDefinition method, int brStart, int brEnd)
        {
            int count = 0;
            if (method == null || !method.HasBody) return count;

            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            ILProcessor ilp = body.GetILProcessor();
            //bool hasDeobf = false;
            List<JumpInstruction> jumpInsList = JumpInstruction.Find(method, -1, -1);

            // ldc.i4
            // switch 
            for (int i = brStart; i <= brEnd; i++)
            {
                //search for switch
                Instruction ins = instructions[i];
                if (ins.OpCode != OpCodes.Switch) continue;

                Instruction[] ops = ins.Operand as Instruction[];
                if (ops == null) continue;

                //search for ldc.i4
                List<Instruction> listLdcI4 = DeobfUtils.FindPrevInstructions(ins, jumpInsList, "ldc.i4", null);
                if (listLdcI4.Count < 1) continue;

                foreach (Instruction insLdcI4 in listLdcI4)
                {
                    if (insLdcI4.OpCode.OperandType != OperandType.InlineI &&
                        insLdcI4.OpCode.OperandType != OperandType.InlineI8 &&
                        insLdcI4.OpCode.OperandType != OperandType.ShortInlineI)
                        continue;

                    //get ldc value
                    int v = (int)InsUtils.GetInstructionOperand(insLdcI4);
                    if (v < 0 || v >= ops.Length) continue;

                    if (insLdcI4.Offset < ins.Offset)
                        i++;

                    insLdcI4.OpCode = OpCodes.Br;
                    insLdcI4.Operand = ops[v];
                    //hasDeobf = true;
                    count++;
                }

            }
            if (count > 0)
            {
                InsUtils.ComputeOffsets(instructions);
            }

            //ldc.i4
            //stloc
            //ldloc
            //switch
            for (int i = brStart; i < brEnd; i++)
            {
                //search for switch
                Instruction ins = instructions[i];
                if (ins.OpCode != OpCodes.Switch) continue;

                Instruction[] ops = ins.Operand as Instruction[];
                if (ops == null) continue;

                //search for ldloc
                List<Instruction> listLd = DeobfUtils.FindPrevInstructions(ins, jumpInsList, "ldloc", null);
                if (listLd.Count < 1) continue;

                //search for stloc                
                foreach (Instruction insLd in listLd)
                {
                    //int insLdIndex = instructions.IndexOf(insLd);
                    List<Instruction> listSt = DeobfUtils.FindPrevInstructions(insLd, jumpInsList, "stloc", insLd.Operand);
                    if (listSt.Count < 1) continue;

                    foreach (Instruction insSt in listSt)
                    {
                        //search for ldc.i4
                        //int insStIndex = instructions.IndexOf(insSt);
                        List<Instruction> listLdcI4 = DeobfUtils.FindPrevInstructions(insSt, jumpInsList, "ldc.i4", null);
                        if (listLdcI4.Count < 1) continue;

                        foreach (Instruction insLdcI4 in listLdcI4)
                        {
                            if (insLdcI4.OpCode.OperandType != OperandType.InlineI &&
                                insLdcI4.OpCode.OperandType != OperandType.InlineI8 &&
                                insLdcI4.OpCode.OperandType != OperandType.ShortInlineI)
                                continue;

                            //get ldc value
                            int v = (int)InsUtils.GetInstructionOperand(insLdcI4);
                            if (v < 0 || v >= ops.Length) continue;

                            if (insSt.Offset < ins.Offset)
                                i++;

                            insLdcI4.OpCode = OpCodes.Br;
                            insLdcI4.Operand = ops[v];

                            //avoid empty stack
                            //if (IsEqual(insLdcI4.Next, insSt))
                            //{
                            //    Instruction insNew = ilp.Create(OpCodes.Ldc_I4, ops.Length);
                            //    ilp.InsertAfter(insLdcI4, insNew);
                            //}

                            //hasDeobf = true;
                            count++;
                        }
                    }
                }
            }

            if (count > 0)
            {
                InsUtils.ComputeOffsets(instructions);
            }

            return count;
        }

        /// <summary>
        /// Should be run after deobfbranch
        /// </summary>
        /// <param name="method"></param>
        public int DeobfFlowRemoveExceptionHandler(MethodDefinition method, bool removeGlobalExceptionHandler)
        {
            if (method == null || !method.HasBody || method.Body.ExceptionHandlers.Count == 0)
                return 0;
            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
            List<ExceptionHandler> toBeRemoved = new List<ExceptionHandler>();
            foreach (ExceptionHandler eh in ehc)
            {
                if (removeGlobalExceptionHandler && DeobfUtils.IsGlobalExceptionHandler(eh, method))
                {
                    toBeRemoved.Add(eh);
                    continue;
                }

                if (eh.HandlerType == ExceptionHandlerType.Catch &&
                    eh.HandlerStart != null &&
                    eh.HandlerEnd != null)
                {
                    Instruction insThrow = eh.HandlerEnd == null ? eh.HandlerEnd : eh.HandlerEnd.Previous;
                    int loopCount = 3;
                    while (insThrow.OpCode.Code != Code.Throw && insThrow.OpCode.Code != Code.Rethrow && insThrow.Previous != null && loopCount > 0)
                    {
                        insThrow = insThrow.Previous;
                        loopCount--;
                    }
                    if (insThrow != null && (insThrow.OpCode.Code == Code.Throw || insThrow.OpCode.Code == Code.Rethrow))
                    {
                        Instruction ins = insThrow.Previous;
                        switch (ins.OpCode.Code)
                        {
                            case Code.Call:
                            case Code.Newobj:
                                string s = ins.Operand.ToString();
                                if (_options.ExceptionHandlerFile.IsMatch(s))
                                {
                                    toBeRemoved.Add(eh);
                                }
                                break;
                        }
                    }
                    continue;
                }
            }

            int count = toBeRemoved.Count;
            if (toBeRemoved.Count > 0)
            {
                foreach (ExceptionHandler eh in toBeRemoved)
                {
                    ehc.Remove(eh);
                }
                count += DeobfFlowUnreachable(method);
            }
            return count;
        }

        public int DeobfFlowPatternInvalidPop(MethodDefinition method)
        {
            int count = 0;
            if(method == null || !method.HasBody || method.Body.ExceptionHandlers.Count < 1) 
                return count;

            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
            ILProcessor ilp = method.Body.GetILProcessor();
            foreach (ExceptionHandler eh in ehc)
            {
                Instruction start = eh.HandlerStart;
                if (start != null && start.OpCode.Code == Code.Pop &&
                    start.Previous != null && 
                        (
                        start.Previous.OpCode.Code == Code.Leave || start.Previous.OpCode.Code == Code.Leave_S || 
                        start.Previous.OpCode.Code == Code.Br || start.Previous.OpCode.Code == Code.Br_S 
                        ) 
                    )
                {
                    InsUtils.ToNop(ilp, start);
                    count++;
                }
            }
            return count;
        }

        public int DeobfFlowPattern(MethodDefinition method)
        {
            int count = 0;
            if (method == null || !method.HasBody || method.Body.Instructions.Count < 1)
                return count;

            if (_options.PatternFile.NopInvalidPop)
            {
                count += DeobfFlowPatternInvalidPop(method);
            }

            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            ILProcessor ilp = body.GetILProcessor();

            List<Pattern> dummyCodes = _options.PatternFile.DummyCodes;
            List<JumpInstruction> jumpInsList = null;
            bool go;

            //bool foundPattern = false;

            for (int i = 0; i < dummyCodes.Count; i++)
            {
                Pattern searchFor = dummyCodes[i];
                List<int> matched = new List<int>(searchFor.MaxLength);
                int index = SearchInstructions(instructions, searchFor, 0, matched);
                while (index >= 0)
                {
                    //foundPattern = true;
                    count++;

                    bool canNop = true;
                    if (searchFor.CheckReference)
                    {
                        for (int j = 0; j < searchFor.MaxLength; j++)
                        {
                            if (searchFor.OpCodeGroups[j].ToNop && matched[j] >= 0)
                            {
                                if (jumpInsList == null)
                                {
                                    jumpInsList = JumpInstruction.Find(method, -1, -1);
                                }
                                if (DeobfUtils.FindReferences(method, matched[j], 1, jumpInsList, false).Count > 0)
                                {
                                    canNop = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (canNop)
                    {
                        for (int j = 0; j < searchFor.MaxLength; j++)
                        {
                            if (searchFor.OpCodeGroups[j].ToNop && matched[j] >= 0)
                            {
                                InsUtils.ToNop(ilp, instructions, matched[j]);
                            }
                        }
                    }

                    index = matched[matched.Count - 1];
                    index++;
                    index = SearchInstructions(instructions, dummyCodes[i], index, matched);
                }

                OpCodeGroup lastOpCodeGroup = searchFor.OpCodeGroups[searchFor.OpCodeGroups.Length - 1];
                if (lastOpCodeGroup.OpCodes.Length == 1)
                {
                    string lastOne = lastOpCodeGroup.OpCodes[0];
                    go = false;
                    if (lastOne == "brtrue")
                    {
                        lastOpCodeGroup.OpCodes[0] = "brfalse";
                        go = true;
                    }
                    else if (lastOne == "brfalse")
                    {
                        lastOpCodeGroup.OpCodes[0] = "brtrue";
                        go = true;
                    }

                    if (go)
                    {
                        try
                        {
                            index = SearchInstructions(instructions, searchFor, 0, matched);
                            while (index >= 0)
                            {
                                //foundPattern = true;
                                count++;

                                bool canNop = true;
                                if (searchFor.CheckReference)
                                {
                                    for (int j = 0; j < searchFor.MaxLength - 1; j++)
                                    {
                                        if (searchFor.OpCodeGroups[j].ToNop && matched[j] >= 0)
                                        {
                                            if (jumpInsList == null)
                                            {
                                                jumpInsList = JumpInstruction.Find(method, -1, -1);
                                            }
                                            if (DeobfUtils.FindReferences(method, matched[j], 1, jumpInsList, false).Count > 0)
                                            {
                                                canNop = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (canNop)
                                {
                                    for (int j = 0; j < searchFor.MaxLength - 1; j++)
                                    {
                                        if (searchFor.OpCodeGroups[j].ToNop && matched[j] >= 0)
                                        {
                                            InsUtils.ToNop(ilp, instructions, matched[j]);
                                        }                                       
                                    }
                                }

                                index = matched[matched.Count - 1];
                                instructions[index].OpCode = OpCodes.Br;

                                index++;
                                index = SearchInstructions(instructions, searchFor, index, matched);
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            lastOpCodeGroup.OpCodes[0] = lastOne;
                        }
                    }
                }
            }

            if (count > 0)
            {
                InsUtils.ComputeOffsets(instructions);
            }
            return count;
        }

        public int DeobfJumpToJump(MethodDefinition method, int brStart, int brEnd)
        {
            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            ILProcessor ilp = body.GetILProcessor();
            int count = 0;

            for (int i = brStart; i <= brEnd; i++)
            {
                if (DeobfUtils.IsDirectJumpInstruction(instructions[i]))
                {
                    Instruction insJumpFrom = instructions[i];

                    Instruction insJumpToOld = insJumpFrom.Operand as Instruction;
                    Instruction insJumpTo = insJumpToOld;

                    //skip nop
                    while (DeobfUtils.IsNopInstruction(insJumpTo))
                    {
                        insJumpTo = insJumpTo.Next;
                    }

                    //jump to nothing
                    if (insJumpTo == null)
                    {
                        continue;
                    }

                    while (DeobfUtils.IsDirectJumpInstruction(insJumpTo) || DeobfUtils.IsDirectLeaveInstruction(insJumpTo))
                    {
                        insJumpTo = insJumpTo.Operand as Instruction;
                    }

                    if (!DeobfUtils.IsEqual(insJumpTo, insJumpToOld))
                    {
                        insJumpFrom.Operand = insJumpTo;
                        count++;
                    }
                }
            }

            return count;
        }

        public int DeobfFlowMoveBlock(MethodDefinition method, int maxMoveCount, int maxRefCount)
        {
            int count = 0;

            if (method == null || !method.HasBody || method.Body.Instructions.Count < 3)
                return count;

            List<InstructionBlock> list = InstructionBlock.Find(method);
            if (list.Count < 3) return count;

            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            int loopCount = list.Count;
            for (int i = 0; i < loopCount; i++)
            {
                if (maxMoveCount > 0 && count >= maxMoveCount)
                    break;

                foreach (InstructionBlock ib in list)
                {
                    InstructionBlock ibMove = ib.NextBlock;
                    if (ibMove == null || ib.EndIndex + 1 == ibMove.StartIndex)
                        continue;
                    if (maxRefCount > 0 && ibMove.RefCount > maxRefCount)
                        continue;
                    if (ib.StartIndex < ibMove.StartIndex && ibMove.JumpDownRefCount > 0)
                        continue;
                    Instruction insJumpFrom = instructions[ib.EndIndex];
                    if (DeobfUtils.IsLeaveInstruction(insJumpFrom))
                        continue;
                    if (IsNearFromOperand(instructions[ibMove.EndIndex]))
                        continue;
                    if (DeobfUtils.IsForeachStatement(insJumpFrom, method, true))
                        continue;
                    if (DeobfUtils.IsSwitchBranchDefault(insJumpFrom, method))
                        continue;
                    if (DeobfUtils.IsBranchMoveBreakingExceptionHandler(ib.EndIndex, ibMove.StartIndex, method))
                        continue;

                    FixExceptionHandlerBeforeMoveBlock(method, ib.EndIndex, ibMove.StartIndex, ibMove.EndIndex);
                    DeobfUtils.MoveBlock(body, ib.EndIndex, ibMove.StartIndex, ibMove.EndIndex);
                    count++;
                    break;
                }
                
                list.Clear();
                list = null;
                list = InstructionBlock.Find(method);
            }

            if (count > 0)
            {
                InsUtils.ComputeOffsets(instructions);
                FixExceptionHandler(body.ExceptionHandlers);
            }

            return count;
        }

        public int DeobfFlowBranch(MethodDefinition method, int brStart, int brEnd, int maxMoveCount, int maxRefCount)
        {
            int moveCount = 0;
            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            ILProcessor ilp = body.GetILProcessor();
            List<JumpInstruction> jumpInsList = null;
            bool hasDeobf = false;

            //DeobfJumpToJump(method, brStart, brEnd);

            List<Instruction> directJumpInsts = null;
            bool canChangLoop = true;
            if (_options.BranchDirection == BranchDirections.WalkThrough)
            {
                canChangLoop = false;
                directJumpInsts = new List<Instruction>();
                WalkThroughMethod(method, null, directJumpInsts, null);
                brStart = 0;
                brEnd = directJumpInsts.Count - 1;
            }

            for (int loop = brStart; loop <= brEnd; loop++)
            {
                int i;
                switch (_options.BranchDirection)
                {
                    case BranchDirections.TopDown:
                        i = loop;
                        break;
                    case BranchDirections.WalkThrough:
                        i = instructions.IndexOf(directJumpInsts[loop]);
                        break;
                    case BranchDirections.BottomUp:
                        i = brStart + brEnd - loop;
                        break;
                    default:
                        throw new NotSupportedException("Unexpected branch direction: " + Enum.GetName(typeof(BranchDirections), _options.BranchDirection));
                }
                
                if (maxMoveCount > 0 && moveCount >= maxMoveCount) break;

                Instruction insJumpFrom = instructions[i];
                if (!DeobfUtils.IsDirectJumpInstruction(insJumpFrom))
                    continue;

                Instruction insJumpToOld = insJumpFrom.Operand as Instruction;
                Instruction insJumpTo = insJumpToOld;

                int fromIndex = i;
                int startIndex = instructions.IndexOf(insJumpTo);

                //skip nop
                while (DeobfUtils.IsNopInstruction(insJumpTo))
                {
                    insJumpTo = insJumpTo.Next;
                }

                //jump to nothing
                if (insJumpTo == null)
                {
                    int insSize = InsUtils.ToNop(ilp, instructions, fromIndex) - 1;
                    if (canChangLoop)
                        brEnd += insSize;
                    moveCount++;
                    continue;
                }

                while (
                    DeobfUtils.IsDirectJumpInstruction(insJumpTo) || 
                    DeobfUtils.IsDirectLeaveInstruction(insJumpTo)
                    )
                {
                    insJumpTo = insJumpTo.Operand as Instruction;
                }

                Instruction insJumpFromNext = insJumpFrom.Next;
                while (DeobfUtils.IsNopInstruction(insJumpFromNext))
                {
                    insJumpFromNext = insJumpFromNext.Next;
                }

                if (insJumpTo == insJumpFromNext)
                {
                    int insSize = InsUtils.ToNop(ilp, instructions, fromIndex) - 1;
                    if (canChangLoop)
                        brEnd += insSize;
                    moveCount++;
                    continue;
                }

                if (!DeobfUtils.IsEqual(insJumpTo, insJumpToOld))
                {
                    startIndex = instructions.IndexOf(insJumpTo);
                    insJumpFrom.Operand = insJumpTo;
                    insJumpToOld = insJumpTo;
                    moveCount++;
                    hasDeobf = true;
                }                

                if (startIndex == fromIndex)
                    continue;

                if (DeobfUtils.IsForeachStatement(insJumpFrom, method, true))
                    continue;
                if (DeobfUtils.IsSwitchBranchDefault(insJumpFrom, method))
                    continue;

                if (maxRefCount > 0)
                {
                    if (jumpInsList == null)
                    {
                        jumpInsList = JumpInstruction.Find(method, -1, -1);
                    }
                    if (DeobfUtils.FindReferences(method, startIndex, maxRefCount + 1, jumpInsList, false).Count > maxRefCount)
                        continue;
                }

                //if (IsSwitchBranch(insJumpFrom, method)) 
                //    continue;
                //if (IsExceptionHandlerEnd(insJumpTo, body.ExceptionHandlers)) 
                //    continue;


                Instruction insJumpToPrev = insJumpTo.Previous;
                int prevCount = 0;
                while (DeobfUtils.IsNopInstruction(insJumpToPrev))
                {
                    insJumpTo = insJumpToPrev;
                    insJumpToPrev = insJumpToPrev.Previous;
                    prevCount++;
                    if (prevCount > instructions.Count) // dead loop
                    {
                        insJumpToPrev = null;
                        prevCount = -1;
                        break;
                    }
                }

                if (!DeobfUtils.IsEqual(insJumpTo, insJumpToOld))
                {
                    startIndex = instructions.IndexOf(insJumpTo);
                    //insJumpFrom.Operand = insJumpTo;
                    //moveCount++;
                    //hasDeobf = true;
                }

                if (prevCount < 0 || startIndex == fromIndex)
                    continue;

                if (insJumpToPrev != null &&
                    !DeobfUtils.IsDirectJumpInstruction(insJumpToPrev) &&
                    !DeobfUtils.IsExitInstruction(insJumpToPrev)
                    )
                {
                    continue;
                }

                //bool found = false;
                //Instruction insLast;
                int endIndex = -1;
                int saveFromIndex = fromIndex;
                int saveStartIndex = startIndex;

                if (startIndex < fromIndex) // jump up
                {
                    if (SearchJumpToEnd(method, ref fromIndex, ref startIndex, ref endIndex))
                    {
                        if (DeobfUtils.IsBranchMoveBreakingExceptionHandler(saveFromIndex, saveStartIndex, method))
                            continue;
                        if (DeobfUtils.IsBranchMoveBreakingExceptionHandler(fromIndex, startIndex, method))
                            continue;

                        FixExceptionHandlerBeforeMoveBlock(method, fromIndex, startIndex, endIndex);

                        if (jumpInsList != null)
                        {
                            JumpInstruction.Remove(jumpInsList, instructions[endIndex]);
                        }
                        int insSize = InsUtils.ToNop(ilp, instructions, endIndex);                        
                        
                        if(canChangLoop)
                            brEnd += insSize - 1;

                        DeobfUtils.MoveBlock(body, fromIndex, startIndex, endIndex);
                        moveCount++;
                        hasDeobf = true;
                        
                    }
                }
                else //jump down
                {
                    if (SearchJumpToEnd(method, ref fromIndex, ref startIndex, ref endIndex))
                    {
                        if (DeobfUtils.IsBranchMoveBreakingExceptionHandler(fromIndex, startIndex, method))
                            continue;

                        if (IsNearFromOperand(instructions[endIndex]))
                            continue;

                        FixExceptionHandlerBeforeMoveBlock(method, fromIndex, startIndex, endIndex);

                        DeobfUtils.MoveBlock(body, fromIndex, startIndex, endIndex);
                        moveCount++;
                        hasDeobf = true;

                        if(canChangLoop && DeobfUtils.IsExitInstruction(insJumpFrom.Next))
                        {
                            loop++;
                        }
                        if (jumpInsList != null)
                        {
                            JumpInstruction.Remove(jumpInsList, instructions[fromIndex]);
                        }
                        int insSize = InsUtils.ToNop(ilp, instructions, fromIndex);

                        if (canChangLoop)
                            brEnd += insSize - 1;
                    }
                }

                //foreach (ExceptionHandler eh in method.Body.ExceptionHandlers)
                //{
                //    if (!DeobfUtils.IsValidExceptionHandler(eh))
                //        throw new ApplicationException("FixExceptionHandler: internal error");
                //}

            } //end main loop

            if (hasDeobf)
            {
                InsUtils.Simplify(instructions);
                //InsUtils.InsUtils.ComputeOffsets(instructions);
                //how to handle embbed exception handler?
                FixExceptionHandler(body.ExceptionHandlers);
            }

            return moveCount;
        }

        private bool IsNearFromOperand(Instruction ins)
        {
            if (ins.Operand is Instruction)
            {
                Instruction tmpNext = ins.Next;
                Instruction tmpTo = ins.Operand as Instruction;
                if (tmpNext != null && tmpNext.OpCode.Name.StartsWith("ld") &&
                    tmpNext.Next != null && tmpNext.Next.Offset == tmpTo.Offset &&
                    tmpNext.Next.OpCode.Code == tmpTo.OpCode.Code)
                {
                    return true;
                }
            }
            return false;
        }

        public int DeobfBoolFunction(string assemblyFile, MethodDefinition searchInMethod)
        {
            int count = 0;
            if (searchInMethod == null || !searchInMethod.HasBody) return count;

            Assembly a = AssemblyUtils.LoadAssemblyFile(assemblyFile);
            if (a == null) return count;

            Mono.Cecil.Cil.MethodBody body = searchInMethod.Body;
            Collection<Instruction> instructions = body.Instructions;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction ins = instructions[i];
                if (ins.OpCode != OpCodes.Call) continue;

                MethodDefinition operand = ins.Operand as MethodDefinition;
                if (operand == null || !operand.IsStatic ||
                    operand.ReturnType.FullName != "System.Boolean"
                    )
                    continue;

                MethodBase mb;
                try
                {
                    mb = AssemblyUtils.ResolveMethod(a, operand);
                }
                catch (Exception ex)
                {
                    mb = null;
                    _options.AppendTextInfoLine(ex.Message);
                }
                if (mb == null) continue;

                count += DeobfBoolFunction(searchInMethod, operand, mb);
            }
            return count;
        }

        public int DeobfBoolFunction(MethodDefinition searchInMethod, MethodDefinition searchForMethod, MethodBase mb)
        {
            int count = 0;
            
            if (_options.IgnoredMethodFile.IsMatch(searchForMethod)) return count;
            if (searchInMethod == null || !searchInMethod.HasBody) return count;
            if (searchInMethod.MetadataToken == searchForMethod.MetadataToken) return count;
            if (searchForMethod.ReturnType.FullName != "System.Boolean") return count;
            if (searchForMethod.Parameters.Count != 0) return count;

            Mono.Cecil.Cil.MethodBody body = searchInMethod.Body;
            Collection<Instruction> instructions = body.Instructions;

            //bool hasDeobf = false;

            for (int i = 0; i < instructions.Count; i++)
            {
                Instruction ins = instructions[i];
                if (ins.OpCode != OpCodes.Call) continue;
                MethodDefinition operand = ins.Operand as MethodDefinition;
                if (operand == null) continue;
                if (operand.MetadataToken != searchForMethod.MetadataToken) continue;

                #region invoke method
                object o;
                try
                {
                    o = mb.Invoke(null, null);
                }
                catch(Exception ex)
                {
                    o = null;
                    _options.IgnoredMethodFile.Ignore(searchForMethod.ToString());

                    while (ex.InnerException != null)
                        ex = ex.InnerException;
                    string errMsg = String.Format("Error when calling 0x{0:x08} ({1}): {2}",
                        mb.MetadataToken,
                        mb.ToString(),
                        ex.Message
                        );
                    _options.AppendTextInfoLine(errMsg, true);

                }

                if (o != null)
                {
                    if (Convert.ToBoolean(o))
                    {
                        instructions[i].OpCode = OpCodes.Ldc_I4_1;
                    }
                    else
                    {
                        instructions[i].OpCode = OpCodes.Ldc_I4_0;
                    }
                    instructions[i].Operand = null;
                    count++;
                    //hasDeobf = true;
                }
                else
                {
                    break;
                }

                #endregion invoke method
            }

            if (count > 0)
            {
                InsUtils.ComputeOffsets(instructions);
            }
            return count;
        }

        public int DeobfFlowBranch(MethodDefinition method, int loopCount, int maxMoveCount, int maxRefCount)
        {
            int moveCount = 0;
            if (method == null || !method.HasBody || method.Body.Instructions.Count < 3)
                return moveCount;

            Mono.Cecil.Cil.MethodBody body = method.Body;

            moveCount = DeobfFlowBranch(method, 0, body.Instructions.Count - 1, maxMoveCount, maxRefCount);
            return moveCount;
        }

        private bool CanChangeBJumpToBrTrueFalse(Instruction bJump)
        {
            /*
            // sometimes useful for non-obfuscated statements             
            Instruction next = bJump.Next;
            while (next != null && !DeobfUtils.IsExitInstruction(next) && !DeobfUtils.IsDirectJumpInstruction(next))
            {
                next = next.Next;
            }
            
            if (DeobfUtils.IsExitInstruction(next))
            {
                next = bJump.Operand as Instruction;
                while (next != null && !DeobfUtils.IsExitInstruction(next) && !DeobfUtils.IsDirectJumpInstruction(next))
                {
                    next = next.Next;
                }
                if (DeobfUtils.IsExitInstruction(next))
                {
                    return false;
                }
            }
            */

            return true;
        }

        private int DeobfFlowConditionalBranchToBrTrueFalse(MethodDefinition method, int brStart, int brEnd)
        {
            //this method is sometimes bad for non-obfuscated statements ...

            //if (method == null || !method.HasBody || method.Body.Instructions.Count < 3)
            //    return 0;

            int count = 0;
            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            ILProcessor ilp = body.GetILProcessor();

            for (int i = brStart; i <= brEnd; i++)
            {
                Instruction ins = instructions[i];
                Instruction newIns;
                switch (ins.OpCode.Code)
                {
                    case Code.Beq:
                    case Code.Beq_S:
                        if (CanChangeBJumpToBrTrueFalse(ins))
                        {
                            newIns = InsUtils.CreateInstruction(OpCodes.Ceq, null);
                            ilp.InsertBefore(ins, newIns);
                            ins.OpCode = OpCodes.Brtrue;
                            i++;
                            count++;
                        }
                        break;
                    case Code.Bgt:
                    case Code.Bgt_S:
                        if (CanChangeBJumpToBrTrueFalse(ins))
                        {
                            newIns = InsUtils.CreateInstruction(OpCodes.Cgt, null);
                            ilp.InsertBefore(ins, newIns);
                            ins.OpCode = OpCodes.Brtrue;
                            i++;
                            count++;
                        }
                        break;
                    case Code.Blt:
                    case Code.Blt_S:
                        if (CanChangeBJumpToBrTrueFalse(ins))
                        {
                            newIns = InsUtils.CreateInstruction(OpCodes.Clt, null);
                            ilp.InsertBefore(ins, newIns);
                            ins.OpCode = OpCodes.Brtrue;
                            i++;
                            count++;
                        }
                        break;
                    case Code.Bgt_Un:
                    case Code.Bgt_Un_S:
                        if (CanChangeBJumpToBrTrueFalse(ins))
                        {
                            newIns = InsUtils.CreateInstruction(OpCodes.Cgt_Un, null);
                            ilp.InsertBefore(ins, newIns);
                            ins.OpCode = OpCodes.Brtrue;
                            i++;
                            count++;
                        }
                        break;
                    case Code.Blt_Un:
                    case Code.Blt_Un_S:
                        if (CanChangeBJumpToBrTrueFalse(ins))
                        {
                            newIns = InsUtils.CreateInstruction(OpCodes.Clt_Un, null);
                            ilp.InsertBefore(ins, newIns);
                            ins.OpCode = OpCodes.Brtrue;
                            i++;
                            count++;
                        }
                        break;
                    //TODO: change other conditional jump to brtrue or brfalse
                }
            }
            return count;
        }

        public int DeobfFlowConditionalBranch(MethodDefinition method, int maxMoveCount)
        {
            int moveCount = 0;
            if (method == null || !method.HasBody || method.Body.Instructions.Count < 3)
                return moveCount;

            Mono.Cecil.Cil.MethodBody body = method.Body;
            moveCount = DeobfFlowConditionalBranch(method, 0, body.Instructions.Count - 1, maxMoveCount);
            return moveCount;
        }

        public int DeobfFlowReflectorFixDupBrTrueFalsePop(MethodDefinition method, int maxMoveCount)
        {
            // Reflector can't handle dup;brtrue/brfalse;pop style (?? operator in c#)
            // So would like to convert it to ? operator or if statement

            int count = 0;

            if (method == null || !method.HasBody || method.Body.Instructions.Count < 3)
                return count;

            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            ILProcessor ilp = body.GetILProcessor();
            List<VariableDefinition> newVarList = new List<VariableDefinition>();

            int brStart = 0;
            int brEnd = instructions.Count - 1;
            for (int loop = brStart; loop <= brEnd; loop++)
            {
                int i;
                i = loop;

                if (maxMoveCount > 0 && count >= maxMoveCount) break;

                Instruction insJumpFrom = instructions[i];
                if (!DeobfUtils.IsConditionalJumpInstruction(insJumpFrom))
                    continue;

                // Original:
                // ...
                // ld... or call..
                // dup <- there should be no jump to here, no checking currently
                // brtrue or brfalse
                // pop            
                // ...

                // Change To:
                // ...
                // ld... or stloc;ldloc...
                // brfalse or brtrue
                // ld...
                // br          
                // ...

                OpCode opc;
                if (insJumpFrom.OpCode.Name.StartsWith("brtrue"))
                    opc = OpCodes.Brfalse;
                else if (insJumpFrom.OpCode.Name.StartsWith("brfalse"))
                    opc = OpCodes.Brtrue;
                else
                    continue;

                Instruction insDup = insJumpFrom.Previous;
                if (insDup == null || insDup.OpCode.Code != Code.Dup)
                    continue;
                
                Instruction insLd = insDup.Previous;
                if (insLd == null) 
                    continue;

                if (!insLd.OpCode.Name.StartsWith("ldarg") &&
                    !insLd.OpCode.Name.StartsWith("ldc") &&
                    !insLd.OpCode.Name.StartsWith("ldf") &&
                    !insLd.OpCode.Name.StartsWith("ldnull") &&
                    !insLd.OpCode.Name.StartsWith("ldloc") &&
                    !insLd.OpCode.Name.StartsWith("lds") &&
                    !insLd.OpCode.Name.StartsWith("call")
                )
                    continue;

                Instruction insPop = insJumpFrom.Next;
                if (insPop == null || insPop.OpCode.Code != Code.Pop)
                    continue;

                if (insLd.OpCode.Name.StartsWith("call"))
                {
                    MethodReference mr = insLd.Operand as MethodReference;
                    if (mr == null || mr.ReturnType.FullName == "System.Void")
                        continue;

                    VariableDefinition newVd = null;
                    foreach (VariableDefinition vd in newVarList)
                    {
                        if (DeobfUtils.IsSameType(vd.VariableType, mr.ReturnType))
                        {
                            newVd = vd;
                            break;
                        }
                    }
                    if (newVd == null)
                    {
                        newVd = new VariableDefinition(mr.ReturnType);
                        newVarList.Add(newVd);
                    }
                    body.Variables.Add(newVd);
                    
                    Instruction newInsSt = Instruction.Create(OpCodes.Stloc, newVd);
                    Instruction newInsLd = Instruction.Create(OpCodes.Ldloc, newVd);

					//Should be InsertAfter, contributed by beket...@gmail.com
                    //ilp.InsertBefore(insLd, newInsSt);
                    //ilp.InsertBefore(insLd, newInsLd);
					ilp.InsertAfter(insLd, newInsLd);
					ilp.InsertAfter(insLd, newInsSt);

					insLd = newInsLd;
                }

                insDup.OpCode = opc;
                insDup.Operand = insPop.Next;
                insPop.OpCode = OpCodes.Br;
                insPop.Operand = insJumpFrom.Operand;
                insJumpFrom.OpCode = insLd.OpCode;
                insJumpFrom.Operand = insLd.Operand;

                if (insLd.OpCode.Name.StartsWith("ldfld"))
                {
                    //it's no necessary this.Field, contributed by beket...@gmail.com
                    //Instruction newIns = Instruction.Create(OpCodes.Ldarg_0);
                    Instruction newIns = Instruction.Create(insLd.Previous.OpCode);
                    ilp.InsertBefore(insJumpFrom, newIns);
                }
                count++;
            }

            if (count > 0)
            {
                InsUtils.ComputeOffsets(instructions);
                //FixExceptionHandler(body.ExceptionHandlers);
            }

            return count;
        }

        public int DeobfFlowReflectorFixBrTrueFalsePopLdDup(MethodDefinition method, int maxMoveCount)
        {
            // Reflector can't handle brtrue/brfalse;pop;ld;dup style
            // "Goto statement target does not exist." error if there is jump to last dup

            int count = 0;

            if (method == null || !method.HasBody || method.Body.Instructions.Count < 3)
                return count;

            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            ILProcessor ilp = body.GetILProcessor();
            List<VariableDefinition> newVarList = new List<VariableDefinition>();

            int brStart = 0;
            int brEnd = instructions.Count - 1;
            for (int loop = brStart; loop <= brEnd; loop++)
            {
                int i;
                i = loop;

                if (maxMoveCount > 0 && count >= maxMoveCount) break;

                Instruction insJumpFrom = instructions[i];
                if (!DeobfUtils.IsConditionalJumpInstruction(insJumpFrom))
                    continue;

                // Original:
                // ...
                // br...
                // pop
                // ld...
                // dup          
                // ...

                // Change To:
                // ...
                // br...
                // pop
                // ld...
                // br -> dup (new)
                // dup          
                // ...

                //if (insJumpFrom.OpCode.Name.StartsWith("brtrue"))
                //else if (insJumpFrom.OpCode.Name.StartsWith("brfalse"))
                //else
                //    continue;

                Instruction insPop = insJumpFrom.Next;
                if (insPop == null || insPop.OpCode.Code != Code.Pop)
                    continue;

                Instruction insLd = insPop.Next;
                if (insLd == null || !insLd.OpCode.Name.StartsWith("ld"))
                    continue;

                Instruction insDup = insLd.Next;
                if (insDup == null || insDup.OpCode.Code != Code.Dup)
                    continue;

                Instruction newIns = Instruction.Create(OpCodes.Br, insDup);
                ilp.InsertBefore(insDup, newIns);
                count++;
            }

            if (count > 0)
            {
                InsUtils.ComputeOffsets(instructions);
                //FixExceptionHandler(body.ExceptionHandlers);
            }

            return count;
        }

        public int DeobfFlowConditionalBranchToDirectJump(Instruction insJumpFrom, MethodDefinition method)
        {
            int changed = 0;

            if (insJumpFrom == null) return changed;
            switch (insJumpFrom.OpCode.Code)
            {
                case Code.Brtrue:
                case Code.Brtrue_S:
                case Code.Brfalse:
                case Code.Brfalse_S:
                    break;
                default:
                    return changed;
            }

            List<JumpInstruction> jumpInsList;

            jumpInsList = JumpInstruction.Find(method, -1, -1);

            //ldc.i4 or ldnull
            //stloc
            //ldloc
            //brtrue or brfalse
            List<Instruction> listLd = DeobfUtils.FindPrevInstructions(insJumpFrom, jumpInsList, "ldloc", null);
            if (listLd.Count > 0)
            {
                foreach (Instruction insPrevLdloc in listLd)
                {
                    List<Instruction> listSt = DeobfUtils.FindPrevInstructions(insPrevLdloc, jumpInsList, "stloc", insPrevLdloc.Operand);
                    foreach (Instruction insPrevStloc in listSt)
                    {
                        changed += DeobfFlowConditionalBranchToDirectJump(insJumpFrom, insPrevStloc, jumpInsList);

                    }//end for each stloc
                }//end for each ldloc                
            }
            if (changed > 0) 
                return changed;

            /* 
             problems 
             
            //ldc.i4
            //stloc
            //ldloc
            //ldc.i4
            //clt or cgt
            //brtrue or brfalse
            List<Instruction> listCmp = DeobfUtils.FindPrevInstructions(insJumpFrom, jumpInsList, "clt", null);
            listCmp.AddRange(DeobfUtils.FindPrevInstructions(insJumpFrom, jumpInsList, "cgt", null));
            ILProcessor ilp = method.Body.GetILProcessor();
            foreach (Instruction insCmp in listCmp)
            {
                List<Instruction> listLdci4_2 = DeobfUtils.FindPrevInstructions(insCmp, jumpInsList, "ldc.i4", null);
                foreach (Instruction insLdci4_2 in listLdci4_2)
                {
                    List<Instruction> listLdloc = DeobfUtils.FindPrevInstructions(insLdci4_2, jumpInsList, "ldloc", null);
                    foreach (Instruction insLdloc in listLdloc)
                    {
                        if (DeobfUtils.FindReferences(method, insLdloc, 1, jumpInsList, false).Count > 0)
                            continue;

                        List<Instruction> listStloc = DeobfUtils.FindPrevInstructions(insLdloc, jumpInsList, "stloc", insLdloc.Operand);
                        foreach (Instruction insStloc in listStloc)
                        {
                            if (insStloc.Operand == null && insLdloc.Operand == null && insStloc.OpCode.Name.Substring(2) != insLdloc.OpCode.Name.Substring(2))
                                continue;

                            if (DeobfUtils.FindReferences(method, insStloc, 1, jumpInsList, false).Count > 0)
                                continue;

                            List<Instruction> listLdci4_1 = DeobfUtils.FindPrevInstructions(insStloc, jumpInsList, "ldc.i4", null);
                            foreach (Instruction insLdci4_1 in listLdci4_1)
                            {
                                int op1 = (int)InsUtils.GetInstructionOperand(insLdci4_1);
                                int op2 = (int)InsUtils.GetInstructionOperand(insLdci4_2);

                                bool isJump;
                                if (insCmp.OpCode.Code == Code.Clt)
                                {
                                    if (insJumpFrom.OpCode.Code == Code.Brfalse || insJumpFrom.OpCode.Code == Code.Brfalse_S)
                                        isJump = (op1 >= op2);
                                    else //if (insJumpFrom.OpCode.Code == Code.Brtrue || insJumpFrom.OpCode.Code == Code.Brtrue_S)
                                        isJump = (op1 < op2);
                                }
                                else if (insCmp.OpCode.Code == Code.Cgt)
                                {
                                    if (insJumpFrom.OpCode.Code == Code.Brfalse || insJumpFrom.OpCode.Code == Code.Brfalse_S)
                                        isJump = (op1 <= op2);
                                    else //if (insJumpFrom.OpCode.Code == Code.Brtrue || insJumpFrom.OpCode.Code == Code.Brtrue_S)
                                        isJump = (op1 > op2);
                                }
                                else if (insCmp.OpCode.Code == Code.Clt_Un)
                                {
                                    if (insJumpFrom.OpCode.Code == Code.Brfalse || insJumpFrom.OpCode.Code == Code.Brfalse_S)
                                        isJump = (unchecked((uint)op1) >= unchecked((uint)op2));
                                    else //if (insJumpFrom.OpCode.Code == Code.Brtrue || insJumpFrom.OpCode.Code == Code.Brtrue_S)
                                        isJump = (unchecked((uint)op1) < unchecked((uint)op2));
                                }
                                else if (insCmp.OpCode.Code == Code.Cgt)
                                {
                                    if (insJumpFrom.OpCode.Code == Code.Brfalse || insJumpFrom.OpCode.Code == Code.Brfalse_S)
                                        isJump = (unchecked((uint)op1) <= unchecked((uint)op2));
                                    else //if (insJumpFrom.OpCode.Code == Code.Brtrue || insJumpFrom.OpCode.Code == Code.Brtrue_S)
                                        isJump = (unchecked((uint)op1) > unchecked((uint)op2));
                                }
                                else
                                {
                                    throw new ApplicationException("Unexpected opcode: " + insCmp.OpCode.Code.ToString());
                                }

                                if (isJump)
                                {
                                    Instruction newIns = Instruction.Create(OpCodes.Br, insJumpFrom.Operand as Instruction);
                                    ilp.InsertBefore(insLdci4_1, newIns);
                                    changed++;
                                }
                                else
                                {
                                    if (insJumpFrom.Next != null)
                                    {
                                        Instruction newIns = Instruction.Create(OpCodes.Br, insJumpFrom.Next);
                                        ilp.InsertBefore(insLdci4_1, newIns);
                                        changed++;
                                    }
                                }
                            }
                        }//end for each stloc
                    }                    
                }//end for each ldloc
            }
            */

            return changed;
        }

        public int DeobfFlowConditionalBranchToDirectJump(Instruction insJumpFrom, Instruction insPrevSearch, List<JumpInstruction> jumpInsList)
        {
            //search for ldc.i4
            List<Instruction> listLdcI4 = DeobfUtils.FindPrevInstructions(insPrevSearch, jumpInsList, "ldc.i4", null);
            //search for ldnull
            List<Instruction> listLdnull = DeobfUtils.FindPrevInstructions(insPrevSearch, jumpInsList, "ldcnull", null);

            int changed = 0;

            listLdcI4.AddRange(listLdnull);
            if (listLdcI4.Count < 1)
                return changed;

            foreach (Instruction insPrevLdci4 in listLdcI4)
            {
                Instruction insTmp = insPrevLdci4.Previous;
                if (insTmp != null && DeobfUtils.IsDirectJumpInstruction(insTmp) && IsNearFromOperand(insTmp))
                    continue;

                //start handling
                int op;

                if (insPrevLdci4.OpCode.Name.StartsWith("ldnull"))
                {
                    op = 0;
                }
                else
                {
                    op = (int)InsUtils.GetInstructionOperand(insPrevLdci4);
                    //string opCodeStr = insPrevLdci4.OpCode.Name.Replace(".s", "");
                    //string opStr = opCodeStr.Substring(opCodeStr.Length - 2, 2);
                    //if (opStr == "i4")
                    //    op = Convert.ToInt32(insPrevLdci4.Operand);
                    //else
                    //    op = Convert.ToInt32(opStr.Substring(1, 1));
                }

                if (op == 0)
                {
                    if (insJumpFrom.OpCode.Code == Code.Brfalse || insJumpFrom.OpCode.Code == Code.Brfalse_S)
                    {
                        insPrevLdci4.OpCode = OpCodes.Br;
                        insPrevLdci4.Operand = insJumpFrom.Operand;
                        changed++;
                    }
                    else //if (insJumpFrom.OpCode.Code == Code.Brtrue || insJumpFrom.OpCode.Code == Code.Brtrue_S)
                    {
                        if (insJumpFrom.Next != null)
                        {
                            insPrevLdci4.OpCode = OpCodes.Br;
                            insPrevLdci4.Operand = insJumpFrom.Next;
                            changed++;
                        }
                    }
                }
                else
                {
                    if (insJumpFrom.OpCode.Code == Code.Brtrue || insJumpFrom.OpCode.Code == Code.Brtrue_S)
                    {
                        insPrevLdci4.OpCode = OpCodes.Br;
                        insPrevLdci4.Operand = insJumpFrom.Operand;
                        changed++;
                    }
                    else //if (insJumpFrom.OpCode.Code == Code.Brfalse || insJumpFrom.OpCode.Code == Code.Brfalse_S)
                    {
                        if (insJumpFrom.Next != null)
                        {
                            insPrevLdci4.OpCode = OpCodes.Br;
                            insPrevLdci4.Operand = insJumpFrom.Next;
                            changed++;
                        }
                    }
                }
                //end handling

            }//end for each ldci4 or ldnull                    

            return changed;
        }

        /// <summary>
        /// Should be after DeobfFlowBranch
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public int DeobfFlowConditionalBranch(MethodDefinition method, int brStart, int brEnd, int maxMoveCount)
        {
            int count = 0;
            if (method == null || !method.HasBody || method.Body.Instructions.Count < 3)
                return count;

            //not always good ...
            count += DeobfFlowConditionalBranchToBrTrueFalse(method, brStart, brEnd);
            if (maxMoveCount > 0 && count >= maxMoveCount) 
                return count;

            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            ILProcessor ilp = body.GetILProcessor();

            //bool hasDeobf = false;

            //List<Instruction> jumpInsts = null;
            ////bool canChangLoop = true;
            //if (_options.BranchDirection == BranchDirections.WalkThrough)
            //{
            //    //canChangLoop = false;
            //    jumpInsts = new List<Instruction>();
            //    WalkThroughMethod(method, null, null, jumpInsts);
            //    brStart = 0;
            //    brEnd = jumpInsts.Count - 1;
            //}

            for (int loop = brStart; loop <= brEnd; loop++)
            {
                int i;
                //switch (_options.BranchDirection)
                //{
                //    case BranchDirections.TopDown:
                        i = loop;
                //        break;
                //    case BranchDirections.WalkThrough:
                //        i = instructions.IndexOf(jumpInsts[loop]);
                //        break;
                //    case BranchDirections.BottomUp:
                //        i = brStart + brEnd - loop;
                //        break;
                //    default:
                //        throw new NotSupportedException("Unexpected branch direction: " + Enum.GetName(typeof(BranchDirections), _options.BranchDirection));
                //}

                if (maxMoveCount > 0 && count >= maxMoveCount) break;

                Instruction insJumpFrom = instructions[i];
                if (!DeobfUtils.IsConditionalJumpInstruction(insJumpFrom)) 
                    continue;

                Instruction insJumpTo = insJumpFrom.Operand as Instruction;
                Instruction insJumpToOld = insJumpTo;

                //skip nop
                while (DeobfUtils.IsNopInstruction(insJumpTo))
                {
                    insJumpTo = insJumpTo.Next;
                    insJumpFrom.Operand = insJumpTo;
                    count++;
                }

                //jump to nothing
                if (insJumpTo == null)
                {
                    int insSize = InsUtils.ToNop(ilp, instructions, i) - 1;
                    //if (canChangLoop)
                        brEnd += insSize;
                    count++;
                    continue;
                }

                while (DeobfUtils.IsDirectJumpInstruction(insJumpTo))
                {
                    insJumpTo = insJumpTo.Operand as Instruction;
                    insJumpFrom.Operand = insJumpTo;
                    count++;
                }

                //if (insJumpTo.OpCode.Code == Code.Ret)
                //{
                //    insJumpFrom.OpCode = OpCodes.Ret;
                //    insJumpFrom.Operand = null;
                //    count++;
                //    continue;
                //}

                count += DeobfFlowConditionalBranchToDirectJump(insJumpFrom, method);
                if (maxMoveCount > 0 && count >= maxMoveCount) 
                    return count;

                Instruction insJumpToPrev = insJumpTo.Previous;
                int prevCount = 0;
                while (DeobfUtils.IsNopInstruction(insJumpToPrev))
                {
                    insJumpTo = insJumpToPrev;
                    insJumpToPrev = insJumpToPrev.Previous;
                    prevCount++;
                    if (prevCount > instructions.Count) // dead loop
                    {
                        insJumpToPrev = null;
                        prevCount = -1;
                        break;
                    }
                }
                if (prevCount < 0) 
                    continue;

                //if (!DeobfUtils.IsEqual(insJumpTo, insJumpToOld))
                //{
                //    insJumpFrom.Operand = insJumpTo;
                //    count++;
                //}

                if (insJumpToPrev != null &&
                    !DeobfUtils.IsDirectJumpInstruction(insJumpToPrev) &&
                    !DeobfUtils.IsExitInstruction(insJumpToPrev)
                    )
                {
                    continue;
                }

                int fromIndex = i;
                int startIndex = instructions.IndexOf(insJumpTo);
                int endIndex = -1;
                int endToIndex = -1;
                int fromEndIndex = -1;
                int fromEndToIndex = -1;
                Instruction insStart;
                Instruction insEnd;

                int saveFromIndex = fromIndex;
                int saveStartIndex = startIndex;

                #region conditional jump up
                if (fromIndex >= startIndex && _options.chkCondBranchUpChecked)
                {
                    if (!SearchEnd(method, startIndex, ref endIndex))
                        continue;

                    if (DeobfUtils.IsBranchMoveBreakingExceptionHandler(saveFromIndex, saveStartIndex, method))
                        continue;
                    if (DeobfUtils.IsBranchMoveBreakingExceptionHandler(fromIndex, startIndex, method))
                        continue;

                    insEnd = instructions[endIndex];

                    if (DeobfUtils.IsForeachStatement(insEnd, method, true))
                        continue;

                    // startIndex
                    // ... fromEndToIndex
                    // endIndex
                    // ...
                    // fromIndex
                    // ...fromEndIndex
                    if (SearchEnd(method, fromIndex, ref fromEndIndex))
                    {
                        Instruction insFromEnd = instructions[fromEndIndex];
                        fromEndToIndex = instructions.IndexOf(insFromEnd.Operand == null ? insFromEnd : insFromEnd.Operand as Instruction);
                        if (startIndex <= fromEndToIndex && fromEndToIndex <= endIndex)
                        {
                            FixExceptionHandlerBeforeMoveBlock(method, fromEndIndex, startIndex, endIndex);
                            DeobfUtils.MoveBlock(body, fromEndIndex, startIndex, endIndex);
                            count++;
                            continue;
                        }
                    }


                    // startIndex
                    // endIndex
                    // ...
                    // fromIndex
                    // ...endToIndex
                    if (insEnd.Operand is Instruction)
                    {
                        Instruction insEndTo = insEnd.Operand as Instruction;
                        endToIndex = instructions.IndexOf(insEndTo);
                        if (endToIndex > fromIndex && startIndex < endIndex && endIndex < fromIndex &&
                            !DeobfUtils.IsCrossExceptionHandler(startIndex, endIndex, method)
                            )
                        {
                            FixExceptionHandlerBeforeMoveBlock(method, fromIndex, startIndex, endIndex);
                            SwitchConditionalInstruction(insJumpFrom);
                            insJumpFrom.Operand = insJumpFrom.Next;
                            DeobfUtils.MoveBlock(body, fromIndex, startIndex, endIndex);
                            count++;
                            continue;
                        }
                    }

                    continue;
                }
                #endregion conditional jump up

                #region conditional jump down
                if (fromIndex < startIndex && _options.chkCondBranchDownChecked)
                {
                    if (!SearchJumpToEnd(method, ref fromIndex, ref startIndex, ref endIndex))
                        continue;

                    if (DeobfUtils.IsBranchMoveBreakingExceptionHandler(fromIndex, startIndex, method))
                        continue;

                    insJumpFrom = instructions[fromIndex];
                    insStart = instructions[startIndex];
                    insEnd = instructions[endIndex];

                    if (DeobfUtils.IsForeachStatement(insEnd, method, true))
                        continue;

                    //if (DeobfUtils.IsBranchMoveBreakingExceptionHandler(insStart, insEnd, body.ExceptionHandlers))
                    //    continue;

                    //if (DeobfUtils.IsInMiddleOfExceptionHandler(fromIndex, startIndex, endIndex, method))
                    //{
                    //    FixExceptionHandlerBeforeMoveBlock(method, startIndex, endIndex);
                    //    SwitchConditionalInstruction(insJumpFrom);
                    //    insJumpFrom.Operand = insJumpFrom.Next;
                    //    DeobfUtils.MoveBlock(body, fromIndex, startIndex, endIndex);
                    //    count++;
                    //    continue;
                    //}

                    endToIndex = instructions.IndexOf(insEnd.Operand == null ? insEnd : insEnd.Operand as Instruction);

                    // startIndex
                    // endIndex
                    // ...
                    // fromIndex
                    if (startIndex < endIndex && endIndex < fromIndex &&
                        !DeobfUtils.IsCrossExceptionHandler(startIndex, endIndex, method)
                        )
                    {
                        FixExceptionHandlerBeforeMoveBlock(method, fromIndex, startIndex, endIndex);
                        SwitchConditionalInstruction(insJumpFrom);
                        insJumpFrom.Operand = insJumpFrom.Next;
                        DeobfUtils.MoveBlock(body, fromIndex, startIndex, endIndex);
                        count++;
                        continue;
                    }

                    // fromIndex
                    // ...endToIndex
                    // startIndex
                    // endIndex
                    if (
                        fromIndex < endToIndex && endToIndex < startIndex && startIndex < endIndex &&
                        !DeobfUtils.IsCrossExceptionHandler(startIndex, endIndex, method)
                        )
                    {
                        FixExceptionHandlerBeforeMoveBlock(method, fromIndex, startIndex, endIndex);
                        SwitchConditionalInstruction(insJumpFrom);
                        insJumpFrom.Operand = insJumpFrom.Next;
                        DeobfUtils.MoveBlock(body, fromIndex, startIndex, endIndex);
                        count++;
                        continue;
                    }

                    // fromIndex
                    // startIndex
                    // endIndex
                    // ...endToIndex
                    if (fromIndex < startIndex &&
                        startIndex < endIndex &&
                        endIndex <= endToIndex)
                    {
                        Instruction insJumpFromNext = insJumpFrom.Next;
                        while (insJumpFromNext != null &&
                            insJumpFromNext.Offset < insStart.Offset &&
                            !DeobfUtils.IsDirectJumpInstruction(insJumpFromNext) &&
                            !DeobfUtils.IsExitInstruction(insJumpFromNext)
                            )
                        {
                            insJumpFromNext = insJumpFromNext.Next;
                        }

                        if (insJumpFromNext != null &&
                            insJumpFromNext.Offset < insStart.Offset &&
                            (DeobfUtils.IsDirectJumpInstruction(insJumpFromNext) || DeobfUtils.IsExitInstruction(insJumpFromNext))
                            )
                        {
                            fromEndIndex = instructions.IndexOf(insJumpFromNext);
                            fromEndToIndex = instructions.IndexOf(insJumpFromNext.Operand == null ? insJumpFromNext : insJumpFromNext.Operand as Instruction);

                            // fromIndex
                            // fromEndIndex
                            // ...others
                            // startIndex 
                            // ...fromEndToIndex
                            // endIndex
                            // ...fromEndToIndex
                            // ...endToIndex
                            if (fromIndex < fromEndIndex - 1 &&
                                fromEndIndex < startIndex - 1 &&
                                startIndex < fromEndToIndex &&
                                endIndex != fromEndToIndex &&
                                fromEndToIndex < endToIndex &&
                                //startIndex - fromEndIndex > 10 &&  //maybe ...
                                !DeobfUtils.IsCrossExceptionHandler(startIndex, endIndex, method)
                                )
                            {
                                bool canMove = true;
                                for (int j = fromEndIndex + 1; j < startIndex; j++)
                                {
                                    Instruction tmp = instructions[j];
                                    if (tmp.Operand is Instruction)
                                    {
                                        Instruction tmpTo = tmp.Operand as Instruction;
                                        int tmpToIndex = instructions.IndexOf(tmpTo);
                                        if (tmpToIndex >= startIndex && tmpToIndex <= endIndex)
                                        {
                                            canMove = false;
                                            break;
                                        }
                                    }
                                }
                                if (canMove)
                                {
                                    FixExceptionHandlerBeforeMoveBlock(method, fromEndIndex, startIndex, endIndex);
                                    DeobfUtils.MoveBlock(body, fromEndIndex, startIndex, endIndex);
                                    count++;
                                }
                                continue;
                            }

                            // fromIndex
                            // fromEndIndex
                            // ...fromEndToIndex
                            // startIndex 
                            // endIndex
                            // ...endToIndex
                            // ...fromEndToIndex

                            //if (fromIndex < fromEndIndex - 1 &&
                            //    fromEndIndex < startIndex - 1 &&
                            //    startIndex < endIndex &&
                            //    endIndex <= endToIndex &&
                            //    ( fromEndToIndex > endIndex || fromEndToIndex < startIndex)
                            //    )
                            //{
                            //    bool canMove = true;
                            //    for (int j = fromIndex + 1; j < startIndex; j++)
                            //    {
                            //        Instruction tmp = instructions[j];
                            //        if (tmp.Operand is Instruction)
                            //        {
                            //            Instruction tmpTo = tmp.Operand as Instruction;
                            //            int tmpToIndex = instructions.IndexOf(tmpTo);
                            //            if (tmpToIndex >= startIndex && tmpToIndex <= endIndex)
                            //            {
                            //                canMove = false;
                            //                break;
                            //            }
                            //        }
                            //    }
                            //    if (canMove)
                            //    {
                            //        FixExceptionHandlerBeforeMoveBlock(method, startIndex, endIndex);
                            //        SwitchConditionalInstruction(insJumpFrom);
                            //        insJumpFrom.Operand = insJumpFrom.Next;
                            //        DeobfUtils.MoveBlock(body, fromIndex, startIndex, endIndex);
                            //        count++;
                            //        continue;
                            //    }
                            //    continue;
                            //}

                        }
                    }
                }
                #endregion conditional jump down

                continue;

            } //end main loop

            if (count > 0)
            {
                InsUtils.Simplify(instructions);
                FixExceptionHandler(body.ExceptionHandlers);
            }

            return count;
        }

        /*
        public int DeobfFlowBlockCopy(MethodDefinition method, int maxMoveCount)
        {
            int count = 0;
            if (method == null || !method.HasBody || method.Body.Instructions.Count < 3)
                return count;

            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;

            int brStart = 0;
            int brEnd = instructions.Count - 1;
            for (int i = brStart; i <= brEnd; i++)
            {
                if (maxMoveCount > 0 && count >= maxMoveCount) break;

                if (!DeobfUtils.IsConditionalJumpInstruction(instructions[i])) continue;

                Instruction insJumpFrom = instructions[i];
                count += DeobfFlowBlockCopy(insJumpFrom, method);
            }

            return count;
        }

        public int DeobfFlowBlockCopy(Instruction insJumpFrom, MethodDefinition method)
        {
            int count = 0;

            Instruction insJumpTo = insJumpFrom.Operand as Instruction;
            if (insJumpTo == null) 
                return count;

            Mono.Cecil.Cil.MethodBody body = method.Body;
            Collection<Instruction> instructions = body.Instructions;
            ILProcessor ilp = body.GetILProcessor();

            int fromIndex = instructions.IndexOf(insJumpFrom);
            int startIndex = instructions.IndexOf(insJumpTo);
            if (fromIndex >= startIndex - 1)
                return count;

            Instruction insJumpToEnd = insJumpTo.Next;
            while (insJumpToEnd != null && 
                !DeobfUtils.IsConditionalJumpInstruction(insJumpToEnd)  && 
                !DeobfUtils.IsDirectJumpInstruction(insJumpToEnd)
                )
            {
                if (DeobfUtils.IsExitInstruction(insJumpToEnd))
                    return count;
                insJumpToEnd = insJumpToEnd.Next;
            }
            if (insJumpToEnd == null)
                return count;

            int endIndex = instructions.IndexOf(insJumpToEnd);
            Instruction insEndTo = insJumpToEnd.Operand as Instruction;
            int endToIndex = instructions.IndexOf(insEndTo);

            if (endToIndex != fromIndex + 1)
                return count;

            DeobfUtils.CopyBlock(body, fromIndex, startIndex, endIndex);
            insJumpFrom.Operand = insJumpFrom.Next;

            count++;
            return count;

        }
        */

        public void SwitchConditionalInstruction(Instruction brCond)
        {
            switch (brCond.OpCode.Code)
            {
                case Code.Brfalse:
                case Code.Brfalse_S:
                    brCond.OpCode = OpCodes.Brtrue;
                    break;
                case Code.Brtrue:
                case Code.Brtrue_S:
                    brCond.OpCode = OpCodes.Brfalse;
                    break;

                case Code.Ble:
                case Code.Ble_S:
                    brCond.OpCode = OpCodes.Bgt;
                    break;
                case Code.Bgt:
                case Code.Bgt_S:
                    brCond.OpCode = OpCodes.Ble;
                    break;

                case Code.Bge:
                case Code.Bge_S:
                    brCond.OpCode = OpCodes.Blt;
                    break;
                case Code.Blt:
                case Code.Blt_S:
                    brCond.OpCode = OpCodes.Bge;
                    break;

                case Code.Bge_Un:
                case Code.Bge_Un_S:
                    brCond.OpCode = OpCodes.Blt_Un;
                    break;
                case Code.Blt_Un:
                case Code.Blt_Un_S:
                    brCond.OpCode = OpCodes.Bge_Un;
                    break;
                case Code.Beq:
                case Code.Beq_S:
                    brCond.OpCode = OpCodes.Bne_Un;
                    break;
                case Code.Bne_Un:
                case Code.Bne_Un_S:
                    brCond.OpCode = OpCodes.Beq;
                    break;
            }
        }

        private bool ExtendStartEndForExceptionHandler(MethodDefinition method, ref int fromIndex, ref int startIndex, ref int endIndex)
        {
            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
            Collection<Instruction> instructions = method.Body.Instructions;
            int lastNewEndIndex = -1;

            for (int i = 0; i < ehc.Count; i++)
            {
                ExceptionHandler eh = ehc[i];
                if (eh.TryStart != null && eh.TryEnd != null &&
                    eh.FilterStart == null && //eh.FilterEnd == null &&
                    eh.HandlerStart != null && eh.HandlerEnd != null &&
                    instructions[startIndex].Offset == eh.TryStart.Offset &&
                    eh.TryEnd.Previous != null &&
                    instructions[endIndex].Offset == eh.TryEnd.Previous.Offset &&
                    eh.HandlerStart.Offset >= eh.TryEnd.Offset)
                {
                    Instruction newEndIns;
                    switch (eh.HandlerType)
                    {
                        case ExceptionHandlerType.Finally:
                        //newEndIns = eh.HandlerEnd;
                        //break;
                        case ExceptionHandlerType.Catch:
                            newEndIns = eh.HandlerEnd.Next == null ? eh.HandlerEnd : eh.HandlerEnd.Previous;
                            break;
                        default:
                            continue;
                    }
                    int newEndIndex = instructions.IndexOf(newEndIns);

                    if (newEndIndex > endIndex && newEndIndex > startIndex && newEndIndex > lastNewEndIndex)
                    {
                        if (fromIndex < startIndex || fromIndex > newEndIndex)
                        {
                            lastNewEndIndex = newEndIndex;
                        }
                    }
                }
            }
            if (lastNewEndIndex >= 0)
            {
                endIndex = lastNewEndIndex;
                return true;
            }
            return false;
        }

        private bool SearchEnd(MethodDefinition method, int startIndex, ref int endIndex)
        {
            Collection<Instruction> instructions = method.Body.Instructions;
            Instruction insStart = instructions[startIndex];
            
            Instruction insEnd = insStart.Next;
            while (insEnd != null &&
                !DeobfUtils.IsDirectJumpInstruction(insEnd) && 
                !DeobfUtils.IsExitInstruction(insEnd) 
                )
            {
                insEnd = insEnd.Next;
            }
            if (insEnd != null)
            {
                endIndex = instructions.IndexOf(insEnd);
                return true;
            }
            return false;
        }

        private bool SearchJumpToEnd(MethodDefinition method, ref int fromIndex, ref int startIndex, ref int endIndex)
        {
            bool found = false;
            Instruction insLast;
            Collection<Instruction> instructions = method.Body.Instructions;
            Instruction insJumpFrom = instructions[fromIndex];
            Instruction insJumpTo = instructions[startIndex];

            if (startIndex < fromIndex) // jump up
            {
                insLast = insJumpFrom.Previous;
                while (insLast != null)
                {
                    //count++;
                    if (DeobfUtils.IsExitInstruction(insLast))
                    {
                        found = true;
                        break;
                    }

                    if (DeobfUtils.IsDirectJumpInstruction(insLast))
                    {
                        found = true;
                        break;
                    }
                    insLast = insLast.Previous;
                }

                if (found)
                {
                    //int insLastJumpToIndex = -1;
                    //if (DeobfUtils.IsDirectJumpInstruction(insLast))
                    //{
                    //    insLastJumpToIndex = instructions.IndexOf((Instruction)insLast.Operand);
                    //}

                    insLast = insLast.Next;
                    startIndex = instructions.IndexOf(insLast);
                    if (startIndex >= fromIndex) return false;

                    endIndex = fromIndex;
                    fromIndex = instructions.IndexOf(insJumpTo.Previous);

                    //if (startIndex < insLastJumpToIndex && insLastJumpToIndex < endIndex) return false;
                    if (fromIndex < 0) return false;
                    if (fromIndex + 1 >= startIndex || startIndex > endIndex) return false;

                    return true;
                }
            }
            else // jump down
            {
                insLast = insJumpTo;
                while (insLast != null)
                {
                    if (DeobfUtils.IsExitInstruction(insLast))
                    {
                        found = true;
                        break;
                    }
                    if (DeobfUtils.IsDirectJumpInstruction(insLast))
                    {
                        found = true;
                        break;
                    }
                    insLast = insLast.Next;
                }

                if (found)
                {
                    //int insLastJumpToIndex = -1;
                    //if (DeobfUtils.IsDirectJumpInstruction(insLast))
                    //{
                    //    insLastJumpToIndex = instructions.IndexOf((Instruction)insLast.Operand);
                    //}

                    endIndex = instructions.IndexOf(insLast);

                    //if (startIndex < insLastJumpToIndex && insLastJumpToIndex < endIndex) return false;
                    if (fromIndex + 1 >= startIndex || startIndex > endIndex) return false;

                    return true;
                }//end of found

            } // end of jump up/down
            return false;

            /* old codes
            if (startIndex < fromIndex) // jump up
            {
                insLast = insJumpFrom.Previous;
                while (insLast != null)
                {
                    //count++;
                    if (IsExitInstruction(insLast))
                    {
                        found = true;
                        break;
                    }

                    if (IsDirectJumpInstruction(insLast))
                    {
                        found = true;
                        break;
                    }
                    insLast = insLast.Previous;
                }

                if (found)
                {
                    insLast = insLast.Next;

                    startIndex = instructions.IndexOf(insLast);
                    if (startIndex >= fromIndex) continue;

                    endIndex = fromIndex;
                    fromIndex = instructions.IndexOf(insJumpTo.Previous);

                    if (fromIndex < 0) continue;
                    if (fromIndex + 1 >= startIndex || startIndex > endIndex) continue;

                    //if (IsExitInstruction(insJumpTo.Next))
                    //{
                    //    i++;
                    //}

                    int insSize = InsUtils.ToNop(ilp, instructions, endIndex);
                    brEnd += insSize - 1;

                    MoveBlocks(instructions, fromIndex, startIndex, endIndex);
                    moveCount++;

                }
            }
            else // jump down
            {
                insLast = insJumpTo;
                while (insLast != null)
                {
                    //count++;
                    if (IsExitInstruction(insLast))
                    {
                        found = true;
                        break;
                    }
                    if (IsDirectJumpInstruction(insLast))
                    {
                        found = true;
                        break;
                    }
                    insLast = insLast.Next;
                }

                if (found)
                {
                    endIndex = instructions.IndexOf(insLast);

                    if (fromIndex + 1 >= startIndex || startIndex > endIndex) continue;

                    MoveBlocks(instructions, fromIndex, startIndex, endIndex);
                    moveCount++;

                    if (IsExitInstruction(insJumpFrom.Next))
                    {
                        i++;
                    }

                    int insSize = InsUtils.ToNop(ilp, instructions, fromIndex);
                    brEnd += insSize - 1;

                }//end of found

            } // end of jump up/down
           */

        }

        /// <summary>
        /// Require ComputeOffset 
        /// </summary>
        /// <param name="ehc"></param>
        private void FixExceptionHandler(Collection<ExceptionHandler> ehc)
        {
            foreach (ExceptionHandler eh in ehc)
            {
                eh.TryStart = FixExceptionHandlerStart(eh.TryStart, eh.TryEnd);
                eh.HandlerStart = FixExceptionHandlerStart(eh.HandlerStart, eh.HandlerEnd);
                eh.FilterStart = FixExceptionHandlerStart(eh.FilterStart, eh.HandlerStart);

                eh.TryEnd = FixExceptionHandlerEnd(eh.TryStart, eh.TryEnd);
                eh.HandlerEnd = FixExceptionHandlerEnd(eh.HandlerStart, eh.HandlerEnd);
                //eh.FilterEnd = FixExceptionHandlerEnd(eh.FilterStart, eh.FilterEnd);

                //TODO
                //if (!DeobfUtils.IsValidExceptionHandler(eh))
                //    throw new ApplicationException("FixExceptionHandler: internal error");
            }
        }

        private Instruction FixExceptionHandlerStart(Instruction start, Instruction end)
        {
            if (start == null || end == null)
                return start;

            Instruction ins = start;
            while (DeobfUtils.IsNopInstruction(ins) && ins.Offset < end.Offset)
            {
                ins = ins.Next;
            }
            while (DeobfUtils.IsDirectJumpInstruction(ins))
            {
                ins = ins.Operand as Instruction;
            }
            if (ins == null || ins.Offset >= end.Offset || ins.Offset >= start.Offset)
                return start;
            return ins;
        }

        private Instruction FixExceptionHandlerEnd(Instruction start, Instruction end)
        {
            if (start == null || end == null || start.Offset <= end.Offset)
                return end;

            Instruction ins = end;
            while (DeobfUtils.IsNopInstruction(ins) && ins.Offset > start.Offset)
            {
                ins = ins.Previous;
            }
            if (ins == null || DeobfUtils.IsEqual(ins, end) || ins.Offset <= start.Offset)
            {
                ins = start;
                while (ins != null
                    && ins.Offset >= start.Offset
                    && !DeobfUtils.IsExitInstruction(ins)
                    && !DeobfUtils.IsDirectJumpInstruction(ins)
                    )
                {
                    ins = ins.Next;
                }
                if (ins == null
                    || ins.Offset <= start.Offset
                    || !DeobfUtils.IsExitInstruction(ins)
                    //|| !IsDirectJumpInstruction(ins)
                    )
                    return end;
            }
            return ins.Next == null ? ins : ins.Next;
        }

        public void FixExceptionHandlerBeforeMoveBlock(MethodDefinition method, int fromIndex, int startIndex, int endIndex)
        {
            Collection<Instruction> ic = method.Body.Instructions;
            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
            //Instruction insFrom = ic[fromIndex];
            Instruction insStart = ic[startIndex];
            Instruction insEnd = ic[endIndex];
            
            foreach (ExceptionHandler eh in ehc)
            {
                //Instruction saveTryStart = eh.TryStart;
                //Instruction saveTryEnd = eh.TryEnd;
                //Instruction saveHandlerStart = eh.HandlerStart;
                //Instruction saveHandlerEnd = eh.HandlerEnd;

                if (DeobfUtils.IsEqual(insStart, eh.HandlerEnd))
                {
                    if (insEnd.Next != null)
                        eh.HandlerEnd = insEnd.Next;
                    else
                        eh.HandlerEnd = eh.HandlerEnd.Previous;
                }
                else if (DeobfUtils.IsEqual(insStart, eh.TryEnd))
                {
                    if (insEnd.Next != null)
                    {
                        eh.TryEnd = insEnd.Next;
                        if (eh.HandlerStart != null && eh.HandlerStart.Offset < eh.TryEnd.Offset)
                        {
                            eh.HandlerStart = eh.TryEnd;
                        }
                    }
                    else
                    {
                        eh.TryEnd = eh.TryEnd.Previous;
                    }

                }
                //else if (DeobfUtils.IsEqual(insFrom.Next, eh.HandlerEnd))
                //{
                //    eh.HandlerEnd = insStart;
                //}

                //TODO
                //if (!DeobfUtils.IsValidExceptionHandler(eh))
                //    throw new ApplicationException("FixExceptionHandlerBeforeMoveBlock: internal error");

            }
        }

        //public bool IsExceptionHandlerEnd(Instruction ins, ExceptionHandler eh)
        //{
        //    if (IsEqual(ins, eh.TryEnd) ||
        //        //IsEqual(ins, eh.FilterEnd) ||
        //        IsEqual(ins, eh.HandlerEnd)
        //        )
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        private bool IsInstructionMatch(Instruction ins, OpCodeGroup opCodeGroup, Collection<Instruction> instructions)
        {
            bool match = false;
            string name = ins.OpCode.Name;
            string[] opCodes = opCodeGroup.OpCodes;
            for (int i = 0; i < opCodes.Length; i++)
            {
                if (opCodeGroup.ExactMatch)
                {
                    if (name.Equals(opCodes[i]))
                    {
                        match = true;
                        break;
                    }
                }
                else
                {
                    if (name.StartsWith(opCodes[i]))
                    {
                        match = true;
                        break;
                    }
                }
            }
            if (match && opCodeGroup.HasExpression)
            {
                if (!opCodeGroup.EvalExpression(ins, instructions))
                {
                    match = false;
                }
            }
            return match;
        }

        protected int SearchInstructions(Collection<Instruction> instructions, Pattern searchFor, int start, List<int> matched)
        {
            int searchLength = instructions.Count - searchFor.MinLength;
            int foundIndex = -1;
            for (int i = start; i < searchLength; i++)
            {
                if (!IsInstructionMatch(instructions[i], searchFor.OpCodeGroups[0], instructions)) continue;

                bool found = true;
                matched.Clear();
                matched.Add(i);

                for (int j = 1, k = 1; j < searchFor.MaxLength; j++)
                {
                    //skip nop
                    while (i + k < searchLength && DeobfUtils.IsNopInstruction(instructions[i + k]))
                    {
                        k++;
                    }
                    if (i >= searchLength) break;

                    if (IsInstructionMatch(instructions[i + k], searchFor.OpCodeGroups[j], instructions))
                    {
                        matched.Add(i + k);
                        k++;
                    }
                    else
                    {
                        if (searchFor.OpCodeGroups[j].Optional)
                        {
                            // k not move
                            matched.Add(-1);
                        }
                        else
                        {
                            found = false;
                            break;
                        }
                    }
                }

                if (found)
                {
                    foundIndex = i;
                    if (matched.Count != searchFor.MaxLength)
                    {
                        throw new ApplicationException("Wrong matched pattern: " + searchFor.Text);
                    }
                    break;
                }
            }
            return foundIndex;
        }

        public void RemoveAttribute(Mono.Cecil.ICustomAttributeProvider cap)
        {
            if (!IsDeobfRemoveAttribute())
                return;

            for (int i = 0; i < cap.CustomAttributes.Count; i++)
            {
                if (_options.IsCancelPending) break;

                if (cap.CustomAttributes[i].Constructor == null) continue;

                TypeReference tr = cap.CustomAttributes[i].Constructor.DeclaringType;
                string attrName = InsUtils.GetOldMemberName(tr);
                TypeDefinition td = Resolve(tr);
                
                AttributeFile af = _options.AttributeFile;
                bool needToRemove = false;
                if (cap is AssemblyDefinition)
                {
                    if (td == null)
                    {
                        needToRemove = (af.IsMatchAssemblyLevel(attrName) || af.IsMatchAllLevel(attrName));
                    }
                    else
                    {
                        needToRemove = (af.IsMatchAssemblyLevel(td) || af.IsMatchAllLevel(td));
                    }                   
                }
                else if (cap is TypeDefinition)
                {
                    if (td == null)
                    {
                        needToRemove = (af.IsMatchClassLevel(attrName) || af.IsMatchAllLevel(attrName));
                    }
                    else
                    {
                        needToRemove = (af.IsMatchClassLevel(td) || af.IsMatchAllLevel(td));
                    }     
                }
                else
                {
                    if (td == null)
                    {
                        needToRemove = af.IsMatchAllLevel(attrName);
                    }
                    else
                    {
                        needToRemove = af.IsMatchAllLevel(td);
                    }
                }

                if (needToRemove)
                {
                    cap.CustomAttributes.RemoveAt(i);
                    i--;
                }
            }
        }

        private bool IsMatchedPropertyMethod(MethodDefinition get, MethodDefinition set)
        {
            if (get == null || set == null)
                return false;
            if (!get.Name.StartsWith("get_") || !set.Name.StartsWith("set_"))
                return false;
            if (get.Name.Substring(4) != set.Name.Substring(4))
                return false;
            if(get.Parameters.Count != set.Parameters.Count - 1)
                return false;
            if (!DeobfUtils.IsSameType(get.ReturnType, set.Parameters[set.Parameters.Count - 1].ParameterType))
                return false;
            for(int i=0; i<get.Parameters.Count;i++) {
                if (!DeobfUtils.IsSameType(get.Parameters[i].ParameterType, set.Parameters[i].ParameterType))
                    return false;
            }
            return true;
        }

        public void AddMissingProperty(TypeDefinition type)
        {
            if (!_options.chkAddMissingPropertyAndEventChecked)
                return;

            MethodDefinition get;
            MethodDefinition set;
            string propertyName;

            for (int i = 0; i < type.Methods.Count; i++)
            {
                propertyName = null;
                get = null;
                set = null;

                MethodDefinition md = type.Methods[i];
                if (md.Name.StartsWith("set_"))
                {
                    if (md.Parameters.Count < 1)
                        continue;
                    set = md;
                    propertyName = md.Name.Substring(4);
                    for (int j = i + 1; j < type.Methods.Count; j++)
                    {
                        md = type.Methods[j];
                        if (IsMatchedPropertyMethod(md, set))
                        {
                            get = md;
                            break;
                        }
                    }
                }
                else if (md.Name.StartsWith("get_"))
                {
                    get = md;
                    propertyName = md.Name.Substring(4);
                    for (int j = i + 1; j < type.Methods.Count; j++)
                    {
                        md = type.Methods[j];
                        if (IsMatchedPropertyMethod(get, md))
                        {
                            set = md;
                            break;
                        }
                    }
                }

                if (String.IsNullOrEmpty(propertyName)) 
                    continue;            

                TypeReference propertyType = null;
                if (get != null) propertyType = get.ReturnType;
                else if (set != null) propertyType = set.Parameters[0].ParameterType;
                
                int parameterCount = (get!=null)?get.Parameters.Count:set.Parameters.Count-1;
                PropertyDefinition property = null;

                foreach (PropertyDefinition pd in type.Properties)
                {
                    if (pd.Name == propertyName && pd.Parameters.Count == parameterCount)
                    {
                        if (pd.SetMethod != null && set != null)
                        {
                            if (DeobfUtils.IsSameMethodExact(pd.SetMethod, set))
                            {
                                property = pd;
                                break;
                            }
                        }
                        else if (pd.GetMethod != null && get != null)
                        {
                            if (DeobfUtils.IsSameMethodExact(pd.GetMethod, get))
                            {
                                property = pd;
                                break;
                            }
                        }
                        else if(pd.SetMethod != null & get != null)
                        {
                            if (IsMatchedPropertyMethod(get, pd.SetMethod))
                            {
                                property = pd;
                                break;
                            }
                        }
                        else if (pd.GetMethod != null & set != null)
                        {
                            if (IsMatchedPropertyMethod(pd.GetMethod, set))
                            {
                                property = pd;
                                break;
                            }
                        }
                        if (property != null)
                            break;
                    }
                }

                if (set != null)
                {
                    if (set.Parameters.Count == 1)
                    {
                        set.Parameters[0].Name = "value";
                    }
                }

                if (property != null)
                {
                    if (property.SetMethod == null && set != null)
                        property.SetMethod = set;
                    if (property.GetMethod == null && get != null)
                        property.GetMethod = get;
                }
                else
                {
                    property = new PropertyDefinition(propertyName,
                        Mono.Cecil.PropertyAttributes.None,
                        propertyType);
                    property.SetMethod = set;
                    property.GetMethod = get;                    
                    type.Properties.Add(property);
                }
                
            }
        }

        
        public void AddMissingEvent(TypeDefinition type)
        {
            if (!_options.chkAddMissingPropertyAndEventChecked || type.IsInterface)
                return;

            MethodDefinition addMethod;
            MethodDefinition removeMethod;
            MethodDefinition invokeMethod;
            string fieldName;

            for (int i = 0; i < type.Methods.Count; i++)
            {
                fieldName = null;
                addMethod = null;
                removeMethod = null;
                invokeMethod = null;

                MethodDefinition md = type.Methods[i];
                if (md.Name.StartsWith("add_"))
                {
                    if (md.Parameters.Count < 1)
                        continue;
                    addMethod = md;
                    fieldName = md.Name.Substring(4);

                    string removeMethodName = "remove_" + fieldName;
                    string invokeMethodName = "invoke_" + fieldName;
                    for (int j = 0; j < type.Methods.Count; j++)
                    {
                        MethodDefinition tmp = type.Methods[j];
                        if (tmp.Name == removeMethodName)
                        {
                            if (tmp.Parameters.Count == md.Parameters.Count &&
                            DeobfUtils.IsSameType(tmp.Parameters[0].ParameterType, md.Parameters[0].ParameterType))
                                removeMethod = tmp;
                        }
                        else if (tmp.Name == invokeMethodName)
                        {
                            if (tmp.Parameters.Count == md.Parameters.Count &&
                            DeobfUtils.IsSameType(tmp.Parameters[0].ParameterType, md.Parameters[0].ParameterType))
                                invokeMethod = tmp;
                        }
                    }
                }

                if (String.IsNullOrEmpty(fieldName) || addMethod == null || removeMethod == null)
                    continue;

                TypeReference fieldType = addMethod.Parameters[0].ParameterType;
                if (fieldType.FullName != "System.EventHandler")
                {
                    TypeDefinition fd = Resolve(fieldType);
                    if (fd == null) continue;
                    if (fd.BaseType.FullName != "System.MulticastDelegate")
                        continue;
                }

                EventDefinition eventDef = null;
                foreach (EventDefinition ed in type.Events)
                {
                    if (ed.Name == fieldName)
                    {
                        eventDef = ed;
                        break;
                    }
                }

                if (eventDef == null)
                {
                    eventDef = new EventDefinition(fieldName, Mono.Cecil.EventAttributes.None, fieldType);                                        
                    type.Events.Add(eventDef);
                }

                if (eventDef.AddMethod == null && addMethod != null)
                {
                    addMethod.Parameters[0].Name = "value";                    
                    eventDef.AddMethod = addMethod;
                }
                if (eventDef.RemoveMethod == null && removeMethod != null)
                {
                    removeMethod.Parameters[0].Name = "value";
                    eventDef.RemoveMethod = removeMethod;
                }
                if (eventDef.InvokeMethod == null && invokeMethod != null)
                {
                    eventDef.InvokeMethod = invokeMethod;
                }

                FixEventFieldName(addMethod, fieldType, fieldName);
            }
        }
        

        private TypeDefinition GetTypeDefinition(TypeReference tr)
        {
            return DeobfUtils.GetTypeDefinition(tr, allAssemblies);
        }

        private bool IsBaseType(TypeDefinition baseType, TypeDefinition inheritType)
        {
            return DeobfUtils.IsBaseType(baseType, inheritType, int.MaxValue, allAssemblies);
        }

        private TypeDefinition Resolve(TypeReference tr)
        {
            return DeobfUtils.Resolve(tr, allAssemblies.Values, this.Options);
        }
        private MethodDefinition Resolve(MethodReference mr)
        {
            return DeobfUtils.Resolve(mr, allAssemblies.Values, this.Options);
        }
        private FieldDefinition Resolve(FieldReference fr)
        {
            return DeobfUtils.Resolve(fr, allAssemblies.Values, this.Options);
        }

        public TypeDefinition Resolve(Type type)
        {
            Assembly a = type.Module.Assembly;
            AssemblyDefinition ad = DeobfUtils.ResolveAssembly(a.FullName, allAssemblies.Values);
            ModuleDefinition module = null;
            if (ad != null)
            {
                //bool isSystemType = Utils.IsSystemType(type.FullName);
                //if (isSystemType)
                //{
                //    string moduleFileName = Path.GetFileName(type.Module.FullyQualifiedName);
                //    foreach (ModuleDefinition md in ad.Modules)
                //    {
                //        if (Path.GetFileName(md.FullyQualifiedName) == moduleFileName)
                //        {
                //            module = md;
                //            break;
                //        }
                //    }
                //}
                //else
                {
                    foreach (ModuleDefinition md in ad.Modules)
                    {
                        if (md.FullyQualifiedName == type.Module.FullyQualifiedName)
                        {
                            module = md;
                            break;
                        }
                    }
                }
            }
            else
            {
                //module = ModuleDefinition.ReadModule(type.Module.FullyQualifiedName);
            }
            if(module != null)
                return (TypeDefinition)module.LookupToken(type.MetadataToken);
            return null;
        }

        public MethodDefinition Resolve(MethodBase method)
        {
            var declaring_type = Resolve(method.DeclaringType);
            if(declaring_type != null)
                return (MethodDefinition)declaring_type.Module.LookupToken(method.MetadataToken);
            return null;
        }

        #region CallPlugin
        public void InitPlugin()
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.Deobfuscator = this;
                }
            }
        }

        private void CallPluginBeforeHandler(AssemblyDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.BeforeHandleAssembly(o);
                }
            }
        }

        private void CallPluginAfterHandler(AssemblyDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.AfterHandleAssembly(o);
                }
            }
        }

        private void CallPluginBeforeHandler(ModuleDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.BeforeHandleModule(o);
                }
            }
        }

        private void CallPluginAfterHandler(ModuleDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.AfterHandleModule(o);
                }
            }
        }

        private void CallPluginBeforeHandler(TypeDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.BeforeHandleType(o);
                }
            }
        }

        private void CallPluginAfterHandler(TypeDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.AfterHandleType(o);
                }
            }
        }

        private void CallPluginBeforeHandler(PropertyDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.BeforeHandleProperty(o);
                }
            }
        }

        private void CallPluginAfterHandler(PropertyDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.AfterHandleProperty(o);
                }
            }
        }

        private void CallPluginBeforeHandler(FieldDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.BeforeHandleField(o);
                }
            }
        }

        private void CallPluginAfterHandler(FieldDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.AfterHandleField(o);
                }
            }
        }

        private void CallPluginBeforeHandler(EventDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.BeforeHandleEvent(o);
                }
            }
        }

        private void CallPluginAfterHandler(EventDefinition o)
        {
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    plugin.AfterHandleEvent(o);
                }
            }
        }

        public int CallPluginBeforeHandler(MethodDefinition o)
        {
            int count = 0;
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    count += plugin.BeforeHandleMethod(o);
                }
            }
            return count;
        }

        public int CallPluginAfterHandler(MethodDefinition o)
        {
            int count = 0;
            if (_options.HasPlugin)
            {
                foreach (IDeobfPlugin plugin in _options.PluginList)
                {
                    count += plugin.AfterHandleMethod(o);
                }
            }
            return count;
        }
        #endregion CallPlugin

    } //end of class
}