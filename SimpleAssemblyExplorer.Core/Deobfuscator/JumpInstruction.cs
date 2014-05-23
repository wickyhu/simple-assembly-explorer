using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace SimpleAssemblyExplorer
{
    public class JumpInstruction
    {
        public Instruction Instruction { get; private set; }
        public List<Instruction> Targets { get; private set; }

        public JumpInstruction(Instruction insJump)
        {
            Instruction = insJump;
            Targets = new List<Instruction>();
            if (insJump.Operand is Instruction)
            {
                Instruction insTarget = (Instruction)insJump.Operand;
                while (DeobfUtils.IsDirectJumpInstruction(insTarget) || DeobfUtils.IsDirectLeaveInstruction(insTarget))
                {
                    insTarget = (Instruction)insTarget.Operand;
                }
                Targets.Add(insTarget);
            }
            else if (insJump.Operand is Instruction[])
            {
                Instruction[] ops = (Instruction[])insJump.Operand;
                for (int i = 0; i < ops.Length; i++)
                {
                    Instruction insTarget = ops[i];
                    if (insTarget == null)
                        continue;
                    while (DeobfUtils.IsDirectJumpInstruction(insTarget) || DeobfUtils.IsDirectLeaveInstruction(insTarget))
                    {
                        insTarget = (Instruction)insTarget.Operand;
                    }
                    Targets.Add(insTarget);
                }
            }
        }

        public bool IsTarget(Instruction ins)
        {
            if (ins == null || ins.Index < 0)
                return false;

            return Targets.Contains(ins);
        }

        public static List<JumpInstruction> Find(MethodDefinition method, int insStart, int insEnd)
        {
            List<JumpInstruction> list = new List<JumpInstruction>();
            Collection<Instruction> ic = method.Body.Instructions;
            if (insStart < 0) insStart = 0;
            if (insEnd < 0) insEnd = ic.Count - 1;

            InsUtils.ComputeIndexes(ic);

            for (int i = insStart; i <= insEnd; i++)
            {
                Instruction insTemp = ic[i];
                if (DeobfUtils.IsJumpInstruction(insTemp))
                {
                    list.Add(new JumpInstruction(insTemp));
                }
            }
            return list;
        }

        public static void Remove(List<JumpInstruction> insList, Instruction ins)
        {
            if (ins == null || ins.Index < 0)
                return;
            
            for(int i=0; i<insList.Count; i++)
            {
                JumpInstruction jiTmp = insList[i];
                if (jiTmp.Instruction.Index == ins.Index)
                {
                    insList.RemoveAt(i);
                    break;
                }
            }

            return;
        }

    }
}
