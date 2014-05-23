using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SimpleUtils.Win
{
    public partial class frmSimpleDialogBase : frmSimpleBase
    {
        public frmSimpleDialogBase()
        {
            InitializeComponent();

            this.EscToExit = true;

        }

        public bool EscToExit { get; set; }

        private void frmSimpleDialogBase_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.EscToExit && e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

    }
}