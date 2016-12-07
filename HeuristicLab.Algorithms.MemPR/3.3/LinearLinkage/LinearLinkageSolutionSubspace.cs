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

using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.LinearLinkage {
  [Item("Solution subspace (linear linkage)", "")]
  [StorableClass]
  public sealed class LinearLinkageSolutionSubspace : Item, ISolutionSubspace<Encodings.LinearLinkageEncoding.LinearLinkage> {

    [Storable]
    private bool[] subspace;
    public bool[] Subspace { get { return subspace; } }

    [StorableConstructor]
    private LinearLinkageSolutionSubspace(bool deserializing) : base(deserializing) { }
    private LinearLinkageSolutionSubspace(LinearLinkageSolutionSubspace original, Cloner cloner)
      : base(original, cloner) {
      subspace = (bool[])original.subspace.Clone();
    }
    public LinearLinkageSolutionSubspace(bool[] subspace) {
      this.subspace = subspace;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearLinkageSolutionSubspace(this, cloner);
    }
  }
}
