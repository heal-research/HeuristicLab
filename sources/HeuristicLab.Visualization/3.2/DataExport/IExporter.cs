namespace HeuristicLab.Visualization.DataExport {
  public interface IExporter {
    void Export(IChartDataRowsModel model);

    string Name { get; }
  }
}