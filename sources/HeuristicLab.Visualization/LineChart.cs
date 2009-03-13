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

    private readonly TextShape titleShape = new TextShape("Title");
    private readonly LegendShape legendShape = new LegendShape();
    private readonly XAxis xAxis = new XAxis();
    private readonly List<RowEntry> rowEntries = new List<RowEntry>();

    private readonly Dictionary<IDataRow, RowEntry> rowToRowEntry = new Dictionary<IDataRow, RowEntry>();

    private readonly ViewSettings viewSettings;

//    private readonly Stack<RectangleD> clippingAreaHistory = new Stack<RectangleD>();
    private readonly WorldShape userInteractionShape = new WorldShape();
    private readonly RectangleShape rectangleShape = new RectangleShape(0, 0, 0, 0, Color.FromArgb(50, 0, 0, 255));
    private IMouseEventListener mouseEventListener;

    private const int YAxisWidth = 100;
    private const int XAxisHeight = 20;

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

      this.model = model;
      viewSettings = model.ViewSettings;
      viewSettings.OnUpdateSettings += UpdateViewSettings;

      Item = model;

      UpdateLayout();
      canvasUI.Resize += delegate { UpdateLayout(); };

      ZoomToFullView();
    }

    private void UpdateViewSettings() {
      titleShape.Font = viewSettings.TitleFont;
      titleShape.Color = viewSettings.TitleColor;

      legendShape.Font = viewSettings.LegendFont;
      legendShape.Color = viewSettings.LegendColor;

      xAxis.Font = viewSettings.XAxisFont;
      xAxis.Color = viewSettings.XAxisColor;

      switch (viewSettings.LegendPosition) {
          case LegendPosition.Bottom:
            setLegendBottom();
            break;

          case LegendPosition.Top:
            setLegendTop();
            break;

          case LegendPosition.Left:
            setLegendLeft();
            break;

          case LegendPosition.Right:
            setLegendRight();
            break;
      }

      canvasUI.Invalidate();
    }

    /// <summary>
    /// Layout management - arranges the inner shapes.
    /// </summary>
    private void UpdateLayout() {
      canvas.ClearShapes();

      foreach (RowEntry rowEntry in rowEntries) {
        canvas.AddShape(rowEntry.Grid);
      }

      foreach (RowEntry rowEntry in rowEntries) {
        canvas.AddShape(rowEntry.LinesShape);
      }

      canvas.AddShape(xAxis);

      int yAxesWidth = 0;

      foreach (RowEntry rowEntry in rowEntries) {
        if (rowEntry.DataRow.ShowYAxis) {
          canvas.AddShape(rowEntry.YAxis);
          yAxesWidth += YAxisWidth;
        }
      }

      canvas.AddShape(titleShape);
      canvas.AddShape(legendShape);

      canvas.AddShape(userInteractionShape);

      titleShape.X = 10;
      titleShape.Y = canvasUI.Height - 10;

      RectangleD linesAreaBoundingBox = new RectangleD(yAxesWidth,
                                                       XAxisHeight,
                                                       canvasUI.Width,
                                                       canvasUI.Height);

      foreach (RowEntry rowEntry in rowEntries) {
        rowEntry.LinesShape.BoundingBox = linesAreaBoundingBox;
        rowEntry.Grid.BoundingBox = linesAreaBoundingBox;
      }

      int yAxisLeft = 0;
      foreach (RowEntry rowEntry in rowEntries) {
        if (rowEntry.DataRow.ShowYAxis) {
          rowEntry.YAxis.BoundingBox = new RectangleD(yAxisLeft,
                                                      linesAreaBoundingBox.Y1,
                                                      yAxisLeft + YAxisWidth,
                                                      linesAreaBoundingBox.Y2);
          yAxisLeft += YAxisWidth;
        }
      }

      userInteractionShape.BoundingBox = linesAreaBoundingBox;
      userInteractionShape.ClippingArea = new RectangleD(0, 0, userInteractionShape.BoundingBox.Width, userInteractionShape.BoundingBox.Height);

      xAxis.BoundingBox = new RectangleD(linesAreaBoundingBox.X1,
                                         0,
                                         linesAreaBoundingBox.X2,
                                         linesAreaBoundingBox.Y1);

      setLegendBottom();
    }

    public void setLegendRight() {
      // legend right
      legendShape.BoundingBox = new RectangleD(canvasUI.Width - 110, 10, canvasUI.Width, canvasUI.Height - 50);
      legendShape.ClippingArea = new RectangleD(0, 0, legendShape.BoundingBox.Width, legendShape.BoundingBox.Height);
      legendShape.Row = false;
      legendShape.CreateLegend();
    }

    public void setLegendLeft() {
      // legend left
      legendShape.BoundingBox = new RectangleD(10, 10, 110, canvasUI.Height - 50);
      legendShape.ClippingArea = new RectangleD(0, 0, legendShape.BoundingBox.Width, legendShape.BoundingBox.Height);
      legendShape.Row = false;
      legendShape.CreateLegend();

      canvasUI.Invalidate();
    }

    public void setLegendTop() {
      // legend top
      legendShape.BoundingBox = new RectangleD(100, canvasUI.Height - canvasUI.Height, canvasUI.Width, canvasUI.Height - 10);
      legendShape.ClippingArea = new RectangleD(0, 0, legendShape.BoundingBox.Width, legendShape.BoundingBox.Height);
      legendShape.Row = true;
      legendShape.Top = true;
      legendShape.CreateLegend();
    }

    public void setLegendBottom() {
      // legend bottom
      legendShape.BoundingBox = new RectangleD(100, 10, canvasUI.Width, 200);
      legendShape.ClippingArea = new RectangleD(0, 0, legendShape.BoundingBox.Width, legendShape.BoundingBox.Height);
      legendShape.Row = true;
      legendShape.Top = false;
      legendShape.CreateLegend();
    }

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
      OptionsDialog optionsdlg = new OptionsDialog(model);
      //var optionsdlg = new OptionsDialog(model, this);
      optionsdlg.ShowDialog(this);
      Invalidate();
    }

    public void OnDataRowChanged(IDataRow row) {
      RowEntry rowEntry = rowToRowEntry[row];

      rowEntry.LinesShape.UpdateStyle(row);

      UpdateLayout();

      canvasUI.Invalidate();
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
      row.DataRowChanged += OnDataRowChanged;

      legendShape.AddLegendItem(new LegendItem(row.Label, row.Color, row.Thickness));
      legendShape.CreateLegend();

      InitLineShapes(row);

      UpdateLayout();
    }

    private void OnDataRowRemoved(IDataRow row) {
      row.ValueChanged -= OnRowValueChanged;
      row.ValuesChanged -= OnRowValuesChanged;
      row.DataRowChanged -= OnDataRowChanged;

      rowToRowEntry.Remove(row);
      rowEntries.RemoveAll(delegate(RowEntry rowEntry) { return rowEntry.DataRow == row; });

      UpdateLayout();
    }

    #endregion

    public void ZoomToFullView() {
      SetClipX(-0.1, model.MaxDataRowValues - 0.9);

      foreach (RowEntry rowEntry in rowEntries) {
        IDataRow row = rowEntry.DataRow;

        SetClipY(rowEntry,
                 row.MinValue - ((row.MaxValue - row.MinValue)*0.05),
                 row.MaxValue + ((row.MaxValue - row.MinValue)*0.05));
      }

      zoomToFullView = true;

      canvasUI.Invalidate();
    }

    private void SetClipX(double x1, double x2) {
      xAxis.ClippingArea = new RectangleD(x1,
                                          0,
                                          x2,
                                          XAxisHeight);

      foreach (RowEntry rowEntry in rowEntries) {
        rowEntry.LinesShape.ClippingArea = new RectangleD(x1,
                                                          rowEntry.LinesShape.ClippingArea.Y1,
                                                          x2,
                                                          rowEntry.LinesShape.ClippingArea.Y2);
        rowEntry.Grid.ClippingArea = new RectangleD(x1,
                                                    rowEntry.Grid.ClippingArea.Y1,
                                                    x2,
                                                    rowEntry.Grid.ClippingArea.Y2);
        rowEntry.YAxis.ClippingArea = new RectangleD(0,
                                                     rowEntry.YAxis.ClippingArea.Y1,
                                                     YAxisWidth,
                                                     rowEntry.YAxis.ClippingArea.Y2);
      }
    }

    private static void SetClipY(RowEntry rowEntry, double y1, double y2) {
      rowEntry.LinesShape.ClippingArea = new RectangleD(rowEntry.LinesShape.ClippingArea.X1,
                                                        y1,
                                                        rowEntry.LinesShape.ClippingArea.X2,
                                                        y2);
      rowEntry.Grid.ClippingArea = new RectangleD(rowEntry.Grid.ClippingArea.X1,
                                                  y1,
                                                  rowEntry.Grid.ClippingArea.X2,
                                                  y2);
      rowEntry.YAxis.ClippingArea = new RectangleD(rowEntry.YAxis.ClippingArea.X1,
                                                   y1,
                                                   rowEntry.YAxis.ClippingArea.X2,
                                                   y2);
    }

    private void InitLineShapes(IDataRow row) {
      RowEntry rowEntry = new RowEntry(row);
      rowEntries.Add(rowEntry);
      rowToRowEntry[row] = rowEntry;

      if ((row.LineType == DataRowType.SingleValue)) {
        if (row.Count > 0) {
          LineShape lineShape = new HorizontalLineShape(0, row[0], double.MaxValue, row[0], row.Color, row.Thickness,
                                                        row.Style);
          rowEntry.LinesShape.AddShape(lineShape);
        }
      } else {
        for (int i = 1; i < row.Count; i++) {
          LineShape lineShape = new LineShape(i - 1, row[i - 1], i, row[i], row.Color, row.Thickness, row.Style);
          rowEntry.LinesShape.AddShape(lineShape);
        }
      }

      ZoomToFullView();
    }

    private void OnRowValueChanged(IDataRow row, double value, int index, Action action) {
      RowEntry rowEntry = rowToRowEntry[row];

      if (row.LineType == DataRowType.SingleValue) {
        if (action == Action.Added) {
          LineShape lineShape = new HorizontalLineShape(0, row[0], double.MaxValue, row[0], row.Color, row.Thickness,
                                                        row.Style);
          rowEntry.LinesShape.AddShape(lineShape);
        } else {
          LineShape lineShape = rowEntry.LinesShape.GetShape(0);
          lineShape.Y1 = value;
          lineShape.Y2 = value;
        }
      } else {
        if (index > rowEntry.LinesShape.Count + 1) {
          throw new NotImplementedException();
        }

        // new value was added
        if (index > 0 && index == rowEntry.LinesShape.Count + 1) {
          LineShape lineShape = new LineShape(index - 1, row[index - 1], index, row[index], row.Color, row.Thickness, row.Style);
          rowEntry.LinesShape.AddShape(lineShape);
        }

        // not the first value
        if (index > 0) {
          rowEntry.LinesShape.GetShape(index - 1).Y2 = value;
        }

        // not the last value
        if (index > 0 && index < row.Count - 1) {
          rowEntry.LinesShape.GetShape(index).Y1 = value;
        }
      }

      ZoomToFullView();
    }

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
      zoomToFullView = false;

      foreach (RowEntry rowEntry in rowEntries) {
        RectangleD clippingArea = CalcPanClippingArea(startPoint, endPoint, rowEntry.LinesShape);

        SetClipX(clippingArea.X1, clippingArea.X1);
        SetClipY(rowEntry, clippingArea.Y1, clippingArea.Y2);
      }

      canvasUI.Invalidate();
    }

    private void PanEnd(Point startPoint, Point endPoint) {
      zoomToFullView = false;

      foreach (RowEntry rowEntry in rowEntries) {
        RectangleD clippingArea = CalcPanClippingArea(startPoint, endPoint, rowEntry.LinesShape);

        SetClipX(clippingArea.X1, clippingArea.X1);
        SetClipY(rowEntry, clippingArea.Y1, clippingArea.Y2);
      }

      canvasUI.Invalidate();
    }

    private static RectangleD CalcPanClippingArea(Point startPoint, Point endPoint, LinesShape linesShape) {
      return Translate.ClippingArea(startPoint, endPoint, linesShape.ClippingArea, linesShape.Viewport);
    }

    private void SetClippingArea(Rectangle rectangle) {
      foreach (RowEntry rowEntry in rowEntries) {
        RectangleD clippingArea = Transform.ToWorld(rectangle, rowEntry.LinesShape.Viewport, rowEntry.LinesShape.ClippingArea);

        SetClipX(clippingArea.X1, clippingArea.X1);
        SetClipY(rowEntry, clippingArea.Y1, clippingArea.Y2);
      }

      userInteractionShape.RemoveShape(rectangleShape);
      canvasUI.Invalidate();
    }

    private void DrawRectangle(Rectangle rectangle) {
      rectangleShape.Rectangle = Transform.ToWorld(rectangle, userInteractionShape.Viewport, userInteractionShape.ClippingArea);
      canvasUI.Invalidate();
    }

    private void canvasUI1_KeyDown(object sender, KeyEventArgs e) {
//      if (e.KeyCode == Keys.Back && clippingAreaHistory.Count > 1) {
//        clippingAreaHistory.Pop();
//
//        RectangleD clippingArea = clippingAreaHistory.Peek();
//
//        SetLineClippingArea(clippingArea, false);
//      }
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

        foreach (RowEntry rowEntry in rowEntries) {
          RectangleD clippingArea = ZoomListener.ZoomClippingArea(rowEntry.LinesShape.ClippingArea, zoomFactor);
          
          SetClipX(clippingArea.X1, clippingArea.X1);
          SetClipY(rowEntry, clippingArea.Y1, clippingArea.Y2);
        }
      }
    }

    #endregion

    private class LinesShape : WorldShape {
      public void UpdateStyle(IDataRow row) {
        foreach (IShape shape in shapes) {
          LineShape lineShape = shape as LineShape;
          if (lineShape != null) {
            lineShape.LSColor = row.Color;
            lineShape.LSDrawingStyle = row.Style;
            lineShape.LSThickness = row.Thickness;
          }
        }
      }

      public int Count {
        get { return shapes.Count; }
      }

      public LineShape GetShape(int index) {
        return (LineShape)shapes[index];
      }
    }

    private class RowEntry {
      private readonly IDataRow dataRow;

      private readonly Grid grid = new Grid();
      private readonly YAxis yAxis = new YAxis();
      private readonly LinesShape linesShape = new LinesShape();

      public RowEntry(IDataRow dataRow) {
        this.dataRow = dataRow;
      }

      public IDataRow DataRow {
        get { return dataRow; }
      }

      public Grid Grid {
        get { return grid; }
      }

      public YAxis YAxis {
        get { return yAxis; }
      }

      public LinesShape LinesShape {
        get { return linesShape; }
      }
    }
  }
}
