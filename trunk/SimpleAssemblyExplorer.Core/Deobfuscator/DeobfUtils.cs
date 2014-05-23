using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data;
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
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public class DeobfUtils
    {
        static Regex regexAscii = new Regex(@"^[a-zA-Z_]{1}[0-9a-zA-Z_$.]*$");
        static Regex regexHandledName = new Regex(@"^((p)|(e)|(f)|(m)|(enum)|(struct)|(delegate)|(i)|(c)|(attr)){1}0[0-9a-f]{1,5}$");

        public static bool IsValidName(string name, DeobfOptions options)
        {
            if (name == "<Module>")
                return true;
            if (regexHandledName.IsMatch(name))
                return true;

            if (options.chkNonAsciiChecked)
            {
                if (!regexAscii.IsMatch(name))
                {
                    return false;
                }
            }

            //if (name.Length < 3) return false; // e.g. PageSize.A4

            if (options.chkRandomNameChecked)
            {
                string[] splittedNames = name.Split('.');
                foreach (string splittedName in splittedNames)
                {
                    if (options.RandomFile.IsRandomName(splittedName))
                        return false;
                }
            }

            if (options.chkRegexChecked)
            {
                if (!String.IsNullOrEmpty(options.txtRegexText))
                {
                    if (Regex.IsMatch(name, options.txtRegexText))
                        return false;
                }

                if (options.RegexFile.IsRandomName(name))
                {
                    return false;
                }
            }

            return true;
        }

        public static void CopyBlock(Mono.Cecil.Cil.MethodBody body, int fromIndex, int startIndex, int endIndex)
        {
            //copy instructions startIndex to endIndex after fromIndex

            if (fromIndex >= startIndex - 1 && fromIndex <= endIndex) return;

            Collection<Instruction> instructions = body.Instructions;            
            Type icType = typeof(Collection<Instruction>);
            Instruction[] alIns = (Instruction[])icType.InvokeMember("items",
                 BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance,
                 null, instructions, null);

            int copyCount = endIndex - startIndex + 1;
            Instruction[] al = new Instruction[instructions.Count + copyCount];
            
            int curIndex = 0;
            //0 to fromIndex
            int count = fromIndex + 1;
            Array.Copy(alIns, 0, al, curIndex, count);
            curIndex += count;
            
            //startIndex to endIndex
            count = copyCount;
            ILProcessor ilp = body.GetILProcessor();
            for (int i = 0, j = curIndex; i < count; i++, j++)
            {
                Instruction ins = instructions[i + startIndex];
                al[j] = InsUtils.CreateInstruction(ins.OpCode, ins.Operand);
            }
            curIndex += count;

            //fromIndex+1 to end
            count = instructions.Count - fromIndex - 1;
            Array.Copy(alIns, fromIndex + 1, al, curIndex, count);
            curIndex += count;

            if (instructions.Count + copyCount != curIndex)
            {
                throw new ApplicationException("Internal CopyBlock error!");
            }

            al[fromIndex].Next = al[fromIndex + 1];
            int index;
            for (int i = 0; i < copyCount; i++)
            {
                index = i + fromIndex + 1;
                al[index].Previous = al[index - 1];
                al[index].Next = (index < al.Length - 1 ? al[index + 1] : null);
            }
            index = fromIndex + copyCount + 1;
            if (index < al.Length)
            {
                al[index].Previous = al[index - 1];
            }

            alIns = null;
            while (instructions.Count < al.Length)
            {
                instructions.Add(InsUtils.CreateInstruction(OpCodes.Nop, null));
            }
            alIns = (Instruction[])icType.InvokeMember("items",
                 BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance,
                 null, instructions, null);           
            Array.Copy(al, alIns, al.Length);
            instructions[instructions.Count - 1].Next = null;

            icType.InvokeMember("size",
                 BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Instance,
                 null, instructions, new object[] { al.Length });

            InsUtils.ComputeOffsets(instructions);
        }

        public static void MoveBlock(Mono.Cecil.Cil.MethodBody body, int fromIndex, int startIndex, int endIndex)
        {
            if (startIndex <= fromIndex && fromIndex <= endIndex) return;

            Collection<Instruction> instructions = body.Instructions;

            //fix exception handler?
            foreach (ExceptionHandler eh in body.ExceptionHandlers)
            {
                if (IsEqual(instructions[startIndex], eh.HandlerEnd))
                {
                    if (instructions[endIndex].Next == null)
                        eh.HandlerEnd = instructions[endIndex].Previous;
                    else
                        eh.HandlerEnd = instructions[endIndex].Next;
                }
                //else if (IsEqual(instructions[startIndex], eh.FilterEnd))
                //{
                //    if (instructions[endIndex].Next == null)
                //        eh.FilterEnd = instructions[endIndex].Previous;
                //    else
                //        eh.FilterEnd = instructions[endIndex].Next;
                //}
                else if (IsEqual(instructions[startIndex], eh.TryEnd))
                {
                    if (instructions[endIndex].Next == null)
                        eh.TryEnd = instructions[endIndex].Previous;
                    else
                        eh.TryEnd = instructions[endIndex].Next;
                }
            }

            Type icType = typeof(Collection<Instruction>);
            Instruction[] alIns = (Instruction[])icType.InvokeMember("items",
                 BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance,
                 null, instructions, null);

            //ArrayList al = new ArrayList(instructions.Count);
            Instruction[] al = new Instruction[instructions.Count];
            int copyCount;

            Instruction fromNext = instructions[fromIndex].Next;
            Instruction startPrev = instructions[startIndex].Previous;
            instructions[fromIndex].Next = instructions[startIndex];
            instructions[startIndex].Previous = instructions[fromIndex];

            Instruction endNext = instructions[endIndex].Next;
            instructions[endIndex].Next = fromNext;
            if (fromNext != null)
                fromNext.Previous = instructions[endIndex];

            if (startPrev != null)
                startPrev.Next = endNext;
            if (endNext != null)
                endNext.Previous = startPrev;

            int curIndex = 0;
            if (fromIndex < startIndex)
            {
                //...
                //fromIndex 
                //...
                //startIndex 
                //...
                //endIndex 
                //...            

                //0 to fromIndex
                copyCount = fromIndex + 1;
                //al.AddRange(alIns.GetRange(0, copyCount));
                Array.Copy(alIns, 0, al, curIndex, copyCount); curIndex += copyCount;
                //startIndex to endIndex
                copyCount = endIndex - startIndex + 1;
                //al.AddRange(alIns.GetRange(startIndex, copyCount));
                Array.Copy(alIns, startIndex, al, curIndex, copyCount); curIndex += copyCount;
                //fromIndex+1 to startIndex-1
                copyCount = startIndex - fromIndex - 1;
                //al.AddRange(alIns.GetRange(fromIndex + 1, copyCount));
                Array.Copy(alIns, fromIndex + 1, al, curIndex, copyCount); curIndex += copyCount;
                //endIndex+1 to end
                copyCount = instructions.Count - endIndex - 1;
                //al.AddRange(alIns.GetRange(endIndex + 1, copyCount));
                Array.Copy(alIns, endIndex + 1, al, curIndex, copyCount); curIndex += copyCount;
            }
            else if (endIndex < fromIndex)
            {
                //...
                //startIndex 
                //...
                //endIndex 
                //...
                //fromIndex 
                //...       

                //0 to startIndex-1
                copyCount = startIndex;
                //al.AddRange(alIns.GetRange(0, copyCount));
                Array.Copy(alIns, 0, al, curIndex, copyCount); curIndex += copyCount;
                //endIndex+1 to fromIndex
                copyCount = fromIndex - endIndex;
                //al.AddRange(alIns.GetRange(endIndex + 1, copyCount));
                Array.Copy(alIns, endIndex + 1, al, curIndex, copyCount); curIndex += copyCount;
                //startIndex to endIndex
                copyCount = endIndex - startIndex + 1;
                //al.AddRange(alIns.GetRange(startIndex, copyCount));
                Array.Copy(alIns, startIndex, al, curIndex, copyCount); curIndex += copyCount;
                //fromIndex+1 to end
                copyCount = instructions.Count - fromIndex - 1;
                //al.AddRange(alIns.GetRange(fromIndex + 1, copyCount));
                Array.Copy(alIns, fromIndex + 1, al, curIndex, copyCount); curIndex += copyCount;
            }

            //if (instructions.Count != alIns.Count)
            if (instructions.Count != curIndex)
            {
                throw new ApplicationException("Internal MoveBlock error!");
            }

            instructions.Clear();
            //alIns.AddRange(al);
            Array.Copy(al, alIns, al.Length);

            icType.InvokeMember("size",
                 BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Instance,
                 null, instructions, new object[] { al.Length });

            InsUtils.ComputeOffsets(instructions);
        }

        public static AssemblyNameReference GetAssemblyName(TypeReference tr)
        {
            AssemblyNameReference anr = null;
            if (tr.Scope == null && tr.Module != null)
            {
                anr = tr.Module.Assembly.Name;
            }
            else if (tr.Scope is AssemblyNameReference)
            {
                anr = (AssemblyNameReference)tr.Scope;
            }
            else if (tr.Scope is ModuleDefinition)
            {
                anr = ((ModuleDefinition)tr.Scope).Assembly.Name;
            }
            else
            {
                SimpleMessage.ShowInfo(String.Format("Cannot find Assembly: {0}", tr.FullName));
            }
            return anr;
        }

        public static bool IsTypeSpecification(TypeReference type)
        {
            switch (type.ElementTypeIntValue)
            {
                case 0x14: //ElementType.Array:
                case 0x10: //ElementType.ByRef:
                case 0x20: //ElementType.CModOpt:
                case 0x1f: //ElementType.CModReqD:
                case 0x1b: //ElementType.FnPtr:
                case 0x15: //ElementType.GenericInst:
                case 0x1e: //ElementType.MVar:
                case 0x45: //ElementType.Pinned:
                case 0x0f: //ElementType.Ptr:
                case 0x1d: //ElementType.SzArray:
                case 0x41: //ElementType.Sentinel:
                case 0x13: //ElementType.Var:
                    return true;
            }
            return false;
        }

        public static bool IsSameType(TypeReference a, TypeReference b)
        {
            return IsSameType(a, b, true);
        }

        public static bool IsSameType(TypeReference a, TypeReference b, bool genericMatch)
        {
            if (a == null || b == null)
                return false;

            if (a.ElementTypeIntValue != 0 && b.ElementTypeIntValue != 0 && a.ElementTypeIntValue != b.ElementTypeIntValue)
                return false;

            if (genericMatch && (a.IsGenericParameter || b.IsGenericParameter))
            {
                return true;
            }

            if (a.IsGenericParameter && b.IsGenericParameter)
                return IsSameType((GenericParameter)a, (GenericParameter)b);

            if (IsTypeSpecification(a) && IsTypeSpecification(b))
                return IsSameType((TypeSpecification)a, (TypeSpecification)b, genericMatch);

            string oldFullName1 = InsUtils.GetOldFullTypeName(a);
            string oldFullName2 = InsUtils.GetOldFullTypeName(b);
            if (oldFullName1 != oldFullName2) return false;

            AssemblyNameReference anr1 = GetAssemblyName(a);
            AssemblyNameReference anr2 = GetAssemblyName(b);
            if (anr1.Name != anr2.Name) return false;

            return true;
        }

        public static bool IsSameType(GenericParameter a, GenericParameter b)
        {
            return a.Position == b.Position;

            //if (gp1.Constraints.Count == gp2.Constraints.Count)
            //{
            //    for (int i = 0; i < gp1.Constraints.Count; i++)
            //    {
            //        if (!IsSameType(gp1.Constraints[i], gp2.Constraints[i]))
            //            return false;
            //    }
            //    return true;
            //}
            //return false;
        }


        public static bool IsSameType(TypeSpecification a, TypeSpecification b, bool genericMatch)
        {
            if (!IsSameType(a.ElementType, b.ElementType, genericMatch))
                return false;

            if (a.IsGenericInstance)
                return IsSameType((GenericInstanceType)a, (GenericInstanceType)b, genericMatch);

            if (a.IsRequiredModifier || a.IsOptionalModifier)
                return IsSameType((IModifierType)a, (IModifierType)b, genericMatch);

            if (a.IsArray)
                return IsSameType((ArrayType)a, (ArrayType)b, genericMatch);

            return true;
        }

        public static bool IsSameType(ArrayType a, ArrayType b, bool genericMatch)
        {
            if (a.Rank != b.Rank || a.Dimensions.Count != b.Dimensions.Count)
                return false;

            return IsSameType(a.ElementType, b.ElementType, genericMatch);
        }

        public static bool IsSameType(IModifierType a, IModifierType b, bool genericMatch)
        {
            return IsSameType(a.ModifierType, b.ModifierType, genericMatch);
        }

        public static bool IsSameType(GenericInstanceType a, GenericInstanceType b, bool genericMatch)
        {
            if (!a.HasGenericArguments)
                return !b.HasGenericArguments;

            if (!b.HasGenericArguments)
                return false;

            if (a.GenericArguments.Count != b.GenericArguments.Count)
                return false;

            for (int i = 0; i < a.GenericArguments.Count; i++)
                if (!IsSameType(a.GenericArguments[i], b.GenericArguments[i], genericMatch))
                    return false;

            return true;
        }

        /*
        public static bool IsSameParameters(Collection<ParameterDefinition> a, Collection<ParameterDefinition> b)
        {
            var count = a.Count;

            if (count != b.Count)
                return false;

            if (count == 0)
                return true;

            for (int i = 0; i < count; i++)
                if (!IsSameType(a[i].ParameterType, b[i].ParameterType))
                    return false;

            return true;
        }
        */

        public static bool IsSameFieldByOldName(FieldReference f1, FieldReference f2)
        {
            return InsUtils.GetOldMemberName(f1) == InsUtils.GetOldMemberName(f2) &&
                   DeobfUtils.IsSameType(f1.FieldType, f2.FieldType, false);
        }

        public static bool IsSamePropertyByOldName(PropertyReference pr1, PropertyReference pr2)
        {
            if (InsUtils.GetOldMemberName(pr1) == InsUtils.GetOldMemberName(pr2) &&
                pr1.Parameters.Count == pr2.Parameters.Count &&
                   DeobfUtils.IsSameType(pr1.PropertyType, pr2.PropertyType, false))
            {
                for (int i = 0; i < pr1.Parameters.Count; i++)
                {
                    if (!DeobfUtils.IsSameType(pr1.Parameters[i].ParameterType, pr2.Parameters[i].ParameterType, false))
                        return false;
                }

                return true;
            }
            return false;
        }        

        public static bool IsSameMethodByOldName(MethodReference mr1, MethodReference mr2, bool genericMatch)
        {
            if (InsUtils.GetOldMemberName(mr1) == InsUtils.GetOldMemberName(mr2) && 
                mr1.Parameters.Count == mr2.Parameters.Count &&
                   DeobfUtils.IsSameType(mr1.ReturnType, mr2.ReturnType, genericMatch))
            {
                for (int i = 0; i < mr1.Parameters.Count; i++)
                {
                    if (!DeobfUtils.IsSameType(mr1.Parameters[i].ParameterType, mr2.Parameters[i].ParameterType, genericMatch))
                        return false;
                }

                if (mr1.GenericParameters.Count != mr2.GenericParameters.Count)
                    return false;

                //MethodDefinition md1 = DeobfUtils.Resolve(mr1);
                //MethodDefinition md2 = DeobfUtils.Resolve(mr2);
                //if (md1 != null && md2 != null)
                //{
                //    if (md1.Overrides.Count == md2.Overrides.Count)
                //    {
                //        for (int i = 0; i < md1.Overrides.Count; i++)
                //        {
                //            if (!DeobfUtils.IsSameType(md1.Overrides[i].DeclaringType, md2.Overrides[i].DeclaringType, false))
                //                return false;
                //            if (md1.MetadataToken.ToUInt32() != md2.MetadataToken.ToUInt32())
                //                return false;
                //        }
                //    }
                //}
                return true;
            }
            return false;
        }

        public static bool IsSameMethodExact(MethodDefinition md1, MethodDefinition md2)
        {
            //don't need old name 
            if (md1.Name != md2.Name) return false;
            if (md1.Parameters.Count != md2.Parameters.Count) return false;
            if (md1.GenericParameters.Count != md2.GenericParameters.Count) return false;
            for (int i = 0; i < md1.Parameters.Count; i++)
            {
                if (!IsSameType(md1.Parameters[i].ParameterType, md2.Parameters[i].ParameterType))
                    return false;
            }

            if (!IsSameType(md1.ReturnType, md2.ReturnType)) 
                return false;
            return true;
        }

        public static bool IsSameMethod(MethodDefinition md1, MethodDefinition md2)
        {
            //don't need old name 
            if (md1.Name != md2.Name) return false;
            if (md1.Parameters.Count != md2.Parameters.Count) return false;
            if (md1.GenericParameters.Count != md2.GenericParameters.Count) return false;
            for (int i = 0; i < md1.Parameters.Count; i++)
            {
                if (!IsSameType(md1.Parameters[i].ParameterType, md2.Parameters[i].ParameterType))
                    return false;
            }

            //It's valid in IL, but not in C#, we want to rename
            //if (!IsSameType(md1.ReturnType, md2.ReturnType)) 
            //    return false;
            return true;
        }

        public static bool IsSameMethodExists(MethodDefinition method)
        {
            bool found = false;
            uint token = method.MetadataToken.ToUInt32();
            TypeDefinition type = method.DeclaringType;

            foreach (MethodDefinition md in type.Methods)
            {
                if (md.MetadataToken.ToUInt32() == token)
                    continue;
                if (IsSameMethod(md, method))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static bool IsSameMethodExists(MethodDefinition method, TypeDefinition type)
        {
            bool found = false;
            foreach (MethodDefinition md in type.Methods)
            {
                if (IsSameMethod(md, method))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static bool IsSameFieldExists(FieldDefinition field)
        {
            bool found = false;
            uint token = field.MetadataToken.ToUInt32();
            TypeDefinition type = field.DeclaringType;

            foreach (FieldDefinition fd in type.Fields)
            {
                if (fd.MetadataToken.ToUInt32() == token)
                    continue;

                //don't check FieldType here because we want to rename
                if (field.Name == fd.Name)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static bool IsSamePropertyExists(PropertyDefinition property)
        {
            bool found = false;
            uint token = property.MetadataToken.ToUInt32();
            TypeDefinition type = property.DeclaringType;

            foreach (PropertyDefinition pd in type.Properties)
            {
                if (pd.MetadataToken.ToUInt32() == token)
                    continue;

                //don't check FieldType here because we want to rename
                if (property.Name == pd.Name)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static bool IsDelegate(TypeDefinition td)
        {
            return td.BaseType != null && td.IsSealed && td.BaseType.FullName == "System.MulticastDelegate";
        }

        public static bool IsAttribute(TypeDefinition td)
        {
            //TODO: handle attribute inheritance?
            return td.BaseType != null && td.BaseType.FullName == "System.Attribute";
        }

        public static bool IsJumpInstruction(Instruction ins)
        {
            if (ins == null) return false;
            switch (ins.OpCode.FlowControl)
            {
                case FlowControl.Branch:
                case FlowControl.Cond_Branch:
                    return true;
                default:
                    switch (ins.OpCode.Code)
                    {
                        case Code.Jmp:
                            return true;
                    }
                    break;
            }
            return false;
        }

        public static bool IsDirectJumpInstruction(Instruction ins)
        {
            if (ins == null) return false;

            switch (ins.OpCode.Code)
            {
                case Code.Br:
                case Code.Br_S:
                case Code.Jmp:
                    return true;

                // don't consider instructions cross try block
                // exception handler will be broken 
                //case Code.Leave:
                //case Code.Leave_S:
                //    if (IsExitInstruction(ins, false))
                //        return true;
                //    break;
            }
            return false;
        }

        public static bool IsExitInstruction(Instruction ins)
        {
            if (ins == null) return false;

            switch (ins.OpCode.Code)
            {
                case Code.Ret:
                    return true;
                case Code.Leave:
                case Code.Leave_S:
                    /*Instruction op = ins.Operand as Instruction;
                    while (IsNopInstruction(op))
                    {
                        op = op.Next;
                    }
                    if (op.OpCode.Code == Code.Ret)
                        return true;
                    //if (deepCheck)
                    {
                        Instruction insNext = op.Next;
                        while (insNext != null)
                        {
                            if (IsExitInstruction(insNext))
                                return true;
                            if (IsBlockDelimiter(insNext))
                                return false;
                            insNext = insNext.Next;
                        }
                    }
                    break;
                     */
                    return true;
                case Code.Throw:
                case Code.Rethrow:
                    return true;
                case Code.Endfinally:
                case Code.Endfilter:
                    return true;
            }
            return false;
        }

        public static bool IsNopInstruction(Instruction ins)
        {
            return ins != null && ins.OpCode.Code == Code.Nop;
        }

        public static bool IsLeaveInstruction(Instruction ins)
        {
            if (ins == null) return false;
            if (ins.OpCode.Code == Code.Leave || ins.OpCode.Code == Code.Leave_S)
            {
                return true;
            }
            return false;
        }

        public static bool IsDirectLeaveInstruction(Instruction ins)
        {
            if (IsLeaveInstruction(ins))
            {
                Instruction op = ins.Operand as Instruction;
                if (IsLeaveInstruction(op))
                    return true;
            }
            return false;
        }

        public static bool IsConditionalJumpInstruction(Instruction ins)
        {
            if (ins == null) return false;

            switch (ins.OpCode.Code)
            {
                case Code.Brfalse:
                case Code.Brfalse_S:
                case Code.Brtrue:
                case Code.Brtrue_S:
                    return true;
                case Code.Ble:
                case Code.Ble_S:
                case Code.Bgt:
                case Code.Bgt_S:
                    return true;
                case Code.Bge:
                case Code.Bge_S:
                case Code.Blt:
                case Code.Blt_S:
                    return true;
                case Code.Blt_Un:
                case Code.Blt_Un_S:
                case Code.Bge_Un:
                case Code.Bge_Un_S:
                    return true;
                case Code.Beq:
                case Code.Beq_S:
                case Code.Bne_Un:
                case Code.Bne_Un_S:
                    return true;
            }
            return false;
        }

        public static bool IsBlockDelimiter(Instruction ins)
        {
            bool b = false;
            if (ins == null) return b;
            switch (ins.OpCode.FlowControl)
            {
                case FlowControl.Branch:
                    b = true;
                    break;
                case FlowControl.Return:
                    //if (ins.OpCode.Code == Code.Ret)
                    b = true;
                    break;
                case FlowControl.Throw:
                    b = true;
                    break;
                //case FlowControl.Break:
            }
            return b;
        }

        public static bool IsSwitchBranch(Instruction insJump, MethodDefinition method)
        {
            if (!IsDirectJumpInstruction(insJump))
                return false;

            Instruction insPrev = insJump.Previous;
            bool found = false;
            while (insPrev != null)
            {
                if (IsDirectJumpInstruction(insPrev) ||
                    IsExitInstruction(insPrev) ||
                    insPrev.OpCode.Code == Code.Switch
                    )
                {
                    found = true;
                    break;
                }
                insPrev = insPrev.Previous;
            }
            if (!found) return false;
            Instruction insStart = insPrev.Next;
            Collection<Instruction> ic = method.Body.Instructions;
            for (int i = 0; i < ic.Count; i++)
            {
                if (ic[i].OpCode.Code != Code.Switch)
                    continue;
                if (IsEqual(ic[i].Next, insStart))
                    return true;
                Instruction[] ops = (Instruction[])ic[i].Operand;
                foreach (Instruction ins in ops)
                {
                    if (IsEqual(ins, insStart))
                        return true;
                }
            }
            return false;
        }

        public static bool IsSwitchBranchDefault(Instruction insJump, MethodDefinition method)
        {
            if (!IsDirectJumpInstruction(insJump))
                return false;
            Instruction insPrev = insJump.Previous;
            while (IsNopInstruction(insPrev)) insPrev = insPrev.Previous;
            if (insPrev == null) return false;
            if (insPrev.OpCode.Code != Code.Switch) return false;
            Instruction[] ops = (Instruction[])insPrev.Operand;
            Instruction insJumpTo = (Instruction)insJump.Operand;
            foreach (Instruction ins in ops)
            {
                //To check ins.Operand?

                if (ins.Offset < insJump.Offset || ins.Offset > insJumpTo.Offset)
                    return false;
            }
            return true;
        }

        public static bool IsForeachStatement(Instruction insJump, MethodDefinition method, bool deepCheck)
        {
            if (!IsDirectJumpInstruction(insJump)) return false;

            Instruction insJumpTo = insJump.Operand as Instruction;
            while (IsDirectJumpInstruction(insJumpTo))
            {
                insJump = insJumpTo;
                insJumpTo = insJumpTo.Operand as Instruction;
            }

            Instruction insTryEnd = insJump.Operand as Instruction;
            int maxCount = method.Body.Instructions.Count;
            int count = 0;
            while (insTryEnd != null && !IsExitInstruction(insTryEnd))
            {
                insTryEnd = insTryEnd.Next;
                count++;
                if (count > maxCount) //dead loop?
                    break;
            }
            if (insTryEnd == null) return false;
            Instruction insHandlerEnd = insTryEnd.Operand as Instruction;
            if (insHandlerEnd == null) return false;
            insTryEnd = insTryEnd.Next;
            if (insTryEnd == null) return false;

            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
            foreach (ExceptionHandler eh in ehc)
            {
                if (eh.HandlerType != ExceptionHandlerType.Finally)
                    continue;
                if (eh.HandlerEnd == null || eh.HandlerStart == null)
                    continue;
                Instruction handlerEndPrev = eh.HandlerEnd.Previous;
                if (handlerEndPrev == null || handlerEndPrev.OpCode.Code != Code.Endfinally)
                    continue;
                handlerEndPrev = handlerEndPrev.Previous;
                if (handlerEndPrev == null || handlerEndPrev.OpCode.Code != Code.Callvirt ||
                    handlerEndPrev.Operand == null || handlerEndPrev.Operand.ToString() != "System.Void System.IDisposable::Dispose()")
                    continue;
                if (!IsEqual(insJump, eh.TryStart))
                    continue;
                if (!IsEqual(insTryEnd, eh.TryEnd))
                    continue;
                if (!IsEqual(insTryEnd, eh.HandlerStart))
                    continue;
                //if (!IsEqual(insHandlerEnd, eh.HandlerEnd))
                //    continue;
                //if (insHandlerEnd.Offset < eh.HandlerEnd.Offset)
                //    continue;                
                return true;
            }

            if (deepCheck)
            {
                foreach (ExceptionHandler eh in ehc)
                {
                    if (eh.HandlerType != ExceptionHandlerType.Finally)
                        continue;
                    if (!IsDirectJumpInstruction(eh.TryStart))
                        continue;
                    if (IsEqual(insJump, eh.TryStart))
                        continue;
                    if (!IsForeachStatement(eh.TryStart, method, false))
                        continue;
                    if (!IsEqual(insJump.Operand as Instruction, eh.TryStart.Operand as Instruction))
                        continue;
                    return true;
                }
            }

            return false;
        }

        public static bool IsEqual(Instruction ins1, Instruction ins2)
        {
            if (ins1 == null || ins2 == null) return false;
            return ins1.Offset == ins2.Offset && ins1.OpCode.Equals(ins2.OpCode);
        }


        public static List<int> FindReferences(MethodDefinition method, int insIndex)
        {
            return FindReferences(method, insIndex, -1, -1, -1, true);
        }

        public static List<int> FindReferences(MethodDefinition method, int insIndex, int maxCount, int insStart, int insEnd, bool includeExceptionHandler)
        {
            List<int> list = new List<int>();
            Collection<Instruction> ic = method.Body.Instructions;

            if (insIndex < 0 || insIndex >= ic.Count)
                return list;

            List<JumpInstruction> insList = JumpInstruction.Find(method, insStart, insEnd);
            list.AddRange(FindReferences(method, insIndex, maxCount, insList, includeExceptionHandler));
            return list;
        }

        public static List<int> FindReferences(MethodDefinition method, int insIndex, int maxCount, List<JumpInstruction> insList, bool includeExceptionHandler)
        {
            Collection<Instruction> ic = method.Body.Instructions;

            if (insIndex < 0 || insIndex >= ic.Count)
                return new List<int>();

            return FindReferences(method, ic[insIndex], maxCount, insList, includeExceptionHandler);
        }

        public static List<int> FindReferences(MethodDefinition method, Instruction ins, int maxCount, List<JumpInstruction> insList, bool includeExceptionHandler)
        {
            List<int> list = new List<int>();

            foreach (JumpInstruction insTemp in insList)
            {
                //if (InsUtils.IsJumpInstruction(insTemp))
                {
                    if (insTemp.IsTarget(ins))
                    {
                        list.Add(insTemp.Instruction.Index);
                    }
                    if (maxCount > 0 && list.Count >= maxCount)
                        break;
                }
            }//end for       

            if (includeExceptionHandler)
            {
                Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
                for (int i = 0; i < ehc.Count; i++)
                {
                    if (IsReferedByExceptionHandlerStartOrEnd(ins, ehc[i]))
                    {
                        list.Add(-i - 1);
                        if (maxCount > 0 && list.Count >= maxCount)
                            break;
                    }
                }
            }
            return list;
        }

        public static bool IsValidExceptionHandler(ExceptionHandler eh)
        {
            if (eh.TryStart != null && eh.TryEnd != null && eh.TryStart.Offset > eh.TryEnd.Offset)
                return false;
            if (eh.HandlerStart != null && eh.HandlerEnd != null && eh.HandlerStart.Offset > eh.HandlerEnd.Offset)
                return false;
            if (eh.FilterStart != null && /*eh.FilterEnd != null &&*/ eh.FilterStart.Offset > eh.HandlerStart.Offset)
                return false;
            if (eh.TryEnd != null && eh.HandlerStart != null && eh.TryEnd.Offset > eh.HandlerStart.Offset)
                return false;
            return true;
        }

        public static bool IsGlobalExceptionHandler(ExceptionHandler eh, MethodDefinition method)
        {
            Collection<Instruction> ic = method.Body.Instructions;
            if (eh.TryStart != null && eh.TryStart.Offset == 0 &&
                eh.TryEnd != null & eh.HandlerStart != null &&
                eh.TryEnd.Offset == eh.HandlerStart.Offset &&
                eh.HandlerEnd != null &&
                (eh.HandlerEnd.Offset == ic[ic.Count - 1].Offset ||
                    (eh.HandlerEnd.Next != null && eh.HandlerEnd.Next.OpCode.Code == Code.Ret &&
                    eh.HandlerEnd.Next.Offset == ic[ic.Count - 1].Offset)
                  )
                )
            {
                return true;
            }
            return false;
        }

        public static bool IsCrossExceptionHandler(int startIndex, int endIndex, Instruction insStart, Instruction insEnd, Collection<Instruction> ic)
        {
            int start = ic.IndexOf(insStart);
            insEnd = insEnd.Next == null ? insEnd : insEnd.Previous;
            int end = ic.IndexOf(insEnd);
            if (startIndex <= start && start <= endIndex)
            {
                if ((start == startIndex || start == endIndex) && DeobfUtils.IsDirectJumpInstruction(insStart))
                {
                }
                else
                {
                    return true;
                }
            }
            if (startIndex <= end && end <= endIndex)
            {
                if ((end == startIndex || end == endIndex) && DeobfUtils.IsDirectJumpInstruction(insEnd))
                {
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsCrossExceptionHandler(int startIndex, int endIndex, MethodDefinition method)
        {
            Collection<Instruction> ic = method.Body.Instructions;
            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;

            foreach (ExceptionHandler eh in ehc)
            {
                if (eh.TryStart != null && eh.TryEnd != null)
                {
                    if (IsCrossExceptionHandler(startIndex, endIndex, eh.TryStart, eh.TryEnd, ic))
                        return true;
                }
                if (eh.HandlerStart != null && eh.HandlerEnd != null)
                {
                    if (IsCrossExceptionHandler(startIndex, endIndex, eh.HandlerStart, eh.HandlerEnd, ic))
                        return true;
                }
                if (eh.FilterStart != null && eh.HandlerStart != null)
                {
                    if (IsCrossExceptionHandler(startIndex, endIndex, eh.FilterStart, eh.HandlerStart, ic))
                        return true;
                }
            }
            return false;
        }

        public static bool IsReferedByExceptionHandlerStartOrEnd(Instruction ins, Collection<ExceptionHandler> ehc)
        {
            foreach (ExceptionHandler eh in ehc)
            {
                if (IsReferedByExceptionHandlerStartOrEnd(ins, eh))
                    return true;
            }
            return false;
        }

        public static bool IsReferedByExceptionHandlerStartOrEnd(Instruction ins, ExceptionHandler eh)
        {

            if (IsEqual(ins, eh.TryStart) || IsEqual(ins, eh.TryEnd) ||
                IsEqual(ins, eh.FilterStart) || //IsEqual(ins, eh.FilterEnd) ||
                IsEqual(ins, eh.HandlerStart) || IsEqual(ins, eh.HandlerEnd)
                )
            {
                return true;
            }

            return false;
        }

        /*
        public static bool IsInMiddleOfExceptionHandler(int fromIndex, int startIndex, int endIndex, MethodDefinition method)
        {
            int start;
            int end;
            Collection<Instruction> ic = method.Body.Instructions;
            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
            foreach (ExceptionHandler eh in ehc)
            {
                if (eh.TryStart != null && eh.TryEnd != null)
                {
                    start = ic.IndexOf(eh.TryStart);
                    end = ic.IndexOf(eh.TryEnd.Next == null ? eh.TryEnd : eh.TryEnd.Previous);
                    if (fromIndex < start && start < startIndex && endIndex < end ||
                        fromIndex > end && start < startIndex && endIndex < end
                        )
                        return true;
                }
                if (eh.HandlerStart != null && eh.HandlerEnd != null)
                {
                    start = ic.IndexOf(eh.HandlerStart);
                    end = ic.IndexOf(eh.HandlerEnd.Next == null ? eh.HandlerEnd : eh.HandlerEnd.Previous);
                    if (fromIndex < start && start < startIndex && endIndex < end ||
                        fromIndex > end && start < startIndex && endIndex < end)
                        return true;
                }
                if (eh.FilterStart != null && eh.FilterEnd != null)
                {
                    start = ic.IndexOf(eh.FilterStart);
                    end = ic.IndexOf(eh.FilterEnd.Next == null ? eh.FilterEnd : eh.FilterEnd.Previous);
                    if (fromIndex < start && start < startIndex && endIndex < end ||
                        fromIndex > end && start < startIndex && endIndex < end)
                        return true;
                }
            }
            return false;
        }

        
        public static bool IsBranchMoveBreakingExceptionHandler(Instruction start, Instruction end, Collection<ExceptionHandler> ehc)
        {
            foreach (ExceptionHandler eh in ehc)
            {
                if (IsBranchMoveBreakingExceptionHandler(start, end, eh))
                    return true;
            }
            return false;
        }

        public static bool IsBranchMoveBreakingExceptionHandler(Instruction start, Instruction end, ExceptionHandler eh)
        {
            if (IsEqual(end.Next, eh.HandlerStart))
                return true;
            if (IsEqual(start, eh.TryEnd))
                return true;
            if (IsEqual(end, eh.HandlerEnd.Previous))
                return true;
            return false;
        }
        */

        private static void GetExceptionHandlerStartEnd(int ehIndex, MethodDefinition method, ref int start, ref int end)
        {
            Collection<Instruction> ic = method.Body.Instructions;
            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
            ExceptionHandler eh = ehc[ehIndex];
            start = ic.IndexOf(eh.TryStart);
            if (eh.HandlerEnd != null)
                end = ic.IndexOf(eh.HandlerEnd.Next == null ? eh.HandlerEnd : eh.HandlerEnd.Previous);
            //else if (eh.FilterEnd != null)
            //    end = ic.IndexOf(eh.FilterEnd.Next == null ? eh.FilterEnd : eh.FilterEnd.Previous);
            else end = ic.IndexOf(eh.TryEnd.Next == null ? eh.TryEnd : eh.TryEnd.Previous);
        }

        public static int FindExceptionHandlerByInstructionIndex(int insIndex, MethodDefinition method)
        {
            int ehIndex = -1;
            int ehStart = -1;
            int ehEnd = -1;

            int start;
            int end;
            Collection<Instruction> ic = method.Body.Instructions;
            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
            for (int i = 0; i < ehc.Count; i++)
            {
                ExceptionHandler eh = ehc[i];

                //if (IsGlobalExceptionHandler(eh, method)) 
                //    continue;

                start = -1;
                end = -1;
                GetExceptionHandlerStartEnd(i, method, ref start, ref end);

                if (start <= insIndex && insIndex <= end)
                {
                    if (ehIndex < 0)
                    {
                        ehIndex = i;
                        ehStart = start;
                        ehEnd = end;
                    }
                    else
                    {
                        if (ehStart <= start && end <= ehEnd)
                        {
                            ehIndex = i;
                            ehStart = start;
                            ehEnd = end;

                        }
                    }
                    continue;
                }
            }
            return ehIndex;
        }

        public static bool IsBranchMoveBreakingExceptionHandler(int fromIndex, int startIndex, MethodDefinition method)
        {
            int ehFromIndex = FindExceptionHandlerByInstructionIndex(fromIndex, method);
            int ehStartIndex = FindExceptionHandlerByInstructionIndex(startIndex, method);
            if (ehFromIndex < 0 && ehStartIndex < 0) return false;
            if (ehFromIndex >= 0 && ehStartIndex < 0) return false;
            if (ehFromIndex < 0 && ehStartIndex >= 0) return false;

            Collection<Instruction> ic = method.Body.Instructions;
            Collection<ExceptionHandler> ehc = method.Body.ExceptionHandlers;
            
            ExceptionHandler ehFrom = ehc[ehFromIndex];
            int fromStart = -1;
            int fromEnd = -1;
            GetExceptionHandlerStartEnd(ehFromIndex, method, ref fromStart, ref fromEnd);

            ExceptionHandler ehStart = ehc[ehStartIndex];
            int startStart = -1;
            int startEnd = -1;
            GetExceptionHandlerStartEnd(ehStartIndex, method, ref startStart, ref startEnd);

            //if (fromStart <= startStart && startEnd <= fromEnd) return false;
            //if (startStart <= fromStart && fromEnd <= startEnd) return false;
            
            if (fromStart <= startStart && startStart <= fromEnd) return false;
            if (fromStart <= startEnd && startEnd <= fromEnd) return false;
            if (startStart <= fromStart && fromStart <= startEnd) return false;
            if (startStart <= fromEnd && fromEnd <= startEnd) return false;          

            return true;
        }

        public static int GetKey(MemberReference memberReference)
        {
            if (memberReference is MethodReference)
            {
                MethodReference mr = (MethodReference)memberReference;
                StringBuilder sb = new StringBuilder();
                sb.Append(InsUtils.GetOldMemberName(mr));
                sb.Append(InsUtils.GetOldMemberName(mr.ReturnType));
                //sb.Append(mr.Parameters.Count.ToString());
                if (mr.HasParameters)
                {
                    foreach (ParameterDefinition pd in mr.Parameters)
                    {
                        sb.Append(InsUtils.GetOldMemberName(pd.ParameterType));
                    }
                }
                //sb.Append(mr.GenericParameters.Count.ToString());
                if (mr.HasGenericParameters)
                {
                    foreach (GenericParameter gp in mr.GenericParameters)
                    {
                        sb.Append(InsUtils.GetOldMemberName(gp));
                    }
                }
                //if (mr.HasOverrides)
                //{
                //    sb.Append(InsUtils.GetOldMemberName(mr.Overrides[0].DeclaringType));
                //    sb.Append(InsUtils.GetOldMemberName(mr.Overrides[0]));
                //}               
                return sb.ToString().GetHashCode();
            }
            else if (memberReference is FieldReference)
            {
                FieldReference fr = (FieldReference)memberReference;
                StringBuilder sb = new StringBuilder();
                sb.Append(InsUtils.GetOldMemberName(fr));
                sb.Append(InsUtils.GetOldMemberName(fr.FieldType));
                return sb.ToString().GetHashCode();
            }

            return InsUtils.GetOldMemberName(memberReference).GetHashCode();
        }

        /*
        public static int GetFullKey(TypeDefinition td, MemberReference mr, string newName)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append(td.Module.Assembly.Name.Name);
            sb.Append(InsUtils.GetOldFullTypeName(td));
            sb.Append(newName);
            if (mr is MethodDefinition)
            {
                MethodDefinition md = mr as MethodDefinition;
                for (int i = 0; i < md.Parameters.Count; i++)
                {
                    sb.Append(InsUtils.GetOldFullTypeName(md.Parameters[i].ParameterType));
                }
            }
            else if (mr is PropertyDefinition)
            {
                PropertyDefinition pd = mr as PropertyDefinition;
                for (int i = 0; i < pd.Parameters.Count; i++)
                {
                    sb.Append(InsUtils.GetOldFullTypeName(pd.Parameters[i].ParameterType));
                }
            }

            //if (mr is MethodDefinition)
            //{
            //    sb.Append(GetOldFullName(((MethodDefinition)mr).ReturnType));
            //}
            //else if (mr is PropertyDefinition)
            //{
            //    sb.Append(GetOldFullName(((PropertyDefinition)mr).PropertyType));
            //} else  if (mr is FieldDefinition)
            //{
            //    sb.Append(GetOldFullName(((FieldDefinition)mr).FieldType));
            //}
            //else if (mr is EventDefinition)
            //{
            //    sb.Append(GetOldFullName(((EventDefinition)mr).EventType));
            //} 

            return sb.ToString().GetHashCode();
        }

        public static void RemoveAttribute(Mono.Cecil.ICustomAttributeProvider cap, string attributeName)
        {
            for (int i = 0; i < cap.CustomAttributes.Count; i++)
            {
                if (cap.CustomAttributes[i].Constructor == null) continue;
                string attrName = cap.CustomAttributes[i].Constructor.DeclaringType.Name;
                if (attrName.IndexOf(attributeName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    cap.CustomAttributes.RemoveAt(i);
                    i--;
                }
            }
        }
        */

        public static Instruction FindPrevInstruction(Instruction ins,
            string[] opNames,
            object operand,
            bool exactMatch)
        {
            if (ins == null) return null;
            Instruction prev = ins.Previous;
            while (IsNopInstruction(prev))
            {
                prev = prev.Previous;
            }
            if (prev != null)
            {
                bool matched = false;
                if (exactMatch)
                {
                    foreach (string opName in opNames)
                    {
                        matched = prev.OpCode.Name.Equals(opName, StringComparison.OrdinalIgnoreCase);
                        if (matched) break;
                    }
                }
                else
                {
                    foreach (string opName in opNames)
                    {
                        matched = prev.OpCode.Name.StartsWith(opName, StringComparison.OrdinalIgnoreCase);
                        if (matched) break;
                    }
                }

                if (matched)
                {
                    if (operand == null)
                    {
                        //found, ignore operand checking
                    }
                    else if (operand != null && prev.Operand != null && operand.ToString() == prev.Operand.ToString())
                    {
                        //found, same operand
                    }
                    else
                    {
                        prev = null;
                    }
                }
                else prev = null;
            }
            return prev;
        }

        public static List<Instruction> FindPrevInstructions(
            Instruction ins,
            List<JumpInstruction> jumpIns,
            string opName, object operand)
        {
            List<Instruction> list = new List<Instruction>();
            Instruction prev = FindPrevInstruction(ins, new string[] { opName }, operand, false);
            if (prev != null) list.Add(prev);

            foreach (JumpInstruction tmp in jumpIns)
            {
                if (tmp.IsTarget(ins))
                {
                    prev = FindPrevInstruction(tmp.Instruction, new string[] { opName }, operand, false);
                    if (prev != null) list.Add(prev);
                }
            }

            return list;
        }


        public static List<TypeDefinition> FindDerivedTypes(TypeDefinition type, int level, Dictionary<string, AssemblyDefinition> allAssemblies)
        {
            List<TypeDefinition> list = new List<TypeDefinition>();
            foreach (AssemblyDefinition ad in allAssemblies.Values)
            {
                foreach (ModuleDefinition md in ad.Modules)
                {
                    foreach (TypeDefinition td in md.AllTypes)
                    {
                        if (type.IsInterface)
                        {
                            if (IsImplementInterface(type, td, allAssemblies))
                            {
                                list.Add(td);                                
                            }
                        }
                        else if (IsBaseType(type, td, level, allAssemblies))
                        {
                            list.Add(td);
                        }
                    }
                }
            }
            return list;
        }

        public static TypeDefinition GetTypeDefinition(TypeReference tr, Dictionary<string, AssemblyDefinition> allAssemblies)
        {
            if (tr == null || String.IsNullOrEmpty(tr.FullName))
                return null;

            if (tr is TypeDefinition)
                return (TypeDefinition)tr;

            TypeDefinition td = null;
            AssemblyDefinition ad = null;

            AssemblyNameReference anr = DeobfUtils.GetAssemblyName(tr);
            if (anr != null)
            {
                if (allAssemblies.ContainsKey(anr.FullName))
                    ad = allAssemblies[anr.FullName];
                if (ad == null && anr.FullName.StartsWith("mscorlib") && allAssemblies.Count == 1)
                {
                    foreach (AssemblyDefinition adTmp in allAssemblies.Values)
                    {
                        ad = adTmp;
                        break;
                    }
                }
            }
            if (ad != null)
            {
                string oldFullName = InsUtils.GetOldFullTypeName(tr);
                //if (tr.IsGenericInstance)
                //{
                //    oldFullName = InsUtils.GetOldFullTypeName(tr.GetElementType());
                //}
                //else
                //{
                //    oldFullName = InsUtils.GetOldFullTypeName(tr);
                //}
                foreach (ModuleDefinition md in ad.Modules)
                {
                    if (md.AllTypesDictionary.ContainsKey(oldFullName))
                    {
                        td = md.AllTypesDictionary[oldFullName];
                    }
                    else
                    {
                        td = md.GetType(oldFullName); //key still old name?
                        if (td == null)
                        {
                            //if (tr.IsGenericInstance)
                            //    td = md.GetType(tr.GetElementType().FullName);
                            //else
                                td = md.GetType(tr.FullName);
                        }
                    }

                    if (td != null)
                    {
                        break;
                    }
                }
            }
            else if (tr.Scope is ModuleReference)
            {
                ModuleReference mod_reference = tr.Scope as ModuleReference;
                if (mod_reference != null)
                {
                    foreach (ModuleDefinition netmodule in tr.Module.Assembly.Modules)
                        if (netmodule.Name == mod_reference.Name)
                        {
                            string oldFullName = InsUtils.GetOldFullTypeName(tr);
                            td = netmodule.GetType(oldFullName);
                            break;
                        }
                }
            }

            //#if DEBUG
            //            if (td == null && ad != null)
            //                SimpleMessage.ShowInfo(String.Format("Cannot find TypeDefinition: {0}", tr.FullName));
            //#endif
            return td;
        }

        public static bool IsBaseType(TypeDefinition baseType, TypeDefinition inheritType, 
            int level, Dictionary<string, AssemblyDefinition> allAssemblies)
        {
            if (inheritType == null) return false;
            TypeReference tmpTr = inheritType.BaseType;
            while (tmpTr != null && level>=0 )
            {
                if (DeobfUtils.IsSameType(baseType, tmpTr))
                    return true;
                
                level--;
                if (level < 0) break;

                TypeDefinition tmpTd = GetTypeDefinition(tmpTr, allAssemblies);
                if (tmpTd == null) break;
                tmpTr = tmpTd.BaseType;
            }
            return false;
        }

        public static bool IsImplementInterface(TypeDefinition baseInterface, TypeDefinition implementType, 
            Dictionary<string, AssemblyDefinition> allAssemblies)
        {
            if (implementType == null) return false;
            foreach (TypeReference tr in implementType.Interfaces)
            {
                TypeDefinition ifType = GetTypeDefinition(tr, allAssemblies);
                if (ifType != null && DeobfUtils.IsSameType(ifType, baseInterface))
                    return true;
                if(IsImplementInterface(baseInterface, ifType, allAssemblies))
                    return true;
            }
            return false;
        }

        #region Resolve
        static IEnumerable<AssemblyDefinition> assemblyResolveList;
        static AssemblyResolveEventHandler assemblyResolveEventHandler = new AssemblyResolveEventHandler(Assembly_ResolveFirst);
        static AssemblyDefinition Assembly_ResolveFirst(object sender, AssemblyNameReference reference)
        {
            if (assemblyResolveList != null)
            {
                foreach (AssemblyDefinition ad in assemblyResolveList)
                {
                    if (ad.FullName == reference.FullName)
                        return ad;
                }
            }
            return null;
        }
        static void SetupResolveFirst(IEnumerable<AssemblyDefinition> assemblyList)
        {
            if (assemblyList == null) return;
            BaseAssemblyResolver bar = GlobalAssemblyResolver.Instance as BaseAssemblyResolver;
            if (bar != null)
            {
                assemblyResolveList = assemblyList;
                bar.ResolveFirst += assemblyResolveEventHandler;
            }
            DefaultAssemblyResolver dar = GlobalAssemblyResolver.Instance as DefaultAssemblyResolver;
            if (dar != null)
            {
                foreach (AssemblyDefinition ad in assemblyList)
                {
                    dar.RemoveAssembly(ad);
                }
            }
        }
        static void RemoveResolveFirst()
        {
            if (assemblyResolveList == null) return;
            BaseAssemblyResolver bar = GlobalAssemblyResolver.Instance as BaseAssemblyResolver;
            if (bar != null)
            {
                bar.ResolveFirst -= assemblyResolveEventHandler;
                assemblyResolveList = null;
            }
        }

        
        public static AssemblyDefinition ResolveAssembly(string assemblyFullName, IEnumerable<AssemblyDefinition> assemblyList)
        {
            try
            {
                SetupResolveFirst(assemblyList);

                if (assemblyResolveList != null)
                {
                    //TargetRuntime tr = TargetRuntime.Net_4_0;
                    foreach (AssemblyDefinition ad in assemblyResolveList)
                    {
                        if (ad.FullName == assemblyFullName)
                            return ad;
                        //tr = ad.MainModule.Runtime;
                    }

                    //it's not working
                    //if (tr == TargetRuntime.Net_2_0 && AssemblyUtils.IsSystemAssembly(assemblyFullName))
                    //{
                    //    string tmpName = assemblyFullName.Replace("4.0.0.0", "2.0.0.0");
                    //    if (tmpName != assemblyFullName)
                    //    {
                    //        try
                    //        {
                    //            AssemblyDefinition ad = Mono.Cecil.GlobalAssemblyResolver.Instance.Resolve(tmpName);
                    //            if (ad != null)
                    //                return ad;
                    //        }
                    //        catch { }
                    //    }
                    //}
                }
            }
            catch { }
            finally
            {
                RemoveResolveFirst();
            }
            
            return null;
        }

        public static FieldDefinition Resolve(FieldReference fr, IEnumerable<AssemblyDefinition> assemblyList, ITextInfo info)
        {
            if (fr == null) 
                return null;
            if (fr is FieldDefinition) 
                return (FieldDefinition)fr;
            try
            {
                SetupResolveFirst(assemblyList);
                return fr.Resolve();
            }
            catch(Exception ex)
            {
                if (info != null && !info.TextInfo.Contains(ex.Message))
                    info.AppendTextInfoLine(ex.Message);
                return null;
            }
            finally
            {
                RemoveResolveFirst();
            }
        }

        public static MethodDefinition Resolve(MethodReference mr, IEnumerable<AssemblyDefinition> assemblyList, ITextInfo info)
        {
            if (mr == null) 
                return null;
            if (mr is MethodDefinition)
                return (MethodDefinition)mr;
            try
            {
                SetupResolveFirst(assemblyList);
                return mr.Resolve();
            }
            catch(Exception ex)
            {
                if (info != null && !info.TextInfo.Contains(ex.Message))
                    info.AppendTextInfoLine(ex.Message);
                return null;
            }
            finally
            {
                RemoveResolveFirst();
            }
        }

        public static TypeDefinition Resolve(TypeReference tr, IEnumerable<AssemblyDefinition> assemblyList, ITextInfo info)
        {
            if (tr == null) 
                return null;
            if (tr is TypeDefinition)
                return (TypeDefinition)tr;
            
            try
            {
                SetupResolveFirst(assemblyList);

                if (tr.Name == null || tr.Name.Length == 0)
                {
                    throw new ApplicationException(
                        String.Format("Could not resolve: type reference 0x{0:x08}, type name is empty.", 
                        tr.MetadataToken.ToUInt32()));
                }

                return tr.Resolve();
            }
            catch(Exception ex)
            {
                if (info != null && !info.TextInfo.Contains(ex.Message))
                    info.AppendTextInfoLine(ex.Message);
                return null;
            }
            finally
            {
                RemoveResolveFirst();
            }
        }
        #endregion Resolve

    } //end of class
}
