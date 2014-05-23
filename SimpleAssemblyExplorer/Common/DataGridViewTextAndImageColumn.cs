using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace SimpleAssemblyExplorer
{
    public class DataGridViewTextAndImageColumn:DataGridViewTextBoxColumn
    {
        public DataGridViewTextAndImageColumn()
        {
            this.CellTemplate = new DataGridViewTextAndImageCell();
        }

        private DataGridViewTextAndImageCell TextAndImageCellTemplate
        {
            get { return this.CellTemplate as DataGridViewTextAndImageCell; }
        }

    }// end of DataGridViewTextAndImageColumn

    public class DataGridViewTextAndImageCellValue
    {
        public string Name { get; private set; }
        public object Value { get; private set; }
        public Image Image { get; set; }

        public DataGridViewTextAndImageCellValue(string name, object value)
        {
            Name = name;
            Value = value;
            Image = null;
        }

        public override string ToString()
        {
            if (Value != null)
            {
                return Value.ToString();
            }
            else
            {
                return Name;
            }
        }

    }


    public class DataGridViewTextAndImageCell : DataGridViewTextBoxCell
    {
        public bool HasCellValue
        {
            get
            {
                return typeof(DataGridViewTextAndImageCellValue).Equals(this.ValueType) &&
                    this.Value != null;
            }
        }

        public DataGridViewTextAndImageCellValue CellValue
        {
            get
            {
                return this.Value as DataGridViewTextAndImageCellValue;
            }
        }

        public Type TrueValueType
        {
            get
            {
                if (HasCellValue)
                {
                    DataGridViewTextAndImageCellValue cv = this.CellValue;
                    if (cv.Value != null)
                        return cv.Value.GetType();
                }
                return this.ValueType;
            }
        }

        public object TrueValue
        {
            get
            {
                if (HasCellValue)
                {
                    DataGridViewTextAndImageCellValue cv = this.CellValue;
                    return cv.Value;
                }
                return this.Value;
            }
        }

        public Image Image
        {
            get
            {
                if (HasCellValue)
                {
                    DataGridViewTextAndImageCellValue cv = this.CellValue;
                    return cv.Image;
                }
                return null;
            }
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds,
        Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
        object value, object formattedValue, string errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
        {
            Image image = null;            
            DataGridViewTextAndImageCellValue cv = value as DataGridViewTextAndImageCellValue;

            if (cv != null)
            {
                image = cv.Image;
            }

            if (image != null && image.Width > 0)
            {
                //this will trigger Clone event because of SharedRow
                DataGridViewRow row = this.DataGridView.Rows[rowIndex];

                //use Bitmap to draw out proper size
                Bitmap bmp = new Bitmap(image);

                int height;
                int width;
                if (bmp.Width > cellBounds.Width)
                {
                    width = cellBounds.Width;
                    height = Convert.ToInt32(1.0 * image.Height * cellBounds.Width / image.Width);
                }
                else
                {
                    width = bmp.Width;
                    height = bmp.Height;
                }

                if (row.Height < height)
                {
                    row.Height = height;
                }               
                
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState,
                   null, "", errorText, cellStyle,
                   advancedBorderStyle, paintParts);

                // Draw the image to the cell.
                GraphicsContainer container = graphics.BeginContainer();

                //graphics.SetClip(cellBounds);
                //graphics.DrawImageUnscaled(image, cellBounds.Location);
                Rectangle rect = new Rectangle(cellBounds.X, cellBounds.Y, width, height);
                graphics.DrawImage(bmp, rect);
                
                graphics.EndContainer(container);               

            }
            else
            {
                // Paint the base content
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState,
                   value, formattedValue, errorText, cellStyle,
                   advancedBorderStyle, paintParts);
            }

        }

        private DataGridViewTextAndImageColumn OwningTextAndImageColumn
        {
            get { return this.OwningColumn as DataGridViewTextAndImageColumn; } 
        }       
        
    } //end of DataGridViewTextAndImageCell
}
