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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.NK {
  [Item("LimitedRandomInteractionsInitializer", "Randomly assignes interactions across bits in the vicinity of each other respecting the maximum distances if possible.")]
  [StorableType("A49E3167-FC04-40F8-A45C-BA8818CE2065")]
  public sealed class LimitedRandomInteractionsInitializer : ParameterizedNamedItem, IInteractionInitializer {
    private class Bounds {
      public readonly int Min;
      public readonly int Max;
      public Bounds(int min, int max) {
        Min = Math.Min(min, max);
        Max = Math.Max(min, max);
      }
      public int Bounded(int n) {
        return Math.Max(Min, Math.Min(Max, n));
      }
    }

    public IValueParameter<IntValue> MaximumDistanceParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumDistance"]; }
    }
    public IValueParameter<DoubleValue> MaximumDistanceRatioParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["MaximumDistanceRatio"]; }
    }

    [StorableConstructor]
    private LimitedRandomInteractionsInitializer(StorableConstructorFlag _) : base(_) { }
    private LimitedRandomInteractionsInitializer(LimitedRandomInteractionsInitializer original, Cloner cloner) : base(original, cloner) { }
    public LimitedRandomInteractionsInitializer() {
      Parameters.Add(new ValueParameter<IntValue>("MaximumDistance", "Maximum distance of interactions in bits or 0 for unlimited"));
      Parameters.Add(new ValueParameter<DoubleValue>("MaximumDistanceRatio", "Maximum distance of interactions as ratio of the total length or 0 for unlimited"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LimitedRandomInteractionsInitializer(this, cloner);
    }

    private int MaximumDistance(int length, int nInteractions) {
      int maxBitDist = MaximumDistanceParameter.Value.Value;
      double maxDistRatio = MaximumDistanceRatioParameter.Value.Value;
      maxBitDist = Math.Min(
        maxBitDist == 0 ? length : maxBitDist,
        maxDistRatio.IsAlmost(0.0) ? length : (int)Math.Round(maxDistRatio * length));
      if (maxBitDist * 2 < nInteractions)
        maxBitDist = (int)Math.Ceiling(0.5 * nInteractions);
      return maxBitDist;
    }

    public BoolMatrix InitializeInterations(int length, int nComponents, int nInteractions, IRandom random) {
      BoolMatrix m = new BoolMatrix(length, nComponents);
      int maxBitDistance = MaximumDistance(length, nInteractions);
      var minBounds = new Bounds(0, length - nInteractions);
      var maxBounds = new Bounds(nInteractions, length - 1);
      for (int c = 0; c < m.Columns; c++) {
        int min = minBounds.Bounded(c - maxBitDistance);
        int max = maxBounds.Bounded(c + maxBitDistance);
        var indices = Enumerable.Range(min, max - min).ToList();
        indices.Remove(c);
        m[c, c] = true;
        while (indices.Count > nInteractions) {
          indices.RemoveAt(random.Next(indices.Count));
        }
        foreach (var i in indices) {
          m[i, c] = true;
        }
      }
      return m;
    }
  }
}
