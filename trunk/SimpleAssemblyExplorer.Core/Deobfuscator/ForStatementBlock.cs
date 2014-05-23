using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace SimpleAssemblyExplorer
{
    public class ForStatementBlock
    {
        public int VarIndex { get; private set; }
        public int StartIndex { get; private set; }
        public int EndIndex { get; private set; }

        public ForStatementBlock(int varIndex, int startIndex, int endIndex) 
        {
            VarIndex = varIndex;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}", StartIndex, EndIndex);
        }    

        public static List<ForStatementBlock> Find(MethodDefinition method)
        {
            List<ForStatementBlock> list = new List<ForStatementBlock>();

            if (method == null || !method.HasBody)
                return list;

            Collection<Instruction> ic = method.Body.Instructions;
            if (ic.Count < 1)
                return list;

            InsUtils.ComputeIndexes(ic);

            for (int i = 0; i < ic.Count; i++)
            {
                const int maxLoop = 50;
                int count = 0;

                Instruction insLdci4 = ic[i];
                
                if (!insLdci4.OpCode.Name.StartsWith("ldc.i4"))
                    continue;

                Instruction insStloc = insLdci4.Next;
                count = 0;
                while (insStloc != null && !insStloc.OpCode.Name.StartsWith("stloc")
                    && count < maxLoop)
                {
                    insStloc = insStloc.Next;
                    count++;
                }
                if (insStloc == null || !insStloc.OpCode.Name.StartsWith("stloc"))
                    continue;

                Instruction insBr = insStloc.Next;
                if (insBr == null || !insBr.OpCode.Name.StartsWith("br") || insBr.Next == null)
                    continue;

                Instruction insLdloc = insBr.Operand as Instruction;
                if (insLdloc == null || !insLdloc.OpCode.Name.StartsWith("ldloc"))
                    continue;

                Instruction insBlt = insLdloc.Next;
                count = 0;
                while (insBlt != null && !insBlt.OpCode.Name.StartsWith("blt")
                    && !insBlt.OpCode.Name.StartsWith("clt")
                    && !insBlt.OpCode.Name.StartsWith("call")
                    && count < maxLoop)
                {
                    insBlt = insBlt.Next;
                    count++;
                }
                if (insBlt == null)
                    continue;
                
                bool skip = false;
                if (insBlt.OpCode.Name.StartsWith("blt"))
                {
                    //ok
                }
                else if (insBlt.OpCode.Name.StartsWith("clt"))
                {
                    if (insBlt.Next != null && insBlt.Next.OpCode.Name.StartsWith("brtrue"))
                        insBlt = insBlt.Next;
                    else
                        skip = true;
                }
                else if (insBlt.OpCode.Name.StartsWith("call"))
                {
                    MethodReference mr = insBlt.Operand as MethodReference;
                    if (mr.FullName == "System.Boolean System.Collections.IEnumerator::MoveNext()")
                    {
                        if (insBlt.Next != null && insBlt.Next.OpCode.Name.StartsWith("brtrue"))
                            insBlt = insBlt.Next;
                        else
                            skip = true;
                    }
                    else
                    {
                        skip = true;
                    }
                }
                else
                {
                    skip = true;
                }
                if (skip) continue;

                Instruction insBrNext = insBlt.Operand as Instruction;
                if (insBrNext == null || insBrNext.Offset != insBr.Next.Offset)
                    continue;

                int varIndex = InsUtils.GetVariableIndex(insStloc);
                if (varIndex < 0) continue;
                ForStatementBlock fsb = new ForStatementBlock(varIndex, insLdci4.Index, insBlt.Index);
                list.Add(fsb);
                i = insBlt.Index;
            }

            return list;
        }
    }//end of class
}
