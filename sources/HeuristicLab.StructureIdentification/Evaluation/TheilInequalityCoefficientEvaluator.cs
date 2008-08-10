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
    private bool differential;
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

    public override IOperation Apply(IScope scope) {
      differential = GetVariableValue<BoolData>("Differential", scope, true).Data;
      return base.Apply(scope);
    }

    public override double Evaluate(int start, int end) {
      double errorsSquaredSum = 0.0;
      double estimatedSquaredSum = 0.0;
      double originalSquaredSum = 0.0;
      for(int sample = start; sample < end; sample++) {
        double prevValue = 0.0;
        if(differential) prevValue = GetOriginalValue(sample - 1);
        double estimatedChange = GetEstimatedValue(sample) - prevValue;
        double originalChange = GetOriginalValue(sample) - prevValue;
        if(!double.IsNaN(originalChange) && !double.IsInfinity(originalChange)) {
          double error = estimatedChange - originalChange;
          errorsSquaredSum += error * error;
          estimatedSquaredSum += estimatedChange * estimatedChange;
          originalSquaredSum += originalChange * originalChange;
        }
      }
      int nSamples = end - start;
      double quality = Math.Sqrt(errorsSquaredSum / nSamples) / (Math.Sqrt(estimatedSquaredSum / nSamples) + Math.Sqrt(originalSquaredSum / nSamples));
      if(double.IsNaN(quality) || double.IsInfinity(quality))
        quality = double.MaxValue;
      return quality;
    }
  }
}
