using System;
using System.Drawing;

namespace HeuristicLab.Visualization {
  public delegate void DataRowChangedHandler(IDataRow row);
  public delegate void ValuesChangedHandler(IDataRow row, double[] values, int index);
  public delegate void ValueChangedHandler(IDataRow row, double value, int index);

  public class DataRow : IDataRow {
    private string label = "";
    private Color color = Color.Black;
    private int thickness = 2;
    private DrawingStyle style = DrawingStyle.Solid;

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
      throw new NotImplementedException();
      // TODO ValueChangedEvent auslösen
    }

    public void AddValue(double value, int index) {
      throw new NotImplementedException();
      // TODO ValueChangedEvent auslösen
    }

    public void AddValues(double[] values) {
      throw new NotImplementedException();
      // TODO ValuesChangedEvent auslösen
    }

    public void AddValues(double[] values, int index) {
      throw new NotImplementedException();
      // TODO ValuesChangedEvent auslösen
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
      get { throw new NotImplementedException(); }
    }

    public double this[int index] {
      get { throw new NotImplementedException(); }
      set {
        throw new NotImplementedException();
        // TODO ValueChangedEvent auslösen
      }
    }
  }
}