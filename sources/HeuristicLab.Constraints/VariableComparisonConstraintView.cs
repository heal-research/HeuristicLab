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
using HeuristicLab.Data;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Visual representation of a <see cref="VariableComparisonConstraint"/>.
  /// </summary>
  public partial class VariableComparisonConstraintView : ViewBase {
    /// <summary>
    /// Gets or sets the VariableComparisonConstraint to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public VariableComparisonConstraint VariableComparisonConstraint {
      get { return (VariableComparisonConstraint)base.Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableComparisonConstraintView"/>.
    /// </summary>
    public VariableComparisonConstraintView() {
      InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableComparisonConstraintView"/> with the given
    /// <paramref name="variableComparisonConstraint"/> to display.
    /// </summary>
    /// <param name="variableComparisonConstraint">The constraint to represent visually.</param>
    public VariableComparisonConstraintView(VariableComparisonConstraint variableComparisonConstraint)
      : this() {
      VariableComparisonConstraint = variableComparisonConstraint;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="VariableComparisonConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      VariableComparisonConstraint.Changed -= new EventHandler(VariableComparisonConstraint_Changed);
      base.RemoveItemEvents();
    }

    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="VariableComparisonConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      VariableComparisonConstraint.Changed += new EventHandler(VariableComparisonConstraint_Changed);
    }

    void VariableComparisonConstraint_Changed(object sender, EventArgs e) {
      Refresh();
    }

    /// <summary>
    /// Updates all controls with the latest values.
    /// </summary> 
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (VariableComparisonConstraint == null) {
        leftVarNameStringDataView.Enabled = false;
        leftVarNameStringDataView.StringData = null;
        rightVarNameStringDataView.Enabled = false;
        rightVarNameStringDataView.StringData = null;
        comparerGroupBox.Enabled = false;
      } else {
        leftVarNameStringDataView.StringData = VariableComparisonConstraint.LeftVarName;
        leftVarNameStringDataView.Enabled = true;
        rightVarNameStringDataView.StringData = VariableComparisonConstraint.RightVarName;
        rightVarNameStringDataView.Enabled = true;
        comparerGroupBox.Enabled = true;
        switch (VariableComparisonConstraint.Comparer.Data) {
          case 0:
            lessRadioButton.Checked = true;
            break;
          case 1:
            lessOrEqualRadioButton.Checked = true;
            break;
          case 2:
            equalRadioButton.Checked = true;
            break;
          case 3:
            greaterOrEqualRadioButton.Checked = true;
            break;
          case 4:
            greaterRadioButton.Checked = true;
            break;
        }
      }
    }

    private void anyRadioButton_CheckedChanged(object sender, EventArgs e) {
      if (VariableComparisonConstraint != null) {
        if (((RadioButton)sender).Checked && ((RadioButton)sender).Name.StartsWith("lessOrEqual")) {
          VariableComparisonConstraint.Comparer.Data = 1;
        } else if (((RadioButton)sender).Checked && ((RadioButton)sender).Name.StartsWith("less")) {
          VariableComparisonConstraint.Comparer.Data = 0;
        } else if (((RadioButton)sender).Checked && ((RadioButton)sender).Name.StartsWith("greaterOrEqual")) {
          VariableComparisonConstraint.Comparer.Data = 3;
        } else if (((RadioButton)sender).Checked && ((RadioButton)sender).Name.StartsWith("greater")) {
          VariableComparisonConstraint.Comparer.Data = 4;
        } else if (((RadioButton)sender).Checked && ((RadioButton)sender).Name.StartsWith("equal")) {
          VariableComparisonConstraint.Comparer.Data = 2;
        } else if (((RadioButton)sender).Checked) {
          Auxiliary.ShowErrorMessageBox("Unknown radio button selected: " + ((RadioButton)sender).Name.ToString());
        }
      }
    }
  }
}
