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
using HeuristicLab.DataAnalysis;
using HeuristicLab.Functions;

namespace HeuristicLab.StructureIdentification {
  public class VarianceAccountedForEvaluator : GPEvaluatorBase {
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'DataSet' and calculates 
the variance-accounted-for quality measure for the estimated values vs. the real values of 'TargetVariable'.

The Variance Accounted For (VAF) function is computed as
VAF(y,y') = ( 1 - var(y-y')/var(y) )
where y' denotes the predicted / modelled values for y and var(x) the variance of a signal x.";
      }
    }

    /// <summary>
    /// The Variance Accounted For (VAF) function calculates is computed as
    /// VAF(y,y') = ( 1 - var(y-y')/var(y) )
    /// where y' denotes the predicted / modelled values for y and var(x) the variance of a signal x.
    /// </summary>
    public VarianceAccountedForEvaluator()
      : base() {
    }


    public override double Evaluate(IScope scope, IFunctionTree functionTree, int targetVariable, Dataset dataset) {
      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      double[] errors = new double[trainingEnd-trainingStart];
      double[] originalTargetVariableValues = new double[trainingEnd-trainingStart];
      double targetMean = dataset.GetMean(targetVariable, trainingStart, trainingEnd);
      for(int sample = trainingStart; sample < trainingEnd; sample++) {
        double estimated = evaluator.Evaluate(sample);
        double original = dataset.GetValue(sample, targetVariable);
        if(!double.IsNaN(original) && !double.IsInfinity(original)) {
          if(double.IsNaN(estimated) || double.IsInfinity(estimated))
            estimated = targetMean + maximumPunishment;
          else if(estimated > (targetMean + maximumPunishment))
            estimated = targetMean + maximumPunishment;
          else if(estimated < (targetMean - maximumPunishment))
            estimated = targetMean - maximumPunishment;
        }

        errors[sample-trainingStart] = original - estimated;
        originalTargetVariableValues[sample-trainingStart] = original;
      }
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + treeSize * (trainingEnd-trainingStart);

      double errorsVariance = Statistics.Variance(errors);
      double originalsVariance = Statistics.Variance(originalTargetVariableValues);
      double quality = 1 - errorsVariance / originalsVariance;

      if(double.IsNaN(quality) || double.IsInfinity(quality)) {
        quality = double.MaxValue;
      }
      return quality;
    }
  }
}
