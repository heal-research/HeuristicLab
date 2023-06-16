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

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  /// <summary>
  /// Plugin class for HeuristicLab.Algorithms.DataAnalysis.Views plugin.
  /// </summary>
  [Plugin("HeuristicLab.Algorithms.DataAnalysis.Views", "Provides views for data analysis algorithms implemented in external libraries (linear regression, linear discriminant analysis, k-means clustering, support vector classification and regression)", "3.4.12.0")]
  [PluginFile("HeuristicLab.Algorithms.DataAnalysis.Views-3.4.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Algorithms.DataAnalysis", "3.4")]
  [PluginDependency("HeuristicLab.Analysis", "3.3")]
  [PluginDependency("HeuristicLab.Collections", "3.3")]
  [PluginDependency("HeuristicLab.Common", "3.3")]
  [PluginDependency("HeuristicLab.Common.Resources", "3.3")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Core.Views", "3.3")]
  [PluginDependency("HeuristicLab.Data", "3.3")]
  [PluginDependency("HeuristicLab.Data.Views", "3.3")]
  [PluginDependency("HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "3.4")]
  [PluginDependency("HeuristicLab.LibSVM", "3.12")]
  [PluginDependency("HeuristicLab.MainForm", "3.3")]
  [PluginDependency("HeuristicLab.MainForm.WindowsForms", "3.3")] 
  [PluginDependency("HeuristicLab.Optimization", "3.3")]
  [PluginDependency("HeuristicLab.Optimization.Views", "3.3")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Views", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic.Regression", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic.Classification", "3.4")]
  public class HeuristicLabAlgorithmsDataAnalysisViewsPlugin : PluginBase {
  }
}
