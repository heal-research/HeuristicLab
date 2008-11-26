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

namespace HeuristicLab.Hive.Server.Console {

  public partial class HiveServerConsole : Form {

    HiveServerManagementConsole information = null;

    public HiveServerConsole() {
      InitializeComponent();
      tbIp.Text = "10.20.53.1";
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
    private void btnLogin_Click(object sender, EventArgs e) {
      if (ipIsValid()) {
        string newIp = tbIp.Text;
        newIp = newIp.Replace(" ", "");

        ServiceLocator.Address = newIp;
        ServiceLocator.Port = this.tbPort.Text;

        this.Visible = false;
        information = new HiveServerManagementConsole();
        information.closeFormEvent += new closeForm(enableForm);
        information.Show();
      }
    }


    private static bool ipIsValid() {
      
      // TODO IP-Adress validation
      return true;
    }

    private void enableForm(bool cf) {
      if (cf) {
        this.Visible = true;
      }
    }
  }
}