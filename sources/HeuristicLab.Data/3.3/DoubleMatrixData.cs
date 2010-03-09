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
  [EmptyStorableClass]
  [Item("DoubleMatrixData", "Represents a matrix of double values.")]
  [Creatable("Test")]
  public sealed class DoubleMatrixData : ValueTypeMatrixData<double>, IStringConvertibleMatrixData {
    public DoubleMatrixData() : base() { }
    public DoubleMatrixData(int rows, int columns) : base(rows, columns) { }
    public DoubleMatrixData(double[,] elements) : base(elements) { }
    private DoubleMatrixData(DoubleMatrixData elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      DoubleMatrixData clone = new DoubleMatrixData(this);
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
    string IStringConvertibleMatrixData.GetValue(int rowIndex, int columIndex) {
      return this[rowIndex, columIndex].ToString();
    }
    bool IStringConvertibleMatrixData.SetValue(string value, int rowIndex, int columnIndex) {
      double val;
      if (double.TryParse(value, out val)) {
        this[rowIndex, columnIndex] = val;
        return true;
      } else {
        return false;
      }
    }
    #endregion
  }
}
