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
  /// A two-dimensional matrix consisting of boolean values.
  /// </summary>
  [EmptyStorableClass]
  public class BoolMatrixData : ArrayDataBase {
    /// <summary>
    /// Gets or sets the boolean elements of the matrix.
    /// </summary>
    /// <remarks>Uses the property <see cref="ArrayDataBase.Data"/> of base 
    /// class <see cref="ArrayDataBase"/>. No own data storage present.</remarks>
    public new bool[,] Data {
      get { return (bool[,])base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoolMatrixData"/> class.
    /// </summary>
    public BoolMatrixData() {
      Data = new bool[1, 1];
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="BoolMatrixData"/> class.
    /// <note type="caution"> No CopyConstructor! <paramref name="data"/> is not copied!</note>
    /// </summary>
    /// <param name="data">The boolean matrix the instance should represent.</param>
    public BoolMatrixData(bool[,] data) {
      Data = data;
    }

    /// <summary>
    /// The string representation of the matrix.
    /// </summary>
    /// <returns>The elements of the matrix as a string seperated by semicolons, line by line.</returns>
    public override string ToString() {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < Data.GetLength(0); i++) {
        for (int j = 0; j < Data.GetLength(1); j++) {
          builder.Append(";");
          builder.Append(Data[i, j].ToString());
        }
      }
      if (builder.Length > 0) builder.Remove(0, 1);
      return builder.ToString();
    }
  }
}
