using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.MainForm.Test {
  public class OpenToolStripButtonItem : HeuristicLab.MainForm.WindowsForms.ToolBarItemBase, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 20; }
    }

    public override string Name {
      get { return "Open"; }
    }

    public override System.Drawing.Image Image {
      get { return HeuristicLab.Common.Resources.Resources.OpenIcon; }
    }

    public override void Execute() {
      new OpenAction().Execute(MainFormManager.MainForm);
    }

    public override void ActiveViewChanged(object sender, EventArgs e) {
      this.ToolStripItem.Enabled = !this.ToolStripItem.Enabled;
      MainFormManager.MainForm.Title = 
        MainFormManager.MainForm.ActiveView == null ? "null" : MainFormManager.MainForm.ActiveView.Caption;
    }
  }
}
