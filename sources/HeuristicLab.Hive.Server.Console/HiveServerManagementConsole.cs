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

    #region private variables
    private ResponseList<ClientGroup> clients = null;
    private ResponseList<ClientInfo> clientInfo = null;
    private ResponseList<Job> jobs = null;
    private ResponseList<UserGroup> userGroups = null;
    private ResponseList<User> usersList = null;

    private Job currentJob = null;
    private ClientInfo currentClient = null;
    private User currentUser = null;
    private int idxCurrentJob = 0;
    private int idxCurrentClient = 0;
    private int idxCurrentUser = 0;
    private bool flagJob = false;
    private bool flagClient = false;
    private bool flagUser = false;

    private ToolTip tt = new ToolTip();
    #endregion

    public HiveServerManagementConsole() {
      InitializeComponent();
      AddClients();
      AddJobs();
      AddUsers();
      timerSyncronize.Start();
    }

    /// <summary>
    /// event on Ticker
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void TickSync(object obj, EventArgs e) {
      AddClients();
      AddJobs();
      AddUsers();
    }

    /// <summary>
    /// Adds clients to ListView and TreeView
    /// </summary>
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
        if (flagClient) {
          ClientClicked();
        }
      }
      catch (Exception ex) {
        closeFormEvent(true, true);
        this.Close();
      }
    }

    /// <summary>
    /// Adds jobs to ListView and TreeView
    /// </summary>
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
            ListViewItem lvi = new ListViewItem(job.Id.ToString(), 0, lvJobCalculating);
            tvJobControl.Nodes[0].Nodes.Add(job.Id.ToString());
            lvJobControl.Items.Add(lvi);
            lvi.ToolTipText = (job.Percentage * 100) + "% of job calculated";
          } else if (job.State == State.finished) {
            ListViewItem lvi = new ListViewItem(job.Id.ToString(), 0, lvJobFinished);
            tvJobControl.Nodes[1].Nodes.Add(job.Id.ToString());
            lvJobControl.Items.Add(lvi);
          } else if (job.State == State.offline) {
            ListViewItem lvi = new ListViewItem(job.Id.ToString(), 0, lvJobPending);
            tvJobControl.Nodes[2].Nodes.Add(job.Id.ToString());
            lvJobControl.Items.Add(lvi);
          }
        } // Jobs
        lvJobControl.Groups.Add(lvJobCalculating);
        lvJobControl.Groups.Add(lvJobFinished);
        lvJobControl.Groups.Add(lvJobPending);
        if (flagJob) {
          JobClicked();
        }
      }
      catch (Exception ex) {
        closeFormEvent(true, true);
        this.Close();
      }
    }

    /// <summary>
    /// Adds users to ListView and TreeView
    /// </summary>
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
        if (flagUser) {
          UserClicked();
        }
      }
      catch (Exception ex) {
        closeFormEvent(true, true);
        this.Close();
      }
    }

    /// <summary>
    /// if one client is clicked, a panel is opened with the details
    /// </summary>
    private void ClientClicked() {
      currentClient = clientInfo.List[idxCurrentClient];
      scClientControl.Panel2.Controls.Clear();
      scClientControl.Panel2.Controls.Add(plClientDetails);
      pbClientControl.Image = ilClientControl.Images[0];
      lblClientName.Text = currentClient.Name;
      lblLogin.Text = currentClient.Login.ToString();
    }

    /// <summary>
    /// if one job is clicked, a panel is opened with the details
    /// </summary>
    private void JobClicked() {
      int yPos = 0;
      currentJob = jobs.List[idxCurrentJob];
      scJobControl.Panel2.Controls.Clear();
      scJobControl.Panel2.Controls.Add(plJobDetails);
      pbJobControl.Image = ilJobControl.Images[0];
      lblJobName.Text = currentJob.Id.ToString();
      progressJob.Value = (int)(currentJob.Percentage * 100);
      yPos = progressJob.Location.Y;
      yPos += 20;
      lblProgress.Location = new Point(
        lblProgress.Location.X, yPos);
      lblProgress.Text = (int)(currentJob.Percentage * 100) + "% calculated";
      yPos += 20;
      lblUserCreatedJob.Location = new Point(
        lblUserCreatedJob.Location.X, yPos);
      lblUserCreatedJob.Text = /* currentJob.User.Name + */ " created Job";
      yPos += 20;
      lblJobCreated.Location = new Point(
        lblJobCreated.Location.X, yPos);
      lblJobCreated.Text = "Created at "/* + currentJob.User.CreatedJob + */;
      if (currentJob.ParentJob != null) {
        yPos += 20;
        lblParentJob.Location = new Point(
          lblParentJob.Location.X, yPos);
        lblParentJob.Text = currentJob.ParentJob.Id + " is parent job";
      }
      yPos += 20;
      lblPriorityJob.Location = new Point(
        lblPriorityJob.Location.X, yPos);
      lblPriorityJob.Text = "Priority of job is " /* + currentJob.Priority */;
      if (currentJob.Client != null) {
        yPos += 20;
        lblClientCalculating.Location = new Point(
          lblClientCalculating.Location.X, yPos);
        lblClientCalculating.Text = currentJob.Client.Name + " calculated Job";
        yPos += 20;
        lblJobCalculationBegin.Location = new Point(
          lblJobCalculationBegin.Location.X, yPos);
        lblJobCalculationBegin.Text = "Startet calculation at " /* + currentJob.User.CalculationBegin */;
        yPos += 20;
        lblJobCalculationEnd.Location = new Point(
          lblJobCalculationEnd.Location.X, yPos);
        lblJobCalculationEnd.Text = "Calculation endet at " /* + currentJob.User.CalculationEnd */;
      }
    }

    /// <summary>
    /// if one user is clicked, a panel is opened with the details
    /// </summary>
    private void UserClicked() {
      currentUser = usersList.List[idxCurrentUser];
      scUserControl.Panel2.Controls.Clear();
      scUserControl.Panel2.Controls.Add(plUserDetails);
      pbUserControl.Image = ilUserControl.Images[0];
      lblUserName.Text = currentUser.Id.ToString();
    }

    #region Eventhandler
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

    private void AddJob_Click(object sender, EventArgs e) {
      AddJobForm newForm = new AddJobForm();
      newForm.Show();
    }

    private void AddUser_Click(object sender, EventArgs e) {
      AddUserForm newForm = new AddUserForm("User", false);
      newForm.Show();
    }

    private void AddUserGroup_Click(object sender, EventArgs e) {
      AddUserForm newForm = new AddUserForm("User", true);
      newForm.Show();
    }

    private void OnLVClientClicked(object sender, EventArgs e) {
      idxCurrentClient = lvClientControl.SelectedItems[0].Index;
      flagClient = true;
      ClientClicked();
    }

    private void OnLVJobControlClicked(object sender, EventArgs e) {
      idxCurrentJob = lvJobControl.SelectedItems[0].Index;
      flagJob = true;
      JobClicked();
    }

    private void OnLVUserControlClicked(object sender, EventArgs e) {
      idxCurrentUser = lvUserControl.SelectedItems[0].Index;
      flagUser = true;
      UserClicked();
    }

    private void btnClientClose_Click(object sender, EventArgs e) {
      scClientControl.Panel2.Controls.Clear();
      scClientControl.Panel2.Controls.Add(lvClientControl);
      flagClient = false;
    }
 
    private void btnJobDetailClose_Click(object sender, EventArgs e) {
      scJobControl.Panel2.Controls.Clear();
      scJobControl.Panel2.Controls.Add(lvJobControl);
      flagJob = false;
    }

    private void btnUserControlClose_Click(object sender, EventArgs e) {
      scUserControl.Panel2.Controls.Clear();
      scUserControl.Panel2.Controls.Add(lvUserControl);
      flagUser = false;
    }

    private void lvJobControl_MouseMove(object sender, MouseEventArgs e) {
      if ((lvJobControl.GetItemAt(e.X, e.Y) != null) &&
        (lvJobControl.GetItemAt(e.X, e.Y).ToolTipText != null)) {
        tt.SetToolTip(lvJobControl, lvJobControl.GetItemAt(e.X, e.Y).ToolTipText);
      }
    }
    #endregion
  }
}