using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Visualization.DataExport;
using HeuristicLab.Visualization.Drawing;
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
    private readonly XAxisGrid xAxisGrid = new XAxisGrid();
    private readonly List<RowEntry> rowEntries = new List<RowEntry>();

    private readonly Dictionary<IDataRow, RowEntry> rowToRowEntry = new Dictionary<IDataRow, RowEntry>();

    private readonly ViewSettings viewSettings;

    private readonly WorldShape userInteractionShape = new WorldShape();
    private readonly RectangleShape rectangleShape = new RectangleShape(0, 0, 0, 0, Color.FromArgb(50, 0, 0, 255));
    private IMouseEventListener mouseEventListener;

    private const int YAxisWidth = 100;
    private const int XAxisHeight = 40;
    private readonly TooltipListener toolTipListener;
    private readonly ToolTip valueToolTip;
    private Point currentMousePos;

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

      valueToolTip = new ToolTip();
      toolTipListener = new TooltipListener();
      toolTipListener.ShowToolTip += ShowToolTip;
      mouseEventListener = toolTipListener;
      currentMousePos = new Point(0, 0);

      this.ResizeRedraw = true;

      canvasUI.BeforePaint += delegate { UpdateLayout(); };

      UpdateLayout();
      ZoomToFullView();
    }

    public Bitmap Snapshot() {
      UpdateLayout();
      Bitmap bmp = new Bitmap(Width, Height);
      using (Graphics g = Graphics.FromImage(bmp)) {
        canvas.Draw(g);
      }
      return bmp;
    }

    /// <summary>
    /// updates the view settings
    /// </summary>
    private void UpdateViewSettings() {
      titleShape.Font = viewSettings.TitleFont;
      titleShape.Color = viewSettings.TitleColor;
      titleShape.Text = model.Title;

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

      titleShape.Text = model.Title;

      if (model.XAxis.ShowGrid) {
        xAxisGrid.Color = model.XAxis.GridColor;
        canvas.AddShape(xAxisGrid);
      }

      foreach (YAxisDescriptor yAxisDescriptor in model.YAxes) {
        YAxisInfo info = GetYAxisInfo(yAxisDescriptor);
        if (yAxisDescriptor.ShowGrid) {
          info.Grid.Color = yAxisDescriptor.GridColor;
          canvas.AddShape(info.Grid);
        }
      }

      foreach (RowEntry rowEntry in rowEntries) {
        canvas.AddShape(rowEntry.LinesShape);
      }

      xAxis.ShowLabel = model.XAxis.ShowLabel;
      xAxis.Label = model.XAxis.Label;

      canvas.AddShape(xAxis);

      int yAxesWidthLeft = 0;
      int yAxesWidthRight = 0;

      foreach (YAxisDescriptor yAxisDescriptor in model.YAxes) {
        YAxisInfo info = GetYAxisInfo(yAxisDescriptor);
        if (yAxisDescriptor.ShowYAxis) {
          canvas.AddShape(info.YAxis);
          info.YAxis.ShowLabel = yAxisDescriptor.ShowYAxisLabel;
          info.YAxis.Label = yAxisDescriptor.Label;
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

      xAxisGrid.BoundingBox = linesAreaBoundingBox;

      foreach (YAxisDescriptor yAxisDescriptor in model.YAxes) {
        YAxisInfo info = GetYAxisInfo(yAxisDescriptor);
        info.Grid.BoundingBox = linesAreaBoundingBox;
      }

      int yAxisLeft = 0;
      int yAxisRight = (int) linesAreaBoundingBox.X2;

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
      userInteractionShape.ClippingArea = new RectangleD(0, 0, userInteractionShape.BoundingBox.Width,
                                                         userInteractionShape.BoundingBox.Height);

      xAxis.BoundingBox = new RectangleD(linesAreaBoundingBox.X1,
                                         0,
                                         linesAreaBoundingBox.X2,
                                         linesAreaBoundingBox.Y1);

      SetLegendPosition();
    }

    private readonly Dictionary<YAxisDescriptor, YAxisInfo> yAxisInfos = new Dictionary<YAxisDescriptor, YAxisInfo>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="yAxisDescriptor"></param>
    /// <returns></returns>
    private YAxisInfo GetYAxisInfo(YAxisDescriptor yAxisDescriptor) {
      YAxisInfo info;

      if (!yAxisInfos.TryGetValue(yAxisDescriptor, out info)) {
        info = new YAxisInfo();
        yAxisInfos[yAxisDescriptor] = info;
      }

      return info;
    }


    #region Legend-specific

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
      legendShape.BoundingBox = new RectangleD(canvasUI.Width - legendShape.GetMaxLabelLength(), 10, canvasUI.Width,
                                               canvasUI.Height - 50);
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
      legendShape.BoundingBox = new RectangleD(100, canvasUI.Height - canvasUI.Height, canvasUI.Width,
                                               canvasUI.Height - 10);
      legendShape.ClippingArea = new RectangleD(0, 0, legendShape.BoundingBox.Width, legendShape.BoundingBox.Height);
      legendShape.Row = true;
      legendShape.Top = true;
      legendShape.CreateLegend();
    }

    public void setLegendBottom() {
      // legend bottom
      legendShape.BoundingBox = new RectangleD(100, 2, canvasUI.Width, canvasUI.Height);
      legendShape.ClippingArea = new RectangleD(0, 0, legendShape.BoundingBox.Width, legendShape.BoundingBox.Height);
      legendShape.Row = true;
      legendShape.Top = false;
      legendShape.CreateLegend();
    }

    #endregion

    /// <summary>
    /// Shows the Tooltip with the real values of a datapoint, if the mousepoint is near to one
    /// </summary>
    /// <param name="location"></param>
    private void ShowToolTip(Point location) {
      valueToolTip.Hide(this);
      if (rowEntries.Count > 0) {
        double dx = Transform.ToWorldX(location.X, this.rowEntries[0].LinesShape.Viewport,
                                       this.rowEntries[0].LinesShape.ClippingArea);
        int ix = (int) Math.Round(dx);
        foreach (var rowEntry in rowEntries) {
          if ((rowEntry.DataRow.Count > ix) && (ix > 0) && ((rowEntry.DataRow.RowSettings.LineType == DataRowType.Normal) || (rowEntry.DataRow.RowSettings.LineType == DataRowType.Points))) {
            Point screenDataP = Transform.ToScreen(new PointD(ix, rowEntry.DataRow[ix]), rowEntry.LinesShape.Viewport,
                                                   rowEntry.LinesShape.ClippingArea);
            if ((Math.Abs(screenDataP.X - location.X) <= 6) && (Math.Abs(screenDataP.Y - location.Y) <= 6)) {
              valueToolTip.Show(("\t x:" + ix + " y:" + rowEntry.DataRow[ix]), this, screenDataP.X, screenDataP.Y);
            }
          }
        }
      }
    }



    private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
      OptionsDialog optionsdlg = new OptionsDialog(model);
      optionsdlg.Show();
      mouseEventListener = toolTipListener;
    }

    private void exportToolStripMenuItem_Click(object sender, EventArgs e) {
      ExportDialog exportdlg = new ExportDialog();
      exportdlg.ShowDialog(this);

      IExporter exporter = exportdlg.SelectedExporter;

      if (exporter != null)
        exporter.Export(model, this);
    }

    public void OnDataRowChanged(IDataRow row) {
      RowEntry rowEntry = rowToRowEntry[row];

      rowEntry.LinesShape.UpdateStyle(row);

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

      legendShape.AddLegendItem(new LegendItem(row));
      legendShape.CreateLegend();

      InitLineShapes(row);

      canvasUI.Invalidate();
    }

    private void OnDataRowRemoved(IDataRow row) {
      row.ValueChanged -= OnRowValueChanged;
      row.ValuesChanged -= OnRowValuesChanged;
      row.DataRowChanged -= OnDataRowChanged;

      rowToRowEntry.Remove(row);
      rowEntries.RemoveAll(delegate(RowEntry rowEntry) { return rowEntry.DataRow == row; });

      canvasUI.Invalidate();
    }

    #endregion

    public void ZoomToFullView() {
      double xmin, xmax;
      GetClippingRange(0, model.MaxDataRowValues-1, out xmin, out xmax);
      SetClipX(xmin, xmax);

      foreach (RowEntry rowEntry in rowEntries) {
        YAxisDescriptor yAxis = rowEntry.DataRow.YAxis;

        double ymin, ymax;
        GetClippingRange(yAxis.MinValue, yAxis.MaxValue, out ymin, out ymax);
        SetClipY(rowEntry, ymin, ymax);
      }

      canvasUI.Invalidate();
    }

    /// <summary>
    /// Calculates the required clipping range such that the specified min/max values
    /// visible including a small padding.
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="clipFrom"></param>
    /// <param name="clipTo"></param>
    private static void GetClippingRange(double minValue, double maxValue, out double clipFrom, out double clipTo) {
      if (minValue == double.MaxValue || maxValue == double.MinValue) {
        clipFrom = -0.1;
        clipTo = 1.1;
      } else {
        double padding = (maxValue - minValue)*0.05;
        clipFrom = minValue - padding;
        clipTo = maxValue + padding;

        if (Math.Abs(clipTo - clipFrom) < double.Epsilon * 5) {
          clipFrom -= 0.1;
          clipTo += 0.1;
        }
      }
    }

    private void SetClipX(double x1, double x2) {
      xAxisGrid.ClippingArea = new RectangleD(x1,
                                              xAxisGrid.ClippingArea.Y1,
                                              x2,
                                              xAxisGrid.ClippingArea.Y2);

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
      xAxisGrid.ClippingArea = new RectangleD(xAxisGrid.ClippingArea.X1,
                                              y1,
                                              xAxisGrid.ClippingArea.X2,
                                              y2);

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

    /// <summary>
    /// Creates the shapes for the data of the given row and stores them.
    /// </summary>
    /// <param name="row">Datarow, whose data items should be converted to shapes</param>
    private void InitLineShapes(IDataRow row) {
      RowEntry rowEntry = new RowEntry(row);
      rowEntries.Add(rowEntry);
      rowToRowEntry[row] = rowEntry;

      if ((row.RowSettings.LineType == DataRowType.SingleValue)) {
        if (row.Count > 0) {
          LineShape lineShape = new HorizontalLineShape(0, row[0], double.MaxValue, row[0], row.RowSettings.Color, row.RowSettings.Thickness,
                                                        row.RowSettings.Style);
          rowEntry.LinesShape.AddShape(lineShape);
        }
      } else if (row.RowSettings.LineType == DataRowType.Points) {
        rowEntry.ShowMarkers(true);      //no lines, only markers are shown!!
        for (int i = 0; i < row.Count; i++)
          rowEntry.LinesShape.AddMarkerShape(new MarkerShape(i, row[i], 8, row.RowSettings.Color));
      } else if (row.RowSettings.LineType == DataRowType.Normal) {
        rowEntry.ShowMarkers(row.RowSettings.ShowMarkers);
        for (int i = 1; i < row.Count; i++) {
          LineShape lineShape = new LineShape(i - 1, row[i - 1], i, row[i], row.RowSettings.Color, row.RowSettings.Thickness, row.RowSettings.Style);
          rowEntry.LinesShape.AddShape(lineShape);
          rowEntry.LinesShape.AddMarkerShape(new MarkerShape(i - 1, row[i - 1], 8, row.RowSettings.Color));
        }
        if (row.Count > 0) {
          rowEntry.LinesShape.AddMarkerShape(new MarkerShape((row.Count - 1), row[(row.Count - 1)], 8, row.RowSettings.Color));
        }
      }

      ZoomToFullView();
    }

    /// <summary>
    /// Handles the event, when a value of a datarow was changed
    /// </summary>
    /// <param name="row">row in which the data was changed</param>
    /// <param name="value">new value of the data point</param>
    /// <param name="index">index in the datarow of the changed datapoint</param>
    /// <param name="action">the performed action (added, modified, deleted)</param>
    private void OnRowValueChanged(IDataRow row, double value, int index, Action action) {
      RowEntry rowEntry = rowToRowEntry[row];

      if (row.RowSettings.LineType == DataRowType.SingleValue) {
        if (action == Action.Added) {
          LineShape lineShape = new HorizontalLineShape(0, row[0], double.MaxValue, row[0], row.RowSettings.Color, row.RowSettings.Thickness,
                                                        row.RowSettings.Style);
          rowEntry.LinesShape.AddShape(lineShape);
        } else if(action==Action.Deleted) {
          throw new ArgumentException("It is unwise to delete the only value of the SinglevalueRow!!");
        }else if(action ==Action.Modified){
          LineShape lineShape = rowEntry.LinesShape.GetShape(0);
          lineShape.Y1 = value;
          lineShape.Y2 = value;
        }
      } else if (row.RowSettings.LineType == DataRowType.Points) {
        if (action == Action.Added) {
          if(rowEntry.LinesShape.Count==0)
            rowEntry.LinesShape.AddMarkerShape(new MarkerShape(0, row[0], 8, row.RowSettings.Color));
          if (index > 0 && index == rowEntry.LinesShape.Count + 1) {
            LineShape lineShape = new LineShape(index - 1, row[index - 1], index, row[index], row.RowSettings.Color, row.RowSettings.Thickness,
                                                row.RowSettings.Style);
            rowEntry.LinesShape.AddShape(lineShape);
            rowEntry.LinesShape.AddMarkerShape(new MarkerShape(index, row[index], 8, row.RowSettings.Color));
          } else {
            throw new ArgumentException("Adding a value is only possible at the end of a row!");
          }
        } else if (action == Action.Modified) {
          // not the first value
          if (index > 0) {
            rowEntry.LinesShape.GetShape(index - 1).Y2 = value;
            ((MarkerShape) rowEntry.LinesShape.markersShape.GetShape(index - 1)).Y = value;
          }

          // not the last value
          if (index < row.Count - 1) {
            rowEntry.LinesShape.GetShape(index).Y1 = value;
            ((MarkerShape) rowEntry.LinesShape.markersShape.GetShape(index)).Y = value;
          }
        } else if(action == Action.Deleted) {
          if (index == row.Count - 1)
            rowEntry.LinesShape.RemoveMarkerShape(rowEntry.LinesShape.markersShape.GetShape(index));
          else
            throw new NotSupportedException("Deleting of values other than the last one is not supported!");
        }

      } else if (row.RowSettings.LineType == DataRowType.Normal) {
        if (index > rowEntry.LinesShape.Count + 1) {
          throw new NotImplementedException();
        }

        if (action == Action.Added) {
          if (rowEntry.LinesShape.Count == 0)
            rowEntry.LinesShape.AddMarkerShape(new MarkerShape(0, row[0], 8, row.RowSettings.Color));
          if (index > 0 && index == rowEntry.LinesShape.Count + 1) {
            LineShape lineShape = new LineShape(index - 1, row[index - 1], index, row[index], row.RowSettings.Color, row.RowSettings.Thickness,
                                                row.RowSettings.Style);
            rowEntry.LinesShape.AddShape(lineShape);
            rowEntry.LinesShape.AddMarkerShape(new MarkerShape(index, row[index], 8, row.RowSettings.Color));
          }
        } else if (action == Action.Modified) {
          // not the first value
          if (index > 0) {
            rowEntry.LinesShape.GetShape(index - 1).Y2 = value;
            ((MarkerShape) rowEntry.LinesShape.markersShape.GetShape(index - 1)).Y = value;
          }

          // not the last value
          if (index < row.Count - 1) {
            rowEntry.LinesShape.GetShape(index).Y1 = value;
            ((MarkerShape) rowEntry.LinesShape.markersShape.GetShape(index)).Y = value;
          }
        } else if (action == Action.Deleted) {
          if (index == row.Count - 1) {
            rowEntry.LinesShape.RemoveMarkerShape(rowEntry.LinesShape.markersShape.GetShape(index));
            rowEntry.LinesShape.RemoveShape(rowEntry.LinesShape.GetShape(index));
          } else
            throw new NotSupportedException("Deleting of values other than the last one is not supported!");
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
          clippingArea = Translate.ClippingArea(startPoint, endPoint, rowEntry.LinesShape.ClippingArea,
                                                rowEntry.LinesShape.Viewport);
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
      rectangleShape.Rectangle = Transform.ToWorld(rectangle, userInteractionShape.Viewport,
                                                   userInteractionShape.ClippingArea);
      canvasUI.Invalidate();
    }

    private void canvasUI1_KeyDown(object sender, KeyEventArgs e) {}

    private void canvasUI1_MouseDown(object sender, MouseEventArgs e) {
      Focus();

      if (e.Button == MouseButtons.Right) {
        valueToolTip.Hide(this);
        mouseEventListener = null;
        this.contextMenu.Show(PointToScreen(e.Location));
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
      if (currentMousePos == e.Location)
        return;
      if (mouseEventListener != null) {
        mouseEventListener.MouseMove(sender, e);
      }

      currentMousePos = e.Location;
    }

    private void canvasUI_MouseUp(object sender, MouseEventArgs e) {
      if (mouseEventListener != null) {
        mouseEventListener.MouseUp(sender, e);
      }

      mouseEventListener = toolTipListener;
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

          double y1 = world.Y - (world.Y - rowEntry.LinesShape.ClippingArea.Y1)*zoomFactor;
          double y2 = world.Y + (rowEntry.LinesShape.ClippingArea.Y2 - world.Y)*zoomFactor;

          SetClipY(rowEntry, y1, y2);
        }

        canvasUI.Invalidate();
      }
    }

    #endregion

    protected override void OnPaint(PaintEventArgs e) {
      UpdateLayout();
      base.OnPaint(e);
    }

    private class LinesShape : WorldShape {
      public readonly CompositeShape markersShape = new CompositeShape();

      public void UpdateStyle(IDataRow row) {
        foreach (IShape shape in shapes) {
          LineShape lineShape = shape as LineShape;
          if (lineShape != null) {
            lineShape.LSColor = row.RowSettings.Color;
            lineShape.LSDrawingStyle = row.RowSettings.Style;
            lineShape.LSThickness = row.RowSettings.Thickness;
          }
        }
        markersShape.ShowChildShapes = row.RowSettings.ShowMarkers;
      }

      /// <summary>
      /// Draws all Shapes in the chart
      /// </summary>
      /// <param name="graphics"></param>
      public override void Draw(Graphics graphics) {
        GraphicsState gstate = graphics.Save();

        graphics.SetClip(Viewport);
        foreach (IShape shape in shapes) {
          // draw child shapes using our own clipping area
          shape.Draw(graphics);
        }
        markersShape.Draw(graphics);
        graphics.Restore(gstate);
      }

      public void AddMarkerShape(IShape shape) {
        shape.Parent = this;
        markersShape.AddShape(shape);
      }

      public void RemoveMarkerShape(IShape shape) {
        shape.Parent = this;
        markersShape.RemoveShape(shape);
      }

      public int Count {
        get { return shapes.Count; }
      }

      public LineShape GetShape(int index) {
        return (LineShape) shapes[index]; //shapes[0] is markersShape!!
      }
    }

    private class RowEntry {
      private readonly IDataRow dataRow;

      private readonly LinesShape linesShape = new LinesShape();

      public RowEntry(IDataRow dataRow) {
        this.dataRow = dataRow;
        linesShape.markersShape.Parent = linesShape;
      }

      public IDataRow DataRow {
        get { return dataRow; }
      }

      public LinesShape LinesShape {
        get { return linesShape; }
      }

      public void ShowMarkers(bool flag) {
        linesShape.markersShape.ShowChildShapes = flag;
      }
    }

    private class YAxisInfo {
      private readonly YAxisGrid grid = new YAxisGrid();
      private readonly YAxis yAxis = new YAxis();

      public YAxisGrid Grid {
        get { return grid; }
      }

      public YAxis YAxis {
        get { return yAxis; }
      }
    }
  }
}