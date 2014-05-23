using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface ITextFileWatcher
    {
        event FileSystemEventHandler TextFileChanged;
    }
}
