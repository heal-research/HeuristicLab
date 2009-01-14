using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Hive.Server
{
    public partial class MainForm : Form {
        public MainForm(Uri address1, Uri address2, Uri address3) {
          InitializeComponent();
          if(address1 != null)
            this.lblAddress1.Text = address1.ToString();
          if (address2 != null)
            this.lblAddress2.Text = address2.ToString();
          if (address3 != null)
            this.lblAddress3.Text = address3.ToString();
        }

    }
}
