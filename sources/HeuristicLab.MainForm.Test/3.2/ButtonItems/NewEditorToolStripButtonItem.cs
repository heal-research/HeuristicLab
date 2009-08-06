using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.MainForm.Test {
  public class NewEditorToolStripButtonItem : ToolStripButtonItemBase, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 12; }
    }

    public override string Name {
      get { return "Editor"; }
    }

    public override string Structure {
      get { return "New"; }
    }

    public override void Execute(IMainForm mainform) {
      new NewEditorAction().Execute(mainform);
    }
  }
}
