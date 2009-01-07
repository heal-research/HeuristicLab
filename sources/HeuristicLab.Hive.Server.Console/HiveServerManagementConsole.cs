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

  public delegate void closeForm(bool cf, bool error);

  public partial class HiveServerManagementConsole : Form {

    public event closeForm closeFormEvent;

    ResponseList<ClientGroup> clients = null;
    ResponseList<ClientInfo> clientInfo = null;
    ResponseList<Job> jobs = null;
    ResponseList<UserGroup> userGroups = null;
    ResponseList<User> usersList = null;
    ListView lvClientDetails = null;

    public HiveServerManagementConsole() {
      InitializeComponent();
      AddClients();
      AddJobs();
      AddUsers();

      timerSyncronize.Tick += new EventHandler(TickSync);
      timerSyncronize.Start();
    }

    private void TickSync(object obj, EventArgs e) {
      AddClients();
      AddJobs();
      AddUsers();
    }

    private void AddClients() {
      try {
        IClientManager clientManager =
          ServiceLocator.GetClientManager();

        clients = clientManager.GetAllClientGroups();

        lvClientControl.Items.Clear();
        tvClientControl.Nodes.Clear();
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

        clientInfo = clientManager.GetAllClients();
        ListViewGroup lvunsorted = new ListViewGroup("unsorted", HorizontalAlignment.Left);
        foreach (ClientInfo ci in clientInfo.List) {
          tvClientControl.Nodes.Add(ci.Name);
          lvClientControl.Items.Add(new ListViewItem(ci.Name, count, lvunsorted));
          count = (count + 1) % 3;
        }
        lvClientControl.Groups.Add(lvunsorted);
        lvClientControl.Sorting = SortOrder.Ascending;
      }
      catch (Exception ex) {
        closeFormEvent(true, true);
        this.Close();
      }
    }

    private void AddJobs() {
      try {
        IJobManager jobManager =
          ServiceLocator.GetJobManager();
        jobs = jobManager.GetAllJobs();

        lvJobControl.Items.Clear();
        tvJobControl.Nodes.Clear();

        ListViewGroup lvJobCalculating = new ListViewGroup("calculating", HorizontalAlignment.Left);
        ListViewGroup lvJobFinished = new ListViewGroup("finished", HorizontalAlignment.Left);
        ListViewGroup lvJobPending = new ListViewGroup("pending", HorizontalAlignment.Left);
        tvJobControl.Nodes.Add("calculating");
        tvJobControl.Nodes.Add("finished");
        tvJobControl.Nodes.Add("pending");
        foreach (Job job in jobs.List) {
          if (job.State == State.calculating) {
            tvJobControl.Nodes[0].Nodes.Add(job.Id.ToString());
            lvJobControl.Items.Add(new ListViewItem(job.Id.ToString(), 0, lvJobCalculating));
          } else if (job.State == State.finished) {
            tvJobControl.Nodes[1].Nodes.Add(job.Id.ToString());
            lvJobControl.Items.Add(new ListViewItem(job.Id.ToString(), 0, lvJobFinished));
          } else if (job.State == State.offline) {
            tvJobControl.Nodes[2].Nodes.Add(job.Id.ToString());
            lvJobControl.Items.Add(new ListViewItem(job.Id.ToString(), 0, lvJobPending));
          }
        } // Jobs
        lvJobControl.Groups.Add(lvJobCalculating);
        lvJobControl.Groups.Add(lvJobFinished);
        lvJobControl.Groups.Add(lvJobPending);
      }
      catch (Exception ex) {
        closeFormEvent(true, true);
        this.Close();
      }
    }

    private void AddUsers() {
      try {
        IUserRoleManager userRoleManager =
          ServiceLocator.GetUserRoleManager();

        userGroups = userRoleManager.GetAllUserGroups();

        lvUserControl.Items.Clear();
        tvUserControl.Nodes.Clear();

        foreach (UserGroup ug in userGroups.List) {
          tvUserControl.Nodes.Add(ug.Name);
          ListViewGroup lvg = new ListViewGroup(ug.Name, HorizontalAlignment.Left);

          foreach (PermissionOwner permOwner in ug.Members) {
            if (permOwner is User) {
              User users = permOwner as User;
              tvUserControl.Nodes[tvUserControl.Nodes.Count - 1].Nodes.Add(users.Name);
              lvUserControl.Items.Add(new ListViewItem(users.Name, 0, lvg));
            }
          }
          lvUserControl.Groups.Add(lvg);

        } // Users
        usersList = userRoleManager.GetAllUsers();
        ListViewGroup lvunsorted = new ListViewGroup("unsorted", HorizontalAlignment.Left);
        foreach (User u in usersList.List) {
          tvUserControl.Nodes.Add(u.Name);
          lvUserControl.Items.Add(new ListViewItem(u.Name, 0, lvunsorted));
        }
        lvUserControl.Groups.Add(lvunsorted);
      }
      catch (Exception ex) {
        closeFormEvent(true, true);
        this.Close();
      }
    }

    /// <summary>
    /// Send event to Login-GUI when closing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Close_Click(object sender, EventArgs e) {
      if (closeFormEvent != null) {
        closeFormEvent(true, false);
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
        closeFormEvent(true, false);
      }
    }

    private void JobToolStripMenuItem1_Click(object sender, EventArgs e) {
      AddJobForm newForm = new AddJobForm();
      newForm.Show();
    }

    private void UserToolStripMenuItem1_Click(object sender, EventArgs e) {
      AddUserForm newForm = new AddUserForm("User", false);
      newForm.Show();
    }

    private void GroupToolStripMenuItem2_Click(object sender, EventArgs e) {
      AddUserForm newForm = new AddUserForm("User", true);
      newForm.Show();

    }

    private void OnClientClicked(object sender, EventArgs e) {
      scClientControl.Panel2.Controls.Clear();
      scClientControl.Panel2.Controls.Add(plClientDetails);
      pbClientControl.Image = ilClientControl.Images[0];
      lblClientName.Text = sender.ToString();

      
    }

    private void btnClose_Click(object sender, EventArgs e) {
      scClientControl.Panel2.Controls.Clear();
      scClientControl.Panel2.Controls.Add(lvClientControl);
    }



  }
}