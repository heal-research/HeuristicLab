using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization {
  public partial class LineChart : ViewBase {
    private readonly IChartDataRowsModel model;
    private int maxDataRowCount;
    private Boolean zoomFullView;
    private double minDataValue;
    private double maxDataValue;

    private readonly WorldShape root;
    private readonly XAxis xAxis;

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
      RectangleD clientRectangle = new RectangleD(-1, -1, 1, 1);

      root = new WorldShape(clientRectangle, clientRectangle);

      xAxis = new XAxis();
      root.AddShape(xAxis);

      canvasUI1.MainCanvas.WorldShape = root;
          
      CreateMouseEventListeners();
         
      this.model = model;
      Item = model;
      maxDataRowCount = 0; 
      //The whole data rows are shown per default
      zoomFullView = true;
      minDataValue = Double.PositiveInfinity;
      maxDataValue = Double.NegativeInfinity;
    }

    public void ResetView() {
      zoomFullView = true;
      ZoomToFullView();
      canvasUI1.Invalidate();
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
      if (row.Count > maxDataRowCount)
        maxDataRowCount = row.Count;
      
      InitLineShapes(row);
      InitXAxis();
    }

    private void InitXAxis() {
      int numLabels = 0;

      foreach (IDataRow row in model.Rows) {
        numLabels = Math.Max(numLabels, row.Count);
      }

      xAxis.ClearLabels();

      for (int i = 0; i < numLabels; i++) {
        xAxis.SetLabel(i, i.ToString());
      }
    }

    private void ZoomToFullView() {
      if(!zoomFullView)
        return;
      RectangleD newClippingArea =  new RectangleD(-0.1,
        minDataValue-((maxDataValue-minDataValue)*0.05),
        maxDataRowCount-0.9,
        maxDataValue + ((maxDataValue - minDataValue) * 0.05));
      root.ClippingArea = newClippingArea;
    }

    private void InitLineShapes(IDataRow row) {
      List<LineShape> lineShapes = new List<LineShape>();
      if (row.Count > 0) {
        maxDataValue = Math.Max(row[0], this.maxDataValue);
        minDataValue = Math.Min(row[0], minDataValue);
      }
      for (int i = 1; i < row.Count; i++) {
        LineShape lineShape = new LineShape(i - 1, row[i - 1], i, row[i], 0, row.Color, row.Thickness, row.Style);
        lineShapes.Add(lineShape);
        // TODO each DataRow needs its own WorldShape so Y Axes can be zoomed independently.
        root.AddShape(lineShape);
        maxDataValue = Math.Max(row[i], maxDataValue);
        minDataValue = Math.Min(row[i], minDataValue);
      }

      rowToLineShapes[row] = lineShapes;
      ZoomToFullView();
      canvasUI1.Invalidate();
    }

    private void OnDataRowRemoved(IDataRow row) {
      row.ValueChanged -= OnRowValueChanged;
      row.ValuesChanged -= OnRowValuesChanged;
    }

    private readonly IDictionary<IDataRow, List<LineShape>> rowToLineShapes = new Dictionary<IDataRow, List<LineShape>>();

    // TODO use action parameter
    private void OnRowValueChanged(IDataRow row, double value, int index, Action action) {
      xAxis.SetLabel(index, index.ToString());

      List<LineShape> lineShapes = rowToLineShapes[row];
      maxDataValue = Math.Max(value, maxDataValue);
      minDataValue = Math.Min(value, minDataValue);

      if (index > lineShapes.Count + 1) {
        throw new NotImplementedException();
      }

      // new value was added
      if (index > 0 && index == lineShapes.Count + 1) {
        if (maxDataRowCount < row.Count)
          maxDataRowCount = row.Count;
        LineShape lineShape = new LineShape(index - 1, row[index - 1], index, row[index], 0, row.Color, row.Thickness, row.Style);
        lineShapes.Add(lineShape);
        // TODO each DataRow needs its own WorldShape so Y Axes can be zoomed independently.
        root.AddShape(lineShape);
      }

      // not the first value
      if (index > 0) {
        lineShapes[index - 1].Y2 = value;
      }

      // not the last value
      if (index > 0 && index < row.Count - 1) {
        lineShapes[index].Y1 = value;
      }
      ZoomToFullView();
      canvasUI1.Invalidate();
    }

    // TODO use action parameter
    private void OnRowValuesChanged(IDataRow row, double[] values, int index, Action action) {
      foreach (double value in values) {
        OnRowValueChanged(row, value, index++, action);
      }
    }

    private void OnModelChanged() {
      InitXAxis();
    }

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
    private MouseEventListener zoomListener;

    private void CreateMouseEventListeners() {
      panListener = new MouseEventListener();
      panListener.OnMouseMove += Pan_OnMouseMove;
      panListener.OnMouseUp += Pan_OnMouseUp;

      zoomListener = new MouseEventListener();
      zoomListener.OnMouseMove += Zoom_OnMouseMove;
      zoomListener.OnMouseUp += Zoom_OnMouseUp;
    }

    private RectangleD startClippingArea;

    private void canvasUI1_MouseDown(object sender, MouseEventArgs e) {
      if (ModifierKeys == Keys.Control) {
        zoomListener.StartPoint = e.Location;
        canvasUI1.MouseEventListener = zoomListener;

        r = Rectangle.Empty;
        rectangleShape = new RectangleShape(e.X, e.Y, e.X, e.Y, 1000, Color.Blue);

        root.AddShape(rectangleShape);
      } else {
        panListener.StartPoint = e.Location;
        canvasUI1.MouseEventListener = panListener;

        startClippingArea = root.ClippingArea;
      }
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

      root.ClippingArea = newClippingArea;
      panListener.StartPoint = startPoint;

      zoomFullView = false; //user wants to pan => no full view

      canvasUI1.Invalidate();
    }

    private void Zoom_OnMouseUp(Point startPoint, Point actualPoint) {
      canvasUI1.MouseEventListener = null;

      RectangleD newClippingArea = Transform.ToWorld(r, canvasUI1.ClientRectangle, root.ClippingArea);
      root.ClippingArea = newClippingArea;
      root.RemoveShape(rectangleShape);

      zoomFullView = false; //user wants to pan => no full view

      canvasUI1.Invalidate();
    }

    private Rectangle r;
    private RectangleShape rectangleShape;

    private void Zoom_OnMouseMove(Point startPoint, Point actualPoint) {
      r = new Rectangle();

      if (startPoint.X < actualPoint.X) {
        r.X = startPoint.X;
        r.Width = actualPoint.X - startPoint.X;
      } else {
        r.X = actualPoint.X;
        r.Width = startPoint.X - actualPoint.X;
      }

      if (startPoint.Y < actualPoint.Y) {
        r.Y = startPoint.Y;
        r.Height = actualPoint.Y - startPoint.Y;
      } else {
        r.Y = actualPoint.Y;
        r.Height = startPoint.Y - actualPoint.Y;
      }

      rectangleShape.Rectangle = Transform.ToWorld(r, canvasUI1.ClientRectangle, root.ClippingArea);
      canvasUI1.Invalidate();
    }
  }
}
