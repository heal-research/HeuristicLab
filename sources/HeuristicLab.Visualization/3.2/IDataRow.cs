using System.Xml;
using HeuristicLab.Visualization.Options;

namespace HeuristicLab.Visualization {
  public enum DataRowType {
    Normal, SingleValue,
    Points
  }

  public interface IDataRow {
    DataRowSettings RowSettings { get; set; }
 
    YAxisDescriptor YAxis { get; set; }

    void AddValue(double value);
    void AddValue(double value, int index);
    void AddValues(double[] values);
    void AddValues(double[] values, int index);

    void ModifyValue(double value, int index);
    void ModifyValues(double[] values, int index);

    void RemoveValue(int index);
    void RemoveValues(int index, int count);

    int Count { get; }
    double this[int index] { get; set; }

    double MinValue { get; }
    double MaxValue { get; }

    XmlNode ToXml(XmlDocument document);
    IDataRow FromXml(XmlNode xmlNode);

    event ValuesChangedHandler ValuesChanged;
    event ValueChangedHandler ValueChanged;
    event DataRowChangedHandler DataRowChanged;
  }
}