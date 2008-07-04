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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Functions;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.StructureIdentification {
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
    public override double Evaluate(IScope scope, IFunctionTree functionTree, int targetVariable, Dataset dataset) {
      double qualityLimit = GetVariableValue<DoubleData>("QualityLimit", scope, false).Data;
      bool useEstimatedValues = GetVariableValue<BoolData>("UseEstimatedTargetValue", scope, false).Data;
      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      int rows = trainingEnd-trainingStart;
      if(useEstimatedValues && backupValues == null) {
        backupValues = new double[rows];
        for(int i = trainingStart; i < trainingEnd; i++) {
          backupValues[i-trainingStart] = dataset.GetValue(i, targetVariable);
        }
      }
      double errorsSquaredSum = 0;
      double targetMean = dataset.GetMean(targetVariable);
      functionTree.PrepareEvaluation(dataset);
      for(int sample = trainingStart; sample < trainingEnd; sample++) {
        double estimated = functionTree.Evaluate(sample);
        double original = dataset.GetValue(sample, targetVariable);
        if(double.IsNaN(estimated) || double.IsInfinity(estimated)) {
          estimated = targetMean + maximumPunishment;
        } else if(estimated > targetMean + maximumPunishment) {
          estimated = targetMean + maximumPunishment;
        } else if(estimated < targetMean - maximumPunishment) {
          estimated = targetMean - maximumPunishment;
        }

        double error = estimated - original;
        errorsSquaredSum += error * error;

        // check the limit and stop as soon as we hit the limit
        if(errorsSquaredSum / rows >= qualityLimit) {
          scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + treeSize * (sample-trainingStart + 1);
          if(useEstimatedValues) RestoreDataset(dataset, targetVariable, trainingStart, sample);
          return errorsSquaredSum / (sample-trainingStart + 1); // return estimated MSE (when the remaining errors are on average the same)
        }
        if(useEstimatedValues) {
          dataset.SetValue(sample, targetVariable, estimated);
        }
      }
      if(useEstimatedValues) RestoreDataset(dataset, targetVariable, trainingStart, trainingEnd);
      errorsSquaredSum /= rows;
      if(double.IsNaN(errorsSquaredSum) || double.IsInfinity(errorsSquaredSum)) {
        errorsSquaredSum = double.MaxValue;
      }
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + treeSize * rows;
      return errorsSquaredSum;
    }

    private void RestoreDataset(Dataset dataset, int targetVariable, int from, int to) {
      for(int i = from; i < to; i++) {
        dataset.SetValue(i, targetVariable, backupValues[i-from]);
      }
    }
  }
}
