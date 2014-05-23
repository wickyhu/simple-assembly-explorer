using System;
using System.Collections.Generic;
using System.Text;

using Reflector;
using Reflector.CodeModel;
using Reflector.CodeModel.Memory;

namespace SimpleAssemblyExplorer.LutzReflector
{
    public class MethodDeclarationInfo
    {
        public IMethodDeclaration MethodDeclaration { get; set; }
        public string Body { get; set; }

        public MethodDeclarationInfo(IMethodDeclaration methodDeclaration, string body)
        {
            this.MethodDeclaration = methodDeclaration;
            this.Body = body;
        }

        public string Name
        {
            get
            {
                return this.MethodDeclaration == null ? String.Empty :
                    String.Format("{0} (0x{1:x08})", this.MethodDeclaration.ToString(), this.MethodDeclaration.Token);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

    } //end of class

}
