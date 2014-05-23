// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.VB;
using ICSharpCode.NRefactory.VB.Ast;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.VB
{
	/// <summary>
	/// Description of VBTextOutputFormatter.
	/// </summary>
	public class VBTextOutputFormatter : IOutputFormatter
	{
		readonly RtfTextOutput output;
		readonly Stack<AstNode> nodeStack = new Stack<AstNode>();
		
		public VBTextOutputFormatter(ITextOutput output)
		{
			if (output == null)
				throw new ArgumentNullException("output");
            this.output = (RtfTextOutput)output;
		}		
	
		
		public void WriteIdentifier(string identifier)
		{
			var definition = GetCurrentDefinition();
			if (definition != null) {
				output.WriteDefinition(identifier, definition);
				return;
			}
			
			object memberRef = GetCurrentMemberReference();
			if (memberRef != null) {
				output.WriteReference(identifier, memberRef);
				return;
			}

			definition = GetCurrentLocalDefinition();
			if (definition != null) {
				output.WriteDefinition(identifier, definition);
				return;
			}

			memberRef = GetCurrentLocalReference();
			if (memberRef != null) {
				output.WriteReference(identifier, memberRef, true);
				return;
			}

			output.Write(identifier);
		}

		MemberReference GetCurrentMemberReference()
		{
			AstNode node = nodeStack.Peek();
			MemberReference memberRef = node.Annotation<MemberReference>();
			if (memberRef == null && node.Role == AstNode.Roles.TargetExpression && (node.Parent is InvocationExpression || node.Parent is ObjectCreationExpression)) {
				memberRef = node.Parent.Annotation<MemberReference>();
			}
			return memberRef;
		}

		object GetCurrentLocalReference()
		{
			AstNode node = nodeStack.Peek();
			ILVariable variable = node.Annotation<ILVariable>();
			if (variable != null) {
				if (variable.OriginalParameter != null)
					return variable.OriginalParameter;
				//if (variable.OriginalVariable != null)
				//    return variable.OriginalVariable;
				return variable;
			}
			return null;
		}

		object GetCurrentLocalDefinition()
		{
			AstNode node = nodeStack.Peek();
			var parameterDef = node.Annotation<ParameterDefinition>();
			if (parameterDef != null)
				return parameterDef;

			if (node is VariableInitializer || node is CatchBlock || node is ForEachStatement) {
				var variable = node.Annotation<ILVariable>();
				if (variable != null) {
					if (variable.OriginalParameter != null)
						return variable.OriginalParameter;
					//if (variable.OriginalVariable != null)
					//    return variable.OriginalVariable;
					return variable;
				} else {

				}
			}

			return null;
		}
		
		object GetCurrentDefinition()
		{
			if (nodeStack == null || nodeStack.Count == 0)
				return null;
			
			var node = nodeStack.Peek();			
			if (IsDefinition(node))
				return node.Annotation<MemberReference>();
			
			node = node.Parent;
			if (IsDefinition(node))
				return node.Annotation<MemberReference>();

			return null;
		}
		
		public void WriteKeyword(string keyword)
		{
            output.WriteColor(keyword, (int)0x80);
		}
		
		public void WriteToken(string token)
		{
            if (IsValue(token))
            {
                output.WriteColor(token, (int)0x800000);
                return;
            }

			// Attach member reference to token only if there's no identifier in the current node.
            //MemberReference memberRef = GetCurrentMemberReference();
            //if (memberRef != null && nodeStack.Peek().GetChildByRole(AstNode.Roles.Identifier).IsNull)
            //    output.WriteReference(token, memberRef);
            //else
				output.Write(token);
		}

        public bool IsValue(string text)
        {
            if (text.StartsWith("\"") && text.EndsWith("\""))
                return true;
            if (text.StartsWith("'") && text.EndsWith("'"))
                return true;
            if (text == "." || text == "," || text == "E" || text == "-")
                return false;
            if (System.Text.RegularExpressions.Regex.IsMatch(text, "^([0-9,.E-]+)([mfuL]|uL){0,1}$"))
                return true;
            return false;
        }

		public void Space()
		{
			output.Write(' ');
		}
		
		public void Indent()
		{
			output.Indent();
		}
		
		public void Unindent()
		{
			output.Unindent();
		}
		
		public void NewLine()
		{
			output.WriteLine();
		}
		
		public void WriteComment(bool isDocumentation, string content)
		{
            int color = (int)0x808080;
			if (isDocumentation)
			{
				output.WriteColor("'''", color);
			}
			else
			{
				output.WriteColor("'", color);
			}
            output.WriteColor(content, color);
			output.WriteLine();
		}
		
		public void StartNode(AstNode node)
		{
			nodeStack.Push(node);
		}
		
		public void EndNode(AstNode node)
		{
			if (nodeStack.Pop() != node)
				throw new InvalidOperationException();
		}

        public void MarkFoldStart()
        {
        }

        public void MarkFoldEnd()
        {
        }
        private static bool IsDefinition(AstNode node)
		{
			return
				node is FieldDeclaration ||
				node is ConstructorDeclaration ||
				node is EventDeclaration ||
				node is DelegateDeclaration ||
				node is OperatorDeclaration||
				node is MemberDeclaration ||
				node is TypeDeclaration;
		}
	}
}
