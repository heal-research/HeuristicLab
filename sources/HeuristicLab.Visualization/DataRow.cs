using System;
using System.Drawing;
using System.Collections.Generic;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public enum Action {
    Added,
    Modified,
    Deleted
  }

  public delegate void DataRowChangedHandler(IDataRow row);
  public delegate void ValuesChangedHandler(IDataRow row, double[] values, int index, Action action);
  public delegate void ValueChangedHandler(IDataRow row, double value, int index, Action action);

  public class DataRow : IDataRow {
    private string label = "";
    private Color color = Color.Black;
    private int thickness = 2;
    private DrawingStyle style = DrawingStyle.Solid;
    private DataRowType lineType = DataRowType.Normal;
    private readonly List<double> dataRow = new List<double>();

    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.##");

    // TODO implement calculation of min and max values
    private double minValue = double.MaxValue;
    private double maxValue = double.MinValue;

    public DataRowType LineType{
      get { return lineType; }
      set { 
        lineType = value;
        OnDataRowChanged(this);
      }
    }

    public ILabelProvider YAxisLabelProvider {
      get { return labelProvider; }
      set {
        this.labelProvider = value;
        OnDataRowChanged(this);
      }
    }

    public DataRow() {
    }
    
    public DataRow(string label) {
      this.Label = label;
    }

    public DataRow(string label, Color color, int thickness, DrawingStyle style, List<double> dataRow) {
     this.Label = label;
      this.Color = color;
      this.Thickness = thickness;
      this.Style = style;
      this.dataRow = dataRow;
    }

    public event DataRowChangedHandler DataRowChanged;

    protected void OnDataRowChanged(IDataRow row) {
      if (DataRowChanged != null) {
        DataRowChanged(this);
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

    private bool showYAxis = true;

    public virtual bool ShowYAxis {
      get { return showYAxis; }
      set {
        showYAxis = value;
        OnDataRowChanged(this);
      }
    }

    public void AddValue(double value) {
      UpdateMinMaxValue(value);

      dataRow.Add(value);
      OnValueChanged(value, dataRow.Count - 1, Action.Added);
    }

    public void AddValue(double value, int index) {
      //check if index is valid
      if (index >= 0 && index < dataRow.Count) {
        dataRow.Insert(index, value);
        OnValueChanged(value, index, Action.Added);
      } else {
        throw new IndexOutOfRangeException();
      }   
    }

    public void AddValues(double[] values) {
      int startInd = dataRow.Count;

      foreach (double d in values) {
        dataRow.Add(d);
      }
      OnValuesChanged(values, startInd, Action.Added); 
    }

    public void AddValues(double[] values, int index) {
      int j = index;

      //check if index to start changes is valid
      if (index >=0 && (index + values.Length) < dataRow.Count) {
        foreach (double d in values) {
          dataRow.Insert(j, d);
          j++;
        }
        OnValuesChanged(values, index, Action.Added);
      } else {
        throw new IndexOutOfRangeException();
      }
    }

    public void ModifyValue(double value, int index) {
      //check if index is valid
      if (index >= 0 && index < dataRow.Count) {
        dataRow[index] = value;
        OnValueChanged(value, index, Action.Modified);
      } else {
        throw new IndexOutOfRangeException();
      }
    }

    public void ModifyValues(double[] values, int index) {
      int startInd = index;
      int modInd = index;

      //check if index to start modification is valid
      if (startInd >=0 && startInd + values.Length < dataRow.Count) {
        foreach (double d in values) {
          dataRow[modInd] = d;
          modInd++;
        }
        OnValuesChanged(values, startInd, Action.Modified);
      } else {
        throw new IndexOutOfRangeException();
      }
    }

    public void RemoveValue(int index) {
      double remVal = dataRow[index];
      //check if index is valid
      if (index >= 0 && index < dataRow.Count) {
        dataRow.RemoveAt(index);
        OnValueChanged(remVal, index, Action.Deleted);
      } else {
        throw new IndexOutOfRangeException();
      }
    }

    public void RemoveValues(int index, int count) {
      double[] remValues = new double[count]; //removed values
      int j = 0;

      //check if count is valid
      if (count > 0) {
        //check if index is valid
        if ((index >= 0) && (index + count <= dataRow.Count)) {
          for (int i = index; i < (index + count); i++) {
            remValues.SetValue(i, j);
            dataRow.RemoveAt(i);
            j++;
          }
          OnValuesChanged(remValues, index, Action.Deleted);
        } else {
          throw new IndexOutOfRangeException();
        }
      } else {
        throw new Exception("parameter count must be > 0!");
      }
    }

    public int Count {
      get { return dataRow.Count; }
    }

    public double this[int index] {
      get { return dataRow[index]; }
      set {
        dataRow[index] = value;
        OnValueChanged(value, index, Action.Modified);
      }
    }

    public double MinValue {
      get { return minValue; }
    }

    public double MaxValue {
      get { return maxValue; }
    }

    private void UpdateMinMaxValue(double value) {
      maxValue = Math.Max(value, maxValue);
      minValue = Math.Min(value, minValue);
    }
  }
}