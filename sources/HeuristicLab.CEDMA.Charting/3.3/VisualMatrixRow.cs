using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.SparseMatrix;

namespace HeuristicLab.CEDMA.Charting {
  public class VisualMatrixRow {
    private MatrixRow row;
    private static Random random = new Random();

    public VisualMatrixRow(MatrixRow row) {
      this.row = row;
      this.visible = true;
      this.selected = false;
      this.xJitter = random.NextDouble() * 2.0 - 1.0;
      this.yJitter = random.NextDouble() * 2.0 - 1.0;
    }

    private bool visible;
    public bool Visible {
      get { return this.visible; }
      set { this.visible = value; }
    }

    private bool selected;
    public bool Selected {
      get { return this.selected; }
      set { this.selected = value; }
    }

    private double xJitter;
    public double XJitter {
      get { return xJitter; }
    }

    private double yJitter;
    public double YJitter {
      get { return yJitter; }
    }

    public object Get(string name) {
      return row.Get(name);
    }

    public void ToggleSelected() {
      selected = !selected;
    }

    public string GetToolTipText() {
      StringBuilder b = new StringBuilder();
      foreach (KeyValuePair<string, object> v in row.Values) {
        if (v.Value is string || v.Value is double || v.Value is int) {
          string val = v.Value.ToString();
          if (val.Length > 40) val = val.Substring(0, 38) + "...";
          b.Append(v.Key).Append(" = ").Append(val).AppendLine();
        }
      }
      return b.ToString();
    }
  }
}
