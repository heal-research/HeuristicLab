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
  public class SaveToolBarItem : HeuristicLab.MainForm.WindowsForms.ToolBarItemBase, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Save"; }
    }

    public override string ToolTipText {
      get { return "Save File (Ctrl + S)"; }
    }

    public override int Position {
      get { return 30; }
    }

    public override Image Image {
      get { return Resources.SaveIcon; }
    }

    public override void Execute() {
      Actions.SaveAction.Execute();
    }
  }
}
