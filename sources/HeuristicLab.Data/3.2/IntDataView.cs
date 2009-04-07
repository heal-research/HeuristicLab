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
  /// The visual representation of the class <see cref="IntData"/>, symbolizing an int value.
  /// </summary>
  public partial class IntDataView : ViewBase {
    /// <summary>
    /// Gets or set the int value to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="HeuristicLab.Core.ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public IntData IntData {
      get { return (IntData)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="IntDataView"/>.
    /// </summary>
    public IntDataView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of the class <see cref="IntDataView"/> with the 
    /// given <paramref name="intData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="intData"/> is not copied!</note>
    /// </summary>
    /// <param name="intData">The integer value to represent visually.</param>
    public IntDataView(IntData intData)
      : this() {
      IntData = intData;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="IntData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      IntData.Changed -= new EventHandler(IntData_Changed);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="IntData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      IntData.Changed += new EventHandler(IntData_Changed);
    }

    /// <summary>
    /// Updates the controls with the latest int value.
    /// </summary>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (IntData == null) {
        dataTextBox.Enabled = false;
      } else {
        dataTextBox.Enabled = true;
        dataTextBox.Text = IntData.ToString();
      }
    }

    private void dataTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      try {
        IntData.Data = int.Parse(dataTextBox.Text);
      }
      catch (Exception) {
        dataTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void IntData_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
