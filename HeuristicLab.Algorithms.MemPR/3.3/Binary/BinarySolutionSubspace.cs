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
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.Binary {
  [Item("Solution subspace (binary)", "")]
  [StorableClass]
  public sealed class BinarySolutionSubspace : Item, ISolutionSubspace {

    [Storable]
    private bool[] subspace;
    public bool[] Subspace { get { return subspace; } }

    [StorableConstructor]
    private BinarySolutionSubspace(bool deserializing) : base(deserializing) { }
    private BinarySolutionSubspace(BinarySolutionSubspace original, Cloner cloner)
      : base(original, cloner) {
      subspace = (bool[])original.subspace.Clone();
    }
    public BinarySolutionSubspace(bool[] subspace) {
      this.subspace = subspace;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinarySolutionSubspace(this, cloner);
    }
  }
}
