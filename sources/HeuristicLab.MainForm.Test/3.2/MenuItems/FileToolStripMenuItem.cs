using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.MainForm.Test.MenuItems {
  public class FileToolStripMenuItem : ToolStripMenuItemBase, ITestUserInterfaceItemProvider{
    public override int Position {
      get { return 1000; }
    }

    public override string Name {
      get { return "File"; }
    }

    public override void Execute(IMainForm mainform) {      
    }
  }
}
