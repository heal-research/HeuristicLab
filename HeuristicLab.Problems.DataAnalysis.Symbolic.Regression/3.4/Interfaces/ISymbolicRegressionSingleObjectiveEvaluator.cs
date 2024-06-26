﻿using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("5dd2601a-b884-48c0-85bc-bc1f437187a3")]
  public interface ISymbolicRegressionSingleObjectiveEvaluator : ISymbolicRegressionEvaluator, ISymbolicDataAnalysisSingleObjectiveEvaluator<IRegressionProblemData> {
    double Evaluate(
      ISymbolicExpressionTree tree,
      IRegressionProblemData problemData,
      IEnumerable<int> rows,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      bool applyLinearScaling = true,
      double lowerEstimationLimit = double.MinValue, 
      double upperEstimationLimit = double.MaxValue);
  }
}
