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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("PWRInsertionManipulator", "Represents a manipulation operation inserting parts of the individual at another position.")]
  [StorableClass]
  public class PWRInsertionManipulator : PWRManipulator {
    [StorableConstructor]
    protected PWRInsertionManipulator(bool deserializing) : base(deserializing) { }
    protected PWRInsertionManipulator(PWRInsertionManipulator original, Cloner cloner) : base(original, cloner) { }
    public PWRInsertionManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PWRInsertionManipulator(this, cloner);
    }

    public static void Apply(IRandom random, PWREncoding individual) {
      int cutIndex = random.Next(individual.PermutationWithRepetition.Length);
      int insertIndex = random.Next(individual.PermutationWithRepetition.Length);
      List<int> perm = ((IntegerVector)(individual.PermutationWithRepetition.Clone())).ToList();
      int aux = perm[cutIndex];
      if (cutIndex > insertIndex) {
        perm.RemoveAt(cutIndex);
        perm.Insert(insertIndex, aux);
      } else {
        perm.Insert(insertIndex, aux);
        perm.RemoveAt(cutIndex);
      }
      individual.PermutationWithRepetition = new IntegerVector(perm.ToArray());
    }

    protected override void Manipulate(IRandom random, PWREncoding individual) {
      Apply(random, individual);
    }

  }
}
