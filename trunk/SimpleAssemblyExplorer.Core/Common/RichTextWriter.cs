
namespace SimpleAssemblyExplorer
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text;

	public class RichTextWriter
	{
        private TextWriter formatter = new StringWriter();
		private int indent = 0;
		private ArrayList colors = new ArrayList();
		private bool newLine = false;
		private string _language;       

		public override string ToString()
		{
			StringWriter sw = new StringWriter();

			sw.Write(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033");
			sw.Write(@"{\colortbl ");
			
			foreach (int color in this.colors)
			{
				sw.Write("\\red");
				sw.Write(((color >> 16) & 0xff).ToString());
				sw.Write("\\green");
				sw.Write(((color >> 8) & 0xff).ToString());
				sw.Write("\\blue");
				sw.Write((color & 0xff).ToString());
				sw.Write(";");
			}
			 
			sw.Write("}");
			sw.Write(@"\par\li150");
			sw.Write(@"\cf0");
			sw.Write(" ");
			sw.Write(formatter.ToString());
			sw.Write(@"\par");
			sw.Write("}");

			return sw.ToString();
		}
	
		public RichTextWriter(string language)
		{
			_language = language;
			this.colors.Add(0x000000);	
		}
	
		public void Write(string text)
		{
            Write(text, true);
		}

        public void Write(string text, bool quoteText)
        {
            this.ApplyIndent();
            if (quoteText)
            {
                this.WriteQuote(text);
            }
            else
            {
                this.formatter.Write(text);
            }
        }

        public void WriteLink(string text, string value)
        {
            this.Write(String.Format(@"{{\rtf1 {0}\v #{1}\v0}}", Encode(text), Encode(value)), false);
        }

		public void WriteDeclaration(string text)
		{
			this.formatter.Write(@"\b ");
			this.Write(text);
			this.formatter.Write(@"\b0 ");
		}

		public void WriteDeclaration(string text, object target)
		{
			this.formatter.Write(@"\b ");
			this.Write(text);
			this.formatter.Write(@"\b0 ");
		}

		public void WriteKeyword(string text)
		{
			this.WriteColor(text, (int) 0x80);
		}

		public void WriteComment(string text)
		{
			this.WriteColor(text, (int) 0x808080);	
		}

		public void WriteLiteral(string text)
		{
			string str = text;
			string start = "\"";
			string prefix = "\\u";
			string end = "\"";

			switch (_language)
			{
				case "Delphi":
					start = "'";
					prefix = "#$";
					end = "'";
					break;
				case "MC++":
					start = "S\"";
					break;
			}

			if (text.StartsWith(start + prefix) && text.EndsWith(end))
			{
				try
				{
					str = String.Format("{0}{1}{2}",
						start,
						SimpleUtils.SimpleParse.ParseUnicodeString(text.Substring(start.Length, text.Length - start.Length - end.Length), prefix, 4),
						end
						);
				}
				catch
				{
				}    
			}
			this.WriteColor(str, (int)0x800000);	
		}

		public void WriteIndent()
		{
			this.indent++;
		}
				
		public void WriteLine()
		{
			this.formatter.Write(@"\par\li150");
			this.newLine = true;
		}

		public void WriteOutdent()
		{
			this.indent--;
		}
		
		public void WriteQuote(string text)
		{         
			this.formatter.Write(Encode(text));
		}

		public void WriteColor(string text, int color)
		{
			this.ApplyIndent();

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

		public void ApplyIndent()
		{
			if (this.newLine)
			{
				for (int i = 0; i < this.indent; i++)
				{
					this.WriteQuote("    ");
				}
				
				this.newLine = false;
			}
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
 
	}//end of class
}