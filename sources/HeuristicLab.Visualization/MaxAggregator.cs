using System;
using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  public class MaxAggregator : IAggregator {
    
    
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
        foreach (var rows in dataRowWatches) {
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

    private bool showYAxis = false;

    public bool ShowYAxis {
      get { return showYAxis; }
      set {
        showYAxis = value;
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
      OnValueChanged(2, 0, Action.Added);
    }

    public void AddValue(double value, int index) {
      throw new NotSupportedException();
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
      get { return 1; }
    }

    public double this[int index] {
      get {
        return curMaxValue;
      }
      set {
        throw new NotSupportedException();
      }
    }

    public double MinValue {
      get { throw new System.NotImplementedException(); }
    }

    public double MaxValue {
      get { throw new System.NotImplementedException(); }
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
