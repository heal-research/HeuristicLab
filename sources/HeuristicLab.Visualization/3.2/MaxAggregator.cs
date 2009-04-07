using System;
using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  public class MaxAggregator : DataRowBase {
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
    double curMaxValue;

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
      if (curMaxValue < newVal) {
        curMaxValue = newVal;
        OnValueChanged(newVal, 0, Action.Modified);
      } else {
        curMaxValue = double.MinValue;
        foreach (IDataRow rows in dataRowWatches) {
          for (int i = 0; i < rows.Count; i++) {
            if (rows[i] > curMaxValue) {
              curMaxValue = rows[i];
            }
          }
        }

      }
      // evtl nur feuern wenn sich geändert hat (jedes mal?)
      OnValueChanged(curMaxValue, 0, Action.Modified);
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
        return curMaxValue;
      }
      set {
        throw new NotSupportedException();
      }
    }

    public override double MinValue {
      get { return curMaxValue; }
    }

    public override double MaxValue {
      get { return curMaxValue; }
    }

    #endregion
  }
}
