using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MainForm.Test {
  public class SaveToolStripMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItemBase, ITestUserInterfaceItemProvider {
    public override string Name {
      get { return "Save"; }
    }

    public override IEnumerable<string> Structure {
      get { return new string[]{"File"}; }
    }

    public override int Position {
      get { return 1300; }
    }

    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.S; }
    }

    public override System.Drawing.Image Image {
      get { return Resources.SaveIcon; }
    }

    public override void Execute() {
      new SaveAction().Execute(MainFormManager.MainForm);
    }
  }
}
