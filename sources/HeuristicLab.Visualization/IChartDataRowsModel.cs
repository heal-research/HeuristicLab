using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Visualization.LabelProvider;
using HeuristicLab.Visualization.Options;

namespace HeuristicLab.Visualization {
  public interface IChartDataRowsModel : IItem {
    string Title { get; set; }
    List<IDataRow> Rows { get; }
    ILabelProvider XAxisLabelProvider { get; set; }

    List<YAxisDescriptor> YAxes { get; }

    void AddDataRow(IDataRow row);
    void RemoveDataRow(IDataRow row);

    int MaxDataRowValues { get; }

    ViewSettings ViewSettings { get; set; }

    event ModelChangedHandler ModelChanged;
    event DataRowAddedHandler DataRowAdded;
    event DataRowRemovedHandler DataRowRemoved;
  }
}