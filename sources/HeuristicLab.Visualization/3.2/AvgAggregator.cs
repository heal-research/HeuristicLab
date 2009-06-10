using System;
using System.Collections.Generic;

namespace HeuristicLab.Visualization {
  public interface IAggregator {
    void AddWatch(IDataRow dataRow);
    void RemoveWatch(IDataRow dataRow);
  }

  public class AvgAggregator : DataRowBase, IAggregator {
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

    private readonly List<IDataRow> dataRowWatches = new List<IDataRow>();
    private double curAvgValue;

    private void dataRow_ValueChanged(IDataRow row, double value, int index, Action action) {
      switch (action) {
        case Action.Added:
          refreshValue(value, action);
          break;
        case Action.Modified:
          refreshValue();
          break;
        case Action.Deleted:
          refreshValue(value, action);
          break;
        default:
          throw new ArgumentOutOfRangeException("action");
      }


    }

    private void dataRow_ValuesChanged(IDataRow row, double[] values, int index, Action action) {
      for (int i = 0; i < values.Length; i++) {
        refreshValue();
      }
    }

    private void dataRow_DataRowChanged(IDataRow row) {
      refreshValue();
    }

    private int count;
    
    public AvgAggregator() {
      curAvgValue = 0;
      count = 0;
      this.RowSettings.LineType = DataRowType.SingleValue;
    }

    private void refreshValue() {
      curAvgValue = double.MinValue;
      double tmpSum = 0;
      count = 0;
      foreach (IDataRow rows in dataRowWatches) {
        for (int i = 0; i < rows.Count; i++) {
          tmpSum += rows[i];
          count++;
        }
      }
      if (count == 0) curAvgValue = 0;
      else curAvgValue = tmpSum / count;
      OnValueChanged(curAvgValue, 0, Action.Modified);
    }
    private void refreshValue(double newVal, Action action) {
      double temp = curAvgValue * count;

      switch (action) {
        case Action.Added:
          temp += newVal;
          count++;
          break;
        case Action.Modified:
          throw new InvalidOperationException();
        case Action.Deleted:
          temp -= newVal;
          count--;
          break;
        default:
          throw new ArgumentOutOfRangeException("action");
      }

      curAvgValue = temp / count;
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

    public override void RemoveValues(int index, int countVals) {
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