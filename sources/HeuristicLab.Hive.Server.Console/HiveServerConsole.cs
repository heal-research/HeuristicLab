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
using System.Security.Cryptography;
using System.Net;

namespace HeuristicLab.Hive.Server.Console {

  public partial class HiveServerConsole : Form {

    HiveServerManagementConsole information = null;

    public HiveServerConsole() {
      InitializeComponent();
    }

    private void tsmiExit_Click(object sender, EventArgs e) {
      this.Close();
    }

    /// <summary>
    /// When login button is clicked, the ManagementConsole
    /// will be opened
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnLogin_Click(object sender, EventArgs e) {
      string newIp = tbIp.Text;
      newIp = newIp.Replace(" ", "");

        ServiceLocator.Address = newIp;
        ServiceLocator.Port = this.tbPort.Text;

        if (IsValid()) {
          this.Visible = false;
          information = new HiveServerManagementConsole();
          information.closeFormEvent += new closeForm(EnableForm);
          information.Show();
        }
    }


    private bool IsValid() {
      if ((tbUserName.Text != "") &&
          (tbPwd.Text != "") &&
          (tbIp.Text != "") &&
          (tbPort.Text != "")) {
        try {
          IPAddress ipAdress;
          int port;
          if ((IPAddress.TryParse(tbIp.Text, out ipAdress)) &&
            int.TryParse(tbPort.Text, out port)) {
            IUserRoleManager userManager =
          ServiceLocator.GetUserRoleManager();
            ResponseList<User> user = userManager.GetAllUsers();
            user = userManager.GetAllUsers();
          } else {
            lblError.Text = "IP or Port not valid";
          }
        }
        catch (Exception ex) {
          lblError.Text = "Server not online";
          return false;
        }
        return true;
      } else {
        if (tbUserName.Text == "") {
          lblError.Text = "Please type in Username";
        } else if (tbPwd.Text == "") {
          lblError.Text = "Please type in Password";
        } else if (tbPort.Text == "") {
          lblError.Text = "Please type in Port";
        } else if (tbIp.Text == "") {
          lblError.Text = "Please type in IP-Adress";
        }
        return false;
      }
    }

    private void EnableForm(bool cf, bool error) {
      if (cf) {
        this.Visible = true;
        if (error == true) {
          lblError.Text = "Something went wrong with the server";
        }
      }
    }
  }
}