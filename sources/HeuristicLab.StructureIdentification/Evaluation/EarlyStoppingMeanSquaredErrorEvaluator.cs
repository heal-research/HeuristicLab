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
        return @"Evaluates 'OperatorTree' for samples 'FirstSampleIndex' - 'LastSampleIndex' (inclusive) and calculates the mean-squared-error
for the estimated values vs. the real values of 'TargetVariable'.
This operator stops the computation as soon as an upper limit for the mean-squared-error is reached.";
      }
    }

    public EarlyStoppingMeanSquaredErrorEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("QualityLimit", "The upper limit of the MSE which is used as early stopping criterion.", typeof(DoubleData), VariableKind.In));
    }

    public override double Evaluate(IScope scope, IFunction function, int targetVariable, Dataset dataset) {
      double qualityLimit = GetVariableValue<DoubleData>("QualityLimit", scope, true).Data;
      double errorsSquaredSum = 0;
      double targetMean = dataset.GetMean(targetVariable);
      for(int sample = 0; sample < dataset.Rows; sample++) {
        double estimated = function.Evaluate(dataset, sample);
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

        // check the limit every 10 samples and stop as soon as we hit the limit
        if(sample % 10 == 9)
          if(qualityLimit < errorsSquaredSum / dataset.Rows ||
            double.IsNaN(errorsSquaredSum) ||
            double.IsInfinity(errorsSquaredSum)) 
            return errorsSquaredSum / sample; // return estimated MSE (when the remaining errors are on average the same)
      }
      errorsSquaredSum /= dataset.Rows;
      if(double.IsNaN(errorsSquaredSum) || double.IsInfinity(errorsSquaredSum)) {
        errorsSquaredSum = double.MaxValue;
      }
      return errorsSquaredSum;
    }
  }
}
