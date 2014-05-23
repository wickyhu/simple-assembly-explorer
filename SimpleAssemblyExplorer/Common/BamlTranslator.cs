namespace SimpleAssemblyExplorer
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Text;
	using System.Reflection;
	using System.Collections.Specialized;
	using System.Diagnostics;

    public class BamlTranslator : BamlBaseTranslator
	{
		private IDictionary assemblyTable = new Hashtable();
		private IDictionary stringTable = new Hashtable();
		private IDictionary typeTable = new Hashtable();
		private IDictionary propertyTable = new Hashtable();
		private IList staticResourceTable = new ArrayList();

		private NamespaceManager namespaceManager = new NamespaceManager();

		private Element rootElement = null;
	
		private Stack elementStack = new Stack();
		private IList constructorParameterTable = new ArrayList();
	
		private TypeDeclaration[] knownTypeTable = null;
		private PropertyDeclaration[] knownPropertyTable = null;
		private Hashtable knownResourceTable = new Hashtable();
	
		private IDictionary dictionaryKeyStartTable = new Hashtable();
		private IDictionary dictionaryKeyPositionTable = new Hashtable();
	
		private int lineNumber = 0;
		private int linePosition = 0;

		public BamlTranslator(Stream stream):base(null, stream)
		{
			BamlBinaryReader reader = new BamlBinaryReader(stream);
	
			int length = reader.ReadInt32();
			string format = new string(new BinaryReader(stream, Encoding.Unicode).ReadChars(length >> 1));
			if (format != "MSBAML")
			{
                throw new NotSupportedException(String.Format("Not supported file format: {0}", format));
			}
	
			int readerVersion = reader.ReadInt32();
			int updateVersion = reader.ReadInt32();
			int writerVersion = reader.ReadInt32();
			if ((readerVersion != 0x00600000) || (updateVersion != 0x00600000) || (writerVersion != 0x00600000))
			{
                throw new NotSupportedException(String.Format("Not supported baml version: {0:x}", readerVersion));
			}
	
			while (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				BamlRecordType recordType = (BamlRecordType) reader.ReadByte();
	
				long position = reader.BaseStream.Position;
				int size = 0;
	
				switch (recordType)
				{
					case BamlRecordType.XmlnsProperty:
					case BamlRecordType.PresentationOptionsAttribute:
					case BamlRecordType.PIMapping:
					case BamlRecordType.AssemblyInfo:
					case BamlRecordType.Property:
					case BamlRecordType.PropertyWithConverter:
					case BamlRecordType.PropertyCustom:
					case BamlRecordType.DefAttribute:
					case BamlRecordType.DefAttributeKeyString:
					case BamlRecordType.TypeInfo:
					case BamlRecordType.AttributeInfo:
					case BamlRecordType.StringInfo:
					case BamlRecordType.Text:
					case BamlRecordType.TextWithConverter:
					case BamlRecordType.TextWithId:
						size = reader.ReadCompressedInt32();
						break;
				}
	
				// Console.WriteLine(recordType.ToString());
	
				switch (recordType)
				{
					case BamlRecordType.DocumentStart:
						bool loadAsync = reader.ReadBoolean();
						int maxAsyncRecords = reader.ReadInt32();
						bool debugBaml = reader.ReadBoolean();
						break;
	
					case BamlRecordType.DocumentEnd:
						break;
	
					case BamlRecordType.ElementStart:
						this.namespaceManager.OnElementStart();
						this.ReadElementStart(reader);
						break;
	
					case BamlRecordType.ElementEnd:
						this.ReadElementEnd();
						this.namespaceManager.OnElementEnd();
						break;
	
					case BamlRecordType.KeyElementStart:
						this.ReadKeyElementStart(reader);
						break;
	
					case BamlRecordType.KeyElementEnd:
						this.ReadKeyElementEnd();
						break;
	
					case BamlRecordType.XmlnsProperty:
						this.ReadXmlnsProperty(reader);
						break;
	
					case BamlRecordType.PIMapping:
						this.ReadNamespaceMapping(reader);
						break;
	
					case BamlRecordType.PresentationOptionsAttribute:
						this.ReadPresentationOptionsAttribute(reader);
						break;
	
					case BamlRecordType.AssemblyInfo:
						this.ReadAssemblyInfo(reader);
						break;
	
					case BamlRecordType.StringInfo:
						this.ReadStringInfo(reader);
						break;
	
					case BamlRecordType.ConnectionId:
						reader.ReadInt32(); // ConnectionId
						break;
	
					case BamlRecordType.Property:
						this.ReadPropertyRecord(reader);
						break;
	
					case BamlRecordType.PropertyWithConverter:
						this.ReadPropertyWithConverter(reader);
						break;
	
					case BamlRecordType.PropertyWithExtension:
						this.ReadPropertyWithExtension(reader);
						break;
	
					case BamlRecordType.PropertyTypeReference:
						this.ReadPropertyTypeReference(reader);
						break;
	
					case BamlRecordType.PropertyWithStaticResourceId:
						this.ReadPropertyWithStaticResourceIdentifier(reader);
						break;
	
					case BamlRecordType.ContentProperty:
						this.ReadContentProperty(reader);
						break;
	
					case BamlRecordType.TypeInfo:
						this.ReadTypeInfo(reader);
						break;
	
					case BamlRecordType.AttributeInfo:
						this.ReadAttributeInfo(reader);
						break;
	
					case BamlRecordType.DefAttribute:
						this.ReadDefAttribute(reader);
						break;
	
					case BamlRecordType.DefAttributeKeyString:
						this.ReadDefAttributeKeyString(reader);
						break;
	
					case BamlRecordType.DefAttributeKeyType:
						this.ReadDefAttributeKeyType(reader);
						break;
	
					case BamlRecordType.Text:
						this.ReadText(reader);
						break;
	
					case BamlRecordType.TextWithConverter:
						this.ReadTextWithConverter(reader);
						break;
	
					case BamlRecordType.PropertyCustom:
						this.ReadPropertyCustom(reader);
						break;
	
					case BamlRecordType.PropertyListStart:
						this.ReadPropertyListStart(reader);
						break;
	
					case BamlRecordType.PropertyListEnd:
						this.ReadPropertyListEnd();
						break;
	
					case BamlRecordType.PropertyDictionaryStart:
						this.ReadPropertyDictionaryStart(reader);
						break;
	
					case BamlRecordType.PropertyDictionaryEnd:
						this.ReadPropertyDictionaryEnd();
						break;
	
					case BamlRecordType.PropertyComplexStart:
						this.ReadPropertyComplexStart(reader);
						break;
	
					case BamlRecordType.PropertyComplexEnd:
						this.ReadPropertyComplexEnd();
						break;
	
					case BamlRecordType.ConstructorParametersStart:
						this.ReadConstructorParametersStart();
						break;
	
					case BamlRecordType.ConstructorParametersEnd:
						this.ReadConstructorParametersEnd();
						break;
	
					case BamlRecordType.ConstructorParameterType:
						this.ReadConstructorParameterType(reader);
						break;
	
					case BamlRecordType.DeferableContentStart:
						int contentSize = reader.ReadInt32();
						break;
	
					case BamlRecordType.StaticResourceStart:
						this.ReadStaticResourceStart(reader);
						break;

					case BamlRecordType.StaticResourceEnd:
						this.ReadStaticResourceEnd(reader);
						break;

					case BamlRecordType.StaticResourceId:
						this.ReadStaticResourceIdentifier(reader);
						break;

					case BamlRecordType.OptimizedStaticResource:
						this.ReadOptimizedStaticResource(reader);
						break;
	
					case BamlRecordType.LineNumberAndPosition:
						this.lineNumber = reader.ReadInt32(); // LineNumber
						this.linePosition = reader.ReadInt32(); // Position
						// Console.WriteLine(lineNumber.ToString() + "," + linePosition.ToString());
						break;
	
					case BamlRecordType.LinePosition:
						this.linePosition = reader.ReadInt32(); // Position
						break;

					case BamlRecordType.TextWithId:
						this.ReadTextWithId(reader);
						break;

					default:
						throw new NotSupportedException(recordType.ToString());
				}
	
				if (size > 0)
				{
					reader.BaseStream.Position = position + size;
				}
			}
		}
	
		public override string ToString()
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				using (IndentationTextWriter indentationTextWriter = new IndentationTextWriter(stringWriter))
				{
					WriteElement(this.rootElement, indentationTextWriter);
				}

				return stringWriter.ToString();
			}
		}

		private static void WriteElement(Element element, IndentationTextWriter writer)
		{
			writer.Write("<");
			WriteTypeDeclaration(element.TypeDeclaration, writer);

			ArrayList attributes = new ArrayList();
			ArrayList properties = new ArrayList();
			Property contentProperty = null;
			foreach (Property property in element.Properties)
			{
				switch (property.PropertyType)
				{
					case PropertyType.List:
					case PropertyType.Dictionary:
						properties.Add(property);
						break;

					case PropertyType.Namespace:
					case PropertyType.Value:
					case PropertyType.Declaration:
						attributes.Add(property);
						break;

					case PropertyType.Complex:
						if (IsExtension(property.Value))
						{
							attributes.Add(property);
						}
						else
						{
							properties.Add(property);
						}
						break;

					case PropertyType.Content:
						contentProperty = property;
						break;
				}
			}

			foreach (Property property in attributes)
			{
				writer.Write(" ");
				WritePropertyDeclaration(property.PropertyDeclaration, element.TypeDeclaration, writer);
				writer.Write("=");
				writer.Write("\"");

				switch (property.PropertyType)
				{
					case PropertyType.Complex:
						WriteComplexElement((Element)property.Value, writer);
						break;

					case PropertyType.Namespace:
					case PropertyType.Declaration:
					case PropertyType.Value:
						writer.Write(property.Value.ToString());
						break;

					default:
						throw new NotSupportedException();
				}


				writer.Write("\"");
			}

			if ((contentProperty != null) || (properties.Count > 0))
			{
				writer.Write(">");

				if ((properties.Count > 0) || (contentProperty.Value is IList))
				{
					writer.WriteLine();
				}

				writer.Indentation++;

				foreach (Property property in properties)
				{
					writer.Write("<");
					WriteTypeDeclaration(element.TypeDeclaration, writer);
					writer.Write(".");
					WritePropertyDeclaration(property.PropertyDeclaration, element.TypeDeclaration, writer);
					writer.Write(">");
					writer.WriteLine();
					writer.Indentation++;
					WritePropertyValue(property, writer);
					writer.Indentation--;
					writer.Write("</");
					WriteTypeDeclaration(element.TypeDeclaration, writer);
					writer.Write(".");
					WritePropertyDeclaration(property.PropertyDeclaration, element.TypeDeclaration, writer);
					writer.Write(">");
					writer.WriteLine();
				}

				if (contentProperty != null)
				{
					WritePropertyValue(contentProperty, writer);
				}

				writer.Indentation--;

				writer.Write("</");
				WriteTypeDeclaration(element.TypeDeclaration, writer);
				writer.Write(">");
			}
			else
			{
				writer.Write(" />");
			}

			writer.WriteLine();
		}

		private static void WriteTypeDeclaration(TypeDeclaration value, TextWriter writer)
		{
			writer.Write(value.ToString());
		}

        private static void WritePropertyDeclaration(PropertyDeclaration value, TypeDeclaration context, TextWriter writer)
        {
            if (value.DeclaringType != null 
                && context != null
                && value.DeclaringType.Name != "XmlNamespace" 
                && value.DeclaringType.Namespace != null 
                && value.DeclaringType.Assembly != null
                && (value.DeclaringType.Assembly != context.Assembly 
                    || value.DeclaringType.Namespace != context.Namespace
                    || value.DeclaringType.Name != context.Name)
               )
            {
                writer.Write(value.DeclaringType.Name);
                writer.Write(".");
            }

            writer.Write(value.ToString());
        }

		private static void WritePropertyValue(Property property, IndentationTextWriter writer)
		{
			object value = property.Value;

			if (value is IList)
			{
				IList elements = (IList)value;

				if ((property.PropertyDeclaration != null) && (property.PropertyDeclaration.Name == "Resources") && (elements.Count == 1) && (elements[0] is Element))
				{
					Element element = (Element) elements[0];
					if ((element.TypeDeclaration.Name == "ResourceDictionary") && (element.TypeDeclaration.Namespace == "System.Windows") && (element.TypeDeclaration.Assembly == "PresentationFramework") && (element.Arguments.Count == 0) && (element.Properties.Count == 1) && (element.Properties[0].PropertyType == PropertyType.Content))
					{
						WritePropertyValue(element.Properties[0], writer);
						return;
					}
				}

				foreach (object child in elements)
				{
					if (child is string)
					{
						writer.Write((string)child);
					}
					else if (child is Element)
					{
						WriteElement((Element)child, writer);
					}
					else
					{
						throw new NotSupportedException();
					}
				}
			}
			else if (value is string)
			{
				string text = (string)value;
				writer.Write(text);
			}
			else if (value is Element)
			{
				Element element = (Element)value;
				WriteElement(element, writer);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		private static void WriteComplexElement(Element element, TextWriter writer)
		{
            //if (element.TypeDeclaration.Name == "Binding")
            //{
            //    Console.WriteLine();
            //}

			writer.Write("{");

			string name = element.TypeDeclaration.ToString();

			if (name.EndsWith("Extension"))
			{
				name = name.Substring(0, name.Length - 9);
			}

			writer.Write(name);

			if ((element.Arguments.Count > 0) || (element.Properties.Count > 0))
			{
				if (element.Arguments.Count > 0)
				{
					writer.Write(" ");

					for (int i = 0; i < element.Arguments.Count; i++)
					{
						if (i != 0)
						{
							writer.Write(", ");
						}

						if (element.Arguments[i] is string)
						{
							writer.Write((string)element.Arguments[i]);
						}
						else if (element.Arguments[i] is TypeDeclaration)
						{
							WriteTypeDeclaration((TypeDeclaration)element.Arguments[i], writer);
						}
						else if (element.Arguments[i] is PropertyDeclaration)
						{
							PropertyDeclaration propertyName = (PropertyDeclaration) element.Arguments[i];
							writer.Write(propertyName.Name);
						}
						else if (element.Arguments[i] is ResourceName)
						{
							ResourceName resourceName = (ResourceName)element.Arguments[i];
							writer.Write(resourceName.Name);
						}
						else if (element.Arguments[i] is Element)
						{
							WriteComplexElement((Element)element.Arguments[i], writer);
						}
						else
						{
							throw new NotSupportedException();
						}
					}
				}

				if (element.Properties.Count > 0)
				{
					writer.Write(" ");

					for (int i = 0; i < element.Properties.Count; i++)
					{
						if ((i != 0) || (element.Arguments.Count > 0))
						{
							writer.Write(", ");
						}

						WritePropertyDeclaration(element.Properties[i].PropertyDeclaration, element.TypeDeclaration, writer);
						writer.Write("=");

						if (element.Properties[i].Value is string)
						{
							writer.Write((string)element.Properties[i].Value);
						}
						else if (element.Properties[i].Value is Element)
						{
							WriteComplexElement((Element)element.Properties[i].Value, writer);
						}
						else if (element.Properties[i].Value is PropertyDeclaration)
						{
							// We had a templatebinding with a converter=,property= syntax. The value
							// of the property was a Property instance in which case we should just
							// write out the value.
							Property prop = (Property)element.Properties[i];
							writer.Write(prop.Value.ToString());
						}
						else
						{
							throw new NotSupportedException();
						}
					}
				}
			}

			writer.Write("}");
		}

		private static bool IsExtension(object value)
		{
			Element element = value as Element;
			if (element != null)
			{
				if (element.Arguments.Count == 0)
				{
					foreach (Property property in element.Properties)
					{
						if (property.PropertyType == PropertyType.Declaration)
						{
							return false;
						}

						if (!IsExtension(property.Value))
						{
							return false;
						}

						// An element with property content such as the following should
						// not be considered an extension.
						//
						// e.g. <Button><Button.Content><sys:DateTime>12/25/2008</sys:DateTime></Button.Content></Button>
						//
						if (property.PropertyType == PropertyType.Content)
						{
							return false;
						}
					}
				}

				return true;
			}

			IList list = value as IList;
			if (list != null)
			{
				return false;
			}

			return true;
		}

		private void AddElementToTree(Element element, BamlBinaryReader reader)
		{
			if (this.rootElement == null)
			{
				this.rootElement = element;
			}
			else
			{
				Property property = this.elementStack.Peek() as Property;
				if (property != null)
				{
					switch (property.PropertyType)
					{
						case PropertyType.List:
						case PropertyType.Dictionary:
							IList elements = (IList)property.Value;
							elements.Add(element);
							break;

						case PropertyType.Complex:
							property.Value = element;
							break;

						default:
							throw new NotSupportedException();
					}
				}
				else
				{
					Element parent = this.elementStack.Peek() as Element;

					if (this.dictionaryKeyPositionTable.Contains(parent))
					{
						int currentPosition = (int) (reader.BaseStream.Position - 1);

						if (!this.dictionaryKeyStartTable.Contains(parent))
						{
							this.dictionaryKeyStartTable.Add(parent, currentPosition);
						}

						int startPosition = (int)this.dictionaryKeyStartTable[parent];

						int position = currentPosition - startPosition;

						IDictionary keyPositionTable = (IDictionary)this.dictionaryKeyPositionTable[parent];
						if ((keyPositionTable != null) && (keyPositionTable.Contains(position)))
						{
							IList list = (IList)keyPositionTable[position];
							foreach (Property keyProperty in list)
							{
								element.Properties.Add(keyProperty);
							}
						}
					}

					if (parent != null)
					{
						// The element could be a parameter to a constructor - e.g. the Type
						// for a ComponentResourceKey in which case it should be an argument
						// of that element.
						//
						if (this.constructorParameterTable.Count > 0 &&
							this.constructorParameterTable[this.constructorParameterTable.Count - 1] == parent)
						{
							parent.Arguments.Add(element);
						}
						else
							this.AddContent(parent, element);
					}
					else
					{
						throw new NotSupportedException();
					}
				}
			}
		}

		private void ReadAssemblyInfo(BamlBinaryReader reader)
		{
			short assemblyIdentifier = reader.ReadInt16();
			string assemblyName = reader.ReadString();
			this.assemblyTable.Add(assemblyIdentifier, assemblyName);
		}
	
		private void ReadPresentationOptionsAttribute(BamlBinaryReader reader)
		{
			string value = reader.ReadString();
			short nameIdentifier = reader.ReadInt16();
	
			Property property = new Property(PropertyType.Value);
			property.PropertyDeclaration = new PropertyDeclaration("PresentationOptions:" + this.stringTable[nameIdentifier]);
			property.Value = value;
	
			Element element = (Element)this.elementStack.Peek();
			element.Properties.Add(property);
		}
	
		private void ReadStringInfo(BamlBinaryReader reader)
		{
			short stringIdentifier = reader.ReadInt16();
			string value = reader.ReadString();

			// This isn't a bug but more of a usability issue. MS tends to use 
			// single character identifiers which makes it difficult to find 
			// the associated resource.
			//
			if (null != value && value.Length == 1)
				value = string.Format("[{0}] {1}", stringIdentifier, value);

			this.stringTable.Add(stringIdentifier, value);
		}
	
		private void ReadTypeInfo(BamlBinaryReader reader)
		{
			short typeIdentifier = reader.ReadInt16();
			short assemblyIdentifier = reader.ReadInt16();
			string typeFullName = reader.ReadString();
	
			assemblyIdentifier = (short) (assemblyIdentifier & 0x0fff);
			string assembly = (string) this.assemblyTable[assemblyIdentifier];

			TypeDeclaration typeDeclaration = null;

			int index = typeFullName.LastIndexOf('.');
			if (index != -1)
			{
				string name = typeFullName.Substring(index + 1);
				string namespaceName = typeFullName.Substring(0, index);
				typeDeclaration = new TypeDeclaration(name, namespaceName, assembly);
			}
			else
			{
				typeDeclaration = new TypeDeclaration(typeFullName, string.Empty, assembly);
			}

			this.typeTable.Add(typeIdentifier, typeDeclaration);
		}
	
		private void ReadAttributeInfo(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
			short ownerTypeIdentifier = reader.ReadInt16();
			BamlAttributeUsage attributeUsage = (BamlAttributeUsage) reader.ReadByte();
			string attributeName = reader.ReadString();

			TypeDeclaration declaringType = this.GetTypeDeclaration(ownerTypeIdentifier);
			PropertyDeclaration propertyName = new PropertyDeclaration(attributeName, declaringType);
			this.propertyTable.Add(attributeIdentifier, propertyName);
		}
	
		private void ReadElementStart(BamlBinaryReader reader)
		{
			short typeIdentifier = reader.ReadInt16();
			byte flags = reader.ReadByte(); // 1 = CreateUsingTypeConverter, 2 = Injected
	
			Element element = new Element();
			element.TypeDeclaration = this.GetTypeDeclaration(typeIdentifier);

			this.AddElementToTree(element, reader);

			this.elementStack.Push(element);
		}
	
		private void ReadElementEnd()
		{
			Property property = this.elementStack.Peek() as Property;
			if ((property != null) && (property.PropertyType == PropertyType.Dictionary))
			{
				property = null;
			}
			else
			{
				Element element = (Element)this.elementStack.Pop();
	
				Property contentProperty = this.GetContentProperty(element);
				if ((contentProperty != null) && (contentProperty.Value == null))
				{
					element.Properties.Remove(contentProperty);
				}

				if (element.TypeDeclaration == this.GetTypeDeclaration(-0x0078))
				{
					bool removeKey = false;

					for (int i = element.Properties.Count - 1; i >= 0; i--)
					{
						if ((element.Properties[i].PropertyDeclaration != null) && (element.Properties[i].PropertyDeclaration.Name == "DataType"))
						{
							removeKey = true;
							break;
						}
					}

					if (removeKey)
					{
						for (int i = element.Properties.Count - 1; i >= 0; i--)
						{
							if ((element.Properties[i].PropertyDeclaration != null) && (element.Properties[i].PropertyDeclaration.Name == "x:Key"))
							{
								element.Properties.Remove(element.Properties[i]);
							}
						}
					}
				}
			}
		}

		private void ReadStaticResourceStart(BamlBinaryReader reader)
		{
			short typeIdentifier = reader.ReadInt16();
			byte flags = reader.ReadByte(); // 1 = CreateUsingTypeConverter, 2 = Injected

			Element element = new Element();
			element.TypeDeclaration = this.GetTypeDeclaration(typeIdentifier);

			this.elementStack.Push(element);

			this.staticResourceTable.Add(element);
		}

		private void ReadStaticResourceEnd(BamlBinaryReader reader)
		{
			this.elementStack.Pop();
		}

		private void ReadKeyElementStart(BamlBinaryReader reader)
		{
			short typeIdentifier = reader.ReadInt16();

			byte flags = reader.ReadByte();
			bool createUsingTypeConverter = ((flags & 1) != 0);
			bool injected = ((flags & 2) != 0);

			int position = reader.ReadInt32();
			bool shared = reader.ReadBoolean();
			bool sharedSet = reader.ReadBoolean();
	
			Property keyProperty = new Property(PropertyType.Complex);
			keyProperty.PropertyDeclaration = new PropertyDeclaration("x:Key");
			// At least for the case where we are processing the key of a dictionary,
			// we should not assume that the key is a type. This is particularly true
			// when the type is a ComponentResourceKey.
			//
			//keyProperty.Value = this.CreateTypeExtension(typeIdentifier);
			keyProperty.Value = this.CreateTypeExtension(typeIdentifier, false);

			Element dictionary = (Element)this.elementStack.Peek();
			this.AddDictionaryEntry(dictionary, position, keyProperty);
			this.elementStack.Push(keyProperty.Value);
		}
	
		private void ReadKeyElementEnd()
		{
			Element parent = (Element) this.elementStack.Pop();
		}
	
		private void ReadConstructorParametersStart()
		{
			Element element = (Element) this.elementStack.Peek();
			this.constructorParameterTable.Add(element);
		}
	
		private void ReadConstructorParametersEnd()
		{
			Element element = (Element)this.elementStack.Peek();
			this.constructorParameterTable.Remove(element);
		}
	
		private void ReadConstructorParameterType(BamlBinaryReader reader)
		{
			short typeIdentifier = reader.ReadInt16();
	
			TypeDeclaration elementName = this.GetTypeDeclaration(typeIdentifier);
	
			Element element = (Element)this.elementStack.Peek();
			element.Arguments.Add(elementName);
		}

		private void ReadOptimizedStaticResource(BamlBinaryReader reader)
		{
			byte extension = reader.ReadByte(); // num1
			short valueIdentifier = reader.ReadInt16();

			bool typeExtension = ((extension & 1) == 1);
			bool staticExtension = ((extension & 2) == 2);

			object element = null;

			if (typeExtension)
			{
				Element innerElement = this.CreateTypeExtension(valueIdentifier);
				element = innerElement;
			}
			else if (staticExtension)
			{
				Element innerElement = new Element();
				innerElement.TypeDeclaration = new TypeDeclaration("x:Static");
				// innerElement.TypeDeclaration = new TypeDeclaration("StaticExtension", "System.Windows.Markup);

				ResourceName resourceName = (ResourceName) this.GetResourceName(valueIdentifier);
				innerElement.Arguments.Add(resourceName);

				element = innerElement;
			}
			else
			{
				string value = (string)this.stringTable[valueIdentifier];
				element = value;
			}

			this.staticResourceTable.Add(element);
		}
	
		private void ReadPropertyWithExtension(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
	
			// 0x025b StaticResource
			// 0x027a TemplateBinding
			// 0x00bd DynamicResource
			short extension = reader.ReadInt16();
			short valueIdentifier = reader.ReadInt16();

			bool typeExtension = ((extension & 0x4000) == 0x4000); 
			bool staticExtension = ((extension & 0x2000) == 0x2000);

			extension = (short) (extension & 0x0fff);
			
			Property property = new Property(PropertyType.Complex);
			property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);
	
			short typeIdentifier = (short) -(extension & 0x0fff);
	
			Element element = new Element();
			element.TypeDeclaration = this.GetTypeDeclaration(typeIdentifier);

			switch (extension)
			{
				case 0x00bd: // DynamicResource
				case 0x025b: // StaticResource
					{
						if (typeExtension)
						{
							Element innerElement = this.CreateTypeExtension(valueIdentifier);
							element.Arguments.Add(innerElement);
						}
						else if (staticExtension)
						{
							Element innerElement = new Element();
							innerElement.TypeDeclaration = new TypeDeclaration("x:Static");
							// innerElement.TypeDeclaration = new TypeDeclaration("StaticExtension", "System.Windows.Markup);

							ResourceName resourceName = (ResourceName) this.GetResourceName(valueIdentifier);
							innerElement.Arguments.Add(resourceName);

							element.Arguments.Add(innerElement);
						}
						else
						{
							string value = (string)this.stringTable[valueIdentifier];
							element.Arguments.Add(value);
						}
					}
					break;

				case 0x25a: // Static
					{
						ResourceName resourceName = (ResourceName) this.GetResourceName(valueIdentifier);
						element.Arguments.Add(resourceName);
					}
					break;

				case 0x027a: // TemplateBinding
					{
						PropertyDeclaration propertyName = this.GetPropertyDeclaration(valueIdentifier);
						element.Arguments.Add(propertyName);
					}
					break;
				
				default:
					throw new NotSupportedException("Unknown property with extension");
			}

			property.Value = element;
	
			Element parent = (Element) this.elementStack.Peek();
			parent.Properties.Add(property);
		}

		private void ReadStaticResourceIdentifier(BamlBinaryReader reader)
		{
			short staticResourceIdentifier = reader.ReadInt16();
			object staticResource = this.GetStaticResource(staticResourceIdentifier);
			Element staticResourceElement = new Element();
			staticResourceElement.TypeDeclaration = this.GetTypeDeclaration(-0x25b);
			staticResourceElement.Arguments.Add(staticResource);

			this.AddElementToTree(staticResourceElement, reader);
		}

		private void ReadPropertyWithStaticResourceIdentifier(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
			
			short staticResourceIdentifier = reader.ReadInt16();
			object staticResource = this.GetStaticResource(staticResourceIdentifier);
	
			Element staticResourcEelement = new Element();
			staticResourcEelement.TypeDeclaration = this.GetTypeDeclaration(-0x25b);
			staticResourcEelement.Arguments.Add(staticResource);
	
			Property property = new Property(PropertyType.Complex);
			property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);
			property.Value = staticResourcEelement;
	
			Element parent = (Element)this.elementStack.Peek();
			parent.Properties.Add(property);
		}
	
		private void ReadPropertyTypeReference(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
			short typeIdentifier = reader.ReadInt16();
	
			Property property = new Property(PropertyType.Complex);
			property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);
			property.Value = this.CreateTypeExtension(typeIdentifier);
	
			Element parent = (Element)this.elementStack.Peek();
			parent.Properties.Add(property);
		}
	
		private void ReadPropertyRecord(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
			string value = reader.ReadString();
	
			Property property = new Property(PropertyType.Value);
			property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);
			property.Value = value;
	
			Element element = (Element)this.elementStack.Peek();
			element.Properties.Add(property);
		}
	
		private void ReadPropertyWithConverter(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
			string value = reader.ReadString();
			short converterTypeIdentifier = reader.ReadInt16();
	
			Property property = new Property(PropertyType.Value);
			property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);
			property.Value = value;
	
			Element element = (Element)this.elementStack.Peek();
			element.Properties.Add(property);
		}
	
		private void ReadPropertyCustom(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
			short serializerTypeIdentifierOriginal = reader.ReadInt16();
            bool typeIdentifier = (serializerTypeIdentifierOriginal & 0x4000) == 0x4000;
            short serializerTypeIdentifier = (short)(serializerTypeIdentifierOriginal & ~0x4000);

			Property property = new Property(PropertyType.Value);
			property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);
	
			switch (serializerTypeIdentifier)
			{
				// PropertyReference
				case 0x0089:
					short identifier = reader.ReadInt16();
					if (typeIdentifier)
					{
						TypeDeclaration declaringType = this.GetTypeDeclaration(identifier);
						string propertyName = reader.ReadString();
						property.Value = new PropertyDeclaration(propertyName, declaringType);
					}
					else
					{
						property.Value = this.GetPropertyDeclaration(identifier);
					}
					break;
	
				case 0x002e: // Boolean
					int value = reader.ReadByte();
					property.Value = (value == 0x01) ? "true" : "false";
					break;
	
				case 0x02e8: // SolidColorBrush
					switch (reader.ReadByte())
					{
						case 0x01: // KnownSolidColor
							uint color = reader.ReadUInt32();
							switch (color)
							{
								case 0x00ffffff:
									property.Value = "Transparent";
									break;
	
								default:
									property.Value = KnownColors.KnownColorFromUInt(color);
									break;
							}
							break;
	
						case 0x02: // OtherColor
							property.Value = reader.ReadString();
							break;
	
						default:
							throw new NotSupportedException();
					}
					break;
	
				case 0x02e9: // IntCollection
					using (StringWriter writer = new StringWriter())
					{
						byte format = reader.ReadByte();
						int count = reader.ReadInt32();
	
						switch (format)
						{
							case 0x01: // Consecutive
								for (int i = 0; i < count; i++)
								{
									if (i != 0)
									{
										writer.Write(",");
									}
	
									int number = reader.ReadInt32();
									writer.Write(number.ToString());
									if (number > count)
									{
										break;
									}
								}
								break;
	
							case 0x02: // Byte
								for (int i = 0; i < count; i++)
								{
									if (i != 0)
									{
										writer.Write(",");
									}

									int number = reader.ReadByte();
									writer.Write(number.ToString());
								}
								break;
	
							case 0x03: // UInt16
								for (int i = 0; i < count; i++)
								{
									if (i != 0)
									{
										writer.Write(",");
									}

									int number = reader.ReadUInt16();
									writer.Write(number.ToString());
								}
								break;
	
							case 0x04: // UInt32
								throw new NotSupportedException();
	
							default:
								throw new NotSupportedException();
						}
	
						property.Value = writer.ToString();
					}
					break;
	
				case 0x02ea: // PathData
					property.Value = PathDataParser.ParseStreamGeometry(reader);
					break;
	
				case 0x02ec: // Point
					using (StringWriter writer = new StringWriter())
					{
						int count = reader.ReadInt32();
						for (int i = 0; i < count; i++)
						{
							if (i != 0)
							{
								writer.Write(" ");
							}
	
							for (int j = 0; j < 2; j++)
							{
								if (j != 0)
								{
									writer.Write(",");
								}
	
								double number = reader.ReadCompressedDouble();
								writer.Write(number.ToString());
							}
						}
	
						property.Value = writer.ToString();
					}
					break;
	
				case 0x02eb: // Point3D
				case 0x02f0: // Vector3D
					using (StringWriter writer = new StringWriter())
					{
						int count = reader.ReadInt32();
						for (int i = 0; i < count; i++)
						{
							if (i != 0)
							{
								writer.Write(" ");
							}
	
							for (int j = 0; j < 3; j++)
							{
								if (j != 0)
								{
									writer.Write(",");
								}
	
								double number = reader.ReadCompressedDouble();
								writer.Write(number.ToString());
							}
						}
	
						property.Value = writer.ToString();
					}
					break;
	
				default:
                    property.Value = String.Format("Not supported serializer type identifier: 0x{0:x}", serializerTypeIdentifier);
                    break;
                    //throw new NotSupportedException();
			}
	
			Element element = (Element)this.elementStack.Peek();
			element.Properties.Add(property);
		}

		private void ReadContentProperty(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
	
			Element element = (Element) this.elementStack.Peek();
	
			Property contentProperty = this.GetContentProperty(element);
			if (contentProperty == null)
			{
				contentProperty = new Property(PropertyType.Content);
				element.Properties.Add(contentProperty);
			}

			PropertyDeclaration propertyName = this.GetPropertyDeclaration(attributeIdentifier);
			if ((contentProperty.PropertyDeclaration != null) && (contentProperty.PropertyDeclaration != propertyName))
			{
				throw new NotSupportedException();
			}

			contentProperty.PropertyDeclaration = propertyName;
		}
	
		private void ReadPropertyListStart(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
	
			Property property = new Property(PropertyType.List);
			property.Value = new ArrayList();
			property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);
	
			Element element = (Element)this.elementStack.Peek();
			element.Properties.Add(property);
	
			this.elementStack.Push(property);
		}
	
		private void ReadPropertyListEnd()
		{
			Property property = (Property) this.elementStack.Pop();
			if (property.PropertyType != PropertyType.List)
			{
				throw new NotSupportedException();
			}
		}
	
		private void ReadPropertyDictionaryStart(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
	
			Property property = new Property(PropertyType.Dictionary);
			property.Value = new ArrayList();
			property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);
	
			Element element = (Element)this.elementStack.Peek();
			element.Properties.Add(property);
	
			this.elementStack.Push(property);
		}
	
		private void ReadPropertyDictionaryEnd()
		{
			Property property = (Property)this.elementStack.Pop();
			if (property.PropertyType != PropertyType.Dictionary)
			{
				throw new NotSupportedException();
			}
		}
	
		private void ReadPropertyComplexStart(BamlBinaryReader reader)
		{
			short attributeIdentifier = reader.ReadInt16();
	
			Property property = new Property(PropertyType.Complex);
			property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);

            //if (property.PropertyDeclaration.Name == "RelativeTransform")
            //{
            //    Console.WriteLine();
            //}

			Element element = (Element)this.elementStack.Peek();
			element.Properties.Add(property);
	
			this.elementStack.Push(property);
		}
	
		private void ReadPropertyComplexEnd()
		{
			Property property = (Property)this.elementStack.Pop();
			if (property.PropertyType != PropertyType.Complex)
			{
				throw new NotSupportedException();
			}
		}
	
		private void ReadXmlnsProperty(BamlBinaryReader reader)
		{
			string prefix = reader.ReadString();
			string xmlNamespace = reader.ReadString();
	
			string[] assemblies = new string[reader.ReadInt16()];
			for (int i = 0; i < assemblies.Length; i++)
			{
				assemblies[i] = (string) this.assemblyTable[reader.ReadInt16()];
			}

			Property property = new Property(PropertyType.Namespace);
			property.PropertyDeclaration = new PropertyDeclaration(prefix, new TypeDeclaration("XmlNamespace", null, null));
			property.Value = xmlNamespace;

			this.namespaceManager.AddMapping(prefix, xmlNamespace);
	
			Element element = (Element)this.elementStack.Peek();
			element.Properties.Add(property);
		}

		private void ReadNamespaceMapping(BamlBinaryReader reader)
		{
			string xmlNamespace = reader.ReadString();
			string clrNamespace = reader.ReadString();
			short assemblyIdentifier = reader.ReadInt16();
			string assembly = (string) this.assemblyTable[assemblyIdentifier];

			this.namespaceManager.AddNamespaceMapping(xmlNamespace, clrNamespace, assembly);
		}

		private void ReadDefAttribute(BamlBinaryReader reader)
		{
			string value = reader.ReadString();
			short attributeIdentifier = reader.ReadInt16();
	
			Property property = new Property(PropertyType.Declaration);
	
			switch (attributeIdentifier)
			{
				case -1:
					property.PropertyDeclaration = new PropertyDeclaration("x:Name");
					break;
	
				case -2:
					property.PropertyDeclaration = new PropertyDeclaration("x:Uid");
					break;
	
				default:
					property.PropertyDeclaration = this.GetPropertyDeclaration(attributeIdentifier);
					break;
			}
	
			property.Value = value;
	
			Element element = (Element)this.elementStack.Peek();
			element.Properties.Add(property);
		}
	
		private void ReadDefAttributeKeyString(BamlBinaryReader reader)
		{
			short valueIdentifier = reader.ReadInt16();
			int position = reader.ReadInt32();
			bool shared = reader.ReadBoolean();
			bool sharedSet = reader.ReadBoolean();
			
			string key = (string) this.stringTable[valueIdentifier];
			if (key == null)
			{
				throw new NotSupportedException();
			}
	
			Property keyProperty = new Property(PropertyType.Value);
			keyProperty.PropertyDeclaration = new PropertyDeclaration("x:Key");
			keyProperty.Value = key;
	
			Element dictionary = (Element)this.elementStack.Peek();

			this.AddDictionaryEntry(dictionary, position, keyProperty);
		}
	
		private void ReadDefAttributeKeyType(BamlBinaryReader reader)
		{
			short typeIdentifier = reader.ReadInt16();
			reader.ReadByte();
			int position = reader.ReadInt32();
			bool shared = reader.ReadBoolean();
			bool sharedSet = reader.ReadBoolean();
	
			Property keyProperty = new Property(PropertyType.Complex);
			keyProperty.PropertyDeclaration = new PropertyDeclaration("x:Key");
			keyProperty.Value = this.CreateTypeExtension(typeIdentifier);
	
			Element dictionary = (Element)this.elementStack.Peek();

			this.AddDictionaryEntry(dictionary, position, keyProperty);
		}
	
		private void ReadText(BamlBinaryReader reader)
		{
			string value = reader.ReadString();
			ReadText(value);
		}

		private void ReadText(string value)
		{	
			Element parent = (Element) this.elementStack.Peek();
			if (this.constructorParameterTable.Contains(parent))
			{
				parent.Arguments.Add(value);
			}
			else
			{
				this.AddContent(parent, value);
			}
		}

		private void ReadTextWithId(BamlBinaryReader reader)
		{
			short id = reader.ReadInt16();
			string value = this.stringTable[id] as string;
			ReadText(value);
		}
	
		private void ReadTextWithConverter(BamlBinaryReader reader)
		{
			string value = reader.ReadString();
			short converterTypeIdentifier = reader.ReadInt16();
			ReadText(value);	
		}
	
		private void AddContent(Element parent, object content)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
	
			Property contentProperty = this.GetContentProperty(parent);
			if (contentProperty == null)
			{
				contentProperty = new Property(PropertyType.Content);
				parent.Properties.Add(contentProperty);
			}
	
			if (contentProperty.Value != null)
			{
				if (contentProperty.Value is string)
				{
					IList value = new ArrayList();
					value.Add(contentProperty.Value);
					contentProperty.Value = value;
				}
				
				if (contentProperty.Value is IList)
				{
					IList value = (IList) contentProperty.Value;
					value.Add(content);
				}
				else
				{
					throw new NotSupportedException();
				}
			}
			else
			{
				if (content is string)
				{
					contentProperty.Value = content;
				}
				else
				{
					IList value = new ArrayList();
					value.Add(content);
					contentProperty.Value = value;
				}
			}
		}
	
		private void AddDictionaryEntry(Element dictionary, int position, Property keyProperty)
		{
			IDictionary table = (IDictionary)this.dictionaryKeyPositionTable[dictionary];
			if (table == null)
			{
				table = new Hashtable();
				this.dictionaryKeyPositionTable.Add(dictionary, table);
			}

			IList list = (IList) table[position];
			if (list == null)
			{
				list = new ArrayList();
				table.Add(position, list);
			}

			list.Add(keyProperty);
		}
	
		private Element CreateTypeExtension(short typeIdentifier)
		{
			return CreateTypeExtension(typeIdentifier, true);
		}

		private Element CreateTypeExtension(short typeIdentifier, bool wrapInType)
		{
			Element element = new Element();
			element.TypeDeclaration = new TypeDeclaration("x:Type");
			// element.TypeDeclaration = new TypeDeclaration("TypeExtension", "System.Windows.Markup");

			TypeDeclaration typeDeclaration = this.GetTypeDeclaration(typeIdentifier);
			if (typeDeclaration == null)
			{
				throw new NotSupportedException();
			}

			if (false == wrapInType)
				element.TypeDeclaration = typeDeclaration;
			else
				element.Arguments.Add(typeDeclaration);

			return element;
		}
	
		private Property GetContentProperty(Element parent)
		{
			foreach (Property property in parent.Properties)
			{
				if (property.PropertyType == PropertyType.Content)
				{
					return property;
				}
			}
	
			return null;
		}

		private TypeDeclaration GetTypeDeclaration(short identifier)
		{
			TypeDeclaration typeDeclaration = null;
	
			if (identifier >= 0)
			{
				typeDeclaration = (TypeDeclaration) this.typeTable[identifier];
			}
			else
			{
				if (this.knownTypeTable == null)
				{
					this.Initialize();
				}

				typeDeclaration = this.knownTypeTable[-identifier];
			}

			// if an xml namespace prefix has been mapped for the specified assembly/clrnamespace
			// use its prefix in the returned type declaration. we have to do this here because
			// later on we may not have access to the mapping information
			string xmlNs = this.namespaceManager.GetXmlNamespace(typeDeclaration);

			if (null != xmlNs)
			{
				string prefix = this.namespaceManager.GetPrefix(xmlNs);

				if (null != prefix)
					typeDeclaration = typeDeclaration.Copy(prefix);
			}

			if (typeDeclaration == null)
			{
				throw new NotSupportedException();
			}

			return typeDeclaration;
		}

		private PropertyDeclaration GetPropertyDeclaration(short identifier)
		{
			PropertyDeclaration propertyDeclaration = null;
	
			if (identifier >= 0)
			{
				propertyDeclaration  = (PropertyDeclaration) this.propertyTable[identifier];
			}
			else
			{
				if (this.knownPropertyTable == null)
				{
					this.Initialize();
				}

				propertyDeclaration = this.knownPropertyTable[-identifier];
			}

			if (propertyDeclaration == null)
			{
                //wicky.patch.start
				//throw new NotSupportedException();
                propertyDeclaration = new PropertyDeclaration(String.Format("UnknownIdentifier{0}", identifier), knownTypeTable[0x0000]);
                //wicky.patch.end
			}

			return propertyDeclaration;
		}

		private object GetStaticResource(short identifier)
		{
			object resource = this.staticResourceTable[identifier];
			return resource;
		}

		private object GetResourceName(short identifier)
		{
			if (identifier >= 0)
			{
				PropertyDeclaration propertyName = (PropertyDeclaration)this.propertyTable[identifier];
				return new ResourceName(propertyName.Name);
			}
			else
			{
				if (knownResourceTable.Count == 0)
				{
					this.Initialize();
				}
	
				identifier = (short)-identifier;
				if (identifier > 0x00e8)
				{
					identifier -= 0x00e8;
				}

				ResourceName resourceName = (ResourceName)this.knownResourceTable[(int)identifier];
				return resourceName;
			}
		}

		private string GetAssembly(string assemblyName)
		{
			return assemblyName;
		}

		private void Initialize()
		{
			knownTypeTable = new TypeDeclaration[0x02f8];
			knownTypeTable[0x0000] = new TypeDeclaration(string.Empty, string.Empty, string.Empty); // Unknown
			knownTypeTable[0x0001] = new TypeDeclaration("AccessText", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0002] = new TypeDeclaration("AdornedElementPlaceholder", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0003] = new TypeDeclaration("Adorner", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0004] = new TypeDeclaration("AdornerDecorator", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0005] = new TypeDeclaration("AdornerLayer", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0006] = new TypeDeclaration("AffineTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0007] = new TypeDeclaration("AmbientLight", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0008] = new TypeDeclaration("AnchoredBlock", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0009] = new TypeDeclaration("Animatable", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x000a] = new TypeDeclaration("AnimationClock", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x000b] = new TypeDeclaration("AnimationTimeline", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x000c] = new TypeDeclaration("Application", "System.Net.Mime", this.GetAssembly("System"));
			knownTypeTable[0x000d] = new TypeDeclaration("ArcSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x000e] = new TypeDeclaration("ArrayExtension", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x000f] = new TypeDeclaration("AxisAngleRotation3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0010] = new TypeDeclaration("BaseIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0011] = new TypeDeclaration("BeginStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0012] = new TypeDeclaration("BevelBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0013] = new TypeDeclaration("BezierSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0014] = new TypeDeclaration("Binding", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0015] = new TypeDeclaration("BindingBase", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0016] = new TypeDeclaration("BindingExpression", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0017] = new TypeDeclaration("BindingExpressionBase", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0018] = new TypeDeclaration("BindingListCollectionView", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0019] = new TypeDeclaration("BitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x001a] = new TypeDeclaration("BitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x001b] = new TypeDeclaration("BitmapEffectCollection", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x001c] = new TypeDeclaration("BitmapEffectGroup", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x001d] = new TypeDeclaration("BitmapEffectInput", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x001e] = new TypeDeclaration("BitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x001f] = new TypeDeclaration("BitmapFrame", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0020] = new TypeDeclaration("BitmapImage", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0021] = new TypeDeclaration("BitmapMetadata", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0022] = new TypeDeclaration("BitmapPalette", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0023] = new TypeDeclaration("BitmapSource", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0024] = new TypeDeclaration("Block", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0025] = new TypeDeclaration("BlockUIContainer", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0026] = new TypeDeclaration("BlurBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0027] = new TypeDeclaration("BmpBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0028] = new TypeDeclaration("BmpBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0029] = new TypeDeclaration("Bold", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x002b] = new TypeDeclaration("Boolean", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x002c] = new TypeDeclaration("BooleanAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x002d] = new TypeDeclaration("BooleanAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x002e] = new TypeDeclaration("BooleanConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x002f] = new TypeDeclaration("BooleanKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0030] = new TypeDeclaration("BooleanKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0031] = new TypeDeclaration("BooleanToVisibilityConverter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x002a] = new TypeDeclaration("BoolIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0032] = new TypeDeclaration("Border", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0033] = new TypeDeclaration("BorderGapMaskConverter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0034] = new TypeDeclaration("Brush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0035] = new TypeDeclaration("BrushConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0036] = new TypeDeclaration("BulletDecorator", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0037] = new TypeDeclaration("Button", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0038] = new TypeDeclaration("ButtonBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0039] = new TypeDeclaration("Byte", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x003a] = new TypeDeclaration("ByteAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x003b] = new TypeDeclaration("ByteAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x003c] = new TypeDeclaration("ByteAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x003d] = new TypeDeclaration("ByteConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x003e] = new TypeDeclaration("ByteKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x003f] = new TypeDeclaration("ByteKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0040] = new TypeDeclaration("CachedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0041] = new TypeDeclaration("Camera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0042] = new TypeDeclaration("Canvas", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0043] = new TypeDeclaration("Char", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x0044] = new TypeDeclaration("CharAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0045] = new TypeDeclaration("CharAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0046] = new TypeDeclaration("CharConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x0047] = new TypeDeclaration("CharIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0048] = new TypeDeclaration("CharKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0049] = new TypeDeclaration("CharKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x004a] = new TypeDeclaration("CheckBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x004b] = new TypeDeclaration("Clock", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x004c] = new TypeDeclaration("ClockController", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x004d] = new TypeDeclaration("ClockGroup", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x004e] = new TypeDeclaration("CollectionContainer", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x004f] = new TypeDeclaration("CollectionView", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0050] = new TypeDeclaration("CollectionViewSource", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0051] = new TypeDeclaration("Color", "Microsoft.Win32", this.GetAssembly("mscorlib"));
			knownTypeTable[0x0052] = new TypeDeclaration("ColorAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0053] = new TypeDeclaration("ColorAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0054] = new TypeDeclaration("ColorAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0055] = new TypeDeclaration("ColorConvertedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0056] = new TypeDeclaration("ColorConvertedBitmapExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0057] = new TypeDeclaration("ColorConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0058] = new TypeDeclaration("ColorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0059] = new TypeDeclaration("ColorKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x005a] = new TypeDeclaration("ColumnDefinition", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x005b] = new TypeDeclaration("CombinedGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x005c] = new TypeDeclaration("ComboBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x005d] = new TypeDeclaration("ComboBoxItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x005e] = new TypeDeclaration("CommandConverter", "System.Windows.Input", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x005f] = new TypeDeclaration("ComponentResourceKey", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0060] = new TypeDeclaration("ComponentResourceKeyConverter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0061] = new TypeDeclaration("CompositionTarget", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0062] = new TypeDeclaration("Condition", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0063] = new TypeDeclaration("ContainerVisual", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0064] = new TypeDeclaration("ContentControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0065] = new TypeDeclaration("ContentElement", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0066] = new TypeDeclaration("ContentPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0067] = new TypeDeclaration("ContentPropertyAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0068] = new TypeDeclaration("ContentWrapperAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0069] = new TypeDeclaration("ContextMenu", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x006a] = new TypeDeclaration("ContextMenuService", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x006b] = new TypeDeclaration("Control", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x006d] = new TypeDeclaration("ControllableStoryboardAction", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x006c] = new TypeDeclaration("ControlTemplate", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x006e] = new TypeDeclaration("CornerRadius", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x006f] = new TypeDeclaration("CornerRadiusConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0070] = new TypeDeclaration("CroppedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0071] = new TypeDeclaration("CultureInfo", "System.Globalization", this.GetAssembly("mscorlib"));
			knownTypeTable[0x0072] = new TypeDeclaration("CultureInfoConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x0073] = new TypeDeclaration("CultureInfoIetfLanguageTagConverter", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0074] = new TypeDeclaration("Cursor", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0075] = new TypeDeclaration("CursorConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0076] = new TypeDeclaration("DashStyle", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0077] = new TypeDeclaration("DataChangedEventManager", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0078] = new TypeDeclaration("DataTemplate", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0079] = new TypeDeclaration("DataTemplateKey", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x007a] = new TypeDeclaration("DataTrigger", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x007b] = new TypeDeclaration("DateTime", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x007c] = new TypeDeclaration("DateTimeConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x007d] = new TypeDeclaration("DateTimeConverter2", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x007e] = new TypeDeclaration("Decimal", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x007f] = new TypeDeclaration("DecimalAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0080] = new TypeDeclaration("DecimalAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0081] = new TypeDeclaration("DecimalAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0082] = new TypeDeclaration("DecimalConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x0083] = new TypeDeclaration("DecimalKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0084] = new TypeDeclaration("DecimalKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0085] = new TypeDeclaration("Decorator", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0086] = new TypeDeclaration("DefinitionBase", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0087] = new TypeDeclaration("DependencyObject", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0088] = new TypeDeclaration("DependencyProperty", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0089] = new TypeDeclaration("DependencyPropertyConverter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x008a] = new TypeDeclaration("DialogResultConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x008b] = new TypeDeclaration("DiffuseMaterial", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x008c] = new TypeDeclaration("DirectionalLight", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x008d] = new TypeDeclaration("DiscreteBooleanKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x008e] = new TypeDeclaration("DiscreteByteKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x008f] = new TypeDeclaration("DiscreteCharKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0090] = new TypeDeclaration("DiscreteColorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0091] = new TypeDeclaration("DiscreteDecimalKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0092] = new TypeDeclaration("DiscreteDoubleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0093] = new TypeDeclaration("DiscreteInt16KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0094] = new TypeDeclaration("DiscreteInt32KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0095] = new TypeDeclaration("DiscreteInt64KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0096] = new TypeDeclaration("DiscreteMatrixKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0097] = new TypeDeclaration("DiscreteObjectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0098] = new TypeDeclaration("DiscretePoint3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0099] = new TypeDeclaration("DiscretePointKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x009a] = new TypeDeclaration("DiscreteQuaternionKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x009b] = new TypeDeclaration("DiscreteRectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x009c] = new TypeDeclaration("DiscreteRotation3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x009d] = new TypeDeclaration("DiscreteSingleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x009e] = new TypeDeclaration("DiscreteSizeKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x009f] = new TypeDeclaration("DiscreteStringKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00a0] = new TypeDeclaration("DiscreteThicknessKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00a1] = new TypeDeclaration("DiscreteVector3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00a2] = new TypeDeclaration("DiscreteVectorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00a3] = new TypeDeclaration("DockPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00a4] = new TypeDeclaration("DocumentPageView", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00a5] = new TypeDeclaration("DocumentReference", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00a6] = new TypeDeclaration("DocumentViewer", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00a7] = new TypeDeclaration("DocumentViewerBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00a8] = new TypeDeclaration("Double", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x00a9] = new TypeDeclaration("DoubleAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00aa] = new TypeDeclaration("DoubleAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00ab] = new TypeDeclaration("DoubleAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00ac] = new TypeDeclaration("DoubleAnimationUsingPath", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00ad] = new TypeDeclaration("DoubleCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00ae] = new TypeDeclaration("DoubleCollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00af] = new TypeDeclaration("DoubleConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x00b0] = new TypeDeclaration("DoubleIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00b1] = new TypeDeclaration("DoubleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00b2] = new TypeDeclaration("DoubleKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00b3] = new TypeDeclaration("Drawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00b4] = new TypeDeclaration("DrawingBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00b5] = new TypeDeclaration("DrawingCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00b6] = new TypeDeclaration("DrawingContext", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00b7] = new TypeDeclaration("DrawingGroup", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00b8] = new TypeDeclaration("DrawingImage", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00b9] = new TypeDeclaration("DrawingVisual", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00ba] = new TypeDeclaration("DropShadowBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00bb] = new TypeDeclaration("Duration", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00bc] = new TypeDeclaration("DurationConverter", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00bd] = new TypeDeclaration("DynamicResourceExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00be] = new TypeDeclaration("DynamicResourceExtensionConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00bf] = new TypeDeclaration("Ellipse", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00c0] = new TypeDeclaration("EllipseGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00c1] = new TypeDeclaration("EmbossBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00c2] = new TypeDeclaration("EmissiveMaterial", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00c3] = new TypeDeclaration("EnumConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x00c4] = new TypeDeclaration("EventManager", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00c5] = new TypeDeclaration("EventSetter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00c6] = new TypeDeclaration("EventTrigger", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00c7] = new TypeDeclaration("Expander", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00c8] = new TypeDeclaration("Expression", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x00c9] = new TypeDeclaration("ExpressionConverter", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x00ca] = new TypeDeclaration("Figure", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00cb] = new TypeDeclaration("FigureLength", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00cc] = new TypeDeclaration("FigureLengthConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00cd] = new TypeDeclaration("FixedDocument", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00ce] = new TypeDeclaration("FixedDocumentSequence", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00cf] = new TypeDeclaration("FixedPage", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00d0] = new TypeDeclaration("Floater", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00d1] = new TypeDeclaration("FlowDocument", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00d2] = new TypeDeclaration("FlowDocumentPageViewer", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00d3] = new TypeDeclaration("FlowDocumentReader", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00d4] = new TypeDeclaration("FlowDocumentScrollViewer", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00d5] = new TypeDeclaration("FocusManager", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00d6] = new TypeDeclaration("FontFamily", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00d7] = new TypeDeclaration("FontFamilyConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00d8] = new TypeDeclaration("FontSizeConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00d9] = new TypeDeclaration("FontStretch", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00da] = new TypeDeclaration("FontStretchConverter", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00db] = new TypeDeclaration("FontStyle", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00dc] = new TypeDeclaration("FontStyleConverter", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00dd] = new TypeDeclaration("FontWeight", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00de] = new TypeDeclaration("FontWeightConverter", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00df] = new TypeDeclaration("FormatConvertedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00e0] = new TypeDeclaration("Frame", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00e1] = new TypeDeclaration("FrameworkContentElement", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00e2] = new TypeDeclaration("FrameworkElement", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00e3] = new TypeDeclaration("FrameworkElementFactory", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00e4] = new TypeDeclaration("FrameworkPropertyMetadata", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00e5] = new TypeDeclaration("FrameworkPropertyMetadataOptions", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00e6] = new TypeDeclaration("FrameworkRichTextComposition", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00e7] = new TypeDeclaration("FrameworkTemplate", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00e8] = new TypeDeclaration("FrameworkTextComposition", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00e9] = new TypeDeclaration("Freezable", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x00ea] = new TypeDeclaration("GeneralTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00eb] = new TypeDeclaration("GeneralTransformCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00ec] = new TypeDeclaration("GeneralTransformGroup", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00ed] = new TypeDeclaration("Geometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00ee] = new TypeDeclaration("Geometry3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00ef] = new TypeDeclaration("GeometryCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00f0] = new TypeDeclaration("GeometryConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00f1] = new TypeDeclaration("GeometryDrawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00f2] = new TypeDeclaration("GeometryGroup", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00f3] = new TypeDeclaration("GeometryModel3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00f4] = new TypeDeclaration("GestureRecognizer", "System.Windows.Ink", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00f5] = new TypeDeclaration("GifBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00f6] = new TypeDeclaration("GifBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00f7] = new TypeDeclaration("GlyphRun", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00f8] = new TypeDeclaration("GlyphRunDrawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00fa] = new TypeDeclaration("Glyphs", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00f9] = new TypeDeclaration("GlyphTypeface", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00fb] = new TypeDeclaration("GradientBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00fc] = new TypeDeclaration("GradientStop", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00fd] = new TypeDeclaration("GradientStopCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x00fe] = new TypeDeclaration("Grid", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x00ff] = new TypeDeclaration("GridLength", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0100] = new TypeDeclaration("GridLengthConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0101] = new TypeDeclaration("GridSplitter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0102] = new TypeDeclaration("GridView", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0103] = new TypeDeclaration("GridViewColumn", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0104] = new TypeDeclaration("GridViewColumnHeader", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0105] = new TypeDeclaration("GridViewHeaderRowPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0106] = new TypeDeclaration("GridViewRowPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0107] = new TypeDeclaration("GridViewRowPresenterBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0108] = new TypeDeclaration("GroupBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0109] = new TypeDeclaration("GroupItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x010a] = new TypeDeclaration("Guid", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x010b] = new TypeDeclaration("GuidConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x010c] = new TypeDeclaration("GuidelineSet", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x010d] = new TypeDeclaration("HeaderedContentControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x010e] = new TypeDeclaration("HeaderedItemsControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x010f] = new TypeDeclaration("HierarchicalDataTemplate", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0110] = new TypeDeclaration("HostVisual", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0111] = new TypeDeclaration("Hyperlink", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0112] = new TypeDeclaration("IAddChild", "System.Windows.Markup", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0113] = new TypeDeclaration("IAddChildInternal", "System.Windows.Markup", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0114] = new TypeDeclaration("ICommand", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0115] = new TypeDeclaration("IComponentConnector", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0118] = new TypeDeclaration("IconBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0119] = new TypeDeclaration("Image", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x011a] = new TypeDeclaration("ImageBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x011b] = new TypeDeclaration("ImageDrawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x011c] = new TypeDeclaration("ImageMetadata", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x011d] = new TypeDeclaration("ImageSource", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x011e] = new TypeDeclaration("ImageSourceConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0116] = new TypeDeclaration("INameScope", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0120] = new TypeDeclaration("InkCanvas", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0121] = new TypeDeclaration("InkPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0122] = new TypeDeclaration("Inline", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0123] = new TypeDeclaration("InlineCollection", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0124] = new TypeDeclaration("InlineUIContainer", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x011f] = new TypeDeclaration("InPlaceBitmapMetadataWriter", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0125] = new TypeDeclaration("InputBinding", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0126] = new TypeDeclaration("InputDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0127] = new TypeDeclaration("InputLanguageManager", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0128] = new TypeDeclaration("InputManager", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0129] = new TypeDeclaration("InputMethod", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x012a] = new TypeDeclaration("InputScope", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x012b] = new TypeDeclaration("InputScopeConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x012c] = new TypeDeclaration("InputScopeName", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x012d] = new TypeDeclaration("InputScopeNameConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x012e] = new TypeDeclaration("Int16", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x012f] = new TypeDeclaration("Int16Animation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0130] = new TypeDeclaration("Int16AnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0131] = new TypeDeclaration("Int16AnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0132] = new TypeDeclaration("Int16Converter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x0133] = new TypeDeclaration("Int16KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0134] = new TypeDeclaration("Int16KeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0135] = new TypeDeclaration("Int32", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x0136] = new TypeDeclaration("Int32Animation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0137] = new TypeDeclaration("Int32AnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0138] = new TypeDeclaration("Int32AnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0139] = new TypeDeclaration("Int32Collection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x013a] = new TypeDeclaration("Int32CollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x013b] = new TypeDeclaration("Int32Converter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x013c] = new TypeDeclaration("Int32KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x013d] = new TypeDeclaration("Int32KeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x013e] = new TypeDeclaration("Int32Rect", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x013f] = new TypeDeclaration("Int32RectConverter", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0140] = new TypeDeclaration("Int64", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x0141] = new TypeDeclaration("Int64Animation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0142] = new TypeDeclaration("Int64AnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0143] = new TypeDeclaration("Int64AnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0144] = new TypeDeclaration("Int64Converter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x0145] = new TypeDeclaration("Int64KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0146] = new TypeDeclaration("Int64KeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0117] = new TypeDeclaration("IStyleConnector", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0147] = new TypeDeclaration("Italic", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0148] = new TypeDeclaration("ItemCollection", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0149] = new TypeDeclaration("ItemsControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x014a] = new TypeDeclaration("ItemsPanelTemplate", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x014b] = new TypeDeclaration("ItemsPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x014c] = new TypeDeclaration("JournalEntry", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x014d] = new TypeDeclaration("JournalEntryListConverter", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x014e] = new TypeDeclaration("JournalEntryUnifiedViewConverter", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x014f] = new TypeDeclaration("JpegBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0150] = new TypeDeclaration("JpegBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0151] = new TypeDeclaration("KeyBinding", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0159] = new TypeDeclaration("KeyboardDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0152] = new TypeDeclaration("KeyConverter", "System.Windows.Input", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0153] = new TypeDeclaration("KeyGesture", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0154] = new TypeDeclaration("KeyGestureConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0155] = new TypeDeclaration("KeySpline", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0156] = new TypeDeclaration("KeySplineConverter", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0157] = new TypeDeclaration("KeyTime", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0158] = new TypeDeclaration("KeyTimeConverter", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x015a] = new TypeDeclaration("Label", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x015b] = new TypeDeclaration("LateBoundBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x015c] = new TypeDeclaration("LengthConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x015d] = new TypeDeclaration("Light", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x015e] = new TypeDeclaration("Line", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0162] = new TypeDeclaration("LinearByteKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0163] = new TypeDeclaration("LinearColorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0164] = new TypeDeclaration("LinearDecimalKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0165] = new TypeDeclaration("LinearDoubleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0166] = new TypeDeclaration("LinearGradientBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0167] = new TypeDeclaration("LinearInt16KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0168] = new TypeDeclaration("LinearInt32KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0169] = new TypeDeclaration("LinearInt64KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x016a] = new TypeDeclaration("LinearPoint3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x016b] = new TypeDeclaration("LinearPointKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x016c] = new TypeDeclaration("LinearQuaternionKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x016d] = new TypeDeclaration("LinearRectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x016e] = new TypeDeclaration("LinearRotation3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x016f] = new TypeDeclaration("LinearSingleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0170] = new TypeDeclaration("LinearSizeKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0171] = new TypeDeclaration("LinearThicknessKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0172] = new TypeDeclaration("LinearVector3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0173] = new TypeDeclaration("LinearVectorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x015f] = new TypeDeclaration("LineBreak", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0160] = new TypeDeclaration("LineGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0161] = new TypeDeclaration("LineSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0174] = new TypeDeclaration("List", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0175] = new TypeDeclaration("ListBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0176] = new TypeDeclaration("ListBoxItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0177] = new TypeDeclaration("ListCollectionView", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0178] = new TypeDeclaration("ListItem", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0179] = new TypeDeclaration("ListView", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x017a] = new TypeDeclaration("ListViewItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x017b] = new TypeDeclaration("Localization", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x017c] = new TypeDeclaration("LostFocusEventManager", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x017d] = new TypeDeclaration("MarkupExtension", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x017e] = new TypeDeclaration("Material", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x017f] = new TypeDeclaration("MaterialCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0180] = new TypeDeclaration("MaterialGroup", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0181] = new TypeDeclaration("Matrix", "System.Windows.Media", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0182] = new TypeDeclaration("Matrix3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0183] = new TypeDeclaration("Matrix3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0184] = new TypeDeclaration("MatrixAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0185] = new TypeDeclaration("MatrixAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0186] = new TypeDeclaration("MatrixAnimationUsingPath", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0187] = new TypeDeclaration("MatrixCamera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0188] = new TypeDeclaration("MatrixConverter", "System.Windows.Media", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0189] = new TypeDeclaration("MatrixKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x018a] = new TypeDeclaration("MatrixKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x018b] = new TypeDeclaration("MatrixTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x018c] = new TypeDeclaration("MatrixTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x018d] = new TypeDeclaration("MediaClock", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x018e] = new TypeDeclaration("MediaElement", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x018f] = new TypeDeclaration("MediaPlayer", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0190] = new TypeDeclaration("MediaTimeline", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0191] = new TypeDeclaration("Menu", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0192] = new TypeDeclaration("MenuBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0193] = new TypeDeclaration("MenuItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0194] = new TypeDeclaration("MenuScrollingVisibilityConverter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0195] = new TypeDeclaration("MeshGeometry3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0196] = new TypeDeclaration("Model3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0197] = new TypeDeclaration("Model3DCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0198] = new TypeDeclaration("Model3DGroup", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0199] = new TypeDeclaration("ModelVisual3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x019a] = new TypeDeclaration("ModifierKeysConverter", "System.Windows.Input", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x019b] = new TypeDeclaration("MouseActionConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x019c] = new TypeDeclaration("MouseBinding", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x019d] = new TypeDeclaration("MouseDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x019e] = new TypeDeclaration("MouseGesture", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x019f] = new TypeDeclaration("MouseGestureConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01a0] = new TypeDeclaration("MultiBinding", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01a1] = new TypeDeclaration("MultiBindingExpression", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01a2] = new TypeDeclaration("MultiDataTrigger", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01a3] = new TypeDeclaration("MultiTrigger", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01a4] = new TypeDeclaration("NameScope", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01a5] = new TypeDeclaration("NavigationWindow", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01a7] = new TypeDeclaration("NullableBoolConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01a8] = new TypeDeclaration("NullableConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x01a6] = new TypeDeclaration("NullExtension", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01a9] = new TypeDeclaration("NumberSubstitution", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01aa] = new TypeDeclaration("Object", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x01ab] = new TypeDeclaration("ObjectAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01ac] = new TypeDeclaration("ObjectAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01ad] = new TypeDeclaration("ObjectDataProvider", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01ae] = new TypeDeclaration("ObjectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01af] = new TypeDeclaration("ObjectKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01b0] = new TypeDeclaration("OrthographicCamera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01b1] = new TypeDeclaration("OuterGlowBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01b2] = new TypeDeclaration("Page", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01b3] = new TypeDeclaration("PageContent", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01b4] = new TypeDeclaration("PageFunctionBase", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01b5] = new TypeDeclaration("Panel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01b6] = new TypeDeclaration("Paragraph", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01b7] = new TypeDeclaration("ParallelTimeline", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01b8] = new TypeDeclaration("ParserContext", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01b9] = new TypeDeclaration("PasswordBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01ba] = new TypeDeclaration("Path", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01bb] = new TypeDeclaration("PathFigure", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01bc] = new TypeDeclaration("PathFigureCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01bd] = new TypeDeclaration("PathFigureCollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01be] = new TypeDeclaration("PathGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01bf] = new TypeDeclaration("PathSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01c0] = new TypeDeclaration("PathSegmentCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01c1] = new TypeDeclaration("PauseStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01c2] = new TypeDeclaration("Pen", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01c3] = new TypeDeclaration("PerspectiveCamera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01c4] = new TypeDeclaration("PixelFormat", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01c5] = new TypeDeclaration("PixelFormatConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01c6] = new TypeDeclaration("PngBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01c7] = new TypeDeclaration("PngBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01c8] = new TypeDeclaration("Point", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x01c9] = new TypeDeclaration("Point3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01ca] = new TypeDeclaration("Point3DAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01cb] = new TypeDeclaration("Point3DAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01cc] = new TypeDeclaration("Point3DAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01cd] = new TypeDeclaration("Point3DCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01ce] = new TypeDeclaration("Point3DCollectionConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01cf] = new TypeDeclaration("Point3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d0] = new TypeDeclaration("Point3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d1] = new TypeDeclaration("Point3DKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d2] = new TypeDeclaration("Point4D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d3] = new TypeDeclaration("Point4DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d4] = new TypeDeclaration("PointAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d5] = new TypeDeclaration("PointAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d6] = new TypeDeclaration("PointAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d7] = new TypeDeclaration("PointAnimationUsingPath", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d8] = new TypeDeclaration("PointCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01d9] = new TypeDeclaration("PointCollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01da] = new TypeDeclaration("PointConverter", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x01db] = new TypeDeclaration("PointIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01dc] = new TypeDeclaration("PointKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01dd] = new TypeDeclaration("PointKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01de] = new TypeDeclaration("PointLight", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01df] = new TypeDeclaration("PointLightBase", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01e0] = new TypeDeclaration("PolyBezierSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01e3] = new TypeDeclaration("Polygon", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01e4] = new TypeDeclaration("Polyline", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01e1] = new TypeDeclaration("PolyLineSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01e2] = new TypeDeclaration("PolyQuadraticBezierSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01e5] = new TypeDeclaration("Popup", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01e6] = new TypeDeclaration("PresentationSource", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01e7] = new TypeDeclaration("PriorityBinding", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01e8] = new TypeDeclaration("PriorityBindingExpression", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01e9] = new TypeDeclaration("ProgressBar", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01ea] = new TypeDeclaration("ProjectionCamera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01eb] = new TypeDeclaration("PropertyPath", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01ec] = new TypeDeclaration("PropertyPathConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01ed] = new TypeDeclaration("QuadraticBezierSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01ee] = new TypeDeclaration("Quaternion", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01ef] = new TypeDeclaration("QuaternionAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01f0] = new TypeDeclaration("QuaternionAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01f1] = new TypeDeclaration("QuaternionAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01f2] = new TypeDeclaration("QuaternionConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01f3] = new TypeDeclaration("QuaternionKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01f4] = new TypeDeclaration("QuaternionKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01f5] = new TypeDeclaration("QuaternionRotation3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01f6] = new TypeDeclaration("RadialGradientBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01f7] = new TypeDeclaration("RadioButton", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01f8] = new TypeDeclaration("RangeBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x01f9] = new TypeDeclaration("Rect", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01fa] = new TypeDeclaration("Rect3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01fb] = new TypeDeclaration("Rect3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0202] = new TypeDeclaration("Rectangle", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0203] = new TypeDeclaration("RectangleGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01fc] = new TypeDeclaration("RectAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01fd] = new TypeDeclaration("RectAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01fe] = new TypeDeclaration("RectAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x01ff] = new TypeDeclaration("RectConverter", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0200] = new TypeDeclaration("RectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0201] = new TypeDeclaration("RectKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0204] = new TypeDeclaration("RelativeSource", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0205] = new TypeDeclaration("RemoveStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0206] = new TypeDeclaration("RenderOptions", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0207] = new TypeDeclaration("RenderTargetBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0208] = new TypeDeclaration("RepeatBehavior", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0209] = new TypeDeclaration("RepeatBehaviorConverter", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x020a] = new TypeDeclaration("RepeatButton", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x020b] = new TypeDeclaration("ResizeGrip", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x020c] = new TypeDeclaration("ResourceDictionary", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x020d] = new TypeDeclaration("ResourceKey", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x020e] = new TypeDeclaration("ResumeStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x020f] = new TypeDeclaration("RichTextBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0210] = new TypeDeclaration("RotateTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0211] = new TypeDeclaration("RotateTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0212] = new TypeDeclaration("Rotation3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0213] = new TypeDeclaration("Rotation3DAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0214] = new TypeDeclaration("Rotation3DAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0215] = new TypeDeclaration("Rotation3DAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0216] = new TypeDeclaration("Rotation3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0217] = new TypeDeclaration("Rotation3DKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0218] = new TypeDeclaration("RoutedCommand", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0219] = new TypeDeclaration("RoutedEvent", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x021a] = new TypeDeclaration("RoutedEventConverter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x021b] = new TypeDeclaration("RoutedUICommand", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x021c] = new TypeDeclaration("RoutingStrategy", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x021d] = new TypeDeclaration("RowDefinition", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x021e] = new TypeDeclaration("Run", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x021f] = new TypeDeclaration("RuntimeNamePropertyAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0220] = new TypeDeclaration("SByte", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x0221] = new TypeDeclaration("SByteConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x0222] = new TypeDeclaration("ScaleTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0223] = new TypeDeclaration("ScaleTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0224] = new TypeDeclaration("ScrollBar", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0225] = new TypeDeclaration("ScrollContentPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0226] = new TypeDeclaration("ScrollViewer", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0227] = new TypeDeclaration("Section", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0228] = new TypeDeclaration("SeekStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0229] = new TypeDeclaration("Selector", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x022a] = new TypeDeclaration("Separator", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x022b] = new TypeDeclaration("SetStoryboardSpeedRatio", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x022c] = new TypeDeclaration("Setter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x022d] = new TypeDeclaration("SetterBase", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x022e] = new TypeDeclaration("Shape", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x022f] = new TypeDeclaration("Single", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x0230] = new TypeDeclaration("SingleAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0231] = new TypeDeclaration("SingleAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0232] = new TypeDeclaration("SingleAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0233] = new TypeDeclaration("SingleConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x0234] = new TypeDeclaration("SingleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0235] = new TypeDeclaration("SingleKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0236] = new TypeDeclaration("Size", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x0237] = new TypeDeclaration("Size3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0238] = new TypeDeclaration("Size3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0239] = new TypeDeclaration("SizeAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x023a] = new TypeDeclaration("SizeAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x023b] = new TypeDeclaration("SizeAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x023c] = new TypeDeclaration("SizeConverter", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x023d] = new TypeDeclaration("SizeKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x023e] = new TypeDeclaration("SizeKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x023f] = new TypeDeclaration("SkewTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0240] = new TypeDeclaration("SkipStoryboardToFill", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0241] = new TypeDeclaration("Slider", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0242] = new TypeDeclaration("SolidColorBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0243] = new TypeDeclaration("SoundPlayerAction", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0244] = new TypeDeclaration("Span", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0245] = new TypeDeclaration("SpecularMaterial", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0246] = new TypeDeclaration("SpellCheck", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0247] = new TypeDeclaration("SplineByteKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0248] = new TypeDeclaration("SplineColorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0249] = new TypeDeclaration("SplineDecimalKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x024a] = new TypeDeclaration("SplineDoubleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x024b] = new TypeDeclaration("SplineInt16KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x024c] = new TypeDeclaration("SplineInt32KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x024d] = new TypeDeclaration("SplineInt64KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x024e] = new TypeDeclaration("SplinePoint3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x024f] = new TypeDeclaration("SplinePointKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0250] = new TypeDeclaration("SplineQuaternionKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0251] = new TypeDeclaration("SplineRectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0252] = new TypeDeclaration("SplineRotation3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0253] = new TypeDeclaration("SplineSingleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0254] = new TypeDeclaration("SplineSizeKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0255] = new TypeDeclaration("SplineThicknessKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0256] = new TypeDeclaration("SplineVector3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0257] = new TypeDeclaration("SplineVectorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0258] = new TypeDeclaration("SpotLight", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0259] = new TypeDeclaration("StackPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x025a] = new TypeDeclaration("StaticExtension", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x025b] = new TypeDeclaration("StaticResourceExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x025c] = new TypeDeclaration("StatusBar", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x025d] = new TypeDeclaration("StatusBarItem", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x025e] = new TypeDeclaration("StickyNoteControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x025f] = new TypeDeclaration("StopStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0260] = new TypeDeclaration("Storyboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0261] = new TypeDeclaration("StreamGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0262] = new TypeDeclaration("StreamGeometryContext", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0263] = new TypeDeclaration("StreamResourceInfo", "System.Windows.Resources", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0264] = new TypeDeclaration("String", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x0265] = new TypeDeclaration("StringAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0266] = new TypeDeclaration("StringAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0267] = new TypeDeclaration("StringConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x0268] = new TypeDeclaration("StringKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0269] = new TypeDeclaration("StringKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x026a] = new TypeDeclaration("StrokeCollection", "System.Windows.Ink", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x026b] = new TypeDeclaration("StrokeCollectionConverter", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x026c] = new TypeDeclaration("Style", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x026d] = new TypeDeclaration("Stylus", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x026e] = new TypeDeclaration("StylusDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x026f] = new TypeDeclaration("TabControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0270] = new TypeDeclaration("TabItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0272] = new TypeDeclaration("Table", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0273] = new TypeDeclaration("TableCell", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0274] = new TypeDeclaration("TableColumn", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0275] = new TypeDeclaration("TableRow", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0276] = new TypeDeclaration("TableRowGroup", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0277] = new TypeDeclaration("TabletDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0271] = new TypeDeclaration("TabPanel", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0278] = new TypeDeclaration("TemplateBindingExpression", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0279] = new TypeDeclaration("TemplateBindingExpressionConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x027a] = new TypeDeclaration("TemplateBindingExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x027b] = new TypeDeclaration("TemplateBindingExtensionConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x027c] = new TypeDeclaration("TemplateKey", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x027d] = new TypeDeclaration("TemplateKeyConverter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x027e] = new TypeDeclaration("TextBlock", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x027f] = new TypeDeclaration("TextBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0280] = new TypeDeclaration("TextBoxBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0281] = new TypeDeclaration("TextComposition", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0282] = new TypeDeclaration("TextCompositionManager", "System.Windows.Input", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0283] = new TypeDeclaration("TextDecoration", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0284] = new TypeDeclaration("TextDecorationCollection", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0285] = new TypeDeclaration("TextDecorationCollectionConverter", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0286] = new TypeDeclaration("TextEffect", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0287] = new TypeDeclaration("TextEffectCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0288] = new TypeDeclaration("TextElement", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0289] = new TypeDeclaration("TextSearch", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x028a] = new TypeDeclaration("ThemeDictionaryExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x028b] = new TypeDeclaration("Thickness", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x028c] = new TypeDeclaration("ThicknessAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x028d] = new TypeDeclaration("ThicknessAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x028e] = new TypeDeclaration("ThicknessAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x028f] = new TypeDeclaration("ThicknessConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0290] = new TypeDeclaration("ThicknessKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0291] = new TypeDeclaration("ThicknessKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0292] = new TypeDeclaration("Thumb", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0293] = new TypeDeclaration("TickBar", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x0294] = new TypeDeclaration("TiffBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0295] = new TypeDeclaration("TiffBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0296] = new TypeDeclaration("TileBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0299] = new TypeDeclaration("Timeline", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x029a] = new TypeDeclaration("TimelineCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x029b] = new TypeDeclaration("TimelineGroup", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x0297] = new TypeDeclaration("TimeSpan", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x0298] = new TypeDeclaration("TimeSpanConverter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x029c] = new TypeDeclaration("ToggleButton", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x029d] = new TypeDeclaration("ToolBar", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x029e] = new TypeDeclaration("ToolBarOverflowPanel", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x029f] = new TypeDeclaration("ToolBarPanel", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02a0] = new TypeDeclaration("ToolBarTray", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02a1] = new TypeDeclaration("ToolTip", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02a2] = new TypeDeclaration("ToolTipService", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02a3] = new TypeDeclaration("Track", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02a4] = new TypeDeclaration("Transform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02a5] = new TypeDeclaration("Transform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02a6] = new TypeDeclaration("Transform3DCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02a7] = new TypeDeclaration("Transform3DGroup", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02a8] = new TypeDeclaration("TransformCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02a9] = new TypeDeclaration("TransformConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02ab] = new TypeDeclaration("TransformedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02aa] = new TypeDeclaration("TransformGroup", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02ac] = new TypeDeclaration("TranslateTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02ad] = new TypeDeclaration("TranslateTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02ae] = new TypeDeclaration("TreeView", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02af] = new TypeDeclaration("TreeViewItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02b0] = new TypeDeclaration("Trigger", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02b1] = new TypeDeclaration("TriggerAction", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02b2] = new TypeDeclaration("TriggerBase", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02b3] = new TypeDeclaration("TypeExtension", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02b4] = new TypeDeclaration("TypeTypeConverter", "System.Net.Configuration", this.GetAssembly("System"));
			knownTypeTable[0x02b5] = new TypeDeclaration("Typography", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02b6] = new TypeDeclaration("UIElement", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02b7] = new TypeDeclaration("UInt16", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x02b8] = new TypeDeclaration("UInt16Converter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x02b9] = new TypeDeclaration("UInt32", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x02ba] = new TypeDeclaration("UInt32Converter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x02bb] = new TypeDeclaration("UInt64", "System", this.GetAssembly("mscorlib"));
			knownTypeTable[0x02bc] = new TypeDeclaration("UInt64Converter", "System.ComponentModel", this.GetAssembly("System"));
			knownTypeTable[0x02be] = new TypeDeclaration("Underline", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02bf] = new TypeDeclaration("UniformGrid", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02c0] = new TypeDeclaration("Uri", "System", this.GetAssembly("System"));
			knownTypeTable[0x02c1] = new TypeDeclaration("UriTypeConverter", "System", this.GetAssembly("System"));
			knownTypeTable[0x02c2] = new TypeDeclaration("UserControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02bd] = new TypeDeclaration("UShortIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02c3] = new TypeDeclaration("Validation", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02c4] = new TypeDeclaration("Vector", "System.Windows", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02c5] = new TypeDeclaration("Vector3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02c6] = new TypeDeclaration("Vector3DAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02c7] = new TypeDeclaration("Vector3DAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02c8] = new TypeDeclaration("Vector3DAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02c9] = new TypeDeclaration("Vector3DCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02ca] = new TypeDeclaration("Vector3DCollectionConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02cb] = new TypeDeclaration("Vector3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02cc] = new TypeDeclaration("Vector3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02cd] = new TypeDeclaration("Vector3DKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02ce] = new TypeDeclaration("VectorAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02cf] = new TypeDeclaration("VectorAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02d0] = new TypeDeclaration("VectorAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02d1] = new TypeDeclaration("VectorCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02d2] = new TypeDeclaration("VectorCollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02d3] = new TypeDeclaration("VectorConverter", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x02d4] = new TypeDeclaration("VectorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02d5] = new TypeDeclaration("VectorKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02d6] = new TypeDeclaration("VideoDrawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02d7] = new TypeDeclaration("ViewBase", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02d8] = new TypeDeclaration("Viewbox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02d9] = new TypeDeclaration("Viewport3D", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02da] = new TypeDeclaration("Viewport3DVisual", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02db] = new TypeDeclaration("VirtualizingPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02dc] = new TypeDeclaration("VirtualizingStackPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02dd] = new TypeDeclaration("Visual", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02de] = new TypeDeclaration("Visual3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02df] = new TypeDeclaration("VisualBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02e0] = new TypeDeclaration("VisualTarget", "System.Windows.Media", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02e1] = new TypeDeclaration("WeakEventManager", "System.Windows", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x02e2] = new TypeDeclaration("WhitespaceSignificantCollectionAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x02e3] = new TypeDeclaration("Window", "System.Windows", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02e4] = new TypeDeclaration("WmpBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02e5] = new TypeDeclaration("WmpBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02e6] = new TypeDeclaration("WrapPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02e7] = new TypeDeclaration("WriteableBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02e8] = new TypeDeclaration("XamlBrushSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02e9] = new TypeDeclaration("XamlInt32CollectionSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02ea] = new TypeDeclaration("XamlPathDataSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02eb] = new TypeDeclaration("XamlPoint3DCollectionSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02ec] = new TypeDeclaration("XamlPointCollectionSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02ed] = new TypeDeclaration("XamlReader", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02ee] = new TypeDeclaration("XamlStyleSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02ef] = new TypeDeclaration("XamlTemplateSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02f0] = new TypeDeclaration("XamlVector3DCollectionSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02f1] = new TypeDeclaration("XamlWriter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02f2] = new TypeDeclaration("XmlDataProvider", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02f3] = new TypeDeclaration("XmlLangPropertyAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
			knownTypeTable[0x02f4] = new TypeDeclaration("XmlLanguage", "System.Windows.Markup", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02f5] = new TypeDeclaration("XmlLanguageConverter", "System.Windows.Markup", this.GetAssembly("PresentationCore"));
			knownTypeTable[0x02f6] = new TypeDeclaration("XmlNamespaceMapping", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
			knownTypeTable[0x02f7] = new TypeDeclaration("ZoomPercentageConverter", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));

			knownPropertyTable = new PropertyDeclaration[0x010d];
			knownPropertyTable[0x0001] = new PropertyDeclaration("Text", knownTypeTable[0x0001]); // AccessText
			knownPropertyTable[0x0002] = new PropertyDeclaration("Storyboard", knownTypeTable[0x0011]); // BeginStoryboard
			knownPropertyTable[0x0003] = new PropertyDeclaration("Children", knownTypeTable[0x001c]); // BitmapEffectGroup
			knownPropertyTable[0x0004] = new PropertyDeclaration("Background", knownTypeTable[0x0032]); // Border
			knownPropertyTable[0x0005] = new PropertyDeclaration("BorderBrush", knownTypeTable[0x0032]); // Border
			knownPropertyTable[0x0006] = new PropertyDeclaration("BorderThickness", knownTypeTable[0x0032]); // Border
			knownPropertyTable[0x0007] = new PropertyDeclaration("Command", knownTypeTable[0x0038]); // ButtonBase
			knownPropertyTable[0x0008] = new PropertyDeclaration("CommandParameter", knownTypeTable[0x0038]); // ButtonBase
			knownPropertyTable[0x0009] = new PropertyDeclaration("CommandTarget", knownTypeTable[0x0038]); // ButtonBase
			knownPropertyTable[0x000a] = new PropertyDeclaration("IsPressed", knownTypeTable[0x0038]); // ButtonBase
			knownPropertyTable[0x000b] = new PropertyDeclaration("MaxWidth", knownTypeTable[0x005a]); // ColumnDefinition
			knownPropertyTable[0x000c] = new PropertyDeclaration("MinWidth", knownTypeTable[0x005a]); // ColumnDefinition
			knownPropertyTable[0x000d] = new PropertyDeclaration("Width", knownTypeTable[0x005a]); // ColumnDefinition
			knownPropertyTable[0x000e] = new PropertyDeclaration("Content", knownTypeTable[0x0064]); // ContentControl
			knownPropertyTable[0x000f] = new PropertyDeclaration("ContentTemplate", knownTypeTable[0x0064]); // ContentControl
			knownPropertyTable[0x0010] = new PropertyDeclaration("ContentTemplateSelector", knownTypeTable[0x0064]); // ContentControl
			knownPropertyTable[0x0011] = new PropertyDeclaration("HasContent", knownTypeTable[0x0064]); // ContentControl
			knownPropertyTable[0x0012] = new PropertyDeclaration("Focusable", knownTypeTable[0x0065]); // ContentElement
			knownPropertyTable[0x0013] = new PropertyDeclaration("Content", knownTypeTable[0x0066]); // ContentPresenter
			knownPropertyTable[0x0014] = new PropertyDeclaration("ContentSource", knownTypeTable[0x0066]); // ContentPresenter
			knownPropertyTable[0x0015] = new PropertyDeclaration("ContentTemplate", knownTypeTable[0x0066]); // ContentPresenter
			knownPropertyTable[0x0016] = new PropertyDeclaration("ContentTemplateSelector", knownTypeTable[0x0066]); // ContentPresenter
			knownPropertyTable[0x0017] = new PropertyDeclaration("RecognizesAccessKey", knownTypeTable[0x0066]); // ContentPresenter
			knownPropertyTable[0x0018] = new PropertyDeclaration("Background", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x0019] = new PropertyDeclaration("BorderBrush", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x001a] = new PropertyDeclaration("BorderThickness", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x001b] = new PropertyDeclaration("FontFamily", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x001c] = new PropertyDeclaration("FontSize", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x001d] = new PropertyDeclaration("FontStretch", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x001e] = new PropertyDeclaration("FontStyle", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x001f] = new PropertyDeclaration("FontWeight", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x0020] = new PropertyDeclaration("Foreground", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x0021] = new PropertyDeclaration("HorizontalContentAlignment", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x0022] = new PropertyDeclaration("IsTabStop", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x0023] = new PropertyDeclaration("Padding", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x0024] = new PropertyDeclaration("TabIndex", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x0025] = new PropertyDeclaration("Template", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x0026] = new PropertyDeclaration("VerticalContentAlignment", knownTypeTable[0x006b]); // Control
			knownPropertyTable[0x0027] = new PropertyDeclaration("Dock", knownTypeTable[0x00a3]); // DockPanel
			knownPropertyTable[0x0028] = new PropertyDeclaration("LastChildFill", knownTypeTable[0x00a3]); // DockPanel
			knownPropertyTable[0x0029] = new PropertyDeclaration("Document", knownTypeTable[0x00a7]); // DocumentViewerBase
			knownPropertyTable[0x002a] = new PropertyDeclaration("Children", knownTypeTable[0x00b7]); // DrawingGroup
			knownPropertyTable[0x002b] = new PropertyDeclaration("Document", knownTypeTable[0x00d3]); // FlowDocumentReader
			knownPropertyTable[0x002c] = new PropertyDeclaration("Document", knownTypeTable[0x00d4]); // FlowDocumentScrollViewer
			knownPropertyTable[0x002d] = new PropertyDeclaration("Style", knownTypeTable[0x00e1]); // FrameworkContentElement
			knownPropertyTable[0x002e] = new PropertyDeclaration("FlowDirection", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x002f] = new PropertyDeclaration("Height", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0030] = new PropertyDeclaration("HorizontalAlignment", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0031] = new PropertyDeclaration("Margin", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0032] = new PropertyDeclaration("MaxHeight", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0033] = new PropertyDeclaration("MaxWidth", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0034] = new PropertyDeclaration("MinHeight", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0035] = new PropertyDeclaration("MinWidth", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0036] = new PropertyDeclaration("Name", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0037] = new PropertyDeclaration("Style", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0038] = new PropertyDeclaration("VerticalAlignment", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x0039] = new PropertyDeclaration("Width", knownTypeTable[0x00e2]); // FrameworkElement
			knownPropertyTable[0x003a] = new PropertyDeclaration("Children", knownTypeTable[0x00ec]); // GeneralTransformGroup
			knownPropertyTable[0x003b] = new PropertyDeclaration("Children", knownTypeTable[0x00f2]); // GeometryGroup
			knownPropertyTable[0x003c] = new PropertyDeclaration("GradientStops", knownTypeTable[0x00fb]); // GradientBrush
			knownPropertyTable[0x003d] = new PropertyDeclaration("Column", knownTypeTable[0x00fe]); // Grid
			knownPropertyTable[0x003e] = new PropertyDeclaration("ColumnSpan", knownTypeTable[0x00fe]); // Grid
			knownPropertyTable[0x003f] = new PropertyDeclaration("Row", knownTypeTable[0x00fe]); // Grid
			knownPropertyTable[0x0040] = new PropertyDeclaration("RowSpan", knownTypeTable[0x00fe]); // Grid
			knownPropertyTable[0x0041] = new PropertyDeclaration("Header", knownTypeTable[0x0103]); // GridViewColumn
			knownPropertyTable[0x0042] = new PropertyDeclaration("HasHeader", knownTypeTable[0x010d]); // HeaderedContentControl
			knownPropertyTable[0x0043] = new PropertyDeclaration("Header", knownTypeTable[0x010d]); // HeaderedContentControl
			knownPropertyTable[0x0044] = new PropertyDeclaration("HeaderTemplate", knownTypeTable[0x010d]); // HeaderedContentControl
			knownPropertyTable[0x0045] = new PropertyDeclaration("HeaderTemplateSelector", knownTypeTable[0x010d]); // HeaderedContentControl
			knownPropertyTable[0x0046] = new PropertyDeclaration("HasHeader", knownTypeTable[0x010e]); // HeaderedItemsControl
			knownPropertyTable[0x0047] = new PropertyDeclaration("Header", knownTypeTable[0x010e]); // HeaderedItemsControl
			knownPropertyTable[0x0048] = new PropertyDeclaration("HeaderTemplate", knownTypeTable[0x010e]); // HeaderedItemsControl
			knownPropertyTable[0x0049] = new PropertyDeclaration("HeaderTemplateSelector", knownTypeTable[0x010e]); // HeaderedItemsControl
			knownPropertyTable[0x004a] = new PropertyDeclaration("NavigateUri", knownTypeTable[0x0111]); // Hyperlink
			knownPropertyTable[0x004b] = new PropertyDeclaration("Source", knownTypeTable[0x0119]); // Image
			knownPropertyTable[0x004c] = new PropertyDeclaration("Stretch", knownTypeTable[0x0119]); // Image
			knownPropertyTable[0x004d] = new PropertyDeclaration("ItemContainerStyle", knownTypeTable[0x0149]); // ItemsControl
			knownPropertyTable[0x004e] = new PropertyDeclaration("ItemContainerStyleSelector", knownTypeTable[0x0149]); // ItemsControl
			knownPropertyTable[0x004f] = new PropertyDeclaration("ItemTemplate", knownTypeTable[0x0149]); // ItemsControl
			knownPropertyTable[0x0050] = new PropertyDeclaration("ItemTemplateSelector", knownTypeTable[0x0149]); // ItemsControl
			knownPropertyTable[0x0051] = new PropertyDeclaration("ItemsPanel", knownTypeTable[0x0149]); // ItemsControl
			knownPropertyTable[0x0052] = new PropertyDeclaration("ItemsSource", knownTypeTable[0x0149]); // ItemsControl
			knownPropertyTable[0x0053] = new PropertyDeclaration("Children", knownTypeTable[0x0180]); // MaterialGroup
			knownPropertyTable[0x0054] = new PropertyDeclaration("Children", knownTypeTable[0x0198]); // Model3DGroup
			knownPropertyTable[0x0055] = new PropertyDeclaration("Content", knownTypeTable[0x01b2]); // Page
			knownPropertyTable[0x0056] = new PropertyDeclaration("Background", knownTypeTable[0x01b5]); // Panel
			knownPropertyTable[0x0057] = new PropertyDeclaration("Data", knownTypeTable[0x01ba]); // Path
			knownPropertyTable[0x0058] = new PropertyDeclaration("Segments", knownTypeTable[0x01bb]); // PathFigure
			knownPropertyTable[0x0059] = new PropertyDeclaration("Figures", knownTypeTable[0x01be]); // PathGeometry
			knownPropertyTable[0x005a] = new PropertyDeclaration("Child", knownTypeTable[0x01e5]); // Popup
			knownPropertyTable[0x005b] = new PropertyDeclaration("IsOpen", knownTypeTable[0x01e5]); // Popup
			knownPropertyTable[0x005c] = new PropertyDeclaration("Placement", knownTypeTable[0x01e5]); // Popup
			knownPropertyTable[0x005d] = new PropertyDeclaration("PopupAnimation", knownTypeTable[0x01e5]); // Popup
			knownPropertyTable[0x005e] = new PropertyDeclaration("Height", knownTypeTable[0x021d]); // RowDefinition
			knownPropertyTable[0x005f] = new PropertyDeclaration("MaxHeight", knownTypeTable[0x021d]); // RowDefinition
			knownPropertyTable[0x0060] = new PropertyDeclaration("MinHeight", knownTypeTable[0x021d]); // RowDefinition
			knownPropertyTable[0x0061] = new PropertyDeclaration("CanContentScroll", knownTypeTable[0x0226]); // ScrollViewer
			knownPropertyTable[0x0062] = new PropertyDeclaration("HorizontalScrollBarVisibility", knownTypeTable[0x0226]); // ScrollViewer
			knownPropertyTable[0x0063] = new PropertyDeclaration("VerticalScrollBarVisibility", knownTypeTable[0x0226]); // ScrollViewer
			knownPropertyTable[0x0064] = new PropertyDeclaration("Fill", knownTypeTable[0x022e]); // Shape
			knownPropertyTable[0x0065] = new PropertyDeclaration("Stroke", knownTypeTable[0x022e]); // Shape
			knownPropertyTable[0x0066] = new PropertyDeclaration("StrokeThickness", knownTypeTable[0x022e]); // Shape
			knownPropertyTable[0x0067] = new PropertyDeclaration("Background", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x0068] = new PropertyDeclaration("FontFamily", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x0069] = new PropertyDeclaration("FontSize", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x006a] = new PropertyDeclaration("FontStretch", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x006b] = new PropertyDeclaration("FontStyle", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x006c] = new PropertyDeclaration("FontWeight", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x006d] = new PropertyDeclaration("Foreground", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x006e] = new PropertyDeclaration("Text", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x006f] = new PropertyDeclaration("TextDecorations", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x0070] = new PropertyDeclaration("TextTrimming", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x0071] = new PropertyDeclaration("TextWrapping", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x0072] = new PropertyDeclaration("Text", knownTypeTable[0x027f]); // TextBox
			knownPropertyTable[0x0073] = new PropertyDeclaration("Background", knownTypeTable[0x0288]); // TextElement
			knownPropertyTable[0x0074] = new PropertyDeclaration("FontFamily", knownTypeTable[0x0288]); // TextElement
			knownPropertyTable[0x0075] = new PropertyDeclaration("FontSize", knownTypeTable[0x0288]); // TextElement
			knownPropertyTable[0x0076] = new PropertyDeclaration("FontStretch", knownTypeTable[0x0288]); // TextElement
			knownPropertyTable[0x0077] = new PropertyDeclaration("FontStyle", knownTypeTable[0x0288]); // TextElement
			knownPropertyTable[0x0078] = new PropertyDeclaration("FontWeight", knownTypeTable[0x0288]); // TextElement
			knownPropertyTable[0x0079] = new PropertyDeclaration("Foreground", knownTypeTable[0x0288]); // TextElement
			knownPropertyTable[0x007a] = new PropertyDeclaration("Children", knownTypeTable[0x029b]); // TimelineGroup
			knownPropertyTable[0x007b] = new PropertyDeclaration("IsDirectionReversed", knownTypeTable[0x02a3]); // Track
			knownPropertyTable[0x007c] = new PropertyDeclaration("Maximum", knownTypeTable[0x02a3]); // Track
			knownPropertyTable[0x007d] = new PropertyDeclaration("Minimum", knownTypeTable[0x02a3]); // Track
			knownPropertyTable[0x007e] = new PropertyDeclaration("Orientation", knownTypeTable[0x02a3]); // Track
			knownPropertyTable[0x007f] = new PropertyDeclaration("Value", knownTypeTable[0x02a3]); // Track
			knownPropertyTable[0x0080] = new PropertyDeclaration("ViewportSize", knownTypeTable[0x02a3]); // Track
			knownPropertyTable[0x0081] = new PropertyDeclaration("Children", knownTypeTable[0x02a7]); // Transform3DGroup
			knownPropertyTable[0x0082] = new PropertyDeclaration("Children", knownTypeTable[0x02aa]); // TransformGroup
			knownPropertyTable[0x0083] = new PropertyDeclaration("ClipToBounds", knownTypeTable[0x02b6]); // UIElement
			knownPropertyTable[0x0084] = new PropertyDeclaration("Focusable", knownTypeTable[0x02b6]); // UIElement
			knownPropertyTable[0x0085] = new PropertyDeclaration("IsEnabled", knownTypeTable[0x02b6]); // UIElement
			knownPropertyTable[0x0086] = new PropertyDeclaration("RenderTransform", knownTypeTable[0x02b6]); // UIElement
			knownPropertyTable[0x0087] = new PropertyDeclaration("Visibility", knownTypeTable[0x02b6]); // UIElement
			knownPropertyTable[0x0088] = new PropertyDeclaration("Children", knownTypeTable[0x02d9]); // Viewport3D
			knownPropertyTable[0x008a] = new PropertyDeclaration("Child", knownTypeTable[0x0002]); // AdornedElementPlaceholder
			knownPropertyTable[0x008b] = new PropertyDeclaration("Child", knownTypeTable[0x0004]); // AdornerDecorator
			knownPropertyTable[0x008c] = new PropertyDeclaration("Blocks", knownTypeTable[0x0008]); // AnchoredBlock
			knownPropertyTable[0x008d] = new PropertyDeclaration("Items", knownTypeTable[0x000e]); // ArrayExtension
			knownPropertyTable[0x008e] = new PropertyDeclaration("Child", knownTypeTable[0x0025]); // BlockUIContainer
			knownPropertyTable[0x008f] = new PropertyDeclaration("Inlines", knownTypeTable[0x0029]); // Bold
			knownPropertyTable[0x0090] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x002d]); // BooleanAnimationUsingKeyFrames
			knownPropertyTable[0x0091] = new PropertyDeclaration("Child", knownTypeTable[0x0032]); // Border
			knownPropertyTable[0x0092] = new PropertyDeclaration("Child", knownTypeTable[0x0036]); // BulletDecorator
			knownPropertyTable[0x0093] = new PropertyDeclaration("Content", knownTypeTable[0x0037]); // Button
			knownPropertyTable[0x0094] = new PropertyDeclaration("Content", knownTypeTable[0x0038]); // ButtonBase
			knownPropertyTable[0x0095] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x003c]); // ByteAnimationUsingKeyFrames
			knownPropertyTable[0x0096] = new PropertyDeclaration("Children", knownTypeTable[0x0042]); // Canvas
			knownPropertyTable[0x0097] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0045]); // CharAnimationUsingKeyFrames
			knownPropertyTable[0x0098] = new PropertyDeclaration("Content", knownTypeTable[0x004a]); // CheckBox
			knownPropertyTable[0x0099] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0054]); // ColorAnimationUsingKeyFrames
			knownPropertyTable[0x009a] = new PropertyDeclaration("Items", knownTypeTable[0x005c]); // ComboBox
			knownPropertyTable[0x009b] = new PropertyDeclaration("Content", knownTypeTable[0x005d]); // ComboBoxItem
			knownPropertyTable[0x009c] = new PropertyDeclaration("Items", knownTypeTable[0x0069]); // ContextMenu
			knownPropertyTable[0x009d] = new PropertyDeclaration("VisualTree", knownTypeTable[0x006c]); // ControlTemplate
			knownPropertyTable[0x009e] = new PropertyDeclaration("VisualTree", knownTypeTable[0x0078]); // DataTemplate
			knownPropertyTable[0x009f] = new PropertyDeclaration("Setters", knownTypeTable[0x007a]); // DataTrigger
			knownPropertyTable[0x00a0] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0081]); // DecimalAnimationUsingKeyFrames
			knownPropertyTable[0x00a1] = new PropertyDeclaration("Child", knownTypeTable[0x0085]); // Decorator
			knownPropertyTable[0x00a2] = new PropertyDeclaration("Children", knownTypeTable[0x00a3]); // DockPanel
			knownPropertyTable[0x00a3] = new PropertyDeclaration("Document", knownTypeTable[0x00a6]); // DocumentViewer
			knownPropertyTable[0x00a4] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x00ab]); // DoubleAnimationUsingKeyFrames
			knownPropertyTable[0x00a5] = new PropertyDeclaration("Actions", knownTypeTable[0x00c6]); // EventTrigger
			knownPropertyTable[0x00a6] = new PropertyDeclaration("Content", knownTypeTable[0x00c7]); // Expander
			knownPropertyTable[0x00a7] = new PropertyDeclaration("Blocks", knownTypeTable[0x00ca]); // Figure
			knownPropertyTable[0x00a8] = new PropertyDeclaration("Pages", knownTypeTable[0x00cd]); // FixedDocument
			knownPropertyTable[0x00a9] = new PropertyDeclaration("References", knownTypeTable[0x00ce]); // FixedDocumentSequence
			knownPropertyTable[0x00aa] = new PropertyDeclaration("Children", knownTypeTable[0x00cf]); // FixedPage
			knownPropertyTable[0x00ab] = new PropertyDeclaration("Blocks", knownTypeTable[0x00d0]); // Floater
			knownPropertyTable[0x00ac] = new PropertyDeclaration("Blocks", knownTypeTable[0x00d1]); // FlowDocument
			knownPropertyTable[0x00ad] = new PropertyDeclaration("Document", knownTypeTable[0x00d2]); // FlowDocumentPageViewer
			knownPropertyTable[0x00ae] = new PropertyDeclaration("VisualTree", knownTypeTable[0x00e7]); // FrameworkTemplate
			knownPropertyTable[0x00af] = new PropertyDeclaration("Children", knownTypeTable[0x00fe]); // Grid
			knownPropertyTable[0x00b0] = new PropertyDeclaration("Columns", knownTypeTable[0x0102]); // GridView
			knownPropertyTable[0x00b1] = new PropertyDeclaration("Content", knownTypeTable[0x0104]); // GridViewColumnHeader
			knownPropertyTable[0x00b2] = new PropertyDeclaration("Content", knownTypeTable[0x0108]); // GroupBox
			knownPropertyTable[0x00b3] = new PropertyDeclaration("Content", knownTypeTable[0x0109]); // GroupItem
			knownPropertyTable[0x00b4] = new PropertyDeclaration("Content", knownTypeTable[0x010d]); // HeaderedContentControl
			knownPropertyTable[0x00b5] = new PropertyDeclaration("Items", knownTypeTable[0x010e]); // HeaderedItemsControl
			knownPropertyTable[0x00b6] = new PropertyDeclaration("VisualTree", knownTypeTable[0x010f]); // HierarchicalDataTemplate
			knownPropertyTable[0x00b7] = new PropertyDeclaration("Inlines", knownTypeTable[0x0111]); // Hyperlink
			knownPropertyTable[0x00b8] = new PropertyDeclaration("Children", knownTypeTable[0x0120]); // InkCanvas
			knownPropertyTable[0x00b9] = new PropertyDeclaration("Child", knownTypeTable[0x0121]); // InkPresenter
			knownPropertyTable[0x00ba] = new PropertyDeclaration("Child", knownTypeTable[0x0124]); // InlineUIContainer
			knownPropertyTable[0x00bb] = new PropertyDeclaration("NameValue", knownTypeTable[0x012c]); // InputScopeName
			knownPropertyTable[0x00bc] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0131]); // Int16AnimationUsingKeyFrames
			knownPropertyTable[0x00bd] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0138]); // Int32AnimationUsingKeyFrames
			knownPropertyTable[0x00be] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0143]); // Int64AnimationUsingKeyFrames
			knownPropertyTable[0x00bf] = new PropertyDeclaration("Inlines", knownTypeTable[0x0147]); // Italic
			knownPropertyTable[0x00c0] = new PropertyDeclaration("Items", knownTypeTable[0x0149]); // ItemsControl
			knownPropertyTable[0x00c1] = new PropertyDeclaration("VisualTree", knownTypeTable[0x014a]); // ItemsPanelTemplate
			knownPropertyTable[0x00c2] = new PropertyDeclaration("Content", knownTypeTable[0x015a]); // Label
			knownPropertyTable[0x00c3] = new PropertyDeclaration("GradientStops", knownTypeTable[0x0166]); // LinearGradientBrush
			knownPropertyTable[0x00c4] = new PropertyDeclaration("ListItems", knownTypeTable[0x0174]); // List
			knownPropertyTable[0x00c5] = new PropertyDeclaration("Items", knownTypeTable[0x0175]); // ListBox
			knownPropertyTable[0x00c6] = new PropertyDeclaration("Content", knownTypeTable[0x0176]); // ListBoxItem
			knownPropertyTable[0x00c7] = new PropertyDeclaration("Blocks", knownTypeTable[0x0178]); // ListItem
			knownPropertyTable[0x00c8] = new PropertyDeclaration("Items", knownTypeTable[0x0179]); // ListView
			knownPropertyTable[0x00c9] = new PropertyDeclaration("Content", knownTypeTable[0x017a]); // ListViewItem
			knownPropertyTable[0x00ca] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0185]); // MatrixAnimationUsingKeyFrames
			knownPropertyTable[0x00cb] = new PropertyDeclaration("Items", knownTypeTable[0x0191]); // Menu
			knownPropertyTable[0x00cc] = new PropertyDeclaration("Items", knownTypeTable[0x0192]); // MenuBase
			knownPropertyTable[0x00cd] = new PropertyDeclaration("Items", knownTypeTable[0x0193]); // MenuItem
			knownPropertyTable[0x00ce] = new PropertyDeclaration("Children", knownTypeTable[0x0199]); // ModelVisual3D
			knownPropertyTable[0x00cf] = new PropertyDeclaration("Bindings", knownTypeTable[0x01a0]); // MultiBinding
			knownPropertyTable[0x00d0] = new PropertyDeclaration("Setters", knownTypeTable[0x01a2]); // MultiDataTrigger
			knownPropertyTable[0x00d1] = new PropertyDeclaration("Setters", knownTypeTable[0x01a3]); // MultiTrigger
			knownPropertyTable[0x00d2] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x01ac]); // ObjectAnimationUsingKeyFrames
			knownPropertyTable[0x00d3] = new PropertyDeclaration("Child", knownTypeTable[0x01b3]); // PageContent
			knownPropertyTable[0x00d4] = new PropertyDeclaration("Content", knownTypeTable[0x01b4]); // PageFunctionBase
			knownPropertyTable[0x00d5] = new PropertyDeclaration("Children", knownTypeTable[0x01b5]); // Panel
			knownPropertyTable[0x00d6] = new PropertyDeclaration("Inlines", knownTypeTable[0x01b6]); // Paragraph
			knownPropertyTable[0x00d7] = new PropertyDeclaration("Children", knownTypeTable[0x01b7]); // ParallelTimeline
			knownPropertyTable[0x00d8] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x01cc]); // Point3DAnimationUsingKeyFrames
			knownPropertyTable[0x00d9] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x01d6]); // PointAnimationUsingKeyFrames
			knownPropertyTable[0x00da] = new PropertyDeclaration("Bindings", knownTypeTable[0x01e7]); // PriorityBinding
			knownPropertyTable[0x00db] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x01f1]); // QuaternionAnimationUsingKeyFrames
			knownPropertyTable[0x00dc] = new PropertyDeclaration("GradientStops", knownTypeTable[0x01f6]); // RadialGradientBrush
			knownPropertyTable[0x00dd] = new PropertyDeclaration("Content", knownTypeTable[0x01f7]); // RadioButton
			knownPropertyTable[0x00de] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x01fe]); // RectAnimationUsingKeyFrames
			knownPropertyTable[0x00df] = new PropertyDeclaration("Content", knownTypeTable[0x020a]); // RepeatButton
			knownPropertyTable[0x00e0] = new PropertyDeclaration("Document", knownTypeTable[0x020f]); // RichTextBox
			knownPropertyTable[0x00e1] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0215]); // Rotation3DAnimationUsingKeyFrames
			knownPropertyTable[0x00e2] = new PropertyDeclaration("Text", knownTypeTable[0x021e]); // Run
			knownPropertyTable[0x00e3] = new PropertyDeclaration("Content", knownTypeTable[0x0226]); // ScrollViewer
			knownPropertyTable[0x00e4] = new PropertyDeclaration("Blocks", knownTypeTable[0x0227]); // Section
			knownPropertyTable[0x00e5] = new PropertyDeclaration("Items", knownTypeTable[0x0229]); // Selector
			knownPropertyTable[0x00e6] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0232]); // SingleAnimationUsingKeyFrames
			knownPropertyTable[0x00e7] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x023b]); // SizeAnimationUsingKeyFrames
			knownPropertyTable[0x00e8] = new PropertyDeclaration("Inlines", knownTypeTable[0x0244]); // Span
			knownPropertyTable[0x00e9] = new PropertyDeclaration("Children", knownTypeTable[0x0259]); // StackPanel
			knownPropertyTable[0x00ea] = new PropertyDeclaration("Items", knownTypeTable[0x025c]); // StatusBar
			knownPropertyTable[0x00eb] = new PropertyDeclaration("Content", knownTypeTable[0x025d]); // StatusBarItem
			knownPropertyTable[0x00ec] = new PropertyDeclaration("Children", knownTypeTable[0x0260]); // Storyboard
			knownPropertyTable[0x00ed] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x0266]); // StringAnimationUsingKeyFrames
			knownPropertyTable[0x00ee] = new PropertyDeclaration("Setters", knownTypeTable[0x026c]); // Style
			knownPropertyTable[0x00ef] = new PropertyDeclaration("Items", knownTypeTable[0x026f]); // TabControl
			knownPropertyTable[0x00f0] = new PropertyDeclaration("Content", knownTypeTable[0x0270]); // TabItem
			knownPropertyTable[0x00f1] = new PropertyDeclaration("Children", knownTypeTable[0x0271]); // TabPanel
			knownPropertyTable[0x00f2] = new PropertyDeclaration("RowGroups", knownTypeTable[0x0272]); // Table
			knownPropertyTable[0x00f3] = new PropertyDeclaration("Blocks", knownTypeTable[0x0273]); // TableCell
			knownPropertyTable[0x00f4] = new PropertyDeclaration("Cells", knownTypeTable[0x0275]); // TableRow
			knownPropertyTable[0x00f5] = new PropertyDeclaration("Rows", knownTypeTable[0x0276]); // TableRowGroup
			knownPropertyTable[0x00f6] = new PropertyDeclaration("Inlines", knownTypeTable[0x027e]); // TextBlock
			knownPropertyTable[0x00f7] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x028e]); // ThicknessAnimationUsingKeyFrames
			knownPropertyTable[0x00f8] = new PropertyDeclaration("Content", knownTypeTable[0x029c]); // ToggleButton
			knownPropertyTable[0x00f9] = new PropertyDeclaration("Items", knownTypeTable[0x029d]); // ToolBar
			knownPropertyTable[0x00fa] = new PropertyDeclaration("Children", knownTypeTable[0x029e]); // ToolBarOverflowPanel
			knownPropertyTable[0x00fb] = new PropertyDeclaration("Children", knownTypeTable[0x029f]); // ToolBarPanel
			knownPropertyTable[0x00fc] = new PropertyDeclaration("ToolBars", knownTypeTable[0x02a0]); // ToolBarTray
			knownPropertyTable[0x00fd] = new PropertyDeclaration("Content", knownTypeTable[0x02a1]); // ToolTip
			knownPropertyTable[0x00fe] = new PropertyDeclaration("Items", knownTypeTable[0x02ae]); // TreeView
			knownPropertyTable[0x00ff] = new PropertyDeclaration("Items", knownTypeTable[0x02af]); // TreeViewItem
			knownPropertyTable[0x0100] = new PropertyDeclaration("Setters", knownTypeTable[0x02b0]); // Trigger
			knownPropertyTable[0x0101] = new PropertyDeclaration("Inlines", knownTypeTable[0x02be]); // Underline
			knownPropertyTable[0x0102] = new PropertyDeclaration("Children", knownTypeTable[0x02bf]); // UniformGrid
			knownPropertyTable[0x0103] = new PropertyDeclaration("Content", knownTypeTable[0x02c2]); // UserControl
			knownPropertyTable[0x0104] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x02c8]); // Vector3DAnimationUsingKeyFrames
			knownPropertyTable[0x0105] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x02d0]); // VectorAnimationUsingKeyFrames
			knownPropertyTable[0x0106] = new PropertyDeclaration("Child", knownTypeTable[0x02d8]); // Viewbox
			knownPropertyTable[0x0107] = new PropertyDeclaration("Children", knownTypeTable[0x02da]); // Viewport3DVisual
			knownPropertyTable[0x0108] = new PropertyDeclaration("Children", knownTypeTable[0x02db]); // VirtualizingPanel
			knownPropertyTable[0x0109] = new PropertyDeclaration("Children", knownTypeTable[0x02dc]); // VirtualizingStackPanel
			knownPropertyTable[0x010a] = new PropertyDeclaration("Content", knownTypeTable[0x02e3]); // Window
			knownPropertyTable[0x010b] = new PropertyDeclaration("Children", knownTypeTable[0x02e6]); // WrapPanel
			knownPropertyTable[0x010c] = new PropertyDeclaration("XmlSerializer", knownTypeTable[0x02f2]); // XmlDataProvider

			knownResourceTable.Add(0x1, new ResourceName("ActiveBorderBrush"));
			knownResourceTable.Add(0x1f, new ResourceName("ActiveBorderColor"));
			knownResourceTable.Add(0x2, new ResourceName("ActiveCaptionBrush"));
			knownResourceTable.Add(0x20, new ResourceName("ActiveCaptionColor"));
			knownResourceTable.Add(0x3, new ResourceName("ActiveCaptionTextBrush"));
			knownResourceTable.Add(0x21, new ResourceName("ActiveCaptionTextColor"));
			knownResourceTable.Add(0x4, new ResourceName("AppWorkspaceBrush"));
			knownResourceTable.Add(0x22, new ResourceName("AppWorkspaceColor"));
			knownResourceTable.Add(0xc6, new ResourceName("Border"));
			knownResourceTable.Add(0xca, new ResourceName("BorderWidth"));
			knownResourceTable.Add(0x40, new ResourceName("CaptionFontFamily"));
			knownResourceTable.Add(0x3f, new ResourceName("CaptionFontSize"));
			knownResourceTable.Add(0x41, new ResourceName("CaptionFontStyle"));
			knownResourceTable.Add(0x43, new ResourceName("CaptionFontTextDecorations"));
			knownResourceTable.Add(0x42, new ResourceName("CaptionFontWeight"));
			knownResourceTable.Add(0xce, new ResourceName("CaptionHeight"));
			knownResourceTable.Add(0xcd, new ResourceName("CaptionWidth"));
			knownResourceTable.Add(0xc7, new ResourceName("CaretWidth"));
			knownResourceTable.Add(0xba, new ResourceName("ClientAreaAnimation"));
			knownResourceTable.Add(0xb9, new ResourceName("ComboBoxAnimation"));
			knownResourceTable.Add(0xd2, new ResourceName("ComboBoxPopupAnimation"));
			knownResourceTable.Add(0x5, new ResourceName("ControlBrush"));
			knownResourceTable.Add(0x23, new ResourceName("ControlColor"));
			knownResourceTable.Add(0x6, new ResourceName("ControlDarkBrush"));
			knownResourceTable.Add(0x24, new ResourceName("ControlDarkColor"));
			knownResourceTable.Add(0x7, new ResourceName("ControlDarkDarkBrush"));
			knownResourceTable.Add(0x25, new ResourceName("ControlDarkDarkColor"));
			knownResourceTable.Add(0x8, new ResourceName("ControlLightBrush"));
			knownResourceTable.Add(0x26, new ResourceName("ControlLightColor"));
			knownResourceTable.Add(0x9, new ResourceName("ControlLightLightBrush"));
			knownResourceTable.Add(0x27, new ResourceName("ControlLightLightColor"));
			knownResourceTable.Add(0xa, new ResourceName("ControlTextBrush"));
			knownResourceTable.Add(0x28, new ResourceName("ControlTextColor"));
			knownResourceTable.Add(0x62, new ResourceName("CursorHeight"));
			knownResourceTable.Add(0xbb, new ResourceName("CursorShadow"));
			knownResourceTable.Add(0x61, new ResourceName("CursorWidth"));
			knownResourceTable.Add(0xb, new ResourceName("DesktopBrush"));
			knownResourceTable.Add(0x29, new ResourceName("DesktopColor"));
			knownResourceTable.Add(0xc9, new ResourceName("DragFullWindows"));
			knownResourceTable.Add(0xa7, new ResourceName("DropShadow"));
			knownResourceTable.Add(0x65, new ResourceName("FixedFrameHorizontalBorderHeight"));
			knownResourceTable.Add(0x66, new ResourceName("FixedFrameVerticalBorderWidth"));
			knownResourceTable.Add(0xa8, new ResourceName("FlatMenu"));
			knownResourceTable.Add(0xa5, new ResourceName("FocusBorderHeight"));
			knownResourceTable.Add(0xa4, new ResourceName("FocusBorderWidth"));
			knownResourceTable.Add(0x67, new ResourceName("FocusHorizontalBorderHeight"));
			knownResourceTable.Add(0x68, new ResourceName("FocusVerticalBorderWidth"));
			knownResourceTable.Add(0xd7, new ResourceName("FocusVisualStyle"));
			knownResourceTable.Add(0xc8, new ResourceName("ForegroundFlashCount"));
			knownResourceTable.Add(0x6a, new ResourceName("FullPrimaryScreenHeight"));
			knownResourceTable.Add(0x69, new ResourceName("FullPrimaryScreenWidth"));
			knownResourceTable.Add(0xc, new ResourceName("GradientActiveCaptionBrush"));
			knownResourceTable.Add(0x2a, new ResourceName("GradientActiveCaptionColor"));
			knownResourceTable.Add(0xbc, new ResourceName("GradientCaptions"));
			knownResourceTable.Add(0xd, new ResourceName("GradientInactiveCaptionBrush"));
			knownResourceTable.Add(0x2b, new ResourceName("GradientInactiveCaptionColor"));
			knownResourceTable.Add(0xe, new ResourceName("GrayTextBrush"));
			knownResourceTable.Add(0x2c, new ResourceName("GrayTextColor"));
			knownResourceTable.Add(0xde, new ResourceName("GridViewItemContainerStyle"));
			knownResourceTable.Add(0xdc, new ResourceName("GridViewScrollViewerStyle"));
			knownResourceTable.Add(0xdd, new ResourceName("GridViewStyle"));
			knownResourceTable.Add(0xa6, new ResourceName("HighContrast"));
			knownResourceTable.Add(0xf, new ResourceName("HighlightBrush"));
			knownResourceTable.Add(0x2d, new ResourceName("HighlightColor"));
			knownResourceTable.Add(0x10, new ResourceName("HighlightTextBrush"));
			knownResourceTable.Add(0x2e, new ResourceName("HighlightTextColor"));
			knownResourceTable.Add(0x6b, new ResourceName("HorizontalScrollBarButtonWidth"));
			knownResourceTable.Add(0x6c, new ResourceName("HorizontalScrollBarHeight"));
			knownResourceTable.Add(0x6d, new ResourceName("HorizontalScrollBarThumbWidth"));
			knownResourceTable.Add(0x11, new ResourceName("HotTrackBrush"));
			knownResourceTable.Add(0x2f, new ResourceName("HotTrackColor"));
			knownResourceTable.Add(0xbd, new ResourceName("HotTracking"));
			knownResourceTable.Add(0x59, new ResourceName("IconFontFamily"));
			knownResourceTable.Add(0x58, new ResourceName("IconFontSize"));
			knownResourceTable.Add(0x5a, new ResourceName("IconFontStyle"));
			knownResourceTable.Add(0x5c, new ResourceName("IconFontTextDecorations"));
			knownResourceTable.Add(0x5b, new ResourceName("IconFontWeight"));
			knownResourceTable.Add(0x71, new ResourceName("IconGridHeight"));
			knownResourceTable.Add(0x70, new ResourceName("IconGridWidth"));
			knownResourceTable.Add(0x6f, new ResourceName("IconHeight"));
			knownResourceTable.Add(0xaa, new ResourceName("IconHorizontalSpacing"));
			knownResourceTable.Add(0xac, new ResourceName("IconTitleWrap"));
			knownResourceTable.Add(0xab, new ResourceName("IconVerticalSpacing"));
			knownResourceTable.Add(0x6e, new ResourceName("IconWidth"));
			knownResourceTable.Add(0x12, new ResourceName("InactiveBorderBrush"));
			knownResourceTable.Add(0x30, new ResourceName("InactiveBorderColor"));
			knownResourceTable.Add(0x13, new ResourceName("InactiveCaptionBrush"));
			knownResourceTable.Add(0x31, new ResourceName("InactiveCaptionColor"));
			knownResourceTable.Add(0x14, new ResourceName("InactiveCaptionTextBrush"));
			knownResourceTable.Add(0x32, new ResourceName("InactiveCaptionTextColor"));
			knownResourceTable.Add(0x15, new ResourceName("InfoBrush"));
			knownResourceTable.Add(0x33, new ResourceName("InfoColor"));
			knownResourceTable.Add(0x16, new ResourceName("InfoTextBrush"));
			knownResourceTable.Add(0x34, new ResourceName("InfoTextColor"));
			knownResourceTable.Add(0x3d, new ResourceName("InternalSystemColorsEnd"));
			knownResourceTable.Add(0x0, new ResourceName("InternalSystemColorsStart"));
			knownResourceTable.Add(0x5d, new ResourceName("InternalSystemFontsEnd"));
			knownResourceTable.Add(0x3e, new ResourceName("InternalSystemFontsStart"));
			knownResourceTable.Add(0xda, new ResourceName("InternalSystemParametersEnd"));
			knownResourceTable.Add(0x5e, new ResourceName("InternalSystemParametersStart"));
			knownResourceTable.Add(0xe8, new ResourceName("InternalSystemThemeStylesEnd"));
			knownResourceTable.Add(0xd6, new ResourceName("InternalSystemThemeStylesStart"));
			knownResourceTable.Add(0x95, new ResourceName("IsImmEnabled"));
			knownResourceTable.Add(0x96, new ResourceName("IsMediaCenter"));
			knownResourceTable.Add(0x97, new ResourceName("IsMenuDropRightAligned"));
			knownResourceTable.Add(0x98, new ResourceName("IsMiddleEastEnabled"));
			knownResourceTable.Add(0x99, new ResourceName("IsMousePresent"));
			knownResourceTable.Add(0x9a, new ResourceName("IsMouseWheelPresent"));
			knownResourceTable.Add(0x9b, new ResourceName("IsPenWindows"));
			knownResourceTable.Add(0x9c, new ResourceName("IsRemotelyControlled"));
			knownResourceTable.Add(0x9d, new ResourceName("IsRemoteSession"));
			knownResourceTable.Add(0x9f, new ResourceName("IsSlowMachine"));
			knownResourceTable.Add(0xa1, new ResourceName("IsTabletPC"));
			knownResourceTable.Add(0x91, new ResourceName("KanjiWindowHeight"));
			knownResourceTable.Add(0xad, new ResourceName("KeyboardCues"));
			knownResourceTable.Add(0xae, new ResourceName("KeyboardDelay"));
			knownResourceTable.Add(0xaf, new ResourceName("KeyboardPreference"));
			knownResourceTable.Add(0xb0, new ResourceName("KeyboardSpeed"));
			knownResourceTable.Add(0xbe, new ResourceName("ListBoxSmoothScrolling"));
			knownResourceTable.Add(0x73, new ResourceName("MaximizedPrimaryScreenHeight"));
			knownResourceTable.Add(0x72, new ResourceName("MaximizedPrimaryScreenWidth"));
			knownResourceTable.Add(0x75, new ResourceName("MaximumWindowTrackHeight"));
			knownResourceTable.Add(0x74, new ResourceName("MaximumWindowTrackWidth"));
			knownResourceTable.Add(0xbf, new ResourceName("MenuAnimation"));
			knownResourceTable.Add(0x18, new ResourceName("MenuBarBrush"));
			knownResourceTable.Add(0x36, new ResourceName("MenuBarColor"));
			knownResourceTable.Add(0x92, new ResourceName("MenuBarHeight"));
			knownResourceTable.Add(0x17, new ResourceName("MenuBrush"));
			knownResourceTable.Add(0x79, new ResourceName("MenuButtonHeight"));
			knownResourceTable.Add(0x78, new ResourceName("MenuButtonWidth"));
			knownResourceTable.Add(0x77, new ResourceName("MenuCheckmarkHeight"));
			knownResourceTable.Add(0x76, new ResourceName("MenuCheckmarkWidth"));
			knownResourceTable.Add(0x35, new ResourceName("MenuColor"));
			knownResourceTable.Add(0xb6, new ResourceName("MenuDropAlignment"));
			knownResourceTable.Add(0xb7, new ResourceName("MenuFade"));
			knownResourceTable.Add(0x4a, new ResourceName("MenuFontFamily"));
			knownResourceTable.Add(0x49, new ResourceName("MenuFontSize"));
			knownResourceTable.Add(0x4b, new ResourceName("MenuFontStyle"));
			knownResourceTable.Add(0x4d, new ResourceName("MenuFontTextDecorations"));
			knownResourceTable.Add(0x4c, new ResourceName("MenuFontWeight"));
			knownResourceTable.Add(0xd1, new ResourceName("MenuHeight"));
			knownResourceTable.Add(0x19, new ResourceName("MenuHighlightBrush"));
			knownResourceTable.Add(0x37, new ResourceName("MenuHighlightColor"));
			knownResourceTable.Add(0xdb, new ResourceName("MenuItemSeparatorStyle"));
			knownResourceTable.Add(0xd3, new ResourceName("MenuPopupAnimation"));
			knownResourceTable.Add(0xb8, new ResourceName("MenuShowDelay"));
			knownResourceTable.Add(0x1a, new ResourceName("MenuTextBrush"));
			knownResourceTable.Add(0x38, new ResourceName("MenuTextColor"));
			knownResourceTable.Add(0xd0, new ResourceName("MenuWidth"));
			knownResourceTable.Add(0x54, new ResourceName("MessageFontFamily"));
			knownResourceTable.Add(0x53, new ResourceName("MessageFontSize"));
			knownResourceTable.Add(0x55, new ResourceName("MessageFontStyle"));
			knownResourceTable.Add(0x57, new ResourceName("MessageFontTextDecorations"));
			knownResourceTable.Add(0x56, new ResourceName("MessageFontWeight"));
			knownResourceTable.Add(0xc5, new ResourceName("MinimizeAnimation"));
			knownResourceTable.Add(0x7f, new ResourceName("MinimizedGridHeight"));
			knownResourceTable.Add(0x7e, new ResourceName("MinimizedGridWidth"));
			knownResourceTable.Add(0x7d, new ResourceName("MinimizedWindowHeight"));
			knownResourceTable.Add(0x7c, new ResourceName("MinimizedWindowWidth"));
			knownResourceTable.Add(0x7b, new ResourceName("MinimumWindowHeight"));
			knownResourceTable.Add(0x81, new ResourceName("MinimumWindowTrackHeight"));
			knownResourceTable.Add(0x80, new ResourceName("MinimumWindowTrackWidth"));
			knownResourceTable.Add(0x7a, new ResourceName("MinimumWindowWidth"));
			knownResourceTable.Add(0xb4, new ResourceName("MouseHoverHeight"));
			knownResourceTable.Add(0xb3, new ResourceName("MouseHoverTime"));
			knownResourceTable.Add(0xb5, new ResourceName("MouseHoverWidth"));
			knownResourceTable.Add(0xd8, new ResourceName("NavigationChromeDownLevelStyle"));
			knownResourceTable.Add(0xd9, new ResourceName("NavigationChromeStyle"));
			knownResourceTable.Add(0xd5, new ResourceName("PowerLineStatus"));
			knownResourceTable.Add(0x83, new ResourceName("PrimaryScreenHeight"));
			knownResourceTable.Add(0x82, new ResourceName("PrimaryScreenWidth"));
			knownResourceTable.Add(0x86, new ResourceName("ResizeFrameHorizontalBorderHeight"));
			knownResourceTable.Add(0x87, new ResourceName("ResizeFrameVerticalBorderWidth"));
			knownResourceTable.Add(0x1b, new ResourceName("ScrollBarBrush"));
			knownResourceTable.Add(0x39, new ResourceName("ScrollBarColor"));
			knownResourceTable.Add(0xcc, new ResourceName("ScrollHeight"));
			knownResourceTable.Add(0xcb, new ResourceName("ScrollWidth"));
			knownResourceTable.Add(0xc0, new ResourceName("SelectionFade"));
			knownResourceTable.Add(0x9e, new ResourceName("ShowSounds"));
			knownResourceTable.Add(0x45, new ResourceName("SmallCaptionFontFamily"));
			knownResourceTable.Add(0x44, new ResourceName("SmallCaptionFontSize"));
			knownResourceTable.Add(0x46, new ResourceName("SmallCaptionFontStyle"));
			knownResourceTable.Add(0x48, new ResourceName("SmallCaptionFontTextDecorations"));
			knownResourceTable.Add(0x47, new ResourceName("SmallCaptionFontWeight"));
			knownResourceTable.Add(0x93, new ResourceName("SmallCaptionHeight"));
			knownResourceTable.Add(0xcf, new ResourceName("SmallCaptionWidth"));
			knownResourceTable.Add(0x89, new ResourceName("SmallIconHeight"));
			knownResourceTable.Add(0x88, new ResourceName("SmallIconWidth"));
			knownResourceTable.Add(0x8b, new ResourceName("SmallWindowCaptionButtonHeight"));
			knownResourceTable.Add(0x8a, new ResourceName("SmallWindowCaptionButtonWidth"));
			knownResourceTable.Add(0xb1, new ResourceName("SnapToDefaultButton"));
			knownResourceTable.Add(0xdf, new ResourceName("StatusBarSeparatorStyle"));
			knownResourceTable.Add(0x4f, new ResourceName("StatusFontFamily"));
			knownResourceTable.Add(0x4e, new ResourceName("StatusFontSize"));
			knownResourceTable.Add(0x50, new ResourceName("StatusFontStyle"));
			knownResourceTable.Add(0x52, new ResourceName("StatusFontTextDecorations"));
			knownResourceTable.Add(0x51, new ResourceName("StatusFontWeight"));
			knownResourceTable.Add(0xc1, new ResourceName("StylusHotTracking"));
			knownResourceTable.Add(0xa0, new ResourceName("SwapButtons"));
			knownResourceTable.Add(0x63, new ResourceName("ThickHorizontalBorderHeight"));
			knownResourceTable.Add(0x64, new ResourceName("ThickVerticalBorderWidth"));
			knownResourceTable.Add(0x5f, new ResourceName("ThinHorizontalBorderHeight"));
			knownResourceTable.Add(0x60, new ResourceName("ThinVerticalBorderWidth"));
			knownResourceTable.Add(0xe0, new ResourceName("ToolBarButtonStyle"));
			knownResourceTable.Add(0xe3, new ResourceName("ToolBarCheckBoxStyle"));
			knownResourceTable.Add(0xe5, new ResourceName("ToolBarComboBoxStyle"));
			knownResourceTable.Add(0xe7, new ResourceName("ToolBarMenuStyle"));
			knownResourceTable.Add(0xe4, new ResourceName("ToolBarRadioButtonStyle"));
			knownResourceTable.Add(0xe2, new ResourceName("ToolBarSeparatorStyle"));
			knownResourceTable.Add(0xe6, new ResourceName("ToolBarTextBoxStyle"));
			knownResourceTable.Add(0xe1, new ResourceName("ToolBarToggleButtonStyle"));
			knownResourceTable.Add(0xc2, new ResourceName("ToolTipAnimation"));
			knownResourceTable.Add(0xc3, new ResourceName("ToolTipFade"));
			knownResourceTable.Add(0xd4, new ResourceName("ToolTipPopupAnimation"));
			knownResourceTable.Add(0xc4, new ResourceName("UIEffects"));
			knownResourceTable.Add(0x8f, new ResourceName("VerticalScrollBarButtonHeight"));
			knownResourceTable.Add(0x94, new ResourceName("VerticalScrollBarThumbHeight"));
			knownResourceTable.Add(0x8e, new ResourceName("VerticalScrollBarWidth"));
			knownResourceTable.Add(0x8d, new ResourceName("VirtualScreenHeight"));
			knownResourceTable.Add(0xa2, new ResourceName("VirtualScreenLeft"));
			knownResourceTable.Add(0xa3, new ResourceName("VirtualScreenTop"));
			knownResourceTable.Add(0x8c, new ResourceName("VirtualScreenWidth"));
			knownResourceTable.Add(0xb2, new ResourceName("WheelScrollLines"));
			knownResourceTable.Add(0x1c, new ResourceName("WindowBrush"));
			knownResourceTable.Add(0x85, new ResourceName("WindowCaptionButtonHeight"));
			knownResourceTable.Add(0x84, new ResourceName("WindowCaptionButtonWidth"));
			knownResourceTable.Add(0x90, new ResourceName("WindowCaptionHeight"));
			knownResourceTable.Add(0x3a, new ResourceName("WindowColor"));
			knownResourceTable.Add(0x1d, new ResourceName("WindowFrameBrush"));
			knownResourceTable.Add(0x3b, new ResourceName("WindowFrameColor"));
			knownResourceTable.Add(0x1e, new ResourceName("WindowTextBrush"));
			knownResourceTable.Add(0x3c, new ResourceName("WindowTextColor"));
			knownResourceTable.Add(0xa9, new ResourceName("WorkArea"));
		}
	
		private enum BamlRecordType : byte
		{
			ClrEvent = 0x13,
			Comment = 0x17,
			AssemblyInfo = 0x1c,
			AttributeInfo = 0x1f,
			ConstructorParametersStart = 0x2a,
			ConstructorParametersEnd = 0x2b,
			ConstructorParameterType = 0x2c,
			ConnectionId = 0x2d,
			ContentProperty = 0x2e,
			DefAttribute = 0x19,
			DefAttributeKeyString = 0x26,
			DefAttributeKeyType = 0x27,
			DeferableContentStart = 0x25,
			DefTag = 0x18,
			DocumentEnd = 0x2,
			DocumentStart = 0x1,
			ElementEnd = 0x4,
			ElementStart = 0x3,
			EndAttributes = 0x1a,
			KeyElementEnd = 0x29,
			KeyElementStart = 0x28,
			LastRecordType = 0x39,
			LineNumberAndPosition = 0x35,
			LinePosition = 0x36,
			LiteralContent = 0xf,
			NamedElementStart = 0x2f,
			OptimizedStaticResource = 0x37,
			PIMapping = 0x1b,
			PresentationOptionsAttribute = 0x34,
			ProcessingInstruction = 0x16,
			Property = 0x5,
			PropertyArrayEnd = 0xa,
			PropertyArrayStart = 0x9,
			PropertyComplexEnd = 0x8,
			PropertyComplexStart = 0x7,
			PropertyCustom = 0x6,
			PropertyDictionaryEnd = 0xe,
			PropertyDictionaryStart = 0xd,
			PropertyListEnd = 0xc,
			PropertyListStart = 0xb,
			PropertyStringReference = 0x21,
			PropertyTypeReference = 0x22,
			PropertyWithConverter = 0x24,
			PropertyWithExtension = 0x23,
			PropertyWithStaticResourceId = 0x38,
			RoutedEvent = 0x12,
			StaticResourceEnd = 0x31,
			StaticResourceId = 0x32,
			StaticResourceStart = 0x30,
			StringInfo = 0x20,
			Text = 0x10,
			TextWithConverter = 0x11,
			TextWithId = 0x33,
			TypeInfo = 0x1d,
			TypeSerializerInfo = 0x1e,
			Unknown = 0x0,
			XmlAttribute = 0x15,
			XmlnsProperty = 0x14
		}
	
		private enum BamlAttributeUsage : short
		{
			Default = 0x0,
			RuntimeName = 0x3,
			XmlLang = 0x1,
			XmlSpace = 0x2
		}
	
	 	private class BamlBinaryReader : BinaryReader
		{
			public BamlBinaryReader(Stream stream) : base(stream)
			{
                if (stream.CanSeek && stream.Position != 0)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
			}
	
			public virtual double ReadCompressedDouble()
			{
				byte leadingByte = this.ReadByte();
				switch (leadingByte)
				{
					case 0x01:
						return 0;
	
					case 0x02:
						return 1;
	
					case 0x03:
						return -1;
	
					case 0x04:
						double value = this.ReadInt32();
						return (value * 1E-06);
	
					case 0x05:
						return this.ReadDouble();
				}
	
				throw new NotSupportedException();
			}
	
			public int ReadCompressedInt32()
			{
				return base.Read7BitEncodedInt();
			}
		}

		private class IndentationTextWriter : TextWriter
		{
			private bool indentationPending = false;
			private int indentation = 0;
			private string indentText = "    ";
			private TextWriter writer = null;

			public IndentationTextWriter(TextWriter writer)
			{
				this.writer = writer;
			}

			private void WriteIndentation()
			{
				if (this.indentationPending)
				{
					for (int i = 0; i < this.indentation; i++)
					{
						this.writer.Write(this.indentText);
					}

					this.indentationPending = false;
				}
			}

			public override void Write(bool value)
			{
				this.WriteIndentation();
				this.writer.Write(value);
			}

			public override void Write(char value)
			{
				this.WriteIndentation();
				this.writer.Write(value);
			}

			public override void Write(string s)
			{
				this.WriteIndentation();
				this.writer.Write(s);
			}

			public override void Write(char[] buffer)
			{
				this.WriteIndentation();
				this.writer.Write(buffer);
			}

			public override void Write(double value)
			{
				this.WriteIndentation();
				this.writer.Write(value);
			}

			public override void Write(int value)
			{
				this.WriteIndentation();
				this.writer.Write(value);
			}

			public override void Write(long value)
			{
				this.WriteIndentation();
				this.writer.Write(value);
			}

			public override void Write(object value)
			{
				this.WriteIndentation();
				this.writer.Write(value);
			}

			public override void Write(float value)
			{
				this.WriteIndentation();
				this.writer.Write(value);
			}

			public override void Write(string format, object arg0)
			{
				this.WriteIndentation();
				this.writer.Write(format, arg0);
			}

			public override void Write(string format, params object[] arg)
			{
				this.WriteIndentation();
				this.writer.Write(format, arg);
			}

			public override void Write(string format, object arg0, object arg1)
			{
				this.WriteIndentation();
				this.writer.Write(format, arg0, arg1);
			}

			public override void Write(char[] buffer, int index, int count)
			{
				this.WriteIndentation();
				this.writer.Write(buffer, index, count);
			}

			public override void WriteLine()
			{
				this.WriteIndentation();
				this.writer.WriteLine();
				this.indentationPending = true;
			}

			public override void WriteLine(bool value)
			{
				this.WriteIndentation();
				this.writer.WriteLine(value);
				this.indentationPending = true;
			}

			public override void WriteLine(char value)
			{
				this.WriteIndentation();
				this.writer.WriteLine(value);
				this.indentationPending = true;
			}

			public override void WriteLine(double value)
			{
				this.WriteIndentation();
				this.writer.WriteLine(value);
				this.indentationPending = true;
			}

			public override void WriteLine(char[] buffer)
			{
				this.WriteIndentation();
				this.writer.WriteLine(buffer);
				this.indentationPending = true;
			}

			public override void WriteLine(int value)
			{
				this.WriteIndentation();
				this.writer.WriteLine(value);
				this.indentationPending = true;
			}

			public override void WriteLine(long value)
			{
				this.WriteIndentation();
				this.writer.WriteLine(value);
				this.indentationPending = true;
			}

			public override void WriteLine(object value)
			{
				this.WriteIndentation();
				this.writer.WriteLine(value);
				this.indentationPending = true;
			}

			public override void WriteLine(float value)
			{
				this.WriteIndentation();
				this.writer.WriteLine(value);
				this.indentationPending = true;
			}

			public override void WriteLine(string s)
			{
				this.WriteIndentation();
				this.writer.WriteLine(s);
				this.indentationPending = true;
			}

			public override void WriteLine(string format, object arg0)
			{
				this.WriteIndentation();
				this.writer.WriteLine(format, arg0);
				this.indentationPending = true;
			}

			public override void WriteLine(string format, params object[] arg)
			{
				this.WriteIndentation();
				this.writer.WriteLine(format, arg);
				this.indentationPending = true;
			}

			public override void WriteLine(string format, object arg0, object arg1)
			{
				this.WriteIndentation();
				this.writer.WriteLine(format, arg0, arg1);
				this.indentationPending = true;
			}

			public override void WriteLine(char[] buffer, int index, int count)
			{
				this.WriteIndentation();
				this.writer.WriteLine(buffer, index, count);
				this.indentationPending = true;
			}

			public int Indentation
			{
				get
				{
					return this.indentation;
				}

				set
				{
					this.indentation = value;
				}
			}

			public override Encoding Encoding
			{
				get 
				{
					return this.writer.Encoding;
				}
			}
		}

		private class Element
		{
			private TypeDeclaration typeDeclaration;
			private PropertyCollection properties = new PropertyCollection();
			private IList arguments = new ArrayList();

			public TypeDeclaration TypeDeclaration
			{
				get
				{
					return this.typeDeclaration;
				}
	
				set
				{
					this.typeDeclaration = value;
				}
			}
	
			public PropertyCollection Properties
			{
				get
				{
					return this.properties;
				}
			}
	
			public IList Arguments
			{
				get
				{
					return this.arguments;
				}
			}
	
			public override string ToString()
			{
				/*
				using (StringWriter stringWriter = new StringWriter())
				{
					using (IndentationTextWriter indentationTextWriter = new IndentationTextWriter(stringWriter))
					{
						WriteElement(this, indentationTextWriter);
					}

					return stringWriter.ToString();
				}
				*/

				return "<" + this.TypeDeclaration.ToString() + ">";
			}
		}
	
		private class TypeDeclaration
		{
			private string name;
			private string namespaceName;
			private string assembly;
			private string xmlPrefix;
	
			public TypeDeclaration(string name)
			{
				this.name = name;
				this.namespaceName = string.Empty;
				this.assembly = string.Empty;
			}

			public TypeDeclaration(string name, string namespaceName, string assembly)
			{
				this.name = name;
				this.namespaceName = namespaceName;
				this.assembly = assembly;
			}

			public TypeDeclaration Copy(string xmlPrefix)
			{
				TypeDeclaration copy = new TypeDeclaration(this.name, this.namespaceName, this.assembly);
				copy.xmlPrefix = xmlPrefix;
				return copy;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public string Namespace
			{
				get
				{
					return this.namespaceName;
				}
			}

			public string Assembly
			{
				get
				{
					return this.assembly;
				}
			}

            bool _typeInited = false;
            Type _type;
            public Type Type
            {
                get
                {
                    if (!_typeInited && !String.IsNullOrEmpty(this.Name) && !String.IsNullOrEmpty(this.Namespace))
                    {
                        _type = Type.GetType(String.Format("{0}.{1}, {2}", this.Namespace, this.Name, this.Assembly), false, true);
                        _typeInited = true;
                    }
                    return _type;
                }
            }

			public string XmlPrefix
			{
				get { return this.xmlPrefix; }
			}

			public override string ToString()
			{
				if (null == this.xmlPrefix || 0 == this.xmlPrefix.Length)
					return this.Name;

				return this.xmlPrefix + ":" + this.Name;
			}
		}
	
		private enum PropertyType
		{
			Value,
			Content,
			Declaration,
			List,
			Dictionary,
			Complex,
			Namespace
		}
	
		private class Property
		{
			private PropertyType propertyType;
			private PropertyDeclaration propertyDeclaration;
			private object value;
	
			public Property(PropertyType propertyType)
			{
				this.propertyType = propertyType;
			}
	
			public PropertyType PropertyType
			{
				get
				{
					return this.propertyType;
				}
			}

			public PropertyDeclaration PropertyDeclaration
			{
				get
				{
					return this.propertyDeclaration;
				}
	
				set
				{
					this.propertyDeclaration = value;
				}
			}
	
			public object Value
			{
				get
				{
					return this.value;
				}
	
				set
				{
					this.value = value;
				}
			}
	
			public override string ToString()
			{
				/*
				using (StringWriter stringWriter = new StringWriter())
				{
					using (IndentationTextWriter indentationTextWriter = new IndentationTextWriter(stringWriter))
					{
						indentationTextWriter.Write(this.PropertyDeclaration.Name);
						indentationTextWriter.Write("=");
						indentationTextWriter.WriteLine();
						WritePropertyValue(this, indentationTextWriter);
					}

					return stringWriter.ToString();
				}
				*/
				
				return this.PropertyDeclaration.Name;
			}
		}
	
		private class PropertyDeclaration
		{
			private string name;
			private TypeDeclaration declaringType;

			public PropertyDeclaration(string name)
			{
				this.name = name;
				this.declaringType = null;
			}

			public PropertyDeclaration(string name, TypeDeclaration declaringType)
			{
				this.name = name;
				this.declaringType = declaringType;
			}

			public TypeDeclaration DeclaringType
			{
				get
				{
					return this.declaringType;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}
	
			public override string ToString()
			{
				if ((this.DeclaringType != null) && (this.DeclaringType.Name == "XmlNamespace") && (this.DeclaringType.Namespace == null) && (this.DeclaringType.Assembly == null))
				{
					if ((this.Name == null) || (this.Name.Length == 0))
					{
						return "xmlns";
					}

					return "xmlns:" + this.Name;
				}

				return this.Name;
			}
		}
	
		private class PropertyCollection : IEnumerable
		{
			private ArrayList list = new ArrayList();
	
			public void Add(Property value)
			{
				this.list.Add(value);
			}
	
			public void Remove(Property value)
			{
				this.list.Remove(value);
			}
	
			public int Count
			{
				get
				{
					return this.list.Count;
				}
			}
	
			public IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator();
			}
	
			public Property this[int index]
			{
				get
				{
					return (Property) this.list[index];
				}
			}
		}
	
		private class ResourceName
		{
			private string name;
	
			public ResourceName(string name)
			{
				this.name = name;
			}
	
			public string Name
			{
				get
				{
					return this.name;
				}
			}
	
			public override string ToString()
			{
				return this.Name;
			}
		}

		private class NamespaceManager
		{
			private HybridDictionary table = new HybridDictionary();
			private HybridDictionary reverseTable = new HybridDictionary();
			private Stack mappingStack = new Stack();

			internal NamespaceManager()
			{
			}

			public void AddNamespaceMapping(string xmlNamespace, string clrNamespace, string assembly)
			{
				ClrNamespace ns = new ClrNamespace(clrNamespace, assembly);
				table[xmlNamespace] = ns;
				reverseTable[ns] = xmlNamespace;
			}

			internal string GetXmlNamespace(TypeDeclaration type)
			{
				ClrNamespace ns = new ClrNamespace(type.Namespace, type.Assembly);
				return (string)reverseTable[ns];
			}

			internal void OnElementStart()
			{
				ElementEntry entry = new ElementEntry();
				this.mappingStack.Push(entry);
			}

			internal void OnElementEnd()
			{
				this.mappingStack.Pop();
			}

			internal void AddMapping(string prefix, string xmlNamespace)
			{
				ElementEntry element = (ElementEntry)this.mappingStack.Peek();
				element.MappingTable[xmlNamespace] = prefix;
			}

			internal string GetPrefix(string xmlNamespace)
			{
				foreach (ElementEntry element in this.mappingStack)
				{
					if (element.HasMappingTable)
					{
						if (element.MappingTable.Contains(xmlNamespace))
							return (string)element.MappingTable[xmlNamespace];
					}
				}

				return null;
			}

			private class ElementEntry
			{
				private HybridDictionary mappingTable;

				internal bool HasMappingTable
				{
					get { return null != this.mappingTable; }
				}

				internal HybridDictionary MappingTable
				{
					get
					{
						if (null == this.mappingTable)
							this.mappingTable = new HybridDictionary();

						return this.mappingTable;
					}
				}
			}
		}

		internal struct ClrNamespace
		{
			public string Namespace;
			public string Assembly;

			public ClrNamespace(string clrNamespace, string assembly)
			{
				this.Namespace = clrNamespace;
				this.Assembly = assembly;
			}
		}

		internal class KnownColors
		{
			private static readonly Hashtable colorTable;

			static KnownColors()
			{
				colorTable = new Hashtable();
				colorTable[0xFFF0F8FF] = "AliceBlue";
				colorTable[0xFFFAEBD7] = "AntiqueWhite";
				colorTable[0xFF00FFFF] = "Aqua";
				colorTable[0xFF7FFFD4] = "Aquamarine";
				colorTable[0xFFF0FFFF] = "Azure";
				colorTable[0xFFF5F5DC] = "Beige";
				colorTable[0xFFFFE4C4] = "Bisque";
				colorTable[0xFF000000] = "Black";
				colorTable[0xFFFFEBCD] = "BlanchedAlmond";
				colorTable[0xFF0000FF] = "Blue";
				colorTable[0xFF8A2BE2] = "BlueViolet";
				colorTable[0xFFA52A2A] = "Brown";
				colorTable[0xFFDEB887] = "BurlyWood";
				colorTable[0xFF5F9EA0] = "CadetBlue";
				colorTable[0xFF7FFF00] = "Chartreuse";
				colorTable[0xFFD2691E] = "Chocolate";
				colorTable[0xFFFF7F50] = "Coral";
				colorTable[0xFF6495ED] = "CornflowerBlue";
				colorTable[0xFFFFF8DC] = "Cornsilk";
				colorTable[0xFFDC143C] = "Crimson";
				colorTable[0xFF00FFFF] = "Cyan";
				colorTable[0xFF00008B] = "DarkBlue";
				colorTable[0xFF008B8B] = "DarkCyan";
				colorTable[0xFFB8860B] = "DarkGoldenrod";
				colorTable[0xFFA9A9A9] = "DarkGray";
				colorTable[0xFF006400] = "DarkGreen";
				colorTable[0xFFBDB76B] = "DarkKhaki";
				colorTable[0xFF8B008B] = "DarkMagenta";
				colorTable[0xFF556B2F] = "DarkOliveGreen";
				colorTable[0xFFFF8C00] = "DarkOrange";
				colorTable[0xFF9932CC] = "DarkOrchid";
				colorTable[0xFF8B0000] = "DarkRed";
				colorTable[0xFFE9967A] = "DarkSalmon";
				colorTable[0xFF8FBC8F] = "DarkSeaGreen";
				colorTable[0xFF483D8B] = "DarkSlateBlue";
				colorTable[0xFF2F4F4F] = "DarkSlateGray";
				colorTable[0xFF00CED1] = "DarkTurquoise";
				colorTable[0xFF9400D3] = "DarkViolet";
				colorTable[0xFFFF1493] = "DeepPink";
				colorTable[0xFF00BFFF] = "DeepSkyBlue";
				colorTable[0xFF696969] = "DimGray";
				colorTable[0xFF1E90FF] = "DodgerBlue";
				colorTable[0xFFB22222] = "Firebrick";
				colorTable[0xFFFFFAF0] = "FloralWhite";
				colorTable[0xFF228B22] = "ForestGreen";
				colorTable[0xFFFF00FF] = "Fuchsia";
				colorTable[0xFFDCDCDC] = "Gainsboro";
				colorTable[0xFFF8F8FF] = "GhostWhite";
				colorTable[0xFFFFD700] = "Gold";
				colorTable[0xFFDAA520] = "Goldenrod";
				colorTable[0xFF808080] = "Gray";
				colorTable[0xFF008000] = "Green";
				colorTable[0xFFADFF2F] = "GreenYellow";
				colorTable[0xFFF0FFF0] = "Honeydew";
				colorTable[0xFFFF69B4] = "HotPink";
				colorTable[0xFFCD5C5C] = "IndianRed";
				colorTable[0xFF4B0082] = "Indigo";
				colorTable[0xFFFFFFF0] = "Ivory";
				colorTable[0xFFF0E68C] = "Khaki";
				colorTable[0xFFE6E6FA] = "Lavender";
				colorTable[0xFFFFF0F5] = "LavenderBlush";
				colorTable[0xFF7CFC00] = "LawnGreen";
				colorTable[0xFFFFFACD] = "LemonChiffon";
				colorTable[0xFFADD8E6] = "LightBlue";
				colorTable[0xFFF08080] = "LightCoral";
				colorTable[0xFFE0FFFF] = "LightCyan";
				colorTable[0xFFFAFAD2] = "LightGoldenrodYellow";
				colorTable[0xFFD3D3D3] = "LightGray";
				colorTable[0xFF90EE90] = "LightGreen";
				colorTable[0xFFFFB6C1] = "LightPink";
				colorTable[0xFFFFA07A] = "LightSalmon";
				colorTable[0xFF20B2AA] = "LightSeaGreen";
				colorTable[0xFF87CEFA] = "LightSkyBlue";
				colorTable[0xFF778899] = "LightSlateGray";
				colorTable[0xFFB0C4DE] = "LightSteelBlue";
				colorTable[0xFFFFFFE0] = "LightYellow";
				colorTable[0xFF00FF00] = "Lime";
				colorTable[0xFF32CD32] = "LimeGreen";
				colorTable[0xFFFAF0E6] = "Linen";
				colorTable[0xFFFF00FF] = "Magenta";
				colorTable[0xFF800000] = "Maroon";
				colorTable[0xFF66CDAA] = "MediumAquamarine";
				colorTable[0xFF0000CD] = "MediumBlue";
				colorTable[0xFFBA55D3] = "MediumOrchid";
				colorTable[0xFF9370DB] = "MediumPurple";
				colorTable[0xFF3CB371] = "MediumSeaGreen";
				colorTable[0xFF7B68EE] = "MediumSlateBlue";
				colorTable[0xFF00FA9A] = "MediumSpringGreen";
				colorTable[0xFF48D1CC] = "MediumTurquoise";
				colorTable[0xFFC71585] = "MediumVioletRed";
				colorTable[0xFF191970] = "MidnightBlue";
				colorTable[0xFFF5FFFA] = "MintCream";
				colorTable[0xFFFFE4E1] = "MistyRose";
				colorTable[0xFFFFE4B5] = "Moccasin";
				colorTable[0xFFFFDEAD] = "NavajoWhite";
				colorTable[0xFF000080] = "Navy";
				colorTable[0xFFFDF5E6] = "OldLace";
				colorTable[0xFF808000] = "Olive";
				colorTable[0xFF6B8E23] = "OliveDrab";
				colorTable[0xFFFFA500] = "Orange";
				colorTable[0xFFFF4500] = "OrangeRed";
				colorTable[0xFFDA70D6] = "Orchid";
				colorTable[0xFFEEE8AA] = "PaleGoldenrod";
				colorTable[0xFF98FB98] = "PaleGreen";
				colorTable[0xFFAFEEEE] = "PaleTurquoise";
				colorTable[0xFFDB7093] = "PaleVioletRed";
				colorTable[0xFFFFEFD5] = "PapayaWhip";
				colorTable[0xFFFFDAB9] = "PeachPuff";
				colorTable[0xFFCD853F] = "Peru";
				colorTable[0xFFFFC0CB] = "Pink";
				colorTable[0xFFDDA0DD] = "Plum";
				colorTable[0xFFB0E0E6] = "PowderBlue";
				colorTable[0xFF800080] = "Purple";
				colorTable[0xFFFF0000] = "Red";
				colorTable[0xFFBC8F8F] = "RosyBrown";
				colorTable[0xFF4169E1] = "RoyalBlue";
				colorTable[0xFF8B4513] = "SaddleBrown";
				colorTable[0xFFFA8072] = "Salmon";
				colorTable[0xFFF4A460] = "SandyBrown";
				colorTable[0xFF2E8B57] = "SeaGreen";
				colorTable[0xFFFFF5EE] = "SeaShell";
				colorTable[0xFFA0522D] = "Sienna";
				colorTable[0xFFC0C0C0] = "Silver";
				colorTable[0xFF87CEEB] = "SkyBlue";
				colorTable[0xFF6A5ACD] = "SlateBlue";
				colorTable[0xFF708090] = "SlateGray";
				colorTable[0xFFFFFAFA] = "Snow";
				colorTable[0xFF00FF7F] = "SpringGreen";
				colorTable[0xFF4682B4] = "SteelBlue";
				colorTable[0xFFD2B48C] = "Tan";
				colorTable[0xFF008080] = "Teal";
				colorTable[0xFFD8BFD8] = "Thistle";
				colorTable[0xFFFF6347] = "Tomato";
				colorTable[0x00FFFFFF] = "Transparent";
				colorTable[0xFF40E0D0] = "Turquoise";
				colorTable[0xFFEE82EE] = "Violet";
				colorTable[0xFFF5DEB3] = "Wheat";
				colorTable[0xFFFFFFFF] = "White";
				colorTable[0xFFF5F5F5] = "WhiteSmoke";
				colorTable[0xFFFFFF00] = "Yellow";
				colorTable[0xFF9ACD32] = "YellowGreen";
			}

			internal static string KnownColorFromUInt(UInt32 argb)
			{
				//Debug.Assert(colorTable.Contains(argb));
				return (string)colorTable[argb];
			}
		}

		private class PathDataParser
		{
			internal static object ParseStreamGeometry(BamlBinaryReader reader)
			{
				StringBuilder sb = new StringBuilder();
				bool shouldClose = false;
				char lastChar = '\0';

				while (true)
				{
					byte b = reader.ReadByte();
					bool bit1 = (b & 0x10) == 0x10;
					bool bit2 = (b & 0x20) == 0x20;
					bool bit3 = (b & 0x40) == 0x40;
					bool bit4 = (b & 0x80) == 0x80;

					switch (b & 0xF)
					{
						case 0x0: //Begin
							{
								shouldClose = bit2;

								AddPathCommand('M', ref lastChar, sb);
								AddPathPoint(reader, sb, bit3, bit4);
								break;
							}
						case 0x1: //LineTo
							{
								AddPathCommand('L', ref lastChar, sb);
								AddPathPoint(reader, sb, bit3, bit4);
								break;
							}
						case 0x2: //QuadraticBezierTo
							{
								AddPathCommand('Q', ref lastChar, sb);
								AddPathPoint(reader, sb, bit3, bit4);
								AddPathPoint(reader, sb);
								break;
							}
						case 0x3: //BezierTo
							{
								AddPathCommand('C', ref lastChar, sb);
								AddPathPoint(reader, sb, bit3, bit4);
								AddPathPoint(reader, sb);
								AddPathPoint(reader, sb);
								break;
							}
						case 0x4: //PolyLineTo
							{
								bool isStroked = bit1;
								bool isSmooth = bit2;
								AddPathCommand('L', ref lastChar, sb);
								int count = reader.ReadInt32();

								for (int i = 0; i < count; i++)
									AddPathPoint(reader, sb);
								break;
							}
						case 0x5: //PolyQuadraticBezierTo
							{
								AddPathCommand('Q', ref lastChar, sb);
								int count = reader.ReadInt32();
								//System.Diagnostics.Debug.Assert(count % 2 == 0);
								for (int i = 0; i < count; i++)
									AddPathPoint(reader, sb);
								break;
							}
						case 0x6: //PolyBezierTo
							{
								AddPathCommand('C', ref lastChar, sb);
								int count = reader.ReadInt32();
								//System.Diagnostics.Debug.Assert(count % 3 == 0);
								for (int i = 0; i < count; i++)
									AddPathPoint(reader, sb);
								break;
							}
						case 0x7: //ArcTo
							{
								double endPtX = ReadPathDouble(reader, bit3);
								double endPtY = ReadPathDouble(reader, bit4);
								byte arcInfo = reader.ReadByte();
								bool isLarge = (arcInfo & 0xF) != 0;
								bool clockWise = (arcInfo & 0xF0) != 0;
								double sizeX = reader.ReadCompressedDouble();
								double sizeY = reader.ReadCompressedDouble();
								double angle = reader.ReadCompressedDouble();
								sb.AppendFormat("A {0},{1} {2} {3} {4} {5},{6} ", sizeX, sizeY, angle, isLarge ? 1 : 0, clockWise ? 1 : 0, endPtX, endPtY);
								lastChar = 'A';
								break;
							}
						case 0x8: //Closed
							{
								if (shouldClose)
								{
									sb.Append("Z");
								}
								else if (sb.Length > 0)
								{
									// trim off the ending space
									sb.Remove(sb.Length - 1, 0);
								}

								return sb.ToString();
							}
						case 0x9: //FillRule
							{
								sb.Insert(0, bit1 ? "F1 " : "F0 ");
								lastChar = 'F';
								break;
							}
						default:
							throw new InvalidOperationException();
					}
				}
			}

			private static void AddPathCommand(char commandChar, ref char lastCommandChar, StringBuilder sb)
			{
				if (commandChar != lastCommandChar)
				{
					lastCommandChar = commandChar;
					sb.Append(commandChar);
					sb.Append(' ');
				}
			}

			private static void AddPathPoint(BamlBinaryReader reader, StringBuilder sb, bool flag1, bool flag2)
			{
				sb.AppendFormat("{0},{1} ", ReadPathDouble(reader, flag1), ReadPathDouble(reader, flag2));
			}

			private static void AddPathPoint(BamlBinaryReader reader, StringBuilder sb)
			{
				sb.AppendFormat("{0},{1} ", reader.ReadCompressedDouble(), reader.ReadCompressedDouble());
			}

			private static double ReadPathDouble(BamlBinaryReader reader, bool isInt)
			{
				if (isInt)
					return reader.ReadInt32() * 1E-06;

				return reader.ReadCompressedDouble();
			}
		}
	}
}