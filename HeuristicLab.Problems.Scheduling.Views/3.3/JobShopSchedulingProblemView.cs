#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Encodings.ScheduleEncoding;
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
      FillGanttChart();
    }

    protected override void DeregisterContentEvents() {
      Content.JobDataParameter.ValueChanged -= JobDataParameterOnValueChanged;
      Content.JobData.ItemsAdded -= JobsOnChanged;
      Content.JobData.ItemsRemoved -= JobsOnRemoved;
      Content.JobData.ItemsReplaced -= JobsOnChanged;
      Content.JobData.CollectionReset -= JobsOnChanged;
      foreach (var job in Content.JobData) {
        job.TasksChanged -= JobOnTasksChanged;
      }
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.JobDataParameter.ValueChanged += JobDataParameterOnValueChanged;
      Content.JobData.ItemsAdded += JobsOnChanged;
      Content.JobData.ItemsRemoved += JobsOnRemoved;
      Content.JobData.ItemsReplaced += JobsOnChanged;
      Content.JobData.CollectionReset += JobsOnChanged;
      foreach (var job in Content.JobData) {
        job.TasksChanged += JobOnTasksChanged;
      }
    }

    private void JobsOnChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<Job>> e) {
      foreach (var job in e.OldItems)
        job.Value.TasksChanged -= JobOnTasksChanged;
      foreach (var job in e.Items)
        job.Value.TasksChanged += JobOnTasksChanged;
      FillGanttChart();
    }

    private void JobsOnRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Job>> e) {
      foreach (var job in e.Items)
        job.Value.TasksChanged -= JobOnTasksChanged;
      FillGanttChart();
    }

    private void JobDataParameterOnValueChanged(object sender, EventArgs e) {
      Content.JobData.ItemsAdded += JobsOnChanged;
      Content.JobData.ItemsRemoved += JobsOnRemoved;
      Content.JobData.ItemsReplaced += JobsOnChanged;
      Content.JobData.CollectionReset += JobsOnChanged;
      foreach (var job in Content.JobData) {
        job.TasksChanged += JobOnTasksChanged;
      }
      FillGanttChart();
    }

    private void JobOnTasksChanged(object sender, EventArgs e) {
      FillGanttChart();
    }

    private void FillGanttChart() {
      ganttChart.Reset();
      if (Content == null) return;
      int jobCount = 0;
      foreach (var j in Content.JobData) {
        double lastEndTime = 0;
        foreach (var t in Content.JobData[jobCount].Tasks) {
          int categoryNr = t.JobNr;
          string categoryName = "Job" + categoryNr;
          ganttChart.AddData(categoryName,
            categoryNr,
            t.TaskNr,
            lastEndTime + 1,
            lastEndTime + t.Duration,
            "Job" + t.JobNr + " - " + "Task#" + t.TaskNr);
          lastEndTime += t.Duration;
        }
        jobCount++;
      }
    }
  }
}
