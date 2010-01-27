#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.SQLServerCompact {
  [Plugin("HeuristicLab.SQLServerCompact-3.5.1.0")]
  [PluginFile("HeuristicLab.SQLServerCompact-3.5.1.0.dll", PluginFileType.Assembly)]
  [PluginFile("System.Data.SqlServerCe-3.5.dll", PluginFileType.Assembly)]
  [PluginFile("SQLServer Compact license.txt",PluginFileType.License)]
  [PluginFile("sqlceca35.dll", PluginFileType.NativeDll)]
  [PluginFile("sqlcecompact35.dll", PluginFileType.NativeDll)]
  [PluginFile("sqlceer35EN.dll", PluginFileType.NativeDll)]
  [PluginFile("sqlceme35.dll", PluginFileType.NativeDll)]
  [PluginFile("sqlceoledb35.dll", PluginFileType.NativeDll)]
  [PluginFile("sqlceqp35.dll", PluginFileType.NativeDll)]
  [PluginFile("sqlcese35.dll", PluginFileType.NativeDll)] 
  public class HeuristicLabSQLServerCompactPlugin : PluginBase {
  }
}
