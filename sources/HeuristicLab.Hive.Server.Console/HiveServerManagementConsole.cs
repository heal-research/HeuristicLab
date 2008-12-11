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

namespace HeuristicLab.Hive.Server.Console {

  public delegate void closeForm(bool cf);

  public partial class HiveServerManagementConsole : Form {

    public event closeForm closeFormEvent;

    ResponseList<ClientGroup> clients = null;
    ResponseList<Job> jobs = null;
    ResponseList<UserGroup> userGroups = null;

    public HiveServerManagementConsole() {
      InitializeComponent();
      addItems();
    }

    private void addItems() {
      IClientManager clientManager =
        ServiceLocator.GetClientManager();

      IJobManager jobManager =
        ServiceLocator.GetJobManager();

      IUserRoleManager userRoleManager =
        ServiceLocator.GetUserRoleManager();


      clients = clientManager.GetAllClientGroups();
      jobs = jobManager.GetAllJobs();

      userGroups = userRoleManager.GetAllUserGroups();

      lvClientControl.Items.Clear();
      int count = 0;
      foreach (ClientGroup cg in clients.List) {
        tvClientControl.Nodes.Add(cg.Name);
        ListViewGroup lvg = new ListViewGroup(cg.Name, HorizontalAlignment.Left);
        foreach (ClientInfo ci in clientManager.GetAllClients().List) {
          tvClientControl.Nodes[tvClientControl.Nodes.Count - 1].Nodes.Add(ci.Name);
          lvClientControl.Items.Add(new ListViewItem(ci.Name, count, lvg));
          count = (count + 1) % 3;
        }
        lvClientControl.Groups.Add(lvg);
      } // Groups


      foreach (Job job in jobs.List) {
        tvJobControl.Nodes.Add(job.JobId.ToString());
      } // Jobs

      foreach (UserGroup ug in userGroups.List) {
        tvUserControl.Nodes.Add(ug.Name);
        ListViewGroup lvg = new ListViewGroup(ug.Name, HorizontalAlignment.Left);

        foreach (User users in ug.Members) {
          tvUserControl.Nodes[tvUserControl.Nodes.Count - 1].Nodes.Add(users.Name);
          lvUserControl.Items.Add(new ListViewItem(users.Name, count, lvg));
        }
        lvUserControl.Groups.Add(lvg);
      } // Users
    }

    /// <summary>
    /// Send event to Login-GUI when closing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void close_Click(object sender, EventArgs e) {
      if (closeFormEvent != null) {
        closeFormEvent(true);
      }
      this.Close();
    }

    /// <summary>
    /// Send evnt to Login-GUI when closing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HiveServerConsoleInformation_FormClosing(object sender, FormClosingEventArgs e) {
      if (closeFormEvent != null) {
        closeFormEvent(true);
      }

    }

    private void jobToolStripMenuItem1_Click(object sender, EventArgs e) {
      AddNewForm newForm = new AddNewForm("Job", false);
      newForm.Show();
    }

    private void userToolStripMenuItem1_Click(object sender, EventArgs e) {
      AddNewForm newForm = new AddNewForm("User", false);
      newForm.Show();
    }

    private void groupToolStripMenuItem2_Click(object sender, EventArgs e) {
      AddNewForm newForm = new AddNewForm("User", true);
      newForm.Show();

    }

  }
}