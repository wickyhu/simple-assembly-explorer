// ---------------------------------------------------------
// Lutz Roeder's .NET Reflector
// Copyright (c) 2000-2005 Lutz Roeder. All rights reserved.
// http://www.aisto.com/roeder
// ---------------------------------------------------------
namespace SAE.FileDisassembler
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.IO;
	using Reflector.CodeModel;
	
	public class TextFormatter : IFormatter
	{
		private StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
		private bool allowProperties = false;
		private bool newLine = false;
		private int indent = 0;

		public override string ToString()
		{
			return this.writer.ToString();
		}
	
		public void Write(string text)
		{
			this.ApplyIndent();
			this.writer.Write(text);
		}
	
		public void WriteDeclaration(string text)
		{
			this.WriteBold(text);	
		}

        public void WriteDeclaration(string text, object target)
        {
            this.WriteBold(text);
        }

        public void WriteComment(string text)
		{
			this.WriteColor(text, (int) 0x808080);	
		}
		
		public void WriteLiteral(string text)
		{
			this.WriteColor(text, (int) 0x800000);
		}
		
		public void WriteKeyword(string text)
		{
			this.WriteColor(text, (int) 0x000080);
		}
	
		public void WriteIndent()
		{
			this.indent++;
		}
				
		public void WriteLine()
		{
			this.writer.WriteLine();
			this.newLine = true;
		}

		public void WriteOutdent()
		{
			this.indent--;
		}

		public void WriteReference(string text, string toolTip, Object reference)
		{
			this.ApplyIndent();
			this.writer.Write(text);
		}

		public void WriteProperty(string propertyName, string propertyValue)
		{
			if (this.allowProperties)
			{
				throw new NotSupportedException();
			}
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

		private void WriteBold(string text)
		{
			this.ApplyIndent();
			this.writer.Write(text);
		}

		private void WriteColor(string text, int color)
		{
			this.ApplyIndent();
			this.writer.Write(text);
		}

		private void ApplyIndent()
		{
			if (this.newLine)
			{
				for (int i = 0; i < this.indent; i++)
				{
					this.writer.Write("    ");
				}

				this.newLine = false;
			}
		}
	}
}