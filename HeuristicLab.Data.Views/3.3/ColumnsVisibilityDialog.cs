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
using System.Windows.Forms;

namespace HeuristicLab.Data.Views {
  public partial class ColumnsVisibilityDialog : Form {
    public ColumnsVisibilityDialog() {
      InitializeComponent();
    }

    public ColumnsVisibilityDialog(IEnumerable<DataGridViewColumn> columns)
      : this() {
      this.Columns = columns;
    }

    private List<DataGridViewColumn> columns;
    public IEnumerable<DataGridViewColumn> Columns {
      get { return this.columns; }
      set {
        this.columns = new List<DataGridViewColumn>(value);
        UpdateCheckBoxes();
      }
    }

    private void UpdateCheckBoxes() {
      this.checkedListBox.Items.Clear();
      foreach (DataGridViewColumn column in columns)
        checkedListBox.Items.Add(column.HeaderText, column.Visible);
    }

    private void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
      this.columns[e.Index].Visible = e.NewValue == CheckState.Checked;
    }

    private void btnShowAll_Click(object sender, System.EventArgs e) {
      for (int i = 0; i < checkedListBox.Items.Count; i++)
        checkedListBox.SetItemChecked(i, true);
    }
    private void btnHideAll_Click(object sender, System.EventArgs e) {
      for (int i = 0; i < checkedListBox.Items.Count; i++)
        checkedListBox.SetItemChecked(i, false);
    }
  }
}
