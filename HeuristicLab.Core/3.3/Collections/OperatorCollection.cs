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
using HEAL.Attic;

namespace HeuristicLab.Core {
  [StorableType("35A02DD1-33F8-4F0D-9DD5-93ED71EE1591")]
  [Item("Operator Collection", "Represents a collection of operators.")]
  public class OperatorCollection : ItemCollection<IOperator> {
    [StorableConstructor]
    protected OperatorCollection(StorableConstructorFlag _) : base(_) { }
    protected OperatorCollection(OperatorCollection original, Cloner cloner) : base(original, cloner) { }
    public OperatorCollection() : base() { }
    public OperatorCollection(IEnumerable<IOperator> collection) : base(collection) { }

    public override IDeepCloneable Clone(Cloner cloner) { return new OperatorCollection(this, cloner); }
  }
}
