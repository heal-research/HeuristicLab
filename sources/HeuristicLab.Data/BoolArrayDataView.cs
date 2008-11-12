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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Data {
  /// <summary>
  /// The visual representation of the class <see cref="BoolArrayData"/>, 
  /// symbolizing an array of boolean values.
  /// </summary>
  public partial class BoolArrayDataView : ArrayDataBaseView {
    /// <summary>
    /// Gets or sets the instance of the boolean array to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ArrayDataBase"/> of 
    /// base class <see cref="ArrayDataBaseView"/>. No own data storage present.</remarks>
    public BoolArrayData BoolArrayData {
      get { return (BoolArrayData)base.ArrayDataBase; }
      set { base.ArrayDataBase = value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="BoolArrayDataView"/>.
    /// </summary>
    public BoolArrayDataView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of the class <see cref="BoolArrayDataView"/> 
    /// with the given <paramref name="boolArrayData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="boolArrayData"/> is not copied!</note>
    /// </summary>
    /// <param name="boolArrayData">The boolean array to represent visually.</param>
    public BoolArrayDataView(BoolArrayData boolArrayData)
      : this() {
      BoolArrayData = boolArrayData;
    }

    /// <summary>
    /// Sets the element on position <paramref name="index"/> to the
    /// given <paramref name="element"/> as boolean value.
    /// </summary>
    /// <param name="index">The position where to substitute the element.</param>
    /// <param name="element">The element to insert.</param>
    protected override void SetArrayElement(int index, string element) {
      bool result;
      bool.TryParse(element, out result);

      BoolArrayData.Data[index] = result;
    }

    /// <summary>
    /// Checks whether the given <paramref name="element"/> can be converted to a boolean value.
    /// </summary>
    /// <param name="element">The data to validate.</param>
    /// <returns><c>true</c> if the <paramref name="element"/> could be converted,
    /// <c>false</c> otherwise.</returns>
    protected override bool ValidateData(string element) {
      bool result;
      return element != null && bool.TryParse(element, out result);
    }
  }
}
