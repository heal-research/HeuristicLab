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

using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.PriorityRulesVector {
  [Item("PriorityRulesVectorEncoding", "Represents an encoding for a Scheduling Problem.")]
  [StorableClass]
  public class PRVEncoding : Item, IScheduleEncoding {
    [Storable]
    public IntegerVector PriorityRulesVector { get; set; }

    [Storable]
    private IntValue nrOfRules;
    public IntValue NrOfRules {
      get {
        return nrOfRules;
      }
    }

    [StorableConstructor]
    protected PRVEncoding(bool deserializing) : base(deserializing) { }
    protected PRVEncoding(PRVEncoding original, Cloner cloner)
      : base(original, cloner) {
      this.nrOfRules = cloner.Clone(original.NrOfRules);
      this.PriorityRulesVector = cloner.Clone(original.PriorityRulesVector);
    }
    public PRVEncoding(int nrOfRules)
      : base() {
      this.nrOfRules = new IntValue(nrOfRules);
      this.PriorityRulesVector = new IntegerVector();
    }
    public PRVEncoding(IntegerVector iv, IntValue nrOfRules)
      : base() {
      this.nrOfRules = (IntValue)nrOfRules.Clone();
      this.PriorityRulesVector = (IntegerVector)iv.Clone();
    }
    public PRVEncoding(int length, IRandom random, int min, int max, IntValue nrOfRules)
      : base() {
      this.nrOfRules = (IntValue)nrOfRules.Clone();
      this.PriorityRulesVector = new IntegerVector(length, random, min, max);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PRVEncoding(this, cloner);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[ ");

      foreach (int i in PriorityRulesVector) {
        sb.Append(i + " ");
      }

      sb.Append("]");
      return sb.ToString();
    }
  }
}
