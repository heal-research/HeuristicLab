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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// The visual representation of a <see cref="DoubleBoundedConstraint"/>.
  /// </summary>
  [Content(typeof(DoubleBoundedConstraint), true)]
  public partial class DoubleBoundedConstraintView : ViewBase {
    /// <summary>
    /// Gets or sets the DoubleBoundedConstraint to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public DoubleBoundedConstraint DoubleBoundedConstraint {
      get { return (DoubleBoundedConstraint)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DoubleBoundedConstraintView"/>.
    /// </summary>
    public DoubleBoundedConstraintView() {
      InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DoubleBoundedConstraintView"/> with the given
    /// <paramref name="doubleBoundedConstraint"/> to display.
    /// </summary>
    /// <param name="doubleBoundedConstraint">The constraint to represent visually.</param>
    public DoubleBoundedConstraintView(DoubleBoundedConstraint doubleBoundedConstraint)
      : this() {
      DoubleBoundedConstraint = doubleBoundedConstraint;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="DoubleBoundedConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      DoubleBoundedConstraint.Changed -= new EventHandler(DoubleBoundedConstraint_Changed); 
      base.RemoveItemEvents();
    }

    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="DoubleBoundedConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      DoubleBoundedConstraint.Changed += new EventHandler(DoubleBoundedConstraint_Changed);
    }

    /// <summary>
    /// Updates all controls with the latest values.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (DoubleBoundedConstraint == null) {
        lbTextBox.Enabled = false;
        lbTextBox.Text = "";
        lbIncludedCheckBox.Enabled = false;
        lbEnabledCheckBox.Checked = false;

        ubTextBox.Enabled = false;
        ubTextBox.Text = "";
        ubIncludedCheckBox.Checked = false;
        ubEnabledCheckBox.Enabled = false;
      } else {
        lbTextBox.Text = DoubleBoundedConstraint.LowerBound.ToString("r");
        lbTextBox.Enabled = DoubleBoundedConstraint.LowerBoundEnabled;
        lbIncludedCheckBox.Checked = DoubleBoundedConstraint.LowerBoundIncluded;
        lbIncludedCheckBox.Enabled = true;
        lbEnabledCheckBox.Checked = DoubleBoundedConstraint.LowerBoundEnabled;
        lbEnabledCheckBox.Enabled = true;

        ubTextBox.Text = DoubleBoundedConstraint.UpperBound.ToString("r");
        ubTextBox.Enabled = DoubleBoundedConstraint.UpperBoundEnabled;
        ubIncludedCheckBox.Checked = DoubleBoundedConstraint.UpperBoundIncluded;
        ubIncludedCheckBox.Enabled = true;
        ubEnabledCheckBox.Checked = DoubleBoundedConstraint.UpperBoundEnabled;
        ubEnabledCheckBox.Enabled = true;
      }
    }

    void DoubleBoundedConstraint_Changed(object sender, EventArgs e) {
      Refresh();
    }

    private void lbEnabledCheckBox_CheckedChanged(object sender, EventArgs e) {
      DoubleBoundedConstraint.LowerBoundEnabled = lbEnabledCheckBox.Checked;
      lbTextBox.Enabled = lbEnabledCheckBox.Checked;
      lbIncludedCheckBox.Enabled = lbEnabledCheckBox.Checked;
    }

    private void ubEnabledCheckBox_CheckedChanged(object sender, EventArgs e) {
      DoubleBoundedConstraint.UpperBoundEnabled = ubEnabledCheckBox.Checked;
      ubTextBox.Enabled = ubEnabledCheckBox.Checked;
      ubIncludedCheckBox.Enabled = ubEnabledCheckBox.Checked;
    }

    private void lbIncludedCheckBox_CheckedChanged(object sender, EventArgs e) {
      DoubleBoundedConstraint.LowerBoundIncluded = lbIncludedCheckBox.Checked;
    }

    private void ubIncludedCheckBox_CheckedChanged(object sender, EventArgs e) {
      DoubleBoundedConstraint.UpperBoundIncluded = ubIncludedCheckBox.Checked;
    }

    private void lbTextBox_Validating(object sender, CancelEventArgs e) {
      double result;
      if (!double.TryParse(lbTextBox.Text, out result)) {
        e.Cancel = true;
      } 
    }

    private void ubTextBox_Validating(object sender, CancelEventArgs e) {
      double result;
      if (!double.TryParse(ubTextBox.Text, out result)) {
        e.Cancel = true;
      } 
    }

    private void lbTextBox_Validated(object sender, EventArgs e) {
      DoubleBoundedConstraint.LowerBound = double.Parse(lbTextBox.Text);
    }

    private void ubTextBox_Validated(object sender, EventArgs e) {
      DoubleBoundedConstraint.UpperBound = double.Parse(ubTextBox.Text);
    }
  }
}
