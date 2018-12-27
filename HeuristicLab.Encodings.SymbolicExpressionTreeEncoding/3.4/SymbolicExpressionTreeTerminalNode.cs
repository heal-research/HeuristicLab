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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public abstract class SymbolicExpressionTreeTerminalNode : SymbolicExpressionTreeNode {
    public override IEnumerable<ISymbolicExpressionTreeNode> Subtrees {
      get { return Enumerable.Empty<ISymbolicExpressionTreeNode>(); }
    }

    [StorableConstructor]
    protected SymbolicExpressionTreeTerminalNode(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeTerminalNode(SymbolicExpressionTreeTerminalNode original, Cloner cloner) : base(original, cloner) { }
    protected SymbolicExpressionTreeTerminalNode() : base() { }

    protected SymbolicExpressionTreeTerminalNode(ISymbol symbol)
      : base() {
      // symbols are reused
      this.Symbol = symbol;
    }

    public override int IndexOfSubtree(ISymbolicExpressionTreeNode tree) {
      return -1;
    }
    public override ISymbolicExpressionTreeNode GetSubtree(int index) {
      throw new NotSupportedException();
    }
    public override void AddSubtree(ISymbolicExpressionTreeNode tree) {
      throw new NotSupportedException();
    }
    public override void InsertSubtree(int index, ISymbolicExpressionTreeNode tree) {
      throw new NotSupportedException();
    }
    public override void RemoveSubtree(int index) {
      throw new NotSupportedException();
    }
  }
}
