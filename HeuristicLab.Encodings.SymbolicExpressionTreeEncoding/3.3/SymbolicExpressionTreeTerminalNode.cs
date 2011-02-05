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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public abstract class SymbolicExpressionTreeTerminalNode : SymbolicExpressionTreeNode {
    private static List<SymbolicExpressionTreeNode> emptyList = new List<SymbolicExpressionTreeNode>();
    public override IList<SymbolicExpressionTreeNode> SubTrees {
      get {
        return SymbolicExpressionTreeTerminalNode.emptyList;
      }
    }

    [StorableConstructor]
    protected SymbolicExpressionTreeTerminalNode(bool deserializing) : base(deserializing) { }
    // don't call storable constructor of base to prevent allocation of sub-trees list in base!
    protected SymbolicExpressionTreeTerminalNode(SymbolicExpressionTreeTerminalNode original, Cloner cloner)
      : base() {
      // symbols are reused
      this.Symbol = original.Symbol;
    }
    protected SymbolicExpressionTreeTerminalNode() : base() { }

    protected SymbolicExpressionTreeTerminalNode(Symbol symbol)
      : base() {
      // symbols are reused
      this.Symbol = symbol;
    }

    public override void AddSubTree(SymbolicExpressionTreeNode tree) {
      throw new NotSupportedException();
    }
    public override void InsertSubTree(int index, SymbolicExpressionTreeNode tree) {
      throw new NotSupportedException();
    }
    public override void RemoveSubTree(int index) {
      throw new NotSupportedException();
    }
  }
}
