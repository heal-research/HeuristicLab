#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class DefineArithmeticTimeSpanProgressionDialog : Form {
    public TimeSpan Minimum { get; private set; }
    public TimeSpan Maximum { get; private set; }
    public TimeSpan Step { get; private set; }

    public IEnumerable<TimeSpan> Values {
      get { return EnumerateProgression().Reverse(); }
    }

    public DefineArithmeticTimeSpanProgressionDialog() {
      InitializeComponent();
      Icon = HeuristicLab.Common.Resources.HeuristicLab.Icon;
      Minimum = TimeSpan.Zero; Maximum = TimeSpan.FromSeconds(60); Step = TimeSpan.FromSeconds(10);
    }
    public DefineArithmeticTimeSpanProgressionDialog(TimeSpan minimum, TimeSpan maximum, TimeSpan step)
      : this() {
      Minimum = minimum;
      Maximum = maximum;
      Step = step;
    }

    private void DefineArithmeticTimeSpanProgressionDialog_Load(object sender, System.EventArgs e) {
      minimumTextBox.Text = TimeSpanHelper.FormatNatural(Minimum, true);
      maximumTextBox.Text = TimeSpanHelper.FormatNatural(Maximum, true);
      stepSizeTextBox.Text = TimeSpanHelper.FormatNatural(Step, true);
    }

    private void textBox_Validating(object sender, CancelEventArgs e) {
      var textBox = (TextBox)sender;
      TimeSpan value;
      errorProvider.SetError(textBox, String.Empty);
      if (!TimeSpanHelper.TryGetFromNaturalFormat(textBox.Text, out value)) {
        errorProvider.SetError(textBox, "Please enter a valid timespan.");
        e.Cancel = true;
        return;
      }
      if (textBox == minimumTextBox) Minimum = value;
      else if (textBox == maximumTextBox) Maximum = value;
      else if (textBox == stepSizeTextBox) Step = value;
      okButton.Enabled = IsValid();
    }

    private bool IsValid() {
      return Minimum <= Maximum && Step >= TimeSpan.Zero;
    }

    private IEnumerable<TimeSpan> EnumerateProgression() {
      var value = Maximum;
      var minimumIncluded = false;
      while (value >= Minimum) {
        if (value == Minimum) minimumIncluded = true;
        yield return value;

        if (Step == TimeSpan.Zero) break; // a step size of 0 will only output maximum and minimum
        value = value.Subtract(Step);
      }
      if (!minimumIncluded) {
        yield return Minimum;
      }
    }
  }
}
