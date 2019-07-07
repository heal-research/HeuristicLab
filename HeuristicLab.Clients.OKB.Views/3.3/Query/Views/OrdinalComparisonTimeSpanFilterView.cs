#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.OKB.Query {
  [View("OrdinalComparisonTimeSpanFilterView View")]
  [Content(typeof(OrdinalComparisonTimeSpanFilter), true)]
  public partial class OrdinalComparisonTimeSpanFilterView : OrdinalComparisonFilterView {
    public new OrdinalComparisonTimeSpanFilter Content {
      get { return (OrdinalComparisonTimeSpanFilter)base.Content; }
      set { base.Content = value; }
    }

    public OrdinalComparisonTimeSpanFilterView() {
      InitializeComponent();
      errorProvider.SetIconAlignment(valueTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(valueTextBox, 2);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content == null) {
        valueTextBox.Text = string.Empty;
      } else if (Content.Value == 0) {
        valueTextBox.Text = "00:00:00";
      } else {
        valueTextBox.Text = TimeSpan.FromSeconds(Content.Value).ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      valueTextBox.Enabled = Content != null;
      valueTextBox.ReadOnly = ReadOnly;
    }

    private void valueTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        label.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        valueTextBox.Text = Content.Value.ToString();
        label.Focus();  // set focus on label to validate data
      }
    }
    private void valueTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      HeuristicLab.Data.IStringConvertibleValue tsv = new HeuristicLab.Data.TimeSpanValue();

      string errorMessage;
      if (!tsv.Validate(valueTextBox.Text, out errorMessage)) {
        e.Cancel = true;
        errorProvider.SetError(valueTextBox, "Invalid TimeSpan Value");
        valueTextBox.SelectAll();
      }
    }
    private void valueTextBox_Validated(object sender, System.EventArgs e) {
      if (Content != null) {
        HeuristicLab.Data.IStringConvertibleValue tsv = new HeuristicLab.Data.TimeSpanValue();

        tsv.SetValue(valueTextBox.Text);
        Content.Value = (long)((HeuristicLab.Data.TimeSpanValue)tsv).Value.TotalSeconds;
        errorProvider.SetError(valueTextBox, string.Empty);
        valueTextBox.Text = tsv.GetValue();
      }
    }
  }
}
