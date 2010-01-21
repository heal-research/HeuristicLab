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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("ReferenceTypeData<T>", "A base class for representing reference types.")]
  public class ReferenceTypeData<T> : Item where T : class {
    [Storable]
    protected T value;
    public T Value {
      get { return value; }
      set {
        if (!value.Equals(this.value)) {
          this.value = value;
          OnValueChanged();
        }
      }
    }

    public ReferenceTypeData() {
      value = default(T);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ReferenceTypeData<T> clone = (ReferenceTypeData<T>)base.Clone(cloner);
      IDeepCloneable deepCloneable = Value as IDeepCloneable;
      if (deepCloneable != null) {
        clone.value = (T)deepCloneable.Clone(cloner);
        return clone;
      }
      ICloneable cloneable = Value as ICloneable;
      if (cloneable != null) {
        clone.value = (T)cloneable.Clone();
        return clone;
      }
      throw new InvalidOperationException("Contained object is not cloneable.");
    }

    public override string ToString() {
      return value == null ? "null" : value.ToString();
    }

    public event EventHandler ValueChanged;
    protected virtual void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
      OnChanged();
    }
  }
}
