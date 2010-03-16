using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm;

namespace HeuristicLab.DeploymentService.AdminClient {
  class EditConnectionMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IUserInterfaceItemProvider {
    public override string Name {
      get { return "Edit Connection Properties"; }
    }

    public override IEnumerable<string> Structure {
      get { return new string[] { "Plugin Server" }; }
    }

    public override int Position {
      get { return 1300; }
    }

    public override void Execute() {
      EditConnectionAction.Execute(MainFormManager.MainForm);
    }
  }
}