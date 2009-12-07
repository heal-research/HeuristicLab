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
  internal class SeparatorMenuItem : MenuSeparatorItemBase, IOptimizerUserInterfaceItemProvider {
    public override IEnumerable<string> Structure {
      get { return new string[] { "&File" }; }
    }

    public override int Position {
      get { return 1998; }
    }
  }
}
