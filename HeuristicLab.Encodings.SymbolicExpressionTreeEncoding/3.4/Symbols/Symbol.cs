#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("8ECC852D-3FE4-4AD8-9F70-582C7F874AE6")]
  [Item("Symbol", "Represents a symbol in a symbolic function tree.")]
  public abstract class Symbol : NamedItem, ISymbol {
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

    [Storable(DefaultValue = true)]
    private bool enabled;
    public virtual bool Enabled {
      get { return enabled; }
      set {
        if (value != enabled) {
          enabled = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    [Storable(DefaultValue = false)]
    private bool @fixed;
    public bool Fixed {
      get { return @fixed; }
      set {
        if (value != @fixed) {
          @fixed = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    public override bool CanChangeName {
      get { return !(this is IReadOnlySymbol); }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    public abstract int MinimumArity { get; }
    public abstract int MaximumArity { get; }
    #endregion

    [StorableConstructor]
    protected Symbol(StorableConstructorFlag _) : base(_) { }
    protected Symbol(Symbol original, Cloner cloner)
      : base(original, cloner) {
      initialFrequency = original.initialFrequency;
      enabled = original.enabled;
      @fixed = original.@fixed;
    }

    protected Symbol(string name, string description)
      : base(name, description) {
      initialFrequency = 1.0;
      enabled = true;
      @fixed = false;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (initialFrequency.IsAlmost(0.0) && !(this is GroupSymbol)) enabled = false;
      #endregion

    }

    public virtual ISymbolicExpressionTreeNode CreateTreeNode() {
      return new SymbolicExpressionTreeNode(this);
    }

    public virtual IEnumerable<ISymbol> Flatten() {
      yield return this;
    }

    public event EventHandler Changed;
    protected virtual void OnChanged(EventArgs e) {
      EventHandler handlers = Changed;
      if (handlers != null)
        handlers(this, e);
    }
  }
}
