
namespace SimpleAssemblyExplorer.LutzReflector
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text;
	using Reflector.CodeModel;
    using SimpleAssemblyExplorer;

	public class RichTextFormatter : IFormatter
	{
        private RichTextWriter writer;
		private bool allowProperties = false;

		public override string ToString()
		{
            return writer.ToString();
		}
	
		public RichTextFormatter(string language)
		{
            writer = new RichTextWriter(language);
		}	

        private void WriteMetadataItem(string text, IMetadataItem mi)
        {
            //this.writer.Write(String.Format(@"{{\rtf1 {0}\v #0x{1:x08}\v0}}", text, mi.Token), false);
            this.writer.WriteLink(text, String.Format("0x{0:x08}", mi.Token));
        }

        public void WriteReference(string text, string toolTip, Object reference)
        {
            this.writer.ApplyIndent();

            bool handled = false;
            if (reference is ITypeDeclaration)
            {
                ITypeReference tr = (ITypeReference)reference;
                if (!Utils.IsSystemType(tr.Namespace))
                {
                    WriteMetadataItem(text, (IMetadataItem)reference);
                    handled = true;
                }
            }
            else if (reference is IMemberReference && reference is IMetadataItem)
            {
                IMemberReference mr = (IMemberReference)reference;
                ITypeReference tr = (ITypeReference)mr.DeclaringType;
                if (tr == null || (tr != null && !Utils.IsSystemType(tr.Namespace)))
                {
                    WriteMetadataItem(text, (IMetadataItem)reference);
                    handled = true;
                }
            }
            if (!handled)
            {
                this.writer.WriteColor(text, 0x006018);
            }
        }

		public void WriteProperty(string propertyName, string propertyValue)
		{
            //if (this.allowProperties)
            //{
            //    throw new NotSupportedException();
            //}
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

        #region IFormatter Members

        public void Write(string value)
        {
            this.writer.Write(value);
        }

        public void WriteComment(string value)
        {
            this.writer.WriteComment(value);
        }

        public void WriteDeclaration(string value, object target)
        {
            this.writer.WriteDeclaration(value, target);
        }

        public void WriteDeclaration(string value)
        {
            this.writer.WriteDeclaration(value);
        }

        public void WriteIndent()
        {
            this.writer.WriteIndent();
        }

        public void WriteKeyword(string value)
        {
            this.writer.WriteKeyword(value);
        }

        public void WriteLine()
        {
            this.writer.WriteLine();
        }

        public void WriteLiteral(string value)
        {
            this.writer.WriteLiteral(value);
        }

        public void WriteOutdent()
        {
            this.writer.WriteOutdent();
        }

        #endregion
    }//end of class
}