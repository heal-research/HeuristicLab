#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.Scheduling.Views {
  [View("JobShop Scheduling Problem View")]
  [Content(typeof(JobShopSchedulingProblem), true)]
  public partial class JobShopSchedulingProblemView : NamedItemView {
    public JobShopSchedulingProblemView() {
      InitializeComponent();
    }
    private SchedulingProblemImportDialog spImportDialog;

    public new JobShopSchedulingProblem Content {
      get { return (JobShopSchedulingProblem)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        parameterCollectionView.Content = null;
        ganttChart.Reset();
      } else {
        parameterCollectionView.Content = ((IParameterizedNamedItem)Content).Parameters;
        FillGanttChart(Content);
      }
    }

    private void FillGanttChart(JobShopSchedulingProblem content) {
      //Add Jobs as Categories
      ganttChart.Reset();
      int jobCount = 0;
      Random random = new Random(1);
      foreach (Job j in content.JobData) {
        double lastEndTime = 0;
        foreach (Task t in content.JobData[jobCount].Tasks) {
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

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      parameterCollectionView.Enabled = Content != null;
      importButton.Enabled = Content != null && !ReadOnly;
    }

    private void importButton_Click(object sender, EventArgs e) {
      if (spImportDialog == null) spImportDialog = new SchedulingProblemImportDialog();

      if (spImportDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          switch (spImportDialog.Format) {
            case SPFormat.ORLib:
              Content.ImportFromORLibrary(spImportDialog.SPFileName);
              break;
          }


          if (!string.IsNullOrEmpty(spImportDialog.OptimalScheduleFileName))
            Content.ImportJSMSolution(spImportDialog.OptimalScheduleFileName);
          OnContentChanged();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
  }
}
