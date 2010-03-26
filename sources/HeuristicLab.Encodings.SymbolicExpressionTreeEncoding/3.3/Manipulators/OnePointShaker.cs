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

namespace HeuristicLab.Encodings.SymbolicExpressionTree {
  public class OnePointShaker : DelegatingOperator {
    public override string Description {
      get { return "Selects a random node of all tree-nodes that have a manipulator defined and manipulates the selected node."; }
    }

    public OnePointShaker()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionLibrary", "Function library that defines mutation operations for functions", typeof(FunctionLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "A random generator (uniform)", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShakingFactor", "Factor that determines the force of the shaking operation", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be mutated", typeof(IGeneticProgrammingModel), VariableKind.In | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      FunctionLibrary library = GetVariableValue<FunctionLibrary>("FunctionLibrary", scope, true);
      IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, false);
      MersenneTwister mt = GetVariableValue<MersenneTwister>("Random", scope, true);

      // get all nodes with local parameters
      var parametricBranches = TreeGardener.GetAllSubTrees(gpModel.FunctionTree).Where(branch => branch.HasLocalParameters);
      if (parametricBranches.Count() == 0) return null; // don't manipulate anything if there are no nodes with a manipulation operator
      IFunctionTree selectedBranch = parametricBranches.ElementAt(mt.Next(parametricBranches.Count()));

      // save the exising sub-scopes in a backup scope
      Scope backupScope = new Scope("backup");
      foreach (Scope subScope in scope.SubScopes) {
        backupScope.AddSubScope(subScope);
      }

      // create a scope for all shaking operations
      Scope tempScope = new Scope("Temp. manipulation scope");
      
      // add the new sub-scopes
      scope.AddSubScope(tempScope);
      scope.AddSubScope(backupScope);

      CompositeOperation next = new CompositeOperation();
      next.ExecuteInParallel = false;
      // the next operation should first manipulate and then restore the sub-scopes from the backup scope
      next.AddOperation(selectedBranch.CreateShakingOperation(tempScope));
      next.AddOperation(new AtomicOperation(new RightReducer(), scope));

      return next;
    }
  }
}
