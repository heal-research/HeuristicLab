using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Optimizer;

namespace HeuristicLab.JsonInterface.OptimizerIntegration.MenuItems {
  internal class ImportJsonTemplateMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "Json-Template..."; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&File", "&Import" }; }
    }
    public override int Position {
      get { return 1501; }
    }


    public override void Execute() {
      FileManager.ImportJsonTemplate();
    }
  }
}
