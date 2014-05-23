using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SimpleAssemblyExplorer.Plugin
{
    /// <summary>
    /// Plugin information
    /// </summary>
    public class PluginInfoBase
    {
        protected string _title;
        protected string _url;
        protected string _author;
        protected string _contact;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PluginInfoBase()
        {
        }

        /// <summary>
        /// Plugin Title which will be shown on menu
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Plugin Author
        /// </summary>
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        /// <summary>
        /// Email address of plugin author
        /// </summary>
        public string Contact
        {
            get { return _contact; }
            set { _contact = value; }
        }

        /// <summary>
        /// Web site of plugin
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }     

    }
}
