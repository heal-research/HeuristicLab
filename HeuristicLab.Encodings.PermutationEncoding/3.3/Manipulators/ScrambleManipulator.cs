#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// Manipulates a permutation array by randomly scrambling the elements in a randomly chosen interval.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Syswerda, G. (1991). Schedule Optimization Using Genetic Algorithms. In Davis, L. (Ed.) Handbook of Genetic Algorithms, Van Nostrand Reinhold, New York, pp 332-349.
  /// </remarks>
  [Item("ScrambleManipulator", "An operator which manipulates a permutation array by randomly scrambling the elements in a randomly chosen interval. It is implemented as described in Syswerda, G. (1991). Schedule Optimization Using Genetic Algorithms. In Davis, L. (Ed.) Handbook of Genetic Algorithms, Van Nostrand Reinhold, New York, pp 332-349.")]
  [StorableClass]
  public class ScrambleManipulator : PermutationManipulator {
    [StorableConstructor]
    protected ScrambleManipulator(bool deserializing) : base(deserializing) { }
    protected ScrambleManipulator(ScrambleManipulator original, Cloner cloner) : base(original, cloner) { }
    public ScrambleManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScrambleManipulator(this, cloner);
    }

    /// <summary>
    /// Mixes the elements of the given <paramref name="permutation"/> randomly 
    /// in a randomly chosen interval.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    public static void Apply(IRandom random, Permutation permutation) {
      int breakPoint1, breakPoint2;
      int[] scrambledIndices, remainingIndices, temp;
      int selectedIndex, index;

      breakPoint1 = random.Next(permutation.Length - 1);
      breakPoint2 = random.Next(breakPoint1 + 1, permutation.Length);

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

      Apply(permutation, breakPoint1, scrambledIndices);
    }

    public static void Apply(Permutation permutation, int startIndex, int[] scrambleArray) {
      Permutation original = (Permutation)permutation.Clone();
      for (int i = 0; i < scrambleArray.Length; i++) {  // scramble permutation between breakpoints
        permutation[startIndex + i] = original[startIndex + scrambleArray[i]];
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
