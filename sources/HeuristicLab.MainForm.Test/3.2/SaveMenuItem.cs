using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.MainForm;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MainForm.Test {
  public class SaveMenuItem : ToolStripMenuItemBase, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "Save"; }
    }

    public override string MenuStructure {
      get { return "File"; }
    }

    public override int Position {
      get { return 1200; }
    }

    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.S; }
    }

    public override System.Drawing.Image Image {
      get { return Resources.SaveIcon; }
    }

    public override void Execute(IMainForm mainform) {
      MessageBox.Show("Save Execute called");
    }
  }
}
