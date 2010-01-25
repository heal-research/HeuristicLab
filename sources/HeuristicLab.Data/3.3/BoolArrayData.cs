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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [EmptyStorableClass]
  [Item("BoolArrayData", "Represents an array of boolean values.")]
  [Creatable("Test")]
  public sealed class BoolArrayData : ValueTypeArrayData<bool>, IStringConvertibleArrayData {
    public BoolArrayData() : base() { }
    public BoolArrayData(int length) : base(length) { }
    public BoolArrayData(bool[] elements) : base(elements) { }
    protected BoolArrayData(BoolArrayData elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      BoolArrayData clone = new BoolArrayData(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    #region IStringConvertibleArrayData Members
    int IStringConvertibleArrayData.Length {
      get { return Length; }
      set { Length = value; }
    }
    bool IStringConvertibleArrayData.Validate(string value) {
      bool b;
      return bool.TryParse(value, out b);
    }
    string IStringConvertibleArrayData.GetValue(int index) {
      return this[index].ToString();
    }
    bool IStringConvertibleArrayData.SetValue(string value, int index) {
      bool b;
      if (bool.TryParse(value, out b)) {
        this[index] = b;
        return true;
      } else {
        return false;
      }
    }
    event EventHandler<EventArgs<int>> IStringConvertibleArrayData.ItemChanged {
      add { base.ItemChanged += value; }
      remove { base.ItemChanged -= value; }
    }
    event EventHandler IStringConvertibleArrayData.Reset {
      add { base.Reset += value; }
      remove { base.Reset -= value; }
    }
    #endregion
  }
}
