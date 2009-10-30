using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MainForm.Test {
  public class OpenToolStripMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItemBase, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "Open"; }
    }

    public override IEnumerable<string> Structure {
      get { return new string[] { "File" }; }
    }

    public override int Position {
      get { return 1200; }
    }

    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.O; }
    }

    public override System.Drawing.Image Image {
      get { return Resources.OpenIcon; }
    }

    public override void Execute() {
      new OpenAction().Execute(MainFormManager.MainForm);
    }
  }
}
