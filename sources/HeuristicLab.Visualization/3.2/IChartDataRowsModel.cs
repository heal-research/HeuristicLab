using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Visualization.Options;

namespace HeuristicLab.Visualization {
  public interface IChartDataRowsModel : IItem {
    string Title { get; set; }

    XAxisDescriptor XAxis { get; }
    List<YAxisDescriptor> YAxes { get; }

    List<IDataRow> Rows { get; }
    void AddDataRow(IDataRow row);
    void RemoveDataRow(IDataRow row);

    int MaxDataRowValues { get; }

    ViewSettings ViewSettings { get; set; }

    YAxisDescriptor DefaultYAxis { get; }

    event ModelChangedHandler ModelChanged;
    event DataRowAddedHandler DataRowAdded;
    event DataRowRemovedHandler DataRowRemoved;
    void AddDataRows(IEnumerable<IDataRow> rows);
  }
}