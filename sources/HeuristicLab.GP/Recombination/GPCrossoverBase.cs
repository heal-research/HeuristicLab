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
using HeuristicLab.Constraints;
using System.Diagnostics;

namespace HeuristicLab.GP {
  public abstract class GPCrossoverBase : OperatorBase {
    private const int MAX_RECOMBINATION_TRIES = 100;

    public GPCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("OperatorLibrary", "The operator library containing all available operators", typeof(GPOperatorLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to mutate", typeof(IFunctionTree), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeSize", "The size (number of nodes) of the tree", typeof(IntData), VariableKind.New));
      AddVariableInfo(new VariableInfo("TreeHeight", "The height of the tree", typeof(IntData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      MersenneTwister random = GetVariableValue<MersenneTwister>("Random", scope, true);
      GPOperatorLibrary opLibrary = GetVariableValue<GPOperatorLibrary>("OperatorLibrary", scope, true);
      TreeGardener gardener = new TreeGardener(random, opLibrary);

      if ((scope.SubScopes.Count % 2) != 0)
        throw new InvalidOperationException("Number of parents is not even");

      int children = scope.SubScopes.Count / 2;
      for (int i = 0; i < children; i++) {
        IFunctionTree parent0 = TakeNextParent(scope);
        IFunctionTree parent1 = TakeNextParent(scope);

        // randomly swap parents to remove a possible bias from selection (e.g. when using gender-specific selection)
        if (random.NextDouble() < 0.5) {
          IFunctionTree tmp = parent0;
          parent0 = parent1;
          parent1 = tmp;
        }

        IFunctionTree child = Cross(scope, gardener, random, parent0, parent1);
        Debug.Assert(gardener.IsValidTree(child));
        IScope childScope = new Scope(i.ToString());
        childScope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), child));
        childScope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeSize"), new IntData(child.Size)));
        childScope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeHeight"), new IntData(child.Height)));
        scope.AddSubScope(childScope);
      }

      return null;
    }

    internal abstract IFunctionTree Cross(IScope scope, TreeGardener gardener, MersenneTwister random, IFunctionTree tree0, IFunctionTree tree1);

    private IFunctionTree TakeNextParent(IScope scope) {
      IFunctionTree parent = GetVariableValue<IFunctionTree>("FunctionTree", scope.SubScopes[0], false);
      scope.RemoveSubScope(scope.SubScopes[0]);
      return parent;
    }
  }
}
