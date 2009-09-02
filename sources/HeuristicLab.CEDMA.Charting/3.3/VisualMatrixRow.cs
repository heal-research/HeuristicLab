using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.SparseMatrix;

namespace HeuristicLab.CEDMA.Charting {
  public class VisualMatrixRow : ItemBase {
    private static Random random = new Random();
    private Dictionary<string, object> dict;

    public VisualMatrixRow() {
      this.dict = new Dictionary<string, object>();
      this.visible = true;
      this.selected = false;
      this.xJitter = random.NextDouble() * 2.0 - 1.0;
      this.yJitter = random.NextDouble() * 2.0 - 1.0;
    }

    public VisualMatrixRow(double xJitter, double yJitter) {
      this.dict = new Dictionary<string, object>();
      this.visible = true;
      this.selected = false;
      this.xJitter = xJitter;
      this.yJitter = yJitter;
    }

    public VisualMatrixRow(MatrixRow<string,object> row)
      : this() {
      foreach (KeyValuePair<string, object> value in row.Values)
        dict[value.Key] = value.Value;
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
      if (!dict.ContainsKey(name))
        return null;
      return dict[name];
    }

    public void Set(string name, object value) {
      this.dict[name] = value;
    }

    public IEnumerable<KeyValuePair<string, object>> Values {
      get { return dict; }
    }

    public void ToggleSelected() {
      selected = !selected;
    }

    public string GetToolTipText() {
      StringBuilder b = new StringBuilder();
      foreach (KeyValuePair<string, object> v in dict) {
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
