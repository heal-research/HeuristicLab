using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.MainForm.Test {
  public class OpenToolStripButtonItem : ToolStripButtonItemBase, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 20; }
    }

    public override string Name {
      get { return "Open"; }
    }

    public override System.Drawing.Image Image {
      get { return HeuristicLab.Common.Resources.Resources.OpenIcon; }
    }

    public override void Execute(IMainForm mainform) {
      new OpenAction().Execute(mainform);
    }
  }
}
