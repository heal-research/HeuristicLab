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

using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.OKB.Query {
  [View("OrdinalComparisonFilter View")]
  [Content(typeof(OrdinalComparisonFilter), true)]
  public partial class OrdinalComparisonFilterView : FilterView {
    public new OrdinalComparisonFilter Content {
      get { return (OrdinalComparisonFilter)base.Content; }
      set { base.Content = value; }
    }

    public OrdinalComparisonFilterView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      comparisonComboBox.SelectedIndex = -1;
      if (Content != null) {
        if (Content.Comparison == OrdinalComparison.Less)
          comparisonComboBox.SelectedItem = "<";
        else if (Content.Comparison == OrdinalComparison.LessOrEqual)
          comparisonComboBox.SelectedItem = "<=";
        else if (Content.Comparison == OrdinalComparison.Equal)
          comparisonComboBox.SelectedItem = "=";
        else if (Content.Comparison == OrdinalComparison.GreaterOrEqual)
          comparisonComboBox.SelectedItem = ">=";
        else if (Content.Comparison == OrdinalComparison.Greater)
          comparisonComboBox.SelectedItem = ">";
        else if (Content.Comparison == OrdinalComparison.NotEqual)
          comparisonComboBox.SelectedItem = "<>";
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      comparisonComboBox.Enabled = Content != null && !ReadOnly;
    }

    private void comparisonComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        if (comparisonComboBox.SelectedItem.ToString() == "<")
          Content.Comparison = OrdinalComparison.Less;
        else if (comparisonComboBox.SelectedItem.ToString() == "<=")
          Content.Comparison = OrdinalComparison.LessOrEqual;
        else if (comparisonComboBox.SelectedItem.ToString() == "=")
          Content.Comparison = OrdinalComparison.Equal;
        else if (comparisonComboBox.SelectedItem.ToString() == ">=")
          Content.Comparison = OrdinalComparison.GreaterOrEqual;
        else if (comparisonComboBox.SelectedItem.ToString() == ">")
          Content.Comparison = OrdinalComparison.Greater;
        else if (comparisonComboBox.SelectedItem.ToString() == "<>")
          Content.Comparison = OrdinalComparison.NotEqual;
      }
    }
  }
}
