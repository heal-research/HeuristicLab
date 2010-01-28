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
  [Item("IntArrayData", "Represents an array of integer values.")]
  [Creatable("Test")]
  public sealed class IntArrayData : ValueTypeArrayData<int>, IStringConvertibleMatrixData {
    public IntArrayData() : base() { }
    public IntArrayData(int length) : base(length) { }
    public IntArrayData(int[] elements) : base(elements) { }
    private IntArrayData(IntArrayData elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      IntArrayData clone = new IntArrayData(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    #region IStringConvertibleMatrixData Members
    StringConvertibleArrayDataDimensions IStringConvertibleMatrixData.Dimensions {
      get { return StringConvertibleArrayDataDimensions.Rows; }
    }
    int IStringConvertibleMatrixData.Rows {
      get { return Length; }
      set { Length = value; }
    }
    int IStringConvertibleMatrixData.Columns {
      get { return 1; }
      set { throw new NotSupportedException("Columns cannot be changed."); }
    }

    bool IStringConvertibleMatrixData.Validate(string value, out string errorMessage) {
      int val;
      bool valid = int.TryParse(value, out val);
      errorMessage = string.Empty;
      if (!valid) {
        StringBuilder sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetIntFormatPattern());
        sb.Append("\")");
        errorMessage = sb.ToString();
      }
      return valid;
    }
    string IStringConvertibleMatrixData.GetValue(int rowIndex, int columIndex) {
      return this[rowIndex].ToString();
    }
    bool IStringConvertibleMatrixData.SetValue(string value, int rowIndex, int columnIndex) {
      int val;
      if (int.TryParse(value, out val)) {
        this[rowIndex] = val;
        return true;
      } else {
        return false;
      }
    }
    event EventHandler<EventArgs<int, int>> IStringConvertibleMatrixData.ItemChanged {
      add { base.ItemChanged += value; }
      remove { base.ItemChanged -= value; }
    }
    event EventHandler IStringConvertibleMatrixData.Reset {
      add { base.Reset += value; }
      remove { base.Reset -= value; }
    }
    #endregion
  }
}
