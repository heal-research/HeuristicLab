using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.Optimizer.MenuItems {
  public class SaveAsMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItemBase, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Save &As..."; }
    }

    public override IEnumerable<string> Structure {
      get { return new string[] { "&File" }; }
    }

    public override int Position {
      get { return 1400; }
    }

    public override ToolStripItemDisplayStyle ToolStripItemDisplayStyle {
      get { return ToolStripItemDisplayStyle.ImageAndText; }
    }

    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.Shift | Keys.S; }
    }

    public override void Execute() {
      Actions.SaveAsAction.Execute();
    }
  }
}
