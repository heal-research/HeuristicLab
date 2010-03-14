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

namespace HeuristicLab.Encodings.Permutation {
  /// <summary>
  /// Manipulates a permutation array by randomly scrambling the elements in a randomly chosen interval.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Syswerda, G. (1991). Schedule Optimization Using Genetic Algorithms. In Davis, L. (Ed.) Handbook of Genetic Algorithms, Van Nostrand Reinhold, New York, pp 332-349.
  /// </remarks>
  [Item("ScrambleManipulator", "An operator which manipulates a permutation array by randomly scrambling the elements in a randomly chosen interval. It is implemented as described in Syswerda, G. (1991). Schedule Optimization Using Genetic Algorithms. In Davis, L. (Ed.) Handbook of Genetic Algorithms, Van Nostrand Reinhold, New York, pp 332-349.")]
  [StorableClass]
  [Creatable("Test")]
  public class ScrambleManipulator : PermutationManipulator {
    /// <summary>
    /// Mixes the elements of the given <paramref name="permutation"/> randomly 
    /// in a randomly chosen interval.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    public static void Apply(IRandom random, Permutation permutation) {
      Permutation original = (Permutation)permutation.Clone();

      int breakPoint1, breakPoint2;
      int[] scrambledIndices, remainingIndices, temp;
      int selectedIndex, index;

      breakPoint1 = random.Next(original.Length - 1);
      breakPoint2 = random.Next(breakPoint1 + 1, original.Length);

      scrambledIndices = new int[breakPoint2 - breakPoint1 + 1];
      remainingIndices = new int[breakPoint2 - breakPoint1 + 1];
      for (int i = 0; i < remainingIndices.Length; i++) {  // initialise indices
        remainingIndices[i] = i;
      }
      for (int i = 0; i < scrambledIndices.Length; i++) {  // generate permutation of indices
        selectedIndex = random.Next(remainingIndices.Length);
        scrambledIndices[i] = remainingIndices[selectedIndex];

        temp = remainingIndices;
        remainingIndices = new int[temp.Length - 1];
        index = 0;
        for (int j = 0; j < remainingIndices.Length; j++) {
          if (index == selectedIndex) {
            index++;
          }
          remainingIndices[j] = temp[index];
          index++;
        }
      }

      for (int i = 0; i <= (breakPoint2 - breakPoint1); i++) {  // scramble permutation between breakpoints
        permutation[breakPoint1 + i] = original[breakPoint1 + scrambledIndices[i]];
      }
    }

    /// <summary>
    /// Mixes the elements of the given <paramref name="permutation"/> randomly 
    /// in a randomly chosen interval.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    protected override void Manipulate(IRandom random, Permutation permutation) {
      Apply(random, permutation);
    }
  }
}
