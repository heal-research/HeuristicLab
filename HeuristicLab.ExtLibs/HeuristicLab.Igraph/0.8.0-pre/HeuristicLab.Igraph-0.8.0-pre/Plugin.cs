#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.IGraph {
  [Plugin("HeuristicLab.IGraph", "Provides functionality of igraph in HeuristicLab", "0.8.0.0")]
  [PluginFile("HeuristicLab.IGraph-0.8.0-pre.dll", PluginFileType.Assembly)]
  [PluginFile("igraph-0.8.0-pre-x86.dll", PluginFileType.NativeDll)]
  [PluginFile("igraph-0.8.0-pre-x64.dll", PluginFileType.NativeDll)]
  [PluginFile("igraph-0.8.0-pre-license.txt", PluginFileType.License)]
  [PluginFile("igraph_version.txt", PluginFileType.Data)]
  public class HeuristicLabIGraphPlugin : PluginBase {
  }
}
