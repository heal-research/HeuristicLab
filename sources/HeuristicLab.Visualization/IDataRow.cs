using System.Drawing;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {

  public enum DataRowType {
    Normal, SingleValue
  }

  public interface IDataRow {
    string Label { get; set; }
    Color Color { get; set; }
    int Thickness { get; set; }
    DrawingStyle Style { get; set; }
    DataRowType LineType { get; set; }
    ILabelProvider YAxisLabelProvider { get; set; }

    /// <summary>
    /// Raised when data row data changed. Should cause redraw in the view.
    /// </summary>

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

    event ValuesChangedHandler ValuesChanged;
    event ValueChangedHandler ValueChanged;
    event DataRowChangedHandler DataRowChanged;
  }
}