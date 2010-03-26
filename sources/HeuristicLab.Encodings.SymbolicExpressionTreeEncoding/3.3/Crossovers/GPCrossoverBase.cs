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
using HeuristicLab.Core;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTree {
  public abstract class GPCrossoverBase : CrossoverBase {
    public GPCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionLibrary", "The operator library containing all available operators", typeof(FunctionLibrary), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The tree to crossover", typeof(IGeneticProgrammingModel), VariableKind.In | VariableKind.New));
    }

    internal abstract IFunctionTree Cross(IScope scope, TreeGardener gardener, IRandom random, IFunctionTree tree0, IFunctionTree tree1);

    protected override void Cross(IScope scope, IRandom random) {
      FunctionLibrary opLibrary = GetVariableValue<FunctionLibrary>("FunctionLibrary", scope, true);
      TreeGardener gardener = new TreeGardener(random, opLibrary);

      if (scope.SubScopes.Count != 2)
        throw new InvalidOperationException("Number of parents must be exactly two.");

      IGeneticProgrammingModel parent0 = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope.SubScopes[0], false);
      IGeneticProgrammingModel parent1 = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope.SubScopes[1], false);

      // randomly swap parents to remove a possible bias from selection (e.g. when using gender-specific selection)
      if (random.NextDouble() < 0.5) {
        IGeneticProgrammingModel tmp = parent0;
        parent0 = parent1;
        parent1 = tmp;
      }

      IFunctionTree child = Cross(scope, gardener, random, parent0.FunctionTree, parent1.FunctionTree);
      Debug.Assert(gardener.IsValidTree(child));
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), new GeneticProgrammingModel(child)));
    }
  }
}
