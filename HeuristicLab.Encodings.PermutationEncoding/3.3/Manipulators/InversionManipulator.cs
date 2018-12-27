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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// An operator which inverts a randomly chosen part of a permutation.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.<br />
  /// </remarks>
  [Item("InversionManipulator", "An operator which inverts a randomly chosen part of a permutation. It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public class InversionManipulator : PermutationManipulator {
    [StorableConstructor]
    protected InversionManipulator(bool deserializing) : base(deserializing) { }
    protected InversionManipulator(InversionManipulator original, Cloner cloner) : base(original, cloner) { }
    public InversionManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InversionManipulator(this, cloner);
    }

    /// <summary>
    /// Inverts a randomly chosen part of a permutation.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    public static void Apply(IRandom random, Permutation permutation) {
      int breakPoint1, breakPoint2;

      breakPoint1 = random.Next(permutation.Length - 1);
      do {
        breakPoint2 = random.Next(permutation.Length - 1);
      } while (breakPoint2 == breakPoint1);
      if (breakPoint2 < breakPoint1) { int h = breakPoint1; breakPoint1 = breakPoint2; breakPoint2 = h; }
      Apply(permutation, breakPoint1, breakPoint2);
    }

    public static void Apply(Permutation permutation, int breakPoint1, int breakPoint2) {
      permutation.Reverse(breakPoint1, breakPoint2 - breakPoint1 + 1);
    }

    /// <summary>
    /// Inverts a randomly chosen part of a permutation.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    protected override void Manipulate(IRandom random, Permutation permutation) {
      Apply(random, permutation);
    }
  }
}
