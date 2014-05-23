using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Mono.Cecil;
using SimpleUtils;
using SimpleUtils.Win;

namespace SimpleAssemblyExplorer
{
    public class ClassEditLogHandler
    {
        frmClassEdit _form;
        TextBox txtLog;

        public ClassEditLogHandler(frmClassEdit form)        
        {
            _form = form;
            txtLog = _form.LogText;
        }

        public void Log(string label, string content)
        {
            SimpleTextbox.AppendText(txtLog, String.Format("{0:yyyy-MM-dd hh:mm:ss}  {1}:  {2}\r\n", DateTime.Now, label, content));
        }

        public void LogLoad(string file)
        {
            Log("Load", file);
        }

        public void LogRename(string oldName, string newName)
        {
            Log("Rename", String.Format("\"{0}\" to \"{1}\"", oldName, newName));
        }

        public void btnSaveLog_Click(object sender, EventArgs e)
        {
            try 
            {
                using (new SimpleWaitCursor())
                {

                    TreeView treeView1 = _form.TreeView;
                    AssemblyDefinition ad = _form.TreeViewHandler.GetCurrentAssembly();
                    if (ad == null)
                    {
                        SimpleMessage.ShowInfo("Cannot determine current assembly.");
                        return;
                    }

                    string initFileName = String.Empty;
                    for (int i = 0; i < treeView1.Nodes.Count; i++)
                    {
                        if (ad.Equals(treeView1.Nodes[i].Tag))
                        {
                            initFileName = treeView1.Nodes[i].Text;
                            break;
                        }
                    }
                    initFileName = initFileName + ".log.txt";
                    string initDir = Config.ClassEditorSaveAsDir;
                    if (!Directory.Exists(initDir))
                        initDir = Environment.CurrentDirectory;
                    string path = SimpleDialog.OpenFile("Save Log",
                        "Text files (*.txt)|*.txt",
                        ".txt", false, initDir, initFileName);
                    if (!String.IsNullOrEmpty(path))
                    {
                        Config.ClassEditorSaveAsDir = Path.GetDirectoryName(path);
                    }

                    if (String.IsNullOrEmpty(path)) return;

                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(txtLog.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }           
        }

    } // end of class
}
