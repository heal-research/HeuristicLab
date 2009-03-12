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

    private void btCancelRecurrence_Click(object sender, EventArgs e) {
      this.Close();
    }

    private void gbAppointment_Enter(object sender, EventArgs e) {

    }

    private void cmbEnd_Click(object sender, EventArgs e) {

    }

    private void dtpToTime_ValueChanged(object sender, EventArgs e) {

    }

    private void chbade_CheckedChanged(object sender, EventArgs e) {
        dtpFromTime.Visible = !chbade.Checked;
        dtpToTime.Visible = !chbade.Checked;
    }
  }
}
