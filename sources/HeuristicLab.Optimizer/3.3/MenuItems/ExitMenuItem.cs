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
  internal class ExitMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItemBase, IOptimizerUserInterfaceItemProvider {
    public override IEnumerable<string> Structure {
      get { return new string[] { "&File" }; }
    }
    public override string Name {
      get { return "E&xit"; }
    }
    public override int Position {
      get { return 1999; }
    }
    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.X; }
   }

    public override void Execute() {
      MainFormManager.MainForm.Close();
    }
  }
}
