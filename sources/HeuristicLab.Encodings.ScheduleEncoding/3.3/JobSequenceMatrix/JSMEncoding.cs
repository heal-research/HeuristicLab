#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix {
  [Item("JobSequenceMatrixEncoding", "Represents an encoding for a scheduling Problem using a list of job sequences to deliver scheduleinformation.")]
  [StorableClass]
  public class JSMEncoding : Item, IScheduleEncoding {
    [Storable]
    public ItemList<Permutation> JobSequenceMatrix { get; set; }

    [StorableConstructor]
    protected JSMEncoding(bool deserializing) : base(deserializing) { }
    protected JSMEncoding(JSMEncoding original, Cloner cloner)
      : base(original, cloner) {
      this.JobSequenceMatrix = cloner.Clone(original.JobSequenceMatrix);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMEncoding(this, cloner);
    }
    public JSMEncoding()
      : base() {
      JobSequenceMatrix = new ItemList<Permutation>();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[ ");

      foreach (Permutation p in JobSequenceMatrix) {
        sb.Append(p.ToString() + " \n");
      }

      sb.Append("]");
      return sb.ToString();
    }


    public override bool Equals(object obj) {
      if (obj.GetType() == typeof(JSMEncoding))
        return AreEqual(this, obj as JSMEncoding);

      return false;
    }
    public override int GetHashCode() {
      if (JobSequenceMatrix.Count == 1)
        return JobSequenceMatrix[0].GetHashCode();
      if (JobSequenceMatrix.Count == 2)
        return JobSequenceMatrix[0].GetHashCode() ^ JobSequenceMatrix[1].GetHashCode();
      return 0;
    }
    private static bool AreEqual(JSMEncoding jSMEncoding1, JSMEncoding jSMEncoding2) {
      if (jSMEncoding1.JobSequenceMatrix.Count != jSMEncoding2.JobSequenceMatrix.Count)
        return false;
      for (int i = 0; i < jSMEncoding1.JobSequenceMatrix.Count; i++) {
        if (!AreEqual(jSMEncoding1.JobSequenceMatrix[i], jSMEncoding2.JobSequenceMatrix[i]))
          return false;
      }
      return true;
    }

    private static bool AreEqual(Permutation p1, Permutation p2) {
      if (p1.Length != p2.Length)
        return false;
      for (int i = 0; i < p1.Length; i++) {
        if (p1[i] != p2[i])
          return false;
      }
      return true;
    }
  }
}
