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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public partial class IntParameterBoundConstraintView : ViewBase {
    public IntParameterBoundConstraint IntParameterBoundConstraint {
      get { return (IntParameterBoundConstraint)Item; }
      set { base.Item = value; }
    }

    public IntParameterBoundConstraintView() {
      InitializeComponent();
    }

    public IntParameterBoundConstraintView(IntParameterBoundConstraint dpbc)
      : this() {
      IntParameterBoundConstraint = dpbc;
    }

    protected override void RemoveItemEvents() {
      IntParameterBoundConstraint.Changed -= new EventHandler(IntParameterBoundConstraint_Changed);
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      IntParameterBoundConstraint.Changed += new EventHandler(IntParameterBoundConstraint_Changed);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (IntParameterBoundConstraint == null) {
        pmTextBox.Enabled = false;
        lbTextBox.Enabled = false;
        lbTextBox.Text = "";
        lbIncludedCheckBox.Enabled = false;
        lbEnabledCheckBox.Checked = false;

        ubTextBox.Enabled = false;
        ubTextBox.Text = "";
        ubIncludedCheckBox.Checked = false;
        ubEnabledCheckBox.Enabled = false;
      } else {
        pmTextBox.Text = IntParameterBoundConstraint.ParameterName;
        lbTextBox.Text = IntParameterBoundConstraint.LowerBound.ToString();
        lbTextBox.Enabled = IntParameterBoundConstraint.LowerBoundEnabled;
        lbIncludedCheckBox.Checked = IntParameterBoundConstraint.LowerBoundIncluded;
        lbIncludedCheckBox.Enabled = true;
        lbEnabledCheckBox.Checked = IntParameterBoundConstraint.LowerBoundEnabled;
        lbEnabledCheckBox.Enabled = true;

        ubTextBox.Text = IntParameterBoundConstraint.UpperBound.ToString();
        ubTextBox.Enabled = IntParameterBoundConstraint.UpperBoundEnabled;
        ubIncludedCheckBox.Checked = IntParameterBoundConstraint.UpperBoundIncluded;
        ubIncludedCheckBox.Enabled = true;
        ubEnabledCheckBox.Checked = IntParameterBoundConstraint.UpperBoundEnabled;
        ubEnabledCheckBox.Enabled = true;
      }
    }

    void IntParameterBoundConstraint_Changed(object sender, EventArgs e) {
      Refresh();
    }

    private void lbEnabledCheckBox_CheckedChanged(object sender, EventArgs e) {
      IntParameterBoundConstraint.LowerBoundEnabled = lbEnabledCheckBox.Checked;
      lbTextBox.Enabled = lbEnabledCheckBox.Checked;
      lbIncludedCheckBox.Enabled = lbEnabledCheckBox.Checked;
    }

    private void ubEnabledCheckBox_CheckedChanged(object sender, EventArgs e) {
      IntParameterBoundConstraint.UpperBoundEnabled = ubEnabledCheckBox.Checked;
      ubTextBox.Enabled = ubEnabledCheckBox.Checked;
      ubIncludedCheckBox.Enabled = ubEnabledCheckBox.Checked;
    }

    private void lbIncludedCheckBox_CheckedChanged(object sender, EventArgs e) {
      IntParameterBoundConstraint.LowerBoundIncluded = lbIncludedCheckBox.Checked;
    }

    private void ubIncludedCheckBox_CheckedChanged(object sender, EventArgs e) {
      IntParameterBoundConstraint.UpperBoundIncluded = ubIncludedCheckBox.Checked;
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
      IntParameterBoundConstraint.LowerBound = int.Parse(lbTextBox.Text);
    }

    private void ubTextBox_Validated(object sender, EventArgs e) {
      IntParameterBoundConstraint.UpperBound = int.Parse(ubTextBox.Text);
    }

    private void pmTextBox_TextChanged(object sender, EventArgs e) {
      IntParameterBoundConstraint.ParameterName = pmTextBox.Text;
    }
  }
}
