using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.MainForm;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public class OpenMenuItem : ToolStripMenuItemBase, IToolStripButtonItem, ITestUserInterfaceItemProvider {
    public override string MenuStructure {
      get { return "File"; }
    }

    public override string Name {
      get { return "Open"; }
    }

    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.O; }
    }

    public override void Execute(IMainForm mainform) {
      MessageBox.Show("Open Execute called");
    }
  }
}
