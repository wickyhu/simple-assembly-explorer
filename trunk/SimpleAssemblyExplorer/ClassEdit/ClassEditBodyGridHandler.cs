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
    public class ClassEditBodyGridHandler
    {
        frmClassEdit _form;
        DataGridView dgBody;
        DataGridView dgGeneral;
        DataGridView dgVariable;
        TabPage tabDetails;

        private DataTable _dtBody = null;
        private DataTable _dtGeneral = null;
        private DataTable _dtVariable = null;

        const int COL_INDEX = 0;
        const int COL_OFFSET = 1;
        const int COL_OPCODE = 2;
        const int COL_OPERAND = 3;
        const int COL_OPERANDTYPE = 4;
        const string TRY_MARK = ".try";

        public ClassEditBodyGridHandler(frmClassEdit form)        
        {
            _form = form;
            dgBody = _form.BodyDataGrid;
            dgGeneral = _form.GeneralDataGrid;
            dgVariable = _form.VariableDataGrid;
            tabDetails = _form.DetailsTabPage;

            Utils.LoadILHelp();
            CreateDataTable();

            _markBlocks = Config.MarkBlocks;
        }

        private void CreateDataTable()
        {
            _dtBody = new DataTable("Body");
            _dtBody.Columns.Add("index", typeof(int));
            _dtBody.Columns.Add("address", typeof(String));
            _dtBody.Columns.Add("opcode", typeof(String));
            _dtBody.Columns.Add("operand", typeof(String));
            _dtBody.Columns.Add("operandtype", typeof(String));

            _dtGeneral = new DataTable("General");
            _dtGeneral.Columns.Add("content", typeof(String));

            _dtVariable = new DataTable("Variable");
            _dtVariable.Columns.Add("index", typeof(int));
            _dtVariable.Columns.Add("name", typeof(String));
            _dtVariable.Columns.Add("type", typeof(TypeReference));
            _dtVariable.Columns.Add("ispinned", typeof(Boolean));
        }


        private void InitDataTableGeneral(MethodDefinition md)
        {
            DataRow dr;

            dr = _dtGeneral.NewRow();
            _dtGeneral.Rows.Add(dr);

            dr = _dtGeneral.NewRow();
            dr["content"] = String.Format("/* Token: 0x{0:x08} */", md.MetadataToken.ToUInt32());
            _dtGeneral.Rows.Add(dr);

            dr = _dtGeneral.NewRow();
            dr["content"] = String.Format("/* RVA: 0x{0:x} */", md.RVA);
            _dtGeneral.Rows.Add(dr);

            dr = _dtGeneral.NewRow();
            dr["content"] = String.Format(".method {0}", InsUtils.GetMethodFullText(md));
            _dtGeneral.Rows.Add(dr);

            if (md.HasOverrides)
            {
                foreach (MethodReference mr in md.Overrides)
                {
                    dr = _dtGeneral.NewRow();
                    dr["content"] = String.Format(".override {0}", mr.ToString());
                    _dtGeneral.Rows.Add(dr);
                }
            }

            if (md.HasBody)
            {
                dr = _dtGeneral.NewRow();
                dr["content"] = String.Format(".maxstack {0}", md.Body.MaxStackSize);
                _dtGeneral.Rows.Add(dr);

                /*
                Collection<VariableDefinition> vdc = md.Body.Variables;
                if (vdc.Count > 0)
                {
                    //dr = _dtGeneral.NewRow();
                    //_dtGeneral.Rows.Add(dr);

                    dr = _dtGeneral.NewRow();
                    dr["content"] = ".locals init (";
                    _dtGeneral.Rows.Add(dr);

                    for (int i = 0; i < vdc.Count; i++)
                    {
                        dr = _dtGeneral.NewRow();
                        dr["content"] = String.Format("    [{0}] {1} {2}{3}", i, vdc[i].VariableType.Name, vdc[i].ToString(), ((i == vdc.Count - 1) ? "" : ","));
                        _dtGeneral.Rows.Add(dr);
                    }

                    dr = _dtGeneral.NewRow();
                    dr["content"] = ")";
                    _dtGeneral.Rows.Add(dr);
                }
                */
            }

            dr = _dtGeneral.NewRow();
            _dtGeneral.Rows.Add(dr);

            dgGeneral.Height = _dtGeneral.Rows.Count * dgGeneral.RowTemplate.Height;

        }

        private void InitDataTableBody(MethodDefinition md)
        {
            DataRow dr;
            if (md.HasBody)
            {
                MethodBody body = md.Body;
                
                dr = _dtBody.NewRow();
                _dtBody.Rows.Add(dr);

                Collection<Instruction> ic = body.Instructions;
                for (int i = 0; i < ic.Count; i++)
                {
                    dr = _dtBody.NewRow();
                    dr["index"] = i;
                    dr["address"] = String.Format("     {0}:", InsUtils.GetAddressText(ic[i]));
                    dr["opcode"] = ic[i].OpCode.ToString();
                    dr["operandtype"] = ic[i].Operand == null ? String.Empty : ic[i].Operand.GetType().Name;
                    dr["operand"] = InsUtils.GetOperandText(ic, i);
                    _dtBody.Rows.Add(dr);
                }

                Collection<ExceptionHandler> ehc = body.ExceptionHandlers;
                for (int i = 0; i < ehc.Count; i++)
                {
                    dr = _dtBody.NewRow();
                    dr["address"] = String.Format("     {0}{1}", TRY_MARK, i);
                    dr["opcode"] = String.Format("{0} to {1}", ic.IndexOf(ehc[i].TryStart), ic.IndexOf(ehc[i].TryEnd));
                    dr["operand"] = String.Format("{0} handler {1} {2} to {3}",
                        ehc[i].HandlerType.ToString(),
                        ehc[i].CatchType == null ? "" : String.Format("catch({0})", ehc[i].CatchType.FullName),
                        ehc[i].HandlerType == ExceptionHandlerType.Filter ? ic.IndexOf(ehc[i].FilterStart) : ic.IndexOf(ehc[i].HandlerStart),
                        ehc[i].HandlerType == ExceptionHandlerType.Filter ? ic.IndexOf(ehc[i].HandlerStart) : ic.IndexOf(ehc[i].HandlerEnd)
                        );
                    _dtBody.Rows.Add(dr);
                }

                dr = _dtBody.NewRow();
                _dtBody.Rows.Add(dr);
            }
        }

        private void InitDataTableVariable(MethodDefinition md)
        {
            DataRow dr;
            if (md.HasBody)
            {
                Collection<VariableDefinition> vdc = md.Body.Variables;
                for (int i = 0; i < vdc.Count; i++)
                {
                    dr = _dtVariable.NewRow();
                    dr["index"] = i;
                    dr["name"] = vdc[i].ToString();
                    dr["type"] = vdc[i].VariableType;
                    dr["ispinned"] = vdc[i].IsPinned;
                    _dtVariable.Rows.Add(dr);
                }
            }
        }

        MethodDefinition _currentMethod = null;
        public MethodDefinition CurrentMethod
        {
            get { return _currentMethod; }
        }

        int _currentRowIndex = -1;
        public int CurrentRowIndex {
            get { return _currentRowIndex; }
            set { _currentRowIndex = value; }
        }

        int _currentVarIndex = -1;

        public void InitBody(MethodDefinition md)
        {
            DateTime startTime = DateTime.Now;

            _form.SetStatusText(null);
            _form.SetStatusCount(0);
            _form.ResourceHandler.ShowDetailsControl(ClassEditResourceHandler.DetailTypes.MethodBody);
            SimpleWaitCursor wc = null;

            try
            {
                if (_currentMethod == md)
                {
                    if (dgBody.CurrentRow != null)
                    {
                        _currentRowIndex = dgBody.CurrentRow.Index;
                    }
                    if (dgVariable.CurrentRow != null)
                    {
                        _currentVarIndex = dgVariable.CurrentRow.Index;
                    }
                }

                tabDetails.SuspendLayout();
                dgBody.SuspendLayout();
                dgGeneral.SuspendLayout();

                _dtBody.Rows.Clear();
                _dtGeneral.Rows.Clear();
                _dtVariable.Rows.Clear();

                if (md == null)
                {
                    _currentMethod = null;
                    return;
                }

                _dtBody.BeginLoadData();
                _dtGeneral.BeginLoadData();
                _dtVariable.BeginLoadData();

                if (md.HasBody && md.Body.Instructions.Count > 100)
                {
                    wc = new SimpleWaitCursor();
                }

                _currentMethod = md;

                InitDataTableGeneral(md);
                InitDataTableBody(md);
                InitDataTableVariable(md);

                if (dgBody.DataSource == null)
                    dgBody.DataSource = _dtBody;

                if (dgGeneral.DataSource == null)
                    dgGeneral.DataSource = _dtGeneral;

                if (dgVariable.DataSource == null)
                    dgVariable.DataSource = _dtVariable;

                _dtBody.EndLoadData();
                _dtGeneral.EndLoadData();
                _dtVariable.EndLoadData();

                if (md.HasBody)
                {
                    Collection<ExceptionHandler> ehc = md.Body.ExceptionHandlers;

                    //check invalid exception handler
                    for (int i = dgBody.Rows.Count - 2; i >= 0; i--)
                    {
                        DataGridViewRow row = dgBody.Rows[i];
                        int ehIndex = GetExceptionHandlerIndex(row);
                        if (ehIndex >= 0)
                        {
                            if (!DeobfUtils.IsValidExceptionHandler(ehc[ehIndex]))
                            {
                                row.DefaultCellStyle.ForeColor = Color.Red;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                _form.SearchHandler.LastHighlightIndex = 0;

                if (_markBlocks)
                    MarkBlocks();

                if (_currentRowIndex >= 0)
                {
                    SetCurrentRow(_currentRowIndex);
                    _currentRowIndex = -1;
                }
                if (_currentVarIndex >= 0)
                {
                    SetCurrentVariable(_currentVarIndex);
                    _currentVarIndex = -1;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                dgBody.ResumeLayout();
                dgGeneral.ResumeLayout();
                tabDetails.ResumeLayout();

                if (wc != null) wc.Dispose();

                if (_currentMethod != null)
                {
                    DateTime endTime = DateTime.Now;
                    _form.SetStatusText(String.Format("{0} (load time: {1} seconds)",
                        _currentMethod.ToString(),
                        (endTime - startTime).TotalSeconds));
                }
            }
        }

        public void dgBody_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = GetInstructionIndex(e.RowIndex);
            if (index >= 0)
            {
                BodyCellDoubleClick(index, e.ColumnIndex);
                return;
            }

            int ehIndex = GetExceptionHandlerIndex(e.RowIndex);
            if (ehIndex >= 0)
            {
                cmEdit_Click(null, null);
                return;
            }
        }


        private void BodyCellDoubleClick(int insIndex, int colIndex)
        {
            Instruction ins = _currentMethod.Body.Instructions[insIndex];

            switch (colIndex)
            {
                case COL_OPERAND:
                    if (ins.Operand is MethodReference)
                    {
                        _form.TreeViewHandler.ShowMethod(ins.Operand as MethodReference);
                    }
                    else if (ins.Operand is Instruction)
                    {
                        int rowIndex = GetRowIndex(_currentMethod.Body.Instructions.IndexOf(ins.Operand as Instruction));
                        SetCurrentRow(rowIndex);
                    }
                    else if (ins.Operand is FieldReference)
                    {
                        _form.TreeViewHandler.ShowField(ins.Operand as FieldReference);
                    }
                    else
                    {
                        cmEdit_Click(null, null);
                    }
                    break;
                default:
                    cmEdit_Click(null, null);
                    break;
            }
        }

        public void dgBody_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Apps && dgBody.SelectedRows.Count > 0)
            {
                Rectangle rect = dgBody.GetCellDisplayRectangle(2, dgBody.SelectedRows[0].Index, true);
                Point point = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                point = dgBody.PointToScreen(point);
                _form.BodyContextMenuStrip.Show(point);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.D)
            {
                cmDeobf_Click(sender, e);
                e.Handled = true;
            }
        }

        public void dgBody_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && !dgBody.Rows[e.RowIndex].Selected)
            {
                //dgvData.Rows[e.RowIndex].Selected = true;
                dgBody.CurrentCell = dgBody.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        bool _markBlocks = false;
        private void MarkBlocks()
        {
            Color c;

            if (_markBlocks)
                c = Color.PaleGoldenrod;
            else
                c = dgBody.DefaultCellStyle.BackColor;

            List<InstructionBlock> list = InstructionBlock.Find(_currentMethod);
            if (list.Count == 0)
                return;

            bool changeColor = false;
            foreach (InstructionBlock ib in list)
            {
                if (changeColor)
                {
                    for (int j = ib.StartIndex; j <= ib.EndIndex; j++)
                    {
                        dgBody.Rows[j + 1].DefaultCellStyle.BackColor = c;
                    }
                }
                changeColor = !changeColor;
            }            
        }

        public void cmMarkBlocks_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
            _markBlocks = mi.Checked;

            MarkBlocks();

            Config.MarkBlocks = _markBlocks;
        }


        //http://windowsclient.net/blogs/faqs/archive/2006/07/10/how-do-i-perform-drag-and-drop-reorder-of-rows.aspx
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexUnderMouseToDrop;

        public void dgBody_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dgBody.DoDragDrop(
                        dgBody.Rows[rowIndexFromMouseDown],
                        DragDropEffects.Move);
                }
            }
        }

        public void dgBody_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // Get the index of the item the mouse is below.
                rowIndexFromMouseDown = dgBody.HitTest(e.X, e.Y).RowIndex;
                int columnIndexFromMouseDown = dgBody.HitTest(e.X, e.Y).ColumnIndex;

                if (GetInstructionIndex(rowIndexFromMouseDown) != -1 &&
                    columnIndexFromMouseDown == COL_INDEX)
                {
                    // Remember the point where the mouse down occurred. 
                    // The DragSize indicates the size that the mouse can move 
                    // before a drag event should be started.                
                    Size dragSize = SystemInformation.DragSize;
                    // Create a rectangle using the DragSize, with the mouse position being
                    // at the center of the rectangle.
                    dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                        e.Y - (dragSize.Height / 2)),
                        dragSize);
                }
                else
                {
                    // Reset the rectangle if the mouse is not over an item in the ListBox.
                    dragBoxFromMouseDown = Rectangle.Empty;
                }
            }
        }

        public void dgBody_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        public void dgBody_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dgBody.PointToClient(new Point(e.X, e.Y));
            // Get the row index of the item the mouse is below. 
            rowIndexUnderMouseToDrop = dgBody.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                //DataGridViewRow rowToMove = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
                int fromIndex = GetInstructionIndex(rowIndexFromMouseDown);
                int toIndex = GetInstructionIndex(rowIndexUnderMouseToDrop);

                if (fromIndex != -1 && toIndex != -1
                    && fromIndex != toIndex
                    && fromIndex != toIndex + 1)
                {
                    Instruction insFrom = _currentMethod.Body.Instructions[fromIndex];
                    Instruction newIns = InsUtils.CreateInstruction(insFrom.OpCode, insFrom.Operand);
                    Instruction insTo = _currentMethod.Body.Instructions[toIndex];

                    _currentMethod.Body.GetILProcessor().Remove(insFrom);
                    _currentMethod.Body.GetILProcessor().InsertAfter(insTo, newIns);

                    if (fromIndex <= toIndex)
                        SetCurrentRow(toIndex + 1);
                    else
                        SetCurrentRow(toIndex + 2);

                    InsUtils.ComputeOffsets(_currentMethod.Body.Instructions);
                    InitBody(_currentMethod);
                }
            }
        }

        public int GetInstructionIndex(int dataGridRowIndex)
        {
            if (dataGridRowIndex < 0 || dataGridRowIndex > dgBody.Rows.Count - 1)
                return -1;
            DataGridViewRow row = dgBody.Rows[dataGridRowIndex];
            return GetInstructionIndex(row);
        }

        public int GetInstructionIndex(DataGridViewRow row)
        {
            if (row == null) return -1;
            object o = row.Cells[COL_INDEX].Value;
            if (o == null || Convert.IsDBNull(o))
                return -1;
            return Convert.ToInt32(o);
        }

        public int GetExceptionHandlerIndex(int dataGridRowIndex)
        {
            if (dataGridRowIndex < 0 || dataGridRowIndex > dgBody.Rows.Count - 1)
                return -1;
            DataGridViewRow row = dgBody.Rows[dataGridRowIndex];
            return GetExceptionHandlerIndex(row);
        }

        public int GetExceptionHandlerIndex(DataGridViewRow row)
        {
            if (row == null) return -1;
            object o = row.Cells[COL_OFFSET].Value;
            if (o == null || Convert.IsDBNull(o))
                return -1;
            string s = o.ToString().Trim();
            if (s.StartsWith(TRY_MARK))
            {
                return Convert.ToInt32(s.Substring(TRY_MARK.Length));
            }
            return -1;
        }

        public int GetRowIndex(int insIndex)
        {
            return insIndex + 1;
        }

        public string GetCellText(DataGridViewCell cell)
        {
            if (cell == null || cell.Value == null || Convert.IsDBNull(cell.Value))
                return String.Empty;
            return cell.Value.ToString().Trim();
        }

        public void SetCurrentRow(int rowIndex)
        {
            if (dgBody.Rows.Count > rowIndex)
            {
                dgBody.CurrentCell = dgBody.Rows[rowIndex].Cells[COL_INDEX];
                dgBody.FirstDisplayedScrollingRowIndex = rowIndex > 10 ? rowIndex - 10 : 0;
            }
        }

        public void SetCurrentVariable(int varIndex)
        {
            if (dgVariable.Rows.Count > varIndex)
            {
                dgVariable.CurrentCell = dgVariable.Rows[varIndex].Cells[COL_INDEX];
                dgVariable.FirstDisplayedScrollingRowIndex = varIndex > 10 ? varIndex - 10 : 0;
            }
        }

        public void cmNop_Click(object sender, EventArgs e)
        {
            if (dgBody.SelectedRows == null || dgBody.SelectedRows.Count < 1) return;

            for (int i = 0; i < dgBody.SelectedRows.Count; i++)
            {
                int insIndex = GetInstructionIndex(dgBody.SelectedRows[i]);
                if (insIndex >= 0)
                {
                    InsUtils.ToNop(_currentMethod.Body.GetILProcessor(), _currentMethod.Body.Instructions, insIndex);
                }
            }

            InsUtils.ComputeOffsets(_currentMethod.Body.Instructions);
            InitBody(_currentMethod);
        }

        public void cmDeobfBranch_Click(object sender, EventArgs e)
        {
            if (dgBody.SelectedRows == null || dgBody.SelectedRows.Count != 1) return;
            int insIndex = GetInstructionIndex(dgBody.SelectedRows[0]);
            if (insIndex >= 0)
            {
                Instruction ins = _currentMethod.Body.Instructions[insIndex];

                Deobfuscator deobf = new Deobfuscator();
                if (_frmDeobfMethod != null)
                {
                    deobf.Options.BranchDirection = _frmDeobfMethod.BranchDirection;
                }

                if (DeobfUtils.IsDirectJumpInstruction(ins))
                {
                    deobf.DeobfFlowBranch(_currentMethod, insIndex, insIndex, 0, 0);
                }
                else if (DeobfUtils.IsConditionalJumpInstruction(ins))
                {
                    deobf.Options.chkCondBranchDownChecked = true;
                    deobf.Options.chkCondBranchUpChecked = true;
                    deobf.DeobfFlowConditionalBranch(_currentMethod, insIndex, insIndex, 0);
                }
                else if (ins.OpCode.Code == Code.Switch)
                {
                    deobf.DeobfFlowSwitch(_currentMethod, insIndex, insIndex);
                }
                InitBody(_currentMethod);
            }
        }

        public void cmMakeBranch_Click(object sender, EventArgs e)
        {
            if (dgBody.SelectedRows == null || dgBody.SelectedRows.Count != 1) return;
            int insIndex = GetInstructionIndex(dgBody.SelectedRows[0]);
            if (insIndex >= 0)
            {
                Instruction ins = _currentMethod.Body.Instructions[insIndex];
                ILProcessor ilp = _currentMethod.Body.GetILProcessor();
                Instruction newIns = Instruction.Create(OpCodes.Br, ins);
                ilp.InsertBefore(ins, newIns);
                InitBody(_currentMethod);
            }
        }

        //public void cmFollow_Click(object sender, EventArgs e)
        //{
        //    int insIndex = GetInstructionIndex(dgBody.CurrentRow);
        //    if (insIndex >= 0)
        //        BodyCellDoubleClick(insIndex, COL_OPERAND);
        //}

        public void dgBody_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == COL_OPERAND)
            {
                DataGridViewRow row = dgBody.Rows[e.RowIndex];
                string type = row.Cells[e.ColumnIndex].Value as string;
                switch (type)
                {
                    case "String":
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "UInt16":
                    case "UInt32":
                    case "UInt64":
                        e.CellStyle.ForeColor = Color.DarkRed;
                        break;
                    case "Instruction":
                    case "Instruction[]":
                        e.CellStyle.ForeColor = Color.FromArgb(0, 0, 192);
                        break;
                    case "MethodDefinition":
                        e.CellStyle.ForeColor = Color.Indigo;
                        break;
                    default:
                        break;
                }
            }
            else if (e.ColumnIndex == COL_OPCODE)
            {
                DataGridViewRow row = dgBody.Rows[e.RowIndex];
                int insIndex = GetInstructionIndex(row);
                if (insIndex >= 0)
                {
                    Instruction ins = _currentMethod.Body.Instructions[insIndex];
                    if (!InsUtils.IsValidInstruction(ins, _currentMethod))
                    {
                        e.CellStyle.ForeColor = Color.Red;
                    }
                }
            }
        }

        public void dgBody_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            DataGridViewRow row = dgBody.Rows[e.RowIndex];
            int insIndex = GetInstructionIndex(row);
            if (insIndex >= 0)
            {
                if (e.ColumnIndex == COL_OPCODE)
                {
                    DataGridViewCell c = row.Cells[e.ColumnIndex];
                    string opCode = c.Value as string;
                    e.ToolTipText = Utils.GetILHelp(opCode);
                }
                else if (e.ColumnIndex == COL_OPERAND)
                {
                    Instruction ins = _currentMethod.Body.Instructions[insIndex];
                    object op = ins.Operand;
                    if (op is FieldReference || op is FieldDefinition)
                    {
                        FieldReference fr = (FieldReference)op;
                        FieldDefinition fd = (fr is FieldDefinition) ? (FieldDefinition)fr : DeobfUtils.Resolve(fr, _form.TreeViewHandler.GetLoadedAssemblyList(), null);
                        if (fd != null && (fd.Attributes & FieldAttributes.HasFieldRVA) != 0)
                        {
                            e.ToolTipText = BytesUtils.BytesToHexStringBlock(fd.InitialValue);
                        }
                        else
                        {
                            e.ToolTipText = String.Format("{0}\r\n{1}", fr.ToString(), TokenUtils.GetFullMetadataTokenString(op is FieldDefinition ? fd.MetadataToken : fr.MetadataToken));
                        }
                    }
                    else if (op is MethodDefinition)
                    {
                        MethodDefinition md = (MethodDefinition)op;
                        e.ToolTipText = String.Format("{0}\r\n{1}", md.ToString(), TokenUtils.GetFullMetadataTokenString(md.MetadataToken));
                    }
                    else if (op is MethodReference)
                    {
                        MethodReference mr = (MethodReference)op;
                        e.ToolTipText = String.Format("{0}\r\n{1}", mr.ToString(), TokenUtils.GetFullMetadataTokenString(mr.MetadataToken));
                    }
                    else
                    {
                        DataGridViewCell c = row.Cells[e.ColumnIndex];
                        if (c.Value != null)
                        {
                            e.ToolTipText = c.Value.ToString();
                        }
                    }
                }
                else if (e.ColumnIndex == COL_OFFSET)
                {
                    MethodBody body = _currentMethod.Body;
                    Instruction ins = body.Instructions[insIndex];
                    int rva = body.CodeRVA + ins.Offset;
                    e.ToolTipText = String.Format("RVA: 0x{0:x}", rva);
                }
            }
        }

        private frmDeobfMethod _frmDeobfMethod = null;
        public void cmDeobf_Click(object sender, EventArgs e)
        {
            if (_currentMethod == null) return;

            if (_frmDeobfMethod == null)
            {
                _frmDeobfMethod = new frmDeobfMethod(_currentMethod);
            }
            else
            {
                _frmDeobfMethod.InitForm(_currentMethod);
            }

            if (_frmDeobfMethod.ShowDialog() != DialogResult.OK) return;

            bool resolveDirAdded = false;
            SimpleWaitCursor wc = new SimpleWaitCursor();
            try
            {
                resolveDirAdded = _form.Host.AddAssemblyResolveDir(_form.SourceDir);

                AssemblyDefinition ad = _currentMethod.DeclaringType.Module.Assembly;
                string file = null;
                foreach (TreeNode n in _form.TreeView.Nodes)
                {
                    if (ad.Equals(n.Tag))
                    {
                        file = _form.TreeViewHandler.GetTreeNodeText(n);
                        break;
                    }
                }

                if (file == null)
                    return;
                
                file = Path.Combine(_form.SourceDir, file);
                if (String.IsNullOrEmpty(file) || !File.Exists(file))
                {
                    throw new ApplicationException("Cannot find assembly file: " + file);
                }
                

                DeobfOptions options = new DeobfOptions();
                options.Clear();

                options.Host = _form.Host;
                options.Rows = new string[] { file };
                options.SourceDir = _form.SourceDir;
                options.OutputDir = _form.SourceDir;
                options.BranchDirection = _frmDeobfMethod.BranchDirection;

                options.LoopCount = Config.DeobfFlowOptionBranchLoopCount;
                options.MaxRefCount = Config.DeobfFlowOptionMaxRefCount;
                options.MaxMoveCount = Config.DeobfFlowOptionMaxMoveCount;

                options.chkPatternChecked = _frmDeobfMethod.AutoFlowPattern;
                options.chkBranchChecked = _frmDeobfMethod.AutoFlowBranch;
                options.chkCondBranchDownChecked = _frmDeobfMethod.AutoFlowConditionalBranchDown;
                options.chkCondBranchUpChecked = _frmDeobfMethod.AutoFlowConditionalBranchUp;
                
                options.chkSwitchChecked = _frmDeobfMethod.AutoFlowSwitch;
                options.chkUnreachableChecked = _frmDeobfMethod.AutoFlowUnreachable;
                options.chkRemoveExceptionHandlerChecked = _frmDeobfMethod.AutoFlowExceptionHandler;
                options.chkBoolFunctionChecked = _frmDeobfMethod.AutoFlowBoolFunction;
                options.chkDelegateCallChecked = _frmDeobfMethod.AutoFlowDelegateCall;
                options.chkDirectCallChecked = _frmDeobfMethod.AutoFlowDirectCall;
                options.chkBlockMoveChecked = _frmDeobfMethod.AutoFlowBlockMove;
                options.chkReflectorFixChecked = _frmDeobfMethod.AutoFlowReflectorFix;
                options.chkRemoveInvalidInstructionChecked = _frmDeobfMethod.AutoFlowRemoveInvalidInstruction;

                options.chkAutoStringChecked = _frmDeobfMethod.AutoString || _frmDeobfMethod.SelectedMethod != null;
                options.StringOptionSearchForMethod = _frmDeobfMethod.SelectedMethod;

                options.PluginList = _frmDeobfMethod.PluginList;
                options.chkInitLocalVarsChecked = _frmDeobfMethod.InitLocalVars;

                Deobfuscator deobf = new Deobfuscator(options);
                int deobfCount = deobf.DeobfMethod(file, _currentMethod);
                
                if (deobfCount > 0)
                {
                    InitBody(_currentMethod);
                }

                //need to be after InitBody
                _form.SetStatusCount(deobfCount);

            }
            catch (Exception ex)
            {
                SimpleMessage.ShowException(ex);
                InitBody(_currentMethod);
            }
            finally
            {
                if (resolveDirAdded)
                    _form.Host.RemoveAssemblyResolveDir(_form.SourceDir);
                wc.Dispose();
            }
        }

        private bool CallEditInstruction(frmClassEditInstruction.EditModes editMode)
        {
            int insIndex = GetInstructionIndex(dgBody.CurrentRow);
            if (insIndex >= 0)
            {
                frmClassEditInstruction f = new frmClassEditInstruction(this._form, _currentMethod, insIndex, editMode);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    InsUtils.ComputeOffsets(_currentMethod.Body.Instructions);
                    InitBody(_currentMethod);
                }
                return true;
            }
            return false;
        }

        public void cmEdit_Click(object sender, EventArgs e)
        {
            if (CallEditInstruction(frmClassEditInstruction.EditModes.Edit))
                return;

            int ehIndex = GetExceptionHandlerIndex(dgBody.CurrentRow);
            if (ehIndex >= 0)
            {
                frmClassEditExceptionHandler f = new frmClassEditExceptionHandler(_currentMethod, ehIndex);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    //InsUtils.ComputeOffsets(_currentMethod.Body.Instructions);
                    InitBody(_currentMethod);
                }
                return;
            }
        }

        public void cmInsertAfter_Click(object sender, EventArgs e)
        {
            CallEditInstruction(frmClassEditInstruction.EditModes.InsertAfter);
        }

        public void cmInsertBefore_Click(object sender, EventArgs e)
        {
            CallEditInstruction(frmClassEditInstruction.EditModes.InsertBefore);
        }

        public void cmNewExceptionHandler_Click(object sender, EventArgs e)
        {
            frmClassEditExceptionHandler f = new frmClassEditExceptionHandler(_currentMethod, -1);
            if (f.ShowDialog() == DialogResult.OK)
            {
                InitBody(_currentMethod);
            }
        }

        public void cmRemove_Click(object sender, EventArgs e)
        {
            if (dgBody.SelectedRows == null || dgBody.SelectedRows.Count < 1) return;

            object[] saved = new object[dgBody.SelectedRows.Count];
            for (int i = 0; i < dgBody.SelectedRows.Count; i++)
            {
                int insIndex = GetInstructionIndex(dgBody.SelectedRows[i]);
                if (insIndex >= 0)
                {
                    saved[i] = _currentMethod.Body.Instructions[insIndex];
                }
                else
                {
                    int ehIndex = GetExceptionHandlerIndex(dgBody.SelectedRows[i]);
                    if (ehIndex >= 0)
                    {
                        saved[i] = _currentMethod.Body.ExceptionHandlers[ehIndex];
                    }
                }

            }

            ILProcessor ilp = _currentMethod.Body.GetILProcessor();
            Collection<ExceptionHandler> ehc = _currentMethod.Body.ExceptionHandlers;
            foreach (object o in saved)
            {
                if (o != null)
                {
                    if (o is Instruction)
                    {                        
                        ilp.Remove((Instruction)o);
                    }
                    else if (o is ExceptionHandler)
                    {
                        ehc.Remove((ExceptionHandler)o);
                    }
                }
            }

            InsUtils.ComputeOffsets(_currentMethod.Body.Instructions);
            InitBody(_currentMethod);
        }

        public void cmDuplicate_Click(object sender, EventArgs e)
        {
            int insIndex = GetInstructionIndex(dgBody.CurrentRow);
            if (insIndex >= 0)
            {
                Instruction ins = _currentMethod.Body.Instructions[insIndex];
                Instruction newIns = InsUtils.CreateInstruction(ins.OpCode, ins.Operand);
                _currentMethod.Body.GetILProcessor().InsertAfter(ins, newIns);

                InsUtils.ComputeOffsets(_currentMethod.Body.Instructions);
                InitBody(_currentMethod);
            }
        }

        public void cmMove_Click(object sender, EventArgs e)
        {
            if (dgBody.SelectedRows == null || dgBody.SelectedRows.Count < 1) return;

            int start = GetInstructionIndex(dgBody.SelectedRows[0]);
            int end = GetInstructionIndex(dgBody.SelectedRows[dgBody.SelectedRows.Count - 1]);
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            if (end - start + 1 != dgBody.SelectedRows.Count)
            {
                SimpleMessage.ShowError("Continuous instructions required.");
                return;
            }

            frmClassEditMoveInstructions f = new frmClassEditMoveInstructions(_currentMethod, start, end);
            if (f.ShowDialog() == DialogResult.OK)
            {
                InsUtils.Simplify(_currentMethod.Body.Instructions);
                //InsUtils.ComputeOffsets(_currentMethod.Body.Instructions);
                InitBody(_currentMethod);

                SetCurrentRow(f.DisplayStartIndex);
                int count = f.DisplayStartIndex + f.DisplayLength;
                for (int i = f.DisplayStartIndex; i < count; i++)
                {
                    dgBody.Rows[i].Selected = true;
                }
            }

        }

        public void cmHighlightOpCode_Click(object sender, EventArgs e)
        {
            int insIndex = GetInstructionIndex(dgBody.CurrentRow);
            if (insIndex >= 0)
            {
                Collection<Instruction> ic = _currentMethod.Body.Instructions;
                Instruction ins = ic[insIndex];
                OpCode oc = ins.OpCode;
                string name = ins.OpCode.Name;
                int count = 0;
                foreach (DataGridViewRow row in dgBody.Rows)
                {
                    insIndex = GetInstructionIndex(row);
                    if (insIndex >= 0)
                    {
                        if (oc == ic[insIndex].OpCode || ic[insIndex].OpCode.Name == name)
                        {
                            row.Selected = true;
                            count++;
                        }
                    }
                }
                if (count < 1)
                    SimpleMessage.ShowInfo("No OpCode found.");

            }
        }

        public void cmHighlightReference_Click(object sender, EventArgs e)
        {
            int insIndex = GetInstructionIndex(dgBody.CurrentRow);
            if (insIndex >= 0)
            {
                List<int> list = DeobfUtils.FindReferences(_currentMethod, insIndex);
                int count = 0;
                if (list != null && list.Count > 0)
                {
                    foreach (DataGridViewRow row in dgBody.Rows)
                    {
                        int index = GetInstructionIndex(row);
                        if (index >= 0 && list.Contains(index))
                        {
                            row.Selected = true;
                            count++;
                        }
                    }
                }
                if (count == 0)
                {
                    SimpleMessage.ShowInfo("No Reference found.");
                }
            }
        }

        public void cmSaveDetailsAs_Click(object sender, EventArgs e)
        {
            string initDir = Config.ClassEditorSaveAsDir;
            string defaultFileName = String.Format("{0}{1}{2}.txt",
                _currentMethod.DeclaringType == null ? "method" : _currentMethod.DeclaringType.Name,
                _currentMethod.Name.StartsWith(".") ? "" : ".",
                _currentMethod.Name);

            defaultFileName = PathUtils.FixFileName(defaultFileName);
            if (defaultFileName.Length > 250)
            {
                defaultFileName = String.Format("{0}.{1:x08}",
                    _currentMethod.DeclaringType == null ? "method" : _currentMethod.DeclaringType.Name,
                    _currentMethod.MetadataToken.ToUInt32());
                defaultFileName = PathUtils.FixFileName(defaultFileName);
            }

            if (!Directory.Exists(initDir))
                initDir = Environment.CurrentDirectory;
            string path = SimpleDialog.OpenFile("Save Instructions",
                "Text files (*.txt)|*.txt",
                ".txt", false, initDir, defaultFileName);
            if (!String.IsNullOrEmpty(path))
            {
                Config.ClassEditorSaveAsDir = Path.GetDirectoryName(path);
            }

            if (String.IsNullOrEmpty(path)) return;

            try
            {
                //Clipboard.Clear();
                //dgBody.SuspendLayout();
                //dgBody.SelectAll();
                //DataObject dataObj = dgBody.GetClipboardContent();
                //Clipboard.SetDataObject(dataObj, true);
                //dgBody.ClearSelection();

                //string data = Clipboard.GetText();
                //using (StreamWriter sw = File.CreateText(path))
                //{
                //    sw.WriteLine(data);
                //}                

                using (StreamWriter sw = File.CreateText(path))
                {
                    DataGridViewRowCollection rows = dgBody.Rows;
                    for (int i = 0; i < rows.Count; i++)
                    {
                        string line = String.Format("{0,-8} {1,-12} {2,-12} {3}",
                            GetCellText(rows[i].Cells[COL_INDEX]),
                            GetCellText(rows[i].Cells[COL_OFFSET]),
                            GetCellText(rows[i].Cells[COL_OPCODE]),
                            GetCellText(rows[i].Cells[COL_OPERAND])
                            );
                        sw.WriteLine(line);
                    }
                }
                _form.SetStatusText(String.Format("{0} saved.", path));
            }
            catch
            {
                throw;
            }
            finally
            {
                dgBody.ResumeLayout();
            }
        }

        SortedList<int, Instruction> _copiedInstructions;

        public void cmCopy_Click(object sender, EventArgs e)
        {
            if (dgBody.SelectedRows == null || dgBody.SelectedRows.Count < 1) return;

            _copiedInstructions = new SortedList<int, Instruction>(dgBody.SelectedRows.Count);
            for (int i = 0; i < dgBody.SelectedRows.Count; i++)
            {
                int insIndex = GetInstructionIndex(dgBody.SelectedRows[i]);
                if (insIndex >= 0)
                {
                    _copiedInstructions.Add(insIndex, _currentMethod.Body.Instructions[insIndex]);
                }
            }
        }

        public void cmPaste_Click(object sender, EventArgs e)
        {
            int insIndex = GetInstructionIndex(dgBody.CurrentRow);
            if (insIndex < 0) return;

            Collection<Instruction> ic = _currentMethod.Body.Instructions;
            Instruction insCurrent = ic[insIndex];
            ILProcessor ilp = _currentMethod.Body.GetILProcessor();
            foreach (KeyValuePair<int, Instruction> kvp in _copiedInstructions)
            {
                Instruction ins = kvp.Value;
                Instruction insNew = InsUtils.CreateInstruction(ins.OpCode, ins.Operand);
                ilp.InsertAfter(insCurrent, insNew);
                insCurrent = insNew;
            }
            InsUtils.ComputeOffsets(ic);
            InitBody(_currentMethod);
            SetCurrentRow(insIndex + 1 + 1);
        }

        private void InitHighlightContextMenu(ToolStripMenuItem highlight)
        {
            //if associate context menu to grid cell, static second level menu won't be shown??

            if (highlight == null || highlight.DropDownItems.Count == 2)
                return;

            highlight.DropDownItems.Clear();

            ToolStripMenuItem mi = new ToolStripMenuItem();
            mi.Name = "cmHighlightOpCode";
            mi.Text = "OpCode";
            mi.Click += new EventHandler(cmHighlightOpCode_Click);
            highlight.DropDownItems.Add(mi);

            mi = new ToolStripMenuItem();
            mi.Name = "cmHighlightReference";
            mi.Text = "Reference";
            mi.Click += new EventHandler(cmHighlightReference_Click);
            highlight.DropDownItems.Add(mi);
        }

        public void cmOp_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip cms = sender as ContextMenuStrip;

            if (dgBody.CurrentRow == null)
            {
                e.Cancel = true;
                return;
            }

            InitHighlightContextMenu(cms.Items["cmHighlight"] as ToolStripMenuItem);

            // mark block
            ((ToolStripMenuItem)cms.Items["cmMarkBlocks"]).Checked = _markBlocks;

            int insIndex = GetInstructionIndex(dgBody.CurrentRow);
            int ehIndex = GetExceptionHandlerIndex(dgBody.CurrentRow);

            // mutiple instructions operation
            if ((insIndex >= 0 || ehIndex >= 0) && dgBody.SelectedRows.Count > 0)
            {
                cms.Items["cmRemove"].Enabled = true;
            }
            else
            {
                cms.Items["cmRemove"].Enabled = false;
            }

            if (insIndex >= 0 && dgBody.SelectedRows.Count > 0)
            {
                cms.Items["cmNop"].Enabled = true;
                cms.Items["cmCopy"].Enabled = true;
                cms.Items["cmMove"].Enabled = true;
            }
            else
            {
                cms.Items["cmNop"].Enabled = false;
                cms.Items["cmCopy"].Enabled = false;
                cms.Items["cmMove"].Enabled = false;
            }

            // paste
            if (insIndex >= 0 && dgBody.SelectedRows.Count == 1 &&
                _copiedInstructions != null && _copiedInstructions.Count > 0)
            {
                cms.Items["cmPaste"].Enabled = true;
            }
            else
            {
                cms.Items["cmPaste"].Enabled = false;
            }

            // single instruction operation
            if ((insIndex >= 0 || ehIndex >= 0) && _form.SaveAssemblyButton.Visible && dgBody.SelectedRows.Count == 1)
            {
                cms.Items["cmEdit"].Enabled = true;
            }
            else
            {
                cms.Items["cmEdit"].Enabled = false;
            }

            if (insIndex >= 0 && _form.SaveAssemblyButton.Visible && dgBody.SelectedRows.Count == 1)
            {
                cms.Items["cmInsertBefore"].Enabled = true;
                cms.Items["cmInsertAfter"].Enabled = true;
                cms.Items["cmDuplicate"].Enabled = true;
                cms.Items["cmHighlight"].Enabled = true;
                cms.Items["cmMakeBranch"].Enabled = true;

                Instruction ins = _currentMethod.Body.Instructions[insIndex];
                if (DeobfUtils.IsDirectJumpInstruction(ins) ||
                    DeobfUtils.IsConditionalJumpInstruction(ins) ||
                    ins.OpCode.Code == Code.Switch)
                {
                    cms.Items["cmDeobfBranch"].Enabled = true;
                }
                else
                {
                    cms.Items["cmDeobfBranch"].Enabled = false;
                }

            }
            else
            {
                cms.Items["cmInsertBefore"].Enabled = false;
                cms.Items["cmInsertAfter"].Enabled = false;
                cms.Items["cmDuplicate"].Enabled = false;
                cms.Items["cmHighlight"].Enabled = false;
                cms.Items["cmDeobfBranch"].Enabled = false;
                cms.Items["cmMakeBranch"].Enabled = false;
            }
        }

     

    } // end of class
}
