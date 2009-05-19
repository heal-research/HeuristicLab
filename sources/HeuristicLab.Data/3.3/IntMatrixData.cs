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
using System.Globalization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  /// <summary>
  /// A two-dimensional matrix of integer values.
  /// </summary>
  [EmptyStorableClass]
  public class IntMatrixData : ArrayDataBase {
    /// <summary>
    /// Gets or sets the int values of the matrix.
    /// </summary>
    /// <remarks>Uses property <see cref="ArrayDataBase.Data"/> of base class <see cref="ArrayDataBase"/>. 
    /// No own data storage present.</remarks>
    public new int[,] Data {
      get { return (int[,])base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IntMatrixData"/>.
    /// </summary>
    public IntMatrixData() {
      Data = new int[1, 1];
    }
    /// <summary>
    /// Initializes a new instance of <see cref="IntMatrixData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="data"/> is not copied!</note>
    /// </summary>
    /// <param name="data">The two-dimensional int matrix the instance should represent.</param>
    public IntMatrixData(int[,] data) {
      Data = data;
    }

    /// <summary>
    /// Creates a new instance of <see cref="IntMatrixDataView"/>.
    /// </summary>
    /// <returns>The created instance as <see cref="IntMatrixDataView"/>.</returns>
    public override IView CreateView() {
      return new IntMatrixDataView(this);
    }

    /// <summary>
    /// The string representation of the matrix.
    /// </summary> 
    /// <returns>The elements of the matrix as a string, line by line, each element separated by a 
    /// semicolon and formatted according to the local number format.</returns>
    public override string ToString() {
      return ToString(CultureInfo.CurrentCulture.NumberFormat);
    }

    /// <summary>
    /// The string representation of the matrix, considering a specified <paramref name="format"/>.
    /// </summary>
    /// <param name="format">The <see cref="NumberFormatInfo"/> the int values are formatted accordingly.</param>
    /// <returns>The elements of the matrix as a string, line by line, each element separated by a 
    /// semicolon and formatted according to the specified <paramref name="format"/>.</returns>
    private string ToString(NumberFormatInfo format) {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < Data.GetLength(0); i++) {
        for (int j = 0; j < Data.GetLength(1); j++) {
          builder.Append(";");
          builder.Append(Data[i, j].ToString(format));
        }
      }
      if (builder.Length > 0) builder.Remove(0, 1);
      return builder.ToString();
    }
  }
}
