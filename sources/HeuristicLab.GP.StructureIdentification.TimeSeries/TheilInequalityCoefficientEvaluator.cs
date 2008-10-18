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

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  public class TheilInequalityCoefficientEvaluator : GPEvaluatorBase {
    private DoubleData theilInequaliy;
    public override string Description {
      get {
        return @"Evaluates 'FunctionTree' for all samples of 'Dataset' and calculates
the 'Theil inequality coefficient (scale invariant)' of estimated values vs. real values of 'TargetVariable'.";
      }
    }

    public TheilInequalityCoefficientEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("TheilInequalityCoefficient", "Theil's inequality coefficient of the model", typeof(DoubleData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      theilInequaliy = GetVariableValue<DoubleData>("TheilInequalityCoefficient", scope, false, false);
      if(theilInequaliy == null) {
        theilInequaliy = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TheilInequalityCoefficient"), theilInequaliy));
      }
      return base.Apply(scope);
    }

    public override void Evaluate(int start, int end) {
      double errorsSquaredSum = 0.0;
      double originalSquaredSum = 0.0;
      for(int sample = start; sample < end; sample++) {
        double prevValue = GetOriginalValue(sample - 1);
        double estimatedChange = GetEstimatedValue(sample) - prevValue;
        double originalChange = GetOriginalValue(sample) - prevValue;
        SetOriginalValue(sample, estimatedChange + prevValue);
        if(!double.IsNaN(originalChange) && !double.IsInfinity(originalChange)) {
          double error = estimatedChange - originalChange;
          errorsSquaredSum += error * error;
          originalSquaredSum += originalChange * originalChange;
        }
      }
      int nSamples = end - start;
      double quality = Math.Sqrt(errorsSquaredSum / nSamples) / Math.Sqrt(originalSquaredSum / nSamples);
      if(double.IsNaN(quality) || double.IsInfinity(quality))
        quality = double.MaxValue;
      theilInequaliy.Data = quality;
    }
  }
}
