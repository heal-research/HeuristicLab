using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.MainForm;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.Test {
  public class ViewsMenuItem : ToolStripMenuItemBase, IToolStripButtonItem, ITestUserInterfaceItemProvider {
    public override string MenuStructure {
      get { return "File"; }
    }

    public override string Name {
      get { return "Views"; }
    }

    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.V; }
    }

    public override System.Drawing.Image Image {
      get { return Resource1.openHS; }
    }

    public override void Execute(IMainForm mainform) {
      MessageBox.Show("Views Execute called");
    }
  }
}
