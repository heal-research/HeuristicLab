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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  [Item("IntData", "A data item which represents an integer value.")]
  [Creatable("Test")]
  public sealed class IntData : ItemBase {
    [Storable]
    private int value;
    public int Value {
      get { return value; }
      set {
        if (this.value != value) {
          this.value = value;
          OnValueChanged();
        }
      }
    }

    public IntData()
      : base() {
      value = 0;
    }
    public IntData(int value)
      : base() {
      this.value = value;
    }

    public override string ToString() {
      return value.ToString();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      IntData clone = new IntData(value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public event EventHandler ValueChanged;
    private void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, new EventArgs());
      OnChanged();
    }
  }
}
