using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace HeuristicLab.Visualization {
  public enum Action {
    Added,
    Modified,
    Deleted
  }

  public delegate void DataRowChangedHandler(IDataRow row);
  public delegate void ValuesChangedHandler(IDataRow row, double[] values, int index, Action action);
  public delegate void ValueChangedHandler(IDataRow row, double value, int index, Action action);

  public class DataRow : DataRowBase {
    private readonly List<double> dataRow = new List<double>();

    private double minValue = double.MaxValue;
    private double maxValue = double.MinValue;

    public DataRow() {
    }
    
    public DataRow(string label) {
      this.RowSettings.Label = label;
    }

    public DataRow(string label, Color color, int thickness, DrawingStyle style, List<double> dataRow) {
      this.RowSettings.Label = label;
      this.RowSettings.Color = color;
      this.RowSettings.Thickness = thickness;
      this.RowSettings.Style = style;
      this.dataRow = dataRow;
      this.RowSettings.ShowMarkers = true;
    }

    public DataRow(string label, Color color, int thickness, DrawingStyle style, List<double> dataRow, bool showMarkers) {
      this.RowSettings.Label = label;
      this.RowSettings.Color = color;
      this.RowSettings.Thickness = thickness;
      this.RowSettings.Style = style;
      this.RowSettings.ShowMarkers = showMarkers;
      this.dataRow = dataRow;
    }

    public override void AddValue(double value) {
      UpdateMinMaxValue(value);

      dataRow.Add(value);
      OnValueChanged(value, dataRow.Count - 1, Action.Added);
    }

    public override void AddValue(double value, int index) {
      //check if index is valid
      if (index >= 0 && index < dataRow.Count) {
        UpdateMinMaxValue(value);
        dataRow.Insert(index, value);
        OnValueChanged(value, index, Action.Added);
      } else {
        throw new IndexOutOfRangeException();
      }   
    }

    public override void AddValues(double[] values) {
      int startIndex = dataRow.Count;

      foreach (double value in values) {
        UpdateMinMaxValue(value);
        dataRow.Add(value);
      }

      OnValuesChanged(values, startIndex, Action.Added); 
    }

    public override void AddValues(double[] values, int index) {
      if (index >= 0 && index < dataRow.Count) {
        for (int i = 0; i < values.Length; i++) {
          double value = values[i];
          UpdateMinMaxValue(value);
          dataRow.Insert(index + i, value);
        }
        OnValuesChanged(values, index, Action.Added);
      } else {
        throw new IndexOutOfRangeException();
      }
    }

    public override void ModifyValue(double value, int index) {
      //check if index is valid
      if (index >= 0 && index < dataRow.Count) {
        UpdateMinMaxValue(value, index); // bad runtime but works
        dataRow[index] = value;
        OnValueChanged(value, index, Action.Modified);
      } else {
        throw new IndexOutOfRangeException();
      }
    }

    public override void ModifyValues(double[] values, int index) {
      //check if index to start modification is valid
      if (index >= 0 && index + values.Length < dataRow.Count) {
        for (int i = 0; i < values.Length; i++) {
          double value = values[i];
          UpdateMinMaxValue(value, index + i); // bad runtime but works
          dataRow[index+i] = value;
        }
        OnValuesChanged(values, index, Action.Modified);
      } else {
        throw new IndexOutOfRangeException();
      }
    }

    public override void RemoveValue(int index) {
      if (index >= 0 && index < dataRow.Count) {
        UpdateMinMaxValueForRemovedValue(index); // bad runtime but works
        double removedValue = dataRow[index];
        dataRow.RemoveAt(index);
        OnValueChanged(removedValue, index, Action.Deleted);
      } else {
        throw new IndexOutOfRangeException();
      }
    }

    public override void RemoveValues(int index, int count) {
      if (count > 0) {
        if ((index >= 0) && (index + count <= dataRow.Count)) {
          double[] removedValues = new double[count];
          for (int i = 0; i < count; i++) {
            removedValues[i] = dataRow[index + i];
            UpdateMinMaxValueForRemovedValue(index); // bad runtime but works
            dataRow.RemoveAt(index);
          }
          OnValuesChanged(removedValues, index, Action.Deleted);
        } else {
          throw new IndexOutOfRangeException();
        }
      } else {
        throw new Exception("parameter count must be > 0!");
      }
    }

    public override int Count {
      get { return dataRow.Count; }
    }

    public override double this[int index] {
      get { return dataRow[index]; }
      set {
        dataRow[index] = value;
        OnValueChanged(value, index, Action.Modified);
      }
    }

    public override double MinValue {
      get { return minValue; }
    }

    public override double MaxValue {
      get { return maxValue; }
    }

    private void UpdateMinMaxValueForRemovedValue(int removedValueIndex) {
      if (minValue == dataRow[removedValueIndex] || maxValue == dataRow[removedValueIndex]) {
        minValue = double.MaxValue;
        maxValue = double.MinValue;

        for (int i = 0; i < dataRow.Count; i++) {
          if (i != removedValueIndex) {
            UpdateMinMaxValue(dataRow[i]);
          }
        }
      }
    }

    public override XmlNode ToXml(IDataRow row, XmlDocument document)
    {
      XmlNode columnElement = document.CreateNode(XmlNodeType.Element, "row", null);

      XmlAttribute idAttr = document.CreateAttribute("label");
      idAttr.Value = row.RowSettings.Label;
      columnElement.Attributes.Append(idAttr);

      XmlAttribute attrColor = document.CreateAttribute("color");
      attrColor.Value = row.RowSettings.Color.ToArgb().ToString();
      columnElement.Attributes.Append(attrColor);

      XmlAttribute attrThickness = document.CreateAttribute("thickness");
      attrThickness.Value = row.RowSettings.Thickness.ToString();
      columnElement.Attributes.Append(attrThickness);

      XmlAttribute attrYAxis = document.CreateAttribute("yAxis");
      attrYAxis.Value = row.YAxis.Label;
      columnElement.Attributes.Append(attrYAxis);

      StringBuilder builder = new StringBuilder();

      for (int i = 0; i < row.Count; i++)
      {
        if (i == 0)
        {
          builder.Append(row[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
          //columnElement.InnerText += row[i].ToString(CultureInfo.InvariantCulture.NumberFormat);
        }
        else
        {
          builder.Append(";" + row[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
          //columnElement.InnerText += ";" + row[i].ToString(CultureInfo.InvariantCulture.NumberFormat);
        }
      }
      columnElement.InnerText += builder.ToString();
      return columnElement;
    }

    public override IDataRow FromXml(XmlNode xmlNode)
    {
      throw new System.NotImplementedException();
    }

    private void UpdateMinMaxValue(double newValue, int oldValueIndex) {
      if (minValue != dataRow[oldValueIndex] && maxValue != dataRow[oldValueIndex])
        UpdateMinMaxValue(newValue);
      else {
        minValue = double.MaxValue;
        maxValue = double.MinValue;

        for (int i = 0; i < dataRow.Count; i++) {
          double value = oldValueIndex != i ? dataRow[i] : newValue;
          UpdateMinMaxValue(value);
        }
      }
    }

    private void UpdateMinMaxValue(double value) {
      maxValue = Math.Max(value, maxValue);
      minValue = Math.Min(value, minValue);
    }
  }
}