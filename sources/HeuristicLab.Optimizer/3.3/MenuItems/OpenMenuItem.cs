using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class OpenMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Open..."; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&File" }; }
    }
    public override int Position {
      get { return 1200; }
    }
    public override Image Image {
      get { return Resources.OpenIcon; }
    }
    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.O; }
    }

    public override void Execute() {
      FileManager.Open();
    }
  }
}
