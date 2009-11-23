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
  public class SaveAllToolBarItem : HeuristicLab.MainForm.WindowsForms.ToolBarItemBase, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Save All"; }
    }

    public override string ToolTipText {
      get { return "Save All Files"; }
    }

    public override int Position {
      get { return 40; }
    }

    public override Image Image {
      get { return Resources.SaveAllIcon; }
    }

    public override void Execute() {
      Actions.SaveAllAction.Execute();
    }
  }
}
