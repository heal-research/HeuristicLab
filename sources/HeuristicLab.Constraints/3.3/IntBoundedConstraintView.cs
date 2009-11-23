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
  /// The visual representation of an <see cref="IntBoundedConstraint"/>.
  /// </summary>
  [Content(typeof(IntBoundedConstraint), true)]
  public partial class IntBoundedConstraintView : ViewBase {
    /// <summary>
    /// Gets or sets the the IntBoundedConstraint to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public IntBoundedConstraint IntBoundedConstraint {
      get { return (IntBoundedConstraint)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IntBoundedConstraintView"/>.
    /// </summary>
    public IntBoundedConstraintView() {
      InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IntBoundedConstraintView"/> with the given
    /// <paramref name="intBoundedConstraint"/> to display.
    /// </summary>
    /// <param name="intBoundedConstraint">The constraint to represent visually.</param>
    public IntBoundedConstraintView(IntBoundedConstraint intBoundedConstraint)
      : this() {
      IntBoundedConstraint = intBoundedConstraint;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="IntBoundedConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      IntBoundedConstraint.Changed -= new EventHandler(IntBoundedConstraint_Changed);
      base.RemoveItemEvents();
    }

    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="IntBoundedConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      IntBoundedConstraint.Changed += new EventHandler(IntBoundedConstraint_Changed);
    }

    /// <summary>
    /// Updates all controls with the latest values.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (IntBoundedConstraint == null) {
        lbTextBox.Enabled = false;
        lbTextBox.Text = "";
        lbIncludedCheckBox.Enabled = false;
        lbEnabledCheckBox.Checked = false;

        ubTextBox.Enabled = false;
        ubTextBox.Text = "";
        ubIncludedCheckBox.Enabled = false;
        ubEnabledCheckBox.Checked = false;
      } else {
        lbTextBox.Text = IntBoundedConstraint.LowerBound + "";
        lbTextBox.Enabled = IntBoundedConstraint.LowerBoundEnabled;
        lbIncludedCheckBox.Checked = IntBoundedConstraint.LowerBoundIncluded;
        lbIncludedCheckBox.Enabled = true;
        lbEnabledCheckBox.Checked = IntBoundedConstraint.LowerBoundEnabled;
        lbEnabledCheckBox.Enabled = true;

        ubTextBox.Text = IntBoundedConstraint.UpperBound + "";
        ubTextBox.Enabled = IntBoundedConstraint.UpperBoundEnabled;
        ubIncludedCheckBox.Checked = IntBoundedConstraint.UpperBoundIncluded;
        ubIncludedCheckBox.Enabled = true;
        ubEnabledCheckBox.Checked = IntBoundedConstraint.UpperBoundEnabled;
        ubEnabledCheckBox.Enabled = true;
      }
    }

    void IntBoundedConstraint_Changed(object sender, EventArgs e) {
      Refresh();
    }

    private void lbEnabledCheckBox_CheckedChanged(object sender, EventArgs e) {
      IntBoundedConstraint.LowerBoundEnabled = lbEnabledCheckBox.Checked;
      lbTextBox.Enabled = lbEnabledCheckBox.Checked;
      lbIncludedCheckBox.Enabled = lbEnabledCheckBox.Checked;
    }

    private void ubEnabledCheckBox_CheckedChanged(object sender, EventArgs e) {
      IntBoundedConstraint.UpperBoundEnabled = ubEnabledCheckBox.Checked;
      ubTextBox.Enabled = ubEnabledCheckBox.Checked;
      ubIncludedCheckBox.Enabled = ubEnabledCheckBox.Checked;
    }

    private void lbIncludedCheckBox_CheckedChanged(object sender, EventArgs e) {
      IntBoundedConstraint.LowerBoundIncluded = lbIncludedCheckBox.Checked;
    }

    private void ubIncludedCheckBox_CheckedChanged(object sender, EventArgs e) {
      IntBoundedConstraint.UpperBoundIncluded = ubIncludedCheckBox.Checked;
    }

    private void lbTextBox_Validating(object sender, CancelEventArgs e) {
      int result;
      if (!int.TryParse(lbTextBox.Text, out result)) {
        e.Cancel = true;
      }
    }

    private void lbTextBox_Validated(object sender, EventArgs e) {
      IntBoundedConstraint.LowerBound = int.Parse(lbTextBox.Text);
    }

    private void ubTextBox_Validating(object sender, CancelEventArgs e) {
      int result;
      if (!int.TryParse(ubTextBox.Text, out result)) {
        e.Cancel = true;
      }
    }

    private void ubTextBox_Validated(object sender, EventArgs e) {
      IntBoundedConstraint.UpperBound = int.Parse(ubTextBox.Text);
    }
  }
}
