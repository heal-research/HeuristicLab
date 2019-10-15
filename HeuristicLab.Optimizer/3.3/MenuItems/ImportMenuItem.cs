using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimizer.MenuItems {
  internal class ImportMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Import"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&File" }; }
    }
    public override int Position {
      get { return 1500; }
    }
    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.Shift | Keys.S; }
    }

    protected override void OnToolStripItemSet(EventArgs e) =>
      ToolStripItem.Enabled = IsUseable();
    
    protected override void OnActiveViewChanged(object sender, EventArgs e) =>
      ToolStripItem.Enabled = IsUseable();

    private bool IsUseable() => ToolStripItem.IsOnDropDown && 
      ((ToolStripDropDownItem)ToolStripItem).DropDownItems.Count > 0;

    public override void Execute() {}
  }
}
