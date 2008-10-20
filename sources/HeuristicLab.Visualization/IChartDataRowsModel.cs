using System.Collections.Generic;

namespace HeuristicLab.Visualization {
  public interface IChartDataRowsModel {
    event ChartDataRowsModelColumnChangedHandler ColumnChanged;
    List<ChartDataRowsModelColumn> Columns { get; }
  }
}
