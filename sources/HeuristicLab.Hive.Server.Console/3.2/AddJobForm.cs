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
using HeuristicLab.Hive.JobBase;
using HeuristicLab.Core;
using System.ServiceModel;

namespace HeuristicLab.Hive.Server.ServerConsole {

  public delegate void addDelegate();

  public partial class AddJobForm : Form {

    public event addDelegate addJobEvent;

    ResponseList<ProjectDto> projects = null;
    IJobManager jobManager;
    IClientManager clientManager; 
    ResponseList<ClientGroupDto> clientGroups;

    Dictionary<Guid, string> clients = null;

    public AddJobForm() {
      InitializeComponent();

      clients = new Dictionary<Guid, string>();
      AddJob();

    }

    private void AddJob() {
      try {

      jobManager =
        ServiceLocator.GetJobManager();
      projects = jobManager.GetAllProjects();
      clientManager =
        ServiceLocator.GetClientManager();
      clientGroups = clientManager.GetAllClientGroups();
      cbProject.Items.Add("none");
      cbProject.SelectedIndex = 0;
      foreach (ProjectDto project in projects.List) {
        cbProject.Items.Add(project.Name);
      }

      AddClientGroups();

      foreach (KeyValuePair<Guid, string> kvp in clients) {
        lbGroupsOut.Items.Add(kvp.Value + " (" + kvp.Key + ")");
      }
      }
      catch (FaultException fe) {
        MessageBox.Show(fe.Message);
      }

    }

    private void AddClientGroups() {
     foreach (ClientGroupDto cg in clientGroups.List) {
       if (cg.Id != Guid.Empty)
       clients.Add(cg.Id, cg.Name);
        AddClientOrGroup(cg);
      }
    }

    private void AddClientOrGroup(ClientGroupDto clientGroup) {
      foreach (ResourceDto resource in clientGroup.Resources) {
        if (resource is ClientGroupDto) {
          if (resource.Id != Guid.Empty)
          clients.Add(resource.Id, resource.Name);
          AddClientOrGroup(resource as ClientGroupDto);
        }
      }
    
    }

    private void BtnAdd_Click(object sender, EventArgs e) {
      try {
        lblError.Text = "";
        int numJobs = Convert.ToInt32(tbNumJobs.Text);
        if (numJobs > 0) {
          for (int i = 0; i < numJobs; i++) {
            JobDto job = new JobDto { State = State.offline, CoresNeeded = 1 };
            
            // if project selected (0 -> none)
            if (cbProject.SelectedIndex != 0) {
              job.Project = projects.List[cbProject.SelectedIndex - 1];
            }

            if (!cbAllGroups.Checked) {
              List<Guid> groupsToCalculate = new List<Guid>();
              foreach (string item in lbGroupsIn.Items) {
                int start = item.IndexOf("(");
                int end = item.IndexOf(")");
                string substring = item.Substring(start + 1, end - start - 1);
                Guid guid = new Guid(substring);
                groupsToCalculate.Add(guid);
              }
              job.AssignedResourceIds = groupsToCalculate;
            }

            SerializedJob computableJob =
              new SerializedJob();
            computableJob.JobInfo = job;
            computableJob.SerializedJobData = PersistenceManager.SaveToGZip(new TestJob());
            Response resp = jobManager.AddNewJob(computableJob);
          }
          if (addJobEvent != null) {
            addJobEvent();
          }
          this.Close();
        } else {
          lblError.Text = "Wrong number of Jobs";
        }

      }
      catch {
        lblError.Text = "There should be a number";
      }
    }

    private void BtnClose_Click(object sender, EventArgs e) {
      this.Close();
    }

    private void cbAllGroups_CheckedChanged(object sender, EventArgs e) {
      foreach (Control control in gbGroups.Controls) {
        control.Enabled = !cbAllGroups.Checked;
      }
    }

    private void btnAddGroup_Click(object sender, EventArgs e) {
      AddGroup();
    }

    private void btnRemoveGroup_Click(object sender, EventArgs e) {
      RemoveGroup();
    }

    private void lbGroupsOut_SelectedIndexChanged(object sender, EventArgs e) {
      AddGroup();
    }

    private void lbGroupsIn_SelectedIndexChanged(object sender, EventArgs e) {
      RemoveGroup();
    }

    private void AddGroup() {
      if (lbGroupsOut.SelectedItem != null) {
        lbGroupsIn.Items.Add(lbGroupsOut.SelectedItem);
        lbGroupsOut.Items.RemoveAt(lbGroupsOut.SelectedIndex);
      }
  
    }

    private void RemoveGroup() {
      if (lbGroupsIn.SelectedItem != null) {
        lbGroupsOut.Items.Add(lbGroupsIn.SelectedItem);
        lbGroupsIn.Items.RemoveAt(lbGroupsIn.SelectedIndex);
      }
    }

    private void btnLoad_Click(object sender, EventArgs e) {

    }


  }
}
