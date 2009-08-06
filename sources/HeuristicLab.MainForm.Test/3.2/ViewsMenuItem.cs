using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.MainForm;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public class ViewsMenuItem : ToolStripMenuItemBase, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "Views"; }
    }

    public override int Position {
      get { return 2000; }
    }

    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.V; }
    }

    public override void Execute(IMainForm mainform) {
      MessageBox.Show("Views Execute called");
    }
  }
}
