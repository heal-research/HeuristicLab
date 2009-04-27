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
  /// The visual representation of the class <see cref="DoubleData"/>, symbolizing a double value.
  /// </summary>
  public partial class DoubleDataView : ViewBase {
    /// <summary>
    /// Gets or sets the double value to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="HeuristicLab.Core.ViewBase.Item"/> of base class 
    /// <see cref="ViewBase"/>. No own data storage present.</remarks>
    public DoubleData DoubleData {
      get { return (DoubleData)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="DoubleDataView"/>.
    /// </summary>
    public DoubleDataView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of the class <see cref="DoubleDataView"/> with the given 
    /// <paramref name="doubleData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="doubleData"/> is not copied!</note>
    /// </summary>
    /// <param name="doubleData">The double value to represent visually.</param>
    public DoubleDataView(DoubleData doubleData)
      : this() {
      DoubleData = doubleData;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="DoubleData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      DoubleData.Changed -= new EventHandler(DoubleData_Changed);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="DoubleData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      DoubleData.Changed += new EventHandler(DoubleData_Changed);
    }

    /// <summary>
    /// Updates the controls with the latest double value.
    /// </summary>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (DoubleData == null) {
        dataTextBox.Enabled = false;
      } else {
        dataTextBox.Enabled = true;
        dataTextBox.Text = DoubleData.Data.ToString("r");
      }
    }

    private void dataTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      try {
        DoubleData.Data = double.Parse(dataTextBox.Text);
      }
      catch (Exception) {
        dataTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void DoubleData_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
