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
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  /// <summary>
  /// The visual representation of the class <see cref="ArrayDataBase"/>.
  /// </summary>
  public partial class ArrayDataBaseView : ViewBase {
    /// <summary>
    /// Gets or sets the instance of the array to represent.
    /// </summary>
    /// <remarks>Uses property <see cref="HeuristicLab.Core.ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public ArrayDataBase ArrayDataBase {
      get { return (ArrayDataBase)Item; }
      protected set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ArrayDataBaseView"/>.
    /// </summary>
    public ArrayDataBaseView() {
      InitializeComponent();
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="ArrayDataBase"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      ArrayDataBase.Changed -= new EventHandler(ArrayDataBase_Changed);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="ArrayDataBase"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ArrayDataBase.Changed += new EventHandler(ArrayDataBase_Changed);
    }

    /// <summary>
    /// Validates the given data.
    /// <note type="caution"> Needs to be overridden in each inherited class!</note>
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when method is not 
    /// overridden in inherited class.</exception>
    /// <param name="element">The data to validate.</param>
    /// <returns><c>true</c> if the data is valid, <c>false</c> otherwise.</returns>
    protected virtual bool ValidateData(string element) {
      throw new InvalidOperationException("ValidateData has to be overridden in each inherited class");
    }
    /// <summary>
    /// Replaces an element at the given <paramref name="index"/> 
    /// with the given <paramref name="element"/>.
    /// <note type="caution"> Needs to be overridden in each inherited class!</note>
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when method is not 
    /// overridden in inherited class.</exception>
    /// <param name="index">The position where to substitute the element.</param>
    /// <param name="element">The element to insert.</param>
    protected virtual void SetArrayElement(int index, string element) {
      throw new InvalidOperationException("SetArrayElement has to be overridden in each inherited class");
    }

    /// <summary>
    /// Updates all controls and the elements of the table with the latest values.
    /// </summary>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (ArrayDataBase != null) {
        int length = ArrayDataBase.Data.Length;
        lengthTextBox.Text = length + "";
        dataGridView.ColumnCount = 1;
        dataGridView.RowCount = length;
        for (int i = 0; i < length; i++) {
          dataGridView.Rows[i].Cells[0].Value = ArrayDataBase.Data.GetValue(i);
        }
      } else {
        lengthTextBox.Text = "0";
        dataGridView.ColumnCount = 1;
        dataGridView.RowCount = 0;
      }
    }

    private void lengthTextBox_Validating(object sender, CancelEventArgs e) {
      int newLength;
      if (int.TryParse(lengthTextBox.Text, out newLength)) {
        if (newLength > 0) {
          e.Cancel = false;
          if (newLength != ArrayDataBase.Data.Length) {
            CreateAndCopyArray(newLength);
          }
        } else {
          // only allow values greater than 0
          e.Cancel = true;
        }
      } else {
        e.Cancel = true;
      }
    }

    /// <summary>
    /// Creates a new array having the specified number (<paramref name="newLength"/>) of elements of the 
    /// current instance (starting from the beginning).
    /// </summary>
    /// <param name="newLength">The size/number of elements of the new array.</param>
    private void CreateAndCopyArray(int newLength) {
      Array newArray = Array.CreateInstance(ArrayDataBase.Data.GetType().GetElementType(), newLength);
      Array.Copy(ArrayDataBase.Data, newArray, Math.Min(newLength, ArrayDataBase.Data.Length));
      ArrayDataBase.Data = newArray;
    }

    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (ValidateData((string)e.FormattedValue)) {
        SetArrayElement(e.RowIndex, (string)e.FormattedValue);
        e.Cancel = false;
      } else {
        e.Cancel = true;
      }
    }

    private void lengthTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
        e.SuppressKeyPress = true;
        dataGridView.Focus();
      }
    }

    #region ArrayDataBase Events
    private void ArrayDataBase_Changed(object sender, EventArgs e) {
      Refresh();
    }
    #endregion
  }
}
