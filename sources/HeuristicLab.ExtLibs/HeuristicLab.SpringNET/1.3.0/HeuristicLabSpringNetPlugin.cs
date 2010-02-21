using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.SpringNET {
  [Plugin("HeuristicLab.SpringNET", "1.3.0")]
  [PluginFile("HeuristicLab.SpringNET-1.3.0.dll", PluginFileType.Assembly)]
  [PluginFile("Common.Logging.dll", PluginFileType.Assembly)]
  [PluginFile("Spring.Services.dll", PluginFileType.Assembly)]
  [PluginFile("Spring.Core.dll", PluginFileType.Assembly)]
  [PluginFile("Spring.Aop.dll", PluginFileType.Assembly)]  
  public class HeuristicLabSpringNetPlugin: PluginBase {

  }
}