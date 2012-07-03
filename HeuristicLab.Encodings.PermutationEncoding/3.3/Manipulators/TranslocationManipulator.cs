#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Manipulates a permutation array by moving a randomly chosen interval of elements to another 
  /// (randomly chosen) position in the array.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1992. Genetic Algorithms + Data Structures = Evolution Programs, Springer Verlag, Berlin Heidelberg.
  /// </remarks>
  [Item("TranslocationManipulator", "An operator which Manipulates a permutation array by moving a randomly chosen interval of elements to another (randomly chosen) position in the array. It is implemented as described in Michalewicz, Z. 1992. Genetic Algorithms + Data Structures = Evolution Programs, Springer Verlag, Berlin Heidelberg.")]
  [StorableClass]
  public class TranslocationManipulator : PermutationManipulator {
    [StorableConstructor]
    protected TranslocationManipulator(bool deserializing) : base(deserializing) { }
    protected TranslocationManipulator(TranslocationManipulator original, Cloner cloner) : base(original, cloner) { }
    public TranslocationManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TranslocationManipulator(this, cloner);
    }

    /// <summary>
    /// Moves a randomly chosen interval of elements to another (randomly chosen) position in the given
    /// <paramref name="permutation"/> array.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation array to manipulate.</param>
    public static void Apply(IRandom random, Permutation permutation) {
      int breakPoint1, breakPoint2, insertPoint, insertPointLimit;

      breakPoint1 = random.Next(permutation.Length - 1);
      breakPoint2 = random.Next(breakPoint1 + 1, permutation.Length);
      insertPointLimit = permutation.Length - breakPoint2 + breakPoint1 - 1;  // get insertion point in remaining part
      if (insertPointLimit > 0)
        insertPoint = random.Next(insertPointLimit);
      else
        insertPoint = 0;

      Apply(permutation, breakPoint1, breakPoint2, insertPoint);
    }

    public static void Apply(Permutation permutation, int breakPoint1, int breakPoint2, int insertPoint) {
      Permutation original = (Permutation)permutation.Clone();

      int i = 0;  // index in new permutation
      int j = 0;  // index in old permutation
      while (i < original.Length) {
        if (i == insertPoint) {  // copy translocated area
          for (int k = breakPoint1; k <= breakPoint2; k++) {
            permutation[i] = original[k];
            i++;
          }
        }
        if (j == breakPoint1) {  // skip area between breakpoints
          j = breakPoint2 + 1;
        }
        if ((i < original.Length) && (j < original.Length)) {
          permutation[i] = original[j];
          i++;
          j++;
        }
      }
    }

    /// <summary>
    /// Moves a randomly chosen interval of elements to another (randomly chosen) position in the given
    /// <paramref name="permutation"/> array.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    protected override void Manipulate(IRandom random, Permutation permutation) {
      Apply(random, permutation);
    }
  }
}
