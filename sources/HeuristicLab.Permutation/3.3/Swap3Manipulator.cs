#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Permutation {
  /// <summary>
  /// Manipulates a permutation array by swaping three randomly chosen elements.
  /// </summary>
  [Item("Swap3Manipulator", "An operator which manipulates a permutation array by swaping three randomly chosen elements.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class Swap3Manipulator : PermutationManipulator {
    /// <summary>
    /// Swaps three randomly chosen elements of the given <paramref name="permutation"/> array.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    public static void Apply(IRandom random, Permutation permutation) {
      int index1, index2, index3, temp;

      index1 = random.Next(permutation.Length);
      index2 = random.Next(permutation.Length);
      index3 = random.Next(permutation.Length);

      if (random.NextDouble() < 0.5) {
        // swap edges 1 and 2
        temp = permutation[index1];
        permutation[index1] = permutation[index2];
        permutation[index2] = temp;
        // swap edges 2 and 3
        temp = permutation[index2];
        permutation[index2] = permutation[index3];
        permutation[index3] = temp;
      } else {
        // swap edges 1 and 3
        temp = permutation[index1];
        permutation[index1] = permutation[index3];
        permutation[index3] = temp;
        // swap edges 2 and 3
        temp = permutation[index2];
        permutation[index2] = permutation[index3];
        permutation[index3] = temp;
      }
    }

    /// <summary>
    /// Swaps three randomly chosen elements of the given <paramref name="permutation"/> array.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    protected override void Manipulate(IRandom random, Permutation permutation) {
      Apply(random, permutation);
    }
  }
}
