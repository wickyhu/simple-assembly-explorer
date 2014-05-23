using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
using System.Resources;
using System.Reflection;

namespace SimpleAssemblyExplorer
{
    public class ClassEditResourceHandler
    {
        public enum BamlTranslators
        {
            ReflectorBamlViewer,
            ILSpyBamlDecompiler
        }

        public enum DetailTypes
        {
            None,
            MethodBody,
            //Resources,
            ResourcesRowAsBinary,
            ResourcesRowAsImageList,
            ResourcesRowAsImage,
            ResourcesRowAsText,
            ImageResource,
            TextResource,
            BinaryResource
        }

        frmClassEdit _form;
        TextBox txtResource;
        PictureBox pbResource;
        DataGridView dgBody;
        DataGridView dgResource;
        Be.Windows.Forms.HexBox hbResource;
        ListView lvResource;
        Panel panelResource;
        TabPage detailsTabPage;

        private DataTable _dtResource = null;

        public ClassEditResourceHandler(frmClassEdit form)
        {
            _form = form;

            detailsTabPage = _form.DetailsTabPage;

            txtResource = _form.ResourceText;
            pbResource = _form.ResourceImage;
            hbResource = _form.ResourceBinary;
            lvResource = _form.ResourceListView;
            panelResource = _form.ResourcePanel;

            dgBody = _form.BodyDataGrid;
            dgResource = _form.ResourceDataGrid;

            CreateDataTable();
        }

        private void CreateDataTable()
        {
            _dtResource = new DataTable("Resource");
            _dtResource.Columns.Add("no", typeof(int));
            _dtResource.Columns.Add("name", typeof(String));
            _dtResource.Columns.Add("value", typeof(DataGridViewTextAndImageCellValue));
            _dtResource.Columns.Add("type", typeof(String));

        }

        public void ShowDetailsControl(DetailTypes type)
        {
            try
            {
                detailsTabPage.SuspendLayout();
                bool switchTab = true;

                switch (type)
                {
                    case DetailTypes.MethodBody:
                        dgBody.Visible = true;
                        dgResource.Visible = false;
                        panelResource.Visible = false;
                        switchTab = false;
                        break;

                    //case DetailTypes.Resources:
                    case DetailTypes.ResourcesRowAsBinary:
                    case DetailTypes.ResourcesRowAsText:
                    case DetailTypes.ResourcesRowAsImageList:
                    case DetailTypes.ResourcesRowAsImage:
                        if (panelResource.Dock == DockStyle.Fill)
                        {
                            panelResource.Dock = DockStyle.Bottom;
                            panelResource.Height = detailsTabPage.Height / 4;
                        }
                        dgBody.Visible = false;
                        dgResource.Visible = true;
                        panelResource.Visible = true;
                        panelResource.AutoScroll = false;
                        switch (type)
                        {
                            //case DetailTypes.Resources:
                            case DetailTypes.ResourcesRowAsText:
                                txtResource.Visible = true;                                
                                hbResource.Visible = false;
                                pbResource.Visible = false;
                                lvResource.Visible = false;
                                break;
                            case DetailTypes.ResourcesRowAsBinary:
                                txtResource.Visible = false;
                                hbResource.Visible = true;
                                pbResource.Visible = false;
                                lvResource.Visible = false;
                                break;
                            case DetailTypes.ResourcesRowAsImage:
                                txtResource.Visible = false;
                                hbResource.Visible = false;
                                pbResource.Visible = true;
                                panelResource.AutoScroll = true;
                                if (pbResource.Image != null)
                                {
                                    pbResource.Dock = DockStyle.None;
                                    pbResource.Size = pbResource.Image.Size;
                                }
                                else
                                {
                                    pbResource.Dock = DockStyle.Fill;
                                }
                                lvResource.Visible = false;
                                break;
                            case DetailTypes.ResourcesRowAsImageList:
                                txtResource.Visible = false;
                                hbResource.Visible = false;
                                pbResource.Visible = false;
                                lvResource.Visible = true;
                                break;
                        }
                        break;

                    case DetailTypes.TextResource:
                    case DetailTypes.ImageResource:
                    case DetailTypes.BinaryResource:
                        if (panelResource.Dock == DockStyle.Bottom)
                        {
                            panelResource.Dock = DockStyle.Fill;
                        }
                        dgBody.Visible = false;
                        dgResource.Visible = false;
                        panelResource.Visible = true;
                        panelResource.AutoScroll = false;
                        switch (type)
                        {
                            case DetailTypes.TextResource:
                                txtResource.Visible = true;
                                pbResource.Visible = false;
                                hbResource.Visible = false;
                                lvResource.Visible = false;
                                break;
                            case DetailTypes.ImageResource:
                                panelResource.AutoScroll = true;
                                txtResource.Visible = false;
                                pbResource.Visible = true;
                                if (pbResource.Image != null)
                                {
                                    pbResource.Dock = DockStyle.None;
                                    pbResource.Size = pbResource.Image.Size;
                                }
                                else
                                {
                                    pbResource.Dock = DockStyle.Fill;
                                }
                                hbResource.Visible = false;
                                lvResource.Visible = false;
                                break;
                            case DetailTypes.BinaryResource:
                                txtResource.Visible = false;
                                pbResource.Visible = false;
                                hbResource.Visible = true;
                                lvResource.Visible = false;
                                break;
                        }
                        break;

                    default:
                        dgBody.Visible = false;
                        dgResource.Visible = false;
                        panelResource.Visible = false;
                        break;
                }

                if (switchTab && _form.TabControl.SelectedTab != detailsTabPage)
                {
                    _form.TabControl.SelectedTab = detailsTabPage;
                }

                detailsTabPage.Tag = type;
            }
            catch
            {
                throw;
            }
            finally
            {
                detailsTabPage.ResumeLayout();
            }
        }

        public Image ConvertToImage(string name, object value)
        {
            string typeName = (value == null ? "System.String" : value.GetType().FullName);
            Image image = null;

            if (typeName.EndsWith("Stream"))
            {
                Stream s = value as Stream;
                if (s != null)
                {
                    if (s.CanSeek) s.Seek(0, SeekOrigin.Begin);
                    if (PathUtils.IsIconExt(name))
                    {
                        Icon ico = new Icon(s);
                        image = ico.ToBitmap();
                    }
                    else if (PathUtils.IsCursorExt(name))
                    {
                        Cursor c = LoadCursor(s);
                        image = ConvertToBitmap(c);
                    }
                    else if (ResourceFile.Default.IsImageResource(name))
                    {
                        try
                        {
                            Bitmap bmp = new Bitmap(s);
                            image = bmp;
                            //if (!ImageAnimator.CanAnimate(bmp))
                            //{
                            //    image = bmp;
                            //}
                            //else
                            //{
                            //    image = new Bitmap(bmp.Width, bmp.Height);
                            //}
                        }
                        catch (ArgumentException) { }
                    }
                }
            }
            else
            {
                switch (typeName)
                {
                    case "System.Drawing.Bitmap":
                        image = (Bitmap)value;
                        break;
                    case "System.Drawing.Icon":
                        image = ((Icon)value).ToBitmap();
                        break;
                    case "System.Drawing.Image":
                        image = (Image)value;
                        break;
                    case "System.Windows.Forms.Cursor":
                        image = ConvertToBitmap((Cursor)value);
                        break;
                }
            }

            return image;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadCursorFromFile(string path);

        public static Cursor LoadCursor(string path)
        {
            IntPtr hCurs = LoadCursorFromFile(path);
            if (hCurs == IntPtr.Zero)
            {                
                throw new Win32Exception("Failed to load cursor.");
            }
            var curs = new Cursor(hCurs);
            // Note: force the cursor to own the handle so it gets released properly
            var fi = typeof(Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            fi.SetValue(curs, true);
            return curs;
        }

        public Cursor LoadCursor(Stream s)
        {
            if (s.CanSeek)
            {
                s.Seek(0, SeekOrigin.Begin);
            }
            byte[] bytes = new byte[s.Length];
            s.Read(bytes, 0, bytes.Length);

            string tmpFile = Path.GetTempFileName();
            using (FileStream fs = new FileStream(tmpFile, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
            }

            var c = LoadCursor(tmpFile);
            File.Delete(tmpFile);
            return c;
        }

        public Bitmap ConvertToBitmap(Cursor c)
        {
            //it seems there is no easy way to write cursor to file
            Bitmap bmp = new Bitmap(c.Size.Width, c.Size.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                c.Draw(g, new Rectangle(0, 0, c.Size.Width, c.Size.Height));
            }
            return bmp;
        }

        private bool InitResourceGridRows(EmbeddedResource er)
        {
            ResourceReader rr;
            try
            {
                rr = new ResourceReader(er.GetResourceStream());
            }
            catch
            {
                rr = null;
            }

            if (rr == null)
                return false;

            DataRow dr;
            IDictionaryEnumerator de = rr.GetEnumerator();
            int count = 0;
            while (de.MoveNext())
            {
                dr = _dtResource.NewRow();
                string name = de.Key as string;
                dr["no"] = count.ToString();
                count++;
                dr["name"] = name;
                object value = de.Value;
                dr["type"] = value == null ? null : value.GetType().FullName;
                try
                {
                    DataGridViewTextAndImageCellValue cv = new DataGridViewTextAndImageCellValue(name, value);
                    cv.Image = ConvertToImage(name, value);
                    dr["value"] = cv;
                }
                catch //(Exception ex)
                {
                    dr["value"] = new DataGridViewTextAndImageCellValue(String.Empty, value);
                    //dr["value"] = new DataGridViewTextAndImageCellValue(String.Empty, String.Format("Error: {0}", ex.Message));
                    //dr["type"] = "System.String";
                }
                _dtResource.Rows.Add(dr);
            }

            rr.Close();
            return true;
        }

        public EmbeddedResource RemoveResource(EmbeddedResource er, string[] keys)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                var rr = new ResourceReader(er.GetResourceStream());
                var ms = new MemoryStream();
                var rw = new ResourceWriter(ms);

                var de = rr.GetEnumerator();
                while (de.MoveNext())
                {
                    string deKey = de.Key as string;
                    bool toBeDeleted = false;
                    foreach (string key in keys)
                    {
                        if (key == deKey)
                        {
                            toBeDeleted = true;
                            break;
                        }
                    }
                    if (toBeDeleted) continue;
                    rw.AddResource(deKey, de.Value);
                }

                rw.Generate();
                rw.Close();
                rr.Close();
                var newEr = new EmbeddedResource(er.Name, er.Attributes, ms.ToArray());
                ms.Close();
                return newEr;
            }
            catch
            {
                throw;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public EmbeddedResource ReplaceResource(EmbeddedResource er, string key, byte[] data)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                var rr = new ResourceReader(er.GetResourceStream());
                var ms = new MemoryStream();
                var rw = new ResourceWriter(ms);

                var de = rr.GetEnumerator();
                while (de.MoveNext())
                {
                    string deKey = de.Key as string;
                    if (key == deKey)
                    {
                        object o = RestoreResourceObject(key, de.Value.GetType().FullName, data);
                        rw.AddResource(key, o);
                    }
                    else
                    {
                        rw.AddResource(deKey, de.Value);
                    }
                }

                rw.Generate();
                rw.Close();
                rr.Close();
                var newEr = new EmbeddedResource(er.Name, er.Attributes, ms.ToArray());
                ms.Close();
                return newEr;
            }
            catch
            {
                throw;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public object LoadResourceObject(string path)
        {
            object o = null;
            if (!File.Exists(path))
                return o;

            string name = Path.GetFileName(path);
            if (ResourceFile.Default.IsImageResource(path))
            {
                //o = new Bitmap(path);
                o = Image.FromFile(path);
            }
            else if (PathUtils.IsStringExt(name))
            {
                o = File.ReadAllText(path);
            }
            else if (PathUtils.IsIconExt(name))
            {
                o = new Icon(path);
            }
            else if (PathUtils.IsCursorExt(name))
            {
                o = new Cursor(path);
            }
            //else if (ResourceFile.Default.IsTextResource(name))
            //{
            //    o = File.ReadAllText(path);
            //}
            else
            {
                o = File.ReadAllBytes(path);
            }
            return o;
        }

        public object RestoreResourceObject(string name, string typeName, byte[] bytes)
        {
            object o = null;
            if (bytes == null) 
                return o;

            if (typeName.EndsWith("Stream"))
            {
                o = new MemoryStream(bytes);
            }
            else if (typeName == "System.Drawing.Bitmap")
            {
                o = new Bitmap(new MemoryStream(bytes));
            }
            else if (typeName == "System.Drawing.Icon")
            {
                o = new Icon(new MemoryStream(bytes));
            }
            else if (typeName == "System.Drawing.Image")
            {
                o = Image.FromStream(new MemoryStream(bytes));
            }
            else if (typeName == "System.Byte[]")
            {
                o = bytes;
            }
            else if (typeName == "System.String")
            {
                o = Encoding.Unicode.GetString(bytes);
            }
            else
            {
                throw new NotImplementedException(typeName);
            }

            return o;
        }

        public byte[] ConvertToBytes(object value)
        {
            if (value == null)
                return null;

            string typeName = value.GetType().FullName;

            byte[] bytes = null;
            if (typeName.EndsWith("Stream"))
            {
                Stream s = value as Stream;
                if (s != null)
                {
                    if (s.CanSeek) s.Seek(0, SeekOrigin.Begin);
                    bytes = new byte[s.Length];
                    s.Read(bytes, 0, bytes.Length);
                }
            }
            else if (typeName == "System.Drawing.Bitmap")
            {
                Bitmap bmp = (Bitmap)value;
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, bmp.RawFormat);
                    bytes = ms.ToArray();
                }
            }
            else if (typeName == "System.Drawing.Icon")
            {
                Icon ico = (Icon)value;
                using (MemoryStream ms = new MemoryStream())
                {
                    ico.Save(ms);
                    bytes = ms.ToArray();
                }
            }
            else if (typeName == "System.Drawing.Image")
            {
                Image img = (Image)value;
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, img.RawFormat);
                    bytes = ms.ToArray();
                }
            }
            else if (typeName == "System.Byte[]")
            {
                bytes = (byte[])value;
            }
            else if (typeName == "System.String")
            {
                bytes = Encoding.Unicode.GetBytes((string)value);
            }

            return bytes;
        }

        public void ShowBinary(DataGridViewRow row)
        {
            var cell = GetValueCell(row);
            if (cell == null || cell.TrueValue == null)
                return;

            string key = GetNameCellValue(row);

            object value = cell.TrueValue;
            byte[] bytes = ConvertToBytes(value);
            if (bytes == null)
                return;

            ShowBinary(bytes,true);
        }

        public void ShowBinary(EmbeddedResource er, bool showingGridBinary)
        {
            ShowBinary(er.GetResourceData(), showingGridBinary);
        }

        public void ShowBinary(byte[] data, bool showingGridBinary)
        {
            if(showingGridBinary)
                ShowDetailsControl(DetailTypes.ResourcesRowAsBinary);
            else
                ShowDetailsControl(DetailTypes.BinaryResource);

            Be.Windows.Forms.DynamicByteProvider dbp = new Be.Windows.Forms.DynamicByteProvider(data);
            hbResource.ByteProvider = null;
            hbResource.ByteProvider = dbp;            
        }

        public void ShowText(string text, DataGridViewRow row = null, bool showingGridText = true)
        {
            txtResource.ReadOnly = true;
            if (row != null || showingGridText)
            {
                ShowDetailsControl(DetailTypes.ResourcesRowAsText);
                if (row == null)
                {
                    row = _form.ResourceDataGrid.CurrentRow;
                    if (row == null && _form.ResourceDataGrid.Rows.Count > 0)
                    {
                        row = _form.ResourceDataGrid.Rows[0];
                    }
                }
                if (row != null)
                {
                    var cell = _form.ResourceHandler.GetValueCell(row);
                    if (cell.TrueValueType.FullName == "System.String")
                    {
                        txtResource.ReadOnly = false;
                    }
                }
            }
            else
            {
                ShowDetailsControl(DetailTypes.TextResource);
                txtResource.ReadOnly = false;
            }

            if (text != null)
            {
                txtResource.Text = text;
            }

            if (!txtResource.ReadOnly)
            {
                _form.SetStatusText("CTRL+S to commit, CTRL+R to rollback.");
            }
        }

        public void ShowText(Stream stream, DataGridViewRow row = null, bool showingGridText = true)
        {
            //use StreamReader to detect encoding?
            string text;

            StreamReader sr = new StreamReader(stream);
            text = sr.ReadToEnd();
            //sr.Close();
            ShowText(text, row, showingGridText);
        }

        private void ShowResource(EmbeddedResource er)
        {
            if (PathUtils.IsResourceExt(er.Name))
            {
                if (InitResourceGridRows(er))
                {
                    //ShowDetailsControl(DetailTypes.Resources);
                    if (_dtResource.Rows.Count == 0)
                    {
                        ShowText(String.Empty);
                    }
                    else
                    {
                        ShowText((string)null);
                    }
                }
                else
                {
                    ShowBinary(er, false);
                }
            }
            else if (ResourceFile.Default.IsTextResource(er.Name))
            {
                ShowText(er.GetResourceStream(), null, false);
            }
            else if (PathUtils.IsIconExt(er.Name))
            {
                Icon ico = new Icon(er.GetResourceStream());
                pbResource.Image = ico.ToBitmap();
                ShowDetailsControl(DetailTypes.ImageResource);
            }
            else if (PathUtils.IsCursorExt(er.Name))
            {
                Cursor c = LoadCursor(er.GetResourceStream());
                Bitmap bmp = ConvertToBitmap(c);
                pbResource.Image = bmp;
                ShowDetailsControl(DetailTypes.ImageResource);
            }
            else if (ResourceFile.Default.IsImageResource(er.Name))
            {
                //wmf size is wrong with Image.FromStream
                //pbResource.Image = new Bitmap(er.GetResourceStream());

                Bitmap bmp = new Bitmap(er.GetResourceStream());
                pbResource.Image = bmp;
                //if (!ImageAnimator.CanAnimate(bmp))
                //{
                //    pbResource.Image = bmp;
                //}
                //else
                //{
                //    pbResource.Image = new Bitmap(bmp.Width, bmp.Height);
                //}
                ShowDetailsControl(DetailTypes.ImageResource);
            }
            else
            {
                //always try to parse as .resources
                if (InitResourceGridRows(er))
                {
                    //ShowDetailsControl(DetailTypes.Resources);
                    if (_dtResource.Rows.Count == 0)
                    {
                        ShowText(String.Empty);
                    }
                    else
                    {
                        ShowText((string)null);
                    }
                }
                else
                {
                    ShowBinary(er, false);
                }
            }
        }

        private void ShowResource(AssemblyLinkedResource alr)
        {
            ShowText(String.Format("AssemblyLinkedResource: {0}", alr.Assembly.FullName), null, false);
        }

        private void ShowResource(LinkedResource lr)
        {
            ShowText(String.Format("LinkedResource: {0}", lr.File), null, false);
        }

        private void ShowResource(Resource r)
        {
            switch (r.ResourceType)
            {
                case ResourceType.Embedded:
                    ShowResource((EmbeddedResource)r);
                    break;
                case ResourceType.AssemblyLinked:
                    ShowResource((AssemblyLinkedResource)r);
                    break;
                case ResourceType.Linked:
                    ShowResource((LinkedResource)r);
                    break;
                default:
                    throw new NotSupportedException("Resource type is not supported: " + r.ResourceType.ToString());
            }
        }



        public void InitResource(Resource r)
        {
            BaseAssemblyResolver bar = GlobalAssemblyResolver.Instance as BaseAssemblyResolver;
            bool savedRaiseResolveException = true;
            try
            {
                if (bar != null)
                {
                    savedRaiseResolveException = bar.RaiseResolveException;
                    bar.RaiseResolveException = false;
                }

                detailsTabPage.SuspendLayout();

                dgResource.SuspendLayout();
                _dtResource.Rows.Clear();

                if (r == null)
                {
                    return;
                }

                _dtResource.BeginLoadData();

                ShowResource(r);

                if (dgResource.DataSource == null)
                    dgResource.DataSource = _dtResource;

                _dtResource.EndLoadData();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (bar != null)
                {
                    bar.RaiseResolveException = savedRaiseResolveException;
                }
                dgResource.ResumeLayout();
                detailsTabPage.ResumeLayout();
            }
        }

        public void dgResource_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Apps && dgResource.SelectedRows.Count > 0)
            {
                Rectangle rect = dgResource.GetCellDisplayRectangle(1, dgResource.SelectedRows[0].Index, true);
                Point point = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                point = dgResource.PointToScreen(point);
                _form.ResourceContextMenuStrip.Show(point);
                e.Handled = true;
            }
        }

        public void dgResource_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && !dgResource.Rows[e.RowIndex].Selected)
            {
                dgResource.CurrentCell = dgResource.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        //const int COL_NO = 0;
        const int COL_NAME = 1;
        const int COL_VALUE = 2;
        public string GetNameCellValue(DataGridViewRow row)
        {            
            return row.Cells[COL_NAME].Value as string;
        }
        public DataGridViewTextAndImageCell GetValueCell(DataGridViewRow row)
        {            
            return row.Cells[COL_VALUE] as DataGridViewTextAndImageCell;
        }

        public void ShowResource(DataGridViewRow row)
        {
            var cell = GetValueCell(row);
            if (cell == null || cell.TrueValue == null) return;

            string resName = GetNameCellValue(row);
            string typeName = cell.TrueValueType.FullName;
            object value = cell.TrueValue;
            bool handled = false;
            if (CanSaveResource(typeName))
            {
                if (typeName.EndsWith("Stream"))
                {
                    Stream s = (Stream)value;
                    if (s != null)
                    {
                        if (s.CanSeek) s.Seek(0, SeekOrigin.Begin);
                        if (PathUtils.IsBamlExt(resName))
                        {                            
                            ShowText(DecompileBaml(s), row);
                            handled = true;
                        }
                        else if (ResourceFile.Default.IsTextResource(resName))
                        {
                            ShowText(s, row);
                            handled = true;
                        }
                    }
                }
                else
                {
                    switch (typeName)
                    {
                        case "System.Windows.Forms.ImageListStreamer":
                            {
                                ImageListStreamer s = (ImageListStreamer)value;
                                ImageList imgList = new ImageList();
                                imgList.ImageStream = s;
                                lvResource.BeginUpdate();
                                lvResource.Items.Clear();
                                lvResource.LargeImageList = imgList;
                                for (int i = 0; i < imgList.Images.Count; i++)
                                {
                                    lvResource.Items.Add(String.Format("Image{0}", i), i);
                                }
                                lvResource.View = View.LargeIcon;
                                lvResource.EndUpdate();
                                ShowDetailsControl(DetailTypes.ResourcesRowAsImageList);
                                handled = true;
                            }
                            break;
                        case "System.Byte[]":
                            {
                                ShowBinary(row);
                                handled = true;
                            }
                            break;
                    }
                }
            }
            if (!handled)
            {
                if (cell.Image != null)
                {
                    pbResource.Image = cell.Image;
                    ShowDetailsControl(DetailTypes.ResourcesRowAsImage);
                }
                else if (typeName.EndsWith("Stream"))
                {
                    Stream s = (Stream)value;
                    if (s != null)
                    {
                        if (s.CanSeek) s.Seek(0, SeekOrigin.Begin);
                        //byte[] bytes = new byte[s.Length];
                        //s.Read(bytes, 0, bytes.Length);
                        ShowBinary(row);
                    }
                    else
                    {
                        //ShowDetailsControl(DetailTypes.Resources);
                        ShowText(String.Empty, row);
                    }
                }
                else
                {
                    object textValue = GetValueCell(row).Value;
                    //ShowDetailsControl(DetailTypes.Resources);
                    ShowText(textValue == null ? String.Empty : textValue.ToString(), row);
                }
            }
        }

        public void dgResource_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgResource.Rows[e.RowIndex];
            ShowResource(row);
        }

        private string GetImageExt(Image img)
        {
            if (img.RawFormat.Equals(ImageFormat.Jpeg)) return ".jpg";
            else if (img.RawFormat.Equals(ImageFormat.Png)) return ".png";
            else if (img.RawFormat.Equals(ImageFormat.Bmp)) return ".bmp";
            else if (img.RawFormat.Equals(ImageFormat.Gif)) return ".gif";
            else if (img.RawFormat.Equals(ImageFormat.Icon)) return ".ico";
            else if (img.RawFormat.Equals(ImageFormat.MemoryBmp)) return ".bmp";
            else if (img.RawFormat.Equals(ImageFormat.Tiff)) return ".tif";
            else if (img.RawFormat.Equals(ImageFormat.Wmf)) return ".wmf";
            else if (img.RawFormat.Equals(ImageFormat.Emf)) return ".emf";
            else if (img.RawFormat.Equals(ImageFormat.Exif)) return ".exif";
            else return String.Empty;
        }

        private string GetOutputResourceFile(string resName, string saveDir)
        {
            string outputFile = resName;
            int p = outputFile.LastIndexOf('/');
            if (p >= 0)
            {
                outputFile = outputFile.Substring(p + 1);
            }
            if (!PathUtils.IsValidFileName(outputFile))
                outputFile = PathUtils.FixFileName(outputFile);
            outputFile = Path.Combine(saveDir, outputFile);
            return outputFile;
        }

        private int SaveStreamResource(string resName, Stream s, string saveDir)
        {
            if (s == null)
                return 0;

            int count = 0;
            bool handled = false;
            string outputFile = GetOutputResourceFile(resName, saveDir);

            if (PathUtils.IsIconExt(outputFile))
            {
                try
                {
                    if (s.CanSeek)
                        s.Seek(0, SeekOrigin.Begin);
                    Icon ico = new Icon(s);
                    using (FileStream fs = File.Create(outputFile))
                    {
                        ico.Save(fs);
                    }
                    count++;
                    handled = true;
                }
                catch (ArgumentException) { }
            }
            else if (PathUtils.IsCursorExt(outputFile))
            {
                try
                {
                    //it seems there is no easy way to write cursor to file
                    //http://msdn.microsoft.com/en-us/library/ms997538.aspx
                    Cursor c = LoadCursor(s);
                    Bitmap bmp = ConvertToBitmap(c);
                    bmp.Save(outputFile + GetImageExt(bmp));
                    count++;
                    handled = true;
                }
                catch (Exception ex)
                {
                    _form.SetStatusText(String.Format("Failed to save {0}: {1}", resName, ex.Message));
                }                
            }
            else if (ResourceFile.Default.IsImageResource(outputFile))
            {
                try
                {
                    Bitmap bmp = new Bitmap(s);
                    bmp.Save(outputFile);
                    count++;
                    handled = true;
                }
                catch (ArgumentException) { }
            }
            else if (PathUtils.IsBamlExt(outputFile))
            {
                try
                {
                    string newOutputFile = Path.ChangeExtension(outputFile, ".xaml");
                    using (StreamWriter sw = File.CreateText(newOutputFile))
                    {
                        sw.WriteLine(DecompileBaml(s));
                    }
                    count++;
                    //handled = true; 
                }
                catch (Exception ex)
                {
                    _form.SetStatusText(String.Format("Failed to translate {0}: {1}", resName, ex.Message));
                }
            }

            if (!handled)
            {
                byte[] buffer = new byte[s.Length];
                if (s.CanSeek)
                {
                    s.Seek(0, SeekOrigin.Begin);
                }
                s.Read(buffer, 0, buffer.Length);
                using (FileStream fs = File.Create(outputFile))
                {
                    fs.Write(buffer, 0, buffer.Length);
                }
                count++;
            }

            return count;
        }

        private int SaveResource(string resName, object value, string saveDir)
        {
            if (value == null)
                return 0;

            string typeName = value.GetType().FullName;
            if (typeName.EndsWith("Stream"))
            {
                Stream s = value as Stream;
                return SaveStreamResource(resName, s, saveDir);
            }

            int count = 0;
            string ext = null;
            string outputFile = GetOutputResourceFile(resName, saveDir);

            switch (typeName)
            {
                case "System.Drawing.Bitmap":
                    {
                        Bitmap bmp = (Bitmap)value;
                        ext = GetImageExt(bmp);
                        if (!String.IsNullOrEmpty(ext))
                            outputFile += ext;
                        bmp.Save(outputFile);
                        count++;
                    }
                    break;
                case "System.Drawing.Image":
                    {
                        Image img = (Image)value;
                        ext = GetImageExt(img);
                        if (!String.IsNullOrEmpty(ext))
                            outputFile += ext;
                        img.Save(outputFile);
                        count++;
                    }
                    break;
                case "System.Drawing.Icon":
                    {
                        Icon ico = (Icon)value;
                        using (FileStream fs = File.Create(outputFile + ".ico"))
                        {
                            ico.Save(fs);
                        }
                        count++;
                    }
                    break;
                case "System.Windows.Forms.Cursor":
                    {
                        //it seems there is no easy way to write cursor to file
                        Cursor c = (Cursor)value;
                        Bitmap bmp = ConvertToBitmap(c);
                        bmp.Save(outputFile + GetImageExt(bmp));
                    }
                    break;
                case "System.Windows.Forms.ImageListStreamer":
                    {
                        ImageListStreamer s = (ImageListStreamer)value;
                        ImageList imgList = new ImageList();
                        imgList.ImageStream = s;
                        for (int i = 0; i < imgList.Images.Count; i++)
                        {
                            Image img = imgList.Images[i];
                            img.Save(String.Format("{0}{1}{2}", outputFile, i + 1, GetImageExt(img)));
                            count++;
                        }
                    }
                    break;
                case "System.Byte[]":
                    {
                        byte[] bytes = (byte[])value;
                        using (FileStream fs = File.Create(outputFile))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        count++;
                    }
                    break;
                case "System.String":
                    {
                        byte[] bytes = Encoding.Unicode.GetBytes((string)value);
                        using (FileStream fs = File.Create(outputFile + ".string"))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        count++;
                    }
                    break;
            }
            return count;
        }


        public void cmResourceViewAsNormal_Click(object sender, EventArgs e)
        {
            if (dgResource.SelectedRows.Count != 1)
                return;

            DataGridViewRow row = dgResource.SelectedRows[0];
            ShowResource(row);
        }

        public void cmResourceViewAsBinary_Click(object sender, EventArgs e)
        {
            if (dgResource.SelectedRows.Count != 1)
                return;

            DataGridViewRow row = dgResource.SelectedRows[0];
            ShowBinary(row);
        }

        public void cmResourceSaveAs_Click(object sender, EventArgs e)
        {
            string initDir = Config.ClassEditorSaveAsDir;
            if (!Directory.Exists(initDir))
                initDir = Environment.CurrentDirectory;
            string saveDir = SimpleDialog.OpenFolder(initDir);
            if (!String.IsNullOrEmpty(saveDir))
            {
                Config.ClassEditorSaveAsDir = saveDir;
            }
            if (String.IsNullOrEmpty(saveDir)) return;

            int count = 0;

            foreach (DataGridViewRow row in dgResource.SelectedRows)
            {
                var cell = GetValueCell(row);
                if (cell == null || cell.TrueValue == null) continue;

                string resName = GetNameCellValue(row);
                count += SaveResource(resName, cell.TrueValue, saveDir);
            }
            _form.SetStatusText(String.Format("{0} file(s) saved.", count));
        }

        private bool CanSaveResource(string typeName)
        {
            return (
                    typeName == "System.String" ||
                    typeName == "System.Drawing.Image" ||
                    typeName == "System.Drawing.Bitmap" ||
                    typeName == "System.Drawing.Icon" ||
                    typeName == "System.Windows.Forms.Cursor" ||
                    typeName == "System.Windows.Forms.ImageListStreamer" ||
                    typeName == "System.Byte[]" ||
                    typeName.EndsWith("Stream")
                    );
        }

        private bool CanViewAsBinary(string typeName)
        {
            return (
                    typeName == "System.String" ||
                    typeName == "System.Drawing.Image" ||
                    typeName == "System.Drawing.Bitmap" ||
                    typeName == "System.Drawing.Icon" ||
                    typeName == "System.Byte[]" ||
                    typeName.EndsWith("Stream")
                    );
        }

        public void cmResource_Opening(object sender, CancelEventArgs e)
        {
            var cms = sender as ContextMenuStrip;
            cms.Items["cmResourceImportFromFile"].Enabled = true;

            if (dgResource.SelectedRows.Count == 0)
            {
                cms.Items["cmResourceSaveAs"].Enabled = false;
                cms.Items["cmResourceViewAsBinary"].Enabled = false;
                cms.Items["cmResourceViewAsNormal"].Enabled = false;
                cms.Items["cmResourceRemove"].Enabled = false;
                return;
            }

            var cell = GetValueCell(dgResource.SelectedRows[0]);
            bool saveAsEnabled = false;
            bool viewAsBinaryEnabled = false;

            if (cell != null && cell.TrueValue != null)
            {
                string typeName = cell.TrueValueType.FullName;
                saveAsEnabled = CanSaveResource(typeName);
                viewAsBinaryEnabled = CanViewAsBinary(typeName);
            }

            cms.Items["cmResourceSaveAs"].Enabled = saveAsEnabled;
            cms.Items["cmResourceViewAsBinary"].Enabled = viewAsBinaryEnabled;
            cms.Items["cmResourceViewAsNormal"].Enabled = viewAsBinaryEnabled;
            cms.Items["cmResourceRemove"].Enabled = true;

            var bamlTranslatorList = ((ToolStripDropDownItem) cms.Items["cmResourceBamlTranslator"]).DropDownItems;
            for(int i=0; i<bamlTranslatorList.Count; i++) {
                var bamlMenu = (ToolStripMenuItem)bamlTranslatorList[i];
                bamlMenu.Checked = (i == Config.ClassEditorBamlTranslator);
            }
        }

        public string DecompileBaml(Stream stream)
        {
            BaseAssemblyResolver bar = GlobalAssemblyResolver.Instance as BaseAssemblyResolver;
            bool savedRaiseResolveException = true;

            try
            {
                if (bar != null)
                {
                    savedRaiseResolveException = bar.RaiseResolveException;
                    bar.RaiseResolveException = false;
                }

                //ILspy.BamlDecompiler translator
                if (Config.ClassEditorBamlTranslator == 1)
                {
                    string currentAssemblyFile;
                    AssemblyDefinition ad;
                    int index;
                    _form.ReflectorHandler.FindCurrentAssembly(out currentAssemblyFile, out ad, out index);
                    if (index < 0)
                        return String.Empty;

                    return new BamlILSpyTranslator(ad, stream).ToString();
                }

                //reflector.bamlviwer translator
                return new BamlTranslator(stream).ToString();
            }
            finally
            {
                if (bar != null)
                {
                    bar.RaiseResolveException = savedRaiseResolveException;
                }
            }

        }

        public void cmResourceImportFromFile_Click(object sender, EventArgs e)
        {
            var fileNames = _form.TreeViewHandler.SelectResourceFile();

            if (fileNames == null || fileNames.Length == 0)
                return;

            var resourceNode = _form.TreeView.SelectedNode;
            var er = resourceNode.Tag as EmbeddedResource;
            if (er == null) return;

            Dictionary<string, object> list = new Dictionary<string, object>(fileNames.Length);
            foreach (string path in fileNames)
            {
                if (String.IsNullOrEmpty(path))
                    continue;

                string name;
                if (PathUtils.IsStringExt(path))
                {
                    name = Path.GetFileNameWithoutExtension(path);
                }
                else
                {
                    name = Path.GetFileName(path);
                }
                string ext = Path.GetExtension(path);
                var o = LoadResourceObject(path);
                list.Add(name, o);
            }

            if (list.Count == 0)
                return;

            var rr = new ResourceReader(er.GetResourceStream());
            var ms = new MemoryStream();
            var rw = new ResourceWriter(ms);

            var de = rr.GetEnumerator();
            while (de.MoveNext())
            {
                string deKey = de.Key as string;
                if (list.ContainsKey(deKey))
                {
                    rw.AddResource(deKey, list[deKey]);
                    list.Remove(deKey);
                }
                else
                {
                    rw.AddResource(deKey, de.Value);
                }
            }
            foreach (string key in list.Keys)
            {
                rw.AddResource(key, list[key]);
            }

            rw.Generate();
            var newEr = new EmbeddedResource(er.Name, er.Attributes, ms.ToArray());
            rw.Close();
            ms.Close();

            var ad = _form.TreeViewHandler.GetCurrentAssembly();
            ad.MainModule.Resources.Remove(er);
            ad.MainModule.Resources.Add(newEr);
            resourceNode.Tag = newEr;
            _form.ResourceHandler.InitResource(newEr);

        }

        public void cmResourceRemove_Click(object sender, EventArgs e)
        {
            var rows = _form.ResourceDataGrid.SelectedRows;
            if (rows == null || rows.Count == 0) return;

            var resourceNode = _form.TreeView.SelectedNode;
            var er = resourceNode.Tag as EmbeddedResource;
            if(er == null) return;

            string[] keys = new string[rows.Count];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = GetNameCellValue(rows[i]);
            }

            var newEr = RemoveResource(er, keys);
            var ad = _form.TreeViewHandler.GetCurrentAssembly();
            ad.MainModule.Resources.Remove(er);
            ad.MainModule.Resources.Add(newEr);
            resourceNode.Tag = newEr;
            _form.ResourceHandler.InitResource(newEr);
        }

        public void cmResourceReflectorBamlViewer_Click(object sender, EventArgs e)
        {
            Config.ClassEditorBamlTranslator = (int)BamlTranslators.ReflectorBamlViewer;
            cmResourceViewAsNormal_Click(sender, e);
        }

        public void cmResourceILSpyBamlDecompiler_Click(object sender, EventArgs e)
        {
            Config.ClassEditorBamlTranslator = (int)BamlTranslators.ILSpyBamlDecompiler;
            cmResourceViewAsNormal_Click(sender, e);
        }
        
    } // end of class


}
