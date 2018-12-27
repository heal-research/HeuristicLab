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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("OperatorList", "Represents a list of operators.")]
  public class OperatorList : ItemList<IOperator> {
    [StorableConstructor]
    protected OperatorList(bool deserializing) : base(deserializing) { }
    protected OperatorList(OperatorList original, Cloner cloner) : base(original, cloner) { }
    public OperatorList() : base() { }
    public OperatorList(int capacity) : base(capacity) { }
    public OperatorList(IEnumerable<IOperator> collection) : base(collection) { }

    public override IDeepCloneable Clone(Cloner cloner) { return new OperatorList(this, cloner); }
  }
}
