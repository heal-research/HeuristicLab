﻿#region License Information
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

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [Plugin("HeuristicLab.Problems.ExternalEvaluation.GP", "Provides a symbolic expression tree grammar and formatters for external evaluation problems.", "3.5.11.0")]
  [PluginFile("HeuristicLab.Problems.ExternalEvaluation.GP-3.5.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Attic", "1.0")]
  [PluginDependency("HeuristicLab.Common", "3.3")]
  [PluginDependency("HeuristicLab.Core", "3.3")]
  [PluginDependency("HeuristicLab.Encodings.SymbolicExpressionTreeEncoding", "3.4")]
  [PluginDependency("HeuristicLab.Persistence", "3.3")]
  [PluginDependency("HeuristicLab.Problems.DataAnalysis.Symbolic", "3.4")]
  [PluginDependency("HeuristicLab.Problems.ExternalEvaluation", "3.4")]
  [PluginDependency("HeuristicLab.ProtobufCS", "2.4.1.473")]
  public class HeuristicLabProblemsExternalEvaluationGPPlugin : PluginBase {
  }
}
