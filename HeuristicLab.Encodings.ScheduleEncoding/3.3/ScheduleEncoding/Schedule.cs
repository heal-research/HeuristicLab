#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("Schedule", "Represents the general solution for scheduling problems.")]
  [StorableType("EA6D14A3-7FA6-4B4B-B7F6-40B42657D398")]
  public class Schedule : NamedItem, ISchedule {

    #region Properties
    [Storable]
    private ItemList<Resource> resources;
    public ItemList<Resource> Resources {
      get { return resources; }
    }
    [Storable]
    private double quality;
    public double Quality {
      get { return quality; }
      set {
        if (quality == value) return;
        quality = value;
        OnQualityChanged();
      }
    }
    [Storable]
    private Dictionary<int, ScheduledTask> lastScheduledTaskOfJob;
    #endregion

    [StorableConstructor]
    private Schedule(StorableConstructorFlag _) : base(_) { }
    private Schedule(Schedule original, Cloner cloner)
      : base(original, cloner) {
      this.resources = cloner.Clone(original.Resources);
      this.quality = original.Quality;
      this.lastScheduledTaskOfJob = original.lastScheduledTaskOfJob.ToDictionary(kvp => kvp.Key, kvp => cloner.Clone(kvp.Value));

      RegisterResourcesEvents();
    }
    public Schedule(int nrOfResources) {
      Name = "Schedule";

      resources = new ItemList<Resource>();
      for (int i = 0; i < nrOfResources; i++) {
        Resources.Add(new Resource(i));
      }
      lastScheduledTaskOfJob = new Dictionary<int, ScheduledTask>();

      RegisterResourcesEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Schedule(this, cloner);
    }

    #region Events
    public event EventHandler QualityChanged;
    private void OnQualityChanged() {
      var changed = QualityChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler ResourcesChanged;
    private void OnResourcesChanged() {
      var changed = ResourcesChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    private void RegisterResourcesEvents() {
      Resources.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Resources_PropertyChanged);
    }
    private void Resources_PropertyChanged(object sender, EventArgs e) {
      OnResourcesChanged();
    }
    #endregion

    public void ScheduleTask(int resNr, double startTime, double duration, int jobNr) {
      ScheduledTask task = new ScheduledTask(resNr, startTime, duration, jobNr);
      Resource affectedResource = resources[task.ResourceNr];
      int i = 0;
      while (i < affectedResource.Tasks.Count && affectedResource.Tasks[i].StartTime < task.StartTime)
        i++;

      if (!lastScheduledTaskOfJob.ContainsKey(jobNr)) {
        lastScheduledTaskOfJob.Add(jobNr, task);
        task.TaskNr = 0;
      } else {
        task.TaskNr = lastScheduledTaskOfJob[jobNr].TaskNr + 1;
        lastScheduledTaskOfJob[jobNr] = task;
      }

      if (i >= affectedResource.Tasks.Count)
        affectedResource.Tasks.Add(task);
      else
        affectedResource.Tasks.Insert(i, task);

    }

    public ScheduledTask GetLastScheduledTaskForJobNr(int jobNr) {
      if (lastScheduledTaskOfJob.ContainsKey(jobNr))
        return lastScheduledTaskOfJob[jobNr];
      else
        return null;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[ ");
      foreach (Resource r in Resources) {
        sb.AppendLine(r.ToString());
      }
      sb.Append("]");
      return sb.ToString();
    }
  }
}
