#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.Windows.Forms;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  public partial class DensityTrackbar : UserControl {

    public decimal Value {
      get { return TickToValue(trackBar.Value); }
      set { trackBar.Value = ValueToTick(value); }
    }
    public event EventHandler ValueChanged;

    public bool Checked {
      get { return radioButton.Checked; }
      set { radioButton.Checked = value; }
    }
    public event EventHandler CheckedChanged;

    public DoubleLimit Limits {
      get { return doubleLimitView.Content; }
    }
    public event EventHandler LimitsChanged;

    private IList<double> trainingValues;

    public DensityTrackbar(string name, DoubleLimit limit, IList<double> trainingValues) {
      InitializeComponent();

      this.trainingValues = trainingValues;

      doubleLimitView.Content = limit;
      doubleLimitView.Content.ValueChanged += doubleLimit_ValueChanged;

      textBox.Text = Value.ToString(CultureInfo.InvariantCulture);
      radioButton.Text = name;

      UpdateDensityChart();
    }

    public void UpdateTrainingValues(IList<double> trainingValues) {
      this.trainingValues = trainingValues;
      UpdateDensityChart();
    }

    private void UpdateDensityChart() {
      chart.UpdateChart(trainingValues, Limits.Lower, Limits.Upper,
        numBuckets: trackBar.Maximum - trackBar.Minimum, minimumHeight: 0.1);
    }

    #region Event Handlers
    private void trackBar_ValueChanged(object sender, EventArgs e) {
      textBox.Text = Value.ToString(CultureInfo.CurrentUICulture);
      textBox.Update();
      OnValueChanged();
    }
    private void doubleLimit_ValueChanged(object sender, EventArgs e) {
      UpdateDensityChart();
      OnLimitsChanged();
      OnValueChanged();
    }
    private void radioButton_CheckedChanged(object sender, EventArgs e) {
      Checked = radioButton.Checked;
      trackBar.Enabled = !Checked;
      textBox.Enabled = !Checked;
      textBox.Text = Checked ? "Plotted" : Value.ToString(CultureInfo.CurrentUICulture);
      radioButton.BackColor = Checked ? SystemColors.ActiveCaption : SystemColors.Control;
      OnCheckedChanged();
    }
    protected override void OnTextChanged(EventArgs e) {
      base.OnTextChanged(e);
      radioButton.Text = Text;
    }

    private void textBox_Validating(object sender, CancelEventArgs e) {
      if (Checked) return;
      decimal number;
      if (!decimal.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.CurrentUICulture.NumberFormat, out number)) {
        e.Cancel = true;
        errorProvider.SetIconAlignment(textBox, ErrorIconAlignment.MiddleLeft);
        errorProvider.SetIconPadding(textBox, 2);
        errorProvider.SetError(textBox, "Illegal number format");
      }
    }

    private void textBox_Validated(object sender, EventArgs e) {
      if (Checked) return;
      errorProvider.SetError(textBox, string.Empty);
      Value = decimal.Parse(textBox.Text, NumberStyles.Any, CultureInfo.CurrentUICulture.NumberFormat);
    }
    #endregion

    protected virtual void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
    }
    protected virtual void OnCheckedChanged() {
      if (CheckedChanged != null)
        CheckedChanged(this, EventArgs.Empty);
    }
    protected virtual void OnLimitsChanged() {
      if (LimitsChanged != null)
        LimitsChanged(this, EventArgs.Empty);
    }

    #region Helpers
    private decimal TickToValue(int tick) {
      return TickToValue(tick, trackBar.Maximum - trackBar.Minimum, (decimal)Limits.Lower, (decimal)Limits.Upper);
    }
    private int ValueToTick(decimal value) {
      return ValueToTick(value, trackBar.Maximum - trackBar.Minimum, (decimal)Limits.Lower, (decimal)Limits.Upper);
    }
    private static decimal TickToValue(int tick, int numberOfTicks, decimal lower, decimal upper) {
      return lower + (upper - lower) * tick / numberOfTicks;
    }
    private static int ValueToTick(decimal value, int numberOfTicks, decimal lower, decimal upper) {
      return (int)((value - lower) / ((upper - lower) / numberOfTicks));
    }
    #endregion
  }
}
