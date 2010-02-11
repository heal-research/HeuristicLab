using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using System.Windows.Forms;

namespace HeuristicLab.Netron {
  [Application("Netron Test Application 3.3", "A test application for netron visualizations.")]
  internal class NetronTestApplication : ApplicationBase {
    public override void Run() {
      Application.Run(new NetronForm());
    }
  }
}
