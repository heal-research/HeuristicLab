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

using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Permutation {
  /// <summary>
  /// An operator which inverts a randomly chosen part of a permutation.
  /// </summary>
  [Item("InversionManipulator", "An operator which inverts a randomly chosen part of a permutation.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class InversionManipulator : PermutationManipulator {

    /// <summary>
    /// Inverts a randomly chosen part of a permutation.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    /// <returns>The new manipulated permutation.</returns>
    public static Permutation Apply(IRandom random, Permutation permutation) {
      Permutation result = (Permutation)permutation.Clone();
      int breakPoint1, breakPoint2;

      breakPoint1 = random.Next(result.Length - 1);
      do {
        breakPoint2 = random.Next(result.Length - 1);
      } while (breakPoint2 == breakPoint1);
      if (breakPoint2 < breakPoint1) { int h = breakPoint1; breakPoint1 = breakPoint2; breakPoint2 = h; }

      for (int i = 0; i <= (breakPoint2 - breakPoint1); i++) {  // invert permutation between breakpoints
        result[breakPoint1 + i] = permutation[breakPoint2 - i];
      }
      return result;
    }

    /// <summary>
    /// Inverts a randomly chosen part of a permutation.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    /// <returns>The new manipulated permuation.</returns>
    protected override Permutation Manipulate(IRandom random, Permutation permutation) {
      return Apply(random, permutation);
    }
  }
}
