using System;
using System.Collections.Generic;

namespace HeuristicLab.Visualization {
  public class AvgLineAggregator : DataRowBase {

    private readonly List<double> dataRow = new List<double>();

    #region IAggregator Members

    public void AddWatch(IDataRow dataRow) {
      dataRowWatches.Add(dataRow);
      dataRow.ValueChanged += dataRow_ValueChanged;
      dataRow.ValuesChanged += dataRow_ValuesChanged;
      dataRow.DataRowChanged += dataRow_DataRowChanged;
    }

    public void RemoveWatch(IDataRow dataRow) {

      dataRowWatches.Remove(dataRow);
      dataRow.DataRowChanged -= dataRow_DataRowChanged;
      dataRow.ValuesChanged -= dataRow_ValuesChanged;
      dataRow.ValueChanged -= dataRow_ValueChanged;
    }


    #endregion

    List<IDataRow> dataRowWatches = new List<IDataRow>();
    
    void dataRow_ValueChanged(IDataRow row, double value, int index, Action action) {
      refreshValue(value);
    }

    void dataRow_ValuesChanged(IDataRow row, double[] values, int index, Action action) {
      for (int i = 0; i < values.Length; i++) {
        refreshValue(values[i]);
      }
    }

    void dataRow_DataRowChanged(IDataRow row) {
      refreshValue(double.MinValue);
    }

    private void refreshValue(double newVal) {
      //alle durchlaufen und neues min berechnen; verbesserung: merken in welcher row lowest wert steckt

      //curAvgValue = double.MinValue;
      double tmpSum = 0;
      int count = dataRowWatches.Count;

      IDataRow firstRow = dataRowWatches[0];
      int count1 = firstRow.Count;
      System.Console.WriteLine("count: " + count1);

      dataRow.Clear();

      if (dataRowWatches.Count >= 2) {
        for (int i = 0; i < count1; i++) {
          tmpSum = 0;
          for (int j = 0; j < count; j++) {
            if (dataRowWatches[j].Count > i) {
              tmpSum += dataRowWatches[j][i];
            }
          }

          this.dataRow.Add(tmpSum/count);
          OnValueChanged(tmpSum / count, dataRow.Count - 1, Action.Added);
        }
      }

      // evtl nur feuern wenn sich geändert hat (jedes mal?)
      //OnDataRowChanged(this);
    }

    #region IDataRow Members

    public override void AddValue(double value) {
      dataRow.Add(value);
      OnValueChanged(value, dataRow.Count - 1, Action.Added);
    }

    public override void AddValue(double value, int index) {
      throw new NotImplementedException();
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

    public override void RemoveValues(int index, int count) {
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

    // TODO calculate min value
    public override double MinValue {
      get { return 0; }
    }

    // TODO calculate max value
    public override double MaxValue {
      get { return 0; }
    }

    #endregion
  }
}
