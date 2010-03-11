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

using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("IntMatrixData", "Represents a matrix of integer values.")]
  [Creatable("Test")]
  [StorableClass(StorableClassType.Empty)]
  public sealed class IntMatrixData : ValueTypeMatrixData<int>, IStringConvertibleMatrixData {
    public IntMatrixData() : base() { }
    public IntMatrixData(int rows, int columns) : base(rows, columns) { }
    public IntMatrixData(int[,] elements) : base(elements) { }
    private IntMatrixData(IntMatrixData elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      IntMatrixData clone = new IntMatrixData(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    #region IStringConvertibleMatrixData Members
    int IStringConvertibleMatrixData.Rows {
      get { return Rows; }
      set { Rows = value; }
    }
    int IStringConvertibleMatrixData.Columns {
      get { return Columns; }
      set { Columns = value; }
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
      return this[rowIndex, columIndex].ToString();
    }
    bool IStringConvertibleMatrixData.SetValue(string value, int rowIndex, int columnIndex) {
      int val;
      if (int.TryParse(value, out val)) {
        this[rowIndex, columnIndex] = val;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
