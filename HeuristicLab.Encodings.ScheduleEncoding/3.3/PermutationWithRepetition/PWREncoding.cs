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

using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("PermutationWithRepetitionEncoding", "Represents a encoding for a standard JobShop Scheduling Problem.")]
  [StorableType("31A66AC4-897D-4986-A6FC-DC301DC06278")]
  public class PWREncoding : Item, ISchedule {

    [Storable]
    public IntegerVector PermutationWithRepetition { get; set; }

    [StorableConstructor]
    protected PWREncoding(StorableConstructorFlag _) : base(_) { }
    protected PWREncoding(PWREncoding original, Cloner cloner)
      : base(original, cloner) {
      this.PermutationWithRepetition = cloner.Clone(original.PermutationWithRepetition);
    }
    public PWREncoding()
      : base() {
      PermutationWithRepetition = new IntegerVector();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PWREncoding(this, cloner);
    }

    public PWREncoding(int nrOfJobs, int nrOfResources, IRandom random)
      : base() {
      PermutationWithRepetition = new IntegerVector(nrOfJobs * nrOfResources);
      int[] lookUpTable = new int[nrOfJobs];

      for (int i = 0; i < PermutationWithRepetition.Length; i++) {
        int newValue = random.Next(nrOfJobs);
        while (lookUpTable[newValue] >= nrOfResources)
          newValue = random.Next(nrOfJobs);

        PermutationWithRepetition[i] = newValue;

        lookUpTable[newValue]++;
      }
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[ ");
      foreach (int i in PermutationWithRepetition) {
        sb.Append(i + " ");
      }
      sb.Append("]");

      return sb.ToString();
    }
  }
}
