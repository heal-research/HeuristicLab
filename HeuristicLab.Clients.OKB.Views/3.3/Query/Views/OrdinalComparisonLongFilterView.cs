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

using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.OKB.Query {
  [View("OrdinalComparisonLongFilter View")]
  [Content(typeof(OrdinalComparisonLongFilter), true)]
  public partial class OrdinalComparisonLongFilterView : OrdinalComparisonFilterView {
    public new OrdinalComparisonLongFilter Content {
      get { return (OrdinalComparisonLongFilter)base.Content; }
      set { base.Content = value; }
    }

    public OrdinalComparisonLongFilterView() {
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
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return)) {
        label.Focus();  // set focus on label to validate data
        e.SuppressKeyPress = true;
      } else if (e.KeyCode == Keys.Escape) {
        valueTextBox.Text = Content.Value.ToString();
        label.Focus();  // set focus on label to validate data
        e.SuppressKeyPress = true;
      }
    }
    private void valueTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      long val;
      if (!long.TryParse(valueTextBox.Text, out val)) {
        e.Cancel = true;
        errorProvider.SetError(valueTextBox, "Invalid Long Value");
        valueTextBox.SelectAll();
      }
    }
    private void valueTextBox_Validated(object sender, System.EventArgs e) {
      if (Content != null) {
        Content.Value = long.Parse(valueTextBox.Text);
        errorProvider.SetError(valueTextBox, string.Empty);
      }
    }
  }
}
