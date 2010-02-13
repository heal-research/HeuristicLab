using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Netron {
  [Plugin("HeuristicLab.Netron", "3.0.2672.2772")]
  [PluginFile("HeuristicLab.Netron-3.0.2672.12446.dll", PluginFileType.Assembly)]
  [PluginFile("Netron.Diagramming.Core-3.0.2672.12446.dll", PluginFileType.Assembly)]
  [PluginFile("Netron.Diagramming.Core License.txt",PluginFileType.License)]
  public class HeuristicLabNetronPlugin : PluginBase {
  }
}
