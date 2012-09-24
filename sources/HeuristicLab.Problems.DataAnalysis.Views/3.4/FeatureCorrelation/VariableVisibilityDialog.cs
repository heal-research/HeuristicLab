#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.DataAnalysis.Views {
  public partial class VariableVisibilityDialog : Form {
    public VariableVisibilityDialog() {
      InitializeComponent();
    }

    public VariableVisibilityDialog(IEnumerable<string> columns, IEnumerable<bool> visibility)
      : this() {
      if (columns.Count() != visibility.Count()) {
        throw new ArgumentException("There have to be as many variables as values for visibility!");
      }
      this.columns = columns.ToList();
      this.visibility = visibility.ToList();
      UpdateCheckBoxes();
    }

    private List<string> columns;
    public IList<string> Columns {
      get { return this.columns; }
    }
    private List<bool> visibility;
    public IList<bool> Visibility {
      get { return this.visibility; }
    }

    private void UpdateCheckBoxes() {
      for (int i = 0; i < columns.Count; i++) {
        checkedListBox.Items.Add(columns[i], visibility[i]);
      }
    }

    public event ItemCheckEventHandler VariableVisibilityChanged;

    private void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
      this.visibility[e.Index] = e.NewValue == CheckState.Checked;
      if (VariableVisibilityChanged != null) {
        VariableVisibilityChanged(this, e);
      }
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
