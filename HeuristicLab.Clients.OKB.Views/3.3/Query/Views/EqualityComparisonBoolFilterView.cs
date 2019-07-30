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
  [View("EqualityComparisonBoolFilter View")]
  [Content(typeof(EqualityComparisonBoolFilter), true)]
  public partial class EqualityComparisonBoolFilterView : EqualityComparisonFilterView {
    public new EqualityComparisonBoolFilter Content {
      get { return (EqualityComparisonBoolFilter)base.Content; }
      set { base.Content = value; }
    }

    public EqualityComparisonBoolFilterView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      valueComboBox.SelectedIndex = -1;
      if (Content != null) {
        valueComboBox.SelectedItem = Content.Value ? "true" : "false";
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      valueComboBox.Enabled = Content != null && !ReadOnly;
    }

    private void valueComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        Content.Value = valueComboBox.SelectedItem.ToString() == "true";
      }
    }
  }
}
