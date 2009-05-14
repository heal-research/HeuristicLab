using System;
using System.Collections.Generic;

namespace HeuristicLab.Visualization {
  public class FloatingAvgAggregator : DataRowBase {

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
      refreshValue();
    }

    void dataRow_DataRowChanged(IDataRow row) {
      refreshValue();
    }

    private int area = 5;
    private void refreshValue() {

      if (dataRowWatches.Count >= 1) {
        IDataRow watchedRow = dataRowWatches[0];
        
        dataRow.Clear();
        OnDataRowChanged(this);

        for (int i = 0; i < watchedRow.Count; i++) {

          double avgVal = 0;
          int count = 0;
          for (int j = Math.Max(0, i-area); j < Math.Min(watchedRow.Count, i+area); j++) {
            avgVal += watchedRow[j];
            count++;
          }

          if (count >= 1)
            avgVal /= count;

          dataRow.Add(avgVal);

          OnValueChanged(avgVal, dataRow.Count - 1, Action.Added);
        }
      }
      //OnValueChanged(avgVal, dataRow.Count - 1, Action.Added);
      OnValuesChanged(dataRow.ToArray(), 0, Action.Modified);
      //OnDataRowChanged(this);                                     
    }

#pragma warning disable 168
    private void refreshLastValues(IDataRow row) {
#pragma warning restore 168
      refreshValue();
    }

    #region IDataRow Members

    public override void AddValue(double value) {
      throw new NotSupportedException();
//      dataRow.Add(value);
//      OnValueChanged(value, dataRow.Count - 1, Action.Added);
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

    #endregion
  }
}
