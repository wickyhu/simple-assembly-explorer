using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;
using Mono.Cecil;

namespace SimpleAssemblyExplorer
{
    public class ClassEditBinaryViewHandler
    {
        frmClassEdit _form;
        HexBox _hexBox;
        ContextMenuStrip _contextMenuStrip;

        ToolStripMenuItem _cutToolStripMenuItem;
        ToolStripMenuItem _copyToolStripMenuItem;
        ToolStripMenuItem _pasteToolStripMenuItem;
        ToolStripMenuItem _selectAllToolStripMenuItem;

        ToolStripMenuItem _commitToolStripMenuItem;
        ToolStripMenuItem _rollbackToolStripMenuItem;

        public ClassEditBinaryViewHandler(frmClassEdit form)
        {
            _form = form;
            _hexBox = form.ResourceBinary;
            
            _hexBox.ByteProviderChanged += _hexBox_ByteProviderChanged;
            //seems the context menu shortcut not always working if the context menu never shown?
            _hexBox.KeyUp += _hexBox_KeyUp; 
        }

        void _hexBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (_hexBox.ReadOnly) return;
            if (e.Control)
            {
                if (e.KeyCode == Keys.S)
                {
                    CommitMenuItem_Click(_hexBox, null);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.R)
                {
                    RollbackMenuItem_Click(_hexBox, null);
                    e.Handled = true;
                }
            }
        }

        void _hexBox_ByteProviderChanged(object sender, EventArgs e)
        {
            CheckBuiltInContextMenu();
        }

        void CheckBuiltInContextMenu()
        {
            if (this._contextMenuStrip == null)
            {
                ContextMenuStrip cms = new ContextMenuStrip();
                
                _cutToolStripMenuItem = new ToolStripMenuItem("Cut", null, new EventHandler(CutMenuItem_Click));
                cms.Items.Add(_cutToolStripMenuItem);

                _copyToolStripMenuItem = new ToolStripMenuItem("Copy", null, new EventHandler(CopyMenuItem_Click));
                cms.Items.Add(_copyToolStripMenuItem);

                _pasteToolStripMenuItem = new ToolStripMenuItem("Paste", null, new EventHandler(PasteMenuItem_Click));
                cms.Items.Add(_pasteToolStripMenuItem);

                cms.Items.Add(new ToolStripSeparator());

                _selectAllToolStripMenuItem = new ToolStripMenuItem("Select All", null, new EventHandler(SelectAllMenuItem_Click));
                cms.Items.Add(_selectAllToolStripMenuItem);

                cms.Items.Add(new ToolStripSeparator());

                _commitToolStripMenuItem = new ToolStripMenuItem("Commit", null, new EventHandler(CommitMenuItem_Click), Keys.Control | Keys.S);
                cms.Items.Add(_commitToolStripMenuItem);

                _rollbackToolStripMenuItem = new ToolStripMenuItem("Rollback", null, new EventHandler(RollbackMenuItem_Click), Keys.Control | Keys.R);
                cms.Items.Add(_rollbackToolStripMenuItem);

                cms.Opening += new CancelEventHandler(ClassEditBinaryViewContextMenu_Opening);

                _contextMenuStrip = cms;
            }

            this._hexBox.ReadOnly = true;
            if (this._hexBox.ByteProvider == null)
            {
                this._hexBox.ContextMenuStrip = null;
            }
            else
            {
                this._hexBox.ContextMenuStrip = _contextMenuStrip;
                this._hexBox.ReadOnly = false;
            }

            if (!_hexBox.ReadOnly)
            {
                _form.SetStatusText("CTRL+S to commit, CTRL+R to rollback.");
            }
        }

        void ClassEditBinaryViewContextMenu_Opening(object sender, CancelEventArgs e)
        {
            _cutToolStripMenuItem.Enabled = this._hexBox.CanCut();
            _copyToolStripMenuItem.Enabled = this._hexBox.CanCopy();
            _pasteToolStripMenuItem.Enabled = this._hexBox.CanPaste();
            _selectAllToolStripMenuItem.Enabled = this._hexBox.CanSelectAll();

            var dbp = (DynamicByteProvider)this._hexBox.ByteProvider;
            _commitToolStripMenuItem.Enabled = !this._hexBox.ReadOnly && dbp.HasChanges();
            _rollbackToolStripMenuItem.Enabled = _commitToolStripMenuItem.Enabled;
        }

        void CutMenuItem_Click(object sender, EventArgs e) { this._hexBox.Cut(); }
        void CopyMenuItem_Click(object sender, EventArgs e) { this._hexBox.Copy(); }
        void PasteMenuItem_Click(object sender, EventArgs e) { this._hexBox.Paste(); }
        void SelectAllMenuItem_Click(object sender, EventArgs e) { this._hexBox.SelectAll(); }

        void CommitMenuItem_Click(object sender, EventArgs e)
        {
            var n = _form.TreeViewHandler.SelectedNode;
            var r = (Resource)n.Tag;
            if (r is EmbeddedResource)
            {
                var er = (EmbeddedResource)r;
                var detailType = (SimpleAssemblyExplorer.ClassEditResourceHandler.DetailTypes)_form.DetailsTabPage.Tag;

                var dbp = (DynamicByteProvider)_hexBox.ByteProvider;
                dbp.ApplyChanges();
                var data = dbp.Bytes.ToArray();

                if (detailType == ClassEditResourceHandler.DetailTypes.BinaryResource)
                {
                    var newEr = new EmbeddedResource(er.Name, er.Attributes, data);
                    var ad = _form.TreeViewHandler.GetCurrentAssembly();
                    ad.MainModule.Resources.Remove(er);
                    ad.MainModule.Resources.Add(newEr);
                    n.Tag = newEr;
                }
                else if (detailType == ClassEditResourceHandler.DetailTypes.ResourcesRowAsBinary)
                {
                    var row = _form.ResourceDataGrid.CurrentRow;
                    string key = _form.ResourceHandler.GetNameCellValue(row);
                    var newEr = _form.ResourceHandler.ReplaceResource(er, key, data);

                    var ad = _form.TreeViewHandler.GetCurrentAssembly();
                    ad.MainModule.Resources.Remove(er);
                    ad.MainModule.Resources.Add(newEr);
                    n.Tag = newEr;

                    var rowIndex = row.Index;
                    _form.ResourceHandler.InitResource(newEr);

                    row = _form.ResourceDataGrid.Rows[rowIndex];
                    _form.ResourceDataGrid.CurrentCell = row.Cells[0];
                    row.Selected = true;
                    _form.ResourceHandler.ShowBinary(row);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            _form.SetStatusText("Changes committed.");
        }
        
        void RollbackMenuItem_Click(object sender, EventArgs e)
        {
            var n = _form.TreeViewHandler.SelectedNode;
            var r = (Resource)n.Tag;
            if (r is EmbeddedResource)
            {
                var er = (EmbeddedResource)r;
                var detailType = (SimpleAssemblyExplorer.ClassEditResourceHandler.DetailTypes)_form.DetailsTabPage.Tag;
                if (detailType == ClassEditResourceHandler.DetailTypes.BinaryResource)
                {
                    _form.ResourceHandler.ShowBinary(er, false);
                }
                else if (detailType == ClassEditResourceHandler.DetailTypes.ResourcesRowAsBinary)
                {
                    var row = _form.ResourceDataGrid.CurrentRow;
                    _form.ResourceHandler.ShowBinary(row);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            _form.SetStatusText("Changes rollbacked.");
        }

    }
}
