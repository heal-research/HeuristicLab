using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Communication.Data {
  [ClassInfo(Name = "HeuristicLab.Communication.Data-3.2")]
  [PluginFile(Filename = "HeuristicLab.Communication.Data-3.2.dll", Filetype = PluginFileType.Assembly)]
  [Dependency(Dependency = "HeuristicLab.Core-3.2")]
  [Dependency(Dependency = "HeuristicLab.Data-3.2")]
  [Dependency(Dependency = "HeuristicLab.Operators.Programmable-3.2")]
  [Dependency(Dependency = "HeuristicLab.Constraints-3.2")]
  public class HeuristicLabCommunicationDataPlugin : PluginBase {
  }
}
