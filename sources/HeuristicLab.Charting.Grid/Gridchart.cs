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
using HeuristicLab.Charting;
using System.Drawing;
using System.Collections; 

namespace HeuristicLab.Charting.Grid {
  public class Gridchart : Chart {
    private int rows;
    public int RowCount {
      get { return rows; }
      set { rows = value; }
    }

    private int columns;
    public int ColumnCount {
      get { return columns; }
      set { columns = value; }
    }

    private Dictionary<string, ICell> myCells;
    public ICollection<ICell> Cells {
      get { return myCells.Values; }
    }

    private bool dragDropEnabled;
    public bool DragDropEnabled {
      get { return dragDropEnabled; }
      set {
        if(dragDropEnabled != (bool)value) {
          dragDropEnabled = (bool)value;
          foreach(IPrimitive cell in this.Group.Primitives) {
            if(cell is CellBase) {
              CellBase b = (CellBase)cell;
              b.DragDropEnabled = dragDropEnabled;
            }
          }
        }
      }
    }

    private bool displayEmptyCells;
    public bool DisplayEmptyCells {
      get { return displayEmptyCells; }
      set {
        if(displayEmptyCells != (bool)value) {
          displayEmptyCells = (bool)value;
          foreach(IPrimitive cell in this.Group.Primitives) {
            if (cell is EmptyCell) {
              EmptyCell e = (EmptyCell)cell;
              e.Display = displayEmptyCells;
            } 
          }
          DragDropEnabled = displayEmptyCells; 
        }
      }
    }

    public Gridchart(PointD lowerLeft, PointD upperRight)
      : base(lowerLeft, upperRight) {
      myCells = new Dictionary<string, ICell>();
      Mode = ChartMode.Select;
      displayEmptyCells = true;
      dragDropEnabled = true; 
      this.Group.Add(new Grid(this, lowerLeft, upperRight, 0, 0));
    }

    public Gridchart(double x1, double y1, double x2, double y2)
      : this(new PointD(x1, y1), new PointD(x2, y2)) {
    }

    public void Redim(int rows, int cols) {
      int oldRowCount = RowCount;
      int oldColumnCount = ColumnCount; 
      RowCount = rows;
      ColumnCount = cols;
      ((Grid)Group.Primitives[Group.Primitives.Count - 1]).SetRowsCols(rows, cols);
      if(rows < oldRowCount) {
        for(int i = rows; i < oldRowCount; i++) {
          for(int j = 0; j < oldColumnCount; j++) {
            myCells.Remove(GenerateKey(i, j));
          }
        }
      }
      if(cols < oldColumnCount) {
        for(int i = cols; i < oldColumnCount; i++) {
          for(int j = 0; j < Math.Min(oldRowCount, rows); j++) {
            myCells.Remove(GenerateKey(j, i));
          }
        }
      }

      for(int i = oldRowCount; i < rows; i++) {
        for(int j = 0; j < cols; j++) {
          this[i, j] = new EmptyCell(this);
          if(!DisplayEmptyCells)
            this[i, j].Display = false; 
        }
      }

      for(int i = oldColumnCount; i < cols; i++) {
        for(int j = 0; j < oldRowCount; j++) {
          this[j, i] = new EmptyCell(this);
          if(!DisplayEmptyCells)
            this[i, j].Display = false; 
        }
      }
    }

    private string GenerateKey(int row, int col) {
      return (row.ToString() + ";" + col.ToString()); 
    }

    public ICell this[int row, int col] {
      get {
        ICell cell;
        if(myCells.TryGetValue(GenerateKey(row, col), out cell)) {
          return cell; 
        } else {
          return null; 
        }
      }
      set {
        string key = GenerateKey(row, col);
        if (myCells.ContainsKey(key)) {
          myCells.Remove(key);
        }
        myCells.Add(key, value);
        PointD ll = this.LowerLeft; 
        SizeD s = this.Size;
        double w = s.Width / ColumnCount;
        double h = s.Width / RowCount;
        if ((bool) value.Display) {
          value.RenderCell(new PointD(ll.X + col*w, ll.Y + (RowCount - row - 1)*h), new PointD(ll.X +(col+1)*w, ll.Y + (RowCount - row)*h));
        }
      }
    }

    public override void Render(Graphics graphics, int width, int height) {
      base.Render(graphics, width, height);
    }
  }
}
