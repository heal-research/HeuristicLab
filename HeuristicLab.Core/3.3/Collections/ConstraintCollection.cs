#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("ConstraintCollection", "Represents a collection of constraints.")]
  public class ConstraintCollection : ItemCollection<IConstraint> {
    [StorableConstructor]
    protected ConstraintCollection(bool deserializing) : base(deserializing) { }
    protected ConstraintCollection(ConstraintCollection original, Cloner cloner) : base(original, cloner) { }
    public ConstraintCollection() : base() { }
    public ConstraintCollection(int capacity) : base(capacity) { }
    public ConstraintCollection(IEnumerable<IConstraint> collection) : base(collection) { }

    public override IDeepCloneable Clone(Cloner cloner) { return new ConstraintCollection(this, cloner); }
  }
}
