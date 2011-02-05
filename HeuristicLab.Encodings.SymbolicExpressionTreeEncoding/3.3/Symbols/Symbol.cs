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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols {
  [StorableClass]
  [Item("Symbol", "Represents a symbol in a symbolic function tree.")]
  public abstract class Symbol : NamedItem {
    #region Properties
    [Storable]
    private double initialFrequency;
    public double InitialFrequency {
      get { return initialFrequency; }
      set {
        if (value < 0.0) throw new ArgumentException("InitialFrequency must be positive");
        if (value != initialFrequency) {
          initialFrequency = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    public override bool CanChangeName {
      get { return false; }
    }
    #endregion

    [StorableConstructor]
    protected Symbol(bool deserializing) : base(deserializing) { }
    protected Symbol(Symbol original, Cloner cloner)
      : base(original, cloner) {
      initialFrequency = original.initialFrequency;
    }
    protected Symbol()
      : base() {
      initialFrequency = 1.0;
    }

    protected Symbol(string name, string description)
      : base(name, description) {
      initialFrequency = 1.0;
    }


    public virtual SymbolicExpressionTreeNode CreateTreeNode() {
      return new SymbolicExpressionTreeNode(this);
    }

    #region events
    public event EventHandler Changed;
    protected void OnChanged(EventArgs e) {
      EventHandler handlers = Changed;
      if (handlers != null)
        handlers(this, e);
    }
    #endregion
  }
}
