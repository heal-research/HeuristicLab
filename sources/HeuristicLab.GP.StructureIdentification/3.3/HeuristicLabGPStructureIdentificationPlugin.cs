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

namespace HeuristicLab.GP.StructureIdentification {
  [ClassInfo(Name = "HeuristicLab.GP.StructureIdentification-3.3")]
  [PluginFile(Filename = "HeuristicLab.GP.StructureIdentification-3.3.dll", Filetype = PluginFileType.Assembly)]
  [Dependency(Dependency = "HeuristicLab.Constraints-3.2")]
  [Dependency(Dependency = "HeuristicLab.Core-3.2")]
  [Dependency(Dependency = "HeuristicLab.Data-3.2")]
  [Dependency(Dependency = "HeuristicLab.DataAnalysis-3.2")]
  [Dependency(Dependency = "HeuristicLab.Evolutionary-3.2")]
  [Dependency(Dependency = "HeuristicLab.GP-3.3")]
  [Dependency(Dependency = "HeuristicLab.Logging-3.2")]
  [Dependency(Dependency = "HeuristicLab.Operators-3.2")]
  [Dependency(Dependency = "HeuristicLab.Operators.Programmable-3.2")]
  [Dependency(Dependency = "HeuristicLab.Random-3.2")]
  [Dependency(Dependency = "HeuristicLab.Selection-3.2")]
  [Dependency(Dependency = "HeuristicLab.Selection.OffspringSelection-3.2")]
  [Dependency(Dependency = "HeuristicLab.SequentialEngine-3.2")]
  [Dependency(Dependency = "HeuristicLab.Modeling-3.2")]
  public class HeuristicLabGPStructureIdentificationPlugin : PluginBase {
  }
}
