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
  [Item("PercentMatrix", "Represents a matrix of double values in percent.")]
  [StorableClass]
  public class PercentMatrix : DoubleMatrix {
    [StorableConstructor]
    protected PercentMatrix(bool deserializing) : base(deserializing) { }
    protected PercentMatrix(PercentMatrix original, Cloner cloner)
      : base(original, cloner) {
    }
    public PercentMatrix() : base() { }
    public PercentMatrix(int rows, int columns) : base(rows, columns) { }
    public PercentMatrix(int rows, int columns, IEnumerable<string> columnNames) : base(rows, columns, columnNames) { }
    public PercentMatrix(int rows, int columns, IEnumerable<string> columnNames, IEnumerable<string> rowNames) : base(rows, columns, columnNames, rowNames) { }
    public PercentMatrix(double[,] elements) : base(elements) { }
    public PercentMatrix(double[,] elements, IEnumerable<string> columnNames) : base(elements, columnNames) { }
    public PercentMatrix(double[,] elements, IEnumerable<string> columnNames, IEnumerable<string> rowNames) : base(elements, columnNames, rowNames) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PercentMatrix(this, cloner);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      if (matrix.Length > 0) {
        for (int i = 0; i < Rows; i++) {
          sb.Append("[").Append(matrix[i, 0].ToString("#0.#################### %"));  // percent format
          for (int j = 1; j < Columns; j++)
            sb.Append(";").Append(matrix[i, j].ToString("#0.#################### %"));  // percent format
          sb.Append("]");
        }
      }
      sb.Append("]");
      return sb.ToString();
    }

    protected override bool Validate(string value, out string errorMessage) {
      value = value.Replace("%", " ");
      return base.Validate(value, out errorMessage);
    }
    protected override string GetValue(int rowIndex, int columIndex) {
      return this[rowIndex, columIndex].ToString("#0.#################### %");  // percent format
    }
    protected override bool SetValue(string value, int rowIndex, int columnIndex) {
      bool percent = value.Contains("%");
      value = value.Replace("%", " ");
      double val;
      if (double.TryParse(value, out val)) {
        if (percent) {
          if (!(val).IsAlmost(this[rowIndex, columnIndex] * 100.0))
            this[rowIndex, columnIndex] = val == 0 ? 0 : val / 100.0;
        } else {
          this[rowIndex, columnIndex] = val;
        }
        return true;
      } else {
        return false;
      }
    }
  }
}
