using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Visualization.Legend;
using HeuristicLab.Visualization.Options;

namespace HeuristicLab.Visualization {
  public partial class LineChart : ViewBase {
    private readonly IChartDataRowsModel model;
    private readonly Canvas canvas;

    private int maxDataRowCount;
    private double minDataValue;
    private double maxDataValue;

    private readonly TextShape titleShape;
    private readonly LinesShape linesShape;
    private readonly LegendShape legendShape;

    private readonly XAxis xAxis;
    private readonly YAxis yAxis;
    private readonly Grid grid;

    private readonly Stack<RectangleD> clippingAreaHistory = new Stack<RectangleD>();
    private readonly WorldShape userInteractionShape;
    private readonly RectangleShape rectangleShape;
    private IMouseEventListener mouseEventListener;
    private bool zoomToFullView;

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

      userInteractionShape = new WorldShape();
      canvas.AddShape(userInteractionShape);

      rectangleShape = new RectangleShape(0, 0, 0, 0, Color.Blue);
      rectangleShape.Opacity = 50;

      maxDataRowCount = 0;
      this.model = model;
      Item = model;

      UpdateLayout();
      canvasUI.Resize += delegate { UpdateLayout(); };

      //The whole data rows are shown per default
      if (zoomToFullView) {
        ZoomToFullView();
      }
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

      userInteractionShape.BoundingBox = linesShape.BoundingBox;
      userInteractionShape.ClippingArea = new RectangleD(0, 0, userInteractionShape.BoundingBox.Width, userInteractionShape.BoundingBox.Height);

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

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
      OptionsDialog optionsdlg = new OptionsDialog(this.model);
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

    public void ZoomToFullView() {
      RectangleD newClippingArea = new RectangleD(-0.1,
                                                  minDataValue - ((maxDataValue - minDataValue)*0.05),
                                                  maxDataRowCount - 0.9,
                                                  maxDataValue + ((maxDataValue - minDataValue)*0.05));

      SetLineClippingArea(newClippingArea, true);

      zoomToFullView = true;
    }

    /// <summary>
    /// Sets the clipping area of the data to display.
    /// </summary>
    /// <param name="clippingArea"></param>
    /// <param name="pushToHistoryStack"></param>
    private void SetLineClippingArea(RectangleD clippingArea, bool pushToHistoryStack) {
      zoomToFullView = false;

      if (pushToHistoryStack) {
        int count = clippingAreaHistory.Count;

        if (count > 40) {
          RectangleD[] clippingAreas = clippingAreaHistory.ToArray();
          clippingAreaHistory.Clear();

          for (int i = count - 20; i < count; i++) {
            clippingAreaHistory.Push(clippingAreas[i]);
          }
        }

        clippingAreaHistory.Push(clippingArea);
      }

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

      canvasUI.Invalidate();
    }

    private void InitLineShapes(IDataRow row) {
      List<LineShape> lineShapes = new List<LineShape>();
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
      } else {
        for (int i = 1; i < row.Count; i++) {
          LineShape lineShape = new LineShape(i - 1, row[i - 1], i, row[i], row.Color, row.Thickness, row.Style);
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
        } else {
          // lineShapes[0].X2 = maxDataRowCount;
          lineShapes[0].Y1 = value;
          lineShapes[0].Y2 = value;
        }
      } else {
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
          LineShape lineShape = new LineShape(index - 1, row[index - 1], index, row[index], row.Color, row.Thickness,
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

    private void Pan(Point startPoint, Point endPoint) {
      RectangleD clippingArea = CalcPanClippingArea(startPoint, endPoint);
      SetLineClippingArea(clippingArea, false);
    }

    private void PanEnd(Point startPoint, Point endPoint) {
      RectangleD clippingArea = CalcPanClippingArea(startPoint, endPoint);
      SetLineClippingArea(clippingArea, true);
    }

    private RectangleD CalcPanClippingArea(Point startPoint, Point endPoint) {
      return Translate.ClippingArea(startPoint, endPoint, linesShape.ClippingArea, linesShape.Viewport);
    }

    private void SetClippingArea(Rectangle rectangle) {
      RectangleD clippingArea = Transform.ToWorld(rectangle, linesShape.Viewport, linesShape.ClippingArea);

      SetLineClippingArea(clippingArea, true);
      userInteractionShape.RemoveShape(rectangleShape);
    }

    private void DrawRectangle(Rectangle rectangle) {
      rectangleShape.Rectangle = Transform.ToWorld(rectangle, userInteractionShape.Viewport, userInteractionShape.ClippingArea);
      canvasUI.Invalidate();
    }

    private void canvasUI1_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Back && clippingAreaHistory.Count > 1) {
        clippingAreaHistory.Pop();

        RectangleD clippingArea = clippingAreaHistory.Peek();

        SetLineClippingArea(clippingArea, false);
      }
    }

    private void canvasUI1_MouseDown(object sender, MouseEventArgs e) {
      Focus();

      if (e.Button == MouseButtons.Right) {
        contextMenuStrip1.Show(PointToScreen(e.Location));
      } else if (e.Button == MouseButtons.Left) {
        if (ModifierKeys == Keys.None) {
          PanListener panListener = new PanListener(e.Location);
          panListener.Pan += Pan;
          panListener.PanEnd += PanEnd;

          mouseEventListener = panListener;
        } else if (ModifierKeys == Keys.Control) {
          ZoomListener zoomListener = new ZoomListener(e.Location);
          zoomListener.DrawRectangle += DrawRectangle;
          zoomListener.SetClippingArea += SetClippingArea;

          rectangleShape.Rectangle = RectangleD.Empty;
          userInteractionShape.AddShape(rectangleShape);

          mouseEventListener = zoomListener;
        }
      }
    }

    private void canvasUI_MouseMove(object sender, MouseEventArgs e) {
      if (mouseEventListener != null) {
        mouseEventListener.MouseMove(sender, e);
      }
    }

    private void canvasUI_MouseUp(object sender, MouseEventArgs e) {
      if (mouseEventListener != null) {
        mouseEventListener.MouseUp(sender, e);
      }

      mouseEventListener = null;
    }

    private void canvasUI1_MouseWheel(object sender, MouseEventArgs e) {
      if (ModifierKeys == Keys.Control) {
        double zoomFactor = (e.Delta > 0) ? 0.9 : 1.1;

        RectangleD clippingArea = ZoomListener.ZoomClippingArea(linesShape.ClippingArea, zoomFactor);

        SetLineClippingArea(clippingArea, true);
      }
    }

    #endregion

    #region Nested type: LinesShape

    internal class LinesShape : WorldShape {}

    #endregion
  }
}