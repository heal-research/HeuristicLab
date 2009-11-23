using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.Optimizer.MenuItems {
  public class SaveAllMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItemBase, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Save Al&l"; }
    }

    public override IEnumerable<string> Structure {
      get { return new string[] { "&File" }; }
    }

    public override int Position {
      get { return 1500; }
    }

    public override ToolStripItemDisplayStyle ToolStripItemDisplayStyle {
      get { return ToolStripItemDisplayStyle.ImageAndText; }
    }

    public override Image Image {
      get { return Resources.SaveAllIcon; }
    }

    public override void Execute() {
      Actions.SaveAllAction.Execute();
    }
  }
}
