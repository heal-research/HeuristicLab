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
  internal class SaveAllToolBarItem : HeuristicLab.MainForm.WindowsForms.ToolBarItemBase, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Save All"; }
    }
    public override string ToolTipText {
      get { return "Save All Files"; }
    }
    public override int Position {
      get { return 40; }
    }
    public override Image Image {
      get { return Resources.SaveAllIcon; }
    }

    public override void MainFormInitialized(object sender, EventArgs e) {
      ToolStripItem.Enabled = false;
    }
    public override void ActiveViewChanged(object sender, EventArgs e) {
      var views = from v in MainFormManager.MainForm.Views
                  where v is IItemView
                  where CreatableAttribute.IsCreatable(((IItemView)v).Item.GetType())
                  select v;
      ToolStripItem.Enabled = views.FirstOrDefault() != null;
    }

    public override void Execute() {
      FileManager.SaveAll();
    }
  }
}
