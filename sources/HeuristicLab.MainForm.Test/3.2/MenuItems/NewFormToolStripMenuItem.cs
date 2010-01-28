using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MainForm.Test {
  public class NewFormToolStripMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "Form"; }
    }

    public override IEnumerable<string> Structure {
      get { return "File/New".Split('/'); }
    }

    public override int Position {
      get { return 1110; }
    
    }

    public override void Execute() {
      new NewFormAction().Execute(MainFormManager.MainForm);
    }
  }
}
