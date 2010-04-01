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
using System.Linq;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Operators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Evaluators;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SymbolicRegressionMeanSquaredErrorEvaluator", "Calculates the mean squared error of a symbolic regression solution.")]
  [StorableClass]
  public class SymbolicRegressionMeanSquaredErrorEvaluator : SymbolicRegressionEvaluator {
    protected override double Evaluate(SymbolicExpressionTree solution, Dataset dataset, StringValue targetVariable, IntValue samplesStart, IntValue samplesEnd, DoubleValue numberOfEvaluatedNodes) {
      double mse = Apply(solution, dataset, targetVariable.Value, samplesStart.Value, samplesEnd.Value);
      numberOfEvaluatedNodes.Value += solution.Size * (samplesEnd.Value - samplesStart.Value);
      return mse;
    }

    public static double Apply(SymbolicExpressionTree solution, Dataset dataset, string targetVariable, int start, int end) {
      SimpleArithmeticExpressionEvaluator evaluator = new SimpleArithmeticExpressionEvaluator();
      var estimatedValues = evaluator.EstimatedValues(solution, dataset, Enumerable.Range(start, end - start));
      var originalValues = dataset.VariableValues(targetVariable, start, end);
      var values = new DoubleMatrix(MatrixExtensions<double>.Create(estimatedValues.ToArray(), originalValues));
      return SimpleMSEEvaluator.Calculate(values);
    }
  }
}
