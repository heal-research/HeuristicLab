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
    public ExecuterView(ExecuterBase executer)
      : base() {
      this.executer = executer;
      InitializeComponent();
      maxActiveJobs.Value = executer.MaxActiveJobs;
      executer.Changed += (sender, args) => UpdateControls();
    }

    protected override void UpdateControls() {
      if (InvokeRequired) Invoke((Action)UpdateControls);
      else {
        base.UpdateControls();
        maxActiveJobs.Value = executer.MaxActiveJobs;
        jobsList.DataSource = executer.GetJobs();
        jobsList.Refresh();
      }
    }

    private void maxActiveJobs_ValueChanged(object sender, EventArgs e) {
      executer.MaxActiveJobs = Convert.ToInt32(maxActiveJobs.Value);
    }
  }
}
