#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class DefineArithmeticProgressionDialog : Form {
    private bool allowOnlyInteger;

    public double Minimum { get; private set; }
    public double Maximum { get; private set; }
    public double Step { get; private set; }

    public IEnumerable<double> Values {
      get { return EnumerateProgression().Reverse(); }
    }

    public DefineArithmeticProgressionDialog() {
      InitializeComponent();
      Icon = HeuristicLab.Common.Resources.HeuristicLab.Icon;
      Minimum = 0; Maximum = 100; Step = 10;
    }
    public DefineArithmeticProgressionDialog(bool allowOnlyInteger)
      : this() {
      this.allowOnlyInteger = allowOnlyInteger;
    }
    public DefineArithmeticProgressionDialog(bool allowOnlyInteger, double minimum, double maximum, double step)
      : this(allowOnlyInteger) {
      Minimum = minimum;
      Maximum = maximum;
      Step = step;
    }

    private void DefineArithmeticProgressionDialog_Load(object sender, System.EventArgs e) {
      minimumTextBox.Text = Minimum.ToString();
      maximumTextBox.Text = Maximum.ToString();
      stepSizeTextBox.Text = Step.ToString();
    }

    private void textBox_Validating(object sender, CancelEventArgs e) {
      var textBox = (TextBox)sender;
      double value = 0;
      if (allowOnlyInteger) {
        int intValue;
        if (!int.TryParse(textBox.Text, out intValue)) {
          errorProvider.SetError(textBox, "Please enter a valid integer value.");
          e.Cancel = true;
          return;
        } else {
          value = intValue;
          errorProvider.SetError(textBox, null);
        }
      } else {
        if (!double.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture.NumberFormat, out value)) {
          errorProvider.SetError(textBox, "Please enter a valid double value.");
          e.Cancel = true;
          return;
        } else errorProvider.SetError(textBox, null);
      }
      if (textBox == minimumTextBox) Minimum = value;
      else if (textBox == maximumTextBox) Maximum = value;
      else if (textBox == stepSizeTextBox) Step = value;
      okButton.Enabled = IsValid();
    }

    private bool IsValid() {
      return Minimum <= Maximum && Step >= 0;
    }

    private IEnumerable<double> EnumerateProgression() {
      double value = Maximum;
      bool minimumIncluded = false;
      int i = 1;
      while (value >= Minimum) {
        if (value.IsAlmost(Minimum)) {
          yield return Minimum;
          minimumIncluded = true;
        } else yield return value;

        if (Step == 0) break; // a step size of 0 will only output maximum and minimum
        if (allowOnlyInteger) {
          value = (int)Maximum - i * (int)Step;
        } else {
          value = Maximum - i * Step;
        }
        i++;
      }
      if (!minimumIncluded) {
        yield return Minimum;
      }
    }
  }
}
