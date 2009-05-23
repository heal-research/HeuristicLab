using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using HeuristicLab.Core;
using System.Text;
using HeuristicLab.Visualization.Options;

namespace HeuristicLab.Visualization{
  public delegate void DataRowAddedHandler(IDataRow row);
  public delegate void DataRowRemovedHandler(IDataRow row);
  public delegate void ModelChangedHandler();

  public class ChartDataRowsModel : ChartDataModelBase, IChartDataRowsModel{
    private string title = "Title";

    private ViewSettings viewSettings = new ViewSettings();

    private readonly XAxisDescriptor xAxisDescriptor = new XAxisDescriptor();

    public ChartDataRowsModel() {
      this.XAxis.XAxisDescriptorChanged += delegate { OnModelChanged(); };
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


    private readonly List<IDataRow> rows = new List<IDataRow>();

    public List<IDataRow> Rows{
      get { return rows; }
    }

    public string Title {
      get { return title; }
      set {
        title = value;
        OnModelChanged();
      }
    }

    public override IView CreateView() {
      return new LineChart(this);
    }

    private readonly YAxisDescriptor defaultYAxisDescriptor = new YAxisDescriptor();

    public void AddDataRow(IDataRow row) {
      if (row.YAxis == null) {
        row.YAxis = defaultYAxisDescriptor;
      }
      rows.Add(row);
      OnDataRowAdded(row);
    }

    public void RemoveDataRow(IDataRow row) {
      rows.Remove(row);
      OnDataRowRemoved(row);
    }

    public YAxisDescriptor DefaultYAxis {
      get { return defaultYAxisDescriptor; }
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

      foreach (IDataRow row in rows) {
        XmlNode columnElement = document.CreateNode(XmlNodeType.Element, "row", null);

        XmlAttribute idAttr = document.CreateAttribute("label");
        idAttr.Value = row.Label;
        columnElement.Attributes.Append(idAttr);

        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < row.Count; i++) {
          if (i == 0) {
            builder.Append(row[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
            //columnElement.InnerText += row[i].ToString(CultureInfo.InvariantCulture.NumberFormat);
          } else {
            builder.Append(";" + row[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
            //columnElement.InnerText += ";" + row[i].ToString(CultureInfo.InvariantCulture.NumberFormat);
          }
        }
        columnElement.InnerText += builder.ToString();
        node.AppendChild(columnElement);
      }

      XmlNode labelProviderNode = document.ImportNode(XAxis.XAxisLabelProvider.GetLabelProviderXmlNode(), true);
      node.AppendChild(labelProviderNode);

      return node;    
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      foreach (XmlNode dataRow in node.ChildNodes) {
        if (dataRow.Name.Equals("LabelProvider")) {
          XAxis.XAxisLabelProvider = XAxis.XAxisLabelProvider.PopulateLabelProviderXmlNode(dataRow);
        } else {
          XmlAttributeCollection attrs = dataRow.Attributes;
          XmlAttribute rowIdAttr = (XmlAttribute)attrs.GetNamedItem("label");
          string rowLabel = rowIdAttr.Value;
          DataRow row = new DataRow();
          row.Label = rowLabel;

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
