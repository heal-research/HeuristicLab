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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.NK {
  [Item("SortedRandomInteractionsInitializer", "Randomly assigns interactions across all bits and sorts components according to bit affinity.")]
  [StorableClass]
  public sealed class SortedRandomInteractionsInitializer : ParameterizedNamedItem, IInteractionInitializer {
    public IConstrainedValueParameter<IBinaryVectorComparer> ComparerParameter {
      get { return (IConstrainedValueParameter<IBinaryVectorComparer>)Parameters["Comparer"]; }
    }
    public IBinaryVectorComparer Comparer {
      get { return ComparerParameter.Value; }
    }

    [StorableConstructor]
    private SortedRandomInteractionsInitializer(bool serializing) : base(serializing) { }
    private SortedRandomInteractionsInitializer(SortedRandomInteractionsInitializer original, Cloner cloner) : base(original, cloner) { }
    public SortedRandomInteractionsInitializer() {
      Parameters.Add(new ConstrainedValueParameter<IBinaryVectorComparer>("Comparer", "Comparison for sorting of component functions."));
      InitializeComparers();
    }

    private void InitializeComparers() {
      foreach (var comparer in ApplicationManager.Manager.GetInstances<IBinaryVectorComparer>())
        ComparerParameter.ValidValues.Add(comparer);
      ComparerParameter.Value = ComparerParameter.ValidValues.First(v => v is AverageBitBinaryVectorComparer);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SortedRandomInteractionsInitializer(this, cloner);
    }

    public BoolMatrix InitializeInterations(int length, int nComponents, int nInteractions, IRandom random) {
      BinaryVector[] components = Enumerable.Range(0, nComponents).Select(i => new BinaryVector(length)).ToArray();
      for (int c = 0; c < components.Length; c++) {
        var indices = Enumerable.Range(0, length).ToList();
        if (indices.Count > c) {
          indices.RemoveAt(c);
          components[c][c] = true;
        }
        while (indices.Count > nInteractions) {
          indices.RemoveAt(random.Next(indices.Count));
        }
        foreach (var i in indices) {
          components[c][i] = true;
        }
      }
      BoolMatrix m = new BoolMatrix(length, nComponents);
      foreach (var c in components.OrderBy(v => v, Comparer).Select((v, j) => new { v, j })) {
        for (int i = 0; i < c.v.Length; i++) {
          m[i, c.j] = c.v[i];
        }
      }
      return m;
    }
  }
}
