using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.SQLServerCompact {
  [Plugin("HeuristicLab.SQLServerCompact-3.2")]
  [PluginFile("HeuristicLab.SQLServerCompact-3.2.dll", PluginFileType.Assembly)]
  [PluginFile("System.Data.SqlServerCe.dll", PluginFileType.Assembly)]
  [PluginFile("sqlceca35.dll", PluginFileType.Data)]
  [PluginFile("sqlcecompact35.dll", PluginFileType.Data)]
  [PluginFile("sqlceer35EN.dll", PluginFileType.Data)]
  [PluginFile("sqlceme35.dll", PluginFileType.Data)]
  [PluginFile("sqlceoledb35.dll", PluginFileType.Data)]
  [PluginFile("sqlceqp35.dll", PluginFileType.Data)]
  [PluginFile("sqlcese35.dll", PluginFileType.Data)] 
  public class HeuristicLabSQLServerCompactPlugin : PluginBase {
  }
}
