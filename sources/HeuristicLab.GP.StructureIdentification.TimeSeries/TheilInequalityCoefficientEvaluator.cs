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
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  public class TheilInequalityCoefficientEvaluator : GPEvaluatorBase {
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'Dataset' and calculates
the 'Theil inequality coefficient (Theil's U2 not U1!)' of estimated values vs. real values of 'TargetVariable'.

U2 = Sqrt(1/N * Sum(P_t - A_t)^2 ) / Sqrt(1/N * Sum(A_t)^2 ) 

where P_t is the predicted change of the target variable and A_t is the measured (original) change.
(P_t = y'_t - y_(t-1), A_t = y_t - y_(t-1)).

U2 is 0 for a perfect prediction and 1 for the naive model y'_t = y_(t-1). An U2 > 1 means the
model is worse than the naive model (=> model is useless).";
      }
    }

    public TheilInequalityCoefficientEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("TheilInequalityCoefficient", "Theil's inequality coefficient (U2) of the model", typeof(DoubleData), VariableKind.New));
      AddVariableInfo(new VariableInfo("TheilInequalityCoefficientBias", "Bias proportion of Theil's inequality coefficient", typeof(DoubleData), VariableKind.New));
      AddVariableInfo(new VariableInfo("TheilInequalityCoefficientVariance", "Variance proportion of Theil's inequality coefficient", typeof(DoubleData), VariableKind.New));
      AddVariableInfo(new VariableInfo("TheilInequalityCoefficientCovariance", "Covariance proportion of Theil's inequality coefficient", typeof(DoubleData), VariableKind.New));
    }

    public override void Evaluate(IScope scope, BakedTreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end, bool updateTargetValues) {
      #region create result variables
      DoubleData theilInequaliy = GetVariableValue<DoubleData>("TheilInequalityCoefficient", scope, false, false);
      if (theilInequaliy == null) {
        theilInequaliy = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TheilInequalityCoefficient"), theilInequaliy));
      }
      DoubleData uBias = GetVariableValue<DoubleData>("TheilInequalityCoefficientBias", scope, false, false);
      if (uBias == null) {
        uBias = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TheilInequalityCoefficientBias"), uBias));
      }
      DoubleData uVariance = GetVariableValue<DoubleData>("TheilInequalityCoefficientVariance", scope, false, false);
      if (uVariance == null) {
        uVariance = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TheilInequalityCoefficientVariance"), uVariance));
      }
      DoubleData uCovariance = GetVariableValue<DoubleData>("TheilInequalityCoefficientCovariance", scope, false, false);
      if (uCovariance == null) {
        uCovariance = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TheilInequalityCoefficientCovariance"), uCovariance));
      }
      #endregion

      double errorsSquaredSum = 0.0;
      double originalSquaredSum = 0.0;
      double[] estimatedChanges = new double[end - start];
      double[] originalChanges = new double[end - start];
      int nSamples = 0;
      for (int sample = start; sample < end; sample++) {
        double prevValue = dataset.GetValue(sample - 1, targetVariable);
        double estimatedChange = evaluator.Evaluate(sample) - prevValue;
        double originalChange = dataset.GetValue(sample, targetVariable) - prevValue;
        if (updateTargetValues) {
          dataset.SetValue(sample, targetVariable, estimatedChange + prevValue);
        }
        if (!double.IsNaN(originalChange) && !double.IsInfinity(originalChange)) {
          double error = estimatedChange - originalChange;
          errorsSquaredSum += error * error;
          originalSquaredSum += originalChange * originalChange;
          estimatedChanges[sample - start] = estimatedChange;
          originalChanges[sample - start] = originalChange;
          nSamples++;
        }
      }
      double quality = Math.Sqrt(errorsSquaredSum / nSamples) / Math.Sqrt(originalSquaredSum / nSamples);
      if (double.IsNaN(quality) || double.IsInfinity(quality))
        quality = double.MaxValue;
      theilInequaliy.Data = quality; // U2

      // decomposition into U_bias + U_variance + U_covariance parts
      double bias = Statistics.Mean(estimatedChanges) - Statistics.Mean(originalChanges);
      bias *= bias; // squared
      uBias.Data = bias / (errorsSquaredSum / nSamples);

      double variance = Statistics.StandardDeviation(estimatedChanges) - Statistics.StandardDeviation(originalChanges);
      variance *= variance; // squared
      uVariance.Data = variance / (errorsSquaredSum / nSamples);

      // all parts add up to one so I don't have to calculate the correlation coefficient for the covariance propotion
      uCovariance.Data = 1.0 - uBias.Data - uVariance.Data;
    }
  }
}
