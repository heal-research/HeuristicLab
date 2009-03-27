using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Visualization.Legend;
using HeuristicLab.Visualization.Options;
using HeuristicLab.Visualization.Test;

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

    private readonly WorldShape userInteractionShape = new WorldShape();
    private readonly RectangleShape rectangleShape = new RectangleShape(0, 0, 0, 0, Color.FromArgb(50, 0, 0, 255));
    private IMouseEventListener mouseEventListener;

    private const int YAxisWidth = 100;
    private const int XAxisHeight = 20;

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

    /// <summary>
    /// updates the view settings
    /// </summary>
    private void UpdateViewSettings() {
      titleShape.Font = viewSettings.TitleFont;
      titleShape.Color = viewSettings.TitleColor;

      legendShape.Font = viewSettings.LegendFont;
      legendShape.Color = viewSettings.LegendColor;

      xAxis.Font = viewSettings.XAxisFont;
      xAxis.Color = viewSettings.XAxisColor;

      SetLegendPosition();

      canvasUI.Invalidate();
    }

    /// <summary>
    /// Layout management - arranges the inner shapes.
    /// </summary>
    private void UpdateLayout() {
      canvas.ClearShapes();

      foreach (YAxisDescriptor yAxisDescriptor in model.YAxes) {
        YAxisInfo info = GetYAxisInfo(yAxisDescriptor);
        canvas.AddShape(info.Grid);
      }

      foreach (RowEntry rowEntry in rowEntries) {
        canvas.AddShape(rowEntry.LinesShape);
      }

      canvas.AddShape(xAxis);

      int yAxesWidthLeft = 0;
      int yAxesWidthRight = 0;

      foreach (YAxisDescriptor yAxisDescriptor in model.YAxes) {
        YAxisInfo info = GetYAxisInfo(yAxisDescriptor);
        if (yAxisDescriptor.ShowYAxis) {
          canvas.AddShape(info.YAxis);
          info.YAxis.Position = yAxisDescriptor.Position;
          switch (yAxisDescriptor.Position) {
            case AxisPosition.Left:
              yAxesWidthLeft += YAxisWidth;
              break;
            case AxisPosition.Right:
              yAxesWidthRight += YAxisWidth;
              break;
            default:
              throw new NotImplementedException();
          }
        }
      }

      canvas.AddShape(titleShape);
      canvas.AddShape(legendShape);

      canvas.AddShape(userInteractionShape);

      titleShape.X = 10;
      titleShape.Y = canvasUI.Height - 10;

      RectangleD linesAreaBoundingBox = new RectangleD(yAxesWidthLeft,
                                                       XAxisHeight,
                                                       canvasUI.Width - yAxesWidthRight,
                                                       canvasUI.Height);

      foreach (RowEntry rowEntry in rowEntries) {
        rowEntry.LinesShape.BoundingBox = linesAreaBoundingBox;
      }

      foreach (YAxisDescriptor yAxisDescriptor in model.YAxes) {
        YAxisInfo info = GetYAxisInfo(yAxisDescriptor);
        info.Grid.BoundingBox = linesAreaBoundingBox;
      }

      int yAxisLeft = 0;
      int yAxisRight = (int)linesAreaBoundingBox.X2;

      foreach (YAxisDescriptor yAxisDescriptor in model.YAxes) {
        YAxisInfo info = GetYAxisInfo(yAxisDescriptor);
        if (yAxisDescriptor.ShowYAxis) {
          switch (yAxisDescriptor.Position) {
            case AxisPosition.Left:
              info.YAxis.BoundingBox = new RectangleD(yAxisLeft,
                                                      linesAreaBoundingBox.Y1,
                                                      yAxisLeft + YAxisWidth,
                                                      linesAreaBoundingBox.Y2);
              yAxisLeft += YAxisWidth;
              break;
            case AxisPosition.Right:
              info.YAxis.BoundingBox = new RectangleD(yAxisRight,
                                                      linesAreaBoundingBox.Y1,
                                                      yAxisRight + YAxisWidth,
                                                      linesAreaBoundingBox.Y2);
              yAxisRight += YAxisWidth;
              break;
            default:
              throw new NotImplementedException();
          }
        }
      }

      userInteractionShape.BoundingBox = linesAreaBoundingBox;
      userInteractionShape.ClippingArea = new RectangleD(0, 0, userInteractionShape.BoundingBox.Width, userInteractionShape.BoundingBox.Height);

      xAxis.BoundingBox = new RectangleD(linesAreaBoundingBox.X1,
                                         0,
                                         linesAreaBoundingBox.X2,
                                         linesAreaBoundingBox.Y1);

      SetLegendPosition();
    }

    private readonly Dictionary<YAxisDescriptor, YAxisInfo> yAxisInfos = new Dictionary<YAxisDescriptor, YAxisInfo>();

    private YAxisInfo GetYAxisInfo(YAxisDescriptor yAxisDescriptor) {
      YAxisInfo info;

      if (!yAxisInfos.TryGetValue(yAxisDescriptor, out info)) {
        info = new YAxisInfo();
        yAxisInfos[yAxisDescriptor] = info;
      }

      return info;
    }

    /// <summary>
    /// sets the legend position
    /// </summary>
    private void SetLegendPosition() {
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
    }

    public void setLegendRight() {
      // legend right
      legendShape.BoundingBox = new RectangleD(canvasUI.Width - legendShape.GetMaxLabelLength(), 10, canvasUI.Width, canvasUI.Height - 50);
      legendShape.ClippingArea = new RectangleD(0, 0, legendShape.BoundingBox.Width, legendShape.BoundingBox.Height);
      legendShape.Row = false;
      legendShape.CreateLegend();
    }

    public void setLegendLeft() {
      // legend left
      legendShape.BoundingBox = new RectangleD(10, 10, canvasUI.Width, canvasUI.Height - 50);
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
      legendShape.BoundingBox = new RectangleD(100, 10, canvasUI.Width, canvasUI.Height/*legendShape.GetHeight4Rows()*/);
      legendShape.ClippingArea = new RectangleD(0, 0, legendShape.BoundingBox.Width, legendShape.BoundingBox.Height);
      legendShape.Row = true;
      legendShape.Top = false;
      legendShape.CreateLegend();
    }

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
      OptionsDialog optionsdlg = new OptionsDialog(model);
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
        YAxisDescriptor yAxisDescriptor = rowEntry.DataRow.YAxis;

        SetClipY(rowEntry,
                 yAxisDescriptor.MinValue - ((yAxisDescriptor.MaxValue - yAxisDescriptor.MinValue)*0.05),
                 yAxisDescriptor.MaxValue + ((yAxisDescriptor.MaxValue - yAxisDescriptor.MinValue)*0.05));
      }

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
      }

      foreach (YAxisDescriptor yAxisDescriptor in model.YAxes) {
        YAxisInfo info = GetYAxisInfo(yAxisDescriptor);
        info.Grid.ClippingArea = new RectangleD(x1,
                                                info.Grid.ClippingArea.Y1,
                                                x2,
                                                info.Grid.ClippingArea.Y2);
        info.YAxis.ClippingArea = new RectangleD(0,
                                                 info.YAxis.ClippingArea.Y1,
                                                 YAxisWidth,
                                                 info.YAxis.ClippingArea.Y2);
      }
    }

    private void SetClipY(RowEntry rowEntry, double y1, double y2) {
      rowEntry.LinesShape.ClippingArea = new RectangleD(rowEntry.LinesShape.ClippingArea.X1,
                                                        y1,
                                                        rowEntry.LinesShape.ClippingArea.X2,
                                                        y2);

      YAxisInfo info = GetYAxisInfo(rowEntry.DataRow.YAxis);

      info.Grid.ClippingArea = new RectangleD(info.Grid.ClippingArea.X1,
                                              y1,
                                              info.Grid.ClippingArea.X2,
                                              y2);
      info.YAxis.ClippingArea = new RectangleD(info.YAxis.ClippingArea.X1,
                                               y1,
                                               info.YAxis.ClippingArea.X2,
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
      RectangleD clippingArea = Translate.ClippingArea(startPoint, endPoint, xAxis.ClippingArea, xAxis.Viewport);

      SetClipX(clippingArea.X1, clippingArea.X2);

      foreach (RowEntry rowEntry in rowEntries) {
        if (rowEntry.DataRow.YAxis.ClipChangeable) {
          clippingArea = Translate.ClippingArea(startPoint, endPoint, rowEntry.LinesShape.ClippingArea, rowEntry.LinesShape.Viewport);
          SetClipY(rowEntry, clippingArea.Y1, clippingArea.Y2);
        }
      }

      canvasUI.Invalidate();
    }

    private void PanEnd(Point startPoint, Point endPoint) {
      Pan(startPoint, endPoint);
    }

    private void SetClippingArea(Rectangle rectangle) {
      RectangleD clippingArea = Transform.ToWorld(rectangle, xAxis.Viewport, xAxis.ClippingArea);

      SetClipX(clippingArea.X1, clippingArea.X2);

      foreach (RowEntry rowEntry in rowEntries) {
        if (rowEntry.DataRow.YAxis.ClipChangeable) {
          clippingArea = Transform.ToWorld(rectangle, rowEntry.LinesShape.Viewport, rowEntry.LinesShape.ClippingArea);

          SetClipY(rowEntry, clippingArea.Y1, clippingArea.Y2);
        }
      }

      userInteractionShape.RemoveShape(rectangleShape);
      canvasUI.Invalidate();
    }

    private void DrawRectangle(Rectangle rectangle) {
      rectangleShape.Rectangle = Transform.ToWorld(rectangle, userInteractionShape.Viewport, userInteractionShape.ClippingArea);
      canvasUI.Invalidate();
    }

    private void canvasUI1_KeyDown(object sender, KeyEventArgs e) {
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
        double zoomFactor = (e.Delta > 0) ? 0.7 : 1.3;

        PointD world;

        world = Transform.ToWorld(e.Location, xAxis.Viewport, xAxis.ClippingArea);

        double x1 = world.X - (world.X - xAxis.ClippingArea.X1)*zoomFactor;
        double x2 = world.X + (xAxis.ClippingArea.X2 - world.X)*zoomFactor;

        SetClipX(x1, x2);

        foreach (RowEntry rowEntry in rowEntries) {
          world = Transform.ToWorld(e.Location, rowEntry.LinesShape.Viewport, rowEntry.LinesShape.ClippingArea);

          double y1 = world.Y - (world.Y - rowEntry.LinesShape.ClippingArea.Y1) * zoomFactor;
          double y2 = world.Y + (rowEntry.LinesShape.ClippingArea.Y2 - world.Y)*zoomFactor;

          SetClipY(rowEntry, y1, y2);
        }

        canvasUI.Invalidate();
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

      private readonly LinesShape linesShape = new LinesShape();

      public RowEntry(IDataRow dataRow) {
        this.dataRow = dataRow;
      }

      public IDataRow DataRow {
        get { return dataRow; }
      }

      public LinesShape LinesShape {
        get { return linesShape; }
      }
    }

    private class YAxisInfo {
      private readonly Grid grid = new Grid();
      private readonly YAxis yAxis = new YAxis();

      public Grid Grid {
        get { return grid; }
      }

      public YAxis YAxis {
        get { return yAxis; }
      }
    }
  }
}
