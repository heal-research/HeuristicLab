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

namespace HeuristicLab.Charting {
  public class Axis : AxisPrimitiveBase {
    public Axis(IChart chart, PointD point, AxisType axisType)
      : base(chart, point, axisType) {
    }
    public Axis(IChart chart, double x, double y, AxisType axisType)
      : this(chart, new PointD(x, y), axisType) {
    }
    public Axis(IChart chart, PointD point, AxisType axisType, Pen pen, Brush brush)
      : base(chart, point, axisType, pen, brush) {
    }
    public Axis(IChart chart, double x, double y, AxisType axisType, Pen pen, Brush brush)
      : this(chart, new PointD(x, y), axisType, pen, brush) {
    }

    public override void Draw(Graphics graphics) {
      int pixelsPerInterval = 100;
      Font axisValuesFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular, GraphicsUnit.Pixel);
      Font axisLabelsFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Regular, GraphicsUnit.Pixel);

      if ((AxisType & AxisType.Horizontal) == AxisType.Horizontal) {
        int intervals = Chart.SizeInPixels.Width / pixelsPerInterval;

        if (intervals > 0) {
          double step = (Chart.UpperRight.X - Chart.LowerLeft.X) / intervals;
          step = Math.Pow(10, Math.Floor(Math.Log10(step)));
          if ((Chart.UpperRight.X - Chart.LowerLeft.X) / (step * 5) > intervals)
            step = step * 5;
          else if ((Chart.UpperRight.X - Chart.LowerLeft.X) / (step * 2) > intervals)
            step = step * 2;

          double x = Math.Floor(Chart.LowerLeft.X / step) * step;
          PointD current = new PointD(x, Point.Y);
          while (current.X <= Chart.UpperRight.X) {
            Point p = Chart.TransformWorldToPixel(current);
            if (ShowGrid)
              graphics.DrawLine(Pens.LightGray, p.X, 0, p.X, Chart.SizeInPixels.Height);
            graphics.DrawLine(Pen, p.X, p.Y - 3, p.X, p.Y + 3);
            graphics.DrawString(current.X.ToString(),
                                axisValuesFont,
                                Brush,
                                p.X,
                                p.Y + 5,
                                StringFormat.GenericDefault);
            current.X = current.X + step;
          }
          if ((HorizontalLabel != null) && (HorizontalLabel != "")) {
            Point p = Chart.TransformWorldToPixel(Point);
            graphics.DrawString(HorizontalLabel,
                                axisLabelsFont,
                                Brush,
                                Chart.SizeInPixels.Width - 20,
                                p.Y + 20,
                                new StringFormat(StringFormatFlags.DirectionRightToLeft));
          }
        }
      }
      if ((AxisType & AxisType.Vertical) == AxisType.Vertical) {
        int intervals = Chart.SizeInPixels.Height / pixelsPerInterval;

        if (intervals > 0) {
          double step = (Chart.UpperRight.Y - Chart.LowerLeft.Y) / intervals;
          step = Math.Pow(10, Math.Floor(Math.Log10(step)));
          if ((Chart.UpperRight.Y - Chart.LowerLeft.Y) / (step * 5) > intervals)
            step = step * 5;
          else if ((Chart.UpperRight.Y - Chart.LowerLeft.Y) / (step * 2) > intervals)
            step = step * 2;

          double y = Math.Floor(Chart.LowerLeft.Y / step) * step;
          PointD current = new PointD(Point.X, y);
          while (current.Y <= Chart.UpperRight.Y) {
            Point p = Chart.TransformWorldToPixel(current);
            if (ShowGrid)
              graphics.DrawLine(Pens.LightGray, 0, p.Y, Chart.SizeInPixels.Width, p.Y);
            graphics.DrawLine(Pen, p.X - 3, p.Y, p.X + 3, p.Y);
            graphics.DrawString(current.Y.ToString(),
                                axisValuesFont,
                                Brush,
                                p.X + 5,
                                p.Y,
                                StringFormat.GenericDefault);
            current.Y = current.Y + step;
          }
          if ((VerticalLabel != null) && (VerticalLabel != "")) {
            Point p = Chart.TransformWorldToPixel(Point);
            graphics.DrawString(VerticalLabel,
                                axisLabelsFont,
                                Brush,
                                p.X - 20,
                                20,
                                new StringFormat(StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical));
          }
        }
      }
      if ((AxisType & AxisType.Horizontal) == AxisType.Horizontal) {
        Point p = Chart.TransformWorldToPixel(Point);
        graphics.DrawLine(Pen, 0, p.Y, Chart.SizeInPixels.Width, p.Y);
      }
      if ((AxisType & AxisType.Vertical) == AxisType.Vertical) {
        Point p = Chart.TransformWorldToPixel(Point);
        graphics.DrawLine(Pen, p.X, 0, p.X, Chart.SizeInPixels.Height);
      }
      base.Draw(graphics);
    }
  }
}
