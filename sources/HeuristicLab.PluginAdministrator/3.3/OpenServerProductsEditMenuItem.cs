using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm;

namespace HeuristicLab.DeploymentService.AdminClient {
  class OpenServerProductsEditorMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IUserInterfaceItemProvider {
    public override string Name {
      get { return "Edit Products"; }
    }

    public override IEnumerable<string> Structure {
      get { return new string[] { "Plugin Server" }; }
    }

    public override int Position {
      get { return 1300; }
    }

    public override void Execute() {
      OpenProductEditorAction.Execute(MainFormManager.MainForm);
    }
  }
}