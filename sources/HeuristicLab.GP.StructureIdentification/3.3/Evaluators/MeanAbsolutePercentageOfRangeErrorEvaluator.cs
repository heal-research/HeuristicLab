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
  public class MeanAbsolutePercentageOfRangeErrorEvaluator : GPEvaluatorBase {
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'Dataset' and calculates
the mean of the absolute percentage error (scale invariant) relative to the range of the target varibale of estimated values vs. real values of 'TargetVariable'.";
      }
    }

    public MeanAbsolutePercentageOfRangeErrorEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("MAPRE", "The mean absolute percentage range error of the model", typeof(DoubleData), VariableKind.New));
    }

    public override void Evaluate(IScope scope, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end, bool updateTargetValues) {
      double errorsSum = 0.0;
      int n = 0;
      double range = dataset.GetRange(targetVariable, start, end);
      for (int sample = start; sample < end; sample++) {
        double estimated = evaluator.Evaluate(sample);
        double original = dataset.GetValue(sample, targetVariable);

        if (updateTargetValues) {
          dataset.SetValue(sample, targetVariable, estimated);
        }

        if (!double.IsNaN(original) && !double.IsInfinity(original) && original != 0.0) {
          double percent_error = Math.Abs((estimated - original) / range);
          errorsSum += percent_error;
          n++;
        }
      }
      double quality = errorsSum / n;
      if (double.IsNaN(quality) || double.IsInfinity(quality))
        quality = double.MaxValue;

      // create a variable for the MAPRE
      DoubleData mapre = GetVariableValue<DoubleData>("MAPRE", scope, false, false);
      if (mapre == null) {
        mapre = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("MAPRE"), mapre));
      }

      mapre.Data = quality;
    }
  }
}
