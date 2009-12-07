using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class SaveAllMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItemBase, IOptimizerUserInterfaceItemProvider {
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

    public override void MainFormInitialized(object sender, EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    public override void ActiveViewChanged(object sender, EventArgs e) {
      ToolStripItem.Enabled = MainFormManager.MainForm.Views.FirstOrDefault() != null;
    }

    public override void Execute() {
      FileManager.SaveAll();
    }
  }
}
