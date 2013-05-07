#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [View("EqualityComparisonFilter View")]
  public partial class EqualityComparisonFilterView : FilterView {
    public new EqualityComparisonFilter Content {
      get { return (EqualityComparisonFilter)base.Content; }
      set { base.Content = value; }
    }

    public EqualityComparisonFilterView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      comparisonComboBox.SelectedIndex = -1;
      if (Content != null) {
        if (Content.Comparison == EqualityComparison.Equal)
          comparisonComboBox.SelectedItem = "=";
        else if (Content.Comparison == EqualityComparison.NotEqual)
          comparisonComboBox.SelectedItem = "<>";
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      comparisonComboBox.Enabled = Content != null && !ReadOnly;
    }

    private void comparisonComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        if (comparisonComboBox.SelectedItem.ToString() == "=")
          Content.Comparison = EqualityComparison.Equal;
        else if (comparisonComboBox.SelectedItem.ToString() == "<>")
          Content.Comparison = EqualityComparison.NotEqual;
      }
    }
  }
}
