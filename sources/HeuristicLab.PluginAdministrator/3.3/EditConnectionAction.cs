using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm;

namespace HeuristicLab.DeploymentService.AdminClient {
  static class EditConnectionAction {
    internal static void Execute(IMainForm mainForm) {
      ConnectionSetupView connectionSetup = new ConnectionSetupView();
      connectionSetup.Show();
    }
  }
}
