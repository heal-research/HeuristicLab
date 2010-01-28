using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.Common.Resources;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.MainForm.Test {
  public class NewEditorToolStripMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "Editor"; }
    }

    public override IEnumerable<string> Structure {
      get { return "File/New".Split('/'); }
    }

    public override int Position {
      get { return 1120; }
    }

    public override void Execute() {
      NewEditorAction.Execute(MainFormManager.MainForm);
    }
  }
}
