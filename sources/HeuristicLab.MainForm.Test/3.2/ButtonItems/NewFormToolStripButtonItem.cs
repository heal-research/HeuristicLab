using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.MainForm.Test {
  public class NewFormToolStripButtonItem : ToolStripButtonItemBase, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 11; }
    }

    public override string Name {
      get { return "Form"; }
    }

    public override string Structure {
      get { return "New"; }
    }

    public override void Execute(IMainForm mainform) {
      new NewFormAction().Execute(mainform);
    }
  }
}
