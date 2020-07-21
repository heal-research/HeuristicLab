#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  [Item("DoubleMatrix", "Represents a matrix of double values.")]
  [StorableType("EFF3CA0C-100C-4DD4-9EFF-2EF3CB0DE793")]
  public class DoubleMatrix : ValueTypeMatrix<double>, IStringConvertibleMatrix {
    [StorableConstructor]
    protected DoubleMatrix(StorableConstructorFlag _) : base(_) { }
    protected DoubleMatrix(DoubleMatrix original, Cloner cloner)
      : base(original, cloner) {
    }
    public DoubleMatrix() : base() { }
    public DoubleMatrix(int rows, int columns) : base(rows, columns) { }
    public DoubleMatrix(int rows, int columns, IEnumerable<string> columnNames) : base(rows, columns, columnNames) { }
    public DoubleMatrix(int rows, int columns, IEnumerable<string> columnNames, IEnumerable<string> rowNames) : base(rows, columns, columnNames, rowNames) { }
    public DoubleMatrix(double[,] elements, bool @readonly = false) : base(elements, @readonly) { }
    public DoubleMatrix(double[,] elements, IEnumerable<string> columnNames, bool @readonly = false) : base(elements, columnNames, @readonly) { }
    public DoubleMatrix(double[,] elements, IEnumerable<string> columnNames, IEnumerable<string> rowNames, bool @readonly = false) : base(elements, columnNames, rowNames, @readonly) { }
    
    public static DoubleMatrix FromRows(IList<double[]> elements, bool @readonly = false, IEnumerable<string> columnNames = null, IEnumerable<string> rowNames = null) {
      if (elements.Count == 0) return new DoubleMatrix(0, 0);
      var mat = new double[elements.Count, elements[0].Length];
      for (var r = 0; r < mat.GetLength(0); r++)
        for (var c = 0; c < mat.GetLength(1); c++)
          mat[r, c] = elements[r][c];
      // TODO: We should avoid the memory copy in this case
      return new DoubleMatrix(mat, columnNames, rowNames, @readonly);
    }
    public static DoubleMatrix FromColumns(IList<double[]> elements, bool @readonly = false, IEnumerable<string> columnNames = null, IEnumerable<string> rowNames = null) {
      if (elements.Count == 0) return new DoubleMatrix(0, 0);
      var mat = new double[elements[0].Length, elements.Count];
      for (var c = 0; c < mat.GetLength(1); c++)
        for (var r = 0; r < mat.GetLength(0); r++)
          mat[r, c] = elements[c][r];
      // TODO: We should avoid the memory copy in this case
      return new DoubleMatrix(mat, columnNames, rowNames, @readonly);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DoubleMatrix(this, cloner);
    }

    protected virtual bool Validate(string value, out string errorMessage) {
      double val;
      bool valid = double.TryParse(value, out val);
      errorMessage = string.Empty;
      if (!valid) {
        StringBuilder sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetDoubleFormatPattern());
        sb.Append("\")");
        errorMessage = sb.ToString();
      }
      return valid;
    }
    protected virtual string GetValue(int rowIndex, int columIndex) {
      return this[rowIndex, columIndex].ToString("r");
    }
    protected virtual bool SetValue(string value, int rowIndex, int columnIndex) {
      double val;
      if (double.TryParse(value, out val)) {
        this[rowIndex, columnIndex] = val;
        return true;
      } else {
        return false;
      }
    }

    public new DoubleMatrix AsReadOnly() {
      return (DoubleMatrix)base.AsReadOnly();
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
