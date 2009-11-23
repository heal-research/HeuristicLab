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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data {
  /// <summary>
  /// The visual representation of the class <see cref="ConstrainedIntDataView"/>, 
  /// symbolizing an int value with some constraints.
  /// </summary>
  [Content(typeof(ConstrainedIntData), true)]
  public partial class ConstrainedIntDataView : ViewBase {
    /// <summary>
    /// Gets or sets the int value to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="HeuristicLab.Core.ViewBase.Item"/> of base class <see cref="ViewBase"/>,
    /// but also own data storage present.</remarks>
    public ConstrainedIntData ConstrainedIntData {
      get { return (ConstrainedIntData)Item; }
      set {
        base.Item = value;
        constrainedDataBaseView.ConstrainedItem = value;
      }
    }

    /// <summary>
    /// Initializes a new instance of class <see cref="ConstrainedIntDataView"/>.
    /// </summary>
    public ConstrainedIntDataView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of class <see cref="ConstrainedIntDataView"/> with the given
    /// <paramref name="constraintIntData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="constraintIntData"/> is not copied!</note>
    /// </summary>
    /// <param name="constraintIntData">The int value to represent visually.</param>
    public ConstrainedIntDataView(ConstrainedIntData constraintIntData)
      : this() {
      ConstrainedIntData = constraintIntData;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="ConstrainedIntData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      ConstrainedIntData.Changed -= new EventHandler(ConstrainedIntData_Changed);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="ConstrainedIntData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ConstrainedIntData.Changed += new EventHandler(ConstrainedIntData_Changed);
    }

    /// <summary>
    /// Updates the controls with the latest int value.
    /// </summary>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (ConstrainedIntData == null) {
        dataTextBox.Enabled = false;
      } else {
        dataTextBox.Enabled = true;
        dataTextBox.Text = ConstrainedIntData.ToString();
      }
    }

    private void dataTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      int value;
      try {
        value = int.Parse(dataTextBox.Text);
        ICollection<IConstraint> violatedConstraints;
        if (!ConstrainedIntData.TrySetData(value, out violatedConstraints)) {
          if (HeuristicLab.Core.Views.Auxiliary.ShowIgnoreConstraintViolationMessageBox(violatedConstraints) == DialogResult.Yes)
            ConstrainedIntData.Data = value;
          else
            e.Cancel = true;
        }
      }
      catch (Exception) {
        dataTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void ConstrainedIntData_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
