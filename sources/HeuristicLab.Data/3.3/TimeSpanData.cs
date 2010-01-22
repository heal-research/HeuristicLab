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
  [Item("TimeSpan", "Represents a duration of time.")]
  [Creatable("Test")]
  public sealed class TimeSpanData : ValueTypeData<TimeSpan>, IStringConvertibleData {
    public TimeSpanData() : base() { }
    public TimeSpanData(TimeSpan value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      TimeSpanData clone = new TimeSpanData(Value);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    string IStringConvertibleData.GetValue() {
      return Value.ToString();
    }
    bool IStringConvertibleData.SetValue(string value) {
      TimeSpan t;
      if (TimeSpan.TryParse(value, out t)) {
        Value = t;
        return true;
      } else {
        return false;
      }
    }
  }
}
