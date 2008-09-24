using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Communication.Operators {
  [ClassInfo(Name = "HeuristicLab.Communication.Operators-3.2")]
  [PluginFile(Filename = "HeuristicLab.Communication.Operators-3.2.dll", Filetype = PluginFileType.Assembly)]
  [Dependency(Dependency = "HeuristicLab.Core-3.2")]
  [Dependency(Dependency = "HeuristicLab.Data-3.2")]
  [Dependency(Dependency = "HeuristicLab.Operators-3.2")]
  [Dependency(Dependency = "HeuristicLab.Operators.Programmable-3.2")]
  [Dependency(Dependency = "HeuristicLab.Random-3.2")]
  [Dependency(Dependency = "HeuristicLab.Communication.Data-3.2")]
  [Dependency(Dependency = "HeuristicLab.Constraints-3.2")]
  public class HeuristicLabCommunicationOperatorsPlugin : PluginBase {
  }
}
