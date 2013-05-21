#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("IntMatrix", "Represents a matrix of integer values.")]
  [StorableClass]
  public class IntMatrix : ValueTypeMatrix<int>, IStringConvertibleMatrix {
    [StorableConstructor]
    protected IntMatrix(bool deserializing) : base(deserializing) { }
    protected IntMatrix(IntMatrix original, Cloner cloner)
      : base(original, cloner) {
    }
    public IntMatrix() : base() { }
    public IntMatrix(int rows, int columns) : base(rows, columns) { }
    public IntMatrix(int rows, int columns, IEnumerable<string> columnNames) : base(rows, columns, columnNames) { }
    public IntMatrix(int rows, int columns, IEnumerable<string> columnNames,IEnumerable<string> rowNames) : base(rows, columns, columnNames,rowNames) { }
    public IntMatrix(int[,] elements) : base(elements) { }
    public IntMatrix(int[,] elements, IEnumerable<string> columnNames) : base(elements,columnNames) { }
    public IntMatrix(int[,] elements, IEnumerable<string> columnNames, IEnumerable<string> rowNames) : base(elements,columnNames,rowNames) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntMatrix(this, cloner);
    }

    protected virtual bool Validate(string value, out string errorMessage) {
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
    protected virtual string GetValue(int rowIndex, int columIndex) {
      return this[rowIndex, columIndex].ToString();
    }
    protected virtual bool SetValue(string value, int rowIndex, int columnIndex) {
      int val;
      if (int.TryParse(value, out val)) {
        this[rowIndex, columnIndex] = val;
        return true;
      } else {
        return false;
      }
    }

    #region IStringConvertibleMatrix Members
    int IStringConvertibleMatrix.Rows {
      get { return Rows; }
      set { Rows = value; }
    }
    int IStringConvertibleMatrix.Columns {
      get { return Columns; }
      set { Columns = value; }
    }
    bool IStringConvertibleMatrix.Validate(string value, out string errorMessage) {
      return Validate(value, out errorMessage);
    }
    string IStringConvertibleMatrix.GetValue(int rowIndex, int columIndex) {
      return GetValue(rowIndex, columIndex);
    }
    bool IStringConvertibleMatrix.SetValue(string value, int rowIndex, int columnIndex) {
      return SetValue(value, rowIndex, columnIndex);
    }
    #endregion
  }
}
