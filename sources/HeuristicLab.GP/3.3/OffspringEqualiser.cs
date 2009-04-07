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
  public class OffspringEqualiser : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public OffspringEqualiser() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("Value", "The value to equalise", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BinSize", "Size of histogram bins", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("EqualiserHistogram", "Histogram of the target distribution", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("AcceptanceProbabilities", "Acceptance probabilities of individuals falling into bins", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Histogram", "The histogram of the actual distribution", typeof(IntArrayData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      int binSize = GetVariableValue<IntData>("BinSize", scope, true).Data;
      DoubleArrayData equaliserHistogram = GetVariableValue<DoubleArrayData>("EqualiserHistogram", scope, true);
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      DoubleArrayData acceptanceProbabilities = GetVariableValue<DoubleArrayData>("AcceptanceProbabilities", scope, true);
      IntArrayData currentHistogram = GetVariableValue<IntArrayData>("Histogram", scope, true);

      int value = GetVariableValue<IntData>("Value", scope, false).Data;
      int bin = value / binSize;
      if(bin<acceptanceProbabilities.Data.Length && random.NextDouble() < acceptanceProbabilities.Data[bin]) {
        currentHistogram.Data[bin]++;
        currentHistogram.FireChanged();
        return new AtomicOperation(SubOperators[0], scope);
      } else {
        scope.AddVariable(new Variable(scope.TranslateName("Quality"), new DoubleData(double.NaN)));
        return null;
      }
    }
  }
}
