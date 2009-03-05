using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Hive.Client.Console {
  public partial class Recurrence : Form {
    public Recurrence() {
      InitializeComponent();
    }

    private void rbtDaily_CheckedChanged(object sender, EventArgs e) {
      if (rbtDaily.Checked) {
        gbDaily.Visible = true;
        gbWeekly.Visible = false;
      } else if (rbtWeekly.Checked) {
        gbDaily.Visible = false;
        gbWeekly.Visible = true;
      }
    }
  }
}
