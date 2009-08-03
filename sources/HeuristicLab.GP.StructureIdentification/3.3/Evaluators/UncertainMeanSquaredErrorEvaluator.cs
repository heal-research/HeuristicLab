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
using HeuristicLab.Random;

namespace HeuristicLab.GP.StructureIdentification {
  public class UncertainMeanSquaredErrorEvaluator : MeanSquaredErrorEvaluator {
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of the dataset and calculates the mean-squared-error
for the estimated values vs. the real values of 'TargetVariable'.
This operator stops the computation as soon as an upper limit for the mean-squared-error is reached.";
      }
    }

    public UncertainMeanSquaredErrorEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinEvaluatedSamples", "", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("QualityLimit", "The upper limit of the MSE which is used as early stopping criterion.", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("ConfidenceBounds", "Confidence bounds of the calculated MSE", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("ActuallyEvaluatedSamples", "", typeof(IntData), VariableKind.New | VariableKind.Out));
    }

    // evaluates the function-tree for the given target-variable and the whole dataset and returns the MSE
    public override void Evaluate(IScope scope, ITreeEvaluator evaluator, HeuristicLab.DataAnalysis.Dataset dataset, int targetVariable, int start, int end, bool updateTargetValues) {
      double qualityLimit = GetVariableValue<DoubleData>("QualityLimit", scope, true).Data;
      int minSamples = GetVariableValue<IntData>("MinEvaluatedSamples", scope, true).Data;
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      DoubleData mse = GetVariableValue<DoubleData>("MSE", scope, false, false);
      if (mse == null) {
        mse = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("MSE"), mse));
      }
      DoubleData confidenceBounds = GetVariableValue<DoubleData>("ConfidenceBounds", scope, false, false);
      if (confidenceBounds == null) {
        confidenceBounds = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("ConfidenceBounds"), confidenceBounds));
      }
      IntData evaluatedSamples = GetVariableValue<IntData>("ActuallyEvaluatedSamples", scope, false, false);
      if (evaluatedSamples == null) {
        evaluatedSamples = new IntData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("ActuallyEvaluatedSamples"), evaluatedSamples));
      }

      int rows = end - start;
      double mean = 0;
      double stdDev = 0;
      double confidenceInterval = 0;
      double m2 = 0;
      int[] indexes = InitIndexes(mt, start, end);
      int n = 0;
      for (int sample = 0; sample < rows; sample++) {
        double estimated = evaluator.Evaluate(indexes[sample]);
        double original = dataset.GetValue(indexes[sample], targetVariable);
        if (!double.IsNaN(original) && !double.IsInfinity(original)) {
          n++;
          double error = estimated - original;
          double squaredError = error * error;
          double delta = squaredError - mean;
          mean = mean + delta / n;
          m2 = m2 + delta * (squaredError - mean);

          if (n > minSamples && n % minSamples == 0) {
            stdDev = Math.Sqrt(Math.Sqrt(m2 / (n - 1)));
            confidenceInterval = 2.364 * stdDev / Math.Sqrt(n);
            if (qualityLimit < mean - confidenceInterval || qualityLimit > mean + confidenceInterval) {
              break;
            }
          }
        }
      }

      evaluatedSamples.Data = n;
      mse.Data = mean;
      stdDev = Math.Sqrt(Math.Sqrt(m2 / (n - 1)));
      confidenceBounds.Data = 2.364 * stdDev / Math.Sqrt(n);
    }

    private int[] InitIndexes(MersenneTwister mt, int start, int end) {
      int n = end - start;
      int[] indexes = new int[n];
      for (int i = 0; i < n; i++) indexes[i] = i + start;
      for (int i = 0; i < n - 1; i++) {
        int j = mt.Next(i, n);
        int tmp = indexes[j];
        indexes[j] = indexes[i];
        indexes[i] = tmp;
      }
      return indexes;
    }
  }
}
