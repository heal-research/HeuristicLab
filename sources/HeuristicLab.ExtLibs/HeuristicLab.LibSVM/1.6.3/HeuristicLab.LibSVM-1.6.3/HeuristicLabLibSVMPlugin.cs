using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.LibSVM {
  [Plugin("HeuristicLab.LibSVM-1.6.3")]
  [PluginFile("HeuristicLab.LibSVM-1.6.3.dll", PluginFileType.Assembly)]
  [PluginFile("LibSVM-1.6.3.dll", PluginFileType.Assembly)]
  [PluginFile("LibSVM License.txt",PluginFileType.License)]
  public class HeuristicLabWinFormsUIPlugin : PluginBase {
  }
}
