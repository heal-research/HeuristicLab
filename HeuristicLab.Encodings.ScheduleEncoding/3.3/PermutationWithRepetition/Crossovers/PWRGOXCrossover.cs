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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition {
  [Item("PWRGeneralizationOrderCrossover", "Represents a crossover operation swapping sequences of the parents to generate offspring.")]
  [StorableClass]
  public class PWRGOXCrossover : PWRCrossover {

    [StorableConstructor]
    protected PWRGOXCrossover(bool deserializing) : base(deserializing) { }
    protected PWRGOXCrossover(PWRGOXCrossover original, Cloner cloner) : base(original, cloner) { }
    public PWRGOXCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PWRGOXCrossover(this, cloner);
    }

    private static int[] GetLookUpForIndividual(List<int> p) {
      var result = new int[p.Count];
      var lookUpTable = new Dictionary<int, int>();

      for (int i = 0; i < p.Count; i++) {
        if (!lookUpTable.ContainsKey(p[i]))
          lookUpTable.Add(p[i], 0);
        result[i] = lookUpTable[p[i]];
        lookUpTable[p[i]]++;
      }

      return result;
    }

    public static PWREncoding Apply(IRandom random, PWREncoding parent1, PWREncoding parent2) {
      var result = new PWREncoding();

      var p1 = ((IntegerVector)(parent1.PermutationWithRepetition.Clone())).ToList();
      var p2 = ((IntegerVector)(parent2.PermutationWithRepetition.Clone())).ToList();
      var child = new List<int>();

      int[] lookUpArrayP1 = GetLookUpForIndividual(p1);
      int[] lookUpArrayP2 = GetLookUpForIndividual(p2);

      int xStringLength = p1.Count / 2;
      int xStringPosition = random.Next(p1.Count / 2);

      //insert elements from parent1 that dont conflict with the "selected" elements form parent2
      for (int i = 0; i < p1.Count; i++) {
        bool isSelected = false;
        for (int j = xStringPosition; j < xStringPosition + xStringLength; j++) {
          if (p1[i] == p2[j] && lookUpArrayP1[i] == lookUpArrayP2[j])
            isSelected = true;
        }
        if (!isSelected)
          child.Add(p1[i]);
      }

      for (int i = xStringPosition; i < xStringPosition + xStringLength; i++) {
        child.Insert(i, p2[i]);
      }

      result.PermutationWithRepetition = new IntegerVector(child.ToArray());
      return result;
    }

    public override PWREncoding Cross(IRandom random, PWREncoding parent1, PWREncoding parent2) {
      return Apply(random, parent1, parent2);
    }

  }
}
