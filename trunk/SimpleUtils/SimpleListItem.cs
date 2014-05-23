using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleUtils
{
    public class SimpleListItem
    {
        string _value;
        string _text;

        public SimpleListItem(string value)
        {
            _value = value;
            _text = value;
        }

        public SimpleListItem(string value, string text)
        {
            _value = value;
            _text = text;
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public override string ToString()
        {
            if (_text == null) return String.Empty;
            return _text;
        }

    }//end of class
}
