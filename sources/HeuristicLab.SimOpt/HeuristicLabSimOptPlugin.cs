using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.SimOpt {
  [ClassInfo(Name = "HeuristicLab.SimOpt-3.2")]
  [PluginFile(Filename = "HeuristicLab.SimOpt-3.2.dll", Filetype = PluginFileType.Assembly)]
  [Dependency(Dependency = "HeuristicLab.Core-3.2")]
  [Dependency(Dependency = "HeuristicLab.Data-3.2")]
  [Dependency(Dependency = "HeuristicLab.Operators-3.2")]
  [Dependency(Dependency = "HeuristicLab.Random-3.2")]
  [Dependency(Dependency = "HeuristicLab.Constraints-3.2")]
  [Dependency(Dependency = "HeuristicLab.Evolutionary-3.2")]
  [Dependency(Dependency = "HeuristicLab.Permutation-3.2")]
  public class HeuristicLabSimOptPlugin : PluginBase {
  }
}
