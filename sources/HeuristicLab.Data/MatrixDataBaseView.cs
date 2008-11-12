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
  /// The basic visual representation of a two-dimensional matrix.
  /// </summary>
  public partial class MatrixDataBaseView : ViewBase {

    /// <summary>
    /// Gets or sets the matrix to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="HeuristicLab.Core.ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public ArrayDataBase ArrayDataBase {
      get { return (ArrayDataBase)Item; }
      protected set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="MatrixDataBaseView"/>.
    /// </summary>
    public MatrixDataBaseView() {
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
    /// <note type="caution"> Needs to be overriden in each inherited class!</note>
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when method is not 
    /// overridden in inherited class.</exception>
    /// <param name="element">The data to validate.</param>
    /// <returns><c>true</c> if the data is valid, <c>false</c> otherwise.</returns>
    protected virtual bool ValidateData(string element) {
      throw new InvalidOperationException("ValidateData has to be overridden in each inherited class");
    }
    /// <summary>
    /// Sets an element of the current instance at the given <paramref name="index"/> 
    /// to the given <paramref name="element"/>.
    /// <note type="caution"> Needs to be overridden in each inherited class!</note>
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when method is not 
    /// overridden in inherited class.</exception>
    /// <param name="row">The row where to substitute the element.</param>
    /// <param name="column">The column where to substitute the element.</param>
    /// <param name="element">The element to insert.</param>        
    protected virtual void SetArrayElement(int row, int column, string element) {
      throw new InvalidOperationException("SetArrayElement has to be overridden in each inherited class");
    }

    /// <summary>
    /// Update all controls with the latest element of the matrix.
    /// </summary>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (ArrayDataBase != null) {
        int rows = ArrayDataBase.Data.GetLength(0);
        int columns = ArrayDataBase.Data.GetLength(1);

        rowsTextBox.Text = rows + "";
        columnsTextBox.Text = columns + "";
        dataGridView.ColumnCount = columns;
        dataGridView.RowCount = rows;
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < columns; j++) {
            dataGridView.Rows[i].Cells[j].Value = ArrayDataBase.Data.GetValue(i, j);
          }
        }
      } else {
        rowsTextBox.Text = "1";
        columnsTextBox.Text = "1";
        dataGridView.ColumnCount = 1;
        dataGridView.RowCount = 1;
      }
    }

    private void textBox_Validating(object sender, CancelEventArgs e) {
      int newValue;
      TextBox source = (TextBox)sender;
      if (int.TryParse(source.Text, out newValue)) {
        if (newValue > 0) {
          e.Cancel = false;
        } else {
          e.Cancel = true;
        }
      } else {
        e.Cancel = true;
      }
    }

    /// <summary>
    /// Creates a new matrix having the specified number (<paramref name="newRows"/>) 
    /// of rows and the specified number (<paramref name="newColumns"/>) of columns of the 
    /// current instance.
    /// </summary>
    /// <param name="newRows">The number of rows of the new matrix.</param>
    /// <param name="newColumns">The number of columns of the new matrix</param>
    private void CreateAndCopyArray(int newRows, int newColumns) {
      Array newArray = Array.CreateInstance(ArrayDataBase.Data.GetType().GetElementType(), newRows, newColumns);
      Array.Copy(ArrayDataBase.Data, newArray, Math.Min(newArray.Length, ArrayDataBase.Data.Length));
      ArrayDataBase.Data = newArray;
    }

    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (ValidateData((string)e.FormattedValue)) {
        SetArrayElement(e.RowIndex, e.ColumnIndex, (string)e.FormattedValue);
        e.Cancel = false;
        Refresh();
      } else {
        e.Cancel = true;
      }
    }

    private void textBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
        e.SuppressKeyPress = true;
        dataGridView.Focus();
      }
    }

    private void textBox_Validated(object sender, EventArgs e) {
      int newRows;
      int newColumns;
      if (int.TryParse(columnsTextBox.Text, out newColumns) && int.TryParse(rowsTextBox.Text, out newRows)) {
        CreateAndCopyArray(newRows, newColumns);
      } else {
        throw new FormatException();
      }
    }

    #region ArrayDataBase Events
    private void ArrayDataBase_Changed(object sender, EventArgs e) {
      Refresh();
    }
    #endregion
  }
}
