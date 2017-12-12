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

using System;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("Arc", "A graph arc connecting two graph vertices, that can have a weight, label, and data object for holding additional info")]
  [StorableClass]
  public class Arc : Item, IArc {
    [Storable]
    public IVertex Source { get; private set; }

    [Storable]
    public IVertex Target { get; private set; }

    [Storable]
    protected string label;
    public string Label {
      get { return label; }
      set {
        if (label == value) return;
        label = value;
        OnChanged(this, EventArgs.Empty);
      }
    }

    [Storable]
    protected double weight;
    public double Weight {
      get { return weight; }
      set {
        if (weight.Equals(value)) return;
        weight = value;
        OnChanged(this, EventArgs.Empty);
      }
    }


    [StorableConstructor]
    protected Arc(bool deserializing) : base(deserializing) { }

    public Arc(IVertex source, IVertex target) {
      Source = source;
      Target = target;
    }

    protected Arc(Arc original, Cloner cloner)
      : base(original, cloner) {
      Source = cloner.Clone(original.Source);
      Target = cloner.Clone(original.Target);
      label = original.Label;
      weight = original.Weight;
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new Arc(this, cloner); }

    public event EventHandler Changed;
    protected virtual void OnChanged(object sender, EventArgs args) {
      var changed = Changed;
      if (changed != null)
        changed(sender, args);
    }
  }

  [StorableClass]
  public class Arc<T> : Arc, IArc<T> where T : class, IDeepCloneable {
    [Storable]
    protected T data;
    public T Data {
      get { return data; }
      set {
        if (data == value) return;
        data = value;
        OnChanged(this, EventArgs.Empty);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Arc<T>(this, cloner);
    }

    protected Arc(Arc<T> original, Cloner cloner)
      : base(original, cloner) {
      if (original.Data != null)
        Data = cloner.Clone(original.Data);
    }

    public Arc(bool deserializing)
      : base(deserializing) {
    }

    public Arc(IVertex source, IVertex target)
      : base(source, target) {
    }

    protected Arc(Arc original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}
