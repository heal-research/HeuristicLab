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

namespace HeuristicLab.GP.StructureIdentification {
  public class MeanSquaredErrorEvaluator : GPEvaluatorBase {
    protected DoubleData mse;
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'DataSet' and calculates the mean-squared-error
for the estimated values vs. the real values of 'TargetVariable'.";
      }
    }

    public MeanSquaredErrorEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("MSE", "The mean squared error of the model", typeof(DoubleData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      mse = GetVariableValue<DoubleData>("MSE", scope, false, false);
      if(mse == null) {
        mse = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("MSE"), mse));
      }

      return base.Apply(scope);
    }

    public override void Evaluate(int start, int end) {
      double errorsSquaredSum = 0;
      for(int sample = start; sample < end; sample++) {
        double original = GetOriginalValue(sample);
        double estimated = GetEstimatedValue(sample);
        SetOriginalValue(sample, estimated);
        if(!double.IsNaN(original) && !double.IsInfinity(original)) {
          double error = estimated - original;
          errorsSquaredSum += error * error;
        }
      }

      errorsSquaredSum /= (end - start);
      if(double.IsNaN(errorsSquaredSum) || double.IsInfinity(errorsSquaredSum)) {
        errorsSquaredSum = double.MaxValue;
      }
      mse.Data = errorsSquaredSum;
    }
  }
}
