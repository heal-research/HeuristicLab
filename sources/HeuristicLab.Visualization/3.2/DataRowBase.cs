using HeuristicLab.Core;
using HeuristicLab.Visualization.Options;

namespace HeuristicLab.Visualization {
  public abstract class DataRowBase : StorableBase, IDataRow {
    protected YAxisDescriptor yAxis;

    private DataRowSettings rowSettings ;

    public DataRowSettings RowSettings {
      get { return rowSettings; }
      set {
        rowSettings.DataVisualSettingChanged -= value_DataVisualSettingChanged;
        value.DataVisualSettingChanged += value_DataVisualSettingChanged;
        rowSettings = value;
      }
    }

    protected DataRowBase() {
      rowSettings = new DataRowSettings();
      rowSettings.DataVisualSettingChanged += value_DataVisualSettingChanged;
    }

    void value_DataVisualSettingChanged(DataRowSettings row) {
      OnDataRowChanged(this);
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