#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("Edge", "An edge between two integer nodes and a flag for the direction.")]
  [StorableClass]
  public class Edge : Item {
    [Storable]
    public int Source { get; private set; }
    [Storable]
    public int Target { get; private set; }
    [Storable]
    public bool Bidirectional { get; private set; }

    [StorableConstructor]
    protected Edge(bool deserializing) : base(deserializing) { }
    protected Edge(Edge original, Cloner cloner)
      : base(original, cloner) {
      Source = original.Source;
      Target = original.Target;
      Bidirectional = original.Bidirectional;
    }
    public Edge(int source, int target, bool bidirectional)
      : base() {
      Source = source;
      Target = target;
      Bidirectional = bidirectional;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Edge(this, cloner);
    }
  }
}
