#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HeuristicLab.Services.Hive.DataAccess;
using HeuristicLab.Services.Hive;
using HeuristicLab.Services.Hive.DataTransfer;
using System.Text;
using System.Web.UI.DataVisualization.Charting;
using DA = HeuristicLab.Services.Hive.DataAccess;

public partial class Status : System.Web.UI.Page {
  protected void Page_Load(object sender, EventArgs e) {
    var dao = ServiceLocator.Instance.HiveDao;
    var resourceName = Request.QueryString["resource"];
    IEnumerable<Guid> resourceIds;
    if (!string.IsNullOrEmpty(resourceName)) {
      var resId = dao.GetResources(x => x.Name == resourceName).Single().Id;
      resourceIds = dao.GetChildResources(resId).Select(x => x.Id).Union(new List<Guid> { resId });
      speedupChartHours.Visible = false;
      speedupChartMinutes.Visible = false;
    } else {
      resourceIds = dao.GetResources(x => true).Select(y => y.Id);
      speedupChartHours.Visible = true;
      speedupChartMinutes.Visible = true;
    }
                                  
    var onlineSlaves = dao.GetSlaves(x => (x.SlaveState == DA.SlaveState.Calculating || x.SlaveState == DA.SlaveState.Idle) && resourceIds.Contains(x.ResourceId));

    int currentlyAvailableCores = onlineSlaves.Where(s => s.Cores.HasValue).Sum(s => s.Cores.Value);
    int currentlyUsedCores = currentlyAvailableCores - onlineSlaves.Where(s => s.FreeCores.HasValue).Sum(s => s.FreeCores.Value);
    int currentlyJobsWaiting = ServiceLocator.Instance.HiveDao.GetTasks(x => x.State == DA.TaskState.Waiting).Count();

    this.availableCoresLabel.Text = currentlyAvailableCores.ToString();
    this.usedCoresLabel.Text = currentlyUsedCores.ToString();
    this.waitingJobsLabel.Text = currentlyJobsWaiting.ToString();

    slavesLabel.Text = string.Join(", ", onlineSlaves.Select(x => string.Format("<a href=\"?resource={0}\">{0}</a> ({1} %)", x.Name, Math.Round(x.CpuUtilization, 2))));

    cpuUtilizationLabel.Text = (onlineSlaves.Count() > 0 ? Math.Round(onlineSlaves.Average(s => s.CpuUtilization), 2).ToString() : "0.0") + " %";

    HeuristicLab.Services.Hive.DataTransfer.Statistics[] stats;
    if (daysDropDownList.SelectedValue == "All") {
      stats = dao.GetStatistics(x => true).OrderBy(x => x.TimeStamp).ToArray();
    } else {
      stats = dao.GetStatistics(x => x.Timestamp >= DateTime.Now.Subtract(TimeSpan.FromDays(int.Parse(daysDropDownList.SelectedValue)))).OrderBy(x => x.TimeStamp).ToArray();
    }
    
    for (int i = 0; i < stats.Length; i++) {
      var s = stats[i];
      var slaveStats = s.SlaveStatistics.Where(x => resourceIds.Contains(x.SlaveId));

      var averageCpuUtilization = slaveStats.Count() > 0 ? slaveStats.Average(x => x.CpuUtilization) : 0.0;
      cpuUtilizationChart.Series[0].Points.Add(new DataPoint(s.TimeStamp.ToOADate(), averageCpuUtilization));

      var cores = slaveStats.Sum(x => x.Cores);

      var usedCores = cores - slaveStats.Sum(x => x.FreeCores);
      coresChart.Series[0].Points.AddXY(s.TimeStamp.ToOADate(), cores);
      coresChart.Series[1].Points.AddXY(s.TimeStamp.ToOADate(), usedCores);

      var memory = slaveStats.Sum(x => x.Memory);
      var usedMemory = memory - slaveStats.Sum(x => x.FreeMemory);
      memoryChart.Series[0].Points.AddXY(s.TimeStamp.ToOADate(), memory / 1024.0);
      memoryChart.Series[1].Points.AddXY(s.TimeStamp.ToOADate(), usedMemory / 1024.0);

      if (i > 0) {
        var execTime = new TimeSpan(s.UserStatistics.Sum(x => x.ExecutionTime.Ticks));
        var execTimePrev = new TimeSpan(stats[i - 1].UserStatistics.Sum(x => x.ExecutionTime.Ticks));
        var execTimeDifference = execTimePrev - execTime;

        var timeDifference = stats[i - 1].TimeStamp - s.TimeStamp; // the difference between statistic entries is not alway exactly 1 minute
        var speedup = execTimeDifference.TotalMinutes / timeDifference.TotalMinutes;
        speedupChartMinutes.Series[0].Points.AddXY(s.TimeStamp.ToOADate(), speedup);
        speedupChartMinutes.Series[1].Points.AddXY(s.TimeStamp.ToOADate(), cores);
      }
      if (i - 60 >= 0) {
        var execTime = new TimeSpan(s.UserStatistics.Sum(x => x.ExecutionTime.Ticks));
        var execTimePrev = new TimeSpan(stats[i - 60].UserStatistics.Sum(x => x.ExecutionTime.Ticks));
        var execTimeDifference = execTimePrev - execTime;

        var timeDifference = stats[i - 60].TimeStamp - s.TimeStamp; // the difference between statistic entries is not alway exactly 1 minute
        var speedup = execTimeDifference.TotalMinutes / timeDifference.TotalMinutes;
        speedupChartHours.Series[0].Points.AddXY(s.TimeStamp.ToOADate(), speedup);
        speedupChartHours.Series[1].Points.AddXY(s.TimeStamp.ToOADate(), cores);
      }
    }
  }

  protected void daysDropDownList_SelectedIndexChanged(object sender, EventArgs e) {

  }
}