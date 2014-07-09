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
using DT = HeuristicLab.Services.Hive.DataTransfer;

public partial class Status : System.Web.UI.Page {
  protected void Page_Load(object sender, EventArgs e) {
    var dao = ServiceLocator.Instance.HiveDao;
    var transactionManager = ServiceLocator.Instance.TransactionManager;
    var resourceName = Request.QueryString["resource"];
    IEnumerable<Guid> resourceIds = new List<Guid>();
    IEnumerable<DT.Slave> onlineSlaves = new List<DT.Slave>();
    int currentlyJobsWaiting = 0;
    Dictionary<Guid, int> calculatingTasksByUser = new Dictionary<Guid,int>();
    Dictionary<Guid, int> waitingTasksByUser = new Dictionary<Guid, int>();
    List<DT.Resource> groups = new List<DT.Resource>();

    transactionManager.UseTransaction(() => {
       groups = dao.GetResources(x => x.ResourceType == "GROUP").ToList();
    }, false, false);    

    if (!string.IsNullOrEmpty(resourceName)) {
        transactionManager.UseTransaction(() =>
        {
            var resId = dao.GetResources(x => x.Name == resourceName).Single().Id;
            resourceIds = dao.GetChildResources(resId).Select(x => x.Id).Union(new List<Guid> { resId });
            calculatingTasksByUser = dao.GetCalculatingTasksByUserForResources(resourceIds.ToList());
            waitingTasksByUser = dao.GetWaitingTasksByUserForResources(resourceIds.ToList());
        }, false, false);        
    } else {
        transactionManager.UseTransaction(() =>
        {
             resourceIds = dao.GetResources(x => true).Select(y => y.Id);
             calculatingTasksByUser = dao.GetCalculatingTasksByUser();
             waitingTasksByUser = dao.GetWaitingTasksByUser();            
        }, false, false);
    }   

    transactionManager.UseTransaction(() =>
    {                     
        onlineSlaves = dao.GetSlaves(x => (x.SlaveState == DA.SlaveState.Calculating || x.SlaveState == DA.SlaveState.Idle) && resourceIds.Contains(x.ResourceId));
        currentlyJobsWaiting = dao.GetLightweightTasks(x => x.State == DA.TaskState.Waiting).Count();            
    }, false, false);

    int overallCurrentlyAvailableCores = onlineSlaves.Where(s => s.Cores.HasValue).Sum(s => s.Cores.Value);
    int currentlyAvailableCores = onlineSlaves.Where(s => s.Cores.HasValue && s.IsAllowedToCalculate).Sum(s => s.Cores.Value);
    int currentlyUsedCores = overallCurrentlyAvailableCores - onlineSlaves.Where(s => s.FreeCores.HasValue).Sum(s => s.FreeCores.Value);
    
    this.overallAvailableCoresLabel.Text = overallCurrentlyAvailableCores.ToString();
    this.availableCoresLabel.Text = currentlyAvailableCores.ToString();
    this.usedCoresLabel.Text = currentlyUsedCores.ToString();
    this.waitingJobsLabel.Text = currentlyJobsWaiting.ToString();

    slavesLabel.Text = string.Join(", ", onlineSlaves.Select(x => string.Format("<a href=\"?resource={0}\">{0}</a> ({1} %)", x.Name, Math.Round(x.CpuUtilization, 2))));
    groupsLabel.Text = "<a href=\"Status.aspx\">All</a>, ";
    groupsLabel.Text += string.Join(", ", groups.Select(x => string.Format("<a href=\"?resource={0}\">{0}</a>", x.Name)));

    overallCpuUtilizationLabel.Text = (onlineSlaves.Count() > 0 ? Math.Round(onlineSlaves.Average(s => s.CpuUtilization), 2).ToString() : "0.0") + " %";
    cpuUtilizationLabel.Text = (onlineSlaves.Count() > 0 && onlineSlaves.Where(x => x.IsAllowedToCalculate).Count() > 0 ? Math.Round(onlineSlaves.Where(x => x.IsAllowedToCalculate).Average(s => s.CpuUtilization), 2).ToString() : "0.0") + " %";

    DT.Statistics[] stats = new DT.Statistics[0];    
    transactionManager.UseTransaction(() =>
    {   
        if (daysDropDownList.SelectedValue == "All") {
          stats = dao.GetStatistics(x => true).OrderBy(x => x.TimeStamp).ToArray();
        } else {
          stats = dao.GetStatistics(x => x.Timestamp >= DateTime.Now.Subtract(TimeSpan.FromDays(int.Parse(daysDropDownList.SelectedValue)))).OrderBy(x => x.TimeStamp).ToArray();
        }
    }, false, false);
    
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
    }

    GenerateTasksByUserTable(waitingTasksByUser, waitingTasksByUserTable);  
    GenerateTasksByUserTable(calculatingTasksByUser, calculatingTasksByUserTable);  
  }

  private void GenerateTasksByUserTable(Dictionary<Guid, int> tasksByUser, Table table) {
    foreach (var kvp in tasksByUser) {
      TableRow curRow = new TableRow();
      TableCell cellUser = new TableCell();
      cellUser.Text = ServiceLocator.Instance.UserManager.GetUserById(kvp.Key).UserName;
      TableCell cellCnt = new TableCell();
      cellCnt.Text = kvp.Value.ToString();

      curRow.Cells.Add(cellUser);
      curRow.Cells.Add(cellCnt);
      table.Rows.Add(curRow);
    }
    if (tasksByUser.Count() > 0) {
      TableRow sumRow = new TableRow();        
      TableCell sumCell = new TableCell();
      sumCell.BorderWidth = Unit.Pixel(3);
      sumCell.Text = tasksByUser.Sum(x => x.Value).ToString();
      sumRow.Cells.Add(new TableCell());
      sumRow.Cells.Add(sumCell);
      table.Rows.Add(sumRow);
    }
  }  
}