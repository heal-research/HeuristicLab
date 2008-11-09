namespace HeuristicLab.Visualization {
  public class ChartDataRowsModelColumn {
    public  int ColumnId;
    public  double[] Values;

    public ChartDataRowsModelColumn(int columnId, double[] values) {
      ColumnId = columnId;
      Values = values;
    }
  }
}