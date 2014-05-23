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
    public class ClassEditVariableGridHandler
    {
        frmClassEdit _form;
        DataGridView dgVariable;
        ContextMenuStrip cmVariable;

        public ClassEditVariableGridHandler(frmClassEdit form)        
        {
            _form = form;
            dgVariable = _form.VariableDataGrid;
            cmVariable = _form.VariableContextMenuStrip;
        }

        const int COL_INDEX = 0;

        private int GetVariableIndex(DataGridViewRow row)
        {
            if (row == null) return -1;
            object o = row.Cells[COL_INDEX].Value;
            if (o == null || Convert.IsDBNull(o))
                return -1;
            return Convert.ToInt32(o);
        }

        //private frmClassEditVariable _frmEditVar = null;
        private bool CallEditVariable(frmClassEditVariable.EditModes editMode)
        {
            int varIndex = GetVariableIndex(dgVariable.CurrentRow);
            if (varIndex >= 0 || editMode == frmClassEditVariable.EditModes.Append)
            {
                //if (_frmEditVar == null)
                //{
                //    _frmEditVar = new frmClassEditVariable(_currentMethod, varIndex, editMode);
                //}
                //else
                //{
                //    _frmEditVar.InitForm(_currentMethod, varIndex, editMode);
                //}

                MethodDefinition currentMethod = _form.BodyGridHandler.CurrentMethod;
                frmClassEditVariable f = new frmClassEditVariable(currentMethod, varIndex, editMode);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    _form.BodyGridHandler.InitBody(currentMethod);
                }
                return true;
            }
            return false;
        }

        public void dgVariable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CallEditVariable(frmClassEditVariable.EditModes.Edit);
        }

        public void dgVariable_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && !dgVariable.Rows[e.RowIndex].Selected)
            {
                dgVariable.CurrentCell = dgVariable.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        public void dgVariable_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Apps)
            {
                if (dgVariable.SelectedRows.Count > 0)
                {
                    Rectangle rect = dgVariable.GetCellDisplayRectangle(1, dgVariable.SelectedRows[0].Index, true);
                    Point point = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                    point = dgVariable.PointToScreen(point);
                    cmVariable.Show(point);
                    e.Handled = true;
                }
                else
                {
                    Rectangle rect = dgVariable.GetColumnDisplayRectangle(1, true);
                    Point point = new Point(rect.Left + rect.Width / 2, rect.Top + dgVariable.ColumnHeadersHeight + dgVariable.RowTemplate.Height / 2);
                    point = dgVariable.PointToScreen(point);
                    cmVariable.Show(point);
                    e.Handled = true;
                }
            }
        }

        public void dgVariable_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (_form.BodyGridHandler.CurrentMethod == null)
                    return;
                Point point = System.Windows.Forms.Control.MousePosition;
                Control c = dgVariable.GetChildAtPoint(point);
                if (c != null) 
                    return;

                //Rectangle rect = dgVariable.GetColumnDisplayRectangle(1, true);
                //point = new Point(rect.Left + rect.Width / 2, rect.Top + dgVariable.ColumnHeadersHeight + dgVariable.RowTemplate.Height / 2);
                //point = dgVariable.PointToScreen(point);
                cmVariable.Show(point);                
            }
        }

        public void cmVarEdit_Click(object sender, EventArgs e)
        {
            CallEditVariable(frmClassEditVariable.EditModes.Edit);
        }

        public void cmVarInsertBefore_Click(object sender, EventArgs e)
        {
            CallEditVariable(frmClassEditVariable.EditModes.InsertBefore);
        }

        public void cmVarInsertAfter_Click(object sender, EventArgs e)
        {
            CallEditVariable(frmClassEditVariable.EditModes.InsertAfter);
        }

        public void cmVarDuplicate_Click(object sender, EventArgs e)
        {
            CallEditVariable(frmClassEditVariable.EditModes.Duplicate);
        }

        public void cmVarAppend_Click(object sender, EventArgs e)
        {
            CallEditVariable(frmClassEditVariable.EditModes.Append);
        }

        public void cmVarRemove_Click(object sender, EventArgs e)
        {
            if (dgVariable.SelectedRows == null || dgVariable.SelectedRows.Count < 1) return;

            MethodDefinition currentMethod = _form.BodyGridHandler.CurrentMethod;
            VariableDefinition[] saved = new VariableDefinition[dgVariable.SelectedRows.Count];
            for (int i = 0; i < dgVariable.SelectedRows.Count; i++)
            {
                int varIndex = GetVariableIndex(dgVariable.SelectedRows[i]);
                if (varIndex >= 0)
                {
                    saved[i] = currentMethod.Body.Variables[varIndex];
                }
            }
            foreach (VariableDefinition vd in saved)
            {
                if (vd != null)
                {
                    currentMethod.Body.Variables.Remove(vd);
                }
            }
            _form.BodyGridHandler.InitBody(currentMethod);
        }

        public void cmVariable_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = sender as ContextMenuStrip;
            //if (dgVariable.Rows.Count == 0)
            //{
            //    e.Cancel = true;
            //    return;
            //}

            if (_form.SaveAssemblyButton.Visible && dgVariable.SelectedRows.Count > 0)
            {
                cms.Items["cmVarRemove"].Enabled = true;
                bool enabled = dgVariable.SelectedRows.Count == 1;
                cms.Items["cmVarEdit"].Enabled = enabled;
                cms.Items["cmVarInsertBefore"].Enabled = enabled;
                cms.Items["cmVarInsertAfter"].Enabled = enabled;
                cms.Items["cmVarDuplicate"].Enabled = enabled;
            }
            else
            {
                foreach (ToolStripItem tsi in cms.Items)
                {
                    tsi.Enabled = false;
                }
                cms.Items["cmVarAppend"].Enabled = true;
            }
        }

    } // end of class
}
