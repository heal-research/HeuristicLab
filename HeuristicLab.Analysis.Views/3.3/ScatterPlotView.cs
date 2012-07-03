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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("ScatterPlot View")]
  [Content(typeof(ScatterPlot), true)]
  public partial class ScatterPlotView : NamedItemView {
    public new ScatterPlot Content {
      get { return (ScatterPlot)base.Content; }
      set { base.Content = value; }
    }

    public ScatterPlotView() {
      InitializeComponent();
      chart.CustomizeAllChartAreas();
    }

    protected override void DeregisterContentEvents() {
      Content.Points.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<PointF>>(Content_Points_Changed);
      Content.Points.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<PointF>>(Content_Points_Changed);
      Content.Points.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<PointF>>(Content_Points_Changed);
      Content.Points.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<PointF>>(Content_Points_Changed);
      Content.AxisNameChanged -= new EventHandler(Content_AxisNameChanged);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Points.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<PointF>>(Content_Points_Changed);
      Content.Points.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<PointF>>(Content_Points_Changed);
      Content.Points.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<PointF>>(Content_Points_Changed);
      Content.Points.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<PointF>>(Content_Points_Changed);
      Content.AxisNameChanged += new EventHandler(Content_AxisNameChanged);
    }

    private void Content_Points_Changed(object sender, CollectionItemsChangedEventArgs<IndexedItem<PointF>> e) {
      RedrawChart();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        ConfigureChart();
        RedrawChart();
      } else {
        ClearChart();
      }
    }

    private void Content_AxisNameChanged(object sender, EventArgs e) {
      ConfigureChart();
    }

    private void ConfigureChart() {
      if (InvokeRequired) {
        Invoke((Action)ConfigureChart);
      } else {
        chart.ChartAreas[0].AxisX.Title = Content.XAxisName;
        chart.ChartAreas[0].AxisY.Title = Content.YAxisName;
        chart.Titles[0].Text = Content.Name;
      }
    }

    private void ClearChart() {
      if (InvokeRequired) {
        Invoke((Action)ClearChart);
      } else {
        chart.Series[0].Points.Clear();
        chart.ChartAreas[0].AxisX.Title = String.Empty;
        chart.ChartAreas[0].AxisY.Title = String.Empty;
        chart.Titles[0].Text = String.Empty;
      }
    }

    private void RedrawChart() {
      if (InvokeRequired) {
        Invoke((Action)RedrawChart);
      } else {
        chart.Series[0].Points.SuspendUpdates();
        chart.Series[0].Points.Clear();
        foreach (var p in Content.Points.ToArray()) {
          chart.Series[0].Points.AddXY(p.X, p.Y);
        }
        chart.Series[0].Points.ResumeUpdates();
      }
    }
  }
}
