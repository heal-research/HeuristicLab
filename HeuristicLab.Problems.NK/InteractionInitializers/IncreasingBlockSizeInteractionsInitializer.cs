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
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.NK {
  [Item("IncreasingBlockSizeInteractionsInitializer", "Randomly assignes interactions across all bits but makes sure that different numbers of ineractions are applied to different bits.")]
  [StorableClass]
  public sealed class IncreasingBlockSizeInteractionsInitializer : ParameterizedNamedItem, IInteractionInitializer {
    [StorableConstructor]
    private IncreasingBlockSizeInteractionsInitializer(bool serializing) : base(serializing) { }
    private IncreasingBlockSizeInteractionsInitializer(IncreasingBlockSizeInteractionsInitializer original, Cloner cloner) : base(original, cloner) { }
    public IncreasingBlockSizeInteractionsInitializer() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IncreasingBlockSizeInteractionsInitializer(this, cloner);
    }

    public BoolMatrix InitializeInterations(int length, int nComponents, int nInteractions, IRandom random) {
      Dictionary<int, int> bitInteractionCounts = new Dictionary<int, int>();
      for (int i = 0; i < length; i++) {
        int count = nInteractions * 2 * i / length;
        if (count > 0)
          bitInteractionCounts[i] = count;
      }
      List<BinaryVector> components = new List<BinaryVector>();
      while (bitInteractionCounts.Count > 0) {
        BinaryVector component = new BinaryVector(length);
        for (int i = 0; i < nInteractions; i++) {
          while (bitInteractionCounts.Count > 0) {
            int bit = bitInteractionCounts.ElementAt(random.Next(bitInteractionCounts.Count)).Key;
            if (bitInteractionCounts[bit]-- <= 0)
              bitInteractionCounts.Remove(bit);
            if (!component[bit]) {
              component[bit] = true;
              break;
            }
          }
        }
        components.Add(component);
      }
      BoolMatrix m = new BoolMatrix(length, components.Count);
      foreach (var c in components.Select((v, j) => new { v, j })) {
        for (int i = 0; i < c.v.Length; i++) {
          m[i, c.j] = c.v[i];
        }
      }
      return m;
    }
  }
}
