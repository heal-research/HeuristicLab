using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimizer.MenuItems {
  class ExportMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Export"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&File" }; }
    }
    public override int Position {
      get { return 1600; }
    }
    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.Shift | Keys.S; }
    }

    protected override void OnToolStripItemSet(EventArgs e) {
      ToolStripItem.Enabled = IsUseable();
    }
    protected override void OnActiveViewChanged(object sender, EventArgs e) {
      ToolStripItem.Enabled = IsUseable();
    }

    private bool IsUseable() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      return (activeView != null) && (activeView.Content != null) &&
             (activeView.Content is IStorableContent) && !activeView.Locked && activeView.Enabled &&
             ToolStripItem.IsOnDropDown && ((ToolStripDropDownItem)ToolStripItem).DropDownItems.Count > 0;
    }

    public override void Execute() { }
  }
}
