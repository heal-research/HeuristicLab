using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization{
  public delegate void DataRowAddedHandler(IDataRow row);
  public delegate void DataRowRemovedHandler(IDataRow row);
  public delegate void ModelChangedHandler();

  public class ChartDataRowsModel : ChartDataModelBase, IChartDataRowsModel{
    private string title;
    private string xAxisLabel;

    private readonly List<IDataRow> rows = new List<IDataRow>();
    private readonly List<string> xLabels = new List<string>();

    public List<string> XLabels{
      get { return xLabels; }
    }

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
    public string XAxisLabel {
      get { return xAxisLabel; }
      set {
        xAxisLabel = value;
        OnModelChanged();
      }
    }

    public override IView CreateView() {
      return new LineChart(this);
    }

    public void AddLabel(string label) {
      xLabels.Add(label);
      OnModelChanged();
    }

    public void AddLabel(string label, int index) {
      xLabels[index] = label;
      OnModelChanged();
    }

    public void AddLabels(string[] labels) {
      foreach (var s in labels){
        AddLabel(s);
      }
      //OnModelChanged();
    }

    public void AddLabels(string[] labels, int index) {
      int i = 0;
      foreach (var s in labels){
        AddLabel(s, index + i);
        i++;
      }
      //OnModelChanged();
    }

    public void ModifyLabel(string label, int index) {
      xLabels[index] = label;
      OnModelChanged();
    }

    public void ModifyLabels(string[] labels, int index) {
      int i = 0;
      foreach (var s in labels){
        ModifyLabel(s, index + i);
        i++;
      }
      //OnModelChanged();
    }

    public void RemoveLabel(int index) {
      xLabels.RemoveAt(index);
      OnModelChanged();
    }

    public void RemoveLabels(int index, int count) {
      for (int i = index; i < index + count; i++ ){
        RemoveLabel(i);
      }
      //OnModelChanged();
    }

    public void AddDataRow(IDataRow row) {
      rows.Add(row);
      OnDataRowAdded(row);
    }

    public void RemoveDataRow(IDataRow row) {
      rows.Remove(row);
      OnDataRowRemoved(row);
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
      throw new NotImplementedException();
      
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      throw new NotImplementedException();

    }
  }
}
