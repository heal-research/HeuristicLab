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

namespace HeuristicLab.Optimizer {
  internal class SaveToolBarItem : HeuristicLab.MainForm.WindowsForms.ToolBarItemBase, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Save"; }
    }
    public override string ToolTipText {
      get { return "Save File (Ctrl + S)"; }
    }
    public override int Position {
      get { return 30; }
    }
    public override Image Image {
      get { return Resources.SaveIcon; }
    }

    public override void MainFormInitialized(object sender, EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    public override void ActiveViewChanged(object sender, EventArgs e) {
      IObjectView activeView = MainFormManager.MainForm.ActiveView as IObjectView;
      ToolStripItem.Enabled = ((activeView != null) && (CreatableAttribute.IsCreatable(activeView.Object.GetType())));
    }

    public override void Execute() {
      FileManager.Save();
    }
  }
}
