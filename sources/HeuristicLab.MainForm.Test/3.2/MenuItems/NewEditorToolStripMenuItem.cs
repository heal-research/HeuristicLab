using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.MainForm;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MainForm.Test {
  public class NewEditorToolStripMenuItem : ToolStripMenuItemBase, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "Editor"; }
    }

    public override string Structure {
      get { return "File/New"; }
    }

    public override int Position {
      get { return 1120; }
    }

    public override void Execute(IMainForm mainform) {
      new NewEditorAction().Execute(mainform);
    }
  }
}
