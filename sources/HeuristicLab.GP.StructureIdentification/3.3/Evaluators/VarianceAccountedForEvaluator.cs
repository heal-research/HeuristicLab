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

namespace HeuristicLab.GP.StructureIdentification {
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
      AddVariableInfo(new VariableInfo("VAF", "The variance-accounted-for quality of the model", typeof(DoubleData), VariableKind.New));

    }

    public override void Evaluate(IScope scope, ITreeEvaluator evaluator, IFunctionTree tree, Dataset dataset, int targetVariable, int start, int end, bool updateTargetValues) {
      int nSamples = end - start;
      double[] errors = new double[nSamples];
      double[] originalTargetVariableValues = new double[nSamples];
      for (int sample = start; sample < end; sample++) {
        double estimated = evaluator.Evaluate(tree, sample);
        double original = dataset.GetValue(sample, targetVariable);
        if (updateTargetValues) {
          dataset.SetValue(sample, targetVariable, estimated);
        }
        if (!double.IsNaN(original) && !double.IsInfinity(original)) {
          errors[sample - start] = original - estimated;
          originalTargetVariableValues[sample - start] = original;
        } else {
          errors[sample - start] = double.NaN;
          originalTargetVariableValues[sample - start] = double.NaN;
        }
      }
      double errorsVariance = Statistics.Variance(errors);
      double originalsVariance = Statistics.Variance(originalTargetVariableValues);
      double quality = 1 - errorsVariance / originalsVariance;

      if (double.IsNaN(quality) || double.IsInfinity(quality)) {
        quality = double.MaxValue;
      }
      DoubleData vaf = GetVariableValue<DoubleData>("VAF", scope, false, false);
      if (vaf == null) {
        vaf = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("VAF"), vaf));
      }

      vaf.Data = quality;
    }
  }
}
