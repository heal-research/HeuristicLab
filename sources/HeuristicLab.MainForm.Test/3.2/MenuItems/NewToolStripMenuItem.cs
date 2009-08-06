using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.MainForm;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MainForm.Test {
  public class NewToolStripMenuItem : ToolStripMenuItemBase, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "New"; }
    }

    public override string Structure {
      get { return "File"; }
    }

    public override int Position {
      get { return 1100; }
    }

    public override System.Drawing.Image Image {
      get { return Resources.NewIcon; }
    }

    public override void Execute(IMainForm mainform) {
    }
  }
}
