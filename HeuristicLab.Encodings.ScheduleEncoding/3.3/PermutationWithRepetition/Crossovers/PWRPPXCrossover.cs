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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition {
  [Item("PWRPPXCrossover", "Represents a crossover operation swapping sequences of the parents to generate offspring.")]
  [StorableType("B5749161-8599-4F89-AAC7-414C53576B10")]
  public class PWRPPXCrossover : PWRCrossover {

    [StorableConstructor]
    protected PWRPPXCrossover(StorableConstructorFlag _) : base(_) { }
    protected PWRPPXCrossover(PWRPPXCrossover original, Cloner cloner) : base(original, cloner) { }
    public PWRPPXCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PWRPPXCrossover(this, cloner);
    }

    public static PWREncoding Apply(IRandom random, PWREncoding parent1, PWREncoding parent2) {
      var result = new PWREncoding();
      var p1 = ((IntegerVector)(parent1.PermutationWithRepetition.Clone())).ToList();
      var p2 = ((IntegerVector)(parent2.PermutationWithRepetition.Clone())).ToList();
      var child = new List<int>();

      var lookUpTable = new bool[parent1.PermutationWithRepetition.Length];
      for (int i = 0; i < lookUpTable.Length; i++) {
        lookUpTable[i] = random.Next(2) == 1;
      }

      foreach (bool b in lookUpTable) {
        if (b) {
          child.Add(p1[0]);
          p2.Remove(p1[0]);
          p1.RemoveAt(0);
        } else {
          child.Add(p2[0]);
          p1.Remove(p2[0]);
          p2.RemoveAt(0);
        }
      }

      result.PermutationWithRepetition = new IntegerVector(child.ToArray());

      return result;
    }

    public override PWREncoding Cross(IRandom random, PWREncoding parent1, PWREncoding parent2) {
      return Apply(random, parent1, parent2);
    }

  }
}
