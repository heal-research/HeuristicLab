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
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Logging {
  /// <summary>
  /// Plugin class for HeuristicLab.Logging plugin.
  /// </summary>
  [ClassInfo(Name = "HeuristicLab.Logging-3.2")]
  [PluginFile(Filename = "HeuristicLab.Logging-3.2.dll", Filetype = PluginFileType.Assembly)]
  [Dependency(Dependency = "HeuristicLab.Charting-3.2")]
  [Dependency(Dependency = "HeuristicLab.Charting.Data-3.2")]
  [Dependency(Dependency = "HeuristicLab.Common-3.2")]
  [Dependency(Dependency = "HeuristicLab.Core-3.2")]
  [Dependency(Dependency = "HeuristicLab.Data-3.2")]
  [Dependency(Dependency = "HeuristicLab.Operators-3.2")]
  public class HeuristicLabLoggingPlugin : PluginBase {
  }
}
