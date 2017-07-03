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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("EnumValue", "An abstract base class for representing values of enum types.")]
  [StorableClass]
  public sealed class EnumValue<T> : ValueTypeValue<T>, IComparable<EnumValue<T>> where T : struct, IComparable {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Enum; }
    }

    static EnumValue() {
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException("Generic type " + typeof(T).Name + " is not an enum.");
    }

    public EnumValue() {
      this.value = default(T);
      this.readOnly = false;
    }
    public EnumValue(T value) {
      this.value = value;
      this.readOnly = false;
    }

    [StorableConstructor]
    private EnumValue(bool deserializing) : base(deserializing) { }
    private EnumValue(EnumValue<T> original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EnumValue<T>(this, cloner);
    }

    public int CompareTo(EnumValue<T> other) {
      return Value.CompareTo(other.Value);
    }
  }
}
