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

using HeuristicLab.Core;
using HeuristicLab.Data;
using System;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.DataAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.GP.StructureIdentification {
  public class EarlyStoppingMeanSquaredErrorEvaluator : MeanSquaredErrorEvaluator {
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of the dataset and calculates the mean-squared-error
for the estimated values vs. the real values of 'TargetVariable'.
This operator stops the computation as soon as an upper limit for the mean-squared-error is reached.";
      }
    }

    public EarlyStoppingMeanSquaredErrorEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("QualityLimit", "The upper limit of the MSE which is used as early stopping criterion.", typeof(DoubleData), VariableKind.In));
    }

    // evaluates the function-tree for the given target-variable and the whole dataset and returns the MSE
    public override void Evaluate(IScope scope, IFunctionTree tree, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end) {
      double qualityLimit = GetVariableValue<DoubleData>("QualityLimit", scope, true).Data;
      DoubleData mse = GetVariableValue<DoubleData>("MSE", scope, false, false);
      if (mse == null) {
        mse = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("MSE"), mse));
      }
      double errorsSquaredSum = 0;
      int rows = end - start;
      int n = 0;
      int sample = start;
      foreach (var estimatedValue in evaluator.Evaluate(dataset, tree, Enumerable.Range(start, end - start))) {
        double original = dataset.GetValue(sample, targetVariable);

        if (!double.IsNaN(original) && !double.IsInfinity(original)) {
          double error = estimatedValue - original;
          errorsSquaredSum += error * error;
          n++;
        }
        // check the limit every 30 samples and stop as soon as we hit the limit
        if (n % 30 == 29 && errorsSquaredSum / rows >= qualityLimit) {
          mse.Data = errorsSquaredSum / (n + 1); // return estimated MSE (when the remaining errors are on average the same)
          return;
        }
        sample++;
      }
      errorsSquaredSum /= n;
      if (double.IsNaN(errorsSquaredSum) || double.IsInfinity(errorsSquaredSum)) {
        errorsSquaredSum = double.MaxValue;
      }

      mse.Data = errorsSquaredSum;
    }
  }
}
