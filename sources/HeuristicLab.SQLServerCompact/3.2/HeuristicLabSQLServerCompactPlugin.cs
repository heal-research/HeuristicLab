using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.SQLServerCompact {
  [ClassInfo(Name = "HeuristicLab.SQLServerCompact-3.2")]
  [PluginFile(Filename = "HeuristicLab.SQLServerCompact-3.2.dll", Filetype = PluginFileType.Assembly)]
  [PluginFile(Filename = "System.Data.SqlServerCe.dll", Filetype = PluginFileType.Assembly)] 
  public class HeuristicLabSQLServerCompactPlugin : PluginBase {
  }
}
