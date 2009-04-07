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
using System.Windows.Forms;

namespace HeuristicLab.Charting {
  public interface IChart {
    ChartMode Mode { get; set; }
    PointD LowerLeft { get; }
    PointD UpperRight { get; }
    SizeD Size { get; }
    Size SizeInPixels { get; }
    SizeD PixelToWorldRatio { get; }
    SizeD WorldToPixelRatio { get; }
    IGroup Group { get; }
    bool UpdateEnabled { get; }

    PointD TransformPixelToWorld(Point point);
    SizeD TransformPixelToWorld(Size size);
    Point TransformWorldToPixel(PointD point);
    Size TransformWorldToPixel(SizeD size);

    void SetPosition(PointD lowerLeft, PointD upperRight);
    void SetPosition(double x1, double y1, double x2, double y2);
    void Move(Offset delta);
    void Move(double dx, double dy);

    void ZoomIn(PointD lowerLeft, PointD upperRight);
    void ZoomIn(double x1, double y1, double x2, double y2);
    void ZoomIn(Point lowerLeft, Point upperRight);
    void ZoomIn(int x1, int y1, int x2, int y2);
    void ZoomOut();
    void Unzoom();

    IPrimitive GetPrimitive(Point point);
    IPrimitive GetPrimitive(int x, int y);
    IList<IPrimitive> GetAllPrimitives(Point point);
    IList<IPrimitive> GetAllPrimitives(int x, int y);

    Cursor GetCursor(Point point);
    Cursor GetCursor(int x, int y);
    string GetToolTipText(Point point);
    string GetToolTipText(int x, int y);

    void MouseClick(Point point, MouseButtons button);
    void MouseClick(int x, int y, MouseButtons button);
    void MouseDoubleClick(Point point, MouseButtons button);
    void MouseDoubleClick(int x, int y, MouseButtons button);
    void MouseMove(Point start, Point end);
    void MouseMove(int x1, int y1, int x2, int y2);
    void MouseDrag(Point start, Point end, MouseButtons button);
    void MouseDrag(int x1, int y1, int x2, int y2, MouseButtons button);

    void Render(Graphics graphics, int width, int height);

    event EventHandler Update;
    void EnforceUpdate();
  }
}
