using System;
using System.Collections.Generic;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization{
  public delegate void DataRowAddedHandler(IDataRow row);
  public delegate void DataRowRemovedHandler(IDataRow row);

  public delegate void ModelChangedHandler();

  public class ChartDataRowsModel : ChartDataModelBase, IChartDataRowsModel {
    private string title;
    private string xAxisLabel;
    private string yAxisLabel;

    public void AddLabel(string label) {
      throw new NotImplementedException();
      // TODO ModelChangedEvent auslösen
    }

    public void AddLabel(string label, int index) {
      throw new NotImplementedException();
      // TODO ModelChangedEvent auslösen
    }

    public void AddLabels(string[] labels) {
      throw new NotImplementedException();
      // TODO ModelChangedEvent auslösen
    }

    public void AddLabels(string[] labels, int index) {
      throw new NotImplementedException();
      // TODO ModelChangedEvent auslösen
    }

    public void ModifyLabel(string label, int index) {
      throw new NotImplementedException();
      // TODO ModelChangedEvent auslösen
    }

    public void ModifyLabels(string[] labels, int index) {
      throw new NotImplementedException();
      // TODO ModelChangedEvent auslösen
    }

    public void RemoveLabel(int index) {
      throw new NotImplementedException();
      // TODO ModelChangedEvent auslösen
    }

    public void RemoveLabels(int index, int count) {
      throw new NotImplementedException();
      // TODO ModelChangedEvent auslösen
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

    public string YAxisLabel {
      get { return yAxisLabel; }
      set {
        yAxisLabel = value;
        OnModelChanged();
      }
    }

    public event ModelChangedHandler ModelChanged;

    protected void OnModelChanged() {
      if (ModelChanged != null) {
        ModelChanged();
      }
    }

    private readonly List<IDataRow> rows = new List<IDataRow>();
    
    public static IDataRow CreateDataRow() {
      throw new NotImplementedException();
    }

    public void AddDataRow(IDataRow row) {
      rows.Add(row);
      OnDataRowAdded(row);
    }

    public void RemoveDataRow(IDataRow row) {
      rows.Remove(row);
      OnDataRowRemoved(row);
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

    public override IView CreateView() {
      return new LineChart(this); //when LineChart is implemented
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
