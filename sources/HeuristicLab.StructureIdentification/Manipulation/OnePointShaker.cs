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
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Data;
using HeuristicLab.Selection;

namespace HeuristicLab.StructureIdentification {
  public class OnePointShaker : OperatorBase {
    public override string Description {
      get { return "Selects a random node of all tree-nodes that have a '"+GPOperatorLibrary.MANIPULATION+"' variable defined and manipulates the selected node."; }
    }

    public OnePointShaker()
      : base() {
      AddVariableInfo(new VariableInfo("OperatorLibrary", "Operator library that defines mutation operations for operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorTree", "The operator tree that should be mutated", typeof(IOperator), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "A random generator (uniform)", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShakingFactor", "Factor that determines the force of the shaking operation", typeof(DoubleData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      GPOperatorLibrary library = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      IOperator op = GetVariableValue<IOperator>("OperatorTree", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);
      TreeGardener gardener = new TreeGardener(mt, library);

      // get all nodes for which a manipulation is defined
      var parametricNodes = gardener.GetAllOperators(op).Where(o => o.GetVariable(GPOperatorLibrary.MANIPULATION) != null);
      IOperator selectedOp = parametricNodes.ElementAt(mt.Next(parametricNodes.Count()));

      IOperator mutation = (IOperator)selectedOp.GetVariable(GPOperatorLibrary.MANIPULATION).Value;

      CompositeOperation next = new CompositeOperation();

      // store all local variables into a temporary scope
      Scope tempScope = new Scope("Temp. manipulation scope");

      foreach(IVariableInfo info in selectedOp.VariableInfos) {
        if(info.Local) {
          tempScope.AddVariable(selectedOp.GetVariable(info.FormalName));
        }
      }

      // save the exising sub-scops in a backup scope
      Scope backupScope = new Scope("backup");
      foreach (Scope subScope in scope.SubScopes) {
        backupScope.AddSubScope(subScope);
      }

      // add the new sub-scopes
      scope.AddSubScope(tempScope);
      scope.AddSubScope(backupScope);

      // the next operation should first manipulate and then restore the sub-scopes from the backup scope
      next.AddOperation(new AtomicOperation(mutation, tempScope));
      next.AddOperation(new AtomicOperation(new RightReducer(), scope));

      return next;
    }
  }
}
