using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public interface IChartDataRowsModel : IItem {
    string Title { get; set; }
    //string XAxisLabel { get; set; }
    //List<string> XLabels { get; }
    List<IDataRow> Rows { get; }
    ILabelProvider XAxisLabelProvider { get; set; }

    void AddDataRow(IDataRow row);
    void RemoveDataRow(IDataRow row);

    //void AddLabel(string label);
    //void AddLabel(string label, int index);
    //void AddLabels(string[] labels);
    //void AddLabels(string[] labels, int index);
    //void ModifyLabel(string label, int index);
    //void ModifyLabels(string[] labels, int index);
    //void RemoveLabel(int index);
    //void RemoveLabels(int index, int count);

    int MaxDataRowValues { get; }

    event ModelChangedHandler ModelChanged;
    event DataRowAddedHandler DataRowAdded;
    event DataRowRemovedHandler DataRowRemoved;
  }
}