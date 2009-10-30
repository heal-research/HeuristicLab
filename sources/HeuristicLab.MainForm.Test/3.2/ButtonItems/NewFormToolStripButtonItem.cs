using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.MainForm.Test {
  public class NewFormToolStripButtonItem : HeuristicLab.MainForm.WindowsForms.ToolBarItemBase, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 11; }
    }

    public override string Name {
      get { return "Form"; }
    }

    public override IEnumerable<string> Structure {
      get { return new string[] { "New" }; }
    }

    public override System.Windows.Forms.ToolStripItemDisplayStyle ToolStripItemDisplayStyle {
      get { return System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText; }
    }

    public override void Execute() {
      new NewFormAction().Execute(MainFormManager.MainForm);
    }
  }
}
