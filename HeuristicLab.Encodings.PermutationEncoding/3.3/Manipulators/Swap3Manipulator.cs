#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// Manipulates a permutation array by swaping three randomly chosen elements.
  /// </summary>
  /// <remarks>
  /// It is implemented such that first 3 positions are randomly chosen in the interval [0;N) with N = length of the permutation with all positions being distinct from each other.
  /// Then position 1 is put in place of position 3, position 2 is put in place of position 1 and position 3 is put in place of position 2.
  /// </remarks>
  [Item("Swap3Manipulator", "An operator which manipulates a permutation array by swaping three randomly chosen elements. It is implemented such that first 3 positions are randomly chosen in the interval [0;N) with N = length of the permutation with all positions being distinct from each other. Then position 1 is put in place of position 3, position 2 is put in place of position 1 and position 3 is put in place of position 2.")]
  [StorableType("3907489F-54DF-4774-85E6-E89CE055B56C")]
  public class Swap3Manipulator : PermutationManipulator {
    [StorableConstructor]
    protected Swap3Manipulator(StorableConstructorFlag _) : base(_) { }
    protected Swap3Manipulator(Swap3Manipulator original, Cloner cloner) : base(original, cloner) { }
    public Swap3Manipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Swap3Manipulator(this, cloner);
    }

    /// <summary>
    /// Swaps three randomly chosen elements of the given <paramref name="permutation"/> array.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="permutation"/> contains less than 3 elements.</exception>
    /// <remarks>
    /// It is implemented such that first 3 positions are randomly chosen in the interval [0;N) with N = length of the permutation with all positions being distinct from each other.
    /// Then position 1 is put in place of position 3, position 2 is put in place of position 1 and position 3 is put in place of position 2.
    /// </remarks>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    public static void Apply(IRandom random, Permutation permutation) {
      if (permutation.Length < 3) throw new ArgumentException("Swap3Manipulator: The permutation must be at least of size 3.", "permutation");
      int index1, index2, index3;

      do {
        index1 = random.Next(permutation.Length);
        index2 = random.Next(permutation.Length);
        index3 = random.Next(permutation.Length);
      } while (index1 == index2 || index2 == index3 || index1 == index3);

      permutation.Swap(index1, index2);
      permutation.Swap(index2, index3);
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
