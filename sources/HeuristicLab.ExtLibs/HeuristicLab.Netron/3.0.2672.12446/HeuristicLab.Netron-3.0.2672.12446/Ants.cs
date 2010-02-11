using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using Netron.Diagramming.Core;

namespace HeuristicLab.Netron {
  internal static class AntsFactory {
    private static RectAnts mRectangular;
    public readonly static Pen Pen = new Pen(Color.Black, 1f);
    static AntsFactory() {
      Pen.DashStyle = DashStyle.Dash;
    }
    public static IAnts GetAnts(object pars, AntTypes type) {
      switch (type) {
        case AntTypes.Rectangle:
          if (mRectangular == null)
            mRectangular = new RectAnts();
          Point[] points = (Point[])pars;
          mRectangular.Start = points[0];
          mRectangular.End = points[1];
          return mRectangular;
        default:
          return null;
      }
    }

    internal class RectAnts : AbstractAnt {
      public RectAnts(Point s, Point e)
        : this() {
        this.Start = s;
        this.End = e;

      }

      public RectAnts()
        : base() {
        Pen.DashStyle = DashStyle.Dash;
      }

      public override void Paint(Graphics g) {
        if (g == null)
          return;
        g.DrawRectangle(AntsFactory.Pen, Start.X, Start.Y, End.X - Start.X, End.Y - Start.Y);

      }
    }

    internal abstract class AbstractAnt : IAnts {
      private Point mStart;
      public Point Start {
        get {
          return mStart;
        }
        set {
          mStart = value;
        }
      }
      private Point mEnd;
      public Point End {
        get {
          return mEnd;
        }
        set {
          mEnd = value;
        }
      }

      public Rectangle Rectangle {
        get { return new Rectangle(mStart.X, mStart.Y, mEnd.X - mStart.X, mEnd.Y - mStart.Y); }
        set {
          mStart = new Point(value.X, value.Y);
          mEnd = new Point(value.Right, value.Bottom);
        }
      }

      public abstract void Paint(Graphics g);
    }
  }
  internal enum AntTypes {
    Rectangle
  }
}
