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

namespace HeuristicLab.Data {
  /// <summary>
  /// The representation of an array of integer values.
  /// </summary>
  public class IntArrayData : ArrayDataBase {
    /// <summary>
    /// Gets or sets the int elements of the array.
    /// </summary>
    /// <remarks>Uses property <see cref="ArrayDataBase.Data"/> of base class <see cref="ArrayDataBase"/>. 
    /// No own data storage present.</remarks>
    public new int[] Data {
      get { return (int[])base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IntArrayData"/>.
    /// </summary>
    public IntArrayData() {
      Data = new int[0];
    }
    /// <summary>
    /// Initializes a new instance of <see cref="IntArrayData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="data"/> is not copied!</note>
    /// </summary>
    /// <param name="data">The array of integers to represent.</param>
    public IntArrayData(int[] data) {
      Data = data;
    }

    /// <summary>
    /// Creates a enw instance of <see cref="IntArrayDataView"/>.
    /// </summary>
    /// <returns>The created instance as <see cref="IntArrayDataView"/>.</returns>
    public override IView CreateView() {
      return new IntArrayDataView(this);
    }

    /// <summary>
    /// The string representation of the array, formatted according to the given <paramref name="format"/>.
    /// </summary>
    /// <param name="format">The <see cref="NumberFormatInfo"></see> the single int values 
    /// should be formatted accordingly.</param>
    /// <returns>The elements of the array as string, each element separated by a semicolon 
    /// and formatted according to the parameter <paramref name="format"/>.</returns>
    private string ToString(NumberFormatInfo format) {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < Data.Length; i++) {
        builder.Append(";");
        builder.Append(Data[i].ToString(format));
      }
      if (builder.Length > 0) builder.Remove(0, 1);
      return builder.ToString();
    }
  }
}
