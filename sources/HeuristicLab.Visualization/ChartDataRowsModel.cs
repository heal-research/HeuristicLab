using System;
using System.Collections.Generic;
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

//      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
//
//      foreach (ChartDataRowsModelColumn column in Columns){
//        XmlNode columnElement = document.CreateNode(XmlNodeType.Element, "column", null);
//
//        XmlAttribute idAttr = document.CreateAttribute("id");
//        idAttr.Value = (column.ColumnId).ToString();
//        columnElement.Attributes.Append(idAttr);
//
//        for (int i = 0; i < column.Values.Length; i++){
//          if (i == 0){
//            columnElement.InnerText += column.Values[i].ToString(CultureInfo.InvariantCulture.NumberFormat);
//          } else{
//            columnElement.InnerText += ";" + column.Values[i].ToString(CultureInfo.InvariantCulture.NumberFormat);
//          }
//        }
//        node.AppendChild(columnElement);
//      }
//      
//      return node;
    }


    
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      throw new NotImplementedException();

//      base.Populate(node, restoredObjects);
//
//      foreach (XmlNode column in node.ChildNodes){
//        XmlAttributeCollection attrs = column.Attributes;
//        XmlAttribute rowIdAttr = (XmlAttribute)attrs.GetNamedItem("id");
//        int rowId = int.Parse(rowIdAttr.Value);
//        AddDataRow(rowId);
//        string[] tokens = column.InnerText.Split(';');
//        double[] data = new double[tokens.Length];
//        for (int i = 0; i < data.Length; i++){
//          if (tokens[i].Length != 0){
//            if (
//              double.TryParse(tokens[i], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out data[i]) ==
//              false){
//              throw new FormatException("Can't parse " + tokens[i] + " as double value.");
//            }
//          }
//        }
//        Columns[rowId-1].Values = data;
//      }
    }
  }
}
