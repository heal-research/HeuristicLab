using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Visualization.Options;
using HeuristicLab.Visualization.Test;

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

      XmlSupport.SetAttribute("Title", title, node);
      
      List<YAxisDescriptor> yAxes = new List<YAxisDescriptor>();
      yAxes.Add(defaultYAxisDescriptor);
      foreach (IDataRow row in rows) {
        if (!yAxes.Contains(row.YAxis)) {
          yAxes.Add(row.YAxis);
        }
      }

      foreach (YAxisDescriptor yAxis in yAxes) {
        XmlNode yAxisElement = document.CreateElement("YAxis");

        XmlSupport.SetAttribute("Label", yAxis.Label, yAxisElement);
        XmlSupport.SetAttribute("GridColor", yAxis.GridColor.ToArgb().ToString(), yAxisElement);
        XmlSupport.SetAttribute("Position", yAxis.Position.ToString(), yAxisElement);
        XmlSupport.SetAttribute("ShowGrid", yAxis.ShowGrid ? "true" : "false", yAxisElement);
        XmlSupport.SetAttribute("ShowYAxis", yAxis.ShowYAxis ? "true" : "false", yAxisElement);
        XmlSupport.SetAttribute("ShowYAxisLabel", yAxis.ShowYAxisLabel ? "true" : "false", yAxisElement);
        XmlSupport.SetAttribute("ClipChangeable", yAxis.ClipChangeable ? "true" : "false", yAxisElement);

        if (yAxis == defaultYAxisDescriptor)
          XmlSupport.SetAttribute("Default", "true", yAxisElement);

        node.AppendChild(yAxisElement);
      }

      XmlNode xAxisElement = document.CreateElement("XAxis");
      XmlSupport.SetAttribute("Color", xAxisDescriptor.Color.ToArgb().ToString(), xAxisElement);
      XmlSupport.SetAttribute("GridColor", xAxisDescriptor.GridColor.ToArgb().ToString(), xAxisElement);
      XmlSupport.SetAttribute("Label", xAxisDescriptor.Label, xAxisElement);
      XmlSupport.SetAttribute("ShowGrid", xAxisDescriptor.ShowGrid ? "true" : "false", xAxisElement);
      XmlSupport.SetAttribute("ShowLabel", xAxisDescriptor.ShowLabel ? "true" : "false", xAxisElement);
      node.AppendChild(xAxisElement);

      foreach (IDataRow row in rows) {
        node.AppendChild(row.ToXml(document));
      }

      node.AppendChild(XAxis.LabelProvider.GetLabelProviderXmlNode(document));

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      title = XmlSupport.GetAttribute("Title", "", node);

      List<YAxisDescriptor> yAxes = new List<YAxisDescriptor>();
      foreach (XmlNode yAxisNode in node.SelectNodes("YAxis")) {
        YAxisDescriptor yAxis = new YAxisDescriptor();
        yAxis.Label = XmlSupport.GetAttribute("Label", "", yAxisNode);
        yAxis.GridColor = Color.FromArgb(int.Parse(XmlSupport.GetAttribute("GridColor", Color.LightBlue.ToArgb().ToString(), yAxisNode)));
        yAxis.Position = (AxisPosition)Enum.Parse(typeof(AxisPosition), XmlSupport.GetAttribute("Position", "Left", yAxisNode));
        yAxis.ShowGrid = XmlSupport.GetAttribute("ShowGrid", "true", yAxisNode) == "true";
        yAxis.ShowYAxis = XmlSupport.GetAttribute("ShowYAxis", "true", yAxisNode) == "true";
        yAxis.ShowYAxisLabel = XmlSupport.GetAttribute("ShowYAxisLabel", "true", yAxisNode) == "true";
        yAxis.ClipChangeable = XmlSupport.GetAttribute("ClipChangeable", "true", yAxisNode) == "true";
        yAxes.Add(yAxis);

        if (XmlSupport.GetAttribute("Default", null, yAxisNode) != null)
          defaultYAxisDescriptor = yAxis;
      }

      XmlNode xAxisElement = node.SelectSingleNode("XAxis");
      if (xAxisElement != null) {
        xAxisDescriptor.Color = Color.FromArgb(int.Parse(XmlSupport.GetAttribute("Color", Color.Blue.ToArgb().ToString(), xAxisElement)));
        xAxisDescriptor.GridColor = Color.FromArgb(int.Parse(XmlSupport.GetAttribute("GridColor", Color.LightBlue.ToArgb().ToString(), xAxisElement)));
        xAxisDescriptor.Label = XmlSupport.GetAttribute("Label", "", xAxisElement);
        xAxisDescriptor.ShowGrid = XmlSupport.GetAttribute("ShowGrid", "true", xAxisElement) == "true";
        xAxisDescriptor.ShowLabel = XmlSupport.GetAttribute("ShowLabel", "true", xAxisElement) == "true";
      }

      XmlNode xAxisLabelProviderNode = node.SelectSingleNode("LabelProvider");
      xAxisDescriptor.LabelProvider = xAxisDescriptor.LabelProvider.PopulateLabelProviderXmlNode(xAxisLabelProviderNode);

      foreach (XmlNode dataRow in node.SelectNodes("Row")) {
        string rowLabel = XmlSupport.GetAttribute("Label", "", dataRow);

        DataRow row = new DataRow();
        row.RowSettings.Label = rowLabel;
        row.RowSettings.Color = Color.FromArgb(Int32.Parse(XmlSupport.GetAttribute("Color", Color.Black.ToArgb().ToString(), dataRow)));
        row.RowSettings.LineType = (DataRowType)Enum.Parse(typeof(DataRowType), XmlSupport.GetAttribute("LineType", "Normal", dataRow));
        row.RowSettings.Thickness = Int32.Parse(XmlSupport.GetAttribute("Thickness", "2", dataRow));
        row.RowSettings.ShowMarkers = XmlSupport.GetAttribute("ShowMarkers", "true", dataRow) == "true";
        row.RowSettings.Style = (DrawingStyle)Enum.Parse(typeof (DrawingStyle), XmlSupport.GetAttribute("Style", DrawingStyle.Solid.ToString(), dataRow));

        foreach (YAxisDescriptor yAxis in yAxes) {
          if (yAxis.Label.Equals(XmlSupport.GetAttribute("YAxis", dataRow)))
            row.YAxis = yAxis;
        }

        string[] tokens = dataRow.InnerText.Split(';');
        foreach (string token in tokens) {
          double value = double.Parse(token, CultureInfo.InvariantCulture);
          row.AddValue(value);
        }

        AddDataRow(row);
      }
    }
  }
}
