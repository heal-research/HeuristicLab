using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class SaveMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Save"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&File" }; }
    }
    public override int Position {
      get { return 1300; }
    }
    public override ToolStripItemDisplayStyle ToolStripItemDisplayStyle {
      get { return ToolStripItemDisplayStyle.ImageAndText; }
    }
    public override Image Image {
      get { return Resources.SaveIcon; }
    }
    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.S; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      IObjectView activeView = MainFormManager.MainForm.ActiveView as IObjectView;
      ToolStripItem.Enabled = ((activeView != null) && (CreatableAttribute.IsCreatable(activeView.Object.GetType())));
    }

    public override void Execute() {
      FileManager.Save();
    }
  }
}
