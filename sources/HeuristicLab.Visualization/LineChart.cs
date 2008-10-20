using System;
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
    /// <param name="model"></param>
    public LineChart(IChartDataRowsModel model) : this() {
      if (model == null) {
        throw new NullReferenceException("Model cannot be null.");
      }

      this.model = model;

      Reset();
    }

    /// <summary>
    /// Resets the line chart by deleting all shapes and reloading all data from the model.
    /// </summary>
    private void Reset() {
      BeginUpdate();

      // TODO clear existing shapes

      // reload data from the model and create shapes
      foreach (ChartDataRowsModelColumn column in model.Columns) {
        AddColumn(column.ColumnId, column.Values);
      }

      EndUpdate();
    }

    /// <summary>
    /// Event handler which gets called when data in the model changes.
    /// </summary>
    /// <param name="type">Type of change</param>
    /// <param name="columnId">Id of the changed column</param>
    /// <param name="values">Values contained within the changed column</param>
    private void OnDataChanged(ChangeType type, long columnId, double[] values) {
      switch (type) {
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
    private void AddColumn(long columnId, double[] values) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Modifies an existing column of the chart.
    /// </summary>
    /// <param name="columnId">Id of the column</param>
    /// <param name="values">Values of the column</param>
    private void ModifyColumn(long columnId, double[] values) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Removes a column from the chart.
    /// </summary>
    /// <param name="columnId">Id of the column</param>
    private void RemoveColumn(long columnId) {
      throw new NotImplementedException();
    }

    #region Add-/RemoveItemEvents

    protected override void AddItemEvents() {
      base.AddItemEvents();
      model.ColumnChanged += OnDataChanged;
    }

    protected override void RemoveItemEvents() {
      base.RemoveItemEvents();
      model.ColumnChanged -= OnDataChanged;
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
  }
}