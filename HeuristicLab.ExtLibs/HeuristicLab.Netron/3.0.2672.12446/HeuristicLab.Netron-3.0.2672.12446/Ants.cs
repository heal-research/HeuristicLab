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

using System.Drawing;
using System.Drawing.Drawing2D;
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
