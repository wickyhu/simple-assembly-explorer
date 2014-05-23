
namespace SimpleAssemblyExplorer.LutzReflector
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text;
	using Reflector.CodeModel;

	public class TextFormatter : IFormatter
	{
		private StringWriter formatter = new StringWriter();
		private bool allowProperties = false;
		private int indent = 0;
		private bool newLine = false;
        private string _language;

		public override string ToString()
		{
			//StringWriter writer = new StringWriter();
            //writer.Write(formatter.ToString());
			//return writer.ToString();
            return formatter.ToString();
		}
	
		public TextFormatter(string language)
		{
            _language = language;
		}
	
		public void Write(string text)
		{
			this.ApplyIndent();
			this.WriteQuote(text);
		}

		public void WriteDeclaration(string text)
		{
            this.Write(text);
		}

		public void WriteDeclaration(string text, object target)
		{
            this.Write(text);
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
            this.formatter.Write("\r\n");
			this.newLine = true;
		}

		public void WriteOutdent()
		{
			this.indent--;
		}

		public void WriteReference(string text, string toolTip, Object reference)
		{
			this.ApplyIndent();
			this.WriteColor(text, 0x006018);			
		}

		public void WriteProperty(string propertyName, string propertyValue)
		{
            //if (this.allowProperties)
            //{
            //    throw new NotSupportedException();
            //}
		}
		
		public void WriteQuote(string text)
		{         
			this.formatter.Write(Encode(text));
		}

		public bool AllowProperties
		{
			set 
			{ 
				this.allowProperties = value; 
			}
			
			get 
			{ 
				return this.allowProperties;
			}
		}

		private void WriteColor(string text, int color)
		{
			this.ApplyIndent();
			this.WriteQuote(text);
		}

		private void ApplyIndent()
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
                        /*
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
                         */
                        writer.Write(ch);

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
