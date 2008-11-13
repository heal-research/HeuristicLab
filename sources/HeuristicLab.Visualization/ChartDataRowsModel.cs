using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
//using System.Collections.Specialized;
using System.Xml;
using System.Xml.Serialization;
using HeuristicLab.Core;
using HeuristicLab.Data;


namespace HeuristicLab.Visualization{
  public class ChartDataRowsModel : ChartDataModelBase, IChartDataRowsModel {

    private IntData test = new IntData(1);
    
    public ChartDataRowsModel(){

      columns = new List<ChartDataRowsModelColumn>();


    }

    public void AddDataRow(int id){

      double[] vals = new double[0];

      ChartDataRowsModelColumn col = new ChartDataRowsModelColumn(id, vals);

      columns.Add(col);
    }

    public void PushData(int dataRowId, double value){

      double[] vals = new double[columns[dataRowId].Values.Length + 1];
      for (int i = 0; i < columns[dataRowId].Values.Length; i++) {
        vals[i] = columns[dataRowId].Values[i];

      }

      vals[vals.Length-1] = value;
      columns[dataRowId].Values = vals;

      RaiseColumnChanged(ChangeType.Add, vals.Length-1, new double[1]{value});
    }

    public override IView CreateView() {
      return new LineChart(this); //when LineChart is implemented
      return new IntDataView(test);
    }

    public event ChartDataRowsModelColumnChangedHandler ColumnChanged;

    private void RaiseColumnChanged(ChangeType type, long columnId, double[] values) {
      if (ColumnChanged != null) {
        ColumnChanged(type, columnId, values);
      }
    }

    private List<ChartDataRowsModelColumn> columns;

    public List<ChartDataRowsModelColumn> Columns {
      get { return columns; }
    }


    
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {

      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      foreach (ChartDataRowsModelColumn column in Columns){
        XmlNode columnElement = document.CreateNode(XmlNodeType.Element, "column", null);

        XmlAttribute idAttr = document.CreateAttribute("id");
        idAttr.Value = (column.ColumnId).ToString();
        columnElement.Attributes.Append(idAttr);

        for (int i = 0; i < column.Values.Length; i++){
          if (i == 0){
            columnElement.InnerText += column.Values[i].ToString(CultureInfo.InvariantCulture.NumberFormat);
          } else{
            columnElement.InnerText += ";" + column.Values[i].ToString(CultureInfo.InvariantCulture.NumberFormat);
          }
        }
        node.AppendChild(columnElement);
      }
      
      return node;

    }


    
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);


      foreach (XmlNode column in node.ChildNodes){
        XmlAttributeCollection attrs = column.Attributes;
        XmlAttribute rowIdAttr = (XmlAttribute)attrs.GetNamedItem("id");
        int rowId = int.Parse(rowIdAttr.Value);
        AddDataRow(rowId);
        string[] tokens = column.InnerText.Split(';');
        double[] data = new double[tokens.Length];
        for (int i = 0; i < data.Length; i++){
          if (tokens[i].Length != 0){
            if (
              double.TryParse(tokens[i], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out data[i]) ==
              false){
              throw new FormatException("Can't parse " + tokens[i] + " as double value.");
            }
          }
        }
        Columns[rowId-1].Values = data;
      }
    }
  }
}