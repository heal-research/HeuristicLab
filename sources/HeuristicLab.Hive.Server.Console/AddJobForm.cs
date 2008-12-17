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
  public partial class AddJobForm : Form {

    ResponseList<Job> jobGroups = null;
    IJobManager jobManager;

    public AddJobForm() {
      InitializeComponent();
      addJob();
    }

    private void addJob() {
      jobManager =
        ServiceLocator.GetJobManager();
      jobGroups = jobManager.GetAllJobs();
      cbParJob.Items.Add("none");
      cbParJob.SelectedIndex = 0; 
      foreach (Job job in jobGroups.List) {
        cbParJob.Items.Add(job.Id);
      }
    }


    private void btnAdd_Click(object sender, EventArgs e) {
      if (cbParJob.SelectedIndex != 0) {
        foreach (Job pjob in jobGroups.List) {
          if (cbParJob.SelectedItem.ToString().Equals(pjob.Id.ToString())) {
            Job job = new Job { ParentJob = pjob };
            Response resp = jobManager.AddNewJob(job);
          }
        }
      } else {
        Job job = new Job();
        Response resp = jobManager.AddNewJob(job);
      }
    }

    private void btnClose_Click(object sender, EventArgs e) {
      this.Close();
    }
  }
}
