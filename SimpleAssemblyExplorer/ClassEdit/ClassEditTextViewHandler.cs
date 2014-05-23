using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class ClassEditTextViewHandler
    {
        frmClassEdit _form;
        TextBox _textbox;

        public ClassEditTextViewHandler(frmClassEdit form)
        {
            _form = form;
            _textbox = form.ResourceText;
            _textbox.KeyUp += _textbox_KeyUp;
        }

        void _textbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (_textbox.ReadOnly)
                return;

            if (e.Control)
            {
                if (e.KeyCode == Keys.S)
                {
                    Commit();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.R)
                {
                    Rollback();
                    e.Handled = true;
                }
            }
        }

        void Commit()
        {
            var n = _form.TreeViewHandler.SelectedNode;
            var r = (Resource)n.Tag;
            if (r is EmbeddedResource)
            {
                var er = (EmbeddedResource)r;
                var detailType = (SimpleAssemblyExplorer.ClassEditResourceHandler.DetailTypes)_form.DetailsTabPage.Tag;
                var data = Encoding.Unicode.GetBytes(_textbox.Text);

                if (detailType == ClassEditResourceHandler.DetailTypes.TextResource)
                {
                    var newEr = new EmbeddedResource(er.Name, er.Attributes, data);
                    var ad = _form.TreeViewHandler.GetCurrentAssembly();
                    ad.MainModule.Resources.Remove(er);
                    ad.MainModule.Resources.Add(newEr);
                    n.Tag = newEr;
                }
                else if (detailType == ClassEditResourceHandler.DetailTypes.ResourcesRowAsText)
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
                    _form.ResourceDataGrid.FirstDisplayedScrollingRowIndex = rowIndex > 10 ? rowIndex - 10 : 0;

                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            _form.SetStatusText("Changes committed.");
        }

        void Rollback()
        {
            var n = _form.TreeViewHandler.SelectedNode;
            var r = (Resource)n.Tag;
            if (r is EmbeddedResource)
            {
                var er = (EmbeddedResource)r;
                var detailType = (SimpleAssemblyExplorer.ClassEditResourceHandler.DetailTypes)_form.DetailsTabPage.Tag;
                if (detailType == ClassEditResourceHandler.DetailTypes.TextResource)
                {
                    _form.ResourceHandler.ShowText(er.GetResourceStream(), null, false);
                }
                else if (detailType == ClassEditResourceHandler.DetailTypes.ResourcesRowAsText)
                {
                    var row = _form.ResourceDataGrid.CurrentRow;
                    var cell = _form.ResourceHandler.GetValueCell(row);
                    _form.ResourceHandler.ShowText(cell.Value == null ? String.Empty : cell.Value.ToString(), row);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            _form.SetStatusText("Changes rollbacked.");
        }

    }//end of class
}
