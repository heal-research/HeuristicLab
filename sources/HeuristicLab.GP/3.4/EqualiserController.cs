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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.GP {
  public class EqualiserController : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public EqualiserController() {
      AddVariableInfo(new VariableInfo("EqualiserHistogram", "Histogram of the target distribution", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("AcceptanceProbabilities", "Acceptance probabilities of individuals falling into bins", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Histogram", "The histogram of the actual distribution", typeof(IntArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Rate", "Parameter that controls the convergence rate of the population to the equalised distribution", typeof(DoubleData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      DoubleArrayData equaliserHistogram = GetVariableValue<DoubleArrayData>("EqualiserHistogram", scope, true);
      DoubleArrayData acceptanceProbabilities = GetVariableValue<DoubleArrayData>("AcceptanceProbabilities", scope, false, false);
      IntArrayData currentHistogram = GetVariableValue<IntArrayData>("Histogram", scope, false, false);
      double rate = GetVariableValue<DoubleData>("Rate", scope, false, false).Data;

      double sum = 0.0;
      Array.ForEach(currentHistogram.Data, delegate(int i) { sum += i; });
      for(int i = 0; i < acceptanceProbabilities.Data.Length; i++) {
        double normalizedDiff = (equaliserHistogram.Data[i] - (currentHistogram.Data[i] / sum)) / equaliserHistogram.Data[i];
        acceptanceProbabilities.Data[i] = acceptanceProbabilities.Data[i] + normalizedDiff * rate;
        currentHistogram.Data[i] = 0;
      }
      currentHistogram.FireChanged();
      acceptanceProbabilities.FireChanged();
      return null;
    }
  }
}
