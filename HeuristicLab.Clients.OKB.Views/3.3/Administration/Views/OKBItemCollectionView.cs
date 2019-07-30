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

using System;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.Administration {
  [View("OKBItemCollection View")]
  [Content(typeof(OKBItemCollection<>), true)]
  public partial class OKBItemCollectionView<T> : ItemCollectionView<T> where T : class, IOKBItem {
    public OKBItemCollectionView() {
      InitializeComponent();
      itemsGroupBox.Text = "OKB Items";
    }

    protected override void removeButton_Click(object sender, System.EventArgs e) {
      try {
        base.removeButton_Click(sender, e);
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog(this, "Delete failed.", ex);
      }
    }
    protected override void itemsListView_KeyDown(object sender, KeyEventArgs e) {
      try {
        base.itemsListView_KeyDown(sender, e);
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog(this, "Delete failed.", ex);
      }
    }
    protected override void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      try {
        base.itemsListView_ItemDrag(sender, e);
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog(this, "Delete failed.", ex);
      }
    }
  }
}
