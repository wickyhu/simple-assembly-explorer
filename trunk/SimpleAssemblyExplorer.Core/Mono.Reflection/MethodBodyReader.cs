//
// MethodBodyReader.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2009 - 2010 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Mono.Reflection {

	class MethodBodyReader {

		static OpCode [] one_byte_opcodes;
		static OpCode [] two_bytes_opcodes;

        //wicky.patch.start
        ITokenResolver tokenResolver;
        //wicly.patch.end

		static MethodBodyReader ()
		{
			one_byte_opcodes = new OpCode [0xe1];
			two_bytes_opcodes = new OpCode [0x1f];

			var fields = typeof (OpCodes).GetFields (
				BindingFlags.Public | BindingFlags.Static);

			for (int i = 0; i < fields.Length; i++) {
				var opcode = (OpCode) fields [i].GetValue (null);
				if (opcode.OpCodeType == OpCodeType.Nternal)
					continue;

				if (opcode.Size == 1)
					one_byte_opcodes [opcode.Value] = opcode;
				else
					two_bytes_opcodes [opcode.Value & 0xff] = opcode;
			}

		}

		MethodBase method;
		MethodBody body;
		Module module;
		Type [] type_arguments;
		Type [] method_arguments;
		ByteBuffer il;
		ParameterInfo [] parameters;
		IList<LocalVariableInfo> locals;
		List<Instruction> instructions = new List<Instruction> ();

		MethodBodyReader (MethodBase method)
		{
            //wicky.patch.start: check DynamicMethod
            byte[] bytes;
            if (DynamicMethodHelper.IsDynamicOrRTDynamicMethod(method))
            {
                DynamicMethod dynamicMethod = DynamicMethodHelper.GetDynamicMethod(method);
                this.method = dynamicMethod;
                bytes = DynamicMethodHelper.GetILAsByteArray(dynamicMethod);
                if (bytes == null) 
                    bytes = new byte[0];                
                if (dynamicMethod.DeclaringType != null)
                    type_arguments = dynamicMethod.DeclaringType.GetGenericArguments();
                this.parameters = dynamicMethod.GetParameters();                
                this.module = dynamicMethod.Module;
                //this.locals = ??
                this.il = new ByteBuffer(bytes);
                this.tokenResolver = new DynamicScopeTokenResolver(dynamicMethod);
            }
            else
            {
                this.method = method;
                this.body = method.GetMethodBody();
                if (this.body == null)
                    bytes = new byte[0];
                else
                    bytes = body.GetILAsByteArray();
                if (bytes == null)
                    bytes = new byte[0];

                bytes = body.GetILAsByteArray();

                if (!(method is ConstructorInfo))
                    method_arguments = method.GetGenericArguments();

                if (method.DeclaringType != null)
                    type_arguments = method.DeclaringType.GetGenericArguments();

                this.parameters = method.GetParameters();
                this.locals = body.LocalVariables;
                this.module = method.Module;
                this.il = new ByteBuffer(bytes);
                this.tokenResolver = new ModuleScopeTokenResolver(method);
            }

            /*
            this.method = method;

			this.body = method.GetMethodBody ();
			if (this.body == null)
				throw new ArgumentException ("Method has no body");

			var bytes = body.GetILAsByteArray ();
			if (bytes == null)
				throw new ArgumentException ("Can not get the body of the method");

			if (!(method is ConstructorInfo))
				method_arguments = method.GetGenericArguments ();

			if (method.DeclaringType != null)
				type_arguments = method.DeclaringType.GetGenericArguments ();

			this.parameters = method.GetParameters ();
			this.locals = body.LocalVariables;
			this.module = method.Module;
			this.il = new ByteBuffer (bytes);
            */

            //wicky.patch.end
        }

		void ReadInstructions ()
		{
			Instruction previous = null;

			while (il.position < il.buffer.Length) {
				var instruction = new Instruction (il.position, ReadOpCode ());

				ReadOperand (instruction);

				if (previous != null) {
					instruction.Previous = previous;
					previous.Next = instruction;
				}

				instructions.Add (instruction);
				previous = instruction;
			}
		}

        void ReadOperand(Instruction instruction)
        {
            switch (instruction.OpCode.OperandType)
            {
                case OperandType.InlineNone:
                    break;
                case OperandType.InlineSwitch:
                    int length = il.ReadInt32();
                    int base_offset = il.position + (4 * length);
                    int[] branches = new int[length];
                    for (int i = 0; i < length; i++)
                        branches[i] = il.ReadInt32() + base_offset;

                    instruction.Operand = branches;
                    break;
                case OperandType.ShortInlineBrTarget:
                    instruction.Operand = (sbyte)(((sbyte)il.ReadByte()) + il.position);
                    break;
                case OperandType.InlineBrTarget:
                    instruction.Operand = il.ReadInt32() + il.position;
                    break;
                case OperandType.ShortInlineI:
                    if (instruction.OpCode == OpCodes.Ldc_I4_S)
                        instruction.Operand = (sbyte)il.ReadByte();
                    else
                        instruction.Operand = il.ReadByte();
                    break;
                case OperandType.InlineI:
                    instruction.Operand = il.ReadInt32();
                    break;
                case OperandType.ShortInlineR:
                    instruction.Operand = il.ReadSingle();
                    break;
                case OperandType.InlineR:
                    instruction.Operand = il.ReadDouble();
                    break;
                case OperandType.InlineI8:
                    instruction.Operand = il.ReadInt64();
                    break;
                case OperandType.InlineSig:
                    //wicky.patch.start
                    //instruction.Operand = module.ResolveSignature(il.ReadInt32());
                    instruction.Operand = tokenResolver.ResolveSignature(il.ReadInt32());
                    //wicky.patch.end
                    break;
                case OperandType.InlineString:
                    //wicky.patch.start
                    //instruction.Operand = module.ResolveString(il.ReadInt32());
                    instruction.Operand = tokenResolver.ResolveString(il.ReadInt32());
                    //wicky.patch.end
                    break;
                //wicky.patch.start
                    /*
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.InlineMethod:
                case OperandType.InlineField:
                    instruction.Operand = module.ResolveMember(il.ReadInt32(), type_arguments, method_arguments);
                    break;
                     */
                case OperandType.InlineTok:
                    instruction.Operand = tokenResolver.ResolveMember(il.ReadInt32());
                    break;
                case OperandType.InlineType:
                    instruction.Operand = tokenResolver.ResolveType(il.ReadInt32());
                    break;
                case OperandType.InlineMethod:
                    instruction.Operand = tokenResolver.ResolveMethod(il.ReadInt32());
                    break;
                case OperandType.InlineField:
                    instruction.Operand = tokenResolver.ResolveField(il.ReadInt32());
                    break;
                //wicky.patch.end
                case OperandType.ShortInlineVar:
                    instruction.Operand = GetVariable(instruction, il.ReadByte());
                    break;
                case OperandType.InlineVar:
                    instruction.Operand = GetVariable(instruction, il.ReadInt16());
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

		object GetVariable (Instruction instruction, int index)
		{
			if (TargetsLocalVariable (instruction.OpCode))
				return GetLocalVariable (index);

			return GetParameter (index);
		}

		static bool TargetsLocalVariable (OpCode opcode)
		{
			return opcode.Name.Contains ("loc");
		}

		LocalVariableInfo GetLocalVariable (int index)
		{
            //wicky.patch.start
            if (locals == null || index >= locals.Count)
                return null;
            //wicky.patch.end
			return locals [index];
		}

		ParameterInfo GetParameter (int index)
		{
			if (!method.IsStatic)
				index--;

			return parameters [index];
		}

		OpCode ReadOpCode ()
		{
			byte op = il.ReadByte ();
			return op != 0xfe
				? one_byte_opcodes [op]
				: two_bytes_opcodes [il.ReadByte ()];
		}

		public static List<Instruction> GetInstructions (MethodBase method)
		{
			var reader = new MethodBodyReader (method);
			reader.ReadInstructions ();
			return reader.instructions;
		}
        
	}
}
