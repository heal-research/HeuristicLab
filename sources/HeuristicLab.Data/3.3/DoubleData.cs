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
  [EmptyStorableClass]
  [Item("Double", "Represents a double value.")]
  [Creatable("Test")]
  public sealed class DoubleData : ValueTypeData<double>, IStringConvertibleData {
    public DoubleData() : base() { }
    public DoubleData(double value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      DoubleData clone = new DoubleData(Value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    public override string ToString() {
      return Value.ToString("r");  // round-trip format
    }

    #region IStringConvertibleData Members
    bool IStringConvertibleData.Validate(string value) {
      double d;
      return double.TryParse(value, out d);
    }
    string IStringConvertibleData.GetValue() {
      return Value.ToString("r");  // round-trip format
    }
    bool IStringConvertibleData.SetValue(string value) {
      double d;
      if (double.TryParse(value, out d)) {
        Value = d;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
