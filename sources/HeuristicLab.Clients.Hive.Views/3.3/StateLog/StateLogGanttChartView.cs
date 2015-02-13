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
using System.Linq;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.Views {
  [View("StateLogGanttChart View")]
  [Content(typeof(StateLogList), IsDefaultView = true)]
  public sealed partial class StateLogGanttChartView : ItemView {
    public new StateLogList Content {
      get { return (StateLogList)base.Content; }
      set { base.Content = value; }
    }

    public StateLogGanttChartView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      // Deregister your event handlers here
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      // Register your event handlers here
    }

    #region Event Handlers (Content)
    // Put event handlers of the content here
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        // Add code when content has been changed and is null
        ganttChart.Reset();
      } else {
        // Add code when content has been changed and is not null
        ganttChart.Reset();
        StateLogGanttChartListView.SetupCategories(ganttChart);
        if (Content.Count > 0) {
          DateTime upperLimit;
          if (Content.Last().State == TaskState.Finished || Content.Last().State == TaskState.Failed || Content.Last().State == TaskState.Aborted) {
            upperLimit = DateTime.FromOADate(Math.Min(Content.Max(x => x.DateTime).ToOADate(), DateTime.Now.AddSeconds(10).ToOADate()));
          } else {
            upperLimit = DateTime.Now;
          }

          for (int i = 0; i < Content.Count - 1; i++) {
            StateLogGanttChartListView.AddData(ganttChart, i.ToString(), Content[i], Content[i + 1], upperLimit);
          }
          StateLogGanttChartListView.AddData(ganttChart, (Content.Count - 1).ToString(), Content[Content.Count - 1], null, upperLimit);
        }
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      // Enable or disable controls based on whether the content is null or the view is set readonly
    }

    #region Event Handlers (child controls)
    // Put event handlers of child controls here.
    #endregion
  }
}
