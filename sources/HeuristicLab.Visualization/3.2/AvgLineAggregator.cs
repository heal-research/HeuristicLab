using System;
using System.Collections.Generic;
using System.Xml;

namespace HeuristicLab.Visualization {
  public class AvgLineAggregator : DataRowBase {

    private readonly List<double> dataRow = new List<double>();
    readonly List<IDataRow> dataRowWatches = new List<IDataRow>();

    #region IAggregator Members

    public void AddWatch(IDataRow watchDataRow) {
      dataRowWatches.Add(watchDataRow);
      watchDataRow.ValueChanged += dataRow_ValueChanged;
      watchDataRow.ValuesChanged += dataRow_ValuesChanged;
      watchDataRow.DataRowChanged += dataRow_DataRowChanged;
    }

    public void RemoveWatch(IDataRow watchDataRow) {
      dataRowWatches.Remove(watchDataRow);
      watchDataRow.DataRowChanged -= dataRow_DataRowChanged;
      watchDataRow.ValuesChanged -= dataRow_ValuesChanged;
      watchDataRow.ValueChanged -= dataRow_ValueChanged;
    }

    #endregion

    void dataRow_ValueChanged(IDataRow row, double value, int index, Action action) {
      switch (action) {
        case Action.Added:
          refreshLastValues(row);
          break;
        case Action.Modified:
          refreshValue();
          break;
        case Action.Deleted:
          refreshLastValues(row);
          break;
        default:
          throw new ArgumentOutOfRangeException("action");
      }
    }

    void dataRow_ValuesChanged(IDataRow row, double[] values, int index, Action action) {
//      for (int i = 0; i < values.Length; i++) {
//        refreshValue();
//      }
      refreshValue();
    }

    void dataRow_DataRowChanged(IDataRow row) {
      refreshValue();
    }

    private void refreshValue() {
      
      int count = dataRowWatches.Count;

      IDataRow firstRow = dataRowWatches[0];
      int count1 = firstRow.Count;
      Console.WriteLine("count: " + count1);

      dataRow.Clear();

      if (dataRowWatches.Count >= 2) {
        for (int i = 0; i < count1; i++) {
          double tmpSum = 0;
          for (int j = 0; j < count; j++) {
            if (dataRowWatches[j].Count > i) {
              tmpSum += dataRowWatches[j][i];
            }
          }

          dataRow.Add(tmpSum / count);
          OnValueChanged(tmpSum / count, dataRow.Count - 1, Action.Added);
        }
      }
    }

    private void refreshLastValues(IDataRow row) {

      int index = row.Count - 1;
      double curAvg = 0;

      foreach (IDataRow watch in dataRowWatches) {
        if (watch.Count >= index +1) {
          curAvg += watch[index]; 
        }
      }

      if (dataRowWatches.Count > 0)
        curAvg /= dataRowWatches.Count;


      if (dataRow.Count <= index) {
        dataRow.Add(curAvg);
        OnValueChanged(curAvg, dataRow.Count - 1, Action.Added);     
      }
      else {
        dataRow[index] = curAvg;
        OnValueChanged(curAvg, dataRow.Count - 1, Action.Modified);  
      }
    }

    #region IDataRow Members

    public override void AddValue(double value) {
      dataRow.Add(value);
      OnValueChanged(value, dataRow.Count - 1, Action.Added);
    }

    public override void AddValue(double value, int index) {
      throw new NotSupportedException();
    }

    public override void AddValues(double[] values) {
      throw new NotSupportedException();
    }

    public override void AddValues(double[] values, int index) {
      throw new NotSupportedException();
    }

    public override void ModifyValue(double value, int index) {
      throw new NotSupportedException();
    }

    public override void ModifyValues(double[] values, int index) {
      throw new NotSupportedException();
    }

    public override void RemoveValue(int index) {
      throw new NotSupportedException();
    }

    public override void RemoveValues(int index, int countVals) {
      throw new NotSupportedException();
    }

    public override int Count {
      get { return dataRow.Count; } //return dataRowWatches.Count; }
    }

    public override double this[int index] {
      get { return dataRow[index]; }
      set {
        dataRow[index] = value;
        OnValueChanged(value, index, Action.Modified);
      }
    }

    public override double MinValue {
      get { return 0; }
    }

    public override double MaxValue {
      get { return 0; }
    }

    public override XmlNode ToXml(XmlDocument document)
    {
      throw new System.NotImplementedException();
    }

    public override IDataRow FromXml(XmlNode xmlNode)
    {
      throw new System.NotImplementedException();
    }

    #endregion
  }
}
