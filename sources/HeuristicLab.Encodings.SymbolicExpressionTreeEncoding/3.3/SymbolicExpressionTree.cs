#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("SymbolicExpressionTree", "Represents a symbolic expression tree.")]
  public class SymbolicExpressionTree : Item {
    private SymbolicExpressionTreeNode root;
    public SymbolicExpressionTreeNode Root {
      get { return root; }
      set {
        if (value == null) throw new ArgumentNullException();
        else if (value != root) {
          root = value;
          OnToStringChanged();
        }
      }
    }

    public int Size {
      get {
        return root.GetSize();
      }
    }

    public int Height {
      get {
        return root.GetHeight();
      }
    }

    public SymbolicExpressionTree() : base() { }

    public SymbolicExpressionTree(SymbolicExpressionTreeNode root) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      SymbolicExpressionTree clone = new SymbolicExpressionTree();
      cloner.RegisterClonedObject(this, clone);
      clone.root = (SymbolicExpressionTreeNode)this.root.Clone();
      return clone;
    }
  }
}
