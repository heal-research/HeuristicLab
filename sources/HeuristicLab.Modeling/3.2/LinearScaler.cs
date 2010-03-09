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
using HeuristicLab.Core;
using HeuristicLab.Data;
using System.Linq;

namespace HeuristicLab.Modeling {
  public class LinearScaler : OperatorBase {
    public LinearScaler()
      : base() {
      AddVariableInfo(new VariableInfo("Values", "Matrix of predicted and original values that should be scaled", typeof(DoubleMatrixData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Alpha", "Maximization problem", typeof(BoolData), VariableKind.New | VariableKind.In));
      AddVariableInfo(new VariableInfo("Beta", "The best solution of the run", typeof(IScope), VariableKind.New | VariableKind.In));
      AddVariableInfo(new VariableInfo("UpperEstimationLimit", "Upper limit for estimated value (optional)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("LowerEstimationLimit", "Lower limit for estimated value (optional)", typeof(DoubleData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      double[,] values = GetVariableValue<DoubleMatrixData>("Values", scope, true).Data;
      DoubleData alpha = GetVariableValue<DoubleData>("Alpha", scope, true, false);
      DoubleData beta = GetVariableValue<DoubleData>("Beta", scope, true, false);
      DoubleData lowerEstimationLimit = GetVariableValue<DoubleData>("LowerEstimationLimit", scope, true, false);
      DoubleData upperEstimationLimit = GetVariableValue<DoubleData>("UpperEstimationLimit", scope, true, false);

      double a, b;
      if (alpha == null || beta == null) {
        // one of the variables is missing -> recalculate 
        CalculateScalingParameters(values, out b, out a);
        scope.AddVariable(new Variable(scope.TranslateName("Alpha"), new DoubleData(a)));
        scope.AddVariable(new Variable(scope.TranslateName("Beta"), new DoubleData(b)));
      } else {
        // both variables are set already
        a = alpha.Data;
        b = beta.Data;
      }
      // check if upper and lower limit are set and use -Inf and +Inf as a default
      double lowerLimit = lowerEstimationLimit == null ? double.NegativeInfinity : lowerEstimationLimit.Data;
      double upperLimit = upperEstimationLimit == null ? double.PositiveInfinity : upperEstimationLimit.Data;
      // apply scaling
      Scale(values, a, b, lowerLimit, upperLimit);

      return null;
    }


    public static void CalculateScalingParameters(double[,] values, out double beta, out double alpha) {
      var result = from row in Enumerable.Range(0, values.GetLength(0))
                   let t = values[row, SimpleEvaluatorBase.ORIGINAL_INDEX]
                   let e = values[row, SimpleEvaluatorBase.ESTIMATION_INDEX]
                   select new { Estimation = e, Target = t };

      // calculate alpha and beta on the subset of rows with valid values 
      var filteredResult = result.Where(x => IsValidValue(x.Target) && IsValidValue(x.Estimation));
      var target = filteredResult.Select(x => x.Target);
      var estimation = filteredResult.Select(x => x.Estimation);
      double tMean = target.Sum() / target.Count();
      double xMean = estimation.Sum() / estimation.Count();
      double sumXT = 0;
      double sumXX = 0;
      foreach (var r in result) {
        double x = r.Estimation;
        double t = r.Target;
        sumXT += (x - xMean) * (t - tMean);
        sumXX += (x - xMean) * (x - xMean);
      }
      if (sumXX != 0) {
        beta = sumXT / sumXX;
      } else {
        beta = 1;
      }
      alpha = tMean - beta * xMean;
    }

    public static void Scale(double[,] values, double alpha, double beta, double lowerEstimationLimit, double upperEstimationLimit) {
      int rows = values.GetLength(0);
      for (int row = 0; row < rows; row++) {
        double estimatedValue = values[row, SimpleEvaluatorBase.ESTIMATION_INDEX];
        if (double.IsNaN(estimatedValue))
          estimatedValue = upperEstimationLimit;
        values[row, SimpleEvaluatorBase.ESTIMATION_INDEX] =
          Math.Min(Math.Max(estimatedValue * beta + alpha, lowerEstimationLimit), upperEstimationLimit); ;
      }
    }

    private static bool IsValidValue(double d) {
      return !double.IsInfinity(d) && !double.IsNaN(d);
    }
  }
}
