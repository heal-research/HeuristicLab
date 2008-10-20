using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;


namespace HeuristicLab.Visualization{
  public class ChartDataRowsModel : ChartDataModelBase, IChartDataRowsModel {

    private readonly ListDictionary dataRows;
    
    public ChartDataRowsModel(){

      dataRows = new ListDictionary();
    }

    public void AddDataRow(int id){
      List<double> row = new List<double>();

      dataRows.Add(id, row);
    }

    public void PushData(int dataRowId, double value){
        ((List<double>)dataRows[dataRowId]).Add(value);

    }

    public event ChartDataRowsModelColumnChangedHandler ColumnChanged;

    private void RaiseColumnChanged(ChangeType type, long columnId, double[] values) {
      if (ColumnChanged != null) {
        ColumnChanged(type, columnId, values);
      }
    }

    public List<ChartDataRowsModelColumn> Columns {
      get { throw new NotImplementedException(); }
    }
  }
}