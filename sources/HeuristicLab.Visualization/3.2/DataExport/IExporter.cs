namespace HeuristicLab.Visualization.DataExport {
  public interface IExporter {
    void Export(IChartDataRowsModel model, LineChart chart);

    string Name { get; }
  }
}