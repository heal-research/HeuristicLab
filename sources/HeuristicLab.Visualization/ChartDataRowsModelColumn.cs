namespace HeuristicLab.Visualization {
  public class ChartDataRowsModelColumn {
    public readonly int ColumnId;
    public readonly double[] Values;

    public ChartDataRowsModelColumn(int columnId, double[] values) {
      ColumnId = columnId;
      Values = values;
    }
  }
}