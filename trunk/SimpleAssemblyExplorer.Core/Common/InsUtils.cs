using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using System.Text;
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
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace SimpleAssemblyExplorer
{

    public class InsUtils
    {
        #region Instructions

        public static int ToNop(ILProcessor ilp, Instruction ins, bool sameSize)
        {
            if (ins == null) return 0;

            int size = ins.GetSize();
            ins.OpCode = OpCodes.Nop;
            ins.Operand = null;
            if (sameSize)
            {
                for (int i = 1; i < size; i++)
                {
                    Instruction newIns = ilp.Create(OpCodes.Nop);
                    ilp.InsertAfter(ins, newIns);
                }
            }
            else
            {
                size = 1;
            }
            return size;
        }

        public static int ToNop(ILProcessor ilp, Instruction ins)
        {
            return ToNop(ilp, ins, false);
        }

        public static int ToNop(ILProcessor ilp, Collection<Instruction> instructions, int index)
        {
            return ToNop(ilp, instructions[index], false);
        }

        public static int ToNop(ILProcessor ilp, Collection<Instruction> instructions, int index, bool sameSize)
        {
            return ToNop(ilp, instructions[index], sameSize);
        }       


        public static object GetInstructionOperand(Instruction ins)
        {
            if (ins.OpCode.Name.StartsWith("ldc.i4"))
            {
                if (ins.Operand != null)
                    return Convert.ToInt32(ins.Operand);
                int p = PathUtils.IndexOfDot(ins.OpCode.Name, 2);
                string s = ins.OpCode.Name.Substring(p + 1);
                if (s == "m1") return -1;
                return Convert.ToInt32(s);
            }
            return ins.Operand;
        }
        #endregion Instructions

        #region Get ... Text
        private static void GetGenericParametersText(StringBuilder sb, Collection<GenericParameter> gpc)
        {
            const string sep = ", ";
            if (gpc.Count > 0)
            {
                sb.Append("<");
                foreach (GenericParameter gp in gpc)
                {
                    sb.Append(GetTypeText(gp.GetElementType()));
                    sb.Append(sep);
                }
                sb.Remove(sb.Length - sep.Length, sep.Length);
                sb.Append(">");
            }
        }
        private static void GetGenericArgumentsText(StringBuilder sb, Collection<TypeReference> gac)
        {
            const string sep = ", ";
            if (gac.Count > 0)
            {
                sb.Append("<");
                foreach (TypeReference tr in gac)
                {
                    sb.Append(GetTypeText(tr));
                    sb.Append(sep);
                }
                sb.Remove(sb.Length - sep.Length, sep.Length);
                sb.Append(">");
            }
        } 

        public static string GetMethodText(MethodReference mr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(mr.Name);

            if (mr.HasGenericParameters)
            {
                GetGenericParametersText(sb, mr.GenericParameters);
            }           

            sb.Append("(");
            for (int i = 0; i < mr.Parameters.Count; i++)
            {
                sb.Append(GetTypeText(mr.Parameters[i].ParameterType));
                if (i < mr.Parameters.Count - 1) sb.Append(", ");
            }
            sb.Append("): ");
            sb.Append(GetTypeText(mr.ReturnType));
            return sb.ToString();
        }

        public static int GetSentinelPosition(IMethodSignature ms)
        {
            if (!ms.HasParameters)
                return -1;

            var parameters = ms.Parameters;
            for (int i = 0; i < parameters.Count; i++)
                if (parameters[i].ParameterType.IsSentinel)
                    return i;

            return -1;
        }

        public static string GetMethodFullText(MethodDefinition md)
        {
            int sentinel = GetSentinelPosition(md);

            StringBuilder sb = new StringBuilder();

            if (md.IsPrivate)
            {
                sb.Append("private");
            }
            else if (md.IsPublic)
            {
                sb.Append("public");
            }
            else if (md.IsInternalCall)
            {
                sb.Append("internal");
            }

            if (md.IsHideBySig)
            {
                sb.Append(" hidebysig");
            }

            if (md.IsSpecialName)
            {
                sb.Append(" specialname");
            }

            if (md.IsRuntimeSpecialName)
            {
                sb.Append(" rtspecialname");
            }

            if (md.IsStatic)
            {
                sb.Append(" static");
            }

            if (md.IsPInvokeImpl)
            {
                sb.Append(" extern");
            }

            if (md.IsNewSlot)
            {
                sb.Append(" newslot");
            }

            if (md.IsAbstract)
            {
                sb.Append(" abstract");
            }

            if (md.IsVirtual)
            {
                sb.Append(" virtual");
            }

            if (md.IsFinal)
            {
                sb.Append(" final");
            }
            
            sb.Append(" ");

            sb.Append(GetTypeText(md.ReturnType));
            sb.Append(" ");
            sb.Append(md.Name);

            if (md.HasGenericParameters)
            {
                GetGenericParametersText(sb, md.GenericParameters);
            }            

            sb.Append("(");
            for (int i = 0; i < md.Parameters.Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                if (i == sentinel)
                    sb.Append("..., ");

                sb.Append(GetTypeText(md.Parameters[i].ParameterType));
                sb.Append(" ");
                sb.Append(md.Parameters[i].Name);
            }
            sb.Append(")");
            return sb.ToString();
        }

        public static string GetInstructionText(Collection<Instruction> ic, int insIndex)
        {
            return GetInstructionText(ic, insIndex, true);
        }

        public static string GetInstructionText(Collection<Instruction> ic, int insIndex, bool deepCheck)
        {
            if (insIndex == -1) return "?";

            // check dead loop
            string opText;
            if (ic[insIndex].Operand is Instruction)
            {
                if (deepCheck)
                {
                    Instruction op = (Instruction)ic[insIndex].Operand;
                    opText = GetInstructionText(ic, ic.IndexOf(op), false);
                }
                else
                {
                    opText = insIndex.ToString();
                }
            }
            else
            {
                opText = InsUtils.GetOperandText(ic, insIndex);
            }
            return String.Format("{0} -> {1} {2}", insIndex, ic[insIndex].OpCode.Name, opText);
        }

        public static string GetAddressText(Instruction ins)
        {
            int offset = ins.Offset;
            if (offset == 0 && ins.Previous != null)
            {
                Instruction i = ins.Previous;
                while (i.Offset == 0 && i.Previous != null) i = i.Previous;

                offset = i.Offset;
                while (i != ins)
                {
                    offset += i.GetSize();
                    i = i.Next;
                }
            }
            return String.Format("L_{0:x04}", offset);
        }

        public static string GetOperandText(Collection<Instruction> ic, int insIndex)
        {
            string text = String.Empty;
            if (insIndex < 0) return text;

            object operand = ic[insIndex].Operand;
            if (operand == null)
            {
                return text;
            }

            switch (operand.GetType().FullName)
            {
                case "System.String":
                    text = String.Format("\"{0}\"", operand);
                    break;
                case "System.Int32":
                case "System.Int16":
                case "System.Int64":
                    long l = Convert.ToInt64(operand);
                    if (l < 100)
                        text = l.ToString();
                    else
                        text = String.Format("0x{0:x}", l);
                    break;
                case "System.UInt32":
                case "System.UInt16":
                case "System.UInt64":
                    ulong ul = Convert.ToUInt64(operand);
                    if (ul < 100)
                        text = ul.ToString();
                    else
                        text = String.Format("0x{0:x}", ul);
                    break;
                case "System.Decimal":
                    text = operand.ToString();
                    break;
                case "System.Double":
                    text = operand.ToString();
                    break;
                case "System.Byte":
                case "System.SByte":
                    text = String.Format("0x{0:x}", operand);
                    break;
                case "System.Boolean":
                    text = operand.ToString();
                    break;
                case "System.Char":
                    text = String.Format("'{0}'", operand);
                    break;
                case "System.DateTime":
                    text = operand.ToString();
                    break;
                case "Mono.Cecil.Cil.Instruction":
                    //text = GetAddressText(operand as Instruction);
                    text = InsUtils.GetInstructionText(ic, ic.IndexOf(operand as Instruction));
                    break;
                case "Mono.Cecil.Cil.Instruction[]":
                    Instruction[] ins = operand as Instruction[];
                    StringBuilder sb = new StringBuilder();
                    if (ins.Length > 0)
                    {
                        for (int i = 0; i < ins.Length; i++)
                        {
                            //sb.AppendFormat("{0}, ", GetAddressText(ins[i]));
                            sb.AppendFormat("{0}, ", ic.IndexOf(ins[i]));
                        }
                        sb.Remove(sb.Length - 2, 2);
                    }
                    text = sb.ToString();
                    break;
                //case "Mono.Cecil.MethodDefinition":
                //case "Mono.Cecil.MethodReference":
                //    text = (operand as MethodReference).Name;
                //    break;
                //case "Mono.Cecil.FieldDefinition":
                //case "Mono.Cecil.FieldReference":
                //    text = (operand as FieldReference).Name;
                //    break;
                default:
                    text = operand.ToString();
                    break;
            }

            return text;
        }

        public static string GetMemberText(MemberReference mr)
        {
            if (mr is MethodDefinition)
                return GetMethodText((MethodDefinition)mr);
            else if (mr is PropertyDefinition)
                return GetPropertyText((PropertyDefinition)mr);
            else if (mr is FieldDefinition)
                return GetFieldText((FieldDefinition)mr);
            else if (mr is EventDefinition)
                return GetEventText((EventDefinition)mr);
            else if (mr is TypeReference)
                return GetTypeText((TypeReference)mr);
            else return mr.Name;
        }

        public static string GetPropertyText(PropertyDefinition pd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(pd.Name);
            if (pd.HasParameters)
            {
                sb.Append("[");
                Collection<ParameterDefinition> parameters = pd.Parameters;
                for (int i = 0; i < parameters.Count; i++)
                {
                    sb.Append(parameters[i].ParameterType.Name);
                    if (i < parameters.Count - 1)
                        sb.Append(",");
                }
                sb.Append("]");
            }
            sb.Append(": ");
            sb.Append(GetTypeText(pd.PropertyType));
            return sb.ToString();
        }

        public static string GetFieldText(FieldDefinition fd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(fd.Name);
            sb.Append(": ");
            sb.Append(GetTypeText(fd.FieldType));
            if ((fd.Attributes & Mono.Cecil.FieldAttributes.HasFieldRVA) != 0)
            {
                sb.AppendFormat(" at D_{0:x08}", fd.RVA);
            }
            return sb.ToString();
        }

        public static string GetEventText(EventDefinition ed)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ed.Name);
            sb.Append(": ");
            sb.Append(GetTypeText(ed.EventType));
            return sb.ToString();
        }

        public static string GetTypeText(TypeReference tr)
        {
            StringBuilder sb = new StringBuilder();
            if (tr.HasGenericParameters || tr is IGenericInstance)
            {
                int p = tr.Name.LastIndexOf("`");
                if (p>0)
                    sb.Append(tr.Name.Substring(0, p));
                else
                    sb.Append(tr.Name);

                if (tr.HasGenericParameters)
                {
                    GetGenericParametersText(sb, tr.GenericParameters);
                }
                else
                {
                    IGenericInstance gi = (IGenericInstance)tr;
                    if (gi.HasGenericArguments)
                    {
                        GetGenericArgumentsText(sb, gi.GenericArguments);
                    }
                }
            }
            else
            {
                sb.Append(tr.Name);
            }
            return sb.ToString();
        }
        #endregion Get ... Text

        public static void ComputeIndexes(Collection<Instruction> ic)
        {            
            for (int i = 0; i < ic.Count; i++)
            {
                ic[i].Index = i;
            }
        }

        public static void ComputeOffsets(Collection<Instruction> ic)
        {
            int offset = 0;

            foreach (Instruction i in ic)
            {
                i.Offset = offset;
                offset += i.GetSize();
            }

            ComputeIndexes(ic);
        }

        public static void Simplify(Collection<Instruction> ic)
        {
            foreach (Instruction i in ic)
            {
                if (i.OpCode.OperandType != OperandType.ShortInlineBrTarget)
                    continue;

                switch (i.OpCode.Code)
                {
                    case Code.Br_S:
                        i.OpCode = OpCodes.Br;
                        break;
                    case Code.Brfalse_S:
                        i.OpCode = OpCodes.Brfalse;
                        break;
                    case Code.Brtrue_S:
                        i.OpCode = OpCodes.Brtrue;
                        break;
                    case Code.Beq_S:
                        i.OpCode = OpCodes.Beq;
                        break;
                    case Code.Bge_S:
                        i.OpCode = OpCodes.Bge;
                        break;
                    case Code.Bgt_S:
                        i.OpCode = OpCodes.Bgt;
                        break;
                    case Code.Ble_S:
                        i.OpCode = OpCodes.Ble;
                        break;
                    case Code.Blt_S:
                        i.OpCode = OpCodes.Blt;
                        break;
                    case Code.Bne_Un_S:
                        i.OpCode = OpCodes.Bne_Un;
                        break;
                    case Code.Bge_Un_S:
                        i.OpCode = OpCodes.Bge_Un;
                        break;
                    case Code.Bgt_Un_S:
                        i.OpCode = OpCodes.Bgt_Un;
                        break;
                    case Code.Ble_Un_S:
                        i.OpCode = OpCodes.Ble_Un;
                        break;
                    case Code.Blt_Un_S:
                        i.OpCode = OpCodes.Blt_Un;
                        break;
                    case Code.Leave_S:
                        i.OpCode = OpCodes.Leave;
                        break;
                }
            }

            ComputeOffsets(ic);
        }

        #region OldName
        public static void RemoveOldMemberName(MemberReference mr)
        {
            if (mr == null)
                return;
            mr.OriginalName = null;
        }

        public static void RemoveOldFullTypeName(MemberReference mr)
        {
            if (mr == null)
                return;
            mr.OriginalFullTypeName = null;
        }

        public static void SaveOldMemberName(MemberReference mr)
        {
            if (mr == null || OldMemberNameExists(mr))
                return;
            mr.OriginalName = mr.Name;
        }

        public static void SaveOldFullTypeName(MemberReference mr)
        {
            if (mr == null || OldFullTypeNameExists(mr))
                return;

            mr.OriginalName = mr.Name;            
            if (mr is TypeReference)
            {
                mr.OriginalFullTypeName = mr.FullName;
                mr.OriginalNamespace = ((TypeReference)mr).Namespace;
            }
            else
            {
                mr.OriginalFullTypeName = mr.DeclaringType.FullName;
                mr.OriginalNamespace = mr.DeclaringType.Namespace;
            }
        }

        public static bool OldMemberNameExists(MemberReference mr)
        {
            return mr != null && !String.IsNullOrEmpty(mr.OriginalName);
        }

        public static bool OldFullTypeNameExists(MemberReference mr)
        {
            return mr != null && !String.IsNullOrEmpty(mr.OriginalFullTypeName);
        }

        public static string GetOldMemberName(MemberReference mr)
        {
            if (mr == null)
                return null;

            MemberReference tmp;
            if (mr is TypeSpecification)
            {
                tmp = ((TypeSpecification)mr).ElementType;
            }
            else
            {
                tmp = mr;
            }
            if (tmp == null)
                return null;

            if(OldMemberNameExists(tmp))
            {
                return tmp.OriginalName;
            }
            else
            {
                return tmp.Name;
            }
        }

        public static string GetOldFullTypeName(TypeReference typeRef)
        {
            TypeReference tr = typeRef;
            if (tr == null)
                return null;

            if (OldFullTypeNameExists(tr))
            {
                return tr.OriginalFullTypeName;
            }

            if (tr is TypeSpecification)
            {
                TypeReference tr1 = ((TypeSpecification)tr).ElementType;
                if (tr1 == null)
                    return null;

                if (OldFullTypeNameExists(tr1))
                {
                    return tr1.OriginalFullTypeName;
                }
                else
                {
                    return tr1.FullName;
                }
            }
            
            return tr.FullName;
        }
        #endregion OldName

        public static Instruction CreateInstruction(OpCode opcode, object operand)
        {
            return (Instruction)Activator.CreateInstance(typeof(Instruction), BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, new object[] { opcode, operand }, null);
            
            //Type t = typeof(ILProcessor);
            //return t.InvokeMember("FinalCreate",
            //    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static |
            //    System.Reflection.BindingFlags.InvokeMethod,
            //    null,
            //    null,
            //    new object[] { opcode, operand }) as Instruction;
        }

        public static void InitInstructionsCombobox(ComboBox cbo, MethodDefinition method, Instruction selectedIns, ref string[] instructionsStr)
        {
            cbo.Enabled = true;
            cbo.Items.Clear();
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;

            Collection<Instruction> ic = method.Body.Instructions;
            if (instructionsStr == null)
            {
                instructionsStr = new string[ic.Count + 1];
                instructionsStr[0] = "N/A";
                for (int i = 1; i < instructionsStr.Length; i++)
                {
                    instructionsStr[i] = InsUtils.GetInstructionText(ic, i - 1);
                }
            }

            cbo.Items.AddRange(instructionsStr);

            if (cbo.Items.Count > 0 && selectedIns != null)
            {
                cbo.SelectedIndex = ic.IndexOf(selectedIns) + 1;
            }
            else
            {
                cbo.SelectedIndex = 0;
            }
        }
       

        public static bool ResolveCustomAttribute(CustomAttribute ca)
        {
            return ca.Constructor.Parameters.Count == ca.ConstructorArguments.Count;
        }

        public static bool IsInvalidInstructionType1(Instruction ins)
        {
            if (ins == null || ins.OpCode.Code == Code.Unused)
                return true;
            if (ins.OpCode.OperandType != OperandType.InlineNone && ins.Operand == null)
                return true;
            if (ins.OpCode.Code == Code.Switch)
            {
                Instruction[] ops = ins.Operand as Instruction[];
                if (ops == null || ops.Length == 0)
                    return true;
            }
            return false;
        }

        public static bool IsInvalidInstructionType2(Instruction ins, MethodDefinition method)
        {
            string opName = ins.OpCode.Name;
            if (opName.StartsWith("ldloc") || opName.StartsWith("stloc"))
            {
                int varIndex = GetVariableIndex(ins);
                if (varIndex < 0 || varIndex >= method.Body.Variables.Count)
                    return true;
            }
            if (opName.StartsWith("ldarg") || opName.StartsWith("starg"))
            {
                int argIndex = GetArgIndex(ins);
                if (method.IsStatic)
                {
                    if (argIndex < 0 || argIndex >= method.Parameters.Count)
                        return true;
                }
                else
                {
                    if (argIndex < 0 || argIndex > method.Parameters.Count)
                        return true;
                }
            }
            return false;
        }

        public static bool IsValidInstruction(Instruction ins, MethodDefinition method)
        {
            if (IsInvalidInstructionType1(ins))
                return false;
            if (IsInvalidInstructionType2(ins, method))
                return false;
            return true;
        }

        public static int GetVariableIndex(Instruction ins)
        {
            int varIndex = -1;
            if (ins == null)
                return varIndex;
            string opName = ins.OpCode.Name;
            if(!opName.StartsWith("ldloc") && !opName.StartsWith("stloc"))
                return varIndex;
            VariableDefinition vd = ins.Operand as VariableDefinition;
            if (vd == null)
            {
                int p = opName.LastIndexOf('.');
                if (p >= 0)
                {
                    varIndex = Convert.ToInt32(opName.Substring(p+1));
                }
            }
            else
            {
                varIndex = vd.Index;
            }
            return varIndex;
        }

        public static int GetArgIndex(Instruction ins)
        {
            int argIndex = -1;
            if (ins == null)
                return argIndex;
            string opName = ins.OpCode.Name;
            if (!opName.StartsWith("ldarg") && !opName.StartsWith("starg"))
                return argIndex;

            ParameterDefinition pd = ins.Operand as ParameterDefinition;
            if (pd == null)
            {
                int p = opName.LastIndexOf('.');
                if (p >= 0)
                {
                    argIndex = Convert.ToInt32(opName.Substring(p + 1));
                }
            }
            else
            {
                argIndex = pd.Index;
            }
            return argIndex;
        }

        public static List<Instruction> FindVariableUsage(VariableDefinition vd, MethodDefinition method)
        {
            List<Instruction> list = new List<Instruction>();
            if (!method.HasBody) return list;
            Collection<Instruction> ic = method.Body.Instructions;
            for(int i=0; i<ic.Count; i++)
            {
                Instruction ins = ic[i];
                int varIndex = GetVariableIndex(ins);
                if (varIndex < 0) continue;
                if (varIndex == vd.Index)
                    list.Add(ins);
            }
            return list;
        }
    }//end of class
}
