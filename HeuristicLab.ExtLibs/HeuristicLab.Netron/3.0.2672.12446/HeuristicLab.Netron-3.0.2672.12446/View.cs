#region License Information
//This end-user license agreement applies to the following software;

//The Netron Diagramming Library
//Cobalt.IDE
//Xeon webserver
//Neon UI Library

//Copyright (C) 2007, Francois M.Vanderseypen, The Netron Project & The Orbifold

//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either version 2
//of the License, or (at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA


//http://www.fsf.org/licensing/licenses/gpl.html

//http://www.fsf.org/licensing/licenses/gpl-faq.html
#endregion

using System;
using System.Drawing;
using Netron.Diagramming.Core;

namespace HeuristicLab.Netron {
  public class View : ViewBase {

    public View(IDiagramControl control)
      : base(control) {
      control.Document.Model.Selection.OnNewSelection += new EventHandler(Selection_OnNewSelection);
      // this.Model.OnEntityAdded += new EventHandler<EntityEventArgs>(Model_OnEntityAdded);
      this.HorizontalRuler.Visible = true;
      this.VerticalRuler.Visible = true;
      GhostsFactory.View = this;
    }

    void Selection_OnNewSelection(object sender, EventArgs e) {
      ShowTracker();
    }

    public override void Paint(Graphics g) {
      base.Paint(g);
      //Rectangle rectangle = WorkArea;
      //g.SetClip(WorkArea);
      g.Transform = ViewMatrix;
      //draw the ghost and ants on top of the diagram
      if (Ants != null)
        Ants.Paint(g);
      if (Ghost != null)
        Ghost.Paint(g);
      if (Tracker != null)
        Tracker.Paint(g);

      g.Transform.Reset();
      //g.PageUnit = GraphicsUnit.Pixel;
      //g.PageScale = 1.0F;
    }

    protected virtual void InvalidateGhostArea() {
      if (Ghost != null) {
        Rectangle area = Ghost.Rectangle;
        // Inflate it a little so we make sure to get everything.
        area.Inflate(20, 20);
        this.Invalidate(area);
      }
    }

    protected virtual void InvalidateTrackerArea() {
      if (Tracker != null) {
        Rectangle area = Tracker.Rectangle;
        // Inflate it a little so we make sure to get everything.
        area.Inflate(20, 20);
        this.Invalidate(area);
      }
    }

    protected virtual void InvalidateAntsArea() {
      if (Ants != null) {
        Rectangle area = Ants.Rectangle;
        // Inflate it a little so we make sure to get everything.
        area.Inflate(20, 20);
        this.Invalidate(area);
      }
    }

    public override void PaintGhostEllipse(
        Point ltPoint,
        Point rbPoint) {
      // Refresh the old ghost area if needed.
      this.InvalidateGhostArea();

      Ghost = GhostsFactory.GetGhost(
          new Point[] { ltPoint, rbPoint },
              GhostTypes.Ellipse);
    }
    public override void PaintGhostRectangle(
        Point ltPoint,
        Point rbPoint) {
      // Refresh the old ghost area if needed.
      this.InvalidateGhostArea();

      Ghost = GhostsFactory.GetGhost(
          new Point[] { ltPoint, rbPoint },
          GhostTypes.Rectangle);
    }
    public override void PaintAntsRectangle(
        Point ltPoint,
        Point rbPoint) {
      // Refresh the old area if needed.
      this.InvalidateAntsArea();
      Ants = AntsFactory.GetAnts(
          new Point[] { ltPoint, rbPoint },
          AntTypes.Rectangle);
    }
    public override void PaintGhostLine(Point ltPoint, Point rbPoint) {
      // Refresh the old ghost area if needed.
      this.InvalidateGhostArea();

      Ghost = GhostsFactory.GetGhost(
          new Point[] { ltPoint, rbPoint },
          GhostTypes.Line);
    }
    public override void PaintGhostLine(
        MultiPointType curveType,
        Point[] points) {
      // Refresh the old ghost area if needed.
      this.InvalidateGhostArea();

      switch (curveType) {
        case MultiPointType.Straight:
          Ghost = GhostsFactory.GetGhost(points, GhostTypes.MultiLine);
          break;
        case MultiPointType.Polygon:
          Ghost = GhostsFactory.GetGhost(points, GhostTypes.Polygon);
          break;
        case MultiPointType.Curve:
          Ghost = GhostsFactory.GetGhost(points, GhostTypes.CurvedLine);
          break;

      }
    }

    public override void PaintTracker(Rectangle rectangle, bool showHandles) {
      // Refresh the old area if needed.
      this.InvalidateTrackerArea();
      Tracker = TrackerFactory.GetTracker(rectangle, TrackerTypes.Default, showHandles);
      rectangle.Inflate(20, 20);
      this.Invalidate(rectangle);
    }

    #region Tracker

    private enum TrackerTypes {
      Default
    }

    private class TrackerFactory {

      private static ITracker defTracker;

      public static ITracker GetTracker(Rectangle rectangle, TrackerTypes type, bool showHandles) {
        switch (type) {
          case TrackerTypes.Default:
            if (defTracker == null) defTracker = new DefaultTracker();
            defTracker.Transform(rectangle);
            defTracker.ShowHandles = showHandles;
            return defTracker;
          default:
            return null;
        }

      }
    }

    private class DefaultTracker : TrackerBase {
      private const int gripSize = 4;
      private const int hitSize = 6;
      float mx, my, sx, sy;

      public DefaultTracker(Rectangle rectangle)
        : base(rectangle) {
      }

      public DefaultTracker()
        : base() { }

      public override void Transform(Rectangle rectangle) {
        this.Rectangle = rectangle;
      }

      public override void Paint(Graphics g) {
        //the main rectangle
        g.DrawRectangle(ArtPalette.TrackerPen, Rectangle);
        #region Recalculate the size and location of the grips
        mx = Rectangle.X + Rectangle.Width / 2;
        my = Rectangle.Y + Rectangle.Height / 2;
        sx = Rectangle.Width / 2;
        sy = Rectangle.Height / 2;
        #endregion
        #region draw the grips
        if (!ShowHandles) return;

        for (int x = -1; x <= 1; x++) {
          for (int y = -1; y <= 1; y++) {
            if (x != 0 || y != 0) //not the middle one
                        {
              g.FillRectangle(ArtPalette.GripBrush, mx + x * sx - gripSize / 2, my + y * sy - gripSize / 2, gripSize, gripSize);
              g.DrawRectangle(ArtPalette.BlackPen, mx + x * sx - gripSize / 2, my + y * sy - gripSize / 2, gripSize, gripSize);
            }
          }
        }
        #endregion
      }

      public override Point Hit(Point p) {
        //no need to test if the handles are not shown
        if (!ShowHandles) return Point.Empty;

        for (int x = -1; x <= +1; x++)
          for (int y = -1; y <= +1; y++)
            if ((x != 0) || (y != 0)) {
              if (new RectangleF(mx + x * sx - hitSize / 2, my + y * sy - hitSize / 2, hitSize, hitSize).Contains(p))
                return new Point(x, y);
            }
        return Point.Empty;
      }
    }
    #endregion
  }
}
