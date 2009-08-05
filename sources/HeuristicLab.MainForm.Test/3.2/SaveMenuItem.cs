using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.MainForm;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public class SaveMenuItem : ToolStripMenuItemBase, ITestUserInterfaceItemProvider {
    public override string MenuStructure {
      get { return "TEST/Test/Test/Test"; }
    }

    public override string Name {
      get { return "Save"; }
    }

    public override void Execute(IMainForm mainform) {
      MessageBox.Show("Save Execute called");
    }
  }
}
