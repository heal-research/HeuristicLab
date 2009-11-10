using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.SQLServerCompact {
  [ClassInfo(Name = "HeuristicLab.SQLServerCompact-3.2")]
  [PluginFile(Filename = "HeuristicLab.SQLServerCompact-3.2.dll", Filetype = PluginFileType.Assembly)]
  [PluginFile(Filename = "System.Data.SqlServerCe.dll", Filetype = PluginFileType.Assembly)]
  [PluginFile(Filename = "sqlceca35.dll", Filetype = PluginFileType.Data)]
  [PluginFile(Filename = "sqlcecompact35.dll", Filetype = PluginFileType.Data)]
  [PluginFile(Filename = "sqlceer35EN.dll", Filetype = PluginFileType.Data)]
  [PluginFile(Filename = "sqlceme35.dll", Filetype = PluginFileType.Data)]
  [PluginFile(Filename = "sqlceoledb35.dll", Filetype = PluginFileType.Data)]
  [PluginFile(Filename = "sqlceqp35.dll", Filetype = PluginFileType.Data)]
  [PluginFile(Filename = "sqlcese35.dll", Filetype = PluginFileType.Data)] 
  public class HeuristicLabSQLServerCompactPlugin : PluginBase {
  }
}
