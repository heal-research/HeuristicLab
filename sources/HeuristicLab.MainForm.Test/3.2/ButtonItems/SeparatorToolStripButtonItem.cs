using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.MainForm.Test {
  public class SeparatorToolStripButtonItem : ToolBarSeparatorItemBase, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 15; }
    }
  }
}
