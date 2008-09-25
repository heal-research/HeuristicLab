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
  public partial class DoubleParameterBoundConstraintView : ViewBase {
    public DoubleParameterBoundConstraint DoubleParameterBoundConstraint {
      get { return (DoubleParameterBoundConstraint)Item; }
      set { base.Item = value; }
    }

    public DoubleParameterBoundConstraintView() {
      InitializeComponent();
    }

    public DoubleParameterBoundConstraintView(DoubleParameterBoundConstraint dpbc)
      : this() {
      DoubleParameterBoundConstraint = dpbc;
    }

    protected override void RemoveItemEvents() {
      DoubleParameterBoundConstraint.Changed -= new EventHandler(DoubleParameterBoundConstraint_Changed);
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      DoubleParameterBoundConstraint.Changed += new EventHandler(DoubleParameterBoundConstraint_Changed);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (DoubleParameterBoundConstraint == null) {
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
        pmTextBox.Text = DoubleParameterBoundConstraint.ParameterName;
        lbTextBox.Text = DoubleParameterBoundConstraint.LowerBound.ToString("r");
        lbTextBox.Enabled = DoubleParameterBoundConstraint.LowerBoundEnabled;
        lbIncludedCheckBox.Checked = DoubleParameterBoundConstraint.LowerBoundIncluded;
        lbIncludedCheckBox.Enabled = true;
        lbEnabledCheckBox.Checked = DoubleParameterBoundConstraint.LowerBoundEnabled;
        lbEnabledCheckBox.Enabled = true;

        ubTextBox.Text = DoubleParameterBoundConstraint.UpperBound.ToString("r");
        ubTextBox.Enabled = DoubleParameterBoundConstraint.UpperBoundEnabled;
        ubIncludedCheckBox.Checked = DoubleParameterBoundConstraint.UpperBoundIncluded;
        ubIncludedCheckBox.Enabled = true;
        ubEnabledCheckBox.Checked = DoubleParameterBoundConstraint.UpperBoundEnabled;
        ubEnabledCheckBox.Enabled = true;
      }
    }

    void DoubleParameterBoundConstraint_Changed(object sender, EventArgs e) {
      Refresh();
    }

    private void lbEnabledCheckBox_CheckedChanged(object sender, EventArgs e) {
      DoubleParameterBoundConstraint.LowerBoundEnabled = lbEnabledCheckBox.Checked;
      lbTextBox.Enabled = lbEnabledCheckBox.Checked;
      lbIncludedCheckBox.Enabled = lbEnabledCheckBox.Checked;
    }

    private void ubEnabledCheckBox_CheckedChanged(object sender, EventArgs e) {
      DoubleParameterBoundConstraint.UpperBoundEnabled = ubEnabledCheckBox.Checked;
      ubTextBox.Enabled = ubEnabledCheckBox.Checked;
      ubIncludedCheckBox.Enabled = ubEnabledCheckBox.Checked;
    }

    private void lbIncludedCheckBox_CheckedChanged(object sender, EventArgs e) {
      DoubleParameterBoundConstraint.LowerBoundIncluded = lbIncludedCheckBox.Checked;
    }

    private void ubIncludedCheckBox_CheckedChanged(object sender, EventArgs e) {
      DoubleParameterBoundConstraint.UpperBoundIncluded = ubIncludedCheckBox.Checked;
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
      DoubleParameterBoundConstraint.LowerBound = double.Parse(lbTextBox.Text);
    }

    private void ubTextBox_Validated(object sender, EventArgs e) {
      DoubleParameterBoundConstraint.UpperBound = double.Parse(ubTextBox.Text);
    }

    private void pmTextBox_TextChanged(object sender, EventArgs e) {
      DoubleParameterBoundConstraint.ParameterName = pmTextBox.Text;
    }
  }
}
