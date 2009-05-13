#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Random;

namespace HeuristicLab.SimOpt {
  public class SimOptSelfAdaptiveNumericVectorNormalMutation : OperatorBase {

    public override string Description {
      get { return @"This operator modifies all elements in the parameter vector using a normal distributed variable with mean 0 and variable sigma"; }
    }

    public SimOptSelfAdaptiveNumericVectorNormalMutation()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "The random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShakingFactors", "The mutation strength vector", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Items", "The parameter vector", typeof(ConstrainedItemList), VariableKind.In | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      DoubleArrayData shakingFactors = GetVariableValue<DoubleArrayData>("ShakingFactors", scope, false);

      ConstrainedItemList parameters = GetVariableValue<ConstrainedItemList>("Items", scope, false);
      ConstrainedItemList temp = null;
      ICollection<IConstraint> tmp;

      NormalDistributedRandom nd = new NormalDistributedRandom(random, 0.0, 1.0);

      do {
        temp = (ConstrainedItemList)parameters.Clone();

        temp.BeginCombinedOperation();
        for (int i = 0; i < temp.Count; i++) {
          if (random.NextDouble() < shakingFactors.Data[i % shakingFactors.Data.Length]) {
            if (((Variable)temp[i]).Value is IntData) {
              ((IntData)((Variable)temp[i]).Value).Data += ((int)(nd.NextDouble() * shakingFactors.Data[i % shakingFactors.Data.Length]));
            } else if (((Variable)temp[i]).Value is DoubleData) {
              ((DoubleData)((Variable)temp[i]).Value).Data += nd.NextDouble() * shakingFactors.Data[i % shakingFactors.Data.Length];
            } else if (((Variable)temp[i]).Value is ConstrainedIntData) {
              int val = ((ConstrainedIntData)((Variable)temp[i]).Value).Data;
              ((ConstrainedIntData)((Variable)temp[i]).Value).TrySetData(val + ((int)(nd.NextDouble() * shakingFactors.Data[i % shakingFactors.Data.Length])));
            } else if (((Variable)temp[i]).Value is ConstrainedDoubleData) {
              double val = ((ConstrainedDoubleData)((Variable)temp[i]).Value).Data;
              ((ConstrainedDoubleData)((Variable)temp[i]).Value).TrySetData(val + nd.NextDouble() * shakingFactors.Data[i % shakingFactors.Data.Length]);
            }
          }
        }
      } while (!temp.EndCombinedOperation(out tmp));

      parameters.BeginCombinedOperation();
      for (int i = 0; i < temp.Count; i++)
        parameters.TrySetAt(i, temp[i], out tmp);
      parameters.EndCombinedOperation(out tmp);

      return null;
    }
  }
}
