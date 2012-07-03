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
  /// Manipulates a permutation array by moving and reversing a randomly chosen interval of elements to another 
  /// (randomly chosen) position in the array.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Fogel, D.B. 1993. Applying Evolutionary Programming to Selected TSP Problems, Cybernetics and Systems, 22, pp 27-36.
  /// </remarks>
  [Item("TranslocationInversionManipulator", "An operator which inverts a randomly chosen part of a permutation and inserts it at a random position. It is implemented as described in Fogel, D.B. 1993. Applying Evolutionary Programming to Selected TSP Problems, Cybernetics and Systems, 22, pp. 27-36.")]
  [StorableClass]
  public class TranslocationInversionManipulator : PermutationManipulator {
    [StorableConstructor]
    protected TranslocationInversionManipulator(bool deserializing) : base(deserializing) { }
    protected TranslocationInversionManipulator(TranslocationInversionManipulator original, Cloner cloner) : base(original, cloner) { }
    public TranslocationInversionManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TranslocationInversionManipulator(this, cloner);
    }

    /// <summary>
    /// Moves a randomly chosen interval of elements to another (randomly chosen) position in the given
    /// <paramref name="permutation"/> array and reverses it.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation array to manipulate.</param>
    public static void Apply(IRandom random, Permutation permutation) {
      Permutation original = (Permutation)permutation.Clone();
      int breakPoint1, breakPoint2, insertPoint, insertPointLimit;

      breakPoint1 = random.Next(original.Length - 1);
      breakPoint2 = random.Next(breakPoint1 + 1, original.Length);
      insertPointLimit = original.Length - breakPoint2 + breakPoint1 - 1;  // get insertion point in remaining part
      if (insertPointLimit > 0)
        insertPoint = random.Next(insertPointLimit);
      else
        insertPoint = 0;

      int i = 0;  // index in new permutation
      int j = 0;  // index in old permutation
      while (i < original.Length) {
        if (i == insertPoint) {  // copy translocated area
          for (int k = breakPoint2; k >= breakPoint1; k--) {
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
    /// <paramref name="permutation"/> array and reverses it.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    protected override void Manipulate(IRandom random, Permutation permutation) {
      Apply(random, permutation);
    }
  }
}
