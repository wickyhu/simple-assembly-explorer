using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Metadata;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System.Collections;
using SimpleUtils;
using SimpleUtils.Win;
using SimpleAssemblyExplorer.Plugin;
using SimpleAssemblyExplorer.LutzReflector;

namespace SimpleAssemblyExplorer
{
    public class ClassEditSearchHandler
    {
        frmClassEdit _form;
        ToolStripComboBox cboSearchType;
        ToolStripComboBox cboSearch;

        public ClassEditSearchHandler(frmClassEdit form)
        {
            _form = form;
            cboSearchType = _form.SearchTypeComboBox;
            cboSearch = _form.SearchTextComboBox;

            cboSearch.Text = Config.ClassEditorLastSearch;
            InitSearchTypes();
        }

        private void InitSearchTypes()
        {
            cboSearchType.Items.Clear();
            foreach (string s in AssemblyUtils.GetEnumNames(typeof(SearchTypes)))
            {
                cboSearchType.Items.Add(s);
            }
            cboSearchType.SelectedIndex = (int)SearchTypes.ClassName;
        }

        private void AddSearchHistory(string searchFor)
        {
            if (cboSearch.Items.Count > 100)
            {
                cboSearch.Items.RemoveAt(0);
            }
            if (!cboSearch.Items.Contains(searchFor))
                cboSearch.Items.Insert(0, searchFor);
        }

        int _lastHighlightIndex = -1;
        public int LastHighlightIndex
        {
            get { return _lastHighlightIndex; }
            set { _lastHighlightIndex = value; }
        }

        public void cboSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            string searchFor = cboSearch.Text.Trim();
            if (cboSearch.Enabled && String.IsNullOrEmpty(searchFor))
                return;

            if (searchFor.StartsWith("0x") && searchFor.Length > 4)
            {
                switch (searchFor.Substring(2, 2))
                {
                    case "02":
                        SearchType = SearchTypes.ClassName;
                        break;
                    case "06":
                        SearchType = SearchTypes.MethodName;
                        break;
                    case "04":
                        SearchType = SearchTypes.FieldName;
                        break;
                    case "17":
                        SearchType = SearchTypes.PropertyName;
                        break;
                    case "14":
                        SearchType = SearchTypes.EventName;
                        break;
                }
            }

            Config.ClassEditorLastSearch = searchFor;

            TreeView treeView1 = _form.TreeView;
            DataGridView dgBody = _form.BodyDataGrid;

            switch (SearchType)
            {
                case SearchTypes.Name:
                case SearchTypes.ClassName:
                case SearchTypes.MethodName:
                case SearchTypes.PropertyName:
                case SearchTypes.FieldName:
                case SearchTypes.EventName:
                case SearchTypes.String:
                    {
                        TreeNode n = _form.TreeViewHandler.FindTreeNode(searchFor);
                        if (n != null)
                        {
                            treeView1.SelectedNode = n;
                            AddSearchHistory(searchFor);
                            if (!treeView1.SelectedNode.IsExpanded)
                            {
                                treeView1.SelectedNode.Expand();
                            }
                        }
                    }
                    break;
                case SearchTypes.Offset:
                    {
                        int offset;
                        if (int.TryParse(searchFor, System.Globalization.NumberStyles.HexNumber, null, out offset))
                        {
                            for (int i = 0; i < dgBody.Rows.Count; i++)
                            {
                                DataGridViewRow row = dgBody.Rows[i];
                                int insIndex = _form.BodyGridHandler.GetInstructionIndex(row);
                                if (insIndex >= 0)
                                {
                                    Instruction ins = _form.BodyGridHandler.CurrentMethod.Body.Instructions[insIndex];
                                    if (ins.Offset == offset)
                                    {
                                        _form.BodyGridHandler.SetCurrentRow(i);
                                        AddSearchHistory(searchFor);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case SearchTypes.Instruction:
                    {
                        int index;
                        if (int.TryParse(searchFor, out index))
                        {
                            for (int i = index; i < dgBody.Rows.Count; i++)
                            {
                                DataGridViewRow row = dgBody.Rows[i];
                                int insIndex = _form.BodyGridHandler.GetInstructionIndex(row);
                                if (insIndex == index)
                                {
                                    _form.BodyGridHandler.SetCurrentRow(i);
                                    AddSearchHistory(searchFor);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case SearchTypes.OpCode:
                    {
                        if (dgBody.Rows.Count > 0)
                        {
                            int currIndex;
                            if (dgBody.CurrentRow != null)
                                currIndex = dgBody.Rows.IndexOf(dgBody.CurrentRow);
                            else
                                currIndex = 0;
                            int startIndex = currIndex;
                            do
                            {
                                startIndex++;
                                if (startIndex >= dgBody.Rows.Count - 1)
                                    startIndex = 0;
                                DataGridViewRow row = dgBody.Rows[startIndex];
                                int insIndex = _form.BodyGridHandler.GetInstructionIndex(row);
                                if (insIndex >= 0)
                                {
                                    Instruction ins = _form.BodyGridHandler.CurrentMethod.Body.Instructions[insIndex];
                                    if (ins.OpCode.Name.ToString().Equals(searchFor, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        _form.BodyGridHandler.SetCurrentRow(startIndex);
                                        AddSearchHistory(searchFor);
                                        break;
                                    }
                                }
                            } while (startIndex != currIndex);
                        }
                    }
                    break;
                case SearchTypes.Operand:
                    {
                        if (dgBody.Rows.Count > 0)
                        {
                            int currIndex;
                            if (dgBody.CurrentRow != null)
                                currIndex = dgBody.Rows.IndexOf(dgBody.CurrentRow);
                            else
                                currIndex = 0;
                            int startIndex = currIndex;
                            do
                            {
                                startIndex++;
                                if (startIndex >= dgBody.Rows.Count - 1)
                                    startIndex = 0;
                                DataGridViewRow row = dgBody.Rows[startIndex];
                                int insIndex = _form.BodyGridHandler.GetInstructionIndex(row);
                                if (insIndex >= 0)
                                {
                                    Instruction ins = _form.BodyGridHandler.CurrentMethod.Body.Instructions[insIndex];
                                    string operandStr = (ins.Operand == null ? String.Empty : ins.Operand.ToString());
                                    if (operandStr.IndexOf(searchFor, 0, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        _form.BodyGridHandler.SetCurrentRow(startIndex);
                                        AddSearchHistory(searchFor);
                                        break;
                                    }
                                }
                            } while (startIndex != currIndex);
                        }
                    }
                    break;
                case SearchTypes.Highlight:
                    {
                        if (dgBody.Rows.Count > 0)
                        {
                            int currIndex = _lastHighlightIndex;
                            int startIndex = currIndex;
                            do
                            {
                                startIndex++;
                                if (startIndex >= dgBody.Rows.Count - 1)
                                    startIndex = 0;
                                DataGridViewRow row = dgBody.Rows[startIndex];
                                if (row.Selected)
                                {
                                    _lastHighlightIndex = startIndex;
                                    dgBody.FirstDisplayedScrollingRowIndex = startIndex < 10 ? 0 : startIndex - 10;
                                    break;
                                }
                            } while (startIndex != currIndex);
                        }
                    }
                    break;
            }
        }

        public void cboSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            cboSearch_SelectedIndexChanged(sender, e);
        }

        public SearchTypes SearchType
        {
            get
            {
                return (SearchTypes)Enum.Parse(typeof(SearchTypes), cboSearchType.SelectedItem.ToString());
            }
            set
            {
                string s = value.ToString();
                if (cboSearchType.Items.Contains(s))
                {
                    cboSearchType.SelectedItem = s;
                }
            }
        }

        public void tbSearchNext_Click(object sender, EventArgs e)
        {
            cboSearch_SelectedIndexChanged(sender, e);
        }

        public void cboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SearchType)
            {
                case SearchTypes.Highlight:
                    cboSearch.Enabled = false;
                    break;
                default:
                    cboSearch.Enabled = true;
                    cboSearch.Focus();
                    break;
            }
        }

        public void rtbText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (_form.TreeView.SelectedNode != null)
            {
                _form.TreeViewHandler.SaveNodeHistory(_form.TreeView.SelectedNode);
            }

            string[] data = e.LinkText.Split('#');
            if(data != null && data.Length>1) 
            {
                string token = data[data.Length-1];
                if (token.StartsWith("0x") && token.Length == 10)
                {
                    //TODO: need cache?
                    cboSearch.Text = token;
                    cboSearch_SelectedIndexChanged(sender, e);
                }
            }
            
        }

        //don't change order because it's used by cboSearchType
        public enum SearchTypes
        {
            Name,
            ClassName,
            MethodName,
            PropertyName,
            FieldName,
            EventName,
            String,
            Offset,
            OpCode,
            Operand,
            Instruction,
            Highlight
        }

    }// end of class
}
