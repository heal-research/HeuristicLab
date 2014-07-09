#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix {
  [Item("JSMShiftChangeManipulator", "Represents a manipulation operation where the operations of a randomly determined job are shifted in one direction for all resources.")]
  [StorableClass]
  public class JSMShiftChangeManipulator : JSMManipulator {

    [StorableConstructor]
    protected JSMShiftChangeManipulator(bool deserializing) : base(deserializing) { }
    protected JSMShiftChangeManipulator(JSMShiftChangeManipulator original, Cloner cloner) : base(original, cloner) { }
    public JSMShiftChangeManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMShiftChangeManipulator(this, cloner);
    }

    public static void Apply(IRandom random, JSMEncoding individual) {
      int nrOfJobs = individual.JobSequenceMatrix[0].Length;
      int jobIndex = random.Next(nrOfJobs);
      int signedFactor = random.Next(2);
      for (int permutationIndex = 0; permutationIndex < individual.JobSequenceMatrix.Count; permutationIndex++) {
        int i = 0;
        Permutation currentPermutation = individual.JobSequenceMatrix[permutationIndex];
        while (i < currentPermutation.Length && currentPermutation[i] != jobIndex)
          i++;
        int shift = (signedFactor == 0) ? random.Next(nrOfJobs / 2) * -1 : random.Next(nrOfJobs / 2);
        int newIndex = shift + i;
        if (newIndex < 0) newIndex = 0;
        if (newIndex >= nrOfJobs) newIndex = nrOfJobs - 1;
        List<int> aux = currentPermutation.ToList<int>();
        int swap = currentPermutation[i];
        aux.RemoveAt(i);
        aux.Insert(newIndex, swap);
        individual.JobSequenceMatrix[permutationIndex] = new Permutation(PermutationTypes.Absolute, aux.ToArray());
      }
    }

    protected override void Manipulate(IRandom random, IScheduleEncoding encoding) {
      var solution = encoding as JSMEncoding;
      if (solution == null) throw new InvalidOperationException("Encoding is not of type JSMEncoding");
      Apply(random, solution);
    }
  }
}
