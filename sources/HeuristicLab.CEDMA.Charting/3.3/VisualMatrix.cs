using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.SparseMatrix;

namespace HeuristicLab.CEDMA.Charting {
  public class VisualMatrix : ItemBase {
    private Dictionary<string, Dictionary<object, double>> categoricalValueIndices;
    private Matrix matrix;

    public VisualMatrix(Matrix matrix, string[] categoricalVariables, string[] ordinalVariables,
      string[] multiDimensionalCategoricalVariables,string[] multiDimensionalOrdinalVariables) {
      this.matrix = matrix;
      this.rows = new List<VisualMatrixRow>();
      foreach(MatrixRow row in matrix.GetRows())
        rows.Add(new VisualMatrixRow(row));

      this.categoricalVariables = categoricalVariables;
      this.ordinalVariables = ordinalVariables;
      this.multiDimensionalOrdinalVariables = multiDimensionalOrdinalVariables;
      this.multiDimensionalCategoricalVariables = multiDimensionalCategoricalVariables;
      this.categoricalValueIndices = new Dictionary<string, Dictionary<object, double>>();

      matrix.Changed += new EventHandler(MatrixChanged);
    }

    private List<VisualMatrixRow> rows;
    public IEnumerable<VisualMatrixRow> Rows {
      get {
        return rows;
      }
    }

    private string[] categoricalVariables;
    public string[] CategoricalVariables {
      get { return categoricalVariables; }
    }

    private string[] ordinalVariables;
    public string[] OrdinalVariables {
      get { return ordinalVariables; }
    }

    private string[] multiDimensionalOrdinalVariables;
    public string[] MultiDimensionalOrdinalVariables {
      get { return multiDimensionalOrdinalVariables; }
    }

    private string[] multiDimensionalCategoricalVariables;
    public string[] MultiDimensionalCategoricalVariables {
      get { return multiDimensionalCategoricalVariables; }
    }
    
    public double IndexOfCategoricalValue(string variable, object value) {
      if (value == null) return double.NaN;
      Dictionary<object, double> valueToIndexMap;
      if (categoricalValueIndices.ContainsKey(variable)) {
        valueToIndexMap = categoricalValueIndices[variable];
      } else {
        valueToIndexMap = new Dictionary<object, double>();
        categoricalValueIndices[variable] = valueToIndexMap;
      }
      if (!valueToIndexMap.ContainsKey(value)) {
        if (valueToIndexMap.Values.Count == 0) valueToIndexMap[value] = 1.0;
        else valueToIndexMap[value] = 1.0 + valueToIndexMap.Values.Max();
      }
      return valueToIndexMap[value];
    }

    private void MatrixChanged(object sender, EventArgs e) {
      this.FireChanged();
    }

     public IEnumerable<string> Attributes {
      get {return CategoricalVariables.Concat(OrdinalVariables);}
    }
  }
}
