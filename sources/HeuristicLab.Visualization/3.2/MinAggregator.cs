using System;
using System.Collections.Generic;

namespace HeuristicLab.Visualization {
  public class MinAggregator : DataRowBase, IAggregator {
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

    public MinAggregator() {
      this.RowSettings.LineType=DataRowType.SingleValue;
    }

    readonly List<IDataRow> dataRowWatches = new List<IDataRow>();
    double curMinValue;

    void dataRow_ValueChanged(IDataRow row, double value, int index, Action action) {
      refreshValue(value);
    }

    void dataRow_ValuesChanged(IDataRow row, double[] values, int index, Action action) {
      for (int i = 0; i < values.Length; i++) {
        refreshValue(values[i]);
      }
    }

    void dataRow_DataRowChanged(IDataRow row) {
      refreshValue(double.MaxValue);
    }

    private void refreshValue(double newVal) {
      //alle durchlaufen und neues min berechnen; verbesserung: merken in welcher row lowest wert steckt
      if (curMinValue > newVal) {
        curMinValue = newVal;
        OnValueChanged(newVal, 0, Action.Modified);
      } else {
        curMinValue = double.MaxValue;
        foreach (IDataRow rows in dataRowWatches) {
          for (int i = 0; i < rows.Count; i++) {
            if (rows[i] < curMinValue) {
              curMinValue = rows[i];
            }
          }
        }

      }
      // evtl nur feuern wenn sich geändert hat (jedes mal?)
      OnValueChanged(curMinValue, 0, Action.Modified);
    }

    #region IDataRow Members

    public override void AddValue(double value) {
      OnValueChanged(2, 0, Action.Added);
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
      get {
        return curMinValue;
      }
      set {
        throw new NotSupportedException();
      }
    }

    public override double MinValue {
      get { return curMinValue; }
    }

    public override double MaxValue {
      get { return curMinValue; }
    }

    #endregion
  }
}
