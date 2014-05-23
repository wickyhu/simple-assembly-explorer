using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace SimpleAssemblyExplorer
{
    public class InstructionBlock
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public InstructionBlock NextBlock { get; private set; }

        public int JumpDownRefCount { get; private set; }
        public int JumpUpRefCount { get; private set; }
        public int RefCount { get { return JumpDownRefCount + JumpUpRefCount; } }

        public InstructionBlock(int startIndex, int endIndex)
        {
            StartIndex = startIndex;

            if (endIndex < startIndex)
                EndIndex = startIndex;
            else
                EndIndex = endIndex;
        }

        public int Size
        {
            get { return EndIndex - StartIndex + 1; }
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}", StartIndex, EndIndex);
        }

        public static List<InstructionBlock> Find(MethodDefinition method)
        {
            List<InstructionBlock> list = new List<InstructionBlock>();

            if (method == null || !method.HasBody)
                return list;

            Collection<Instruction> ic = method.Body.Instructions;
            if (ic.Count < 1)
                return list;

            int firstIndex = 0;
            int lastIndex = firstIndex;

            Instruction insLast = ic[lastIndex];
            while (lastIndex < ic.Count)
            {
                if (DeobfUtils.IsBlockDelimiter(insLast) || lastIndex + 1 >= ic.Count)
                {
                    InstructionBlock ib = new InstructionBlock(firstIndex, lastIndex);
                    list.Add(ib);

                    firstIndex = lastIndex + 1;
                    lastIndex = firstIndex;
                }
                else
                {
                    lastIndex++;
                }
                if (lastIndex >= ic.Count) 
                    break;
                insLast = ic[lastIndex];
            }

            int insCount = 0;
            foreach (InstructionBlock ib in list)
            {
                insCount += ib.EndIndex - ib.StartIndex + 1;

                Instruction insTo = null;
                if (ic[ib.EndIndex].Operand is Instruction)
                {
                    insTo = ic[ib.EndIndex].Operand as Instruction;
                }
                else
                {
                    int prev = ib.EndIndex - 1;
                    if (prev >= ib.StartIndex && ic[prev].Operand is Instruction)
                    {
                        insTo = ic[ib.EndIndex].Operand as Instruction;
                    }
                }
                if (insTo != null)
                {
                    int to = ic.IndexOf(insTo);
                    foreach (InstructionBlock ib2 in list)
                    {
                        if (ib2.StartIndex == ib.StartIndex)
                            continue;
                        if (ib2.StartIndex <= to && to <= ib2.EndIndex)
                        {
                            ib.NextBlock = ib2;

                            if (ib.StartIndex < ib2.StartIndex)
                                ib2.JumpDownRefCount++;
                            else
                                ib2.JumpUpRefCount++;
                        }
                    }
                }
            }

            if (insCount != ic.Count)
            {
                throw new ApplicationException("internal error in InstructionBlock.Find.");
            }

            return list;
        }

    } //end of class
}