#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.SA {
  /// <summary>
  /// Plugin class for HeuristicLab.SA plugin.
  /// </summary>
  [ClassInfo(Name = "HeuristicLab.SA-3.2")]
  [PluginFile(Filename = "HeuristicLab.SA-3.2.dll", Filetype = PluginFileType.Assembly)]
  [Dependency(Dependency = "HeuristicLab.Common-3.2")]
  [Dependency(Dependency = "HeuristicLab.Core-3.2")]
  [Dependency(Dependency = "HeuristicLab.Data-3.2")]
  [Dependency(Dependency = "HeuristicLab.Operators-3.2")]
  [Dependency(Dependency = "HeuristicLab.Random-3.2")]
  [Dependency(Dependency = "HeuristicLab.Selection-3.2")]
  [Dependency(Dependency = "HeuristicLab.Logging-3.2")]
  [Dependency(Dependency = "HeuristicLab.SequentialEngine-3.2")]
  [Dependency(Dependency = "HeuristicLab.Selection.OffspringSelection-3.2")]
  [Dependency(Dependency = "HeuristicLab.Evolutionary-3.2")]
  public class HeuristicLabSAPlugin : PluginBase {
  }
}
