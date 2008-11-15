using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Hive.Client.ExecutionEngine {
  [ClassInfo(Name = "HeuristicLab.Hive.Client.ExecutionEngine-3.2")]
  [PluginFile(Filename = "HeuristicLab.Hive.Client.ExecutionEngine-3.2.dll", Filetype = PluginFileType.Assembly)]
  [Dependency(Dependency = "HeuristicLab.Hive.Client.Core-3.2")]
  [Dependency(Dependency = "HeuristicLab.Core-3.2")]
  public class ExecutionEnginePlugin: PluginBase {
  }
}
