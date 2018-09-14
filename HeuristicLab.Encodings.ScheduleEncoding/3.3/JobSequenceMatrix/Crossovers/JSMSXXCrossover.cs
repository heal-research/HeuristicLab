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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix {
  [Item("JSMSubsequenceExchangeCrossover", "Represents a crossover operation identifiying and exchanging equal subsequences of the parents to generate offspring.")]
  [StorableClass]
  public class JSMSXXCrossover : JSMCrossover {

    [StorableConstructor]
    protected JSMSXXCrossover(bool deserializing) : base(deserializing) { }
    protected JSMSXXCrossover(JSMSXXCrossover original, Cloner cloner) : base(original, cloner) { }
    public JSMSXXCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMSXXCrossover(this, cloner);
    }

    public static JSMEncoding Apply(IRandom random, JSMEncoding parent1, JSMEncoding parent2) {
      var result = new JSMEncoding();
      int subSequenceLength = random.Next(parent1.JobSequenceMatrix[0].Length);
      for (int i = 0; i < parent1.JobSequenceMatrix.Count; i++) {
        var p1 = (Permutation)parent1.JobSequenceMatrix[i].Clone();
        var p2 = (Permutation)parent2.JobSequenceMatrix[i].Clone();
        FindAndExchangeSubsequences(p1, p2, subSequenceLength);
        result.JobSequenceMatrix.Add(p1);
      }
      return result;
    }

    private static void FindAndExchangeSubsequences(Permutation p1, Permutation p2, int subSequenceLength) {
      for (int i = 0; i <= p1.Length - subSequenceLength; i++) {
        int[] ss1 = GetSubSequenceAtPosition(p1, i, subSequenceLength);
        for (int j = 0; j <= p2.Length - subSequenceLength; j++) {
          int[] ss2 = GetSubSequenceAtPosition(p2, j, subSequenceLength);
          if (AreEqualSubsequences(ss1, ss2)) {
            ExchangeSubsequences(p1, i, p2, j, subSequenceLength);
            return;
          }
        }
      }
    }

    private static void ExchangeSubsequences(Permutation p1, int index1, Permutation p2, int index2, int subSequenceLength) {
      var aux = (Permutation)p1.Clone();
      for (int i = 0; i < subSequenceLength; i++) {
        p1[i + index1] = p2[i + index2];
        p2[i + index2] = aux[i + index1];
      }
    }

    private static bool AreEqualSubsequences(int[] ss1, int[] ss2) {
      int counter = 0;
      for (int i = 0; i < ss1.Length; i++) {
        for (int j = 0; j < ss2.Length; j++) {
          if (ss1[i] == ss2[j])
            counter++;
        }
      }
      return counter == ss1.Length;
    }

    private static int[] GetSubSequenceAtPosition(Permutation p1, int index, int subSequenceLength) {
      var result = new int[subSequenceLength];
      for (int i = 0; i < subSequenceLength; i++)
        result[i] = p1[i + index];

      return result;
    }

    public override JSMEncoding Cross(IRandom random, JSMEncoding parent1, JSMEncoding parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
