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
  public class TheilInequalityCoefficientEvaluator : GPEvaluatorBase {
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'Dataset' and calculates
the 'Theil inequality coefficient (scale invariant)' of estimated values vs. real values of 'TargetVariable'.";
      }
    }

    public TheilInequalityCoefficientEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Differential", "Wether to calculate the coefficient for the predicted change vs. original change or for the absolute prediction vs. original value", typeof(BoolData), VariableKind.In));
    }

    public override double Evaluate(IScope scope, IFunctionTree functionTree, int targetVariable, Dataset dataset) {
      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      bool difference = GetVariableValue<BoolData>("Differential", scope, true).Data;
      double errorsSquaredSum = 0.0;
      double estimatedSquaredSum = 0.0;
      double originalSquaredSum = 0.0;
      for(int sample = trainingStart; sample < trainingEnd; sample++) {
        double prevValue = 0.0;
        if(difference) prevValue = dataset.GetValue(sample - 1, targetVariable);
        double estimatedChange = evaluator.Evaluate(sample) - prevValue;
        double originalChange = dataset.GetValue(sample, targetVariable) - prevValue;
        if(!double.IsNaN(originalChange) && !double.IsInfinity(originalChange)) {
          if(double.IsNaN(estimatedChange) || double.IsInfinity(estimatedChange))
            estimatedChange = maximumPunishment;
          else if(estimatedChange > maximumPunishment)
            estimatedChange = maximumPunishment;
          else if(estimatedChange < -maximumPunishment)
            estimatedChange = - maximumPunishment;

          double error = estimatedChange - originalChange;
          errorsSquaredSum += error * error;
          estimatedSquaredSum += estimatedChange * estimatedChange;
          originalSquaredSum += originalChange * originalChange;
        }
      }
      int nSamples = trainingEnd - trainingStart;
      double quality = Math.Sqrt(errorsSquaredSum / nSamples) / (Math.Sqrt(estimatedSquaredSum/nSamples) + Math.Sqrt(originalSquaredSum/nSamples));
      if(double.IsNaN(quality) || double.IsInfinity(quality)) 
        quality = double.MaxValue;
      return quality;
    }
  }
}
