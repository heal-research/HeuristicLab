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
  public class OpenToolBarItem : HeuristicLab.MainForm.WindowsForms.ToolBarItemBase, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Open..."; }
    }

    public override string ToolTipText {
      get { return "Open File (Ctrl + O)"; }
    }

    public override int Position {
      get { return 20; }
    }

    public override Image Image {
      get { return Resources.OpenIcon; }
    }

    public override void Execute() {
      Actions.OpenAction.Execute();
    }
  }
}
