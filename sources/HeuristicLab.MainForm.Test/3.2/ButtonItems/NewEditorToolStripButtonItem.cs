using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.MainForm.Test {
  public class NewEditorToolStripButtonItem : HeuristicLab.MainForm.WindowsForms.ToolBarItem, ITestUserInterfaceItemProvider {
    public override int Position {
      get { return 12; }
    }

    public override string Name {
      get { return "Editor"; }
    }

    public override IEnumerable<string> Structure {
      get { return new string []{"New"}; }
    }

    public override System.Windows.Forms.ToolStripItemDisplayStyle ToolStripItemDisplayStyle {
      get { return System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText; }
    }

    public override void Execute() {
      NewEditorAction.Execute(MainFormManager.MainForm);
    }
  }
}
