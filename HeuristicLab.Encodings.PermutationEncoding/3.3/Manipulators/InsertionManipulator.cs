#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Manipulates a permutation array by moving randomly one element to another position in the array.
  /// </summary> 
  /// <remarks>
  /// It is implemented as described in Fogel, D.B. (1988). An Evolutionary Approach to the Traveling Salesman Problem, Biological Cybernetics, 60, pp. 139-144.
  /// </remarks>
  [Item("InsertionManipulator", "An operator which moves randomly one element to another position in the permutation (Insertion is a special case of Translocation). It is implemented as described in Fogel, D.B. (1988). An Evolutionary Approach to the Traveling Salesman Problem, Biological Cybernetics, 60, pp. 139-144.")]
  [StorableClass]
  public class InsertionManipulator : PermutationManipulator {
    [StorableConstructor]
    protected InsertionManipulator(bool deserializing) : base(deserializing) { }
    protected InsertionManipulator(InsertionManipulator original, Cloner cloner) : base(original, cloner) { }
    public InsertionManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InsertionManipulator(this, cloner);
    }

    /// <summary>
    /// Moves an randomly chosen element in the specified <paramref name="permutation"/> array 
    /// to another randomly generated position.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    public static void Apply(IRandom random, Permutation permutation) {
      Permutation original = (Permutation)permutation.Clone();
      int cutIndex, insertIndex, number;

      cutIndex = random.Next(original.Length);
      insertIndex = random.Next(original.Length);
      number = original[cutIndex];

      int i = 0;  // index in new permutation
      int j = 0;  // index in old permutation
      while (i < original.Length) {
        if (j == cutIndex) {
          j++;
        }
        if (i == insertIndex) {
          permutation[i] = number;
          i++;
        }
        if ((i < original.Length) && (j < original.Length)) {
          permutation[i] = original[j];
          i++;
          j++;
        }
      }
    }

    /// <summary>
    /// Moves an randomly chosen element in the specified <paramref name="permutation"/> array 
    /// to another randomly generated position.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    protected override void Manipulate(IRandom random, Permutation permutation) {
      Apply(random, permutation);
    }
  }
}
