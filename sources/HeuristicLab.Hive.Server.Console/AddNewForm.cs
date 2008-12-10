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

    ResponseList<UserGroup> userGroups = null;

    public AddNewForm(string addForm, bool group) {

      InitializeComponent();
      this.Name = "Add " + addForm;

      lblOne.Text = addForm;

      lblGroup.Text = addForm + " Groups";
    }

    private void addJob() {
      IUserRoleManager userRoleManager =
ServiceLocator.GetUserRoleManager();
      userGroups = userRoleManager.GetAllUserGroups();

    }

    private void addUser() {

    }
  }
}
