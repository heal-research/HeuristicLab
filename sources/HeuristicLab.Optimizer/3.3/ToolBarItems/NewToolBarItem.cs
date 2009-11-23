using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.Optimizer {
  public class NewToolBarItem : HeuristicLab.MainForm.WindowsForms.ToolBarItemBase, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "New..."; }
    }

    public override string ToolTipText {
      get { return "New Item (Ctrl + N)"; }
    }

    public override int Position {
      get { return 10; }
    }

    public override Image Image {
      get { return Resources.NewIcon; }
    }

    public override void Execute() {
      Actions.NewAction.Execute();
    }
  }
}
