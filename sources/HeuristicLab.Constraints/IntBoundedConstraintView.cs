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
  public partial class IntBoundedConstraintView : ViewBase {
    public IntBoundedConstraint IntBoundedConstraint {
      get { return (IntBoundedConstraint)Item; }
      set { base.Item = value; }
    }

    public IntBoundedConstraintView() {
      InitializeComponent();
    }

    public IntBoundedConstraintView(IntBoundedConstraint intBoundedConstraint)
      : this() {
      IntBoundedConstraint = intBoundedConstraint;
    }

    protected override void RemoveItemEvents() {
      IntBoundedConstraint.Changed -= new EventHandler(IntBoundedConstraint_Changed);
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      IntBoundedConstraint.Changed += new EventHandler(IntBoundedConstraint_Changed);
    }

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
