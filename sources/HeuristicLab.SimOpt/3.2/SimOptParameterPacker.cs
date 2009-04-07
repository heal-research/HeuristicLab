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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class SimOptParameterPacker : OperatorBase {
    public override string Description {
      get { return @"Takes the parameters in the subscope and creates or a updates a parameter vector. The order of the subscopes is assumed to be the same as the parameters appear in the vector.
If the parameter vector could not be updated due to a constraint violation, the first suboperator is returned as the next operation."; }
    }

    public SimOptParameterPacker()
      : base() {
      AddVariableInfo(new VariableInfo("Items", "The ConstrainedItemList to be updated or created", typeof(ConstrainedItemList), VariableKind.New | VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("DeleteParameters", "Whether or not the subscopes containing the parameters should be removed afterwards", typeof(BoolData), VariableKind.In));
      AddVariable(new Variable("DeleteParameters", new BoolData(true)));
      GetVariableInfo("DeleteParameters").Local = true;
    }

    public override IOperation Apply(IScope scope) {
      // ----- FETCH THE PARAMETER VECTOR ----- //
      bool updateVector = true;
      ConstrainedItemList cil;
      try {
        cil = GetVariableValue<ConstrainedItemList>("Items", scope, false);
      } catch (ArgumentException) {
        updateVector = false;
        // the parameter vector is fetched from a higher scope and added locally
        cil = GetVariableValue<ConstrainedItemList>("Items", scope, true);
      }
      ConstrainedItemList tempcil = (ConstrainedItemList)cil.Clone();
      bool delete = GetVariableValue<BoolData>("DeleteParameters", scope, true).Data;

      ICollection<IConstraint> violatedConstraints;

      tempcil.BeginCombinedOperation();
      // ----- FETCH PARAMETERS AND UPDATE TEMPORARY VECTOR ----- //
      for (int i = 0; i < scope.SubScopes.Count; i++) {
        IVariable var = scope.SubScopes[i].GetVariable(((IVariable)tempcil[i]).Name);
        if (var == null) throw new ArgumentNullException(scope.SubScopes[i].Name, "Could not find parameter " + ((IVariable)tempcil[i]).Name + " in this scope");
        tempcil.TrySetAt(i, var, out violatedConstraints);
      }

      // ----- CONSTRAINT HANDLING ----- //
      IVariableInfo info = GetVariableInfo("Items");
      bool success = tempcil.EndCombinedOperation(out violatedConstraints);
      if (success) {
        if (!updateVector) {
          if (info.Local) AddVariable(new Variable(info.ActualName, tempcil));
          else scope.AddVariable(new Variable(scope.TranslateName("Items"), tempcil));
        } else {
          if (info.Local) GetVariable(info.ActualName).Value = tempcil;
          else scope.GetVariable(scope.TranslateName("Items")).Value = tempcil;
        }
      } else if (!updateVector) { // in case there was an error and the parameter vector is not in the current scope, add it
        if (info.Local) AddVariable(new Variable(info.ActualName, cil));
        else scope.AddVariable(new Variable(scope.TranslateName("Items"), cil));
      }

      // ----- DELETE SUBSCOPES ----- //
      if (delete) {
        while (scope.SubScopes.Count > 0) scope.RemoveSubScope(scope.SubScopes[0]);
      }

      if (!success) return new AtomicOperation(SubOperators[0], scope);
      else return null;
    }
  }
}
