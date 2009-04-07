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
  public interface IPrimitive {
    IChart Chart { get; }
    IGroup Group { get; set; }
    Pen Pen { get; set; }
    Brush Brush { get; set; }
    bool UpdateEnabled { get; }
    bool Selected { get; set; }
    string ToolTipText { get; set; }
    object Tag { get; set; }

    void Move(Offset delta);
    void Move(double dx, double dy);

    bool ContainsPoint(PointD point);
    bool ContainsPoint(double x, double y);

    Cursor GetCursor(PointD point);
    Cursor GetCursor(double x, double y);
    string GetToolTipText(PointD point);
    string GetToolTipText(double x, double y);

    void OneLayerUp();
    void OneLayerDown();
    void IntoForeground();
    void IntoBackground();

    void MouseClick(PointD point, MouseButtons button);
    void MouseClick(double x, double y, MouseButtons button);
    void MouseDoubleClick(PointD point, MouseButtons button);
    void MouseDoubleClick(double x, double y, MouseButtons button);
    void MouseMove(PointD point, Offset offset);
    void MouseMove(double x, double y, double dx, double dy);
    void MouseDrag(PointD point, Offset offset, MouseButtons button);
    void MouseDrag(double x, double y, double dx, double dy, MouseButtons button);

    void PreDraw(Graphics graphics);
    void Draw(Graphics graphics);
    void PostDraw(Graphics graphics);

    event EventHandler Update;
    void EnforceUpdate();
  }
}
