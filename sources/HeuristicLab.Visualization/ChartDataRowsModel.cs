using System;
using System.Collections.Generic;
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

      double[] vals = new double[10]{4.0, 2.2, 0.2, 4, 18.2333, 1.1, 7, 99.99, 8, 2.6};

      ChartDataRowsModelColumn col = new ChartDataRowsModelColumn(id, vals);

      columns.Add(col);
    }

    public void PushData(int dataRowId, double value){

      //double[] vals = new double[columns[dataRowId].Values.Length+1];
      //for (int i = 0; i < columns[dataRowId].Values.Length; i++ ){
      //  vals[i] = columns[dataRowId].Values[i];

      //}
        
      //vals[vals.Length] = value;
      //columns[dataRowId].Values = vals;
    }

    public override IView CreateView() {
      //return new LineChart(this); when LineChart is implemented
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
          columnElement.InnerText += column.Values[i].ToString() + ";";
        }
          //columnElement.InnerText = "xx";
          node.AppendChild(columnElement);
      }
      
      //node.InnerText = "test1";
      //test.Data = 11;
      return node;

    }


    /*
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      //Data = bool.Parse(node.InnerText);
    }
   */
  }
}