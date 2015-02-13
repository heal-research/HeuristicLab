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

using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.OKB.Query {
  [View("OrdinalComparisonFloatFilter View")]
  [Content(typeof(OrdinalComparisonFloatFilter), true)]
  public partial class OrdinalComparisonFloatFilterView : OrdinalComparisonFilterView {
    public new OrdinalComparisonFloatFilter Content {
      get { return (OrdinalComparisonFloatFilter)base.Content; }
      set { base.Content = value; }
    }

    public OrdinalComparisonFloatFilterView() {
      InitializeComponent();
      errorProvider.SetIconAlignment(valueTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(valueTextBox, 2);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      valueTextBox.Text = Content == null ? string.Empty : Content.Value.ToString();
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
      float val;
      if (!float.TryParse(valueTextBox.Text, out val)) {
        e.Cancel = true;
        errorProvider.SetError(valueTextBox, "Invalid Float Value");
        valueTextBox.SelectAll();
      }
    }
    private void valueTextBox_Validated(object sender, System.EventArgs e) {
      if (Content != null) {
        Content.Value = float.Parse(valueTextBox.Text);
        errorProvider.SetError(valueTextBox, string.Empty);
      }
    }
  }
}
