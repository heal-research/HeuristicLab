#region License Information

/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Shape Constraint View")]
  [Content(typeof(ShapeConstraint), true)]
  public sealed partial class ShapeConstraintView : ItemView {
    public new ShapeConstraint Content {
      get => (ShapeConstraint)base.Content;
      set => base.Content = value;
    }

    public ShapeConstraintView() {
      InitializeComponent();
      int[] items = { 1, 2, 3 };
      numberOfDerivationsComboBox.DataSource = items;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        this.regionView.Content = null;
      } else {
        this.regionView.Content = Content.Regions;
        UpdateControls();
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += Content_Changed;
    }

    protected override void DeregisterContentEvents() {
      Content.Changed -= Content_Changed;
      base.DeregisterContentEvents();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      variableInput.Enabled = Content != null && !Locked && !ReadOnly && Content.Variable != String.Empty;
      numberOfDerivationsComboBox.Enabled = Content != null && !Locked && !ReadOnly;
      lowerboundInput.Enabled = Content != null && !Locked && !ReadOnly;
      upperboundInput.Enabled = Content != null && !Locked && !ReadOnly;
      thresholdLowerBoundInput.Enabled = Content != null && !Locked && !ReadOnly;
      thresholdUpperBoundInput.Enabled = Content != null && !Locked && !ReadOnly;
      weightTextBox.Enabled = Content != null && !Locked && !ReadOnly;
    }


    #region helpers

    private static double ParseDoubleValue(string input, Control control, ErrorProvider errorProvider) {
      input = input.ToLower();
      switch (input) {
        case "inf.":
        case "+inf.":
        case "Infinity":
          return double.PositiveInfinity;
        case "-inf.":
        case "-Infinity":
          return double.NegativeInfinity;
        default: {
            if (double.TryParse(input, out var value)) {
              return value;
            }

            errorProvider.SetError(control, "Invalid input: value must be a double.");
            return double.NaN;
          }
      }
    }

    private void UpdateControls() {
      if (Content == null) {
        lowerboundInput.Text = string.Empty;
        upperboundInput.Text = string.Empty;
        thresholdLowerBoundInput.Text = string.Empty;
        thresholdUpperBoundInput.Text = string.Empty;
        weightTextBox.Text = string.Empty;
        return;
      }

      lowerboundInput.Text = Content.Interval.LowerBound.ToString();
      upperboundInput.Text = Content.Interval.UpperBound.ToString();
      thresholdLowerBoundInput.Text = Content.Threshold.LowerBound.ToString();
      thresholdUpperBoundInput.Text = Content.Threshold.UpperBound.ToString();
      weightTextBox.Text = Content.Weight.ToString();

      variableInput.Text = Content.Variable;
      if (!Content.IsDerivative) {
        numberOfDerivationsComboBox.Enabled = false;
        numberOfDerivationsComboBox.SelectedItem = null;
      } else {
        numberOfDerivationsComboBox.Enabled = true;
        numberOfDerivationsComboBox.SelectedItem = Content.NumberOfDerivations;
      }

      if (Content.Regions.Count > 0) {
        regionView.Content = Content.Regions;
        regionLabel.Visible = true;
        regionView.Visible = true;
      } else {
        regionLabel.Visible = false;
        regionView.Visible = false;
      }
    }

    #endregion

    #region control event handlers

    private void lowerboundInput_Validating(object sender, CancelEventArgs e) {
      var value = ParseDoubleValue(lowerboundInput.Text, lowerboundInput, errorProvider);
      if (double.IsNaN(value)) {
        errorProvider.SetError(lowerboundInput, "Invalid input: lower bound must be a double value.");
        e.Cancel = true;
        return;
      }

      if (value > Content.Interval.UpperBound) {
        errorProvider.SetError(lowerboundInput, "Invalid input: lower bound must be smaller than upper bound.");
        e.Cancel = true;
        return;
      }

      errorProvider.SetError(lowerboundInput, string.Empty);
      e.Cancel = false;
    }

    private void lowerboundInput_Validated(object sender, EventArgs e) {
      var value =
        ParseDoubleValue(lowerboundInput.Text, lowerboundInput, errorProvider);
      if (!double.IsNaN(value)) Content.Interval = new Interval(value, Content.Interval.UpperBound);
    }

    private void upperboundInput_Validating(object sender, CancelEventArgs e) {
      var value = ParseDoubleValue(upperboundInput.Text, upperboundInput, errorProvider);
      if (double.IsNaN(value)) {
        errorProvider.SetError(upperboundInput, "Invalid Input: upper bound must be a double value.");
        e.Cancel = true;
        return;
      }

      if (value < Content.Interval.LowerBound) {
        errorProvider.SetError(upperboundInput, "Invalid input: upper bound must be bigger than lower bound.");
        e.Cancel = true;
        return;
      }

      errorProvider.SetError(upperboundInput, string.Empty);
      e.Cancel = false;
    }

    private void upperboundInput_Validated(object sender, EventArgs e) {
      var value =
        ParseDoubleValue(upperboundInput.Text, upperboundInput, errorProvider);
      if (!double.IsNaN(value)) Content.Interval = new Interval(Content.Interval.LowerBound, value);
    }

    private void numberderivationInput_SelectedIndexChanged(object sender, EventArgs e) {
      if (numberOfDerivationsComboBox.SelectedItem == null) {
        Content.NumberOfDerivations = 0;
        numberOfDerivationsComboBox.Enabled = false;
        return;
      }

      if (Content != null) {
        if ((int)numberOfDerivationsComboBox.SelectedItem == 1)
          Content.NumberOfDerivations = 1;
        else if ((int)numberOfDerivationsComboBox.SelectedItem == 2)
          Content.NumberOfDerivations = 2;
        else if ((int)numberOfDerivationsComboBox.SelectedItem == 3)
          Content.NumberOfDerivations = 3;
      }
    }

    private void weightInput_TextChanged(object sender, EventArgs e) {
      var value = ParseDoubleValue(weightTextBox.Text, weightTextBox, errorProvider);
      if (!double.IsNaN(value)) Content.Weight = value;
    }

    private void variableInput_TextChanged(object sender, EventArgs e) {
      var value = variableInput.Text;
      if (!String.IsNullOrEmpty(value)) Content.Variable = value;
    }

    private void thresholdLowerBoundInput_Validating(object sender, CancelEventArgs e) {
      var value = ParseDoubleValue(thresholdLowerBoundInput.Text, thresholdLowerBoundInput, errorProvider);
      if (double.IsNaN(value)) {
        errorProvider.SetError(thresholdLowerBoundInput, "Invalid input: threshold lower bound must be a double value.");
        e.Cancel = true;
        return;
      }

      if (value > Content.Threshold.UpperBound) {
        errorProvider.SetError(thresholdLowerBoundInput, "Invalid input: threshold lower bound must be smaller than threshold upper bound.");
        e.Cancel = true;
        return;
      }

      errorProvider.SetError(thresholdLowerBoundInput, string.Empty);
      e.Cancel = false;
    }

    private void thresholdLowerBoundInput_Validated(object sender, EventArgs e) {
      var value = ParseDoubleValue(thresholdLowerBoundInput.Text, thresholdLowerBoundInput, errorProvider);
      if (!double.IsNaN(value)) Content.Threshold = new Interval(value, Content.Threshold.UpperBound);
    }

    private void thresholdUpperBoundInput_Validating(object sender, CancelEventArgs e) {
      var value = ParseDoubleValue(thresholdUpperBoundInput.Text, thresholdUpperBoundInput, errorProvider);
      if (double.IsNaN(value)) {
        errorProvider.SetError(thresholdUpperBoundInput, "Invalid Input: threshold upper bound must be a double value.");
        e.Cancel = true;
        return;
      }

      if (value < Content.Threshold.LowerBound) {
        errorProvider.SetError(thresholdUpperBoundInput, "Invalid input: threshold upper bound must be bigger than threshold lower bound.");
        e.Cancel = true;
        return;
      }

      errorProvider.SetError(thresholdUpperBoundInput, string.Empty);
      e.Cancel = false;
    }

    private void thresholdUpperBoundInput_Validated(object sender, EventArgs e) {
      var value = ParseDoubleValue(thresholdUpperBoundInput.Text, thresholdUpperBoundInput, errorProvider);
      if (!double.IsNaN(value)) Content.Threshold = new Interval(Content.Threshold.LowerBound, value);
    }
    #endregion

    #region content event handlers

    private void Content_Changed(object sender, EventArgs e) {
      UpdateControls();
    }
    #endregion

    
  }
}