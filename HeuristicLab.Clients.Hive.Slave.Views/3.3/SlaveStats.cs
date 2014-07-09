#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;


namespace HeuristicLab.Clients.Hive.SlaveCore.Views {

  [View("HeuristicLab Slave Stats View")]
  [Content(typeof(SlaveItem), IsDefaultView = false)]
  public partial class SlaveStats : ItemView {
    private SlaveDisplayStat lastSlaveDisplayStat;

    public new SlaveItem Content {
      get { return (SlaveItem)base.Content; }
      set {
        if (base.Content != value) {
          base.Content = value;
        }
      }
    }

    public SlaveStats() {
      InitializeComponent();
      txtSlaveState.Text = SlaveDisplayStat.NoService.ToString();
      lastSlaveDisplayStat = SlaveDisplayStat.NoService;
      Content_SlaveDisplayStateChanged(this, new EventArgs<SlaveDisplayStat>(lastSlaveDisplayStat));
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      Content.SlaveStatusChanged -= new System.EventHandler<EventArgs<StatusCommons>>(Content_SlaveStatusChanged);
      Content.SlaveDisplayStateChanged -= new EventHandler<EventArgs<SlaveDisplayStat>>(Content_SlaveDisplayStateChanged);

      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();

      Content.SlaveStatusChanged += new System.EventHandler<EventArgs<StatusCommons>>(Content_SlaveStatusChanged);
      Content.SlaveDisplayStateChanged += new EventHandler<EventArgs<SlaveDisplayStat>>(Content_SlaveDisplayStateChanged);
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    #region Event Handlers
    void Content_SlaveStatusChanged(object sender, EventArgs<StatusCommons> e) {
      if (InvokeRequired) {
        Action<object, EventArgs<StatusCommons>> action = new Action<object, EventArgs<StatusCommons>>(Content_SlaveStatusChanged);
        Invoke(action, sender, e);
      } else {
        RenderJobChart(e.Value);
        RenderCoreChart(e.Value);
      }
    }

    void Content_SlaveDisplayStateChanged(object sender, EventArgs<SlaveDisplayStat> e) {
      if (InvokeRequired) {
        Action<object, EventArgs<SlaveDisplayStat>> action = new Action<object, EventArgs<SlaveDisplayStat>>(Content_SlaveDisplayStateChanged);
        Invoke(action, sender, e);
      } else {
        txtSlaveState.Text = e.Value.ToString();
        lastSlaveDisplayStat = e.Value;
      }
    }
    #endregion

    private void RenderJobChart(StatusCommons status) {
      if (InvokeRequired) {
        Invoke(new Action<StatusCommons>(RenderJobChart), status);
      } else {
        taskChart.Series[0].Points.Clear();
        taskChart.Series[1].Points.Clear();
        taskChart.Series[2].Points.Clear();
        taskChart.Series[3].Points.Clear();
        taskChart.Series[4].Points.Clear();


        DataPoint pJobs = new DataPoint(1, status.Jobs.Count);
        DataPoint pJobsAborted = new DataPoint(2, status.JobsAborted);
        DataPoint pJobsDone = new DataPoint(3, status.JobsFinished);
        DataPoint pJobsFetched = new DataPoint(4, status.JobsFetched);
        DataPoint pJobsFailed = new DataPoint(5, status.JobsFailed);

        pJobs.LegendText = "Current tasks: " + status.Jobs.Count;
        pJobs.Color = System.Drawing.Color.Yellow;
        pJobs.ToolTip = pJobs.LegendText;
        taskChart.Series[0].Color = System.Drawing.Color.Yellow;
        taskChart.Series[0].LegendText = pJobs.LegendText;
        taskChart.Series[0].Points.Add(pJobs);

        pJobsAborted.LegendText = "Aborted tasks: " + status.JobsAborted;
        pJobsAborted.Color = System.Drawing.Color.Orange;
        pJobsAborted.ToolTip = pJobsAborted.LegendText;
        taskChart.Series[1].Color = System.Drawing.Color.Orange;
        taskChart.Series[1].LegendText = pJobsAborted.LegendText;
        taskChart.Series[1].Points.Add(pJobsAborted);

        pJobsDone.LegendText = "Finished tasks: " + status.JobsFinished;
        pJobsDone.Color = System.Drawing.Color.Green;
        pJobsDone.ToolTip = pJobsDone.LegendText;
        taskChart.Series[2].Color = System.Drawing.Color.Green;
        taskChart.Series[2].LegendText = pJobsDone.LegendText;
        taskChart.Series[2].Points.Add(pJobsDone);

        pJobsFetched.LegendText = "Fetched tasks: " + status.JobsFetched;
        pJobsFetched.ToolTip = pJobsFetched.LegendText;
        pJobsFetched.Color = System.Drawing.Color.Blue;
        taskChart.Series[3].Color = System.Drawing.Color.Blue;
        taskChart.Series[3].LegendText = pJobsFetched.LegendText;
        taskChart.Series[3].Points.Add(pJobsFetched);

        pJobsFailed.LegendText = "Failed tasks: " + status.JobsFailed;
        pJobsFailed.ToolTip = pJobsFailed.LegendText;
        pJobsFailed.Color = System.Drawing.Color.Red;
        taskChart.Series[4].Color = System.Drawing.Color.Red;
        taskChart.Series[4].LegendText = pJobsFailed.LegendText;
        taskChart.Series[4].Points.Add(pJobsFailed);
      }
    }

    private void RenderCoreChart(StatusCommons statusCommons) {
      if (InvokeRequired) {
        Invoke(new Action<StatusCommons>(RenderCoreChart), statusCommons);
      } else {
        int usedCores = statusCommons.TotalCores - statusCommons.FreeCores;
        DataPoint pFreeCores = new DataPoint(statusCommons.FreeCores, statusCommons.FreeCores);
        DataPoint pUsedCores = new DataPoint(usedCores, usedCores);

        coresChart.Series[0].Points.Clear();

        pFreeCores.LegendText = "Free: " + statusCommons.FreeCores;
        pFreeCores.Color = System.Drawing.Color.Green;
        pUsedCores.LegendText = "Used: " + usedCores;
        pUsedCores.Color = System.Drawing.Color.Red;

        coresChart.Series[0].Points.Add(pFreeCores);
        coresChart.Series[0].Points.Add(pUsedCores);
      }
    }
  }
}
