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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.Permutation {
  /// <summary>
  /// Base class for cross over permutations.
  /// </summary>
  public abstract class PermutationCrossoverBase : CrossoverBase {
    /// <summary>
    /// Initializes a new instance of <see cref="PermutationCrossoverBase"/> with one variable info
    /// (<c>Permutation</c>).
    /// </summary>
    public PermutationCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("Permutation", "Parent and child permutations", typeof(Permutation), VariableKind.In | VariableKind.New));
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/> with
    /// the given random number generator (<paramref name="random"/>) to create a new 
    /// <paramref name="child"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the two permutations have a different
    /// length.</exception>
    /// <remarks>Calls <see cref="Cross(HeuristicLab.Core.IScope, HeuristicLab.Core.IRandom, int[], int[])"/>.</remarks>
    /// <param name="scope">The scope where to get the actual child variable name.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <param name="child">The child scope which to assign the permutated data.</param>
    protected sealed override void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child) {
      Permutation perm1 = parent1.GetVariableValue<Permutation>("Permutation", false);
      Permutation perm2 = parent2.GetVariableValue<Permutation>("Permutation", false);

      if (perm1.Data.Length != perm2.Data.Length) throw new InvalidOperationException("Cannot apply crossover to permutations of different length.");

      int[] result = Cross(scope, random, perm1.Data, perm2.Data);
      child.AddVariable(new Variable(scope.TranslateName("Permutation"), new Permutation(result)));
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/> with
    /// the given random number generator (<paramref name="random"/>) .
    /// </summary>
    /// <param name="scope">The scope of the variables.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    protected abstract int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2);
  }
}
