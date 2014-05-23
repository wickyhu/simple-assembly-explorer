
using System;
using System.Collections;
using System.IO;
using ICSharpCode.NRefactory;
using Mono.Cecil;

namespace ICSharpCode.Decompiler
{
	public class RtfTextOutput : ITextOutput
	{
		readonly TextWriter formatter;
		int indent;
		bool needsIndent;
        int line = 1;
        int column = 1;
        ArrayList colors = new ArrayList();

		public RtfTextOutput(TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			this.formatter = writer;
            this.colors.Add(0x000000);
            CurrentLine = 1;
		}

        public RtfTextOutput() : this(new StringWriter())
		{
		}

		public override string ToString()
		{
            StringWriter writer = new StringWriter();

            writer.Write(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033");
            writer.Write(@"{\colortbl ");

            foreach (int color in this.colors)
            {
                writer.Write("\\red");
                writer.Write(((color >> 16) & 0xff).ToString());
                writer.Write("\\green");
                writer.Write(((color >> 8) & 0xff).ToString());
                writer.Write("\\blue");
                writer.Write((color & 0xff).ToString());
                writer.Write(";");
            }

            writer.Write("}");
            writer.Write(@"\par\li150");
            writer.Write(@"\cf0");
            writer.Write(" ");
            writer.Write(formatter.ToString());
            writer.Write(@"\par");
            writer.Write("}");

            return writer.ToString();
		}

        public int CurrentLine { get; set; }
        public int CurrentColumn { get; set; }

		public void Indent()
		{
			indent++;
		}
		
		public void Unindent()
		{
			indent--;
		}
		
		void WriteIndent()
		{
			if (needsIndent) {
				needsIndent = false;
				for (int i = 0; i < indent; i++) {
                    this.WriteQuote("    ");
				}
			}
		}
		
		public void Write(char ch)
		{
			WriteIndent();
            WriteQuote(ch.ToString());
		}
		
		public void Write(string text)
		{
			WriteIndent();
            this.WriteQuote(text);
		}
		
		public void WriteLine()
		{
            this.formatter.Write(@"\par\li150");
            needsIndent = true;
            ++CurrentLine;
		}
		
        public void WriteDefinition(string text, object definition, bool isLocal = false)
        {
            this.formatter.Write(@"\b ");
            this.Write(text);
            this.formatter.Write(@"\b0 ");            
        }

        public void WriteReference(string text, object reference, bool isLocal = false)
        {
            this.WriteIndent();

            bool handled = false;
            if (reference is TypeReference)
            {
                TypeReference tr = (TypeReference)reference;
                if (!IsSystemType(tr.FullName))
                {
                    if (!(tr is TypeDefinition))
                    {
                        try
                        {
                            tr = tr.Resolve();
                        }
                        catch { }
                    }
                    if (tr is TypeDefinition)
                    {
                        WriteMetadataItem(text, (IMetadataTokenProvider)reference);
                        handled = true;
                    }
                }
            }
            else if (reference is MemberReference)
            {
                MemberReference mr = (MemberReference)reference;
                TypeReference tr = (TypeReference)mr.DeclaringType;
                if (tr == null || (tr != null && !IsSystemType(tr.FullName)))
                {
                    WriteMetadataItem(text, mr);
                    handled = true;
                }
            }
            if (!handled)
            {
                this.WriteColor(text, 0x006018);
            }
		}

        public void WriteColor(string text, int color)
        {
            this.WriteIndent();

            int index = this.colors.IndexOf(color);
            if (index == -1)
            {
                index = this.colors.Count;
                this.colors.Add(color);
            }

            this.formatter.Write("\\cf" + index.ToString() + " ");
            this.WriteQuote(text);
            this.formatter.Write("\\cf0 ");


        }
        
        private void WriteMetadataItem(string text, IMetadataTokenProvider mi)
        {
            this.formatter.Write(String.Format(@"{{\rtf1 {0}\v #0x{1:x08}\v0}}", Encode(text), mi.MetadataToken.ToUInt32()));
        }	

        public void WriteQuote(string text)
        {
            this.formatter.Write(Encode(text));
        }

        public string Encode(string text)
        {
            string str = text;
            StringWriter writer = new StringWriter();
            CharEnumerator enumerator;
            char ch;

            try
            {
                enumerator = str.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ch = enumerator.Current;
                    ushort num = ch;

                    if (num <= 0xff)
                    {
                        switch (ch)
                        {
                            case '\\':
                                writer.Write("\\\\");
                                break;
                            case '}':
                                writer.Write("\\}");
                                break;
                            case '{':
                                writer.Write("\\{");
                                break;
                            default:
                                writer.Write(ch);
                                break;
                        }
                    }
                    else
                    {
                        writer.Write(String.Format(@"\u{0:d}?", num));
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Dispose();
                }
            }

            str = writer.ToString();
            return str;
        }
       
        public bool IsSystemType(string typeFullName)
        {
            if (String.IsNullOrEmpty(typeFullName))
                return false;
            string name;
            if (typeFullName.EndsWith("."))
                name = typeFullName;
            else
                name = String.Format("{0}.", typeFullName);
            return name.StartsWith("System.") || name.StartsWith("Microsoft.");
        }

        public void MarkFoldStart(string collapsedText = "...", bool defaultCollapsed = false)
        {
        }

        public void MarkFoldEnd()
        {
        }

        public void AddDebuggerMemberMapping(MemberMapping memberMapping)
        {
        }

        public TextLocation Location
        {
            get
            {
                return new TextLocation(line, column + (needsIndent ? indent : 0));
            }
        }

	}
}
