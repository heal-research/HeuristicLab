#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SymbolicRegressionPearsonsRSquaredEvaluator", "Calculates the pearson r² correlation coefficient of a symbolic regression solution.")]
  [StorableClass]
  public class SymbolicRegressionPearsonsRSquaredEvaluator : SingleObjectiveSymbolicRegressionEvaluator {

    public override bool Maximization {
      get { return true; }
    }
    
    [StorableConstructor]
    protected SymbolicRegressionPearsonsRSquaredEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionPearsonsRSquaredEvaluator(SymbolicRegressionPearsonsRSquaredEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicRegressionPearsonsRSquaredEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionPearsonsRSquaredEvaluator(this, cloner);
    }
    public override double Evaluate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows) {
      double mse = Calculate(interpreter, solution, lowerEstimationLimit, upperEstimationLimit, dataset, targetVariable, rows);
      return mse;
    }

    public static double Calculate(ISymbolicExpressionTreeInterpreter interpreter, SymbolicExpressionTree solution, double lowerEstimationLimit, double upperEstimationLimit, Dataset dataset, string targetVariable, IEnumerable<int> rows) {
      IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, dataset, rows);
      IEnumerable<double> originalValues = dataset.GetEnumeratedVariableValues(targetVariable, rows);
      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      OnlinePearsonsRSquaredEvaluator r2Evaluator = new OnlinePearsonsRSquaredEvaluator();

      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        double estimated = estimatedEnumerator.Current;
        double original = originalEnumerator.Current;
        if (double.IsNaN(estimated))
          estimated = upperEstimationLimit;
        else
          estimated = Math.Min(upperEstimationLimit, Math.Max(lowerEstimationLimit, estimated));
        r2Evaluator.Add(original, estimated);
      }

      if (estimatedEnumerator.MoveNext() || originalEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in original and estimated enumeration doesn't match.");
      } else {
        return r2Evaluator.RSquared;
      }
    }
  }
}
