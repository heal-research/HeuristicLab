using System;
using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  public class AvgLineAggregator : IAggregator {

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

    private string label = "";
    private Color color = Color.Black;
    private int thickness = 2;
    private DrawingStyle style = DrawingStyle.Solid;
    private DataRowType lineType = DataRowType.Normal;


    public string Label {
      get { return label; }
      set {
        label = value;
        OnDataRowChanged(this);
      }
    }

    public Color Color {
      get { return color; }
      set {
        color = value;
        OnDataRowChanged(this);
      }
    }

    public int Thickness {
      get { return thickness; }
      set {
        thickness = value;
        OnDataRowChanged(this);
      }
    }

    public DrawingStyle Style {
      get { return style; }
      set {
        style = value;
        OnDataRowChanged(this);
      }
    }

    public DataRowType LineType {
      get { return lineType; }
      set {
        lineType = value;
        OnDataRowChanged(this);
      }
    }

    public LabelProvider.ILabelProvider YAxisLabelProvider {
      get {
        throw new NotImplementedException();
      }
      set {
        throw new NotImplementedException();
      }
    }

    public void AddValue(double value) {
      dataRow.Add(value);
      OnValueChanged(value, dataRow.Count - 1, Action.Added);
    }

    public void AddValue(double value, int index) {
      throw new System.NotImplementedException();
    }

    public void AddValues(double[] values) {
      throw new NotSupportedException();
    }

    public void AddValues(double[] values, int index) {
      throw new NotSupportedException();
    }

    public void ModifyValue(double value, int index) {
      throw new NotSupportedException();
    }

    public void ModifyValues(double[] values, int index) {
      throw new NotSupportedException();
    }

    public void RemoveValue(int index) {
      throw new NotSupportedException();
    }

    public void RemoveValues(int index, int count) {
      throw new NotSupportedException();
    }

    public int Count {
      get { return dataRowWatches.Count; }
    }

    public double this[int index] {
      get { return dataRow[index]; }
      set {
        dataRow[index] = value;
        OnValueChanged(value, index, Action.Modified);
      }
    }

    public event ValuesChangedHandler ValuesChanged;

    protected void OnValuesChanged(double[] values, int index, Action action) {
      if (ValuesChanged != null) {
        ValuesChanged(this, values, index, action);
      }
    }

    public event ValueChangedHandler ValueChanged;

    protected void OnValueChanged(double value, int index, Action action) {
      if (ValueChanged != null) {
        ValueChanged(this, value, index, action);
      }
    }

    public event DataRowChangedHandler DataRowChanged;

    protected void OnDataRowChanged(IDataRow row) {
      if (DataRowChanged != null) {
        DataRowChanged(this);
      }
    }

    #endregion
  }
}
