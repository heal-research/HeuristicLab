#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Common;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class DefineArithmeticProgressionDialog : Form {
    private bool allowOnlyInteger;

    public decimal Minimum { get; private set; }
    public decimal Maximum { get; private set; }
    public decimal Step { get; private set; }

    public IEnumerable<decimal> Values {
      get { return SequenceGenerator.GenerateSteps(Minimum, Maximum, Step, includeEnd: true); }
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
    public DefineArithmeticProgressionDialog(bool allowOnlyInteger, decimal minimum, decimal maximum, decimal step)
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
      decimal value = 0;
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
        if (!decimal.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture.NumberFormat, out value)) {
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
  }
}
