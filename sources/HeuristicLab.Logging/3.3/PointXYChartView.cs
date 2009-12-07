#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Charting;
using HeuristicLab.Charting.Data;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Logging {
  /// <summary>
  /// Visual representation of a <see cref="PointXYChart"/>.
  /// </summary>
  [Content(typeof(PointXYChart), true)]
  public partial class PointXYChartView : ItemViewBase {
    private static int[] colors = new int[] {
      182,182,255, 
      218,255,182,
      255,182,218, 
      182,255,255, 
      218,182,255, 
      255,182,255,
      255,182,182, 
      255,218,182, 
      255,255,182, 
      182,255,182, 
      182,255,218, 
      182,218,255
    };

    /// <summary>
    /// Gets or sets the chart to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public PointXYChart PointXYChart {
      get { return (PointXYChart)base.Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PointXYChartView"/>.
    /// </summary>
    public PointXYChartView() {
      InitializeComponent();
      Caption = "PointXYChart View";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="PointXYChartView"/> with the given 
    /// <paramref name="pointXYChart"/>.
    /// </summary>
    /// <param name="pointXYChart">The chart to represent visually.</param>
    public PointXYChartView(PointXYChart pointXYChart)
      : this() {
      PointXYChart = pointXYChart;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="PointXYChart"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      if (PointXYChart != null) {
        PointXYChart.Values.ItemAdded -= new EventHandler<EventArgs<IItem, int>>(Values_ItemAdded);
        PointXYChart.Values.ItemRemoved -= new EventHandler<EventArgs<IItem, int>>(Values_ItemRemoved);
      }
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IOperatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      if (PointXYChart != null) {
        PointXYChart.Values.ItemAdded += new EventHandler<EventArgs<IItem, int>>(Values_ItemAdded);
        PointXYChart.Values.ItemRemoved += new EventHandler<EventArgs<IItem, int>>(Values_ItemRemoved);
      }
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      Datachart datachart = new Datachart(-50, -5000, 1000, 55000);
      datachart.Title = "Point X/Y Chart";
      dataChartControl.ScaleOnResize = false;
      dataChartControl.Chart = datachart;
      datachart.Group.Clear();
      datachart.Group.Add(new Axis(datachart, 0, 0, AxisType.Both));
      double maxY = double.MinValue, minY = double.MaxValue;
      double maxX = double.MinValue, minX = double.MaxValue;
      if (PointXYChart != null) {
        datachart.UpdateEnabled = false;
        for (int i = 0; i < PointXYChart.Values.Count; i++) {
          int colorIndex = (i % 12) * 3;
          Color curCol = Color.FromArgb(colors[colorIndex], colors[colorIndex + 1], colors[colorIndex + 2]);
          Pen p = new Pen(curCol);
          SolidBrush b = new SolidBrush(curCol);
          datachart.AddDataRow(PointXYChart.ConnectDots.Data ? DataRowType.Lines : DataRowType.Points, p, b);
        }

        for (int i = 0; i < PointXYChart.Values.Count; i++) {
          ItemList list = (ItemList) PointXYChart.Values[i];
          for (int j = 0; j < list.Count; j++) {
           ItemList value = ((ItemList)list[j]);
           double x = ((DoubleData)value[0]).Data;
           double y = ((DoubleData)value[1]).Data;
            if (!double.IsInfinity(x) && !double.IsNaN(x) && !double.IsInfinity(y) && !double.IsNaN(y)) {
              if (x < minX) minX = x;
              if (x > maxX) maxX = x;
              if (y < minY) minY = y;
              if (y > maxY) maxY = y;

              datachart.AddDataPoint(i, x, y);                                                         
            }
          }
        }
        datachart.ZoomIn(minX - (Math.Abs(maxX - minX) * 0.1), minY - (Math.Abs(maxY -minY) * 0.1), maxX + (Math.Abs(maxX - minX) * 0.1), maxY + (Math.Abs(maxY - minY) * 0.1));
        datachart.UpdateEnabled = true;
        datachart.EnforceUpdate();
      }
    }

    #region Values Events
    private delegate void ItemIndexDelegate(object sender, EventArgs<IItem, int> e);
    private void Values_ItemRemoved(object sender, EventArgs<IItem, int> e) {
      if (InvokeRequired) {
        Invoke(new ItemIndexDelegate(Values_ItemRemoved), sender, e);
      } else {
        Datachart datachart = dataChartControl.Chart;
      }
    }
    private void Values_ItemAdded(object sender, EventArgs<IItem, int> e) {
      if (InvokeRequired) {
        Invoke(new ItemIndexDelegate(Values_ItemAdded), sender, e);
      } else {
        //Datachart datachart = dataChartControl.Chart;
        //ItemList list = (ItemList)e.Item;
        //datachart.UpdateEnabled = false;
        //for (int i = 0; i < list.Count; i++)
        //  datachart.AddDataPoint(i, e.Index, ((DoubleArrayData)list[i]).Data[0]);
        //datachart.UpdateEnabled = true;
        //datachart.EnforceUpdate();
      }
    }
    #endregion
  }
}
