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
  public class CoefficientOfDeterminationEvaluator : GPEvaluatorBase {
    private DoubleData r2;
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'Dataset' and calculates
the 'coefficient of determination' of estimated values vs. real values of 'TargetVariable'.";
      }
    }

    public CoefficientOfDeterminationEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("R2", "The coefficient of determination of the model", typeof(DoubleData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      r2 = GetVariableValue<DoubleData>("R2", scope, false, false);
      if(r2 == null) {
        r2 = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("R2"), r2));
      }

      return base.Apply(scope);
    }

    public override void Evaluate(int start, int end) {
      double errorsSquaredSum = 0.0;
      double originalDeviationTotalSumOfSquares = 0.0;
      for(int sample = start; sample < end; sample++) {
        double estimated = GetEstimatedValue(sample);
        double original = GetOriginalValue(sample);
        SetOriginalValue(sample, estimated);
        if(!double.IsNaN(original) && !double.IsInfinity(original)) {
          double error = estimated - original;
          errorsSquaredSum += error * error;

          double origDeviation = original - TargetMean;
          originalDeviationTotalSumOfSquares += origDeviation * origDeviation;
        }
      }

      double quality = 1 - errorsSquaredSum / originalDeviationTotalSumOfSquares;
      if(quality > 1)
        throw new InvalidProgramException();
      if(double.IsNaN(quality) || double.IsInfinity(quality))
        quality = double.MaxValue;
      r2.Data = quality;
    }
  }
}
