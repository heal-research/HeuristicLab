using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.MainForm.Test {
  public class NewToolStripButtonItem : HeuristicLab.MainForm.WindowsForms.ToolBarItemBase, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 10; }
    }

    public override string Name {
      get { return "New"; }
    }

    public override bool IsDropDownButton {
      get { return true; }
    }

    public override System.Drawing.Image Image {
      get { return HeuristicLab.Common.Resources.Resources.NewIcon; }
    }

    public override void Execute() {
    }
  }
}
