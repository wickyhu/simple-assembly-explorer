using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleAssemblyExplorer
{
    public class ResourceFile : TextFile
    {
        static ResourceFile _default;
        public static ResourceFile Default
        {
            get
            {
                if (_default == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _default, new ResourceFile("Resource.txt"), null);
                }
                return _default;
            }
        }

        public ResourceFile(string fileName)
            : base(fileName)
        {
        }

        List<string> _textStringList;
        public List<string> TextStringList
        {
            get
            {
                ReadFile();
                return _textStringList;
            }
            set { _textStringList = value; }
        }

        List<string> _imageStringList;
        public List<string> ImageStringList
        {
            get
            {
                ReadFile();
                return _imageStringList;
            }
            set { _imageStringList = value; }
        }

        public bool IsInList(string name, List<string> list)
        {
            foreach (string s in list)
            {
                if (name.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public bool IsTextResource(string name)
        {
            return IsInList(name, TextStringList);
        }


        public bool IsImageResource(string name)
        {
            return IsInList(name, ImageStringList);
        }

        public override void ParseLines()
        {
            string profileName = null;

            _imageStringList = new List<string>();
            _textStringList = new List<string>();

            foreach (string line in this.Lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    profileName = line.Substring(1, line.Length - 2);
                }
                else if (profileName == "Text")
                {
                    _textStringList.Add(line);
                }
                else if (profileName == "Image")
                {
                    _imageStringList.Add(line);
                }
                //else if (profileName == "Binary")
                //{

                //}

            }//end foreach
            
        }      

        public override void Clear()
        {
            Clear(_imageStringList);
            Clear(_textStringList);
            base.Clear();
        }       

    }//end of class   

}
