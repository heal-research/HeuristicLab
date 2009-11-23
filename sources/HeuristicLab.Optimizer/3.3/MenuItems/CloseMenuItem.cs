using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.Optimizer.MenuItems {
  public class CloseMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItemBase, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Close"; }
    }

    public override IEnumerable<string> Structure {
      get { return new string[] { "&File" }; }
    }

    public override int Position {
      get { return 1600; }
    }

    public override void Execute() {
      MainFormManager.MainForm.CloseView(MainFormManager.MainForm.ActiveView);
    }
  }
}
