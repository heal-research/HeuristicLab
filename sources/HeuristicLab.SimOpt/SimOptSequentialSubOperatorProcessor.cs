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

namespace HeuristicLab.SimOpt {
  public class SimOptSequentialSubOperatorProcessor : OperatorBase {
    public override string Description {
      get {
        return @"Applies its suboperators on the parameter vector one for each index, afterwards it checks if the parameter vector satisfies all the constraints. If not it restores the old parameter vector and tries again until a maximum amount of tries (default: 100) has been reached.";
      }
    }

    public SimOptSequentialSubOperatorProcessor()
      : base() {
      AddVariableInfo(new VariableInfo("Items", "The parameter vector", typeof(ConstrainedItemList), VariableKind.New | VariableKind.In | VariableKind.Out));
      AddVariable(new Variable("ItemsBackup", new NullData()));
      AddVariableInfo(new VariableInfo("MaximumTries", "", typeof(IntData), VariableKind.In));
      GetVariableInfo("MaximumTries").Local = true;
      AddVariable(new Variable("MaximumTries", new IntData(100)));
      AddVariable(new Variable("Tries", new IntData(0)));
    }

    public override IOperation Apply(IScope scope) {
      ConstrainedItemList parameterVector = GetVariableValue<ConstrainedItemList>("Items", scope, false, false);
      // Mode: The parameter vector does not yet exist in the current scope
      if (parameterVector == null) { // the parameter does not yet exist in the current scope
        parameterVector = GetVariableValue<ConstrainedItemList>("Items", scope, true); // search for it
        parameterVector = (ConstrainedItemList)parameterVector.Clone(); // clone it
        scope.AddVariable(new Variable(scope.TranslateName("Items"), parameterVector)); // and add it to the current scope
      }
      // Mode: The parameter vector is marked for manipulation/initialization (constraint check is suspended)
      if (parameterVector.ConstraintCheckSuspended) {
        ICollection<IConstraint> violatedConstraints;
        if (parameterVector.EndCombinedOperation(out violatedConstraints)) {
          ((IntData)GetVariable("Tries").Value).Data = 0;
          GetVariable("ItemsBackup").Value = new NullData();
          return null; // manipulation/initialization was successful
        } else { // restore old vector
          int maximumTries = GetVariableValue<IntData>("MaximumTries", scope, true).Data;
          IntData tries = (GetVariable("Tries").Value as IntData);
          if (tries.Data >= maximumTries) throw new InvalidOperationException("ERROR: no valid solution in " + maximumTries.ToString() + " tries");
          parameterVector = (ConstrainedItemList)GetVariable("ItemsBackup").Value;
          scope.GetVariable(scope.TranslateName("Items")).Value = parameterVector;
          ((IntData)GetVariable("Tries").Value).Data++;
        }
      }
      // perform the sub operators in sequential order on the indices of the parameter vector
      GetVariable("ItemsBackup").Value = (ConstrainedItemList)parameterVector.Clone();
      CompositeOperation co = new CompositeOperation();
      for (int i = 0; i < SubOperators.Count; i++) {
        if (SubOperators[i].GetVariable("Index") != null) {
          SubOperators[i].GetVariable("Index").Value = new IntData(i);
        }
        if (SubOperators[i].GetVariableInfo("Items") != null) {
          SubOperators[i].GetVariableInfo("Items").ActualName = GetVariableInfo("Items").ActualName;
        }
      }
      for (int i = 0; i < SubOperators.Count; i++)
        co.AddOperation(new AtomicOperation(SubOperators[i], scope));
      // add self to check if the manipulation/initialization did not violate any constraints
      co.AddOperation(new AtomicOperation(this, scope));
      parameterVector.BeginCombinedOperation();
      return co;
    }
  }
}
