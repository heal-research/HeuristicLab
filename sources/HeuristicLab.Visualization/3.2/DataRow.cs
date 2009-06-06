using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Xml;
using HeuristicLab.Core;

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

    public DataRow() {}
    
    public DataRow(string label) {
      this.RowSettings.Label = label;
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

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      XmlSupport.SetAttribute("Label", RowSettings.Label, node);
      XmlSupport.SetAttribute("Color", RowSettings.Color.ToArgb().ToString(), node);
      XmlSupport.SetAttribute("LineType", RowSettings.LineType.ToString(), node);
      XmlSupport.SetAttribute("Thickness", RowSettings.Thickness.ToString(), node);
      XmlSupport.SetAttribute("ShowMarkers", RowSettings.ShowMarkers ? "true" : "false", node);
      XmlSupport.SetAttribute("Style", RowSettings.Style.ToString(), node);

      node.AppendChild(PersistenceManager.Persist("YAxis", this.YAxis, document, persistedObjects));

      List<string> strValues = new List<string>();
      for (int i = 0; i < this.Count; i++) {
        strValues.Add(this[i].ToString(CultureInfo.InvariantCulture));
      }
      XmlElement valuesElement = document.CreateElement("Values");
      valuesElement.InnerText = string.Join(";", strValues.ToArray());
      node.AppendChild(valuesElement);

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      this.RowSettings.Label = XmlSupport.GetAttribute("Label", "", node);
      this.RowSettings.Color = Color.FromArgb(Int32.Parse(XmlSupport.GetAttribute("Color", Color.Black.ToArgb().ToString(), node)));
      this.RowSettings.LineType = (DataRowType)Enum.Parse(typeof(DataRowType), XmlSupport.GetAttribute("LineType", "Normal", node));
      this.RowSettings.Thickness = Int32.Parse(XmlSupport.GetAttribute("Thickness", "2", node));
      this.RowSettings.ShowMarkers = XmlSupport.GetAttribute("ShowMarkers", "true", node) == "true";
      this.RowSettings.Style = (DrawingStyle)Enum.Parse(typeof(DrawingStyle), XmlSupport.GetAttribute("Style", DrawingStyle.Solid.ToString(), node));

      XmlNode yAxisNode = node.SelectSingleNode("YAxis");
      if (yAxisNode != null) {
        this.YAxis = (YAxisDescriptor)PersistenceManager.Restore(yAxisNode, restoredObjects);
      }

      XmlNode valuesNode = node.SelectSingleNode("Values");
      if (valuesNode != null) {
        string[] strValues = valuesNode.InnerText.Split(';');
        foreach (string strValue in strValues) {
          double value = double.Parse(strValue, CultureInfo.InvariantCulture);
          this.AddValue(value);
        }
      }
    }

    public override string ToString() {
      return RowSettings.Label;
    }
  }
}