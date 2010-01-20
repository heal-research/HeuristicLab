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
  internal class OperatorsMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItemBase, IOptimizerUserInterfaceItemProvider {
    private OperatorsSidebar view;
    private ToolStripMenuItem menuItem;

    public override string Name {
      get { return "&Operators"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&View" }; }
    }
    public override int Position {
      get { return 2100; }
    }
    public override ToolStripItemDisplayStyle ToolStripItemDisplayStyle {
      get { return ToolStripItemDisplayStyle.Text; }
    }

    public override void MainFormInitialized(object sender, EventArgs e) {
      view = new OperatorsSidebar();
      view.Dock = DockStyle.Left;
      MainFormManager.MainForm.ViewHidden += new EventHandler<ViewEventArgs>(MainForm_ViewHidden);

      menuItem = ToolStripItem as ToolStripMenuItem;
      if (menuItem != null) {
        menuItem.Checked = true;
        menuItem.CheckOnClick = true;
      }
      MainFormManager.MainForm.ShowView(view);
    }

    private void MainForm_ViewHidden(object sender, ViewEventArgs e) {
      if ((e.View == view) && (menuItem != null))
        menuItem.Checked = false;
    }

    public override void Execute() {
      if (menuItem != null) {
        if (menuItem.Checked)
          MainFormManager.MainForm.ShowView(view);
        else
          MainFormManager.MainForm.HideView(view);
      } else {
        MainFormManager.MainForm.ShowView(view);
      }
    }
  }
}
