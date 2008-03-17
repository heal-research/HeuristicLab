using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.ES {
  [ClassInfo(Name = "HeuristicLab.ES")]
  [PluginFile(Filename = "HeuristicLab.ES.dll", Filetype = PluginFileType.Assembly)]
  [Dependency(Dependency = "HeuristicLab.Core")]
  [Dependency(Dependency = "HeuristicLab.Data")]
  [Dependency(Dependency = "HeuristicLab.Evolutionary")]
  [Dependency(Dependency = "HeuristicLab.Operators")]
  [Dependency(Dependency = "HeuristicLab.Random")]
  [Dependency(Dependency = "HeuristicLab.Selection")]
  [Dependency(Dependency = "HeuristicLab.Logging")]
  [Dependency(Dependency = "HeuristicLab.SequentialEngine")]
  public class HeuristicLabESPlugin : PluginBase {
  }
}
