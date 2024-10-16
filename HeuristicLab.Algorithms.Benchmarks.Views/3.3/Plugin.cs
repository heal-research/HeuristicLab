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

namespace HeuristicLab.Algorithms.Benchmarks.Views {
  /// <summary>
  /// Plugin class for HeuristicLab.Algorithms.Benchmarks plugin.
  /// </summary>
  [Plugin("HeuristicLab.Algorithms.Benchmarks.Views", "Provides views for performance benchmark algorithms.", "3.3.16.0")]
  [PluginFile("HeuristicLab.Algorithms.Benchmarks.Views-3.3.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Algorithms.Benchmarks", "3.3")]
  [PluginDependency("HeuristicLab.MainForm", "3.3")]
  [PluginDependency("HeuristicLab.MainForm.WindowsForms", "3.3")]
  [PluginDependency("HeuristicLab.Optimization", "3.3")]
  [PluginDependency("HeuristicLab.Optimization.Views", "3.3")]
  public class HeuristicLabAlgorithmsBenchmarksViewsPlugin : PluginBase {
  }
}
