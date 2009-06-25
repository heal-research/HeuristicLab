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
using System.Threading;
using System.ServiceModel;

namespace HeuristicLab.Hive.Server.ServerConsole {

  /// <summary>
  /// if form is closed the loginform gets an information
  /// </summary>
  /// <param name="cf"></param>
  /// <param name="error"></param>
  public delegate void closeForm(bool cf, bool error);

  public partial class HiveServerManagementConsole : Form {

    public event closeForm closeFormEvent;

    #region private variables
    private ResponseList<Job> jobs = null;

    List<ListViewGroup> jobGroup;
    private Dictionary<Guid, List<ListViewItem>> clientList = new Dictionary<Guid, List<ListViewItem>>();
    private List<Changes> changes = new List<Changes>();

    private Job currentJob = null;
    private ClientInfo currentClient = null;

    private TreeNode currentGroupNode = null;
    Guid parentgroup = Guid.Empty;
    private ToolTip tt = new ToolTip();

    private const string NOGROUP = "No group";

    #endregion

    #region Properties
    private IClientManager ClientManager {
      get {
        try {
          return ServiceLocator.GetClientManager();
        }
        catch (FaultException ex) {
          MessageBox.Show(ex.Message);
        }
        return null;
      }
    }
    #endregion

    public HiveServerManagementConsole() {
      InitializeComponent();
      Init();
      AddClients();
      AddJobs();
      timerSyncronize.Start();
    }

    private TreeNode hoverNode; // node being hovered over during DnD
    private void Init() {

      //adding context menu items for jobs
      menuItemAbortJob.Click += (s, e) => {
        IJobManager jobManager = ServiceLocator.GetJobManager();
        if (lvJobControl.SelectedItems.Count == 1) {
          jobManager.AbortJob(((Job)(lvJobControl.SelectedItems[0].Tag)).Id);
        }
      };

      //adding context menu items for jobs
      menuItemGetSnapshot.Click += (s, e) => {
        IJobManager jobManager = ServiceLocator.GetJobManager();
        if (lvJobControl.SelectedItems.Count == 1) {
          jobManager.RequestSnapshot(((Job)(lvJobControl.SelectedItems[0].Tag)).Id);
        }
      };

      //adding group
      menuItemAddGroup.Click += (s, e) => {
        AddGroup addgroup = new AddGroup();
        parentgroup = Guid.Empty;
        if ((tvClientControl.SelectedNode != null) && (((ClientGroup)tvClientControl.SelectedNode.Tag).Id != Guid.Empty)) {
          parentgroup = ((ClientGroup)tvClientControl.SelectedNode.Tag).Id;
        }
        addgroup.addGroupEvent += new AddGroupDelegate(addgroup_addGroupEvent);
        addgroup.Show();
      };

      //adding group
      menuItemDeleteGroup.Click += (s, e) => {
        IClientManager clientManager = ServiceLocator.GetClientManager();
        if (tvClientControl.SelectedNode != null) {
          Response resp = clientManager.DeleteClientGroup(((ClientGroup)tvClientControl.SelectedNode.Tag).Id);
          if (tvClientControl.SelectedNode == currentGroupNode) {
            currentGroupNode = null;
          }
          tvClientControl.Nodes.Remove(tvClientControl.SelectedNode);
          AddClients();
        }
      };

      lvClientControl.ItemDrag += delegate(object sender, ItemDragEventArgs e) {
        List<string> itemIDs = new List<string>((sender as ListView).SelectedItems.Count);
        foreach (ListViewItem item in (sender as ListView).SelectedItems) {
          itemIDs.Add(item.Name);
        }
        (sender as ListView).DoDragDrop(itemIDs.ToArray(), DragDropEffects.Move);
      };

      tvClientControl.DragEnter += delegate(object sender, DragEventArgs e) {
        e.Effect = DragDropEffects.Move;
      };

      tvClientControl.DragOver += delegate(object sender, DragEventArgs e) {
        Point mouseLocation = tvClientControl.PointToClient(new Point(e.X, e.Y));
        TreeNode node = tvClientControl.GetNodeAt(mouseLocation);
        if (node != null && ((ClientGroup)node.Tag).Id != Guid.Empty) {
          e.Effect = DragDropEffects.Move;
          if (hoverNode == null) {
            node.BackColor = Color.LightBlue;
            node.ForeColor = Color.White;
            hoverNode = node;
          } else if (hoverNode != node) {
            hoverNode.BackColor = Color.White;
            hoverNode.ForeColor = Color.Black;
            node.BackColor = Color.LightBlue;
            node.ForeColor = Color.White;
            hoverNode = node;
          }
        } else {
          e.Effect = DragDropEffects.None;
        }
      };

      tvClientControl.DragDrop += delegate(object sender, DragEventArgs e) {
        if (e.Data.GetDataPresent(typeof(string[]))) {
          Point dropLocation = (sender as TreeView).PointToClient(new Point(e.X, e.Y));
          TreeNode dropNode = (sender as TreeView).GetNodeAt(dropLocation);
          if (((ClientGroup)dropNode.Tag).Id != Guid.Empty) {
            Dictionary<ClientInfo, Guid> clients = new Dictionary<ClientInfo, Guid>();
            foreach (ListViewItem lvi in lvClientControl.SelectedItems) {
              Guid groupId = Guid.Empty;
              foreach (ListViewGroup lvg in lvClientGroups) {
                if (lvi.Group.Header == ((ClientGroup)lvg.Tag).Name) {
                  groupId = ((ClientGroup)lvg.Tag).Id;
                }
              }
              clients.Add((ClientInfo)lvi.Tag, groupId);
            }
            ChangeGroup(clients, ((ClientGroup)dropNode.Tag).Id);
          }
          tvClientControl_DragLeave(null, EventArgs.Empty);
          AddClients();
        }
      };
    }

    private void ChangeGroup(Dictionary<ClientInfo, Guid> clients, Guid clientgroupID) {
      IClientManager clientManager = ServiceLocator.GetClientManager();
      foreach (KeyValuePair<ClientInfo, Guid> client in clients) {
        if (client.Key.Id != Guid.Empty) {
          Response resp = clientManager.DeleteResourceFromGroup(client.Value, client.Key.Id);
        }
        clientManager.AddResourceToGroup(clientgroupID, client.Key);
      }
    }

    #region Backgroundworker
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

    private void updaterWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      RefreshForm();
    }

    private void updaterWoker_DoWork(object sender, DoWorkEventArgs e) {

      changes.Clear();

      //#region ClientInfo
      //ResponseList<ClientInfo> clientInfoOld = clientInfo;
      //clientInfo = ClientManager.GetAllClients();

      //IDictionary<int, ClientInfo> clientInfoOldHelp;

      //CloneList(clientInfoOld, out clientInfoOldHelp);

      //GetDelta(clientInfoOld.List, clientInfoOldHelp);
      //#endregion

      #region Clients
      //ResponseList<ClientGroup> clientsOld = clients;

      // newClients = ClientManager.GetAllClientGroups();

      //IDictionary<Guid, ClientGroup> clientsOldHelp;

      //CloneList(clientsOld, out clientsOldHelp);

      //GetDelta(clientsOld.List, clientsOldHelp);
      //DetermineDelta();
      #endregion

      #region Job
      ResponseList<Job> jobsOld = jobs;
      try {
        IJobManager jobManager =
                ServiceLocator.GetJobManager();

        jobs = jobManager.GetAllJobs();

        IDictionary<int, Job> jobsOldHelp;
        CloneList(jobsOld, out jobsOldHelp);

        GetDelta(jobsOld.List, jobsOldHelp);

      }
      catch (FaultException fe) {
        MessageBox.Show(fe.Message);
      }

      #endregion

      foreach (Changes change in changes) {
        System.Diagnostics.Debug.WriteLine(change.ID + " " + change.ChangeType);
      }

    }

    #endregion


    /// <summary>
    /// Adds Exceptionobs to ListView and TreeView
    /// </summary>
    private void AddJobs() {
      try {
        List<ListViewItem> jobObjects = new List<ListViewItem>();
        IJobManager jobManager =
          ServiceLocator.GetJobManager();
        jobs = jobManager.GetAllJobs();

        lvJobControl.Items.Clear();

        ListViewGroup lvJobCalculating = new ListViewGroup("calculating", HorizontalAlignment.Left);
        ListViewGroup lvJobFinished = new ListViewGroup("finished", HorizontalAlignment.Left);
        ListViewGroup lvJobPending = new ListViewGroup("pending", HorizontalAlignment.Left);

        jobGroup = new List<ListViewGroup>();
        jobGroup.Add(lvJobCalculating);
        jobGroup.Add(lvJobFinished);
        jobGroup.Add(lvJobPending);

        foreach (Job job in jobs.List) {
          if (job.State == State.calculating) {
            ListViewItem lvi = new ListViewItem(job.Id.ToString(), 1, lvJobCalculating);
            lvi.Tag = job;
            jobObjects.Add(lvi);

            lvi.ToolTipText = (job.Percentage * 100) + "% of job calculated";
          } else if (job.State == State.finished) {
            ListViewItem lvi = new ListViewItem(job.Id.ToString(), 0, lvJobFinished);
            lvi.Tag = job;
            jobObjects.Add(lvi);
            //lvJobControl.Items.Add(lvi);
          } else if (job.State == State.offline) {
            ListViewItem lvi = new ListViewItem(job.Id.ToString(), 2, lvJobPending);
            lvi.Tag = job;
            jobObjects.Add(lvi);
          }
        } // Jobs
        lvJobControl.BeginUpdate();
        foreach (ListViewItem lvi in jobObjects) {
          lvJobControl.Items.Add(lvi);
        }
        // actualization
        lvJobControl.Groups.Add(lvJobCalculating);
        lvJobControl.Groups.Add(lvJobFinished);
        lvJobControl.Groups.Add(lvJobPending);
        lvJobControl.EndUpdate();

        if (currentJob != null) {
          JobClicked();
        }
      }
      catch (Exception ex) {
        closeFormEvent(true, true);
        this.Close();
      }
    }

    List<ListViewGroup> lvClientGroups;
    private void AddClients() {
      lvClientGroups = new List<ListViewGroup>();
      clientList.Clear();
      tvClientControl.Nodes.Clear();
      try {
        ResponseList<ClientGroup> clientGroups = ClientManager.GetAllClientGroups();

        foreach (ClientGroup cg in clientGroups.List) {
          AddClientOrGroup(cg, null);
        }

        if (currentGroupNode != null) {
          lvClientControl.Items.Clear();
          lvClientControl.Groups.Clear();
          AddGroupsToListView(currentGroupNode);
        }
        tvClientControl.ExpandAll();

      }
      catch (FaultException fe) {
        MessageBox.Show(fe.Message);
      }
    }

    private void AddClientOrGroup(ClientGroup clientGroup, TreeNode currentNode) {
      currentNode = CreateTreeNode(clientGroup, currentNode);
      List<ListViewItem> clientGroupList = new List<ListViewItem>();
      ListViewGroup lvg;
      if (string.IsNullOrEmpty(clientGroup.Name)) {
        lvg = new ListViewGroup(NOGROUP, HorizontalAlignment.Left);
      } else {
        lvg = new ListViewGroup(clientGroup.Name, HorizontalAlignment.Left);
      }
      lvClientControl.Groups.Add(lvg);
      lvg.Tag = clientGroup;
      lvClientGroups.Add(lvg);
      foreach (Resource resource in clientGroup.Resources) {
        if (resource is ClientInfo) {
          int percentageUsage = CapacityRam(((ClientInfo)resource).NrOfCores, ((ClientInfo)resource).NrOfFreeCores);
          int usage = 3;
          if ((((ClientInfo)resource).State != State.offline) &&
            (((ClientInfo)resource).State != State.nullState)) {
            if ((percentageUsage >= 0) && (percentageUsage <= 25)) {
              usage = 0;
            } else if ((percentageUsage > 25) && (percentageUsage <= 75)) {
              usage = 1;
            } else if ((percentageUsage > 75) && (percentageUsage <= 100)) {
              usage = 2;
            }
          }
          ListViewItem lvi = new ListViewItem(resource.Name, usage, lvg);
          lvi.Tag = resource as ClientInfo;
          clientGroupList.Add(lvi);
        } else if (resource is ClientGroup) {
          AddClientOrGroup(resource as ClientGroup, currentNode);
        }
      }
      clientList.Add(clientGroup.Id, clientGroupList);
    }

    private TreeNode CreateTreeNode(ClientGroup clientGroup, TreeNode currentNode) {
      TreeNode tn;
      if (string.IsNullOrEmpty(clientGroup.Name)) {
        tn = new TreeNode(NOGROUP);
      } else {
        tn = new TreeNode(clientGroup.Name);
      }
      tn.Tag = clientGroup;
      if (currentNode == null) {
        tvClientControl.Nodes.Add(tn);
      } else {
        currentNode.Nodes.Add(tn);
      }
      return tn;
    }

    private void AddGroupsToListView(TreeNode node) {
      if (node != null) {
        ListViewGroup lvg = new ListViewGroup(node.Text, HorizontalAlignment.Left);
        lvClientControl.Groups.Add(lvg);
        lvg.Tag = node.Tag;
        foreach (ListViewItem item in clientList[((ClientGroup)node.Tag).Id]) {
          item.Group = lvg;
          lvClientControl.Items.Add(item);
        }

        if (node.Nodes != null) {
          foreach (TreeNode curNode in node.Nodes) {
            AddGroupsToListView(curNode);
          }
        }
      }
    }

    /// <summary>
    /// if one job is clicked, the details for the clicked job are shown
    /// in the second panel
    /// </summary>
    private void JobClicked() {
      plJobDetails.Visible = true;
      lvJobDetails.Items.Clear();

      lvSnapshots.Enabled = true;

      if (currentJob.State == State.offline) {
        pbJobControl.Image = ilLargeImgJob.Images[2];
      } else if (currentJob.State == State.calculating) {
        pbJobControl.Image = ilLargeImgJob.Images[1];
      } else if (currentJob.State == State.finished) {
        pbJobControl.Image = ilLargeImgJob.Images[0];
      }

      lblJobName.Text = currentJob.Id.ToString();
      progressJob.Value = (int)(currentJob.Percentage * 100);
      lblProgress.Text = (int)(currentJob.Percentage * 100) + "% calculated";

      ListViewItem lvi = new ListViewItem();
      lvi.Text = "User:";
      lvi.SubItems.Add(currentJob.UserId.ToString());
      lvJobDetails.Items.Add(lvi);

      lvi = null;
      lvi = new ListViewItem();
      lvi.Text = "created at:";
      lvi.SubItems.Add(currentJob.DateCreated.ToString());
      lvJobDetails.Items.Add(lvi);

      if (currentJob.ParentJob != null) {
        lvi = null;
        lvi = new ListViewItem();
        lvi.Text = "Parent job:";
        lvi.SubItems.Add(currentJob.ParentJob.ToString());
        lvJobDetails.Items.Add(lvi);
      }

      lvi = null;
      lvi = new ListViewItem();
      lvi.Text = "Priority:";
      lvi.SubItems.Add(currentJob.Priority.ToString());
      lvJobDetails.Items.Add(lvi);

      if (currentJob.Project != null) {
        lvi = null;
        lvi = new ListViewItem();
        lvi.Text = "Project:";
        lvi.SubItems.Add(currentJob.Project.Name.ToString());
        lvJobDetails.Items.Add(lvi);
      }

      if (currentJob.Client != null) {
        lvi = null;
        lvi = new ListViewItem();
        lvi.Text = "Calculation begin:";
        lvi.SubItems.Add(currentJob.DateCalculated.ToString());
        lvJobDetails.Items.Add(lvi);


        lvi = null;
        lvi = new ListViewItem();
        lvi.Text = "Client calculated:";
        lvi.SubItems.Add(currentJob.Client.Name.ToString());
        lvJobDetails.Items.Add(lvi);

        if (currentJob.State == State.finished) {
          IJobManager jobManager =
            ServiceLocator.GetJobManager();
          ResponseObject<JobResult> jobRes = jobManager.GetLastJobResultOf(currentJob.Id);

          lvi = null;
          lvi = new ListViewItem();
          lvi.Text = "Calculation ended:";
          lvi.SubItems.Add(jobRes.Obj.DateFinished.ToString());
          lvJobDetails.Items.Add(lvi);
        }
      }
      if (currentJob.State != State.offline) {
        lvSnapshots.Items.Clear();
        GetSnapshotList();
      } else {
        lvSnapshots.Visible = false;
      }
    }

    /// <summary>
    /// if one client is clicked, the details for the clicked client are shown
    /// in the second panel
    /// </summary>
    private void ClientClicked() {
      plClientDetails.Visible = true;

      if (currentClient != null) {
        int percentageUsage = CapacityRam(currentClient.NrOfCores, currentClient.NrOfFreeCores);
        int usage = 3;
        if ((currentClient.State != State.offline) && (currentClient.State != State.nullState)) {
          if ((percentageUsage >= 0) && (percentageUsage <= 25)) {
            usage = 0;
          } else if ((percentageUsage > 25) && (percentageUsage <= 75)) {
            usage = 1;
          } else if ((percentageUsage > 75) && (percentageUsage <= 100)) {
            usage = 2;
          }
        }
        pbClientControl.Image = ilLargeImgClient.Images[usage];
        lblClientName.Text = currentClient.Name;
        lblLogin.Text = currentClient.Login.ToString();
        lblState.Text = currentClient.State.ToString();
      }
    }


    private void RefreshForm() {
      foreach (Changes change in changes) {
        if (change.Types == Type.Job) {
          RefreshJob(change);
        }
      }
    }

    private void RefreshJob(Changes change) {
      if (change.ChangeType == Change.Update) {
        for (int i = 0; i < lvJobControl.Items.Count; i++) {
          if (lvJobControl.Items[i].Text == change.ID.ToString()) {
            foreach (Job job in jobs.List) {
              if (job.Id == change.ID) {
                lvJobControl.Items[i].Tag = job;
                if (currentJob.Id == change.ID) {
                  currentJob = job;
                  JobClicked();
                }
                break;
              }
            }
            State state = jobs.List[change.Position].State;
            System.Diagnostics.Debug.WriteLine(lvJobControl.Items[i].Text.ToString());
            if (state == State.finished) {
              lvJobControl.Items[i].Group = jobGroup[1];
              lvJobControl.Items[i].ImageIndex = 0;
              System.Diagnostics.Debug.WriteLine("finished");
            } else if (state == State.calculating) {
              lvJobControl.Items[i].Group = jobGroup[0];
              lvJobControl.Items[i].ImageIndex = 1;
              System.Diagnostics.Debug.WriteLine("calculating");
            } else if (state == State.offline) {
              lvJobControl.Items[i].Group = jobGroup[2];
              lvJobControl.Items[i].ImageIndex = 2;
              System.Diagnostics.Debug.WriteLine("offline");

            }
            lvJobControl.Refresh();
          }
        }
      } else if (change.ChangeType == Change.Create) {

        ListViewItem lvi = new ListViewItem(
          change.ID.ToString(), 2, jobGroup[2]);
        foreach (Job job in jobs.List) {
          if (job.Id == change.ID) {
            lvi.Tag = job;
            break;
          }
        }
        lvJobControl.Items.Add(lvi);

      } else if (change.ChangeType == Change.Delete) {
        for (int i = 0; i < lvJobControl.Items.Count; i++) {
          if (change.ID.ToString() == lvJobControl.Items[i].Text.ToString()) {
            lvJobControl.Items[i].Remove();
            break;
          }
        }
      }
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
      newForm.addJobEvent += new addDelegate(updaterWoker.RunWorkerAsync);
    }

    private void OnLVJobControlClicked(object sender, EventArgs e) {
      currentJob = (Job)lvJobControl.SelectedItems[0].Tag;
      JobClicked();
    }

    private void lvJobControl_MouseMove(object sender, MouseEventArgs e) {
      if ((lvJobControl.GetItemAt(e.X, e.Y) != null) &&
        (lvJobControl.GetItemAt(e.X, e.Y).ToolTipText != null)) {
        tt.SetToolTip(lvJobControl, lvJobControl.GetItemAt(e.X, e.Y).ToolTipText);
      }
    }

    private void lvJobControl_MouseUp(object sender, MouseEventArgs e) {
      // If the right mouse button was clicked and released,
      // display the shortcut menu assigned to the ListView. 
      lvJobControl.ContextMenuStrip.Items.Clear();
      ListViewHitTestInfo hitTestInfo = lvJobControl.HitTest(e.Location);
      if (e.Button == MouseButtons.Right && hitTestInfo.Item != null && lvJobControl.SelectedItems.Count == 1) {
        Job selectedJob = (Job)lvJobControl.SelectedItems[0].Tag;

        if (selectedJob != null && selectedJob.State == State.calculating) {
          lvJobControl.ContextMenuStrip.Items.Add(menuItemAbortJob);
          lvJobControl.ContextMenuStrip.Items.Add(menuItemGetSnapshot);
        }
      }
      lvJobControl.ContextMenuStrip.Show(new Point(e.X, e.Y));
    }

    private void tvClientControl_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
      lvClientControl.Items.Clear();
      lvClientControl.Groups.Clear();
      currentGroupNode = e.Node;
      AddGroupsToListView(e.Node);
    }

    private void groupToolStripMenuItem_Click(object sender, EventArgs e) {
      AddGroup addgroup = new AddGroup();
      parentgroup = Guid.Empty;
      if ((tvClientControl.SelectedNode != null) && (((ClientGroup)tvClientControl.SelectedNode.Tag).Id != Guid.Empty)) {
        parentgroup = ((ClientGroup)tvClientControl.SelectedNode.Tag).Id;
      }
      addgroup.addGroupEvent += new AddGroupDelegate(addgroup_addGroupEvent);
      addgroup.Show();
    }

    private void projectToolStripMenuItem_Click(object sender, EventArgs e) {
      AddProject addproject = new AddProject();
      addproject.AddProjectEvent += new AddProjectDelegate(addproject_AddProjectEvent);
      addproject.Show();
    }

    private void OnLVClientClicked(object sender, EventArgs e) {
      currentClient = (ClientInfo)lvClientControl.SelectedItems[0].Tag;
      ClientClicked();
    }

    private void tvClientControl_MouseUp(object sender, MouseEventArgs e) {
      // If the right mouse button was clicked and released,
      // display the shortcut menu assigned to the ListView. 
      contextMenuGroup.Items.Clear();
      TreeViewHitTestInfo hitTestInfo = tvClientControl.HitTest(e.Location);
      tvClientControl.SelectedNode = hitTestInfo.Node;
      if (e.Button != MouseButtons.Right) return;
      if (hitTestInfo.Node != null) {
        Resource selectedGroup = (Resource)tvClientControl.SelectedNode.Tag;

        if (selectedGroup != null) {
          contextMenuGroup.Items.Add(menuItemAddGroup);
          contextMenuGroup.Items.Add(menuItemDeleteGroup);
        }
      } else {
        contextMenuGroup.Items.Add(menuItemAddGroup);
      }
      tvClientControl.ContextMenuStrip.Show(tvClientControl, new Point(e.X, e.Y));
    }

    private void addproject_AddProjectEvent(string name) {
      IJobManager jobManager = ServiceLocator.GetJobManager();

      Project pg = new Project() { Name = name };
      jobManager.CreateProject(pg);

    }

    private void addgroup_addGroupEvent(string name) {
      IClientManager clientManager = ServiceLocator.GetClientManager();

      if (parentgroup != Guid.Empty) {
        ClientGroup cg = new ClientGroup() { Name = name };
        ResponseObject<ClientGroup> respcg = clientManager.AddClientGroup(cg);
        Response res = clientManager.AddResourceToGroup(parentgroup, respcg.Obj);
        if (res != null && !res.Success) {
          MessageBox.Show(res.StatusMessage, "Error adding Group", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      } else {
        ClientGroup cg = new ClientGroup() { Name = name };
        clientManager.AddClientGroup(cg);
      }
      AddClients();
    }


    private void Refresh_Click(object sender, EventArgs e) {
      Form overlayingForm = new Form();

      overlayingForm.Show();
      overlayingForm.FormBorderStyle = FormBorderStyle.None;
      overlayingForm.BackColor = Color.Gray;
      overlayingForm.Opacity = 0.4;
      overlayingForm.Size = this.Size;
      overlayingForm.Location = this.Location;

      //Label lbl = new Label();
      //overlayingForm.Controls.Add(lbl);
      //lbl.AutoSize = true;
      //lbl.Text = "Loading";
      //lbl.Name = "lblName";
      //lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      //lbl.ForeColor = Color.Black;
      //lbl.BackColor = Color.Transparent;
      //lbl.Location = new Point(overlayingForm.Width / 2, overlayingForm.Height / 2);

      AddClients();

      overlayingForm.Close();
    }

    private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e) {
      lvClientControl.View = View.LargeIcon;
      lvJobControl.View = View.LargeIcon;
      largeIconsToolStripMenuItem.CheckState = CheckState.Checked;
      smallIconsToolStripMenuItem.CheckState = CheckState.Unchecked;
      listToolStripMenuItem.CheckState = CheckState.Unchecked;
    }

    private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e) {
      lvClientControl.View = View.SmallIcon;
      lvJobControl.View = View.SmallIcon;
      largeIconsToolStripMenuItem.CheckState = CheckState.Unchecked;
      smallIconsToolStripMenuItem.CheckState = CheckState.Checked;
      listToolStripMenuItem.CheckState = CheckState.Unchecked;
    }

    private void listToolStripMenuItem_Click(object sender, EventArgs e) {
      lvClientControl.View = View.List;
      lvJobControl.View = View.List;
      largeIconsToolStripMenuItem.CheckState = CheckState.Unchecked;
      smallIconsToolStripMenuItem.CheckState = CheckState.Unchecked;
      listToolStripMenuItem.CheckState = CheckState.Checked;
    }

    private void tvClientControl_DragLeave(object sender, EventArgs e) {
      foreach (TreeNode node in tvClientControl.Nodes) {
        node.BackColor = Color.White;
        node.ForeColor = Color.Black;
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

    private bool IsEqual(ClientInfo ci1, ClientInfo ci2) {
      if (ci2 == null) {
        return false;
      }
      if (ci1.Id.Equals(ci2.Id)) {
        return true;
      } else return false;
    }

    private int CapacityRam(int noCores, int freeCores) {
      if (noCores > 0) {
        int capacity = ((noCores - freeCores) / noCores) * 100;
        System.Diagnostics.Debug.WriteLine(capacity);
        return capacity;
      }
      return 100;
    }

    private void GetDelta(IList<Job> oldJobs, IDictionary<int, Job> helpJobs) {
      bool found = false;
      for (int i = 0; i < jobs.List.Count; i++) {
        Job job = jobs.List[i];
        for (int j = 0; j < oldJobs.Count; j++) {

          Job jobold = oldJobs[j];

          if (job.Id.Equals(jobold.Id)) {

            found = true;
            bool change = false;
            if (job.State != jobold.State) {
              change = true;
            }
            if (job.State != State.offline) {
              if ((!IsEqual(job.Client, jobold.Client)) || (job.State != jobold.State)
                   || (job.Percentage != jobold.Percentage)) {
                change = true;
              }
            } else if (job.DateCalculated != jobold.DateCalculated) {
              change = true;
            }
            if (change) {
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

    private void GetSnapshotList() {

      lvSnapshots.Items.Clear();
      IJobManager jobManager = ServiceLocator.GetJobManager();

      ResponseList<JobResult> jobRes = jobManager.GetAllJobResults(currentJob.Id);

      if (jobRes.List != null) {
        foreach (JobResult jobresult in jobRes.List) {
          ListViewItem curSnapshot = new ListViewItem(jobresult.ClientId.ToString());
          double percentage = jobresult.Percentage * 100;
          curSnapshot.SubItems.Add(percentage.ToString() + " %");
          curSnapshot.SubItems.Add(jobresult.Timestamp.ToString());
          lvSnapshots.Items.Add(curSnapshot);
        }
      }

      if ((jobRes.List == null) && (jobRes.List.Count == 0)) {
        lvSnapshots.Visible = false;
      } else {
        lvSnapshots.Visible = true;
      }

    }

    #endregion



  }
}