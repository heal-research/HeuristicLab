#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("ValueTypeValue<T>", "An abstract base class for representing values of value types.")]
  [StorableClass]
  public abstract class ValueTypeValue<T> : Item where T : struct {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.ValueType; }
    }

    [Storable]
    protected T value;
    public virtual T Value {
      get { return value; }
      set {
        if (!value.Equals(this.value)) {
          this.value = value;
          OnValueChanged();
        }
      }
    }

    protected ValueTypeValue() {
      this.value = default(T);
    }
    protected ValueTypeValue(T value) {
      this.value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ValueTypeValue<T> clone = (ValueTypeValue<T>)base.Clone(cloner);
      clone.value = value;
      return clone;
    }

    public override string ToString() {
      return value.ToString();
    }

    public event EventHandler ValueChanged;
    protected virtual void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
      OnToStringChanged();
    }
  }
}
