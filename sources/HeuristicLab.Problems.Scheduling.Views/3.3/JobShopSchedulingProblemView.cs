#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization.Views;

namespace HeuristicLab.Problems.Scheduling.Views {
  [View("JobShop Scheduling Problem View")]
  [Content(typeof(JobShopSchedulingProblem), true)]
  public partial class JobShopSchedulingProblemView : ProblemView {

    public new JobShopSchedulingProblem Content {
      get { return (JobShopSchedulingProblem)base.Content; }
      set { base.Content = value; }
    }

    public JobShopSchedulingProblemView() {
      InitializeComponent();
      Controls.Remove(parameterCollectionView);
      parameterCollectionView.Dock = DockStyle.Fill;
      problemTabPage.Controls.Add(parameterCollectionView);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        ganttChart.Reset();
      } else {
        FillGanttChart(Content);
      }
    }

    protected override void DeregisterContentEvents() {
      Content.JobDataParameter.ValueChanged -= JobDataParameter_ValueChanged;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.JobDataParameter.ValueChanged += JobDataParameter_ValueChanged;
    }

    void JobDataParameter_ValueChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        FillGanttChart(Content);
      }
    }

    private void FillGanttChart(JobShopSchedulingProblem content) {
      ganttChart.Reset();
      int jobCount = 0;
      foreach (var j in content.JobData) {
        double lastEndTime = 0;
        foreach (var t in content.JobData[jobCount].Tasks) {
          int categoryNr = t.JobNr;
          string categoryName = "Job" + categoryNr;
          ganttChart.AddData(categoryName,
            categoryNr,
            t.TaskNr,
            lastEndTime + 1,
            lastEndTime + t.Duration,
            "Job" + t.JobNr + " - " + "Task#" + t.TaskNr.ToString());
          lastEndTime += t.Duration;
        }
        jobCount++;
      }
    }
  }
}
