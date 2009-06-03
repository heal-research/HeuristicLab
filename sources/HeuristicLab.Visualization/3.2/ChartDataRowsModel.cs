using System;
using System.Collections.Generic;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Visualization.Options;

namespace HeuristicLab.Visualization{
  public delegate void DataRowAddedHandler(IDataRow row);
  public delegate void DataRowRemovedHandler(IDataRow row);
  public delegate void ModelChangedHandler();

  public class ChartDataRowsModel : ItemBase, IChartDataRowsModel {
    private string title = "Title";
    private ViewSettings viewSettings = new ViewSettings();
    private XAxisDescriptor xAxisDescriptor;
    private YAxisDescriptor defaultYAxisDescriptor = new YAxisDescriptor();
    private readonly List<IDataRow> rows = new List<IDataRow>();

    public ChartDataRowsModel() {
      this.XAxis = new XAxisDescriptor();
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
      private set {
        this.xAxisDescriptor = value;
        this.xAxisDescriptor.XAxisDescriptorChanged += delegate { OnModelChanged(); };
      }
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

      foreach (IDataRow row in rows) {
        node.AppendChild(PersistenceManager.Persist("DataRow", row, document, persistedObjects));
      }
      node.AppendChild(PersistenceManager.Persist("DefaultYAxis", this.DefaultYAxis, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("XAxis", this.XAxis, document, persistedObjects));

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      title = XmlSupport.GetAttribute("Title", "", node);

      foreach (XmlNode dataRowNode in node.SelectNodes("DataRow")) {
        IDataRow dataRow = (IDataRow)PersistenceManager.Restore(dataRowNode, restoredObjects);
        AddDataRow(dataRow);
      }

      XmlNode defaultYAxisNode = node.SelectSingleNode("DefaultYAxis");
      if (defaultYAxisNode != null)
        this.defaultYAxisDescriptor = (YAxisDescriptor)PersistenceManager.Restore(defaultYAxisNode, restoredObjects);

      XmlNode xAxisNode = node.SelectSingleNode("XAxis");
      if (xAxisNode != null)
        this.XAxis = (XAxisDescriptor)PersistenceManager.Restore(xAxisNode, restoredObjects);
    }
  }
}
