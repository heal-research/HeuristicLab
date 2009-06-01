using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Xml;
using HeuristicLab.Core;
using System.Text;
using HeuristicLab.Visualization.Options;

namespace HeuristicLab.Visualization{
  public delegate void DataRowAddedHandler(IDataRow row);
  public delegate void DataRowRemovedHandler(IDataRow row);
  public delegate void ModelChangedHandler();

  public class ChartDataRowsModel : ItemBase, IChartDataRowsModel {
    private string title = "Title";
    private ViewSettings viewSettings = new ViewSettings();
    private readonly XAxisDescriptor xAxisDescriptor = new XAxisDescriptor();
    private YAxisDescriptor defaultYAxisDescriptor = new YAxisDescriptor();
    private readonly List<IDataRow> rows = new List<IDataRow>();

    public ChartDataRowsModel() {
      this.XAxis.XAxisDescriptorChanged += delegate { OnModelChanged(); };
    }

    public override IView CreateView() {
      return new LineChart(this);
    }

    public string Title {
      get { return title; }
      set {
        title = value;
        OnModelChanged();
      }
    }

    public XAxisDescriptor XAxis {
      get { return xAxisDescriptor; }
    }

    public List<YAxisDescriptor> YAxes {
      get {
        Dictionary<YAxisDescriptor, object> yaxes = new Dictionary<YAxisDescriptor, object>();

        foreach (IDataRow row in rows) {
          yaxes[row.YAxis] = null;
        }

        return new List<YAxisDescriptor>(yaxes.Keys);
      }
    }

    public YAxisDescriptor DefaultYAxis {
      get { return defaultYAxisDescriptor; }
    }

    public List<IDataRow> Rows{
      get { return rows; }
    }

    public void AddDataRow(IDataRow row) {
      if (row.YAxis == null) {
        row.YAxis = defaultYAxisDescriptor;
      }
      rows.Add(row);
      OnDataRowAdded(row);
    }

    public void AddDataRows(params IDataRow[] rows) {
      foreach (IDataRow row in rows) {
        AddDataRow(row);
      }
    }

    public void RemoveDataRow(IDataRow row) {
      rows.Remove(row);
      OnDataRowRemoved(row);
    }

    // TODO implement calculation of max data row values
    public int MaxDataRowValues {
      get {
        int max = 0;

        foreach (IDataRow row in rows) {
          max = Math.Max(max, row.Count);
        }

        return max;
      }
    }

    public ViewSettings ViewSettings {
      get { return viewSettings; }
      set { viewSettings = value; }
    }

    public event ModelChangedHandler ModelChanged;

    protected void OnModelChanged() {
      if (ModelChanged != null) {
        ModelChanged();
      }
    }

    public event DataRowAddedHandler DataRowAdded;

    protected void OnDataRowAdded(IDataRow row) {
      if (DataRowAdded != null) {
        DataRowAdded(row);
      }
    }

    public event DataRowRemovedHandler DataRowRemoved;

    protected void OnDataRowRemoved(IDataRow row) {
      if (DataRowRemoved != null) {
        DataRowRemoved(row);
      }
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      
      List<YAxisDescriptor> yAxis = new List<YAxisDescriptor>();
      yAxis.Add(defaultYAxisDescriptor);
      foreach (IDataRow row in rows) {
        XmlNode rowElement = row.ToXml(row, document);
        
        if (!yAxis.Contains(row.YAxis)) {
          yAxis.Add(row.YAxis);
        }
        
        node.AppendChild(rowElement);
      }

      foreach (YAxisDescriptor axis in yAxis) {
        XmlNode yAxisElement = document.CreateNode(XmlNodeType.Element, "yAxis", null);

        XmlAttribute attrLabel = document.CreateAttribute("label");
        attrLabel.Value = axis.Label;
        yAxisElement.Attributes.Append(attrLabel);

        if (axis == defaultYAxisDescriptor) {
          XmlAttribute attrDefault = document.CreateAttribute("default");
          attrDefault.Value = "true";
          yAxisElement.Attributes.Append(attrDefault);
        }
        node.AppendChild(yAxisElement);
        
      }

      XmlNode labelProviderNode = document.ImportNode(XAxis.LabelProvider.GetLabelProviderXmlNode(), true);
      node.AppendChild(labelProviderNode);

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      List<YAxisDescriptor> yAxis = new List<YAxisDescriptor>();
      foreach (XmlNode dataRow in node.ChildNodes)
      {
        if (dataRow.Name.Equals("yAxis"))
        {
          XmlAttributeCollection attrs = dataRow.Attributes;

          XmlAttribute attrYAxisLabel = (XmlAttribute)attrs.GetNamedItem("label");
          string axisLabel = attrYAxisLabel.Value;
          YAxisDescriptor axis = new YAxisDescriptor();
          axis.Label = axisLabel;
          yAxis.Add(axis);

          XmlAttribute attrDefault = (XmlAttribute)attrs.GetNamedItem("default");
          if (attrDefault != null) {
            defaultYAxisDescriptor = axis;
          }
        }
      }

      foreach (XmlNode dataRow in node.ChildNodes) {
        if (dataRow.Name.Equals("LabelProvider")) {
          XAxis.LabelProvider = XAxis.LabelProvider.PopulateLabelProviderXmlNode(dataRow);
        } else if (dataRow.Name.Equals("row")) {
          XmlAttributeCollection attrs = dataRow.Attributes;
          XmlAttribute rowIdAttr = (XmlAttribute)attrs.GetNamedItem("label");
          string rowLabel = rowIdAttr.Value;
          string rowColor = attrs.GetNamedItem("color").Value;

          DataRow row = new DataRow();
          row.RowSettings.Label = rowLabel;
          row.RowSettings.Color = Color.FromName(rowColor);

          string yAxisLabel = attrs.GetNamedItem("yAxis").Value;
          foreach (YAxisDescriptor axis in yAxis) {
            if (axis.Label.Equals(yAxisLabel)) {
              row.YAxis = axis;
            }
          }

          string[] tokens = dataRow.InnerText.Split(';');
          double[] data = new double[tokens.Length];
          for (int i = 0; i < data.Length; i++) {
            if (tokens[i].Length != 0) {
              if (
                double.TryParse(tokens[i], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out data[i]) ==
                false) {
                throw new FormatException("Can't parse " + tokens[i] + " as double value.");
              }
            }
          }
          row.AddValues(data);
          AddDataRow(row);
        }
      }
    }
  }
}
