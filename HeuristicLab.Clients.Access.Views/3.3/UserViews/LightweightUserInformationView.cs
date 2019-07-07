#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Access.Views {
  [View("RefreshableLightweightUserInformation View")]
  [Content(typeof(LightweightUser), true)]
  public partial class LightweightUserInformationView : AsynchronousContentView {
    public new LightweightUser Content {
      get { return (LightweightUser)base.Content; }
      set { base.Content = value; }
    }

    public LightweightUserInformationView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      rolesListView.Items.Clear();
      groupsListView.Items.Clear();
      rolesListView.SmallImageList.Images.Clear();
      groupsListView.SmallImageList.Images.Clear();

      if (Content == null) {
        userNameTextBox.Clear();
        fullNameTextBox.Clear();
        emailTextBox.Clear();
      } else {
        userNameTextBox.Text = Content.UserName;
        fullNameTextBox.Text = Content.FullName;
        emailTextBox.Text = Content.EMail;

        foreach (Role r in Content.Roles)
          rolesListView.Items.Add(CreateListViewRoleItem(r));

        foreach (UserGroup g in Content.Groups)
          groupsListView.Items.Add(CreateListViewGroupItem(g));

        rolesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        groupsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked;
      //userNameTextBox.ReadOnly = !enabled;
      fullNameTextBox.ReadOnly = !enabled;
      emailTextBox.ReadOnly = !enabled;
      //rolesListView.Enabled = enabled;
      //groupsListView.Enabled = enabled;
      changePasswordButton.Enabled = enabled;
    }

    private void changePasswordButton_Click(object sender, System.EventArgs e) {
      using (ChangePasswordDialog dialog = new ChangePasswordDialog()) {
        dialog.ShowDialog(this);
      }
    }

    private ListViewItem CreateListViewRoleItem(Role item) {
      ListViewItem listViewItem = new ListViewItem();

      listViewItem.Text = item.ToString();
      rolesListView.SmallImageList.Images.Add(item.ItemImage);
      listViewItem.ImageIndex = rolesListView.SmallImageList.Images.Count - 1;
      listViewItem.Tag = item;

      return listViewItem;
    }

    private ListViewItem CreateListViewGroupItem(UserGroup item) {
      ListViewItem listViewItem = new ListViewItem();

      listViewItem.Text = item.ToString();
      groupsListView.SmallImageList.Images.Add(item.ItemImage);
      listViewItem.ImageIndex = rolesListView.SmallImageList.Images.Count - 1;
      listViewItem.Tag = item;

      return listViewItem;
    }

    private void fullNameTextBox_TextChanged(object sender, System.EventArgs e) {
      if (UserInformation.Instance.User.FullName != fullNameTextBox.Text) {
        UserInformation.Instance.User.FullName = fullNameTextBox.Text;
        OnUserInformationChanged();
      }
    }

    private void emailTextBox_TextChanged(object sender, System.EventArgs e) {
      if (UserInformation.Instance.User.EMail != emailTextBox.Text) {
        UserInformation.Instance.User.EMail = emailTextBox.Text;
        OnUserInformationChanged();
      }
    }

    #region Events
    public event EventHandler UserInformationChanged;
    private void OnUserInformationChanged() {
      EventHandler handler = UserInformationChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
