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

using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Selection;

namespace HeuristicLab.GP.Operators {
  public class FullTreeShaker : DelegatingOperator {
    public override string Description {
      get { return "Manipulates all tree nodes for which a manipulator is defined."; }
    }

    public FullTreeShaker()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "A random generator (uniform)", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionLibrary", "Function library that defines function mutations", typeof(FunctionLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShakingFactor", "Variable that determines the force of the shaking operation", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be mutated", typeof(IGeneticProgrammingModel), VariableKind.In | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      FunctionLibrary library = GetVariableValue<FunctionLibrary>("FunctionLibrary", scope, true);
      IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);

      // save all existing sub-scopes in a backup scope
      Scope backupScope = new Scope("backup");
      foreach (Scope subScope in scope.SubScopes) {
        backupScope.AddSubScope(subScope);
      }

      // create a scope for all shaking operations
      Scope tempScope = new Scope("Temp. manipulation scope");
      scope.AddSubScope(tempScope); // scope containing a subscope for each manipulation
      scope.AddSubScope(backupScope); // scope containing the old subscopes

      // create a composite operation for all shaking operations
      CompositeOperation next = new CompositeOperation();
      next.ExecuteInParallel = false;

      // enqueue all shaking operations
      foreach (IFunctionTree subTree in TreeGardener.GetAllSubTrees(gpModel.FunctionTree).Where(x=>x.HasLocalParameters)) {
        IOperation shakingOperation = subTree.CreateShakingOperation(tempScope);
        next.AddOperation(shakingOperation);
      }

      // schedule a reducer operation to delete all temporary scopes and restore
      // the subscopes of the backup scope after all manipulations are finished.
      next.AddOperation(new AtomicOperation(new RightReducer(), scope));
      return next;
    }
  }
}
