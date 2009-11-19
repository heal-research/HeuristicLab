using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MainForm.Test {
  public class SeparatorMenuItem : MenuSeparatorItemBase, ITestUserInterfaceItemProvider {
    public override IEnumerable<string> Structure {
      get { return new string[] { "File" }; }
    }

    public override int Position {
      get { return 1120; }

    }
  }
}
