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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [EmptyStorableClass]
  [Item("StringData", "Represents a string.")]
  [Creatable("Test")]
  public sealed class StringData : Item, IComparable, IStringConvertibleData {
    [Storable]
    private string value;
    public string Value {
      get { return value; }
      set {
        if (value != this.value) {
          if ((value != null) || (this.value != string.Empty)) {
            this.value = value != null ? value : string.Empty;
            OnChanged();
          }
        }
      }
    }

    public StringData() {
      this.value = string.Empty;
    }
    public StringData(string value) {
      this.value = value != null ? value : string.Empty;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      StringData clone = new StringData(Value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public override string ToString() {
      return value;
    }

    public int CompareTo(object obj) {
      StringData other = obj as StringData;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }

    #region IStringConvertibleData Members
    bool IStringConvertibleData.Validate(string value, out string errorMessage) {
      if (value == null) {
        errorMessage = "Invalid Value (string must not be null)";
        return false;
      } else {
        errorMessage = string.Empty;
        return true;
      }
    }
    string IStringConvertibleData.GetValue() {
      return Value;
    }
    bool IStringConvertibleData.SetValue(string value) {
      if (value != null) {
        Value = value;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
