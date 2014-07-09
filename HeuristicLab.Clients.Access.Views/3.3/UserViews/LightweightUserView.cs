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

using System;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Access.Views {
  [View("LightweightUser View")]
  [Content(typeof(ItemList<UserGroupBase>), true)]
  public partial class LightweightUserView : ItemListView<UserGroupBase> {
    public LightweightUserView() {
      InitializeComponent();
      this.showDetailsCheckBox.Checked = false;
      this.itemsGroupBox.Text = "Users and Groups";
    }

    protected override void addButton_Click(object sender, System.EventArgs e) {
      using (LightweightUserGroupSelectionDialog dlg = new LightweightUserGroupSelectionDialog()) {
        DialogResult res = dlg.ShowDialog(this);
        if (res == DialogResult.OK) {
          dlg.SelectedUsersAndGroups.ForEach(x => {
            if (!Content.Contains(x)) {
              Content.Add(x);
              OnSelectedUsersChanged();
            }
          });
        }
      }
    }

    protected override void removeButton_Click(object sender, EventArgs e) {
      base.removeButton_Click(sender, e);
      OnSelectedUsersChanged();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      this.showDetailsCheckBox.Enabled = false;
    }

    public event EventHandler SelectedUsersChanged;
    protected virtual void OnSelectedUsersChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnSelectedUsersChanged);
      else {
        EventHandler handler = SelectedUsersChanged;
        if (handler != null)
          handler(this, EventArgs.Empty);
      }
    }
  }
}
