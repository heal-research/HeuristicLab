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
  public class VarianceAccountedForEvaluator : OperatorBase {
    public override string Description {
      get { return @"Evaluates 'OperatorTree' for samples 'FirstSampleIndex' - 'LastSampleIndex' (inclusive) and calculates 
the variance-accounted-for quality measure for the estimated values vs. the real values of 'TargetVariable'.

The Variance Accounted For (VAF) function is computed as
VAF(y,y') = ( 1 - var(y-y')/var(y) )
where y' denotes the predicted / modelled values for y and var(x) the variance of a signal x."; }
    }

    /// <summary>
    /// The Variance Accounted For (VAF) function calculates is computed as
    /// VAF(y,y') = ( 1 - var(y-y')/var(y) )
    /// where y' denotes the predicted / modelled values for y and var(x) the variance of a signal x.
    /// </summary>
    public VarianceAccountedForEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("OperatorTree", "The function tree that should be evaluated", typeof(IFunction), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the target variable in the dataset", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FirstSampleIndex", "Index of the first row of the dataset on which the function should be evaluated", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("LastSampleIndex", "Index of the last row of the dataset on which the function should be evaluated (inclusive)", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("PunishmentFactor", "Punishment factor for invalid estimations", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("UseEstimatedTargetValues", "When the function tree contains the target variable this variable determines " +
      "if we should use the estimated or the original values of the target variable in the evaluation", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Variance accounted for quality of the model", typeof(DoubleData), VariableKind.New));

    }


    private double[] originalTargetVariableValues = new double[1];
    private double[] errors = new double[1];

    public override IOperation Apply(IScope scope) {

      int firstSampleIndex = GetVariableValue<IntData>("FirstSampleIndex", scope, true).Data;
      int lastSampleIndex = GetVariableValue<IntData>("LastSampleIndex", scope, true).Data;

      if(lastSampleIndex < firstSampleIndex) {
        throw new InvalidProgramException();
      }

      IFunction function = GetVariableValue<IFunction>("OperatorTree", scope, true);

      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);

      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      bool useEstimatedTargetValues = GetVariableValue<BoolData>("UseEstimatedTargetValues", scope, true).Data;
      double punishmentFactor = GetVariableValue<DoubleData>("PunishmentFactor", scope, true).Data;

      if(originalTargetVariableValues.Length != lastSampleIndex - firstSampleIndex + 1) {
        originalTargetVariableValues = new double[lastSampleIndex - firstSampleIndex + 1];
        errors = new double[lastSampleIndex - firstSampleIndex + 1];
      }

      double maximumPunishment = punishmentFactor * dataset.GetRange(targetVariable, firstSampleIndex, lastSampleIndex);

      double targetMean = dataset.GetMean(targetVariable, firstSampleIndex, lastSampleIndex);

      for(int sample = firstSampleIndex; sample <= lastSampleIndex; sample++) {

        double estimated = function.Evaluate(dataset, sample);
        double original =  dataset.GetValue(sample, targetVariable);

        if(!double.IsNaN(original) && !double.IsInfinity(original)) {
          if(double.IsNaN(estimated) || double.IsInfinity(estimated))
            estimated = targetMean + maximumPunishment;
          else if(estimated > (targetMean + maximumPunishment))
            estimated = targetMean + maximumPunishment;
          else if(estimated < (targetMean - maximumPunishment))
            estimated = targetMean - maximumPunishment;
        }

        errors[sample-firstSampleIndex] = original - estimated;
        originalTargetVariableValues[sample-firstSampleIndex] = original;
        if(useEstimatedTargetValues) {
          dataset.SetValue(sample, targetVariable, estimated);
        }
      }

      double errorsVariance = Statistics.Variance(errors);
      double originalsVariance = Statistics.Variance(originalTargetVariableValues);
      double quality = 1 - errorsVariance / originalsVariance;

      if(double.IsNaN(quality) || double.IsInfinity(quality)) {
        quality = double.MaxValue;
      }

      if(useEstimatedTargetValues) {
        // restore original values of the target variable
        for(int sample = firstSampleIndex; sample <= lastSampleIndex; sample++) {
          dataset.SetValue(sample, targetVariable, originalTargetVariableValues[sample - firstSampleIndex]);
        }
      }

      scope.AddVariable(new HeuristicLab.Core.Variable("Quality", new DoubleData(quality)));
      return null;
    }
  }
}
