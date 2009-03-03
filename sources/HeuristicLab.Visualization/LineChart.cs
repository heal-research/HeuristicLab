using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Visualization.Legend;
using HeuristicLab.Visualization.Options;

namespace HeuristicLab.Visualization {
  public partial class LineChart : ViewBase {
    internal class LinesShape : WorldShape {}
    private readonly IChartDataRowsModel model;
    private readonly Canvas canvas;

    private int maxDataRowCount;
    private Boolean zoomFullView;
    private double minDataValue;
    private double maxDataValue;
  

    private readonly TextShape titleShape;
    private readonly LinesShape linesShape;
    private readonly LegendShape legendShape;

    private readonly XAxis xAxis;
    private readonly YAxis yAxis;
    private readonly Grid grid;

    private readonly WorldShape berni;
    private readonly RectangleShape mousePointer;

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

    
      canvas = canvasUI.Canvas;

      grid = new Grid();
      canvas.AddShape(grid);

      linesShape = new LinesShape();
      canvas.AddShape(linesShape);

      xAxis = new XAxis();
      canvas.AddShape(xAxis);

      yAxis = new YAxis();
      canvas.AddShape(yAxis);

      titleShape = new TextShape(0, 0, model.Title, 15);
      canvas.AddShape(titleShape);

      //  horizontalLineShape = new HorizontalLineShape(this.maxDataValue, Color.Yellow, 4, DrawingStyle.Solid);
      //  root.AddShape(horizontalLineShape);

      legendShape = new LegendShape();
      canvas.AddShape(legendShape);

      berni = new WorldShape();
      canvas.AddShape(berni);

      mousePointer = new RectangleShape(10, 10, 20, 20, Color.Black);
      berni.AddShape(mousePointer);

      maxDataRowCount = 0;
      this.model = model;
      Item = model;

      UpdateLayout();
      canvasUI.Resize += delegate { UpdateLayout(); };

      //The whole data rows are shown per default
      ResetView();
    }

    /// <summary>
    /// Layout management - arranges the inner shapes.
    /// </summary>
    private void UpdateLayout() {
      titleShape.X = 10;
      titleShape.Y = canvasUI.Height - 10;

      const int yAxisWidth = 100;
      const int xAxisHeight = 20;

      linesShape.BoundingBox = new RectangleD(yAxisWidth,
                                              xAxisHeight,
                                              canvasUI.Width,
                                              canvasUI.Height);

      berni.BoundingBox = linesShape.BoundingBox;
      berni.ClippingArea = new RectangleD(0, 0, berni.BoundingBox.Width, berni.BoundingBox.Height);

      grid.BoundingBox = linesShape.BoundingBox;


      yAxis.BoundingBox = new RectangleD(0,
                                         linesShape.BoundingBox.Y1,
                                         linesShape.BoundingBox.X1,
                                         linesShape.BoundingBox.Y2);

      xAxis.BoundingBox = new RectangleD(linesShape.BoundingBox.X1,
                                         0,
                                         linesShape.BoundingBox.X2,
                                         linesShape.BoundingBox.Y1);

      legendShape.BoundingBox = new RectangleD(10, 10, 110, canvasUI.Height - 50);
      legendShape.ClippingArea = new RectangleD(0, 0, legendShape.BoundingBox.Width,
                                                legendShape.BoundingBox.Height);
    }

    public void ResetView() {
      zoomFullView = true;
      ZoomToFullView();

      canvasUI.Invalidate();
    }

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
      var optionsdlg = new OptionsDialog(this.model);
      optionsdlg.ShowDialog(this);
    }

    public void OnDataRowChanged(IDataRow row) {
      foreach (LineShape ls in rowToLineShapes[row]) {
        ls.LSColor = row.Color;
        ls.LSThickness = row.Thickness;
        ls.LSDrawingStyle = row.Style;
      }
      canvasUI.Invalidate();
    }

    #region Add-/RemoveItemEvents

    private readonly IDictionary<IDataRow, List<LineShape>> rowToLineShapes =
      new Dictionary<IDataRow, List<LineShape>>();

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
      row.DataRowChanged += OnDataRowChanged;

      if (row.Count > maxDataRowCount) {
        maxDataRowCount = row.Count;
        //   UpdateSingleValueRows();
      }

      legendShape.AddLegendItem(new LegendItem(row.Label, row.Color, row.Thickness));
      legendShape.CreateLegend();
      InitLineShapes(row);
    }

    private void OnDataRowRemoved(IDataRow row) {
      row.ValueChanged -= OnRowValueChanged;
      row.ValuesChanged -= OnRowValuesChanged;
      row.DataRowChanged -= OnDataRowChanged;
    }

    #endregion

    private void ZoomToFullView() {
      if (!zoomFullView) {
        return;
      }
      var newClippingArea = new RectangleD(-0.1,
                                           minDataValue - ((maxDataValue - minDataValue)*0.05),
                                           maxDataRowCount - 0.9,
                                           maxDataValue + ((maxDataValue - minDataValue)*0.05));

      SetLineClippingArea(newClippingArea);
      historyStack.Push(newClippingArea);
    }

    /// <summary>
    /// Sets the clipping area of the data to display.
    /// </summary>
    /// <param name="clippingArea"></param>
    private void SetLineClippingArea(RectangleD clippingArea) {
      linesShape.ClippingArea = clippingArea;

      grid.ClippingArea = linesShape.ClippingArea;

      // horizontalLineShape.ClippingArea = linesShape.ClippingArea;


      xAxis.ClippingArea = new RectangleD(linesShape.ClippingArea.X1,
                                          xAxis.BoundingBox.Y1,
                                          linesShape.ClippingArea.X2,
                                          xAxis.BoundingBox.Y2);

      yAxis.ClippingArea = new RectangleD(yAxis.BoundingBox.X1,
                                          linesShape.ClippingArea.Y1,
                                          yAxis.BoundingBox.X2,
                                          linesShape.ClippingArea.Y2);
    }

    private void InitLineShapes(IDataRow row) {
      var lineShapes = new List<LineShape>();
      if (rowToLineShapes.Count == 0) {
        minDataValue = Double.PositiveInfinity;
        maxDataValue = Double.NegativeInfinity;
      }
      if ((row.Count > 0)) {
        maxDataValue = Math.Max(row[0], maxDataValue);
        minDataValue = Math.Min(row[0], minDataValue);
      }
      if ((row.LineType == DataRowType.SingleValue)) {
        if (row.Count > 0) {
          LineShape lineShape = new HorizontalLineShape(0, row[0], double.MaxValue, row[0], row.Color, row.Thickness,
                                                        row.Style);
          lineShapes.Add(lineShape);
          // TODO each DataRow needs its own WorldShape so Y Axes can be zoomed independently.
          linesShape.AddShape(lineShape);
        }
      }
      else {
        for (int i = 1; i < row.Count; i++) {
          var lineShape = new LineShape(i - 1, row[i - 1], i, row[i], row.Color, row.Thickness, row.Style);
          lineShapes.Add(lineShape);
          // TODO each DataRow needs its own WorldShape so Y Axes can be zoomed independently.
          linesShape.AddShape(lineShape);
          maxDataValue = Math.Max(row[i], maxDataValue);
          minDataValue = Math.Min(row[i], minDataValue);
        }
      }
      //horizontalLineShape.YVal = maxDataValue;
      rowToLineShapes[row] = lineShapes;
      ZoomToFullView();

      canvasUI.Invalidate();
    }

    // TODO use action parameter
    private void OnRowValueChanged(IDataRow row, double value, int index, Action action) {
      List<LineShape> lineShapes = rowToLineShapes[row];
      maxDataValue = Math.Max(value, maxDataValue);
      minDataValue = Math.Min(value, minDataValue);
      if (row.LineType == DataRowType.SingleValue) {
        if (action == Action.Added) {
          LineShape lineShape = new HorizontalLineShape(0, row[0], double.MaxValue, row[0], row.Color, row.Thickness,
                                                        row.Style);
          lineShapes.Add(lineShape);
          // TODO each DataRow needs its own WorldShape so Y Axes can be zoomed independently.
          linesShape.AddShape(lineShape);
        }
        else {
          // lineShapes[0].X2 = maxDataRowCount;
          lineShapes[0].Y1 = value;
          lineShapes[0].Y2 = value;
        }
      }
      else {
        //  horizontalLineShape.YVal = maxDataValue;
        if (index > lineShapes.Count + 1) {
          throw new NotImplementedException();
        }

        // new value was added
        if (index > 0 && index == lineShapes.Count + 1) {
          if (maxDataRowCount < row.Count) {
            maxDataRowCount = row.Count;
            //  UpdateSingleValueRows();
          }
          var lineShape = new LineShape(index - 1, row[index - 1], index, row[index], row.Color, row.Thickness,
                                        row.Style);
          lineShapes.Add(lineShape);
          // TODO each DataRow needs its own WorldShape so Y Axes can be zoomed independently.
          linesShape.AddShape(lineShape);
        }

        // not the first value
        if (index > 0) {
          lineShapes[index - 1].Y2 = value;
        }

        // not the last value
        if (index > 0 && index < row.Count - 1) {
          lineShapes[index].Y1 = value;
        }
      }

      ZoomToFullView();
      canvasUI.Invalidate();
    }

    // TODO remove (see ticket #501)
    public IList<IDataRow> GetRows() {
      return model.Rows;
    }


    // TODO use action parameter
    private void OnRowValuesChanged(IDataRow row, double[] values, int index, Action action) {
      foreach (double value in values) {
        OnRowValueChanged(row, value, index++, action);
      }
    }

    private void OnModelChanged() {
      titleShape.Text = model.Title;

      canvasUI.Invalidate();
    }

  
    #region Begin-/EndUpdate

    private int beginUpdateCount;

    public void BeginUpdate() {
      beginUpdateCount++;
    }

    public void EndUpdate() {
      if (beginUpdateCount == 0) {
        throw new InvalidOperationException("Too many EndUpdates.");
      }

      beginUpdateCount--;

      if (beginUpdateCount == 0) {
        canvasUI.Invalidate();
      }
    }

    #endregion

    #region Zooming / Panning

    private readonly Stack<RectangleD> historyStack = new Stack<RectangleD>();
    private RectangleShape rectangleShape;

    private void canvasUI1_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Back && historyStack.Count > 1) {
        historyStack.Pop();

        RectangleD clippingArea = historyStack.Peek();

        SetNewClippingArea(clippingArea);
        canvasUI.Invalidate();
      }
    }

    private void canvasUI1_MouseDown(object sender, MouseEventArgs e) {
      Focus();
      if (e.Button == MouseButtons.Right) {
        contextMenuStrip1.Show(PointToScreen(e.Location));
      }
      else {
        if (ModifierKeys == Keys.Control) {
          CreateZoomListener(e);
        }
        else {
          CreatePanListener(e);
        }
      }
    }

    private void canvas_MouseMove(object sender, MouseEventArgs e) {
      double x = Transform.ToWorldX(e.X, berni.Viewport, berni.ClippingArea);
      double y = Transform.ToWorldY(e.Y, berni.Viewport, berni.ClippingArea);

      mousePointer.Rectangle = new RectangleD(x-1, y-1, x+1, y+1);
      canvasUI.Invalidate();
    }

    private void canvasUI1_MouseWheel(object sender, MouseEventArgs e) {
      if (ModifierKeys == Keys.Control) {
        double zoomFactor = (e.Delta > 0) ? 0.9 : 1.1;

        RectangleD clippingArea = ZoomListener.ZoomClippingArea(linesShape.ClippingArea, zoomFactor);

        SetLineClippingArea(clippingArea);
        canvasUI.Invalidate();
      }
    }

    private void CreateZoomListener(MouseEventArgs e) {
      var zoomListener = new ZoomListener(e.Location);
      zoomListener.DrawRectangle += DrawRectangle;
      zoomListener.OnMouseUp += OnZoom_MouseUp;

      canvasUI.MouseEventListener = zoomListener;

      rectangleShape = new RectangleShape(e.X, e.Y, e.X, e.Y, Color.Blue);
      rectangleShape.Opacity = 50;

      linesShape.AddShape(rectangleShape);
    }

    private void OnZoom_MouseUp(object sender, MouseEventArgs e) {
      canvasUI.MouseEventListener = null;

      RectangleD clippingArea = rectangleShape.Rectangle;

      SetLineClippingArea(clippingArea);
      historyStack.Push(clippingArea);

      linesShape.RemoveShape(rectangleShape);

      zoomFullView = false; //user wants to zoom => no full view

      canvasUI.Invalidate();
    }

    private void DrawRectangle(Rectangle rectangle) {
      rectangleShape.Rectangle = Transform.ToWorld(rectangle, canvasUI.ClientRectangle, linesShape.ClippingArea);
      canvasUI.Invalidate();
    }

    private void CreatePanListener(MouseEventArgs e) {
      PanListener panListener = new PanListener(canvasUI.ClientRectangle, linesShape.ClippingArea, e.Location);

      panListener.SetNewClippingArea += SetNewClippingArea;
      panListener.OnMouseUp += delegate {
                                 historyStack.Push(linesShape.ClippingArea);
                                 canvasUI.MouseEventListener = null;
                               };

      canvasUI.MouseEventListener = panListener;
    }

    private void SetNewClippingArea(RectangleD newClippingArea) {
      SetLineClippingArea(newClippingArea);

      zoomFullView = false;
      canvasUI.Invalidate();
    }

    #endregion

   
  }
}
