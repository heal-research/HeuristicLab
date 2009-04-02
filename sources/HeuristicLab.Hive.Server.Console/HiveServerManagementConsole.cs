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

    private Dictionary<Guid, ListViewGroup> clientObjects;
    private Dictionary<Guid, ListViewItem> clientInfoObjects;
    private Dictionary<Guid, ListViewItem> jobObjects;

    private Job currentJob = null;
    private ClientInfo currentClient = null;
    private string nameCurrentJob = "";
    private string nameCurrentClient = "";
    private string nameCurrentUser = "";
    private bool flagJob = false;
    private bool flagClient = false;
    private bool flagUser = false;

    private List<Changes> changes = new List<Changes>();

    private ToolTip tt = new ToolTip();
    #endregion

    public HiveServerManagementConsole() {
      InitializeComponent();
      AddClients();
      AddJobs();
      timerSyncronize.Start();
    }

    /// <summary>
    /// event on Ticker
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void TickSync(object obj, EventArgs e) {
      if (!updaterWoker.IsBusy) {
        updaterWoker.RunWorkerAsync();
      }
    }

    /// <summary>
    /// Adds clients to ListView and TreeView
    /// </summary>
    private void AddClients() {
      try {
        clientObjects = new Dictionary<Guid, ListViewGroup>();
        clientInfoObjects = new Dictionary<Guid, ListViewItem>();
        IClientManager clientManager =
          ServiceLocator.GetClientManager();

        clients = clientManager.GetAllClientGroups();
        lvClientControl.Items.Clear();
        tvClientControl.Nodes.Clear();
        int count = 0;
        List<Guid> inGroup = new List<Guid>();
        foreach (ClientGroup cg in clients.List) {
          tvClientControl.Nodes.Add(cg.Name);
          ListViewGroup lvg = new ListViewGroup(cg.Name, HorizontalAlignment.Left);
          foreach (ClientInfo ci in cg.Resources) {
            tvClientControl.Nodes[tvClientControl.Nodes.Count - 1].Nodes.Add(ci.Name);
            ListViewItem item = null;
            if ((ci.State == State.offline) || (ci.State == State.nullState)) {
              item = new ListViewItem(ci.Name, 3, lvg);
            } else {
              item = new ListViewItem(ci.Name, count, lvg);
            }
            item.Tag = ci.Id;
            lvClientControl.Items.Add(item);
            clientInfoObjects.Add(ci.Id, item);
            count = (count + 1) % 3;
            inGroup.Add(ci.Id);
            
          }
          lvClientControl.BeginUpdate();
          lvClientControl.Groups.Add(lvg);
          lvClientControl.EndUpdate();
          clientObjects.Add(cg.Id, lvg);
        } // Groups

        clientInfo = clientManager.GetAllClients();
        ListViewGroup lvunsorted = new ListViewGroup("no group", HorizontalAlignment.Left);
        foreach (ClientInfo ci in clientInfo.List) {
          bool help = false;
          foreach (Guid client in inGroup) {
            if (client == ci.Id) {
              help = true;
              break;
            }
          }
          if (!help) {
            tvClientControl.Nodes.Add(ci.Name);
            ListViewItem item = null;
            if ((ci.State == State.offline) || (ci.State == State.nullState)) {
              item = new ListViewItem(ci.Name, 3, lvunsorted);
            } else {
              item = new ListViewItem(ci.Name, count, lvunsorted);
            }
            item.Tag = ci.Id;
            lvClientControl.Items.Add(item);
            count = (count + 1) % 3;
          }
        }
        lvClientControl.BeginUpdate();
        lvClientControl.Groups.Add(lvunsorted);
        lvClientControl.EndUpdate();
        if (flagClient) {
          ClientClicked();
        }
      }
      catch (Exception ex) {
        closeFormEvent(true, true);
        this.Close();
      }
    }


    List<ListViewGroup> jobGroup;
    /// <summary>
    /// Adds jobs to ListView and TreeView
    /// </summary>
    private void AddJobs() {
      try {
        jobObjects = new Dictionary<Guid, ListViewItem>();
        IJobManager jobManager =
          ServiceLocator.GetJobManager();
        jobs = jobManager.GetAllJobs();

        lvJobControl.Items.Clear();
        tvJobControl.Nodes.Clear();

        ListViewGroup lvJobCalculating = new ListViewGroup("calculating", HorizontalAlignment.Left);
        ListViewGroup lvJobFinished = new ListViewGroup("finished", HorizontalAlignment.Left);
        ListViewGroup lvJobPending = new ListViewGroup("pending", HorizontalAlignment.Left);

        jobGroup = new List<ListViewGroup>();
        jobGroup.Add(lvJobCalculating);
        jobGroup.Add(lvJobFinished);
        jobGroup.Add(lvJobPending);
        
        tvJobControl.Nodes.Add("calculating");
        tvJobControl.Nodes.Add("finished");
        tvJobControl.Nodes.Add("pending");
        foreach (Job job in jobs.List) {
          if (job.State == State.calculating) {
            ListViewItem lvi = new ListViewItem(job.Id.ToString(), 0, lvJobCalculating);
            jobObjects.Add(job.Id, lvi);
            tvJobControl.Nodes[0].Nodes.Add(job.Id.ToString());

            //lvJobControl.Items.Add(lvi);
            
            lvi.ToolTipText = (job.Percentage * 100) + "% of job calculated";
          } else if (job.State == State.finished) {
            ListViewItem lvi = new ListViewItem(job.Id.ToString(), 0, lvJobFinished);
            jobObjects.Add(job.Id, lvi);
            tvJobControl.Nodes[1].Nodes.Add(job.Id.ToString());
            //lvJobControl.Items.Add(lvi);
          } else if (job.State == State.offline) {
            ListViewItem lvi = new ListViewItem(job.Id.ToString(), 0, lvJobPending);
            jobObjects.Add(job.Id, lvi);
            tvJobControl.Nodes[2].Nodes.Add(job.Id.ToString());
            //lvJobControl.Items.Add(lvi);
          }
        } // Jobs
        lvJobControl.BeginUpdate();
        foreach (ListViewItem lvi in jobObjects.Values) {
          lvJobControl.Items.Add(lvi);
        }
        lvJobControl.Groups.Add(lvJobCalculating);
        lvJobControl.Groups.Add(lvJobFinished);
        lvJobControl.Groups.Add(lvJobPending);
        lvJobControl.EndUpdate();
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
    /// if one client is clicked, a panel is opened with the details
    /// </summary>
    private void ClientClicked() {
      int i = 0;
      while (clientInfo.List[i].Name != nameCurrentClient) {
        i++;
      }
      currentClient = clientInfo.List[i];
      scClientControl.Panel2.Controls.Clear();
      scClientControl.Panel2.Controls.Add(plClientDetails);
      pbClientControl.Image = ilClientControl.Images[0];
      lblClientName.Text = currentClient.Name;
      lblLogin.Text = currentClient.Login.ToString();
      lblState.Text = currentClient.State.ToString();
    }

    /// <summary>
    /// if one job is clicked, a panel is opened with the details
    /// </summary>
    private void JobClicked() {
      int i = 0;
      while (jobs.List[i].Id.ToString() != nameCurrentJob) {
        i++;
      }
      lvSnapshots.Enabled = false;
      int yPos = 0;
      currentJob = jobs.List[i];
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
      lblJobCreated.Text = "Created at " + currentJob.DateCreated;
      if (currentJob.ParentJob != null) {
        yPos += 20;
        lblParentJob.Location = new Point(
          lblParentJob.Location.X, yPos);
        lblParentJob.Text = currentJob.ParentJob.Id + " is parent job";
      }
      yPos += 20;
      lblPriorityJob.Location = new Point(
        lblPriorityJob.Location.X, yPos);
      lblPriorityJob.Text = "Priority of job is " + currentJob.Priority;
      if (currentJob.Client != null) {
        yPos += 20;
        lblClientCalculating.Location = new Point(
          lblClientCalculating.Location.X, yPos);
        lblClientCalculating.Text = currentJob.Client.Name + " calculated Job";
        yPos += 20;
        lblJobCalculationBegin.Location = new Point(
          lblJobCalculationBegin.Location.X, yPos);
        lblJobCalculationBegin.Text = "Startet calculation at " + currentJob.DateCalculated ;
        
        if (currentJob.State == State.finished) {
          yPos += 20;
          lblJobCalculationEnd.Location = new Point(
            lblJobCalculationEnd.Location.X, yPos);

          IJobManager jobManager =
            ServiceLocator.GetJobManager();

          ResponseObject<JobResult> jobRes = jobManager.GetLastJobResultOf(currentJob.Id);

          
          lblJobCalculationEnd.Text = "Calculation ended at " + jobRes.Obj.DateFinished;
        }
      }                       
      if (currentJob.State != State.offline) {
        yPos += 20;
        lvSnapshots.Location = new Point(
          lvSnapshots.Location.X, yPos);
        lvSnapshots.Size = new Size(lvSnapshots.Size.Width,
          plJobDetails.Size.Height - 10 - yPos);
        lvSnapshots.Enabled = true;
      }
    }

    private void Refresh() {
      foreach (Changes change in changes) {
        if (change.Types == Type.Job) {
          RefreshJob(change);
        } else if (change.Types == Type.Client) {
          RefreshClient(change);
        } else if (change.Types == Type.ClientGroup) {
          RefreshClientGroup(change);
        }
      }
    }

    private void RefreshJob(Changes change) {
      if (change.ChangeType == Change.Update) {
        for (int i = 0; i < lvJobControl.Items.Count; i++) {
          if (lvJobControl.Items[i].Text == change.ID.ToString()) {
            State state = jobs.List[change.Position].State;
            System.Diagnostics.Debug.WriteLine(lvJobControl.Items[i].Text.ToString());
            if (state == State.finished) {
              lvJobControl.Items[i].Group = jobGroup[1];
              System.Diagnostics.Debug.WriteLine("finished");
            } else if (state == State.calculating) {
              lvJobControl.Items[i].Group = jobGroup[0];
              System.Diagnostics.Debug.WriteLine("calculating");
            } else if (state == State.offline) {
              lvJobControl.Items[i].Group = jobGroup[2];
              System.Diagnostics.Debug.WriteLine("offline");

            }
            lvJobControl.Refresh();
          }
        }
      } else if (change.ChangeType == Change.Create) {
        ListViewItem lvi = new ListViewItem(
          change.ID.ToString(), 0, jobGroup[2]);
        jobObjects.Add(change.ID, lvi);
        lvJobControl.Items.Add(lvi);

      } else if (change.ChangeType == Change.Delete) {
        jobObjects.Remove(change.ID);
        for (int i = 0; i < lvJobControl.Items.Count; i++) {
          if (change.ID.ToString() == lvJobControl.Items[i].Text.ToString()) {
            lvJobControl.Items[i].Remove();
            break;
          }
        }
      }
    }

    private void RefreshClient(Changes change) {
      if (change.ChangeType == Change.Update) {
        for (int i = 0; i < lvClientControl.Items.Count; i++) {
          if (lvClientControl.Items[i].Tag.ToString() == change.ID.ToString()) {
            State state = clientInfo.List[change.Position].State;
            System.Diagnostics.Debug.WriteLine(lvClientControl.Items[i].Text.ToString());
            if ((state == State.offline) || (state == State.nullState)) {
              lvClientControl.Items[i].ImageIndex = 3;
            } else {
              lvClientControl.Items[i].ImageIndex = 1;
            }
            lvClientControl.Refresh();
          }
        }


      } else if (change.ChangeType == Change.Create) {
        
      } else if (change.ChangeType == Change.Delete) {
        clientInfoObjects.Remove(change.ID);
        for (int i = 0; i < lvClientControl.Items.Count; i++) {
          if (change.ID.ToString() == lvClientControl.Items[i].Text.ToString()) {
            lvClientControl.Items[i].Remove();
            break;
          }
        }

      }
    }

    private void RefreshClientGroup(Changes change) {

    }

    #region Eventhandlers
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
      //newForm.addJobEvent += new addDelegate(updaterWoker.RunWorkerAsync);
    }

    private void OnLVClientClicked(object sender, EventArgs e) {
      nameCurrentClient = lvClientControl.SelectedItems[0].Text;
      flagClient = true;
      ClientClicked();
    }

    private void OnTVClientClicked(object sender, TreeViewEventArgs e) {
      bool clientgroup = false;
      foreach (ClientGroup cg in clients.List) {
        if (tvClientControl.SelectedNode.Text == cg.Name) {
          clientgroup = true;
        }
      }
      if (clientgroup == false) {
        nameCurrentClient = tvClientControl.SelectedNode.Text;
        flagClient = true;
        ClientClicked();
      }

    }

    private void OnLVJobControlClicked(object sender, EventArgs e) {
      nameCurrentJob = lvJobControl.SelectedItems[0].Text;
      flagJob = true;
      JobClicked();
    }

    private void OnTVJobControlClicked(object sender, TreeViewEventArgs e) {
      try {
        nameCurrentJob = Convert.ToInt32(tvJobControl.SelectedNode.Text).ToString();
        flagJob = true;
        JobClicked();
      }
      catch (Exception ex) { }

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

    private void lvJobControl_MouseMove(object sender, MouseEventArgs e) {
      if ((lvJobControl.GetItemAt(e.X, e.Y) != null) &&
        (lvJobControl.GetItemAt(e.X, e.Y).ToolTipText != null)) {
        tt.SetToolTip(lvJobControl, lvJobControl.GetItemAt(e.X, e.Y).ToolTipText);
      }
    }

    private void updaterWoker_DoWork(object sender, DoWorkEventArgs e) {

        changes.Clear();
      IClientManager clientManager =
          ServiceLocator.GetClientManager();
      
      #region ClientInfo
      ResponseList<ClientInfo> clientInfoOld = clientInfo;
      clientInfo = clientManager.GetAllClients();

      IDictionary<int, ClientInfo> clientInfoOldHelp;

      CloneList(clientInfoOld, out clientInfoOldHelp);

      GetDelta(clientInfoOld.List, clientInfoOldHelp);
      #endregion

      #region Clients
      ResponseList<ClientGroup> clientsOld = clients;
     
      clients = clientManager.GetAllClientGroups();

      IDictionary<int, ClientGroup> clientsOldHelp;

      CloneList(clientsOld, out clientsOldHelp);

      GetDelta(clientsOld.List, clientsOldHelp);
      #endregion

      #region Job
      ResponseList<Job> jobsOld = jobs;
      IJobManager jobManager =
          ServiceLocator.GetJobManager();

      jobs = jobManager.GetAllJobs();

      IDictionary<int, Job> jobsOldHelp;
      CloneList(jobsOld, out jobsOldHelp);

      GetDelta(jobsOld.List, jobsOldHelp);

      #endregion

      foreach (Changes change in changes) {
        System.Diagnostics.Debug.WriteLine(change.ID + " " + change.ChangeType);
      }

    }
    #endregion

    #region Helper methods

    private void CloneList(ResponseList<Job> oldList, out IDictionary<int, Job> newList) {
      newList = new Dictionary<int, Job>();
      for (int i = 0; i < oldList.List.Count; i++) {
        newList.Add(i, oldList.List[i]);
      }
    }

    private void CloneList(ResponseList<ClientInfo> oldList, out IDictionary<int, ClientInfo> newList) {
      newList = new Dictionary<int, ClientInfo>();
      for (int i = 0; i < oldList.List.Count; i ++) {
        newList.Add(i, oldList.List[i]);
      }
    }

    private void CloneList(ResponseList<ClientGroup> oldList, out IDictionary<int, ClientGroup> newList) {
      newList = new Dictionary<int, ClientGroup>();
      for (int i = 0; i < oldList.List.Count; i++) {
        newList.Add(i, oldList.List[i]);
      }
    }

    private bool IsEqual(ClientInfo ci1, ClientInfo ci2) {
      if (ci2 == null) {
        return false;
      }
      if (ci1.Id.Equals(ci2.Id)) {
        return true;
      } else return false;
    }

    private void GetDelta(IList<ClientInfo> oldClient, IDictionary<int, ClientInfo> helpClients) {
      bool found = false;

      for (int i = 0; i < clientInfo.List.Count; i ++) {
        ClientInfo ci = clientInfo.List[i];
        for (int j = 0; j < oldClient.Count; j++) {
          ClientInfo cio = oldClient[j];
          if (ci.Id.Equals(cio.Id)) {
            found = true;
            if (ci.State != cio.State) {
              changes.Add(new Changes { Types = Type.Client, ID = ci.Id, ChangeType = Change.Update, Position = i });
            }
            int removeAt = -1;
            foreach (KeyValuePair<int, ClientInfo> kvp in helpClients) {
              if (cio.Id.Equals(kvp.Value.Id)) {
                removeAt = kvp.Key;
                break;
              }
            }
            if (removeAt >= 0) {
              helpClients.Remove(removeAt);
            }
            break;
          }
        }
        if (found == false) {
          changes.Add(new Changes { Types = Type.Client, ID = ci.Id, ChangeType = Change.Create });
        }
        found = false;
      }
      foreach (KeyValuePair<int, ClientInfo> kvp in helpClients) {
        changes.Add(new Changes { Types = Type.Client, ID = kvp.Value.Id, ChangeType = Change.Delete, Position = kvp.Key });
      }

    }

    private void GetDelta(IList<ClientGroup> oldClient, IDictionary<int, ClientGroup> helpClients) {

      bool found = false;
      for (int i = 0; i < clients.List.Count; i++) {
        ClientGroup cg = clients.List[i];
        for (int j = 0; j < oldClient.Count; i++) {
          ClientGroup cgo = oldClient[j];
          if (cg.Id.Equals(cgo.Id)) {
            found = true;
            foreach (Resource resource in cg.Resources) {
              foreach (Resource resourceold in cgo.Resources) {
                if (resource.Id.Equals(resourceold.Id)) {
                  if (resourceold.Name != resource.Name) {
                    changes.Add(new Changes { Types = Type.Client, ID = cg.Id, ChangeType = Change.Update, Position = i });
                  }
                }
              }
            }
            for (int k = 0; k < helpClients.Count; k++) {
              if (cgo.Id.Equals(helpClients[k].Id)) {
                helpClients.Remove(k);
                break;
              }
            }
            break;
          }
        }
        if (found == false) {
          changes.Add(new Changes { Types = Type.ClientGroup, ID = cg.Id, ChangeType = Change.Create });
        }
        found = false;
      }
      foreach (KeyValuePair<int, ClientGroup> kvp in helpClients) {
        changes.Add(new Changes { Types = Type.ClientGroup, ID = kvp.Value.Id, ChangeType = Change.Delete, Position = kvp.Key });
      }
    }

    private void GetDelta(IList<Job> oldJobs, IDictionary<int, Job> helpJobs) {
      bool found = false;
      for (int i = 0; i < jobs.List.Count; i ++ ) {
        Job job = jobs.List[i];
        for (int j = 0; j < oldJobs.Count; j++) {

          Job jobold = oldJobs[j];

          if (job.Id.Equals(jobold.Id)) {

            found = true;
            if (job.State != State.offline) {
              if (!IsEqual(job.Client, jobold.Client)) {
                changes.Add(new Changes { Types = Type.Job, ID = job.Id, ChangeType = Change.Update, Position = i });
              } else if (job.State != jobold.State) {
                changes.Add(new Changes { Types = Type.Job, ID = job.Id, ChangeType = Change.Update, Position = i });
              }
            } else if (job.DateCalculated != jobold.DateCalculated) {
              changes.Add(new Changes { Types = Type.Job, ID = job.Id, ChangeType = Change.Update, Position = i });
            }

            int removeAt = -1;
            foreach (KeyValuePair<int, Job> kvp in helpJobs) {
              if (job.Id.Equals(kvp.Value.Id)) {
                removeAt = kvp.Key;
                break;
              }
            }
            if (removeAt >= 0) {
              helpJobs.Remove(removeAt);
            }
            break;
          }

        }
        if (found == false) {
          changes.Add(new Changes { Types = Type.Job, ID = job.Id, ChangeType = Change.Create });
          System.Diagnostics.Debug.WriteLine("new Job: " + job.Id);
        }
        found = false;
      }
      foreach (KeyValuePair<int, Job> kvp in helpJobs) {
        changes.Add(new Changes { Types = Type.Job, ID = kvp.Value.Id, ChangeType = Change.Delete, Position = kvp.Key });
        System.Diagnostics.Debug.WriteLine("delete Job: " + kvp.Value.Id);
      }
    }


    #endregion

    private void updaterWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      Refresh();
    }


  }
}