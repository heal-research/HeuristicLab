using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Server {
  public partial class ExecuterView : ViewBase {
    private ExecuterBase executer;
    public ExecuterView(ExecuterBase executer) {
      this.executer = executer;
      InitializeComponent();
      refreshTimer.Enabled = true;
    }

    private void refreshTimer_Tick(object sender, EventArgs e) {
      jobsList.DataSource = executer.GetJobs();
    }

    private void maxActiveJobsUpDown_ValueChanged(object sender, EventArgs e) {
      executer.MaxActiveJobs = Convert.ToInt32(maxActiveJobs.Value);
    }
  }
}
