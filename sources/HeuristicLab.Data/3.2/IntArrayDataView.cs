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
  /// The visual representation of the class <see cref="IntArrayData"/>, 
  /// symbolizing an array of int values.
  /// </summary>
  public partial class IntArrayDataView : ArrayDataBaseView {
    /// <summary>
    /// Gets or set the int array to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ArrayDataBase"/> of base class
    /// <see cref="ArrayDataBaseView"/>. No own data storage present.</remarks>
    public IntArrayData IntArrayData {
      get { return (IntArrayData)base.ArrayDataBase; }
      set { base.ArrayDataBase = value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="IntArrayDataView"/>.
    /// </summary>
    public IntArrayDataView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of the class <see cref="IntArrayDataView"/> with the given
    /// <paramref name="intArrayData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="intArrayData"/> is not copied!</note>
    /// </summary>
    /// <param name="intArrayData">The int array to represent visually.</param>
    public IntArrayDataView(IntArrayData intArrayData)
      : this() {
      IntArrayData = intArrayData;
    }

    /// <summary>
    /// Subsitutes an element at the given 
    /// <paramref name="index"/> with the given <paramref name="element"/>.
    /// </summary>
    /// <param name="index">The position of the element to substitute.</param>
    /// <param name="element">The element to insert.</param>
    protected override void SetArrayElement(int index, string element) {
      int result;
      int.TryParse(element, out result);

      IntArrayData.Data[index] = result;
    }

    /// <summary>
    /// Checks whether the given <paramref name="element"/> can be converted into a bool value.
    /// </summary>
    /// <param name="element">The element to check.</param>
    /// <returns><c>true</c> if the <paramref name="element"/> could be converted,
    /// <c>false</c> otherwise.</returns>
    protected override bool ValidateData(string element) {
      int result;
      return element != null && int.TryParse(element, out result);
    }
  }
}
