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
using System.Linq;
using HeuristicLab.Charting;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;
using System.Diagnostics;
using HeuristicLab.SparseMatrix;


namespace HeuristicLab.CEDMA.Charting {
  public class ModelingBubbleChart :BubbleChart {
    public ModelingBubbleChart(VisualMatrix matrix, PointD lowerLeft, PointD upperRight)
      : base(matrix,lowerLeft, upperRight) {

      matrix.Changed += new EventHandler(matrix_Changed);
    }

    public ModelingBubbleChart(VisualMatrix matrix, double x1, double y1, double x2, double y2)
      : this(matrix, new PointD(x1, y1), new PointD(x2, y2)) {
    }

    public void matrix_Changed(object sender, EventArgs e) {
      Refresh();
    }

    public override void MouseDoubleClick(Point point, MouseButtons button) {
      if (button == MouseButtons.Left) {
        VisualMatrixRow row = GetMatrixRow(point);
        if (row != null) {
          var model = (IItem)PersistenceManager.RestoreFromGZip((byte[])row.Get("PersistedData"));
          if (model != null)
            PluginManager.ControlManager.ShowControl(model.CreateView());
        }
      } else {
        base.MouseDoubleClick(point, button);
      }
    }

    public override void MouseDrag(Point start, Point end, MouseButtons button) {
      if (button == MouseButtons.Left && Mode == ChartMode.Select) {
        PointD a = TransformPixelToWorld(start);
        PointD b = TransformPixelToWorld(end);
        double minX = Math.Min(a.X, b.X);
        double minY = Math.Min(a.Y, b.Y);
        double maxX = Math.Max(a.X, b.X);
        double maxY = Math.Max(a.Y, b.Y);
        HeuristicLab.Charting.Rectangle rect = new HeuristicLab.Charting.Rectangle(this, minX, minY, maxX, maxY);

        List<IPrimitive> primitives = new List<IPrimitive>();
        primitives.AddRange(points.Primitives);

        foreach (FixedSizeCircle p in primitives) {
          if (rect.ContainsPoint(p.Point)) {
            VisualMatrixRow r;
            primitiveToMatrixRowDictionary.TryGetValue(p, out r);
            if (r != null) r.ToggleSelected();
          }
        }
        if (primitives.Count() > 0) matrix.FireChanged();
      } else {
        base.MouseDrag(start, end, button);
      }
    }

    public override void MouseClick(Point point, MouseButtons button) {
      if (button == MouseButtons.Left) {
        VisualMatrixRow r = GetMatrixRow(point);
        if (r != null) {
          r.ToggleSelected();
          matrix.FireChanged();
        }
      } else {
        base.MouseClick(point, button);
      }
    }
  }
}
