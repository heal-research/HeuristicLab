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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// Manipulates a permutation array by moving randomly one element to another position in the array.
  /// </summary> 
  /// <remarks>
  /// It is implemented as described in Fogel, D.B. (1988). An Evolutionary Approach to the Traveling Salesman Problem, Biological Cybernetics, 60, pp. 139-144.
  /// </remarks>
  [Item("InsertionManipulator", "An operator which moves randomly one element to another position in the permutation (Insertion is a special case of Translocation). It is implemented as described in Fogel, D.B. (1988). An Evolutionary Approach to the Traveling Salesman Problem, Biological Cybernetics, 60, pp. 139-144.")]
  [StorableType("E8C09728-ACB7-491B-B87C-BE8E2B5A5B0B")]
  public class InsertionManipulator : PermutationManipulator {
    [StorableConstructor]
    protected InsertionManipulator(StorableConstructorFlag _) : base(_) { }
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
      var cutIndex = random.Next(permutation.Length);
      var insertIndex = random.Next(permutation.Length);

      if (cutIndex == insertIndex) return;

      permutation.Move(cutIndex, cutIndex, insertIndex);
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
