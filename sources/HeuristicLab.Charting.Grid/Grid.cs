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

namespace HeuristicLab.Charting.Grid {
  public class Grid : Group {
    private int rows;
    public int RowCount {
      get { return rows; }
    }

    private int columns;
    public int ColumnCount {
      get { return columns; }
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

    private void UpdateLines() {
      if (Primitives.Count > 0) {
        this.Clear(); 
      }
      if ((rows == 0) || (columns == 0)) {
        return; 
      }
      double width = Size.Width / ColumnCount;
      double height = Size.Height / RowCount;
      PointD ll = LowerLeft;
      PointD ur = UpperRight;
      for(int col = 0; col <= ColumnCount; col++) {
        ur.X = ll.X;
        this.Add(new Line(Chart, ll, ur, this.Pen));
        ll.X += width;
      }
      ll = LowerLeft;
      ur = UpperRight;
      for(int row = 0; row <= RowCount; row++) {
        ur.Y = ll.Y;
        this.Add(new Line(Chart, ll, ur, this.Pen));
        ll.Y += height;
      }
    }

    public Grid(IChart chart, PointD lowerLeft, PointD upperRight, int rows, int cols) : base(chart) {
      SetPosition(lowerLeft, upperRight);
      this.rows = rows;
      this.columns = cols;
      UpdateLines(); 
      // UpdateEnabled = false; // vielleicht doch besser net? 
    }

    public virtual void SetPosition(PointD lowerLeft, PointD upperRight) {
      if((lowerLeft.X > upperRight.X) || (lowerLeft.Y > upperRight.Y))
        throw new ArgumentException("Lower left point is greater than upper right point");
      myLowerLeft = lowerLeft;
      myUpperRight = upperRight;
      UpdateLines(); 
      OnUpdate();
    }

    public virtual void SetRowsCols(int rows, int columns) {
      this.rows = rows;
      this.columns = columns;
      UpdateLines();
      OnUpdate(); 
    }

    public override void Move(Offset delta) {
      // do nothing
    }

    public override bool ContainsPoint(PointD point) {
      // throw new Exception("The method or operation is not implemented.");
      return false; 
    }
  }
}
