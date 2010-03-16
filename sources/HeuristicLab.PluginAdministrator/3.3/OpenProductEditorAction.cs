using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm;

namespace HeuristicLab.DeploymentService.AdminClient {
  static class OpenProductEditorAction {
    internal static void Execute(IMainForm mainForm) {
      ProductEditor editor = new ProductEditor();
      editor.Show();
    }
  }
}
