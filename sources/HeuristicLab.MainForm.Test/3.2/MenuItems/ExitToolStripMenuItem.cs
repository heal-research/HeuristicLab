using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.MainForm;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MainForm.Test {
  public class CloseToolStripMenuItem : ToolStripMenuItemBase, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "Exit"; }
    }

    public override string Structure {
      get { return "File"; }
    }

    public override int Position {
      get { return 1400; }
    }

    public override void Execute(IMainForm mainform) {
      mainform.CloseForm();
    }
  }
}
