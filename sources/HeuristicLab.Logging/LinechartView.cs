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

namespace HeuristicLab.Logging {
  public partial class LinechartView : ViewBase {
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

    public Linechart Linechart {
      get { return (Linechart)base.Item; }
      set { base.Item = value; }
    }

    public LinechartView() {
      InitializeComponent();
      Caption = "Linechart View";
    }
    public LinechartView(Linechart linechart)
      : this() {
      Linechart = linechart;
    }

    protected override void RemoveItemEvents() {
      if(Linechart != null) {
        Linechart.Values.ItemAdded -= new EventHandler<ItemIndexEventArgs>(Values_ItemAdded);
        Linechart.Values.ItemRemoved -= new EventHandler<ItemIndexEventArgs>(Values_ItemRemoved);
      }
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      if(Linechart != null) {
        Linechart.Values.ItemAdded += new EventHandler<ItemIndexEventArgs>(Values_ItemAdded);
        Linechart.Values.ItemRemoved += new EventHandler<ItemIndexEventArgs>(Values_ItemRemoved);
      }
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      Datachart datachart = new Datachart(-50, -5000, 1000, 55000);
      datachart.Title = "Line Chart";
      dataChartControl.ScaleOnResize = false;
      dataChartControl.Chart = datachart;
      datachart.Group.Clear();
      datachart.Group.Add(new Axis(datachart, 0, 0, AxisType.Both));
      double maxY = double.MinValue, minY = double.MaxValue;
      if(Linechart != null) {
        datachart.UpdateEnabled = false;
        for(int i = 0; i < Linechart.NumberOfLines; i++) {
          int colorIndex = (i % 12)*3;
          Color curCol = Color.FromArgb(colors[colorIndex],colors[colorIndex + 1], colors[colorIndex + 2]);
          Pen p = new Pen(curCol);
          SolidBrush b = new SolidBrush(curCol);
          datachart.AddDataRow(DataRowType.Lines, p, b);
        }

        for(int i = 0; i < Linechart.Values.Count; i++) {
          ItemList list = (ItemList)Linechart.Values[i];
          for(int j = 0; j < list.Count; j++) {
            double value = ((DoubleData)list[j]).Data;
            if(!double.IsInfinity(value) && !double.IsNaN(value)) {
              if(value < minY) minY = value;
              if(value > maxY) maxY = value;
              datachart.AddDataPoint(j, i, value);
            }
          }
        }
        datachart.ZoomIn(-Linechart.Values.Count * 0.05, minY - (minY * 0.1), Linechart.Values.Count * 1.05, maxY * 1.05);
        datachart.UpdateEnabled = true;
        datachart.EnforceUpdate();
      }
    }

    #region Values Events
    private delegate void ItemIndexDelegate(object sender, ItemIndexEventArgs e);
    private void Values_ItemRemoved(object sender, ItemIndexEventArgs e) {
      if(InvokeRequired) {
        Invoke(new ItemIndexDelegate(Values_ItemRemoved), sender, e);
      } else {
        Datachart datachart = dataChartControl.Chart;
      }
    }
    private void Values_ItemAdded(object sender, ItemIndexEventArgs e) {
      if(InvokeRequired) {
        Invoke(new ItemIndexDelegate(Values_ItemAdded), sender, e);
      } else {
        Datachart datachart = dataChartControl.Chart;
        ItemList list = (ItemList)e.Item;
        datachart.UpdateEnabled = false;
        for(int i = 0; i < list.Count; i++)
          datachart.AddDataPoint(i, e.Index, ((DoubleData)list[i]).Data);
        datachart.UpdateEnabled = true;
        datachart.EnforceUpdate();
      }
    }
    #endregion
  }
}
