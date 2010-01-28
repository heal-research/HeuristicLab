using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.MainForm.Test {
  public class SaveToolStripButtonItem : HeuristicLab.MainForm.WindowsForms.ToolBarItem, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 30; }
    }

    public override string Name {
      get { return "Save"; }
    }

    public override System.Drawing.Image Image {
      get { return HeuristicLab.Common.Resources.Resources.SaveIcon; }
    }

    public override void Execute() {
      new SaveAction().Execute(MainFormManager.MainForm);
    }

    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IMainForm mainform = MainFormManager.MainForm;
      if (mainform.ActiveView == null)
        this.ToolStripItem.Enabled = false;
      else
        this.ToolStripItem.Enabled = !(mainform.ActiveView is FormView1);
    }

    protected override void OnViewChanged(object sender, EventArgs e) {
      this.ToolStripItem.Enabled = !this.ToolStripItem.Enabled;
    }
  }
}
