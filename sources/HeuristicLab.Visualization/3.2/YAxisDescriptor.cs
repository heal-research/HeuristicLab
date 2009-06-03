using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Visualization.LabelProvider;
using HeuristicLab.Visualization.Test;

namespace HeuristicLab.Visualization {
  public delegate void YAxisDescriptorChangedHandler(YAxisDescriptor sender);

  public class YAxisDescriptor : StorableBase {
    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.##");
    private readonly List<IDataRow> dataRows = new List<IDataRow>();
    private bool showYAxis = true;
    private bool showYAxisLabel = true;
    private string label = "";
    private bool clipChangeable = true;
    private AxisPosition position = AxisPosition.Left;

    private bool showGrid = true;
    private Color gridColor = Color.LightBlue;

    public event YAxisDescriptorChangedHandler YAxisDescriptorChanged;

    private void OnYAxisDescriptorChanged() {
      if (YAxisDescriptorChanged != null) {
        YAxisDescriptorChanged(this);
      }
    }

    public List<IDataRow> DataRows {
      get { return dataRows; }
    }

    public bool ShowYAxis {
      get { return showYAxis; }
      set {
        showYAxis = value;
        OnYAxisDescriptorChanged();
      }
    }

    public bool ShowYAxisLabel {
      get { return showYAxisLabel; }
      set {
        showYAxisLabel = value;
        OnYAxisDescriptorChanged();
      }
    }

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set {
        labelProvider = value;
        OnYAxisDescriptorChanged();
      }
    }

    public double MinValue {
      get {
        double min = double.MaxValue;

        foreach (IDataRow row in dataRows) {
          min = Math.Min(min, row.MinValue);
        }

        return min;
      }
    }

    public double MaxValue {
      get {
        double max = double.MinValue;

        foreach (IDataRow row in dataRows) {
          max = Math.Max(max, row.MaxValue);
        }

        return max;
      }
    }

    public string Label {
      get { return label; }
      set {
        label = value;
        OnYAxisDescriptorChanged();
      }
    }

    public bool ClipChangeable {
      get { return clipChangeable; }
      set { clipChangeable = value; }
    }

    public AxisPosition Position {
      get { return position; }
      set {
        position = value;
        OnYAxisDescriptorChanged();
      }
    }

    public bool ShowGrid {
      get { return showGrid; }
      set {
        showGrid = value;
        OnYAxisDescriptorChanged();
      }
    }

    public Color GridColor {
      get { return gridColor; }
      set {
        gridColor = value;
        OnYAxisDescriptorChanged();
      }
    }

    public void AddDataRow(IDataRow row) {
      if (row.YAxis != null) {
        row.YAxis.DataRows.Remove(row);
      }
      this.DataRows.Add(row);
      OnYAxisDescriptorChanged();
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      XmlSupport.SetAttribute("Label", this.Label, node);
      XmlSupport.SetAttribute("GridColor", this.GridColor.ToArgb().ToString(), node);
      XmlSupport.SetAttribute("Position", this.Position.ToString(), node);
      XmlSupport.SetAttribute("ShowGrid", this.ShowGrid ? "true" : "false", node);
      XmlSupport.SetAttribute("ShowYAxis", this.ShowYAxis ? "true" : "false", node);
      XmlSupport.SetAttribute("ShowYAxisLabel", this.ShowYAxisLabel ? "true" : "false", node);
      XmlSupport.SetAttribute("ClipChangeable", this.ClipChangeable ? "true" : "false", node);

      node.AppendChild(PersistenceManager.Persist("LabelProvider", this.LabelProvider, document, persistedObjects));

      foreach (IDataRow row in dataRows) {
        node.AppendChild(PersistenceManager.Persist("DataRow", row, document, persistedObjects));
      }

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      this.Label = XmlSupport.GetAttribute("Label", "", node);
      this.GridColor = Color.FromArgb(int.Parse(XmlSupport.GetAttribute("GridColor", Color.LightBlue.ToArgb().ToString(), node)));
      this.Position = (AxisPosition)Enum.Parse(typeof(AxisPosition), XmlSupport.GetAttribute("Position", "Left", node));
      this.ShowGrid = XmlSupport.GetAttribute("ShowGrid", "true", node) == "true";
      this.ShowYAxis = XmlSupport.GetAttribute("ShowYAxis", "true", node) == "true";
      this.ShowYAxisLabel = XmlSupport.GetAttribute("ShowYAxisLabel", "true", node) == "true";
      this.ClipChangeable = XmlSupport.GetAttribute("ClipChangeable", "true", node) == "true";

      XmlNode labelProviderNode = node.SelectSingleNode("LabelProvider");
      if (labelProviderNode != null)
        this.labelProvider = (ILabelProvider)PersistenceManager.Restore(labelProviderNode, restoredObjects);

      foreach (XmlNode dataRowNode in node.SelectNodes("DataRow")) {
        IDataRow row = (IDataRow)PersistenceManager.Restore(dataRowNode, restoredObjects);
        AddDataRow(row);
      }
    }
  }
}