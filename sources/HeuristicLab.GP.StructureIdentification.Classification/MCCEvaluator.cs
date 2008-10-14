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

namespace HeuristicLab.GP.Classification {
  public class MCCEvaluator : GPEvaluatorBase {
    private double limit;
    private double[] original = new double[1];
    private double[] estimated = new double[1];
    private DoubleData mcc;
    public override string Description {
      get {
        return @"Calculates the matthews correlation coefficient for a given model and class separation threshold";
      }
    }
    public MCCEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("ClassSeparation", "The value of separation between negative and positive target classification values (for instance 0.5 if negative=0 and positive=1).", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MCC", "The matthews correlation coefficient of the model", typeof(DoubleData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      mcc = GetVariableValue<DoubleData>("MCC", scope, false, false);
      if(mcc == null) {
        mcc = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("MCC"), mcc));
      }
      limit = GetVariableValue<DoubleData>("ClassSeparation", scope, true).Data;
      return base.Apply(scope);
    }

    public override void Evaluate(int start, int end) {
      int nSamples = end - start;
      if(estimated.Length != nSamples) {
        estimated = new double[nSamples];
        original = new double[nSamples];
      }

      double positive = 0;
      double negative = 0;
      for(int sample = start; sample < end; sample++) {
        double est = GetEstimatedValue(sample);
        double orig = GetOriginalValue(sample);
        SetOriginalValue(sample, est);
        estimated[sample - start] = est;
        original[sample - start] = orig;
        if(orig >= limit) positive++;
        else negative++;
      }
      Array.Sort(estimated, original);
      double best_mcc = -1.0;
      double tp = 0;
      double fn = positive;
      double tn = negative;
      double fp = 0;
      for(int i = original.Length - 1; i >= 0; i--) {
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
      this.mcc.Data = best_mcc;
    }
  }
}
