using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Security;
using System.Diagnostics;
using System.Xml;
using SimpleAssemblyExplorer;
using SimpleAssemblyExplorer.Plugin;
using SimpleAssemblyExplorer.LutzReflector;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
	public partial class Deobfuscator : IDeobfuscator
	{
		List<DeobfError> errors = null;
		public List<DeobfError> Errors
		{
			get { return errors; }
		}

		Dictionary<string, AssemblyDefinition> allAssemblies = null;
		Dictionary<string, string> allFiles = null;
		Dictionary<string, int> completedAssemblies = null;
		Dictionary<string, List<TypeDefinition>> derivedTypes = null;

		Dictionary<int, string> propertyNames = null;
		Dictionary<int, string> methodNames = null;
		Dictionary<int, string> eventNames = null;
		Dictionary<int, string> fieldNames = null;

		Dictionary<string, uint> memberCount = null;

		Dictionary<string, int> typesCompleted = new Dictionary<string, int>();
		Dictionary<string, string> typesWaiting = new Dictionary<string, string>();
		Dictionary<string, List<TypeDefinition>> typesInCustomAttribute = new Dictionary<string, List<TypeDefinition>>();
		
		AssemblyDefinition currentAssembly = null;
		ModuleDefinition currentModule = null;

		DeobfOptions _options;
		public DeobfOptions Options
		{
			get { return _options; }
		}

		public Deobfuscator()
			: this(new DeobfOptions())
		{
		}

		public Deobfuscator(DeobfOptions options)
		{
			_options = options;
		}

		public void LoadAllAssemblies()
		{
			allAssemblies = new Dictionary<string, AssemblyDefinition>();
			allFiles = new Dictionary<string, string>();
			_options.OutputFiles = new string[_options.Rows.Length];

			for (int i = 0; i < _options.Rows.Length; i++)
			{
				if (_options.IsCancelPending) break;

                string sourceFile = _options.Rows[i];
                string file = Path.Combine(_options.SourceDir, sourceFile);
                if (!File.Exists(file))
                {
                    file = sourceFile;
                }
				_options.OutputFiles[i] = GetOutputFile(file);
				_options.AppendTextInfo(String.Format("Loading : {0}\r\n", file));
                
				AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(file);

				string assemblyName = ad.Name.FullName;

				if (allAssemblies.ContainsKey(assemblyName))
				{
					throw new ApplicationException("Can't handle two same assemblies tegother: " + assemblyName);
				}

				SaveOldName(ad);
				allAssemblies.Add(assemblyName, ad);
				allFiles.Add(assemblyName, file);
			}

			_options.AppendTextInfo("\r\n");
		}

		public void Go()
		{
			bool resolveDirAdded = false;
			try
			{
				InitPlugin();

				resolveDirAdded = _options.Host.AddAssemblyResolveDir(_options.SourceDir);

				_options.SetTextInfo(String.Format("=== Started at {0} ===\r\n\r\n", DateTime.Now));

				errors = new List<DeobfError>();

				derivedTypes = new Dictionary<string, List<TypeDefinition>>();
				propertyNames = new Dictionary<int, string>();
				methodNames = new Dictionary<int, string>();
				eventNames = new Dictionary<int, string>();
				fieldNames = new Dictionary<int, string>();
				memberCount = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);

				LoadAllAssemblies();

				int count = 0;
				completedAssemblies = new Dictionary<string, int>();
				while (count < allAssemblies.Count)
				{
					if (_options.IsCancelPending) break;

					foreach (string fullName in allAssemblies.Keys)
					{
						if (_options.IsCancelPending) break;

						if (completedAssemblies.ContainsKey(fullName)) continue;

						AssemblyDefinition ad = allAssemblies[fullName];

						bool allOK = true;
						foreach (ModuleDefinition md in ad.Modules)
						{
							foreach (AssemblyNameReference anr in md.AssemblyReferences)
							{
								if (!allAssemblies.ContainsKey(anr.FullName)) 
									continue;
								if (anr.FullName == fullName) //self reference?
									continue;
								if (!completedAssemblies.ContainsKey(anr.FullName))
								{
									allOK = false;
									break;
								}
							}
							if (!allOK) break;
						}

						if (_options.IsCancelPending) break;

						if (allOK)
						{
							string file = allFiles[fullName];
							_options.Host.SetStatusText(file, true);

							_options.AppendTextInfo(String.Format("Deobfuscating: {0}\r\n", file));

							Deobf(ad, file);

							completedAssemblies.Add(fullName, 1);
							count++;
						}
					}
				}

				allAssemblies.Clear();
				allAssemblies = null;
				allFiles.Clear();
				allFiles = null;

				derivedTypes.Clear();
				derivedTypes = null;

				completedAssemblies.Clear();
				completedAssemblies = null;

				propertyNames.Clear();
				propertyNames = null;
				methodNames.Clear();
				methodNames = null;
				eventNames.Clear();
				eventNames = null;
				fieldNames.Clear();
				fieldNames = null;

				memberCount.Clear();
				memberCount = null;

				if (_options.IsCancelPending)
				{
					_options.AppendTextInfo("\r\nUser breaked.\r\n");
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				if (resolveDirAdded)
					_options.Host.RemoveAssemblyResolveDir(_options.SourceDir);

				_options.AppendTextInfo(String.Format("\r\n=== Completed at {0} ===\r\n\r\n", DateTime.Now));

				_options.Host.ResetProgress();
				_options.Host.SetStatusText(null);
			}

		}

		private void SaveOldName(AssemblyDefinition ad)
		{
			foreach (ModuleDefinition module in ad.Modules)
			{
				foreach (TypeDefinition td in module.AllTypes)
				{
					InsUtils.SaveOldFullTypeName(td);
					
					//it seems GetTypeReferences doesn't return all type references?
					//foreach (FieldDefinition fd in td.Fields)
					//{
					//    InsUtils.SaveOldFullTypeName(fd.FieldType);
					//}
					//foreach (PropertyDefinition pd in td.Properties)
					//{
					//    InsUtils.SaveOldFullTypeName(pd.PropertyType);
					//}
					//foreach (EventDefinition ed in td.Events)
					//{
					//    InsUtils.SaveOldFullTypeName(ed.EventType);
					//}
					//foreach (MethodDefinition md in td.Methods)
					//{
					//    foreach (ParameterDefinition pd in md.Parameters)
					//    {
					//        InsUtils.SaveOldFullTypeName(pd.ParameterType);
					//    }
					//}
				}
				foreach (TypeReference tf in module.GetTypeReferences())
				{
					if (tf != null)
					{
						InsUtils.SaveOldFullTypeName(tf);
					}
				}
				foreach (MemberReference mr in module.GetMemberReferences())
				{
					if (mr != null)
					{
						InsUtils.SaveOldFullTypeName(mr);
					}
				}

				//test 
				//foreach (TypeDefinition td in module.AllTypes)
				//{
				//    if (td.MetadataToken.RID == 0x33e)
				//    {
				//        foreach (MethodDefinition md in td.Methods)
				//        {
				//            if (md.HasParameters)
				//            {
				//                foreach (ParameterDefinition pd in md.Parameters)
				//                {
				//                    if (!InsUtils.OldFullTypeNameExists(pd.ParameterType))
				//                    {
				//                    }
				//                }
				//            }
				//        }
				//    }
				//}//end for test

			}
		}

		private string GetOutputFile(string file)
		{
			string outputDir = _options.OutputDir;
			string outputFile;

			if (_options.SourceDir == outputDir || 
				Path.GetDirectoryName(file) == outputDir 
				)
			{
				outputFile = String.Format("{0}.Deobf{1}", Path.GetFileNameWithoutExtension(file), Path.GetExtension(file));
			}
			else
			{
				outputFile = Path.GetFileName(file);
			}

			if (!String.IsNullOrEmpty(_options.OutputSuffix))
			{
				outputFile = String.Format("{0}.{1}{2}", Path.GetFileNameWithoutExtension(outputFile), _options.OutputSuffix, Path.GetExtension(outputFile));
			}

			outputFile = Path.Combine(outputDir, outputFile);
			return outputFile;
		}


		private void DeobfNamespaces(AssemblyDefinition ad)
		{
			if (IsDeobfName())
			{
				_options.Host.SetStatusText("Renaming Name Space ...");

				//string asmName = ad.Name.Name;
				Dictionary<string, string> nameSpaces = new Dictionary<string, string>();
				int nameSpaceCount = 1;

				foreach (ModuleDefinition module in ad.Modules)
				{
					foreach (TypeDefinition type in module.AllTypes)
					{
						if (String.IsNullOrEmpty(type.Namespace)) continue;
						if (type.Namespace == "<Module>") continue;

						if (!IsValidName(type.Namespace))
						{
							string oldName = type.Namespace;

							if (nameSpaces.ContainsKey(oldName))
							{
								type.Namespace = (string)nameSpaces[oldName];
							}
							else
							{
								//string newNameSpace = String.Format("{0}.NS{1:d3}", asmName, nameSpaceCount);
								string newNameSpace = String.Format("NS{0:d3}", nameSpaceCount);
								nameSpaceCount++;
								nameSpaces.Add(oldName, newNameSpace);
								type.Namespace = newNameSpace;
							}
						}
					}
				}
				//handle resources for namespaces
				foreach (string key in nameSpaces.Keys)
				{
					MatchResourceName(ad, key, nameSpaces[key]);
				}
			}
		}

		private void Deobf(AssemblyDefinition assemblyDef, string file)
		{
			string outputDir = _options.OutputDir;
			AssemblyDefinition ad = assemblyDef;
			
			currentAssembly = assemblyDef;

			string adName = ad.Name.Name;

			string outputFile = GetOutputFile(file);            

			RemoveAttribute(ad);

			if (!IsDeobfName() && 
                !IsDeobfString() && !IsDeobfFlowAfterString() && 
                !IsDeobfFlow() && !IsDeobfOther())
			{
				goto exit;
			}

			CallPluginBeforeHandler(ad);

			DeobfNamespaces(ad);
			TryRestoreEntryPointMethodName(ad);

			typesCompleted.Clear();
			typesWaiting.Clear();
			typesInCustomAttribute.Clear();

			//handle types            
			foreach (ModuleDefinition module in ad.Modules)
			{
				if (_options.IsCancelPending) break;

				currentModule = module;

				CallPluginBeforeHandler(module);

				RemoveAttribute(module);

				#region search for types
				_options.Host.SetStatusText("Searching for types ...");

				foreach (TypeDefinition td in module.AllTypes)
				{
					if (_options.IsCancelPending) break;

					//if (td.IsSpecialName || td.IsRuntimeSpecialName) continue;
					string tokenString = TokenUtils.GetMetadataTokenString(td.MetadataToken);
					if(typesWaiting.ContainsKey(tokenString)) continue;
					typesWaiting.Add(tokenString, td.FullName);
				}
				#endregion search for types

				#region handle types
				_options.Host.InitProgress(0, typesWaiting.Count);
				_options.Host.SetProgress(0);
				int passes = 1;

				while (typesWaiting.Count > 0)
				{
					if (_options.IsCancelPending) break;

					int count = 0;
					foreach (TypeDefinition td in module.AllTypes)
					{
						if (_options.IsCancelPending) break;

						//if (td.IsSpecialName || td.IsRuntimeSpecialName) continue;

						string typeToken = TokenUtils.GetMetadataTokenString(td.MetadataToken);
						if (typesCompleted.ContainsKey(typeToken)) continue;

						if (_options.IgnoredTypeFile.IsMatch(td))
						{
							_options.Host.SetStatusText(String.Format("Ignored {0}: {1} ...", adName, InsUtils.GetOldFullTypeName(td)));
							typesCompleted.Add(typeToken, 1);
							typesWaiting.Remove(typeToken);
							_options.Host.SetProgress(typesCompleted.Count);
							count++;
							continue;
						}

						bool ok = true;

						//ensure base type is handled at first
						if (ok) ok = IsBaseTypeHandled(td);
						//ensure interface is handled at first
						if (ok) ok = IsInterfaceHandled(td);
						
						//ensure generic type is handled at first 
						if (passes <= 2)
						{
							if (ok) ok = IsGenericParameterHandled(td);
						}

						//ensure type used in custom attribute is handled at first
						if (passes <= 1)
						{
							if (ok) ok = IsCustomAttributeHandled(td);
						}

						if (_options.IsCancelPending) break;

						//finally ok
						if (ok)
						{
							HandleType(td, file);

							typesCompleted.Add(typeToken, 1);
							typesWaiting.Remove(typeToken);
							_options.Host.SetProgress(typesCompleted.Count);
							count++;
						}

					}

					//to avoid bug and dead loop
					if (count == 0 && typesWaiting.Count > 0)
					{
						if (passes <= 2)
						{
							passes++;
							continue;
						}

						List<string> notHandledTypes = new List<string>(typesWaiting.Count);
						foreach (string key in typesWaiting.Keys)
						{
							notHandledTypes.Add(String.Format("{0} ({1})", typesWaiting[key], key));
						}
						errors.Add(new DeobfError(module, notHandledTypes));
					
						break;
					}
				}

				_options.Host.ResetProgress();
				typesWaiting.Clear();
				typesCompleted.Clear();
				typesInCustomAttribute.Clear();

				#endregion handle types

				#region handle type references
				if (!_options.IsCancelPending)
				{
					_options.Host.SetStatusText("Handling type references ...");
					foreach (TypeReference tr in module.GetTypeReferences())
					{
						if (_options.IsCancelPending) break;
						FixTypeReference(tr);
					}

					//foreach (TypeDefinition td in module.AllTypes)
					//{
					//    if (_options.IsCancelPending) break;
					//    //it seems GetTypeReferences doesn't return all type references?
					//    foreach (FieldDefinition fd in td.Fields)
					//    {
					//        FixTypeReference(fd.FieldType);
					//    }
					//    if (_options.IsCancelPending) break;
					//    foreach (PropertyDefinition pd in td.Properties)
					//    {
					//        FixTypeReference(pd.PropertyType);
					//    }
					//    if (_options.IsCancelPending) break;
					//    foreach (EventDefinition ed in td.Events)
					//    {
					//        FixTypeReference(ed.EventType);
					//    }
					//    if (_options.IsCancelPending) break;
					//    foreach (MethodDefinition md in td.Methods)
					//    {
					//        if (_options.IsCancelPending) break;
					//        foreach (ParameterDefinition pd in md.Parameters)
					//        {
					//            FixTypeReference(pd.ParameterType);
					//        }
					//    }
					//}

				}
				#endregion handle type references

				#region handle member references
				if (!_options.IsCancelPending)
				{
					_options.Host.SetStatusText("Handling member references ...");
					foreach (MemberReference mbr in module.GetMemberReferences())
					{
						if (_options.IsCancelPending) break;
						FixMemberReference(mbr);
					}
				}
				#endregion handle member references

				CallPluginAfterHandler(module);

				currentModule = null;
			}//end of each module

			CallPluginAfterHandler(ad);

			currentAssembly = null;

		exit:
			if (!_options.IsCancelPending)
			{
				try
				{
					ad.Write(outputFile);
				}
				catch
				{
					try
					{
						File.Delete(outputFile);
					}
					catch { }
					throw;
				}
			}
			_options.Host.SetStatusText(null);
			_options.Host.ResetProgress();
		}

		private string GetNewName(string prefix)
		{
			uint count;
			if (memberCount.TryGetValue(prefix, out count))
				count++;
			else
				count = 1;
			memberCount[prefix] = count;
			string format;
			format = "{0}{1:x06}";
			return String.Format(format, prefix, count);
		}

		private string GetNewName(MemberReference mr, Dictionary<int, string> names, string prefix)
		{
			int key = DeobfUtils.GetKey(mr);
			string newName;

			if (names.ContainsKey(key))
			{
				newName = names[key];
			}
			else
			{
				newName = GetNewName(prefix);
				names.Add(key, newName);
			}

			/*
			int fullKey = GetFullKey(td, mr, newName);
			if (allNames.ContainsKey(fullKey))
			{
				newName = GetNewName(prefix);
				fullKey = GetFullKey(td, mr, newName);
				while (allNames.ContainsKey(fullKey))
				{
					newName = GetNewName(prefix);
					fullKey = GetFullKey(td, mr, newName);
				}
			}
			allNames.Add(fullKey, 1);
			*/

			return newName;
		}

		private string GetNewPropertyName(PropertyDefinition pd)
		{
			return GetNewName(pd, propertyNames, "p");
		}

		private string GetNewEventName(EventDefinition ed)
		{
			return GetNewName(ed, eventNames, "e");
		}

		private string GetNewFieldName(FieldDefinition fd)
		{
			return GetNewName(fd, fieldNames, "f");
		}

		private string GetNewMethodName(MethodDefinition md)
		{
			return GetNewName(md, methodNames, "m");
		}

		private string GetNewMethodName()
		{
			return GetNewName("m");
		}

		private string GetNewFieldName()
		{
			return GetNewName("f");
		}

		private string GetNewPropertyName()
		{
			return GetNewName("p");
		}

        static Regex regexAsciiChar = new Regex(@"[0-9a-zA-Z_$.]");
        private string GetHexName(string name)
        {

            if (String.IsNullOrEmpty(name))
                return String.Empty;
            char[] chars = name.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                if ((ch >= '0' && ch <= '9')
                    || (ch >= 'A' && ch <= 'Z')
                    || (ch >= 'a' && ch <= 'z')
                    || ch == '_'
                    || ch == '$'
                    || ch == '.')
                    sb.Append(ch);
                else sb.AppendFormat(BytesUtils.BytesToHexString(Encoding.Unicode.GetBytes(ch.ToString())));
            }
            string hexName = sb.ToString();
            char startChar = hexName.Substring(0, 1).ToCharArray()[0];
            if ((startChar >= 'a' && startChar <= 'z') 
                || (startChar >= 'A' && startChar <= 'Z') 
                || (startChar == '_'))
                return hexName;
            return String.Format("x{0}", hexName);
        }

        public int TryGetArity(TypeDefinition type, out int arity)
		{
			if (type.HasGenericParameters)
			{
				string typeName = type.Name;
				var index = typeName.LastIndexOf('`');
				if (index > 0)
				{
					if (int.TryParse(typeName.Substring(index + 1), out arity))
					{
						if(arity == type.GenericParameters.Count)
							return index;
					}
				}
			}
			arity = 0;
			return -1;
		}

		public string GetNewTypeName(TypeDefinition td)
		{
			uint rid = td.MetadataToken.RID;
			string name;
			if (td.IsEnum)
				name = String.Format("enum0{0:x}", rid);
			else if (td.IsValueType)
				name = String.Format("struct0{0:x}", rid);
			else if (DeobfUtils.IsDelegate(td))
				name = String.Format("delegate0{0:x}", rid);
			else if (td.IsInterface)
				name = String.Format("i{0:x06}", rid);
			else if (DeobfUtils.IsAttribute(td))
				name = String.Format("attr0{0:x}Attribute", rid);
			else
				name = String.Format("c{0:x06}", rid);

			//need to keep generic argument suffix to let cecil works
			int arity;
			if (TryGetArity(td, out arity) >= 0)
				name = String.Format("{0}`{1}", name, arity);

			return name;
		}

		private bool IsDeobfName()
		{
			return _options.chkNonAsciiChecked || _options.chkRandomNameChecked || _options.chkRegexChecked;
		}

        private bool IsDeobfHexRename()
        {
            return _options.chkHexRenameChecked;
        }

		private bool IsDeobfString()
		{
			return _options.chkAutoStringChecked || _options.StringOptionSearchForMethod != null;
		}

		private bool IsDeobfFlow()
		{
			return _options.chkBoolFunctionChecked || _options.chkPatternChecked ||
				_options.chkBranchChecked || _options.chkCondBranchDownChecked ||
				_options.chkSwitchChecked || _options.chkUnreachableChecked ||
				_options.chkRemoveExceptionHandlerChecked ||
				_options.chkDirectCallChecked || 
                _options.chkRemoveInvalidInstructionChecked;
		}

		private bool IsDeobfFlowAfterString()
		{
			return _options.chkDelegateCallChecked;
		}

		private bool IsDeobfOther()
		{
			return _options.chkRemoveDummyMethodChecked ||
				_options.chkRemoveAttributeChecked ||
				_options.chkRemoveSealedChecked ||
				_options.chkInternalToPublicChecked ||
				_options.chkInitLocalVarsChecked ||
				IsDeobfHandlePlugin();
		}

		private bool IsDeobfHandlePlugin()
		{
			return _options.HasPlugin;
		}

		private bool IsDeobfRemoveAttribute()
		{
			return _options.chkRemoveAttributeChecked;
		}

		public int DeobfInitLocalVars(MethodDefinition method)
		{
			int initCount = 0;

			if (method == null || !method.HasBody ||
				method.Body.Instructions.Count == 0 ||
				method.Body.Variables.Count == 0)
				return initCount;

			Collection<VariableDefinition> vdc = method.Body.Variables;
			Collection<Instruction> ic = method.Body.Instructions;
			ILProcessor ilp = method.Body.GetILProcessor();
			List<ForStatementBlock> fsbList = ForStatementBlock.Find(method);

			foreach (VariableDefinition vd in vdc)
			{
				if (vd.IsPinned)
					continue;

				if (fsbList.Count > 0)
				{
					bool skip = false;
					foreach (ForStatementBlock fsb in fsbList)
					{
						if (vd.Index == fsb.VarIndex)
						{
							skip = true;
							break;
						}
					}
					if (skip) continue;

					List<Instruction> insList = InsUtils.FindVariableUsage(vd, method);
					foreach (Instruction ins in insList)
					{
						ForStatementBlock usedOnlyInFor = null;                        
						foreach (ForStatementBlock fsb in fsbList)
						{
							if (fsb.StartIndex <= ins.Index && ins.Index <= fsb.EndIndex)
							{
								if (usedOnlyInFor == null)
								{
									usedOnlyInFor = fsb;
								}
								else
								{
									if (usedOnlyInFor.StartIndex <= fsb.StartIndex && fsb.EndIndex <= usedOnlyInFor.EndIndex)
									{
									}
									else if (fsb.StartIndex <= usedOnlyInFor.StartIndex && usedOnlyInFor.EndIndex <= fsb.EndIndex)
									{
										usedOnlyInFor = fsb;
									}
									else
									{
										usedOnlyInFor = null;
										break;
									}
								}
							}                                
						}
						if (usedOnlyInFor != null)
						{
							skip = true;
							break;
						}
					}
					if (skip) continue;
				}

				initCount++;

				TypeReference tr = vd.VariableType;
				Instruction insNew;
				switch (tr.FullName)
				{
					case "System.Int32":
					case "System.Int16":
					case "System.UInt32":
					case "System.UInt16":
					case "System.Byte":
					case "System.SByte":
					case "System.Boolean":
						insNew = InsUtils.CreateInstruction(OpCodes.Stloc_S, vd);
						ilp.InsertBefore(ic[0], insNew);
						insNew = InsUtils.CreateInstruction(OpCodes.Ldc_I4_0, null);
						ilp.InsertBefore(ic[0], insNew);
						break;
					case "System.Int64":
					case "System.UInt64":
						insNew = InsUtils.CreateInstruction(OpCodes.Stloc_S, vd);
						ilp.InsertBefore(ic[0], insNew);
						insNew = InsUtils.CreateInstruction(OpCodes.Ldc_I8, (long)0);
						ilp.InsertBefore(ic[0], insNew);
						break;
					case "System.Double":
						insNew = InsUtils.CreateInstruction(OpCodes.Stloc_S, vd);
						ilp.InsertBefore(ic[0], insNew);
						insNew = InsUtils.CreateInstruction(OpCodes.Ldc_R8, (double)0);
						ilp.InsertBefore(ic[0], insNew);
						break;
					case "System.Char":
						insNew = InsUtils.CreateInstruction(OpCodes.Stloc_S, vd);
						ilp.InsertBefore(ic[0], insNew);
						insNew = InsUtils.CreateInstruction(OpCodes.Ldc_I4_S, (sbyte)0x20);
						ilp.InsertBefore(ic[0], insNew);
						break;
					case "System.Decimal":
						{
							TypeDefinition td = Resolve(tr);
							if (td == null) break;
							foreach (MethodDefinition md in td.Methods)
							{
								if (md.IsConstructor && md.Parameters.Count == 1 && md.Parameters[0].ParameterType.FullName == "System.Int32")
								{
									insNew = InsUtils.CreateInstruction(OpCodes.Stloc_S, vd);
									ilp.InsertBefore(ic[0], insNew);
									insNew = InsUtils.CreateInstruction(OpCodes.Newobj, method.Module.Import(md));
									ilp.InsertBefore(ic[0], insNew);
									insNew = InsUtils.CreateInstruction(OpCodes.Ldc_I4_0, null);
									ilp.InsertBefore(ic[0], insNew);
									break;
								}
							}
						}
						break;
					default:
						if (tr is ArrayType)
						{
							insNew = InsUtils.CreateInstruction(OpCodes.Stloc_S, vd);
							ilp.InsertBefore(ic[0], insNew);
							insNew = InsUtils.CreateInstruction(OpCodes.Ldnull, null);
							ilp.InsertBefore(ic[0], insNew);
						}
						else
						{
							TypeDefinition td = Resolve(tr);
							if (td == null) td = GetTypeDefinition(tr);
							if (td == null) break;

							if (td.IsEnum)
							{
								foreach (FieldDefinition fd in td.Fields)
								{
									if (fd.IsStatic)
									{
										insNew = InsUtils.CreateInstruction(OpCodes.Stloc_S, vd);
										ilp.InsertBefore(ic[0], insNew);
										insNew = InsUtils.CreateInstruction(OpCodes.Ldc_I4, Convert.ToInt32(fd.Constant));
										ilp.InsertBefore(ic[0], insNew);
										break;
									}
								}
							}
							else if (td.IsValueType)
							{
								insNew = InsUtils.CreateInstruction(OpCodes.Initobj, tr);
								ilp.InsertBefore(ic[0], insNew);
								insNew = InsUtils.CreateInstruction(OpCodes.Ldloca_S, vd);
								ilp.InsertBefore(ic[0], insNew);
							}
							else
							{
								insNew = InsUtils.CreateInstruction(OpCodes.Stloc_S, vd);
								ilp.InsertBefore(ic[0], insNew);
								insNew = InsUtils.CreateInstruction(OpCodes.Ldnull, null);
								ilp.InsertBefore(ic[0], insNew);
							}
						}
						break;
				}
			}

			return initCount;
		}       

		public bool IsValidName(string name)
		{
			return DeobfUtils.IsValidName(name, _options);
		}

		private bool IsTypeHandled(TypeDefinition td)
		{
			//ensure td is in currentModule
			if (allAssemblies.Count > 1)
			{
				//TODO: there maybe issues with multiple modules?
				if (currentModule != null && td.Module.FullyQualifiedName != currentModule.FullyQualifiedName)
					return true;
				//if (currentAssembly != null && td.Module.Assembly.FullName != currentAssembly.FullName)
				//    return true;
			}

			string tokenString = TokenUtils.GetMetadataTokenString(td.MetadataToken);
			return typesCompleted.ContainsKey(tokenString);
		}
		private bool IsBaseTypeHandled(TypeDefinition td)
		{
			if (td.BaseType != null)
			{
				TypeDefinition baseType = GetTypeDefinition(td.BaseType);
				if (baseType != null)
				{
					return IsTypeHandled(baseType);
				}
			}
			return true;
		}
		private bool IsInterfaceHandled(TypeDefinition td)
		{
			if (td.Interfaces.Count > 0)
			{
				for (int i = 0; i < td.Interfaces.Count; i++)
				{
					TypeDefinition ifType = GetTypeDefinition(td.Interfaces[i]);
					if (ifType != null)
					{
						if (!IsTypeHandled(ifType))
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		private bool IsGenericParameterHandled(TypeDefinition td, GenericParameter gp)
		{
			TypeDefinition gpType = GetTypeDefinition(gp);
			if (gpType != null)
			{
				if (gpType.FullName != td.FullName && !IsTypeHandled(gpType))
				{
					return false;
				}
			}
			else if (gp.HasConstraints)
			{
				foreach (TypeReference tr in gp.Constraints)
				{
					gpType = GetTypeDefinition(tr);
					if (gpType != null)
					{
						if (gpType.FullName != td.FullName && !IsTypeHandled(gpType))
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		private bool IsGenericParameterHandled(TypeDefinition td)
		{
			if (td.HasGenericParameters)
			{
				foreach (GenericParameter gp in td.GenericParameters)
				{
					if (!IsGenericParameterHandled(td, gp))
						return false;
				}
			}
			foreach (MethodDefinition method in td.Methods)
			{
				if (method.HasGenericParameters)
				{
					foreach (GenericParameter gp in method.GenericParameters)
					{
						if (!IsGenericParameterHandled(td, gp))
							return false;
					}
				}
			}
			return true;
		}
		private List<TypeDefinition> GetUsedTypeInCustomAttribute(TypeDefinition td)
		{
			List<TypeDefinition> list;
			string key = TokenUtils.GetMetadataTokenString(td.MetadataToken);
			if (typesInCustomAttribute.TryGetValue(key, out list))
				return list;

			list = new List<TypeDefinition>();
			GetUsedTypeInCustomAttribute(td, list);

			foreach (PropertyDefinition pd in td.Properties)
			{
				GetUsedTypeInCustomAttribute(pd, list);
			}
			foreach (FieldDefinition fd in td.Fields)
			{
				GetUsedTypeInCustomAttribute(fd, list);
			}
			foreach (MethodDefinition md in td.Methods)
			{
				GetUsedTypeInCustomAttribute(md, list);
			}
			typesInCustomAttribute.Add(key, list);
			return list;
		}

		private void AddUsedTypeInCustomAttributeToList(/*TypeDefinition td,*/ CustomAttributeArgument caa, List<TypeDefinition> list)
		{
			//if (caa.Type.FullName != "System.Type") return;
			if (caa.Value is TypeDefinition)
			{
				list.Add((TypeDefinition)caa.Value);
			}

			/* seems cecil 0.9 resolve all ...
			string typeName;
			if (caa.Value is TypeReference)
			{
				typeName = ((TypeReference)caa.Value).FullName;
			}
			else
			{
				typeName = caa.Value as string;
			}
			if (typeName == null) return;

			TypeDefinition type = td.Module.GetType(typeName);
			if (type != null)
			{
				list.Add(type);
			}
			*/
		}
		private void GetUsedTypeInCustomAttribute(/*TypeDefinition td,*/ Mono.Cecil.ICustomAttributeProvider cap, List<TypeDefinition> list)
		{
			if (cap.HasCustomAttributes)
			{
				foreach (CustomAttribute ca in cap.CustomAttributes)
				{
					//if (!ca.IsResolvable) continue;
					try
					{
						InsUtils.ResolveCustomAttribute(ca);
					}
					catch (Exception ex)
					{
						_options.AppendTextInfoLine(ex.Message);
						continue;
					}

					if (ca.HasConstructorArguments && ca.Constructor.HasParameters && ca.Constructor.Parameters.Count == ca.ConstructorArguments.Count)
					{
						for (int i = 0; i < ca.Constructor.Parameters.Count; i++)
						{
							ParameterDefinition pd = ca.Constructor.Parameters[i];
							string typeName = pd.ParameterType.FullName;
							if (typeName == "System.Type")
							{
								CustomAttributeArgument caa = ca.ConstructorArguments[i];
								AddUsedTypeInCustomAttributeToList(caa, list);
							}
							else if (typeName == "System.Object")
							{
								CustomAttributeArgument caa = ca.ConstructorArguments[i];
								if (caa.Value is CustomAttributeArgument)
									caa = (CustomAttributeArgument)caa.Value;

								if (caa.Type.FullName == "System.Type" && caa.Value is TypeSpecification)
								{
									TypeSpecification typeSpec = (TypeSpecification)caa.Value;
									TypeReference tr;
									if (typeSpec.IsArray)
									{
										tr = typeSpec.GetElementType();
									}
									else
									{
										tr = typeSpec;
									}
									 TypeDefinition td = GetTypeDefinition(tr);
									 if (td == null) td = Resolve(tr);
									 //if (td == null) td = tr.Module.GetType(tr.FullName);
									 if (td != null) list.Add(td);
								}
							}
						}
					}
					if (ca.HasFields)
					{
						foreach (Mono.Cecil.CustomAttributeNamedArgument cana in ca.Fields)
						{
							CustomAttributeArgument caa = cana.Argument;
							TypeReference tr = caa.Type;
							if (tr.FullName != "System.Type") continue;

							AddUsedTypeInCustomAttributeToList(caa, list);
						}
					}
					if (ca.HasProperties)
					{
						foreach (Mono.Cecil.CustomAttributeNamedArgument cana in ca.Properties)
						{
							CustomAttributeArgument caa = cana.Argument;
							TypeReference tr = caa.Type;
							if (tr.FullName != "System.Type") continue;

							AddUsedTypeInCustomAttributeToList(caa, list);
						}
					}
				}//end of foreach
			}//end of if                          

		}
		private bool IsCustomAttributeHandled(TypeDefinition td)
		{
			List<TypeDefinition> list = GetUsedTypeInCustomAttribute(td);
			foreach (TypeDefinition type in list)
			{
				if (IsBaseType(td, type)) continue;
				if (type != null && type.FullName != td.FullName && !IsTypeHandled(type))
				{
					return false;
				}
			}
			return true;
		}

		//private void FixCustomAttribute(TypeDefinition td, CustomAttributeArgument caa)
		//{
		//    object v = caa.Value;
		//    if (v is CustomAttributeArgument)
		//    {
		//        FixCustomAttribute(td, (CustomAttributeArgument)v);
		//    }
		//    else if (v is TypeReference)
		//    {
		//        if (v is ArrayType)
		//        {
		//            ArrayType atTmp = (ArrayType)v;
		//            if (atTmp.ElementType is GenericInstanceType)
		//            {
		//                GenericInstanceType gitTmp = (GenericInstanceType)atTmp.ElementType;
		//                TypeReference tr = (TypeReference)gitTmp.ElementType;
		//                string typeName = tr.FullName;
		//                TypeDefinition pdType = GetTypeDefinition(tr);
		//                if (pdType != null && pdType.FullName != typeName)
		//                {
		//                    GenericInstanceType gitNew = new GenericInstanceType(pdType);
		//                    foreach (TypeReference trTmp in gitTmp.GenericArguments)
		//                        gitNew. GenericArguments.Add(trTmp);
		//                    caa.Value = new ArrayType(gitNew);
		//                }
		//            }
		//            else
		//            {
		//                TypeReference tr = (TypeReference)atTmp.ElementType;
		//                string typeName = tr.FullName;
		//                TypeDefinition pdType = GetTypeDefinition(tr);
		//                if (pdType != null && pdType.FullName != typeName)
		//                    caa.Value = new ArrayType(pdType);
		//            }
		//        }
		//        else if (v is GenericInstanceType)
		//        {
		//            GenericInstanceType gitTmp = (GenericInstanceType)v;
		//            TypeReference tr = (TypeReference)gitTmp.ElementType;
		//            string typeName = tr.FullName;
		//            TypeDefinition pdType = GetTypeDefinition(tr);
		//            if (pdType != null && pdType.FullName != typeName)
		//            {
		//                GenericInstanceType gitNew = new GenericInstanceType(pdType);
		//                foreach (TypeReference trTmp in gitTmp.GenericArguments)
		//                    gitNew.GenericArguments.Add(trTmp);
		//                caa.Value = gitNew;

		//            }
		//        }
		//        else
		//        {
		//            TypeReference tr = (TypeReference)v;
		//            string typeName = tr.FullName;
		//            TypeDefinition pdType = GetTypeDefinition(tr);
		//            if (pdType != null && pdType.FullName != typeName)
		//                caa.Value = pdType;
		//        }
		//    }
		//}

		private void FixCustomAttribute(TypeDefinition td, Mono.Cecil.ICustomAttributeProvider cap)
		{
			if (cap.HasCustomAttributes)
			{
				foreach (CustomAttribute ca in cap.CustomAttributes)
				{
					//if (!ca.IsResolvable) continue;
					TypeDefinition caType = Resolve(ca.AttributeType);
					if (caType == null)  continue;

					try
					{
						InsUtils.ResolveCustomAttribute(ca);
					}
					catch (Exception ex)
					{
						_options.AppendTextInfoLine(ex.Message);
						continue;
					}

					//Cecil 0.9 fix this?
					//if (ca.HasConstructorArguments && ca.Constructor.HasParameters && ca.Constructor.Parameters.Count == ca.ConstructorArguments.Count)
					//{
					//    for (int i = 0; i < ca.Constructor.Parameters.Count; i++)
					//    {
					//        ParameterDefinition pd = ca.Constructor.Parameters[i];
					//        string typeName = pd.ParameterType.FullName;
					//        if (typeName == "System.Type")
					//        {
					//            CustomAttributeArgument caa = ca.ConstructorArguments[i];
					//            FixCustomAttribute(td, caa);
					//        }
					//        else if (typeName == "System.Object")
					//        {
					//            CustomAttributeArgument caa = ca.ConstructorArguments[i];
					//            if (caa.Value is CustomAttributeArgument)
					//            {
					//                CustomAttributeArgument caaTmp = (CustomAttributeArgument)caa.Value;
					//                if (caaTmp.Type.FullName == "System.Type" && caaTmp.Value is TypeSpecification)
					//                {
					//                    FixCustomAttribute(td, caa);
					//                }
					//            }
					//            else if (caa.Type.FullName == "System.Type" && caa.Value is TypeSpecification)
					//            {
					//                FixCustomAttribute(td, caa);
					//            }
					//        }

					//        continue;
					//    }
					//}

					if (ca.HasFields)
					{
						for (int i = 0; i < ca.Fields.Count; i++)
						{
							Mono.Cecil.CustomAttributeNamedArgument cana = ca.Fields[i];
							TypeReference tr = cana.Argument.Type;

							foreach (FieldDefinition fd in caType.Fields)
							{
								if (cana.Name == InsUtils.GetOldMemberName(fd) &&
									DeobfUtils.IsSameType(fd.FieldType, tr, false) &&
									fd.Name != cana.Name)
								{
									CustomAttributeUtils.SetFieldName(ca, i, fd.Name);
									break;
								}
							}
							continue;
						}
					}
					if (ca.HasProperties)
					{
						for (int i = 0; i < ca.Properties.Count; i++)
						{
							Mono.Cecil.CustomAttributeNamedArgument cana = ca.Properties[i];                            
							TypeReference tr = cana.Argument.Type;

							foreach (PropertyDefinition pd in caType.Properties)
							{                                
								if (cana.Name == InsUtils.GetOldMemberName(pd) &&
									DeobfUtils.IsSameType(pd.PropertyType, tr, false) &&
									pd.Name != cana.Name)
								{
									CustomAttributeUtils.SetPropertyName(ca, i, pd.Name);
									break;
								}
							}
							continue;
						}
					}
				}//end of for
			}//end of if
			return;
		}

		private void FixCustomAttribute(TypeDefinition td)
		{
			FixCustomAttribute(td, td);
			foreach (PropertyDefinition pd in td.Properties)
			{
				FixCustomAttribute(td, pd);
			}
			foreach (FieldDefinition fd in td.Fields)
			{
				FixCustomAttribute(td, fd);
			}
			foreach (MethodDefinition md in td.Methods)
			{
				FixCustomAttribute(td, md);
			}            
		}

		private void FixTypeReference(TypeReference typeRef)
		{
			TypeDefinition td = GetTypeDefinition(typeRef);
			if (td != null)
			{
				if (typeRef.IsGenericParameter)
				{
					typeRef.Name = td.Name;
				}
				else if (td.IsGenericInstance)
				{
					//GenericInstanceType git = (GenericInstanceType)(TypeReference)td;
					//foreach (TypeReference tr in git.GenericArguments)
					//{
					//    FixTypeReference(tr);
					//}
				}
				else
				{
					typeRef.Namespace = td.Namespace;
					typeRef.Name = td.Name;
				}
			}
		}
		private void FixGenericParameter(GenericParameter gp)
		{
			TypeDefinition td = GetTypeDefinition(gp);
			if (td != null)
			{
				gp.Name = td.Name;
			}
			else if (gp.HasConstraints)
			{
				foreach (TypeReference tr in gp.Constraints)
				{
					td = GetTypeDefinition(tr);
					if (td != null)
					{
						gp.Name = td.Name;
						break;
					}
				}
			}
			else if (!IsValidName(gp.Name))
			{
				gp.Name = String.Format("T{0}", gp.Position);
			}
		}

		private void FixMemberReference(MemberReference mbRef)
		{
			bool matched = false;

			if (mbRef is MethodReference || mbRef is MethodSpecification)
			{
				TypeDefinition td = GetTypeDefinition(mbRef.DeclaringType);
				if (td == null) return;

				if (mbRef is MethodSpecification)
					mbRef = ((MethodSpecification)mbRef).ElementMethod;

				MethodReference mr = mbRef as MethodReference;
				if (mr.Name == ".ctor" || mr.Name == ".cctor")
				{
					matched = true;
				}
				else
				{
					MethodDefinition m1 = FindMatchedMethodByOldName(td, mr, false);
					if (m1 != null && mr.Name != m1.Name)
					{
						SetMethodName(mr, m1);
						matched = true;
					}
				}
			}
			else if (mbRef is FieldReference)
			{
				TypeDefinition td = GetTypeDefinition(mbRef.DeclaringType);
				if (td == null) return;

				FieldReference fr = mbRef as FieldReference;
				foreach (FieldReference f1 in td.Fields)
				{
					//string oldName = InsUtils.GetOldMemberName(f1);
					if (DeobfUtils.IsSameFieldByOldName(fr, f1) && fr.Name != f1.Name)
					{
						fr.Name = f1.Name;
						matched = true;
						break;
					}
				}
			}            

			if (!matched)
			{
				//_host.SetStatusText(String.Format("MemberReference: {0} not handled.", mbRef.ToString()));
			}
		}

		private void SetMethodName(MethodReference dest, MethodReference source)
		{
			dest.Name = source.Name;
			for (int i = 0; i < source.Parameters.Count; i++)
			{
				dest.Parameters[i].Name = source.Parameters[i].Name;
			}
		}

		private void SetPropertyName(PropertyReference dest, PropertyReference source)
		{
			dest.Name = source.Name;
			for (int i = 0; i < source.Parameters.Count; i++)
			{
				dest.Parameters[i].Name = source.Parameters[i].Name;
			}
		}

		private MethodDefinition FindMatchedMethodByOldName(TypeDefinition type, MethodReference mr, bool genericMatch)
		{
			MethodDefinition retMethod = null;
			foreach (MethodDefinition md in type.Methods)
			{
				if (DeobfUtils.IsSameMethodByOldName(mr, md, genericMatch))
				{
					retMethod = md;
					break;
				}
			}        
			return retMethod;
		}

		private MethodDefinition FindMatchedMethod(TypeDefinition type, MethodReference mr)
		{
			MethodDefinition retMethod = null;
			MethodDefinition method = Resolve(mr);
			foreach (MethodDefinition md in type.Methods)
			{
				if (DeobfUtils.IsSameMethod(method, md))
				{
					retMethod = md;
					break;
				}
			}
			return retMethod;
		}

		private FieldDefinition FindMatchedFieldByOldName(TypeDefinition type, FieldReference fr)
		{
			FieldDefinition retField = null;
			foreach (FieldDefinition fd in type.Fields)
			{
				if (DeobfUtils.IsSameFieldByOldName(fr, fd))
				{
					retField = fd;
					break;
				}
			}
			return retField;
		}

		private PropertyDefinition FindMatchedPropertyByOldName(TypeDefinition type, PropertyReference pr)
		{
			PropertyDefinition retProperty = null;
			foreach (PropertyDefinition pd in type.Properties)
			{
				if (DeobfUtils.IsSamePropertyByOldName(pr, pd))
				{
					retProperty = pd;
					break;
				}
			}
			return retProperty;
		}

		private void HandleGenericParameters(Collection<GenericParameter> gpc)
		{
			foreach (GenericParameter gp in gpc)
			{
				FixGenericParameter(gp);
			}
		}

		private void HandleTypeName(TypeDefinition type)
		{
			if (IsDeobfName() && !type.IgnoreRename)
			{
				string typeName;
				int arity;

				int p = TryGetArity(type, out arity);
				if (p >= 0)
					typeName = type.Name.Substring(0, p);
				else
					typeName = type.Name;
				
				if (!IsValidName(typeName))
				{
					string oldFullName = type.FullName;
					//string t = GetTokenString(type.MetadataToken);

					string newName;

                    //if (IsDeobfHexRename())
                    //{
                    //    newName = GetHexName(type.Name);
                    //}
                    //else
                    {
                        newName = GetNewTypeName(type);
                    }
					type.Name = newName;

					//Dictionary<int, string> test = new Dictionary<int, string>();
					//for (int x = 0; x < type.Methods.Count; x++)
					//{
					//    test.Add(x, type.Methods[x].Name + "=" + type.Methods[x].Name.GetHashCode().ToString());
					//}

					if (!type.IsInterface && !type.IsValueType)
					{
						MatchResourceName(type.Module.Assembly, oldFullName, type.FullName);
					}
				}
				if (type.HasGenericParameters)
				{
					HandleGenericParameters(type.GenericParameters);
				}
			}
		}

		private void RemoveSealed(TypeDefinition type)
		{
			if (_options.chkRemoveSealedChecked)
			{
				if (!type.IsEnum && !type.IsValueType && !DeobfUtils.IsDelegate(type))
					type.IsSealed = false;
			}
		}

		private void RemoveDummyMethod(TypeDefinition type)
		{
			if (!_options.chkRemoveDummyMethodChecked) return;

			for (int i = 0; i < type.Methods.Count; i++)
			{
				if (_options.IsCancelPending) break;

				MethodDefinition method = type.Methods[i];
				if (String.Empty.Equals(method.Name)
					&& (!method.HasBody || (method.HasBody && method.Body.Instructions.Count == 0))
					//TODO: need to check method reference here?
					)
				{
					type.Methods.RemoveAt(i);
					i--;
				}
			}

		}

		private void ChangeInternalToPublic(TypeDefinition type)
		{
			if (!_options.chkInternalToPublicChecked)
				return;
			if (type.IsNested)
				type.IsNestedPublic = true;
			else
				type.IsPublic = true;
		}

		private void ChangeInternalToPublic(MethodDefinition method)
		{
			if (!_options.chkInternalToPublicChecked)
				return;
			if (method.IsFamily || method.IsFamilyOrAssembly || method.IsFamilyAndAssembly)
				return;
			method.IsPublic = true;
		}

        private void ChangeInternalToPublic(FieldDefinition field)
        {
            if (!_options.chkInternalToPublicChecked)
                return;
            if (field.IsFamily || field.IsFamilyOrAssembly || field.IsFamilyAndAssembly)
                return;
            field.IsPublic = true;
        }

		private void HandleProperyName(TypeDefinition type)
		{
			if (!IsDeobfName() && !IsDeobfRemoveAttribute() && !IsDeobfHandlePlugin()) 
				return;

			#region rename properties
			foreach (PropertyDefinition property in type.Properties)
			{
				if (_options.IsCancelPending) break;

				CallPluginBeforeHandler(property);

				if (IsDeobfName() && !property.IgnoreRename)
				{
					//if (property.Tag != null) continue;

					string oldName = property.Name;

					if (!IsValidName(property.Name))
					{
						InsUtils.SaveOldMemberName(property);

                        if (IsDeobfHexRename())
                        {
                            property.Name = GetHexName(property.Name);
                        }
                        else
                        {
                            if (property.GetMethod != null && property.GetMethod.IgnoreRename && property.GetMethod.Name.StartsWith("get_"))
                            {
                                property.Name = property.GetMethod.Name.Substring(4);
                            }
                            else if (property.SetMethod != null && property.SetMethod.IgnoreRename && property.SetMethod.Name.StartsWith("set_"))
                            {
                                property.Name = property.SetMethod.Name.Substring(4);
                            }
                            else
                            {
                                property.Name = GetNewPropertyName(property);
                            }

                            if (DeobfUtils.IsSamePropertyExists(property))
                            {
                                property.Name = GetNewPropertyName();
                            }
                        }

					}//end if !IsValidName

					if (property.SetMethod != null)
					{
						MethodDefinition method = property.SetMethod;
						InsUtils.SaveOldMemberName(method);
						if (method.IgnoreRename)
						{
							//the method has been handled as overrided or interface implementation
						}
						else
						{
                            if (IsDeobfHexRename())
                            {
                                method.Name = GetHexName(method.Name);
                            }
                            else
                            {
                                if (CanRenameMethod(method))
                                {
                                    method.Name = "set_" + property.Name;
                                    method.IgnoreRename = true;
                                    RenameOverridedMethod(method);
                                }
                            }
						}
					}

					if (property.GetMethod != null)
					{
						MethodDefinition method = property.GetMethod;
						InsUtils.SaveOldMemberName(method);

						if (method.IgnoreRename)
						{
							//the method has been handled as overrided or interface implementation
						}
						else
						{
                            if (IsDeobfHexRename())
                            {
                                method.Name = GetHexName(method.Name);
                            }
                            else
                            {
                                if (CanRenameMethod(method))
                                {
                                    method.Name = "get_" + property.Name;
                                    method.IgnoreRename = true;
                                    RenameOverridedMethod(method);
                                }
                            }
						}
					}
				}

				if (IsDeobfRemoveAttribute())
				{
					RemoveAttribute(property);
				}

				CallPluginAfterHandler(property);
			}
			#endregion rename properties

		}

		private void FixEventFieldName(MethodDefinition addMethod, TypeReference eventType, string fieldName)
		{
			if (addMethod.HasBody)
			{
				Collection<Instruction> ic = addMethod.Body.Instructions;
				FieldDefinition fd = null;
				foreach (Instruction ins in ic)
				{
					if (ins.OpCode.Code == Code.Stfld)
					{
						FieldReference fr = ins.Operand as FieldReference;
						if (fr != null &&
							DeobfUtils.IsSameType(eventType, fr.FieldType)
							)
						{
							fd = Resolve(fr);
							break;
						}
					}
				}
				if (fd != null)
				{
					fd.Name = fieldName;
					fd.IgnoreRename = true;
				}
			}
		}

		private void HandleEventName(TypeDefinition type)
		{
			if (!IsDeobfName() && !IsDeobfRemoveAttribute() && !IsDeobfHandlePlugin()) 
				return;

			#region rename events
			foreach (EventDefinition ed in type.Events)
			{
				if (_options.IsCancelPending) break;

				CallPluginBeforeHandler(ed);

				if (IsDeobfName() && !ed.IgnoreRename)
				{
					string oldName = ed.Name;

					if (!IsValidName(ed.Name))
					{
						InsUtils.SaveOldMemberName(ed);
                        if (IsDeobfHexRename())
                        {
                            ed.Name = GetHexName(ed.Name);
                        }
                        else
                        {
                            ed.Name = GetNewEventName(ed);
                        }
					}

					if (ed.AddMethod != null)
					{
						MethodDefinition method = ed.AddMethod;
						InsUtils.SaveOldMemberName(method);

						if (method.IgnoreRename)
						{
							//the method has been handled as overrided or interface implementation
						}
						else
						{
                            if (IsDeobfHexRename())
                            {
                                method.Name = GetHexName(method.Name);
                            }
                            else
                            {
                                if (CanRenameMethod(method))
                                {
                                    method.Name = "add_" + ed.Name;
                                    method.IgnoreRename = true;
                                    RenameOverridedMethod(method);
                                }
                            }
						}

						FixEventFieldName(method, ed.EventType, ed.Name);
					}
					if (ed.RemoveMethod != null)
					{
						MethodDefinition method = ed.RemoveMethod;
						InsUtils.SaveOldMemberName(method);

						if (method.IgnoreRename)
						{
							//the method has been handled as overrided or interface implementation
						}
						else
						{
                            if (IsDeobfHexRename())
                            {
                                method.Name = GetHexName(method.Name);
                            }
                            else
                            {
                                if (CanRenameMethod(method))
                                {
                                    method.Name = "remove_" + ed.Name;
                                    method.IgnoreRename = true;
                                    RenameOverridedMethod(method);
                                }
                            }
						}
					}
					if (ed.InvokeMethod != null)
					{
						MethodDefinition method = ed.InvokeMethod;
						InsUtils.SaveOldMemberName(method);

						if (method.IgnoreRename)
						{
							//the method has been handled as overrided or interface implementation
						}
						else
						{
                            if (IsDeobfHexRename())
                            {
                                method.Name = GetHexName(method.Name);
                            }
                            else
                            {
                                if (CanRenameMethod(method))
                                {
                                    method.Name = "invoke_" + ed.Name;
                                    method.IgnoreRename = true;
                                    RenameOverridedMethod(method);
                                }
                            }
						}
					}
				}

				if (IsDeobfRemoveAttribute())
				{
					RemoveAttribute(ed);
				}

				CallPluginAfterHandler(ed);
			}
			#endregion rename events

		}

		private void HandleFieldName(TypeDefinition type)
		{
			if (!IsDeobfName() && !IsDeobfRemoveAttribute() && !IsDeobfHandlePlugin()) 
				return;

			#region rename fields
			foreach (FieldDefinition field in type.Fields)
			{
				if (_options.IsCancelPending) break;

				CallPluginBeforeHandler(field);

                ChangeInternalToPublic(field);

				if (IsDeobfName() && !field.IgnoreRename)
				{
					string oldName = field.Name;

					if (!IsValidName(field.Name) || _options.KeywordFile.IsKeyword(field.Name))
					{
						InsUtils.SaveOldMemberName(field);

                        if (IsDeobfHexRename())
                        {
                            field.Name = GetHexName(field.Name);
                        }
                        else
                        {
                            field.Name = GetNewFieldName(field);
                            if (DeobfUtils.IsSameFieldExists(field))
                            {
                                field.Name = GetNewFieldName();
                            }
                        }

						//whywhy?
						//if (!type.IsInterface)
						//{
						//    MatchResourceName(type.Module.Assembly, oldName, field.Name);
						//}
					}
				}

				if (IsDeobfRemoveAttribute())
				{
					RemoveAttribute(field);
				}

				CallPluginAfterHandler(field);
			}
			#endregion rename fields
		}

		private void HandleMethod(TypeDefinition type, string file)
		{
			#region handle methods
		  
			foreach (MethodDefinition method in type.Methods)
			{
				if (_options.IsCancelPending) break;

				CallPluginBeforeHandler(method);

				ChangeInternalToPublic(method);

				if (IsDeobfName())
				{
					string oldName = method.Name;
					
					for (int i = 0; i < method.Parameters.Count; i++)
					{
						ParameterDefinition pd = method.Parameters[i];
						if (!IsValidName(pd.Name))
						{
							pd.Name = String.Format("p{0}", i);
						}
					}

					if (method.HasBody)
					{
						Collection<VariableDefinition> vars = method.Body.Variables;
						for (int i = 0; i < vars.Count; i++)
						{
							if (!IsValidName(vars[i].Name))
								vars[i].Name = String.Format("v{0}", vars[i].Index);
						}
					}

					if (method.IsConstructor)
					{
						//don't need to rename constructor
					}
					else
					{
                        InsUtils.SaveOldMemberName(method);

                        if (!IsValidName(method.Name))
                        {
                            if (IsDeobfHexRename())
                            {
                                method.Name = GetHexName(method.Name);
                            }
                            else
                            {
                                if (!TrySetMethodNameByPInvokeInfo(method))
                                {

                                    //if (type.IsInterface)
                                    //{
                                    //    //force to use new name, because we don't like Overrides in C#
                                    //    //there is an issue: if class implicit implement two or more interfaces which have same method signatures,
                                    //    //there will be error because we force to rename the method name here
                                    //    method.Name = GetNewName("m");

                                    //    if (list == null)
                                    //        list = FindDerivedTypes(type);
                                    //    RenameOverridedMethod(method, list);
                                    //}
                                    //else 
                                    if (method.IsVirtual && method.HasOverrides &&
                                         method.Name != method.Overrides[0].Name &&
                                         IsValidName(method.Overrides[0].Name) &&
                                         TryRestoreMethodNameByOverride(method)
                                            )
                                    {
                                        //try to restore override name at first
                                        //this will break overrided method in inherited types, 
                                        //we can search and rename them at the same time,
                                        //but it may be too much heavy, especially for multiple assemblies, 
                                        //we need to search for inherited types in all modules for each virtual method?
                                        RenameOverridedMethod(method);
                                    }
                                    else if (!method.IgnoreRename && CanRenameMethod(method))
                                    {
                                        if (TrySetMethodNameBySignature(method))
                                        {
                                            //method name is set to same new name
                                        }
                                        else
                                        {
                                            method.Name = GetNewMethodName();
                                            method.IgnoreRename = true;
                                        }

                                        RenameOverridedMethod(method);
                                    }
                                }
                            }
                        }
                        else // valid name
                        {
                            TrySetMethodNameByPInvokeInfo(method);
                        }
					}

					if (method.HasGenericParameters)
					{
						HandleGenericParameters(method.GenericParameters);
					}

				}

				if (IsDeobfFlow())
				{
					DeobfFlow(file, method);
				}

				if (IsDeobfString())
				{
					DeobfString(file, method,
						_options.chkAutoStringChecked ? null : _options.StringOptionSearchForMethod,
						_options.chkAutoStringChecked ? null : _options.StringOptionCalledMethod
						);
				}

				if (IsDeobfFlowAfterString())
				{
					DeobfFlowAfterString(file, method);
				}

				if (_options.chkInitLocalVarsChecked)
				{
					DeobfInitLocalVars(method);
				}

				if (IsDeobfRemoveAttribute())
				{
					RemoveAttribute(method);
				}

				CallPluginAfterHandler(method);
			}            
			 
			#endregion handle methods
		}

		Regex regexPInvokeEntryOrdinal = new Regex(@"^#{0,1}\d+$");
		private string GetPInvokeName(PInvokeInfo pii, bool includeModuleName)
		{
			if(regexPInvokeEntryOrdinal.IsMatch(pii.EntryPoint))
			{
				return String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(pii.Module.Name), pii.EntryPoint.Replace("#", ""));
			}
			if (includeModuleName)
			{
				return String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(pii.Module.Name), pii.EntryPoint);
			}
			return pii.EntryPoint;
		}

		private bool TrySetMethodNameByPInvokeInfo(MethodDefinition method)
		{
			if (method.IsPInvokeImpl && method.PInvokeInfo != null)
			{
				if (method.Name != method.PInvokeInfo.EntryPoint)
				{
					string savedName = method.Name;
					method.Name = GetPInvokeName(method.PInvokeInfo, false);
					if (DeobfUtils.IsSameMethodExists(method))
						method.Name = GetPInvokeName(method.PInvokeInfo, true);
					if (DeobfUtils.IsSameMethodExists(method))
					{
						method.Name = savedName;
						return false;
					}
					method.IgnoreRename = true;
					return true;
				}
				else
				{
					method.IgnoreRename = true;
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		private bool TrySetMethodNameBySignature(MethodDefinition method)
		{
			string savedName = method.Name;
			method.Name = GetNewMethodName(method);
			if (DeobfUtils.IsSameMethodExists(method))
			{
				method.Name = savedName;
				return false;
			}

			if (method.IsNewSlot)
			{
				if (IsMethodNameUsedInBaseType(method))
				{
					method.Name = savedName;
					return false;
				}
			}

			method.IgnoreRename = true;
			return true;
		}

		private void RenameOverridedMethod(MethodDefinition method)
		{
			if (method.IsVirtual || method.IsAbstract)
			{
				TypeDefinition type = method.DeclaringType;
				List<TypeDefinition> list = FindDerivedTypes(type, int.MaxValue);
				RenameOverridedMethod(method, list);
			}
		}

		private void RenameOverridedMethod(MethodDefinition method, List<TypeDefinition> derivedTypeList)
		{
			TypeDefinition type = method.DeclaringType;
			foreach (TypeDefinition derivedType in derivedTypeList)
			{
				MethodDefinition overridedMethod = FindMatchedMethodByOldName(derivedType, method, true);
				if (overridedMethod != null && overridedMethod.IsVirtual && 
					((overridedMethod.IsReuseSlot && !type.IsInterface) || (overridedMethod.IsNewSlot && type.IsInterface)) &&
					!overridedMethod.HasOverrides )
				{
					InsUtils.SaveOldMemberName(overridedMethod);
					overridedMethod.Name = method.Name;
					overridedMethod.IgnoreRename = true;
				}
			}
		}

		private bool CanRenameMethod(MethodDefinition method, TypeDefinition searchInType)
		{
            if (method.IsVirtual && method.IsNewSlot && searchInType != null)
			{
				foreach (TypeReference tr in searchInType.Interfaces)
				{
					TypeDefinition td = Resolve(tr);
					if (td == null) continue;

					MethodDefinition originalMethod = FindMatchedMethodByOldName(td, method, true);
					if (originalMethod != null && originalMethod.DeclaringType != null &&
						originalMethod.DeclaringType.Module != null &&
						originalMethod.DeclaringType.Module.Assembly != null &&
						!allAssemblies.ContainsKey(originalMethod.DeclaringType.Module.Assembly.Name.FullName)
						)
						return false;

					if (td.IsInterface && !CanRenameMethod(method, td))
						return false;
				}
			}
			return true;
		}

		private bool CanRenameMethod(MethodDefinition method)
		{
			if (method.IsVirtual && method.IsNewSlot)
			{
				TypeDefinition type = method.DeclaringType;
				return CanRenameMethod(method, type);
			}
			return true;
		}

		private bool IsMethodNameUsedInBaseType(MethodDefinition method)
		{
			TypeDefinition baseType = Resolve(method.DeclaringType.BaseType);
			while (baseType != null)
			{
				MethodDefinition md = FindMatchedMethod(baseType, method);
				if (md != null && md.IsNewSlot && md.IsVirtual)
				{
					return true;
				}
				baseType = Resolve(baseType.BaseType);
			}
			return false;
		}

		private bool TryRestoreEntryPointMethodName(AssemblyDefinition ad)
		{
			if (IsDeobfName())
			{
				MethodDefinition md = ad.EntryPoint;
				if (md != null && md.IsStatic && md.Name != "Main")
				{
					string savedName = md.Name;
					md.Name = "Main";
					if (DeobfUtils.IsSameMethodExists(md))
					{
						md.Name = savedName;
						return false;
					}
					return true;
				}
			}
			return false;
		}

		private bool TryRestoreMethodNameByOverride(MethodDefinition method)
		{
			if (!method.HasOverrides) 
				return false;

			string savedName = method.Name;
			string newName = method.Overrides[0].Name;
			method.Name = newName;
			if (DeobfUtils.IsSameMethodExists(method))
			{
				method.Name = savedName;
				return false;
			}

			//check whether newName already used in base types
			if (method.IsNewSlot)
			{
				if (IsMethodNameUsedInBaseType(method))
				{
					method.Name = savedName;
					return false;
				}
			}
			method.IgnoreRename = true;
			return true;
		}

		private List<TypeDefinition> FindDerivedTypes(TypeDefinition type, int level)
		{
			string key = TokenUtils.GetMetadataTokenString(type.MetadataToken);
			if (derivedTypes.ContainsKey(key))
			{
				return derivedTypes[key];
			}

			List<TypeDefinition> list = DeobfUtils.FindDerivedTypes(type, level, allAssemblies);
			derivedTypes.Add(key, list);
			return list;
		}

		private void HandleType(TypeDefinition type, string file)
		{
			ModuleDefinition module = type.Module;
			AssemblyDefinition ad = module.Assembly;
			string adName = ad.Name.Name;

			_options.Host.SetStatusText(String.Format("Handling {0}: {1} ...", adName, InsUtils.GetOldFullTypeName(type)));

			CallPluginBeforeHandler(type);

			RemoveAttribute(type);
			HandleTypeName(type);            
			RemoveSealed(type);
			RemoveDummyMethod(type);
			ChangeInternalToPublic(type);
			
			HandleProperyName(type);
			HandleFieldName(type);
			HandleEventName(type);

			// put method after property's method
			HandleMethod(type, file);

			FixCustomAttribute(type);
			AddMissingProperty(type);
			AddMissingEvent(type);

			CallPluginAfterHandler(type);
		}

		private void MatchResourceName(AssemblyDefinition ad, string oldName, string newName)
		{
			string matchName = oldName.Replace("/", ".") + ".";
			char[] matchBytes = matchName.ToCharArray();
			foreach (ModuleDefinition module in ad.Modules)
			{
				foreach (Resource r in module.Resources)
				{
					if (r.Name.EndsWith("Properties.Resources.resources", StringComparison.OrdinalIgnoreCase))
						continue;

					bool startWith = true;
					char[] rBytes = r.Name.ToCharArray();
					if (rBytes.Length < matchBytes.Length)
					{
						startWith = false;
					}
					else
					{
						for (int i = 0; i < matchBytes.Length; i++)
						{
							if (matchBytes[i] != rBytes[i])
							{
								startWith = false;
								break;
							}
						}
					}
					//if (r.Name.StartsWith(matchName))
					if (startWith)
					{                        
						r.Name = String.Format("{0}.{1}", newName.Replace("/", "."), r.Name.Substring(matchName.Length));
					}
				}
			}
		}

		public int DeobfMethod(string file, MethodDefinition method)
		{
			LoadAllAssemblies();

			int deobfCount = 0;
			if (_options.HasPlugin)
			{
				InitPlugin();
				deobfCount += CallPluginBeforeHandler(method);
			}

			deobfCount += DeobfFlow(file, method);

			if (_options.chkAutoStringChecked)
			{
				deobfCount += DeobfString(file, method, 
					_options.StringOptionSearchForMethod,
					_options.StringOptionCalledMethod);
			}

			deobfCount += DeobfFlowAfterString(file, method);

			if (_options.chkInitLocalVarsChecked)
			{
				deobfCount += DeobfInitLocalVars(method);
			}

			if (_options.HasPlugin)
			{
				deobfCount += CallPluginAfterHandler(method);
			}

			return deobfCount;
		}

		public int DeobfFlow(string file, MethodDefinition method)
		{
            if (_options.IgnoredMethodFile.IsExceedMaxInstructionCount(method))
                return 0;

			string assemblyFile = Path.Combine(_options.SourceDir, file);

			int loopCount = _options.LoopCount;
			int maxMoveCount = _options.MaxMoveCount;

			int count = 0;

            if (_options.chkRemoveInvalidInstructionChecked)
            {
                count += DeobfFlowRemoveInvalidInstruction(method);
            }

			for (int i = 0; i < loopCount; i++)
			{
				int deobfCount = 0;

				if (_options.chkBoolFunctionChecked)
					deobfCount += DeobfBoolFunction(assemblyFile, method);

				if (_options.chkPatternChecked)
					deobfCount += DeobfFlowPattern(method);

				if (_options.chkBranchChecked)
					deobfCount += DeobfFlowBranch(method, _options.LoopCount, _options.MaxMoveCount, _options.MaxRefCount);

				if (_options.chkSwitchChecked)
					deobfCount += DeobfFlowSwitch(method);

				if (_options.chkCondBranchDownChecked || _options.chkCondBranchUpChecked)
					deobfCount += DeobfFlowConditionalBranch(method, _options.MaxMoveCount);

				if (_options.chkDirectCallChecked)
					deobfCount += DeobfFlowDirectCall(method);

				if (_options.chkRemoveExceptionHandlerChecked)
					deobfCount += DeobfFlowRemoveExceptionHandler(method, this.Options.ExceptionHandlerFile.RemoveGlobalExceptionHandler);

				if (_options.chkUnreachableChecked)
					deobfCount += DeobfFlowUnreachable(method);

				count += deobfCount;
				if (deobfCount < 1) break;
			}

			if (_options.chkBlockMoveChecked)
			{
				count += DeobfFlowMoveBlock(method, _options.MaxMoveCount, _options.MaxRefCount);
			}

			if (_options.chkReflectorFixChecked)
			{
				count += DeobfFlowReflectorFixDupBrTrueFalsePop(method, _options.MaxMoveCount);
				count += DeobfFlowReflectorFixBrTrueFalsePopLdDup(method, _options.MaxMoveCount);                
			}

			return count;
		}

		public int DeobfFlowAfterString(string file, MethodDefinition method)
		{
			string assemblyFile = Path.Combine(_options.SourceDir, file);

			int loopCount = _options.LoopCount;
			int count = 0;

			for (int i = 0; i < loopCount; i++)
			{
				int deobfCount = 0;

				if (_options.chkDelegateCallChecked)
					deobfCount += DeobfFlowDelegateCall(file, method);

				count += deobfCount;
				if (deobfCount < 1) break;
			}

			return count;
		}

		#region IDeobfuscator Members

		public void AppendTextInfoLine(string text)
		{
			_options.AppendTextInfoLine(text);
		}

		public bool IsCancelPending
		{
			get { return _options.IsCancelPending; }
		}

		#endregion
	}//end of class
}