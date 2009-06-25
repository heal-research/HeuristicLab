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
using System.Threading;
using System.ServiceModel;

namespace HeuristicLab.Hive.Server.ServerConsole {

  public partial class HiveServerConsole : Form {

    HiveServerManagementConsole information = null;

    public HiveServerConsole() {
      InitializeComponent();
#if(DEBUG)
      tbIp.Text = "10.20.53.1";
      tbPort.Text = WcfSettings.GetDefaultPort().ToString();
      tbUserName.Text = "admin";
      tbPwd.Text = "admin";
#endif
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

    /// <summary>
    /// if the input is correct and the login-method returns a 
    /// valid response
    /// </summary>
    /// <returns></returns>
    private bool IsValid() {
      Thread t = new Thread(new ThreadStart(ShowWaitDlg));
      if (IsFormValidated()) {
        IPAddress ipAdress = IPAddress.Parse(tbIp.Text);
        int port = int.Parse(tbPort.Text);
        ServiceLocator.Address = tbIp.Text.Replace(" ", "");
        ServiceLocator.Port = tbPort.Text;
        IServerConsoleFacade scf = ServiceLocator.GetServerConsoleFacade();
        Response resp;
        try {
          lblError.Text = "Trying to logon...";
          this.Cursor = Cursors.WaitCursor;
          t.Start();
          resp = scf.Login(tbUserName.Text, tbPwd.Text);
          t.Abort();
          this.Cursor = Cursors.Default;
          if (resp.Success == true) return true;
          lblError.Text = resp.StatusMessage;
          MessageBox.Show("Wrong username or password");
        }
        catch (EndpointNotFoundException ene) {
          t.Abort();
          this.Cursor = Cursors.Default;
          MessageBox.Show(ene.Message);
          lblError.Text = "No Service at this address!";
          scf = null;
        }
        catch (Exception ex) {
          //login failed
          t.Abort();
          this.Cursor = Cursors.Default;
          MessageBox.Show(ex.Message);
          lblError.Text = "Logon failed! Please restart console";
        }
      }
        return false;
    }


    private void ShowWaitDlg() {
      LogonDlg dlg = new LogonDlg();
      dlg.ShowDialog();
    }

    /// <summary>
    /// Validates the form.
    /// </summary>
    /// <returns></returns>
    private bool IsFormValidated() {
      bool isValid = true;
      if (String.IsNullOrEmpty(tbUserName.Text)) {
        lblError.Text = "Please type in Username.";
        isValid = false;
      }
      if (String.IsNullOrEmpty(tbPwd.Text)) {
        lblError.Text = "Please type in Password.";
        isValid = false;
      }
      if (String.IsNullOrEmpty(tbIp.Text)) {
        lblError.Text = "Please type in Port.";
        isValid = false;
      }
      if (String.IsNullOrEmpty(tbPort.Text)) {
        lblError.Text = "Please type in IP-Address.";
        isValid = false;
      }
      try {
        int.Parse(tbPort.Text);
      }
      catch (Exception ex) {
        isValid = false;
        lblError.Text = "Please verify entered Port.";
      }
      try {
        IPAddress.Parse(tbIp.Text);
      }
      catch (Exception ex) {
        isValid = false;
        lblError.Text = "Please verify entered IP address.";
      }
      return isValid;
    }

    private void EnableForm(bool cf, bool error) {
      if (cf) {
        this.Visible = true;
        ServiceLocator.ShutDownFacade();
        lblError.Text = "";
        if (error == true) {
          lblError.Text = "Establishing server connection failed.";
        }
      }
    }

    private void HiveServerConsole_KeyPress(object sender, KeyPressEventArgs e) {
      if (e.KeyChar == (char)Keys.Return) {
        BtnLogin_Click(sender, e);
      }
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      Dispose();
    }
  }
}
