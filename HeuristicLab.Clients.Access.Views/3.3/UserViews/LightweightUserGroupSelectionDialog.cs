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
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Access.Views {
  public partial class LightweightUserGroupSelectionDialog : Form {
    private ItemList<UserGroupBase> selectedUsersAndGroups;
    public ItemList<UserGroupBase> SelectedUsersAndGroups {
      get {
        return selectedUsersAndGroups;
      }
    }

    public LightweightUserGroupSelectionDialog() {
      InitializeComponent();
    }

    private void OkButton_Click(object sender, System.EventArgs e) {
      selectedUsersAndGroups = lightweightUserGroupSelectionView.GetSelectedItems();
      this.DialogResult = System.Windows.Forms.DialogResult.OK;
      Close();
    }

    private void CancelButton_Click(object sender, System.EventArgs e) {
      selectedUsersAndGroups = lightweightUserGroupSelectionView.GetSelectedItems();
      this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      Close();
    }
  }
}
