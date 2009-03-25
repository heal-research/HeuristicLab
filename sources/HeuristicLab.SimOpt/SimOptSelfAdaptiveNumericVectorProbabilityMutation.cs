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

namespace HeuristicLab.SimOpt {
  public class SimOptSelfAdaptiveNumericVectorProbabilityMutation : OperatorBase {

    public override string Description {
      get { return @"Takes the values of a strategy vectors as possibilities to manipulate a certain dimension in the parameter vector"; }
    }

    public SimOptSelfAdaptiveNumericVectorProbabilityMutation()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "The random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Probabilities", "The probability vector", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Items", "The parameter vector", typeof(ConstrainedItemList), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("MaxVector", "Vector containing the maximum values", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinVector", "Vector containing the minimum values", typeof(DoubleArrayData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      DoubleArrayData max = GetVariableValue<DoubleArrayData>("MaxVector", scope, true);
      DoubleArrayData min = GetVariableValue<DoubleArrayData>("MinVector", scope, true);
      DoubleArrayData probs = GetVariableValue<DoubleArrayData>("Probabilities", scope, false);

      ConstrainedItemList parameters = GetVariableValue<ConstrainedItemList>("Items", scope, false);
      int tries;
      ConstrainedItemList temp = null;
      ICollection<IConstraint> tmp;

      for (tries = 0; tries < 100; tries++) {
        temp = (ConstrainedItemList)parameters.Clone();

        temp.BeginCombinedOperation();
        for (int i = 0; i < temp.Count; i++) {
          if (random.NextDouble() < probs.Data[i % probs.Data.Length]) {
            if (((Variable)temp[i]).Value is IntData) {
              ((IntData)((Variable)temp[i]).Value).Data = random.Next((int)Math.Floor(min.Data[i % min.Data.Length]), (int)Math.Ceiling(max.Data[i % max.Data.Length]));
            } else if (((Variable)temp[i]).Value is DoubleData) {
              ((DoubleData)((Variable)temp[i]).Value).Data = min.Data[i] + (max.Data[i % max.Data.Length] - min.Data[i % min.Data.Length]) * random.NextDouble();
            } else if (((Variable)temp[i]).Value is ConstrainedIntData) {
              ((ConstrainedIntData)((Variable)temp[i]).Value).TrySetData(random.Next((int)Math.Floor(min.Data[i % min.Data.Length]), (int)Math.Ceiling(max.Data[i % max.Data.Length])));
            } else if (((Variable)temp[i]).Value is ConstrainedDoubleData) {
              ((ConstrainedDoubleData)((Variable)temp[i]).Value).TrySetData(min.Data[i % min.Data.Length] + (max.Data[i % max.Data.Length] - min.Data[i % min.Data.Length]) * random.NextDouble());
            }
          }
        }
        if (temp.EndCombinedOperation(out tmp)) break;
      }

      if (tries < 100) {
        parameters.BeginCombinedOperation();
        for (int i = 0; i < temp.Count; i++)
          parameters.TrySetAt(i, temp[i], out tmp);
        parameters.EndCombinedOperation(out tmp);
      } else throw new InvalidOperationException("ERROR in SimOptSelfAdaptiveNumericVectorProbabilityMutation: no feasible result in 100 tries");

      return null;
    }
  }
}
