using System;
using System.Drawing;
using System.Collections.Generic;

namespace HeuristicLab.Visualization {
  public delegate void DataRowChangedHandler(IDataRow row);
  public delegate void ValuesChangedHandler(IDataRow row, double[] values, int index);
  public delegate void ValueChangedHandler(IDataRow row, double value, int index);

  public class DataRow : IDataRow {
    private string label = "";
    private Color color = Color.Black;
    private int thickness = 2;
    private DrawingStyle style = DrawingStyle.Solid;
    private List<double> dataRow = new List<double>();

    /// <summary>
    /// Raised when data row data changed. Should cause redraw in the view.
    /// </summary>
    public event DataRowChangedHandler DataRowChanged;

    protected void OnDataRowChanged(IDataRow row) {
      if (DataRowChanged != null) {
        DataRowChanged(this);
      }
    }

    public event ValuesChangedHandler ValuesChanged;

    protected void OnValuesChanged(double[] values, int index) {
      if (ValuesChanged != null) {
        ValuesChanged(this, values, index);
      }
    }

    public event ValueChangedHandler ValueChanged;

    protected void OnValueChanged(double value, int index) {
      if (ValueChanged != null) {
        ValueChanged(this, value, index);
      }
    }

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

    public void AddValue(double value) {
      dataRow.Add(value);
      OnValueChanged(value, dataRow.Count - 1);
    }

    public void AddValue(double value, int index) {
      dataRow.Add(value);
      OnValueChanged(value, index);
    }

    public void AddValues(double[] values) {
      int startInd = dataRow.Count;

      foreach (double d in values) {
        dataRow.Add(d);
      }

      OnValuesChanged(values, startInd); 
    }

    public void AddValues(double[] values, int index) {
      //check if index to start changes is valid
      if (index + values.Length < dataRow.Count) {
        foreach (double d in values) {
          dataRow.Add(d);
        }
        OnValuesChanged(values, index);
      } else {
        throw new System.IndexOutOfRangeException();
      }
    }

    public void ModifyValue(double value, int index) {
      throw new NotImplementedException();
      // TODO ValueChangedEvent auslösen
    }

    public void ModifyValues(double[] values, int index) {
      throw new NotImplementedException();
      // TODO ValuesChangedEvent auslösen
    }

    public void RemoveValue(int index) {
      throw new NotImplementedException();
      // TODO ValueChangedEvent auslösen
    }

    public void RemoveValues(int index, int count) {
      throw new NotImplementedException();
      // TODO ValuesChangedEvent auslösen
    }

    public int Count {
      get { return dataRow.Count; }
    }

    public double this[int index] {
      get { return dataRow[index]; }
      set {
        dataRow[index] = value;
        OnValueChanged(value, index);
      }
    }
  }
}