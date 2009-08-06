using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.MainForm.Test {
  public class SaveToolStripButtonItem : ToolStripButtonItemBase, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 30; }
    }

    public override string Name {
      get { return "Save"; }
    }

    public override System.Drawing.Image Image {
      get { return HeuristicLab.Common.Resources.Resources.SaveIcon; }
    }

    public override void Execute(IMainForm mainform) {
      new SaveAction().Execute(mainform);
    }
  }
}
