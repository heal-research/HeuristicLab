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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.Data.Views {
  public abstract partial class StringConvertibleMatrixVisibilityDialog<T> : Form where T : DataGridViewBand {
    public StringConvertibleMatrixVisibilityDialog() {
      InitializeComponent();
    }

    public StringConvertibleMatrixVisibilityDialog(IEnumerable<T> bands)
      : this() {
      this.Bands = bands;
      this.visibility = bands.Select(b => b.Visible).ToList();
      UpdateCheckBoxes();
    }

    private List<T> bands;
    public IEnumerable<T> Bands {
      get { return this.bands; }
      set { this.bands = new List<T>(value); }
    }

    private List<bool> visibility;
    public IList<bool> Visibility {
      get { return this.visibility; }
    }

    protected abstract void UpdateCheckBoxes();

    protected void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
      this.bands[e.Index].Visible = e.NewValue == CheckState.Checked;
      this.visibility[e.Index] = e.NewValue == CheckState.Checked;
    }

    protected void btnShowAll_Click(object sender, System.EventArgs e) {
      for (int i = 0; i < checkedListBox.Items.Count; i++)
        checkedListBox.SetItemChecked(i, true);
    }
    protected void btnHideAll_Click(object sender, System.EventArgs e) {
      for (int i = 0; i < checkedListBox.Items.Count; i++)
        checkedListBox.SetItemChecked(i, false);
    }
  }
}
