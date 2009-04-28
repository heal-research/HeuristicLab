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
  public class Chart : IChart {
    private PointD originalLowerLeft;
    private PointD originalUpperRight;
    private List<Offset> zoomHistory;

    private ChartMode myMode;
    public virtual ChartMode Mode {
      get { return myMode; }
      set {
        myMode = value;
        OnUpdate();
      }
    }

    private PointD myLowerLeft;
    public virtual PointD LowerLeft {
      get { return myLowerLeft; }
    }
    private PointD myUpperRight;
    public virtual PointD UpperRight {
      get { return myUpperRight; }
    }
    public virtual SizeD Size {
      get { return new SizeD(UpperRight.X - LowerLeft.X, UpperRight.Y - LowerLeft.Y); }
    }
    private Size mySizeInPixels;
    public Size SizeInPixels {
      get { return mySizeInPixels; }
    }
    public SizeD PixelToWorldRatio {
      get { return new SizeD(Size.Width / SizeInPixels.Width, Size.Height / SizeInPixels.Height); }
    }
    public SizeD WorldToPixelRatio {
      get { return new SizeD(SizeInPixels.Width / Size.Width, SizeInPixels.Height / Size.Height); }
    }

    private IGroup myGroup;
    public IGroup Group {
      get { return myGroup; }
    }

    private bool myUpdateEnabled;
    public bool UpdateEnabled {
      get { return myUpdateEnabled; }
      set { myUpdateEnabled = value; }
    }

    public Chart(PointD lowerLeft, PointD upperRight) {
      myUpdateEnabled = false;
      zoomHistory = new List<Offset>();
      myMode = ChartMode.Move;
      SetPosition(lowerLeft, upperRight);
      mySizeInPixels = new Size(100, 100);
      myGroup = new Group(this);
      Group.Update += new EventHandler(Group_Update);
      myUpdateEnabled = true;
    }
    public Chart(double x1, double y1, double x2, double y2)
      : this(new PointD(x1, y1), new PointD(x2, y2)) {
    }

    public PointD TransformPixelToWorld(Point point) {
      double x = LowerLeft.X + point.X * PixelToWorldRatio.Width;
      double y = LowerLeft.Y + (SizeInPixels.Height - point.Y) * PixelToWorldRatio.Height;
      return new PointD(x, y);
    }
    public SizeD TransformPixelToWorld(Size size) {
      double width = size.Width * PixelToWorldRatio.Width;
      double height = size.Height * PixelToWorldRatio.Height;
      return new SizeD(width, height);
    }
    public Point TransformWorldToPixel(PointD point) {
      int x = (int)((point.X - LowerLeft.X) * WorldToPixelRatio.Width);
      int y = (int)((UpperRight.Y - point.Y) * WorldToPixelRatio.Height);
      return new Point(x, y);
    }
    public Size TransformWorldToPixel(SizeD size) {
      int width = (int)(size.Width * WorldToPixelRatio.Width);
      int height = (int)(size.Height * WorldToPixelRatio.Height);
      return new Size(width, height);
    }

    public virtual void SetPosition(PointD lowerLeft, PointD upperRight) {
      if ((lowerLeft.X >= upperRight.X) || (lowerLeft.Y >= upperRight.Y))
        throw new ArgumentException("Lower left point is greater or equal than upper right point");

      originalLowerLeft = lowerLeft;
      originalUpperRight = upperRight;
      if (zoomHistory != null) zoomHistory.Clear();
      myLowerLeft = lowerLeft;
      myUpperRight = upperRight;
      OnUpdate();
    }
    public void SetPosition(double x1, double y1, double x2, double y2) {
      SetPosition(new PointD(x1, y1), new PointD(x2, y2));
    }
    public virtual void Move(Offset delta) {
      myLowerLeft += delta;
      myUpperRight += delta;
      OnUpdate();
    }
    public void Move(double dx, double dy) {
      Move(new Offset(dx, dy));
    }

    public virtual void ZoomIn(PointD lowerLeft, PointD upperRight) {
      zoomHistory.Insert(0, LowerLeft - lowerLeft);
      zoomHistory.Insert(0, UpperRight - upperRight);
      myLowerLeft = lowerLeft;
      myUpperRight = upperRight;
      OnUpdate();
    }
    public void ZoomIn(double x1, double y1, double x2, double y2) {
      ZoomIn(new PointD(x1, y1), new PointD(x2, y2));
    }
    public virtual void ZoomIn(Point lowerLeft, Point upperRight) {
      ZoomIn(TransformPixelToWorld(lowerLeft), TransformPixelToWorld(upperRight));
    }
    public void ZoomIn(int x1, int y1, int x2, int y2) {
      ZoomIn(new Point(x1, y1), new Point(x2, y2));
    }
    public virtual void ZoomOut() {
      if (zoomHistory.Count > 0) {
        Offset upperRight = zoomHistory[0];
        zoomHistory.RemoveAt(0);
        Offset lowerLeft = zoomHistory[0];
        zoomHistory.RemoveAt(0);
        myLowerLeft = LowerLeft + lowerLeft;
        myUpperRight = UpperRight + upperRight;
        OnUpdate();
      } else {
        myLowerLeft.X -= Size.Width / 4;
        myLowerLeft.Y -= Size.Height / 4;
        myUpperRight.X += Size.Width / 4;
        myUpperRight.Y += Size.Height / 4;
        OnUpdate();
      }
    }
    public virtual void Unzoom() {
      SetPosition(originalLowerLeft, originalUpperRight);
    }

    public virtual IPrimitive GetPrimitive(Point point) {
      return Group.GetPrimitive(TransformPixelToWorld(point));
    }
    public IPrimitive GetPrimitive(int x, int y) {
      return GetPrimitive(new Point(x, y));
    }
    public virtual IList<IPrimitive> GetAllPrimitives(Point point) {
      return Group.GetAllPrimitives(TransformPixelToWorld(point));
    }
    public IList<IPrimitive> GetAllPrimitives(int x, int y) {
      return GetAllPrimitives(new Point(x, y));
    }

    public virtual Cursor GetCursor(Point point) {
      Cursor cursor = Group.GetCursor(TransformPixelToWorld(point));
      if (cursor != null) return cursor;

      if (Mode == ChartMode.Move) return Cursors.Hand;
      if (Mode == ChartMode.Select) return Cursors.Arrow;
      if (Mode == ChartMode.Zoom) return Cursors.Arrow;
      return Cursors.Default;
    }
    public Cursor GetCursor(int x, int y) {
      return GetCursor(new Point(x, y));
    }
    public virtual string GetToolTipText(Point point) {
      return Group.GetToolTipText(TransformPixelToWorld(point));
    }
    public string GetToolTipText(int x, int y) {
      return GetToolTipText(new Point(x, y));
    }

    public virtual void MouseClick(Point point, MouseButtons button) {
      if (Mode == ChartMode.Select) {
        if (button == MouseButtons.Left) {
          IPrimitive clicked = GetPrimitive(point);
          UpdateEnabled = false;
          foreach (IPrimitive primitive in Group.SelectedPrimitives)
            primitive.Selected = false;
          UpdateEnabled = true;
          if (clicked != null) {
            PointD p = TransformPixelToWorld(point);
            clicked.MouseClick(p, button);
          } else OnUpdate();
        }
      } else if (Mode == ChartMode.Move) {
        if (button == MouseButtons.Middle) {
          PointD center = LowerLeft + new Offset(Size.Width / 2, Size.Height / 2);
          Move(TransformPixelToWorld(point) - center);
        }
      }
    }
    public void MouseClick(int x, int y, MouseButtons button) {
      MouseClick(new Point(x, y), button);
    }
    public virtual void MouseDoubleClick(Point point, MouseButtons button) {
    }
    public void MouseDoubleClick(int x, int y, MouseButtons button) {
      MouseDoubleClick(new Point(x, y), button);
    }
    public virtual void MouseMove(Point start, Point end) {
    }
    public void MouseMove(int x1, int y1, int x2, int y2) {
      MouseMove(new Point(x1, y1), new Point(x2, y2));
    }
    public virtual void MouseDrag(Point start, Point end, MouseButtons button) {
      PointD p1 = TransformPixelToWorld(start);
      PointD p2 = TransformPixelToWorld(end);
      Offset offset = p2 - p1;
      if (Mode == ChartMode.Move) {
        Move(-1 * offset.DX, -1 * offset.DY);
      } else if (Mode == ChartMode.Select) {
        if (button == MouseButtons.Left) {
          IList<IPrimitive> selected = Group.SelectedPrimitives;
          UpdateEnabled = false;
          foreach (IPrimitive primitive in selected)
            primitive.MouseDrag(p1, offset, button);
          UpdateEnabled = true;
          OnUpdate();
        }
      }
    }
    public void MouseDrag(int x1, int y1, int x2, int y2, MouseButtons button) {
      MouseDrag(new Point(x1, y1), new Point(x2, y2), button);
    }

    public virtual void Render(Graphics graphics, int width, int height) {
      mySizeInPixels = new Size(width, height);
      Group.PreDraw(graphics);
      Group.Draw(graphics);
      Group.PostDraw(graphics);
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
    private void Group_Update(object sender, EventArgs e) {
      OnUpdate();
    }
  }
}
