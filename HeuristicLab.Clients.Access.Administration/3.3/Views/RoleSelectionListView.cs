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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Access.Administration {
  [View("RoleSelectionList View")]
  [Content(typeof(IItemList<Role>), false)]
  public partial class RoleSelectionListView : ItemListView<Role> {
    public RoleSelectionListView() {
      InitializeComponent();
      ItemsListView.CheckBoxes = true;
      ItemsListView.View = System.Windows.Forms.View.Details;
      showDetailsCheckBox.Checked = false;
      itemsGroupBox.Text = "Roles";
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();

      addButton.Enabled = false;
      removeButton.Enabled = false;
      showDetailsCheckBox.Enabled = false;
      moveUpButton.Enabled = false;
      moveDownButton.Enabled = false;
    }

    protected override void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      base.itemsListView_SelectedIndexChanged(sender, e);
      moveUpButton.Enabled = false;
      moveDownButton.Enabled = false;
      removeButton.Enabled = false;
    }
  }
}
