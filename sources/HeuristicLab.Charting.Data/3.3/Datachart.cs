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
using System.Text;
using System.Drawing;
using HeuristicLab.Charting;

namespace HeuristicLab.Charting.Data {
  public class Datachart : Chart {
    private IList<DataRowType> dataRowTypes;

    private string myTitle;
    public string Title {
      get { return myTitle; }
      set {
        if(myTitle != value) {
          myTitle = value;
          OnUpdate();
        }
      }
    }

    public Datachart(PointD lowerLeft, PointD upperRight)
      : base(lowerLeft, upperRight) {
      dataRowTypes = new List<DataRowType>();
    }
    public Datachart(double x1, double y1, double x2, double y2)
      : this(new PointD(x1, y1), new PointD(x2, y2)) {
    }

    public void AddDataRow(DataRowType dataRowType, Pen pen, Brush brush) {
      Group group = new Group(this);
      group.Pen = pen;
      group.Brush = brush;
      Group.Add(group);
      dataRowTypes.Add(dataRowType);
      if(dataRowType == DataRowType.Lines) {
        Group lines = new Group(this);
        group.Add(lines);
        Group points = new Group(this);
        group.Add(points);
      }
    }
    public void RemoveDataRow(int index) {
      Group.Remove(Group.Primitives[index]);
      dataRowTypes.RemoveAt(index);
    }

    public void AddDataPoint(int dataRowIndex, PointD point) {
      DataRowType dataRowType = dataRowTypes[dataRowIndex];
      Group group = (Group)Group.Primitives[dataRowIndex];

      switch(dataRowType) {
        case DataRowType.Lines:
          Group lines = (Group)group.Primitives[1];
          Group points = (Group)group.Primitives[0];
          if(points.Primitives.Count > 0) {
            FixedSizeRectangle lastRect = (FixedSizeRectangle)points.Primitives[0];
            Line line = new Line(this, lastRect.Point, point, group.Pen);
            lines.Add(line);
          }
          FixedSizeRectangle rect = new FixedSizeRectangle(this, point, new Size(5, 5), group.Pen, group.Brush);
          rect.ToolTipText = "(" + point.X.ToString() + " ; " + point.Y.ToString() + ")";
          points.Add(rect);
          break;
        case DataRowType.Bars:
          if(group.Primitives.Count < 2) {
            group.Add(new FixedSizeRectangle(this, point, new Size(0, 0), group.Pen, group.Brush));
            if(group.Primitives.Count == 2) {
              PointD p = ((FixedSizeRectangle)group.Primitives[0]).Point;
              PointD q = ((FixedSizeRectangle)group.Primitives[1]).Point;
              double y0 = Math.Min(p.Y, q.Y);
              double y1 = Math.Max(p.Y, q.Y);
              double x0 = Math.Min(p.X, q.X);
              double x1 = Math.Max(p.X, q.X);
              Rectangle bar = new Rectangle(this, x0, y0, x1, y1, group.Pen, group.Brush);
              bar.ToolTipText = "Height=" + (y1 - y0);
              group.Add(bar);
            }
          }
          break;
        case DataRowType.Points:
          FixedSizeRectangle r = new FixedSizeRectangle(this, point, new Size(5, 5), group.Pen, group.Brush);
          r.ToolTipText = "(" + point.X.ToString() + " ; " + point.Y.ToString() + ")";
          group.Add(r);
          break;
      }
    }
    public void AddDataPoint(int dataRowIndex, double x, double y) {
      AddDataPoint(dataRowIndex, new PointD(x, y));
    }
  }
}
