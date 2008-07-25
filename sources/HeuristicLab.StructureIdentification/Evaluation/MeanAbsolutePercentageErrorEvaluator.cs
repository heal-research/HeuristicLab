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
  public class MeanAbsolutePercentageErrorEvaluator : GPEvaluatorBase {
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'Dataset' and calculates
the 'mean absolute percentage error (scale invariant)' of estimated values vs. real values of 'TargetVariable'.";
      }
    }

    public MeanAbsolutePercentageErrorEvaluator()
      : base() {
    }

    public override double Evaluate(IScope scope, IFunctionTree functionTree, int targetVariable, Dataset dataset) {
      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      double errorsSum = 0.0;
      functionTree.PrepareEvaluation(dataset);
      for(int sample = trainingStart; sample < trainingEnd; sample++) {
        double estimated = functionTree.Evaluate(sample);
        double original = dataset.GetValue(sample, targetVariable);
        if(!double.IsNaN(original) && !double.IsInfinity(original)) {
          if(double.IsNaN(estimated) || double.IsInfinity(estimated))
            estimated = maximumPunishment;
          else if(estimated > maximumPunishment)
            estimated = maximumPunishment;
          else if(estimated < -maximumPunishment)
            estimated = -maximumPunishment;

          double percent_error = Math.Abs((estimated - original) / original);
          errorsSum += percent_error;
        }
      }
      int nSamples = trainingEnd - trainingStart;
      double quality = errorsSum / nSamples;
      if(double.IsNaN(quality) || double.IsInfinity(quality))
        quality = double.MaxValue;
      return quality;
    }
  }
}
