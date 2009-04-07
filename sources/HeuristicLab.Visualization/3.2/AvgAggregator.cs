using System;
using System.Collections.Generic;

namespace HeuristicLab.Visualization {
  public class AvgAggregator : DataRowBase {
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

    private List<IDataRow> dataRowWatches = new List<IDataRow>();
    private double curAvgValue;

    private void dataRow_ValueChanged(IDataRow row, double value, int index, Action action) {
      refreshValue(value);
    }

    private void dataRow_ValuesChanged(IDataRow row, double[] values, int index, Action action) {
      for (int i = 0; i < values.Length; i++) {
        refreshValue(values[i]);
      }
    }

    private void dataRow_DataRowChanged(IDataRow row) {
      refreshValue(double.MinValue);
    }

    private void refreshValue(double newVal) {
      //alle durchlaufen und neues min berechnen; verbesserung: merken in welcher row lowest wert steckt

      curAvgValue = double.MinValue;
      double tmpSum = 0;
      int count = 0;
      foreach (IDataRow rows in dataRowWatches) {
        for (int i = 0; i < rows.Count; i++) {
          tmpSum += rows[i];
          count++;
        }
      }
      curAvgValue = tmpSum/count;
      // evtl nur feuern wenn sich geändert hat (jedes mal?)
      OnValueChanged(curAvgValue, 0, Action.Modified);
    }

    #region IDataRow Members

    public override void AddValue(double value) {
      throw new NotSupportedException();
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

    public override void RemoveValues(int index, int count) {
      throw new NotSupportedException();
    }

    public override int Count {
      get { return 1; }
    }

    public override double this[int index] {
      get { return curAvgValue; }
      set { throw new NotSupportedException(); }
    }

    public override double MinValue {
      get { return curAvgValue; }
    }

    public override double MaxValue {
      get { return curAvgValue; }
    }

    #endregion
  }
}