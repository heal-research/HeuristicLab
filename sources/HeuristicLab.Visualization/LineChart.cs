using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
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
      if (model == null) {
        throw new NullReferenceException("Model cannot be null.");
      }

      //TODO: correct Rectangle to fit
      RectangleD clientRectangle = new RectangleD(-1, -1, 11, 11);
      canvasUI1.MainCanvas.WorldShape = new WorldShape(clientRectangle, clientRectangle);

      CreateMouseEventListeners();

      this.model = model;
      Item = (IItem)model;
    }

    #region Add-/RemoveItemEvents

    protected override void AddItemEvents() {
      base.AddItemEvents();

      model.DataRowAdded += OnDataRowAdded;
      model.DataRowRemoved += OnDataRowRemoved;
      model.ModelChanged += OnModelChanged;

      foreach (IDataRow row in model.Rows) {
        OnDataRowAdded(row);
      }
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
        LineShape lineShape = new LineShape(i - 1, row[i - 1], i, row[i], 0, row.Color, row.Thickness, row.Style);
        lineShapes.Add(lineShape);
        // TODO each DataRow needs its own WorldShape so Y Axes can be zoomed independently.
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

    // TODO use action parameter
    private void OnRowValueChanged(IDataRow row, double value, int index, Action action) {
      List<LineShape> lineShapes = rowToLineShapes[row];

      if (index > lineShapes.Count + 1) {
        throw new NotImplementedException();
      }

      // new value was added
      if (index > 0 && index == lineShapes.Count + 1) {
        LineShape lineShape = new LineShape(index - 1, row[index - 1], index, row[index], 0, row.Color, row.Thickness, row.Style);
        lineShapes.Add(lineShape);
        // TODO each DataRow needs its own WorldShape so Y Axes can be zoomed independently.
        canvasUI1.MainCanvas.WorldShape.AddShape(lineShape);
      }

      // not the first value
      if (index > 0) {
        lineShapes[index - 1].Y2 = value;
      }

      // not the last value
      if (index > 0 && index < row.Count - 1) {
        lineShapes[index].Y1 = value;
      }

      canvasUI1.Invalidate();
    }

    // TODO use action parameter
    private void OnRowValuesChanged(IDataRow row, double[] values, int index, Action action) {
      foreach (double value in values) {
        OnRowValueChanged(row, value, index++, action);
      }
    }

    private void OnModelChanged() {}

    #endregion

    #region Begin-/EndUpdate

    private int beginUpdateCount = 0;

    public void BeginUpdate() {
      beginUpdateCount++;
    }

    public void EndUpdate() {
      if (beginUpdateCount == 0) {
        throw new InvalidOperationException("Too many EndUpdates.");
      }

      beginUpdateCount--;

      if (beginUpdateCount == 0) {
        Invalidate();
      }
    }

    #endregion

    private MouseEventListener panListener;

    private void CreateMouseEventListeners() {
      panListener = new MouseEventListener();
      panListener.OnMouseMove += Pan_OnMouseMove;
      panListener.OnMouseUp += Pan_OnMouseUp;
    }


    private RectangleD startClippingArea;

    private void canvasUI1_MouseDown(object sender, MouseEventArgs e) {
      panListener.StartPoint = e.Location;
      canvasUI1.MouseEventListener = panListener;

      startClippingArea = canvasUI1.MainCanvas.WorldShape.ClippingArea;
    }

    private void Pan_OnMouseUp(Point startPoint, Point actualPoint) {
       canvasUI1.MouseEventListener = null;
    }

    private void Pan_OnMouseMove(Point startPoint, Point actualPoint) {
      Rectangle viewPort = canvasUI1.ClientRectangle;

      PointD worldStartPoint = Transform.ToWorld(startPoint, viewPort, startClippingArea);
      PointD worldActualPoint = Transform.ToWorld(actualPoint, viewPort, startClippingArea);

      double xDiff = worldActualPoint.X - worldStartPoint.X;
      double yDiff = worldActualPoint.Y - worldStartPoint.Y;

      RectangleD newClippingArea = new RectangleD();
      newClippingArea.X1 = startClippingArea.X1 - xDiff;
      newClippingArea.X2 = startClippingArea.X2 - xDiff;
      newClippingArea.Y1 = startClippingArea.Y1 - yDiff;
      newClippingArea.Y2 = startClippingArea.Y2 - yDiff;

      canvasUI1.MainCanvas.WorldShape.ClippingArea = newClippingArea;
      panListener.StartPoint = startPoint;

      canvasUI1.Invalidate();
    }
  }
}