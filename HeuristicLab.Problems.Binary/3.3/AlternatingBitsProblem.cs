#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Binary {
  [Item("Alternating Bits Problem", "Represents a problem whose objective is to achieve an alternating sequence of bits, long sequences of consecutive bits receive a penalty.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 220)]
  [StorableClass]
  public class AlternatingBitsProblem : BinaryProblem {
    public override bool Maximization {
      get { return false; }
    }

    public AlternatingBitsProblem()
      : base() {
      Encoding.Length = 256;
      BestKnownQuality = 0;
    }

    [StorableConstructor]
    protected AlternatingBitsProblem(bool deserializing) : base(deserializing) { }
    protected AlternatingBitsProblem(AlternatingBitsProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlternatingBitsProblem(this, cloner);
    }

    public override double Evaluate(BinaryVector vector, IRandom random) {
      var quality = 0.0;
      var curr = vector[0];
      var lastOk = 0;
      for (var i = 1; i < vector.Length; i++) {
        if (curr && !vector[i] || !curr && vector[i]) {
          lastOk = i;
        } else quality += (i - lastOk);
        curr = vector[i];
      }
      return quality;
    }
  }
}
