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
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.GP.StructureIdentification.Classification {
  public class ClassificationMeanSquaredErrorEvaluator : GPClassificationEvaluatorBase {
    private const double EPSILON = 1.0E-7;
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'DataSet' and calculates the mean-squared-error
for the estimated values vs. the real values of 'TargetVariable'.";
      }
    }

    public ClassificationMeanSquaredErrorEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("MSE", "The mean squared error of the model", typeof(DoubleData), VariableKind.New));
    }

    public override void Evaluate(IScope scope, ITreeEvaluator evaluator, HeuristicLab.DataAnalysis.Dataset dataset, int targetVariable, double[] classes, double[] thresholds, int start, int end) {
      double errorsSquaredSum = 0;
      for (int sample = start; sample < end; sample++) {
        double estimated = evaluator.Evaluate(sample);
        double original = dataset.GetValue(sample, targetVariable);
        if (!double.IsNaN(original) && !double.IsInfinity(original)) {
          double error = estimated - original;
          // between classes use squared error
          // on the lower end and upper end only add linear error if the absolute error is larger than 1
          // the error>1.0 constraint is needed for balance because in the interval ]-1, 1[ the squared error is smaller than the absolute error
          if ((IsEqual(original, classes[0]) && error < -1.0) ||
            (IsEqual(original, classes[classes.Length - 1]) && error > 1.0)) {
            errorsSquaredSum += Math.Abs(error); // only add linear error below the smallest class or above the largest class
          } else {
            errorsSquaredSum += error * error;
          }
        }
      }

      errorsSquaredSum /= (end - start);
      if (double.IsNaN(errorsSquaredSum) || double.IsInfinity(errorsSquaredSum)) {
        errorsSquaredSum = double.MaxValue;
      }

      DoubleData mse = GetVariableValue<DoubleData>("MSE", scope, false, false);
      if (mse == null) {
        mse = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("MSE"), mse));
      }

      mse.Data = errorsSquaredSum;
    }

    private bool IsEqual(double x, double y) {
      return Math.Abs(x - y) < EPSILON;
    }
  }
}
