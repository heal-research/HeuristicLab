using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization {
  public partial class LineChart : ViewBase {
    internal class LinesShape : WorldShape {}

    private readonly IChartDataRowsModel model;
    private int maxDataRowCount;
    private Boolean zoomFullView;
    private double minDataValue;
    private double maxDataValue;
    private bool minMaxLineEnabled;
    private MinMaxLineShape minMaxLineShape;
    private IShape minLineShape;

    private readonly WorldShape root;
    private readonly TextShape titleShape;
    private readonly LinesShape linesShape;
    private readonly LegendShape legendShape;

    private readonly XAxis xAxis;
    private readonly YAxis yAxis;
    private readonly Grid grid;

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

      root = new WorldShape();

      grid = new Grid();
      minMaxLineEnabled = true;
      root.AddShape(grid);

      linesShape = new LinesShape();
      root.AddShape(linesShape);

      legendShape = new LegendShape(0, 0, 0, 0, 0, Color.Black);
      //legendShape.AddLegendItem(new LegendItem("test", Color.Red, 5));
      //legendShape.AddLegendItem(new LegendItem("test1", Color.Blue, 5));
      //legendShape.AddLegendItem(new LegendItem("test2", Color.Pink, 5));
      root.AddShape(legendShape);

      xAxis = new XAxis();
      root.AddShape(xAxis);

      yAxis = new YAxis();
      root.AddShape(yAxis);

      titleShape = new TextShape(0, 0, model.Title, 15);
      root.AddShape(titleShape);

      minMaxLineShape = new MinMaxLineShape(this.minDataValue, this.maxDataValue, 0, Color.Yellow, 4, DrawingStyle.Solid);
      root.AddShape(minMaxLineShape);
      canvas.MainCanvas.WorldShape = root;
      canvas.Resize += delegate { UpdateLayout(); };

      UpdateLayout();
      maxDataRowCount = 0;
      this.model = model;
      Item = model;


      //The whole data rows are shown per default
      ResetView();
    }

    /// <summary>
    /// Layout management - arranges the inner shapes.
    /// </summary>
    private void UpdateLayout() {
      root.ClippingArea = new RectangleD(0, 0, canvas.Width, canvas.Height);

      titleShape.X = 10;
      titleShape.Y = canvas.Height - 10;

      const int yAxisWidth = 100;
      const int xAxisHeight = 20;

      linesShape.BoundingBox = new RectangleD(yAxisWidth,
                                              xAxisHeight,
                                              canvas.Width,
                                              canvas.Height);

      grid.BoundingBox = linesShape.BoundingBox;

      minMaxLineShape.BoundingBox = linesShape.BoundingBox;

      yAxis.BoundingBox = new RectangleD(0,
                                         linesShape.BoundingBox.Y1,
                                         linesShape.BoundingBox.X1,
                                         linesShape.BoundingBox.Y2);

      xAxis.BoundingBox = new RectangleD(linesShape.BoundingBox.X1,
                                         0,
                                         linesShape.BoundingBox.X2,
                                         linesShape.BoundingBox.Y1);

      legendShape.BoundingBox = new RectangleD(10, 10, 110, canvas.Height - 50);
    }

    public void ResetView() {
      zoomFullView = true;
      ZoomToFullView();

      canvas.Invalidate();
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
      if (row.Count > maxDataRowCount) {
        maxDataRowCount = row.Count;
      }

      legendShape.AddLegendItem(new LegendItem(row.Label, row.Color, row.Thickness));
      legendShape.CreateLegend();
      InitLineShapes(row);
    }

    private void ZoomToFullView() {
      if (!zoomFullView) {
        return;
      }
      RectangleD newClippingArea = new RectangleD(-0.1,
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

      minMaxLineShape.ClippingArea = linesShape.ClippingArea;

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
      List<LineShape> lineShapes = new List<LineShape>();
      if (rowToLineShapes.Count == 0) {
        minDataValue = Double.PositiveInfinity;
        maxDataValue = Double.NegativeInfinity;
      }
      if (row.Count > 0) {
        maxDataValue = Math.Max(row[0], maxDataValue);
        minDataValue = Math.Min(row[0], minDataValue);
      }
      for (int i = 1; i < row.Count; i++) {
        LineShape lineShape = new LineShape(i - 1, row[i - 1], i, row[i], 0, row.Color, row.Thickness, row.Style);
        lineShapes.Add(lineShape);
        // TODO each DataRow needs its own WorldShape so Y Axes can be zoomed independently.
        linesShape.AddShape(lineShape);
        maxDataValue = Math.Max(row[i], maxDataValue);
        minDataValue = Math.Min(row[i], minDataValue);
      }
      minMaxLineShape.YMax = maxDataValue;
      minMaxLineShape.YMin = minDataValue;
      rowToLineShapes[row] = lineShapes;
      ZoomToFullView();

      canvas.Invalidate();
    }

    private void OnDataRowRemoved(IDataRow row) {
      row.ValueChanged -= OnRowValueChanged;
      row.ValuesChanged -= OnRowValuesChanged;
    }

    private readonly IDictionary<IDataRow, List<LineShape>> rowToLineShapes =
      new Dictionary<IDataRow, List<LineShape>>();

    // TODO use action parameter
    private void OnRowValueChanged(IDataRow row, double value, int index, Action action) {
      List<LineShape> lineShapes = rowToLineShapes[row];
      maxDataValue = Math.Max(value, maxDataValue);
      minDataValue = Math.Min(value, minDataValue);
      minMaxLineShape.YMax = maxDataValue;
      minMaxLineShape.YMin = minDataValue;
      if (index > lineShapes.Count + 1) {
        throw new NotImplementedException();
      }

      // new value was added
      if (index > 0 && index == lineShapes.Count + 1) {
        if (maxDataRowCount < row.Count) {
          maxDataRowCount = row.Count;
        }
        LineShape lineShape = new LineShape(index - 1, row[index - 1], index, row[index], 0, row.Color, row.Thickness,
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
      ZoomToFullView();

      canvas.Invalidate();
    }


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

      Invalidate();
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
        canvas.Invalidate();
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
        canvas.Invalidate();
      }
    }

    private void canvasUI1_MouseDown(object sender, MouseEventArgs e) {
      Focus();
      if (e.Button == System.Windows.Forms.MouseButtons.Right) {
        if (this.ParentForm != null)
          this.contextMenuStrip1.Show(e.Location.X + this.ParentForm.Location.X,
                                      e.Location.Y + this.ParentForm.Location.Y + 50);
        else {
          this.contextMenuStrip1.Show(e.Location.X, e.Location.Y);
        }
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

    private void canvasUI1_MouseWheel(object sender, MouseEventArgs e) {
      if (ModifierKeys == Keys.Control) {
        double zoomFactor = (e.Delta > 0) ? 0.9 : 1.1;

        RectangleD clippingArea = ZoomListener.ZoomClippingArea(linesShape.ClippingArea, zoomFactor);

        SetLineClippingArea(clippingArea);
        canvas.Invalidate();
      }
    }

    private void CreateZoomListener(MouseEventArgs e) {
      ZoomListener zoomListener = new ZoomListener(e.Location);
      zoomListener.DrawRectangle += DrawRectangle;
      zoomListener.OnMouseUp += OnZoom_MouseUp;

      canvas.MouseEventListener = zoomListener;

      rectangleShape = new RectangleShape(e.X, e.Y, e.X, e.Y, Color.Blue);
      rectangleShape.Opacity = 50;

      linesShape.AddShape(rectangleShape);
    }

    private void OnZoom_MouseUp(object sender, MouseEventArgs e) {
      canvas.MouseEventListener = null;

      RectangleD clippingArea = rectangleShape.Rectangle;

      SetLineClippingArea(clippingArea);
      historyStack.Push(clippingArea);

      linesShape.RemoveShape(rectangleShape);

      zoomFullView = false; //user wants to zoom => no full view

      canvas.Invalidate();
    }

    private void DrawRectangle(Rectangle rectangle) {
      rectangleShape.Rectangle = Transform.ToWorld(rectangle, canvas.ClientRectangle, linesShape.ClippingArea);
      canvas.Invalidate();
    }

    private void CreatePanListener(MouseEventArgs e) {
      PanListener panListener = new PanListener(canvas.ClientRectangle, linesShape.ClippingArea, e.Location);

      panListener.SetNewClippingArea += SetNewClippingArea;
      panListener.OnMouseUp += delegate {
                                 historyStack.Push(linesShape.ClippingArea);
                                 canvas.MouseEventListener = null;
                               };

      canvas.MouseEventListener = panListener;
    }

    private void SetNewClippingArea(RectangleD newClippingArea) {
      SetLineClippingArea(newClippingArea);

      zoomFullView = false;
      canvas.Invalidate();
    }

    #endregion

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
      var optionsdlg = new OptionsDialog(this);
      optionsdlg.Show();
    }

    public void ApplyChangesToRow(IDataRow row) {
      foreach (var ls in rowToLineShapes[row]) {
        ls.LSColor = row.Color;
        ls.LSThickness = row.Thickness;
        if (row.Style == DrawingStyle.Dashed) {
          ls.LSDashStyle = DashStyle.Dash;
        }
        else {
          ls.LSDashStyle = DashStyle.Solid; //default
        }
      }
      canvas.Invalidate();
    }
  }
}