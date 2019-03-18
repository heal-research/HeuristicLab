#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("JSMJobbasedOrderCrossover", "Represents a crossover operation swapping subsequences of the parents to generate offspring.")]
  [StorableClass]
  public class JSMJOXCrossover : JSMCrossover {

    [StorableConstructor]
    protected JSMJOXCrossover(bool deserializing) : base(deserializing) { }
    protected JSMJOXCrossover(JSMJOXCrossover original, Cloner cloner) : base(original, cloner) { }
    public JSMJOXCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMJOXCrossover(this, cloner);
    }

    public static JSMEncoding Apply(IRandom random, JSMEncoding p1, JSMEncoding p2) {
      var result = new JSMEncoding(random.Next());

      int nrOfResources = p1.JobSequenceMatrix.Count;
      int nrOfJobs = p1.JobSequenceMatrix[0].Length;

      //Determine randomly which jobindexes persist
      var persist = new BoolArray(nrOfJobs);
      for (int i = 0; i < persist.Length; i++) {
        persist[i] = random.Next(2) == 1;
      }

      bool dominantParent = random.Next(2) == 1;
      JSMEncoding parent1 = dominantParent ? p1 : p2;
      JSMEncoding parent2 = dominantParent ? p2 : p1;

      //Fill childmatrix with values
      for (int resIndex = 0; resIndex < nrOfResources; resIndex++) {
        result.JobSequenceMatrix.Add(new Permutation(PermutationTypes.Absolute, nrOfJobs));
        int parent2index = 0;
        for (int jobIndex = 0; jobIndex < nrOfJobs; jobIndex++) {
          if (persist[parent1.JobSequenceMatrix[resIndex][jobIndex]])
            result.JobSequenceMatrix[resIndex][jobIndex] = parent1.JobSequenceMatrix[resIndex][jobIndex];
          else {
            while (persist[parent2.JobSequenceMatrix[resIndex][parent2index]])
              parent2index++;
            result.JobSequenceMatrix[resIndex][jobIndex] = parent2.JobSequenceMatrix[resIndex][parent2index];
            parent2index++;
          }
        }
      }

      return result;
    }

    public override JSMEncoding Cross(IRandom random, JSMEncoding parent1, JSMEncoding parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
