#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SymbolicRegressionNormalizedMeanSquaredErrorEvaluator", "Calculates the normalized mean squared error of a symbolic regression solution.")]
  [StorableClass]
  public class SymbolicRegressionNormalizedMeanSquaredErrorEvaluator : SingleObjectiveSymbolicRegressionEvaluator {
    public SymbolicRegressionNormalizedMeanSquaredErrorEvaluator()
      : base() {
    }

    public override double Evaluate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows) {
      double nmse = Calculate(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows);
      return nmse;
    }

    public static double Calculate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows) {
      return SymbolicRegressionScaledNormalizedMeanSquaredErrorEvaluator.CalculateWithScaling(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows, 1.0, 0.0);
    }
  }
}
