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

namespace HeuristicLab.GP {
  public class FullTreeShaker : DelegatingOperator {
    public override string Description {
      get { return "Manipulates all tree nodes for which a '" + FunctionBase.MANIPULATION + "' variable is defined."; }
    }

    public FullTreeShaker()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "A random generator (uniform)", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "Operator library that defines operator mutations", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShakingFactor", "Variable that determines the force of the shaking operation", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be mutated", typeof(IFunctionTree), VariableKind.In | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      GPOperatorLibrary library = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      IFunctionTree tree = GetVariableValue<IFunctionTree>("FunctionTree", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);

      // enqueue all mutation operations as parallel operations
      CompositeOperation next = new CompositeOperation();
      next.ExecuteInParallel = false;

      Scope tempScope = new Scope("Temp. manipulation scope");

      TreeGardener gardener = new TreeGardener(mt, library);
      var parametricBranches = gardener.GetAllSubTrees(tree).Where(branch => branch.Function.GetVariable(FunctionBase.MANIPULATION) != null);
      foreach(IFunctionTree subTree in parametricBranches) {
        IOperator mutation = (IOperator)subTree.Function.GetVariable(FunctionBase.MANIPULATION).Value;

        // store all local variables into a temporary scope
        Scope mutationScope = new Scope();
        foreach(IVariable variable in subTree.LocalVariables) {
          mutationScope.AddVariable(variable);
        }

        tempScope.AddSubScope(mutationScope);
        next.AddOperation(new AtomicOperation(mutation, mutationScope));
      }

      // save all existing sub-scopes in a backup scope
      Scope backupScope = new Scope("backup");
      foreach(Scope subScope in scope.SubScopes) {
        backupScope.AddSubScope(subScope);
      }

      scope.AddSubScope(tempScope); // scope containing a subscope for each manipulation
      scope.AddSubScope(backupScope); // scope containing the old subscopes

      // schedule a reducer operation to delete all temporary scopes and restore
      // the subscopes of the backup scope after all manipulations are finished.
      next.AddOperation(new AtomicOperation(new RightReducer(), scope));
      return next;
    }
  }
}
