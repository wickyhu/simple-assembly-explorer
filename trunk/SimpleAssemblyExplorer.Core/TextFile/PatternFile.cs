using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Mono.Cecil.Metadata;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace SimpleAssemblyExplorer
{
    public class PatternFile : TextFile
    {
        static PatternFile _default;
        public static PatternFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new PatternFile("Pattern.txt"), null);
                }
                return _default;
            }
        }

        public PatternFile(string fileName)
            : base(fileName)
        {
        }

        List<Pattern> _dummyCodes;
        public List<Pattern> DummyCodes
        {
            get
            {
                ReadFile();
                return _dummyCodes;
            }
        }

        const string NOP_INVALID_POP = "NOP.INVALID.POP";
        bool _nopInvalidPop = false;
        public bool NopInvalidPop
        {
            get { return _nopInvalidPop; }
            set { _nopInvalidPop = value; }
        }

        public override void Clear()
        {
            Clear(_dummyCodes);
            _nopInvalidPop = false;
            base.Clear();
        }

        public override void ParseLines()
        {
            LoadPatterns();           
        }

        private void LoadPatterns()
        {
            _dummyCodes = new List<Pattern>();

            foreach (string line in this.Lines)
            {
                if (line.Equals(NOP_INVALID_POP, StringComparison.OrdinalIgnoreCase))
                {
                    _nopInvalidPop = true;
                    continue;
                }

                string[] opCodeGroupStr = line.Split(new char[] { ';' });
                OpCodeGroup[] opCodeGroups = new OpCodeGroup[opCodeGroupStr.Length];
                
                bool checkReference = false;

                for (int i = 0; i < opCodeGroupStr.Length; i++)
                {
                    string str = opCodeGroupStr[i];
                    opCodeGroups[i] = new OpCodeGroup(str);

                    if (str.StartsWith("~"))
                    {
                        checkReference = true;
                        str = str.Substring(1);
                    }

                    if (str.StartsWith("*"))
                    {
                        opCodeGroups[i].ExactMatch = true;
                        str = str.Substring(1);
                    }
                    else
                    {
                        opCodeGroups[i].ExactMatch = false;
                    }

                    if (str.StartsWith("!"))
                    {
                        opCodeGroups[i].ToNop = false;
                        str = str.Substring(1);
                    }
                    else
                    {
                        opCodeGroups[i].ToNop = true;
                    }

                    //if (str.StartsWith("~"))
                    //{
                    //    opCodeGroups[i].CheckReference = true;
                    //    str = str.Substring(1);
                    //}
                    //else
                    //{
                    //    opCodeGroups[i].CheckReference = false;
                    //}

                    if (str.StartsWith("[") && str.EndsWith("]"))
                    {
                        opCodeGroups[i].Optional = true;
                        opCodeGroups[i].OpCodes = str.Substring(1, str.Length - 2).Split(new char[] { ',' });
                    }
                    else
                    {
                        opCodeGroups[i].Optional = false;
                        int p = str.IndexOf(":");
                        if (p > 0)
                        {
                            opCodeGroups[i].Expression = str.Substring(p + 1);
                            str = str.Substring(0, p);
                        }
                        else
                        {
                            opCodeGroups[i].Expression = null;
                        }
                        opCodeGroups[i].OpCodes = str.Split(new char[] { ',' });
                    }

                }

                _dummyCodes.Add(new Pattern(line, opCodeGroups, checkReference));
            }
        }

    } //end of PatternFile

    public class Pattern
    {
        public OpCodeGroup[] OpCodeGroups;
        public int MaxLength;
        public int MinLength;
        public string Text;
        public bool CheckReference = false;

        public Pattern(string text, OpCodeGroup[] opCodeGroups, bool checkReference)
        {
            Text = text;
            CheckReference = checkReference;

            if (opCodeGroups == null || opCodeGroups.Length < 1)
            {
                throw new ApplicationException("Invalid Pattern: " + (text == null ? String.Empty : text));
            }

            OpCodeGroups = opCodeGroups;
            MaxLength = opCodeGroups.Length;
            MinLength = MaxLength;
            for (int i = 0; i < MaxLength; i++)
            {
                if (opCodeGroups[i].Optional)
                    MinLength--;
            }
        }

        public override string ToString()
        {
            return Text;
        }

    }//end of class Pattern
   
    public class OpCodeGroup
    {
        public bool Optional = false;
        public string[] OpCodes;
        public bool ToNop = true;
        public string Expression;
        public bool ExactMatch = false;
        public string Text;

        public OpCodeGroup(string text)
        {
            Text = text;
        }

        public bool HasExpression
        {
            get { return !String.IsNullOrEmpty(Expression); }
        }

        public bool EvalExpression(Instruction ins, Collection<Instruction> instructions)
        {
            object op = InsUtils.GetInstructionOperand(ins);
            if (op is Instruction)
            {
                int insIndex1 = instructions.IndexOf(ins);
                int insIndex2 = instructions.IndexOf((Instruction)op);
                return Expression.Equals((insIndex2 - insIndex1).ToString());
            }
            if (Expression.IndexOf("op") >= 0)
            {
                string expr = Expression.Replace("op", op == null ? "null" : op.ToString());
                object o = Evaluator.Eval(expr);
                return Convert.ToBoolean(o);
            }
            return false;
        }

        public override string ToString()
        {
            return Text;
        }

    }//end of class OpCodeGroup

}
