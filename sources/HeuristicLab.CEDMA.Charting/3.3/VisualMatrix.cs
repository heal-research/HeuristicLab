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

    public VisualMatrix() {
      this.rows = new List<VisualMatrixRow>();
      this.categoricalVariables = new List<string>();
      this.ordinalVariables = new List<string>();
      this.multiDimensionalCategoricalVariables = new List<string>();
      this.multiDimensionalOrdinalVariables = new List<string>();
      this.categoricalValueIndices = new Dictionary<string, Dictionary<object, double>>();    }

    public VisualMatrix(Matrix matrix, IEnumerable<string> categoricalVariables, IEnumerable<string> ordinalVariables,
      IEnumerable<string> multiDimensionalCategoricalVariables, IEnumerable<string> multiDimensionalOrdinalVariables)
      : this() {
      this.matrix = matrix;
      this.matrix.Changed += new EventHandler(MatrixChanged);

      foreach (MatrixRow row in matrix.GetRows())
        rows.Add(new VisualMatrixRow(row));

      this.categoricalVariables.AddRange(categoricalVariables);
      this.ordinalVariables.AddRange(ordinalVariables);
      this.multiDimensionalCategoricalVariables.AddRange(multiDimensionalCategoricalVariables);
      this.multiDimensionalOrdinalVariables.AddRange(multiDimensionalOrdinalVariables);
    }

    private List<VisualMatrixRow> rows;
    public IEnumerable<VisualMatrixRow> Rows {
      get {
        return rows;
      }
    }

    public void AddRow(VisualMatrixRow row) {
      this.rows.Add(row);
    }

    public void RemoveRow(VisualMatrixRow row) {
      this.rows.Remove(row);
    }

    public void ClearAllRows(VisualMatrixRow row) {
      this.rows.Clear();
    }

    private List<string> categoricalVariables;
    public IEnumerable<string> CategoricalVariables {
      get { return categoricalVariables; }
    }

    public void AddCategoricalVariable(string name) {      
      this.categoricalVariables.Add(name);
    }

    public void RemoveCategoricalVariable(string name) {
      this.categoricalVariables.Remove(name);
    }

    public void ClearCategoricalVariables() {
      this.categoricalVariables.Clear();
    }

    private List<string> ordinalVariables;
    public IEnumerable<string> OrdinalVariables {
      get { return ordinalVariables; }
    }

    public void AddOrdinalVariable(string name) {
      this.ordinalVariables.Add(name);
    }

    public void RemoveOrdinalVariable(string name) {
      this.ordinalVariables.Remove(name);
    }

    public void ClearOrdinalVariables() {
      this.ordinalVariables.Clear();
    }

    private List<string> multiDimensionalOrdinalVariables;
    public IEnumerable<string> MultiDimensionalOrdinalVariables {
      get { return multiDimensionalOrdinalVariables; }
    }

    public void AddMultiDimensionalOrdinalVariable(string name) {
      this.multiDimensionalOrdinalVariables.Add(name);
    }

    public void RemoveMultiDimensionalOrdinalVariable(string name) {
      this.multiDimensionalOrdinalVariables.Remove(name);
    }

    public void ClearMultiDimensionalOrdinalVariables() {
      this.multiDimensionalOrdinalVariables.Clear();
    }

    private List<string> multiDimensionalCategoricalVariables;
    public IEnumerable<string> MultiDimensionalCategoricalVariables {
      get { return multiDimensionalCategoricalVariables; }
    }

    public void AddMultiDimensionalCategoricalVariable(string name) {
      this.multiDimensionalCategoricalVariables.Add(name);
    }

    public void RemoveMultiDimensionalCategoricalVariable(string name) {
      this.multiDimensionalCategoricalVariables.Remove(name);
    }

    public void ClearMultiDimensionalCategoricalVariables() {
      this.multiDimensionalCategoricalVariables.Clear();
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
      get { return CategoricalVariables.Concat(OrdinalVariables); }
    }
  }
}
