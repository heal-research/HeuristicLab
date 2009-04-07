using System.Drawing;

namespace HeuristicLab.Visualization {
  public abstract class DataRowBase : IDataRow {
    private string label = "";
    private Color color = Color.Black;
    private int thickness = 2;
    private DrawingStyle style = DrawingStyle.Solid;
    private DataRowType lineType = DataRowType.Normal;
    private YAxisDescriptor yAxis;

    public string Label {
      get { return label; }
      set {
        label = value;
        OnDataRowChanged(this);
      }
    }

    public Color Color {
      get { return color; }
      set {
        color = value;
        OnDataRowChanged(this);
      }
    }

    public int Thickness {
      get { return thickness; }
      set {
        thickness = value;
        OnDataRowChanged(this);
      }
    }

    public DrawingStyle Style {
      get { return style; }
      set {
        style = value;
        OnDataRowChanged(this);
      }
    }

    public DataRowType LineType {
      get { return lineType; }
      set {
        lineType = value;
        OnDataRowChanged(this);
      }
    }

    public YAxisDescriptor YAxis {
      get { return yAxis; }
      set {
        yAxis = value;
        yAxis.AddDataRow(this);
        yAxis.YAxisDescriptorChanged += delegate { OnDataRowChanged(this); };
        OnDataRowChanged(this);
      }
    }

    protected void OnDataRowChanged(IDataRow row) {
      if (DataRowChanged != null) {
        DataRowChanged(this);
      }
    }

    protected void OnValuesChanged(double[] values, int index, Action action) {
      if (ValuesChanged != null) {
        ValuesChanged(this, values, index, action);
      }
    }

    protected void OnValueChanged(double value, int index, Action action) {
      if (ValueChanged != null) {
        ValueChanged(this, value, index, action);
      }
    }

    public abstract void AddValue(double value);

    public abstract void AddValue(double value, int index);

    public abstract void AddValues(double[] values);

    public abstract void AddValues(double[] values, int index);

    public abstract void ModifyValue(double value, int index);

    public abstract void ModifyValues(double[] values, int index);

    public abstract void RemoveValue(int index);

    public abstract void RemoveValues(int index, int count);

    public abstract int Count { get; }

    public abstract double this[int index] { get; set; }

    public abstract double MinValue { get; }

    public abstract double MaxValue { get; }

    public event ValuesChangedHandler ValuesChanged;

    public event ValueChangedHandler ValueChanged;

    public event DataRowChangedHandler DataRowChanged;
  }
}