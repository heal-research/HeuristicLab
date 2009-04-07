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
  public abstract class PrimitiveBase : IPrimitive {
    private IChart myChart;
    public IChart Chart {
      get { return myChart; }
    }
    private IGroup myGroup;
    public IGroup Group {
      get { return myGroup; }
      set { myGroup = value; }
    }

    private Pen myPen;
    public virtual Pen Pen {
      get { return myPen; }
      set {
        myPen = value;
        OnUpdate();
      }
    }
    private Brush myBrush;
    public virtual Brush Brush {
      get { return myBrush; }
      set {
        myBrush = value;
        OnUpdate();
      }
    }

    private bool myUpdateEnabled;
    public bool UpdateEnabled {
      get { return myUpdateEnabled; }
      set { myUpdateEnabled = value; }
    }

    private bool mySelected;
    public virtual bool Selected {
      get { return mySelected; }
      set {
        mySelected = value;
        OnUpdate();
      }
    }

    private string myToolTipText;
    public string ToolTipText {
      get { return myToolTipText; }
      set { myToolTipText = value; }
    }

    private object myTag;
    public object Tag {
      get { return myTag; }
      set { myTag = value; }
    }

    protected PrimitiveBase(IChart chart) {
      myChart = chart;
      myGroup = null;
      myPen = Pens.Black;
      myBrush = Brushes.White;
      myUpdateEnabled = true;
      mySelected = false;
      myToolTipText = null;
      myTag = null;
    }
    protected PrimitiveBase(IChart chart, Pen pen, Brush brush)
      : this(chart) {
      myPen = pen;
      myBrush = brush;
    }

    public abstract void Move(Offset delta);
    public void Move(double dx, double dy) {
      Move(new Offset(dx, dy));
    }

    public abstract bool ContainsPoint(PointD point);
    public bool ContainsPoint(double x, double y) {
      return ContainsPoint(new PointD(x, y));
    }

    public virtual Cursor GetCursor(PointD point) {
      return null;
    }
    public Cursor GetCursor(double x, double y) {
      return GetCursor(new PointD(x, y));
    }
    public virtual string GetToolTipText(PointD point) {
      return ToolTipText;
    }
    public string GetToolTipText(double x, double y) {
      return GetToolTipText(new PointD(x, y));
    }

    public void OneLayerUp() {
      Group.OneLayerUp(this);
    }
    public void OneLayerDown() {
      Group.OneLayerDown(this);
    }
    public void IntoForeground() {
      Group.IntoForeground(this);
    }
    public void IntoBackground() {
      Group.IntoBackground(this);
    }

    public virtual void MouseClick(PointD point, MouseButtons button) {
      if (button == MouseButtons.Left) {
        Selected = true;
      }
    }
    public void MouseClick(double x, double y, MouseButtons button) {
      MouseClick(new PointD(x, y), button);
    }
    public virtual void MouseDoubleClick(PointD point, MouseButtons button) {
    }
    public void MouseDoubleClick(double x, double y, MouseButtons button) {
      MouseDoubleClick(new PointD(x, y), button);
    }
    public virtual void MouseMove(PointD point, Offset offset) {
    }
    public void MouseMove(double x, double y, double dx, double dy) {
      MouseMove(new PointD(x, y), new Offset(dx, dy));
    }
    public virtual void MouseDrag(PointD point, Offset offset, MouseButtons button) {
      if (button == MouseButtons.Left) {
        Move(offset);
      }
    }
    public void MouseDrag(double x, double y, double dx, double dy, MouseButtons button) {
      MouseDrag(new PointD(x, y), new Offset(dx, dy), button);
    }

    public virtual void PreDraw(Graphics graphics) {
    }
    public virtual void Draw(Graphics graphics) {
    }
    public virtual void PostDraw(Graphics graphics) {
    }

    public event EventHandler Update;
    public void EnforceUpdate() {
      if (Update != null) {
        Update(this, new EventArgs());
      }
    }
    protected virtual void OnUpdate() {
      if ((UpdateEnabled) && (Update != null)) {
        Update(this, new EventArgs());
      }
    }
  }
}
