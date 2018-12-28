#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Data {
  [Item("StringConvertibleArray", "Represents an array of string convertible values.")]
  [StorableType("68FCA40B-4ACF-4D7A-B4FA-67DBA481125E")]
  public abstract class StringConvertibleArray<T> : ValueTypeArray<T>, IStringConvertibleArray where T : struct {
    [StorableConstructor]
    protected StringConvertibleArray(StorableConstructorFlag _) : base(_) { }
    protected StringConvertibleArray(StringConvertibleArray<T> original, Cloner cloner)
      : base(original, cloner) { }

    protected StringConvertibleArray() : base() { }
    protected StringConvertibleArray(int length) : base(length) { }
    protected StringConvertibleArray(T[] elements) : base(elements) { }

    protected abstract bool Validate(string value, out string errorMessage);
    protected abstract string GetValue(int index);
    protected abstract bool SetValue(string value, int index);

    #region IStringConvertibleArray Members
    bool IStringConvertibleArray.Validate(string value, out string errorMessage) {
      return Validate(value, out errorMessage);
    }
    string IStringConvertibleArray.GetValue(int index) {
      return GetValue(index);
    }
    bool IStringConvertibleArray.SetValue(string value, int index) {
      return SetValue(value, index);
    }
    #endregion
  }
}
