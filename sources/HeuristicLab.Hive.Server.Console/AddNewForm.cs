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

namespace HeuristicLab.Hive.Server.Console {
  public partial class AddNewForm : Form {

    ResponseList<Job> jobGroups = null;
    ResponseList<UserGroup> userGroups = null;
    IUserRoleManager userRoleManager;
    bool group;

    public AddNewForm(string addForm, bool group) {
      this.group = group;
      InitializeComponent();
      this.Name = "Add " + addForm;

      lblOne.Text = addForm;
      if (group) {
        lblOne.Text += " Group";
      }

      lblGroup.Text = addForm + " Groups";

      if (addForm == "Job") {
        addJob();
      }
      if (addForm == "User") {
        addUser();
      }
    }

    private void addJob() {
      IJobManager jobManager =
        ServiceLocator.GetJobManager();
      jobGroups = jobManager.GetAllJobs();
      cbGroups.Items.Add("none");
      foreach (UserGroup ug in userGroups.List) {
        cbGroups.Items.Add(ug.Name);
      }
    }

    private void addUser() {
     userRoleManager = ServiceLocator.GetUserRoleManager();
      userGroups = userRoleManager.GetAllUserGroups();
      cbGroups.Items.Add("none");
      cbGroups.SelectedItem = cbGroups.Items.Count;
      foreach (UserGroup ug in userGroups.List) {
        cbGroups.Items.Add(ug.Name);
      }
    }

    private void btnAdd_Click(object sender, EventArgs e) {
      if (!group) {
        if (tbOne.Text != "") {
          User u = new User() { Name = tbOne.Text, Password = tbPwd.Text };
          ResponseObject<User> respUser = userRoleManager.AddNewUser(u);
          if (!cbGroups.SelectedText.Equals("none")) {
            u = respUser.Obj;
            foreach (UserGroup ug in userGroups.List) {
              if (cbGroups.SelectedItem.ToString().Equals(ug.Name)) {
                Response resp = userRoleManager.AddUserToGroup
                  (ug.PermissionOwnerId, u.PermissionOwnerId);
              }
            }
          }
        }
      } else {
        UserGroup ug = new UserGroup { Name = tbOne.Text };
        ResponseObject<UserGroup> respug = userRoleManager.AddNewUserGroup(ug);
        if (!cbGroups.SelectedText.Equals("none")) {
          ug = respug.Obj;
          foreach (UserGroup ugs in userGroups.List) {
            if (cbGroups.SelectedText.Equals(ugs.Name)) {
              Response resp = userRoleManager.AddUserGroupToGroup
                (ug.PermissionOwnerId, ugs.PermissionOwnerId);
            }
          }
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e) {
      this.Close();
    }
  }
}
