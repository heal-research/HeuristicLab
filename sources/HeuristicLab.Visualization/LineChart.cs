using System;
using System.Collections.Generic;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization {
  public partial class LineChart : ViewBase {
    private readonly IChartDataRowsModel model;

    /// <summary>
    /// This constructor shouldn't be called. Only required for the designer.
    /// </summary>
    public LineChart() {
      InitializeComponent();
    }

    /// <summary>
    /// Initializes the chart.
    /// </summary>
    /// <param name="model">Referenz to the model, for data</param>
    public LineChart(IChartDataRowsModel model) : this() {
      if (model == null)
        throw new NullReferenceException("Model cannot be null.");

      this.model = model;
      this.Item = (IItem)model;

      //TODO: correct Rectangle to fit
      RectangleD clientRectangle = new RectangleD(-1, -1, 11, 11);
      canvasUI1.MainCanvas.WorldShape = new WorldShape(clientRectangle, clientRectangle);
    }

    #region Add-/RemoveItemEvents

    protected override void AddItemEvents() {
      base.AddItemEvents();

      model.DataRowAdded += OnDataRowAdded;
      model.DataRowRemoved += OnDataRowRemoved;
      model.ModelChanged += OnModelChanged;
    }

    protected override void RemoveItemEvents() {
      base.RemoveItemEvents();

      model.DataRowAdded -= OnDataRowAdded;
      model.DataRowRemoved -= OnDataRowRemoved;
      model.ModelChanged -= OnModelChanged;
    }

    private void OnDataRowAdded(IDataRow row) {
      row.ValueChanged += OnRowValueChanged;
      row.ValuesChanged += OnRowValuesChanged;

      InitShapes(row);
    }

    private void InitShapes(IDataRow row) {
      List<LineShape> lineShapes = new List<LineShape>();

      for (int i = 1; i < row.Count; i++) {
        LineShape lineShape = new LineShape(i - 1, row[i - 1], i, row[i], 0, row.Color);
        lineShapes.Add(lineShape);
        canvasUI1.MainCanvas.WorldShape.AddShape(lineShape);
      }

      rowToLineShapes[row] = lineShapes;

      canvasUI1.Invalidate();
    }

    private void OnDataRowRemoved(IDataRow row) {
      row.ValueChanged -= OnRowValueChanged;
      row.ValuesChanged -= OnRowValuesChanged;
    }

    private readonly IDictionary<IDataRow, List<LineShape>> rowToLineShapes = new Dictionary<IDataRow, List<LineShape>>();

    private void OnRowValueChanged(IDataRow row, double value, int index) {
      List<LineShape> lineShapes = rowToLineShapes[row];

      if (index > lineShapes.Count+1)
        throw new NotImplementedException();

      // new value was added
      if (index > 0 && index == lineShapes.Count+1) {
        LineShape lineShape = new LineShape(index - 1, row[index - 1], index, row[index], 0, row.Color);
        lineShapes.Add(lineShape);
        canvasUI1.MainCanvas.WorldShape.AddShape(lineShape);
      }

      // not the first value
      if (index > 0)
        lineShapes[index-1].Y2 = value;

      // not the last value
      if (index > 0 && index < row.Count)
        lineShapes[index].Y1 = value;

      canvasUI1.Invalidate();
    }

    private void OnRowValuesChanged(IDataRow row, double[] values, int index) {
      foreach (double value in values) {
        OnRowValueChanged(row, value, index++);
      }
    }

    private void OnModelChanged() {
    }

    #endregion

    #region Begin-/EndUpdate

    private int beginUpdateCount = 0;

    public void BeginUpdate() {
      beginUpdateCount++;
    }

    public void EndUpdate() {
      if (beginUpdateCount == 0)
        throw new InvalidOperationException("Too many EndUpdates.");

      beginUpdateCount--;

      if (beginUpdateCount == 0)
        Invalidate();
    }

    #endregion
  }
}