using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;
using System.Globalization;
using SimpleUtils;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;

namespace SimpleAssemblyExplorer
{
    public partial class frmRunMethod : frmBase
    {
        private IHost _host;
        private string[] _rows;
        private string _sourceDir;
        private DataTable _parameters;

        public frmRunMethod(IHost mainForm, string[] rows, string sourceDir)
        {
            InitializeComponent();

            _host = mainForm;
            _rows = rows;
            _sourceDir = SimplePath.GetFullPath(sourceDir);

            _parameters = new DataTable();
            _parameters.Columns.Add("name", typeof(String));
            _parameters.Columns.Add("type", typeof(String));
            _parameters.Columns.Add("value", typeof(String));

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            MethodDefinition md = _frmClassView.SelectedMethod;
            if (md == null)
            {
                SimpleMessage.ShowInfo("Please choose a method");
                return;
            }

            bool resolveDirAdded = false;
            try
            {
                Utils.EnableUI(this.Controls, false);
                resolveDirAdded = _host.AddAssemblyResolveDir(_sourceDir);

                //string path = _rows[0];
                MethodBase mb = AssemblyUtils.ResolveMethod(md);

                object[] p = new object[_parameters.Rows.Count];

                for (int i = 0; i < _parameters.Rows.Count; i++)
                {
                    string s;
                    s = _parameters.Rows[i]["value"] == null ? String.Empty : _parameters.Rows[i]["value"].ToString();
                    if (!String.IsNullOrEmpty(s) && s.IndexOf("\\u") >= 0)
                    {
                        try
                        {
                            s = SimpleParse.ParseUnicodeString(s);
                        }
                        catch
                        {
                        }
                    }

                    string paramType = _parameters.Rows[i]["type"].ToString();
                    switch (paramType)
                    {
                        case "System.String":
                            p[i] = s;
                            break;
                        case "System.Int32":
                            if (s.StartsWith("0x"))
                                p[i] = Int32.Parse(s.Substring(2), NumberStyles.HexNumber);
                            else
                                p[i] = Int32.Parse(s, NumberStyles.Any);
                            break;
                        case "System.Int16":
                            if (s.StartsWith("0x"))
                                p[i] = Int16.Parse(s.Substring(2), NumberStyles.HexNumber);
                            else
                                p[i] = Int16.Parse(s, NumberStyles.Any);
                            break;
                        case "System.Int64":
                            if (s.StartsWith("0x"))
                                p[i] = Int64.Parse(s.Substring(2), NumberStyles.HexNumber);
                            else
                                p[i] = Int64.Parse(s, NumberStyles.Any);
                            break;
                        case "System.UInt32":
                            if (s.StartsWith("0x"))
                                p[i] = UInt32.Parse(s.Substring(2), NumberStyles.HexNumber);
                            else
                                p[i] = UInt32.Parse(s, NumberStyles.Any);
                            break;
                        case "System.UInt16":
                            if (s.StartsWith("0x"))
                                p[i] = UInt16.Parse(s.Substring(2), NumberStyles.HexNumber);
                            else
                                p[i] = UInt16.Parse(s, NumberStyles.Any);
                            break;
                        case "System.UInt64":
                            if (s.StartsWith("0x"))
                                p[i] = UInt64.Parse(s.Substring(2), NumberStyles.HexNumber);
                            else
                                p[i] = UInt64.Parse(s, NumberStyles.Any);
                            break;
                        case "System.Decimal":
                            if (s.StartsWith("0x"))
                                p[i] = Decimal.Parse(s.Substring(2), NumberStyles.HexNumber);
                            else
                                p[i] = Decimal.Parse(s, NumberStyles.Any);
                            break;
                        case "System.Double":
                            if (s.StartsWith("0x"))
                                p[i] = Double.Parse(s.Substring(2), NumberStyles.HexNumber);
                            else
                                p[i] = Double.Parse(s, NumberStyles.Any);
                            break;
                        case "System.Byte":
                            p[i] = Convert.ToByte(s);
                            break;
                        case "System.SByte":
                            p[i] = Convert.ToSByte(s);
                            break;
                        case "System.Boolean":
                            p[i] = Convert.ToBoolean(s);
                            break;
                        case "System.Char":
                            p[i] = Convert.ToChar(s);
                            break;
                        case "System.DateTime":
                            p[i] = DateTime.Parse(s);
                            break;
                        case "System.Byte[]":                            
                            string[] byteStrs = s.Replace(" ","").Replace("\r","").Replace("\n","").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            byte[] bytes = new byte[byteStrs.Length];
                            for (int j = 0; j < bytes.Length; j++ )
                            {
                                string tmpStr = byteStrs[j];
                                if (tmpStr.StartsWith("0x"))
                                    bytes[j] = Byte.Parse(tmpStr.Substring(2), NumberStyles.HexNumber);
                                else
                                    bytes[j] = Byte.Parse(tmpStr, NumberStyles.Any);
                            }
                            p[i] = bytes;
                            break;
                        default:
                            p[i] = s;
                            break;
                    }
                }


                SimpleTextbox.AppendText(txtInfo, String.Format("=== Started at {0} ===\r\n", DateTime.Now));

                object o = mb.Invoke(null, p);
                if (o != null)
                {
                    SimpleTextbox.AppendText(txtInfo, String.Format("Return Value: {0}\r\n", o));
                    ParameterInfo[] pi = mb.GetParameters();
                    for (int i = 0; i < pi.Length; i++)
                    {
                        if (pi[i].IsOut || pi[i].ParameterType.IsByRef)
                        {
                            SimpleTextbox.AppendText(txtInfo, String.Format("{0}: {1}\r\n", pi[i].Name, p[i]));
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                SimpleMessage.ShowException(ex);
            }
            finally
            {
                Utils.EnableUI(this.Controls, true);
                if(resolveDirAdded)
                    _host.RemoveAssemblyResolveDir(_sourceDir);
                SimpleTextbox.AppendText(txtInfo, String.Format("=== Completed at {0} ===\r\n\r\n", DateTime.Now));
            }
           
            //this.Close();
        }

        frmClassEdit _frmClassView = null;
        private void btnSelectMethod_Click(object sender, EventArgs e)
        {
            if(_frmClassView==null)
                _frmClassView = new frmClassEdit(
                    new ClassEditParams() {
                        Host = _host,
                        Rows = _rows,
                        SourceDir = _sourceDir,
                        ObjectType = ObjectTypes.Method,
                        ShowStaticOnly = true,
                        ShowSelectButton = true
                    });
            _frmClassView.ShowDialog();

            if (_frmClassView.SelectedMethod != null)
            {
                txtMethod.Text = _frmClassView.SelectedPath;

                _parameters.Clear();
                MethodDefinition md = _frmClassView.SelectedMethod;
                for (int i = 0; i < md.Parameters.Count; i++)
                {
                    ParameterDefinition pd = md.Parameters[i];
                    DataRow dr = _parameters.NewRow();
                    dr["name"] = pd.Name;
                    dr["type"] = pd.ParameterType.FullName;
                    dr["value"] = String.Empty;
                    _parameters.Rows.Add(dr);
                }
                dgvParams.DataSource = _parameters;
            }
            else
            {
                txtMethod.Text = String.Empty;
                dgvParams.DataSource = null;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtInfo.Text = String.Empty;
        }

        //private void txtInfo_TextChanged(object sender, EventArgs e)
        //{
        //    Utils.ScrollToBottom(txtInfo);
        //}

    } // end of class
}