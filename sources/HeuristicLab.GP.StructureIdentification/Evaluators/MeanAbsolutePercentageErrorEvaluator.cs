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
  public class MeanAbsolutePercentageErrorEvaluator : GPEvaluatorBase {
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'Dataset' and calculates
the 'mean absolute percentage error (scale invariant)' of estimated values vs. real values of 'TargetVariable'.";
      }
    }

    public MeanAbsolutePercentageErrorEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("MAPE", "The mean absolute percentage error of the model", typeof(DoubleData), VariableKind.New));
    }

    public override void Evaluate(IScope scope, BakedTreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end, bool updateTargetValues) {
      double errorsSum = 0.0;
      int n = 0;
      for(int sample = start; sample < end; sample++) {
        double estimated = evaluator.Evaluate(sample);
        double original = dataset.GetValue(targetVariable, sample);

        if(updateTargetValues) {
          dataset.SetValue(targetVariable, sample, estimated);
        }
        
        if(!double.IsNaN(original) && !double.IsInfinity(original) && original != 0.0) {
          double percent_error = Math.Abs((estimated - original) / original);
          errorsSum += percent_error;
          n++;
        }
      }
      double quality = errorsSum / n;
      if(double.IsNaN(quality) || double.IsInfinity(quality))
        quality = double.MaxValue;

      // create a variable for the MAPE
      DoubleData mape = GetVariableValue<DoubleData>("MAPE", scope, false, false);
      if(mape == null) {
        mape = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("MAPE"), mape));
      }

      mape.Data = quality;
    }
  }
}
