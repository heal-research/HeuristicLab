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
  public partial class AddJobForm : Form {

    ResponseList<Job> jobGroups = null;
    IJobManager jobManager;

    public AddJobForm() {
      InitializeComponent();
      AddJob();
    }

    private void AddJob() {
      jobManager =
        ServiceLocator.GetJobManager();
      jobGroups = jobManager.GetAllJobs();
      cbParJob.Items.Add("none");
      cbParJob.SelectedIndex = 0; 
      foreach (Job job in jobGroups.List) {
        cbParJob.Items.Add(job.Id);
      }
    }


    private void BtnAdd_Click(object sender, EventArgs e) {
      try {
        lblError.Text = "";
        int numJobs = Convert.ToInt32(tbNumJobs.Text);
        if (numJobs > 0) {
          for (int i = 0; i < numJobs; i++) {
            if (cbParJob.SelectedIndex != 0) {
              foreach (Job pjob in jobGroups.List) {
                if (cbParJob.SelectedItem.ToString().Equals(pjob.Id.ToString())) {
                  Job job = new Job { ParentJob = pjob, State = State.offline };
                  Response resp = jobManager.AddNewJob(job);
                }
              }
            } else {
              Job job = new Job { State = State.offline };
              Response resp = jobManager.AddNewJob(job);
            }
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


  }
}
