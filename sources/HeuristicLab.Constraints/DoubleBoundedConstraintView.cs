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
  public partial class DoubleBoundedConstraintView : ViewBase {
    public DoubleBoundedConstraint DoubleBoundedConstraint {
      get { return (DoubleBoundedConstraint)Item; }
      set { base.Item = value; }
    }

    public DoubleBoundedConstraintView() {
      InitializeComponent();
    }

    public DoubleBoundedConstraintView(DoubleBoundedConstraint doubleBoundedConstraint)
      : this() {
      DoubleBoundedConstraint = doubleBoundedConstraint;
    }

    protected override void RemoveItemEvents() {
      DoubleBoundedConstraint.Changed -= new EventHandler(DoubleBoundedConstraint_Changed); 
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      DoubleBoundedConstraint.Changed += new EventHandler(DoubleBoundedConstraint_Changed);
    }

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
