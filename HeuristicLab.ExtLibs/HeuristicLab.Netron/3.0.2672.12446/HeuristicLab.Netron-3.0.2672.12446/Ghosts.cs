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
  internal static class GhostsFactory {
    private static RectGhost mRectangular;
    private static LineGhost mLine;
    private static EllipticGhost mEllipse;
    private static MultiLineGhost mMultiLine;
    private static CurvedLineGhost mCurvedLine;
    private static PolygonGhost mPolygon;

    private static IView mView;
    public static IView View {
      get { return mView; }
      set { mView = value; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    public static IGhost GetGhost(object pars, GhostTypes type) {
      Point[] points;
      switch (type) {
        case GhostTypes.Rectangle:
          if (mRectangular == null)
            mRectangular = new RectGhost(View);
          points = (Point[])pars;
          mRectangular.Start = points[0];
          mRectangular.End = points[1];
          return mRectangular;
        case GhostTypes.Ellipse:
          if (mEllipse == null)
            mEllipse = new EllipticGhost(View);
          points = (Point[])pars;
          mEllipse.Start = points[0];
          mEllipse.End = points[1];
          return mEllipse;
        case GhostTypes.Line:
          if (mLine == null)
            mLine = new LineGhost(View);
          points = (Point[])pars;
          mLine.Start = points[0];
          mLine.End = points[1];
          return mLine;
        case GhostTypes.MultiLine:
          if (mMultiLine == null)
            mMultiLine = new MultiLineGhost(View);
          points = (Point[])pars;
          mMultiLine.Points = points;
          return mMultiLine;
        case GhostTypes.CurvedLine:
          if (mCurvedLine == null)
            mCurvedLine = new CurvedLineGhost(View);
          points = (Point[])pars;
          mCurvedLine.Points = points;
          return mCurvedLine;
        case GhostTypes.Polygon:
          if (mPolygon == null)
            mPolygon = new PolygonGhost(View);
          points = (Point[])pars;
          mPolygon.Points = points;
          return mPolygon;
        default:
          return null;
      }
    }
  }

  internal class PolygonGhost : MultiLineGhost {
    public PolygonGhost(IView view) : base(view) { }
    public override void Paint(Graphics g) {
      g.Transform = View.ViewMatrix;
      g.DrawPolygon(ArtPalette.GhostPen, Points);
    }
  }

  internal class CurvedLineGhost : MultiLineGhost {
    public CurvedLineGhost(IView view)
      : base(view) { }
    public override void Paint(Graphics g) {
      g.Transform = View.ViewMatrix;
      g.DrawCurve(ArtPalette.GhostPen, Points);
    }
  }

  internal class MultiLineGhost : AbstractGhost {
    private Point[] points;
    public Point[] Points {
      get {
        return points;
      }
      set {
        points = value;
        Start = value[0];
        End = value[value.Length - 1];
      }
    }
    public MultiLineGhost(IView view, Point[] points)
      : base(view) {
      this.points = points;
    }
    public MultiLineGhost(IView view)
      : base(view) {

    }
    public override void Paint(Graphics g) {
      if (g == null)
        return;
      g.Transform = View.ViewMatrix;
      g.DrawLines(ArtPalette.GhostPen, points);
    }
  }

  internal class RectGhost : AbstractGhost {
    public RectGhost(IView view, Point s, Point e)
      : base(view, s, e) { }
    public RectGhost(IView view)
      : base(view) {
    }
    public override void Paint(Graphics g) {
      if (g == null)
        return;
      g.FillRectangle(ArtPalette.GhostBrush, Rectangle);
      g.DrawRectangle(ArtPalette.GhostPen, Rectangle);
    }
  }

  internal class EllipticGhost : AbstractGhost {
    public EllipticGhost(IView view, Point s, Point e)
      : base(view, s, e) {
    }
    public EllipticGhost(IView view)
      : base(view) {
    }
    public override void Paint(Graphics g) {
      if (g == null)
        return;
      g.Transform = View.ViewMatrix;
      g.FillEllipse(ArtPalette.GhostBrush, Rectangle);
      g.DrawEllipse(ArtPalette.GhostPen, Rectangle);
    }
  }

  internal class LineGhost : AbstractGhost {
    public LineGhost(IView view, Point s, Point e)
      : base(view, s, e) {
    }
    public LineGhost(IView view)
      : base(view) {
    }

    public override void Paint(Graphics g) {
      if (g == null)
        return;
      g.Transform = View.ViewMatrix;
      g.DrawLine(ArtPalette.GhostPen, Start, End);
    }
  }

  internal abstract class AbstractGhost : IGhost {
    private Point mStart;
    private Point mEnd;
    private IView mView;
    public IView View {
      get { return mView; }
      set { mView = value; }
    }
    public Rectangle Rectangle {
      get {
        return Rectangle.FromLTRB(Math.Min(mStart.X, mEnd.X), Math.Min(mStart.Y, mEnd.Y), Math.Max(mStart.X, mEnd.X), Math.Max(mStart.Y, mEnd.Y));
      }
      set {
        //was orginally empty
        throw new NotImplementedException();
      }
    }
    public Point Start {
      get {
        return mStart;
      }
      set {
        mStart = value;
      }
    }
    public Point End {
      get {
        return mEnd;
      }
      set {
        mEnd = value;
      }
    }
    protected AbstractGhost(IView view, Point s, Point e)
      : this(view) {
      this.mStart = s;
      this.mEnd = e;
    }
    protected AbstractGhost(IView view) {
      mView = view;
    }
    public abstract void Paint(Graphics g);
  }

  internal enum GhostTypes {
    Rectangle,
    Ellipse,
    Line,
    MultiLine,
    CurvedLine,
    Polygon
  }
}
