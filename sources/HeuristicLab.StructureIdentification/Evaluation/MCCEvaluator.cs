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
  public class MCCEvaluator : GPEvaluatorBase {
    public override string Description {
      get {
        return @"TASK";
      }
    }

    public MCCEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("ClassSeparation", "The value of separation between negative and positive target classification values (for instance 0.5 if negative=0 and positive=1).", typeof(DoubleData), VariableKind.In));
    }

    public override double Evaluate(IScope scope, IFunctionTree functionTree, int targetVariable, Dataset dataset) {
      double limit = GetVariableValue<DoubleData>("ClassSeparation", scope, false).Data;
      double[] estimated = new double[dataset.Rows];
      double[] original = new double[dataset.Rows];
      double positive = 0;
      double negative = 0;
      double targetMean = dataset.GetMean(targetVariable);
      for(int sample = 0; sample < dataset.Rows; sample++) {
        double est = functionTree.Evaluate(dataset, sample);
        double orig = dataset.GetValue(sample, targetVariable);
        if(double.IsNaN(est) || double.IsInfinity(est)) {
          est = targetMean + maximumPunishment;
        } else if(est > targetMean + maximumPunishment) {
          est = targetMean + maximumPunishment;
        } else if(est < targetMean - maximumPunishment) {
          est = targetMean - maximumPunishment;
        }
        estimated[sample] = est;
        original[sample] = orig;
        if(orig >= limit) positive++;
        else negative++;
      }
      Array.Sort(estimated, original);
      double best_mcc = -1.0;
      double tp = 0;
      double fn = positive;
      double tn = negative;
      double fp = 0;
      for(int i = original.Length-1; i >= 0 ; i--) {
        if(original[i] >= limit) {
          tp++; fn--;
        } else {
          tn--; fp++;
        }
        double mcc = (tp * tn - fp * fn) / Math.Sqrt(positive * (tp + fn) * (tn + fp) * negative);
        if(best_mcc < mcc) {
          best_mcc = mcc;
        }
      }
      return best_mcc;
    }
  }
}
