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

      double[] vals = new double[]{};

      ChartDataRowsModelColumn col = new ChartDataRowsModelColumn(id, vals);

      columns.Add(col);
    }

    public void PushData(int dataRowId, double value){

      columns[dataRowId].Values[columns[dataRowId].Values.Length+1] = value;
    }

    public override IView CreateView() {
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
        XmlNode child = document.CreateNode(XmlNodeType.Element, "column", null);
        child.InnerText = "xx";
        node.AppendChild(child);
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