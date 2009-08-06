using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MainForm.Test {
  public class OpenMenuItem : ToolStripMenuItemBase, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "Open"; }
    }   

    public override string MenuStructure {
      get { return "File"; }
    }

    public override int Position {
      get { return 1100; }
    }

    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.O; }
    }

    public override System.Drawing.Image Image {
      get { return Resources.OpenIcon; }
    }

    public override void Execute(IMainForm mainform) {
      MessageBox.Show("Open Execute called");
    }
  }
}
