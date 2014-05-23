using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SimpleAssemblyExplorer.Plugin
{
    /// <summary>
    /// Return values of plugin
    /// </summary>
    public enum PluginReturns
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        None,
        /// <summary>
        /// Refresh Grid
        /// </summary>
        Refresh
    }

    /// <summary>
    /// Source files type which plugin can handle
    /// </summary>
    public enum SourceTypes
    {
        /// <summary>
        /// Assemblies
        /// </summary>
        Assembly,
        /// <summary>
        /// Executable assembly
        /// </summary>
        Executable,
        /// <summary>
        /// IL files
        /// </summary>
        ILFile,
        /// <summary>
        /// Any files
        /// </summary>
        Any
    }

    /// <summary>
    /// Rows whick plugin can handle
    /// </summary>
    public enum RowTypes
    {
        /// <summary>
        /// One row
        /// </summary>
        One,
        /// <summary>
        /// Multiple row
        /// </summary>
        Multiple
    }
    
    public class MainPluginInfo : PluginInfoBase
    {
        protected SourceTypes _sourceType;
        protected RowTypes _rowType;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainPluginInfo()
        {
        }

        /// <summary>
        /// Source files which plugin can handle
        /// </summary>
        public SourceTypes SourceType
        {
            get { return _sourceType; }
            set { _sourceType = value; }
        }

        /// <summary>
        /// Rows which plugin can handle
        /// </summary>
        public RowTypes RowType
        {
            get { return _rowType; }
            set { _rowType = value; }
        }

    }

    /// <summary>
    /// Plugin Argument
    /// </summary>
    public class PluginArgument
    {
        IHost _host;
        string[] _rows;
        string _sourceDir;
        object _data;

        /// <summary>
        /// Plugin Argument
        /// </summary>
        /// <param name="host">provide access to host</param>
        /// <param name="rows">selected rows</param>
        /// <param name="sourceDir">current source directory</param>
        /// <param name="data">additional data</param>
        public PluginArgument(IHost host, string[] rows, string sourceDir, object data)
        {
            Init(host, rows, sourceDir, data);
        }

        public PluginArgument(IHost host, string[] rows, string sourceDir)
        {
            Init(host, rows, sourceDir, null);
        }

        private void Init(IHost host, string[] rows, string sourceDir, object data)
        {
            _host = host;
            _rows = rows;
            _sourceDir = sourceDir;
            _data = data;
        }

        public IHost Host
        {
            get { return _host; }
        }

        public string[] Rows
        {
            get { return _rows; }
        }

        public string SourceDir
        {
            get { return _sourceDir; }
        }

        public object Data
        {
            get { return _data; }
        }

    }
}
