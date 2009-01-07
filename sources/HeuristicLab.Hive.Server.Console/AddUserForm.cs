#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Server.ServerConsole {
  public partial class AddUserForm : Form {

    ResponseList<UserGroup> userGroups = null;
    IUserRoleManager userRoleManager;
    bool group;

    public AddUserForm(string addForm, bool group) {
      this.group = group;
      InitializeComponent();
      this.Name = "Add " + addForm;

      lblOne.Text = addForm;
      if (group) {
        lblOne.Text += " Group";
      }

      lblGroup.Text = addForm + " Groups";

      if (addForm == "User") {
        AddUser();
      }
    }

    private void AddUser() {
     userRoleManager = ServiceLocator.GetUserRoleManager();
      userGroups = userRoleManager.GetAllUserGroups();
      cbGroups.Items.Add("none");
      cbGroups.SelectedIndex = 0;
      foreach (UserGroup ug in userGroups.List) {
        cbGroups.Items.Add(ug.Name);
      }
    }

    private void BtnAdd_Click(object sender, EventArgs e) {
      if (!group) {
        if (tbOne.Text != "") {
          User u = new User() { Name = tbOne.Text, Password = tbPwd.Text };
          ResponseObject<User> respUser = userRoleManager.AddNewUser(u);
          if (respUser.Success == true) {
            if (cbGroups.SelectedIndex != 0) {
              u = respUser.Obj;
              foreach (UserGroup ug in userGroups.List) {
                if (cbGroups.SelectedItem.ToString().Equals(ug.Name)) {
                  Response resp = userRoleManager.AddUserToGroup
                    (ug.Id, u.Id);
                }
              }
            }
          }
        }
      } else {
        UserGroup ug = new UserGroup { Name = tbOne.Text };
        ResponseObject<UserGroup> respug = userRoleManager.AddNewUserGroup(ug);
        if (respug.Success == true) {
          if (!cbGroups.SelectedText.Equals("none")) {
            ug = respug.Obj;
            foreach (UserGroup ugs in userGroups.List) {
              if (cbGroups.SelectedText.Equals(ugs.Name)) {
                Response resp = userRoleManager.AddUserGroupToGroup
                  (ug.Id, ugs.Id);
              }
            }
          }
        }
      }
      this.Close();
    }

    private void BtnClose_Click(object sender, EventArgs e) {
      this.Close();
    }
  }
}
