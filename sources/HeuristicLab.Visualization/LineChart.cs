using System;
using System.Drawing;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization
{
  public partial class LineChart : ViewBase
  {
    private readonly IChartDataRowsModel model;
    private Color[] lineColors;

    /// <summary>
    /// This constructor shouldn't be called. Only required for the designer.
    /// </summary>
    public LineChart()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Initializes the chart.
    /// </summary>
    /// <param name="model">Referenz to the model, for data</param>
    public LineChart(IChartDataRowsModel model) : this()
    {
      if (model == null)
      {
        throw new NullReferenceException("Model cannot be null.");
      }

      this.model = model;

      //TODO: read line colors instead of static ones
      lineColors = new Color[3];
      lineColors[0] = Color.Red;
      lineColors[1] = Color.Green;
      lineColors[2] = Color.Blue;
      
      //TODO: correct Rectangle to fit
      RectangleD clientRectangle = new RectangleD(ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Right,
                                                  ClientRectangle.Bottom);
      canvasUI1.MainCanvas.WorldShape = new WorldShape(clientRectangle, clientRectangle);
      Reset();
    }

    /// <summary>
    /// Resets the line chart by deleting all shapes and reloading all data from the model.
    /// </summary>
    private void Reset()
    {
      // TODO an neues model interface anpassen   
      throw new NotImplementedException();
      
//      BeginUpdate();
//
//      // TODO clear existing shapes
//
//      WorldShape mainWorld = canvasUI1.MainCanvas.WorldShape;
//
//      double spacing = mainWorld.BoundingBox.Width/model.Columns.Count;
//      double oldX = 0;
//      double currentX = spacing;
//      ChartDataRowsModelColumn oldColumn = null;
//      // reload data from the model and create shapes
//      foreach (ChartDataRowsModelColumn column in model.Columns)
//      {
//        if (oldColumn != null)
//        {
//          if (column.Values != null)
//          {
//            for (int i = 0; i < column.Values.Length; i++)
//            {
//              LineShape line = new LineShape(oldX, oldColumn.Values[i], currentX, column.Values[i], 0, lineColors[i]);
//              mainWorld.AddShape(line);
//            }
//            oldX = currentX;
//            currentX += spacing;
//          }
//          oldColumn = column;
//        }
//        else
//        {
//          oldColumn = column;
//        }
//
//        canvasUI1.Invalidate();
//
//        //   AddColumn(column.ColumnId, column.Values);
//      }
//
//      EndUpdate();
    }

    /// <summary>
    /// Event handler which gets called when data in the model changes.
    /// </summary>
    /// <param name="type">Type of change</param>
    /// <param name="columnId">Id of the changed column</param>
    /// <param name="values">Values contained within the changed column</param>
    private void OnDataChanged(ChangeType type, long columnId, double[] values)
    {
      switch (type)
      {
        case ChangeType.Add:
          AddColumn(columnId, values);
          break;
        case ChangeType.Modify:
          ModifyColumn(columnId, values);
          break;
        case ChangeType.Remove:
          RemoveColumn(columnId);
          break;
        default:
          throw new ArgumentOutOfRangeException("type");
      }
    }

    /// <summary>
    /// Adds a new column to the chart.
    /// </summary>
    /// <param name="columnId">Id of the column</param>
    /// <param name="values">Values of the column</param>
    private void AddColumn(long columnId, double[] values)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Modifies an existing column of the chart.
    /// </summary>
    /// <param name="columnId">Id of the column</param>
    /// <param name="values">Values of the column</param>
    private void ModifyColumn(long columnId, double[] values)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Removes a column from the chart.
    /// </summary>
    /// <param name="columnId">Id of the column</param>
    private void RemoveColumn(long columnId)
    {
      throw new NotImplementedException();
    }

    #region Add-/RemoveItemEvents

    protected override void AddItemEvents()
    {
      // TODO an neues model interface anpassen   
      throw new NotImplementedException();
//      base.AddItemEvents();
//      model.ColumnChanged += OnDataChanged;
    }

    protected override void RemoveItemEvents()
    {
      // TODO an neues model interface anpassen   
      throw new NotImplementedException();

//      base.RemoveItemEvents();
//      model.ColumnChanged -= OnDataChanged;
    }

    #endregion

    #region Begin-/EndUpdate

    private int beginUpdateCount = 0;

    public void BeginUpdate()
    {
      beginUpdateCount++;
    }

    public void EndUpdate()
    {
      if (beginUpdateCount == 0)
      {
        throw new InvalidOperationException("Too many EndUpdates.");
      }

      beginUpdateCount--;

      if (beginUpdateCount == 0)
      {
        Invalidate();
      }
    }

    #endregion
  }
}