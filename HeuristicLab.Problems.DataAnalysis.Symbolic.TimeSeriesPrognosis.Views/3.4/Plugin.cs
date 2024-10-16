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

using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis.Views {
  [Plugin("HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis.Views", "Provides views for symbolic time-series prognosis problem classes.", "3.4.12.0")]
  [PluginFile("HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis.Views-3.4.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Common.Resources", "3.3")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Core.Views", "3.3")]
  [PluginDependency("HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "3.4")]
  [PluginDependency("HeuristicLab.MainForm", "3.3")]
  [PluginDependency("HeuristicLab.MainForm.WindowsForms", "3.3")]
  [PluginDependency("HeuristicLab.Optimization","3.3")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic.Regression", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic.Views", "3.4")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Views", "3.4")]
  public class HeuristicLabProblemsDataAnalysisSymbolicTimeSeriesPrognosisViewsPlugin : PluginBase {
  }
}
