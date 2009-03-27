using System;
using System.Collections.Generic;
using System.Drawing;
using HeuristicLab.Visualization.LabelProvider;
using HeuristicLab.Visualization.Test;

namespace HeuristicLab.Visualization {
  public delegate void YAxisDescriptorChangedHandler(YAxisDescriptor sender);

  public class YAxisDescriptor {
    private ILabelProvider yAxisLabelProvider = new ContinuousLabelProvider("0.##");
    private readonly List<IDataRow> dataRows = new List<IDataRow>();
    private bool showYAxis = true;
    private string label = "";
    public bool ClipChangeable = true;
    private AxisPosition position = AxisPosition.Left;

    private bool showGrid = true;
    private Color gridColor = Color.LightBlue;

    public event YAxisDescriptorChangedHandler YAxisDescriptorChanged;

    private void OnYAxisDescriptorChanged() {
      if (YAxisDescriptorChanged != null) {
        YAxisDescriptorChanged(this);
      }
    }

    public List<IDataRow> DataRows {
      get { return dataRows; }
    }

    public bool ShowYAxis {
      get { return showYAxis; }
      set {
        showYAxis = value;
        OnYAxisDescriptorChanged();
      }
    }

    public ILabelProvider YAxisLabelProvider {
      get { return yAxisLabelProvider; }
      set {
        yAxisLabelProvider = value;
        OnYAxisDescriptorChanged();
      }
    }

    public double MinValue {
      get {
        double min = double.MaxValue;

        foreach (IDataRow row in dataRows) {
          min = Math.Min(min, row.MinValue);
        }

        return min;
      }
    }

    public double MaxValue {
      get {
        double max = double.MinValue;

        foreach (IDataRow row in dataRows) {
          max = Math.Max(max, row.MaxValue);
        }

        return max;
      }
    }

    public string Label {
      get { return label; }
      set {
        label = value;
        OnYAxisDescriptorChanged();
      }
    }

    public bool Zoom_ {
      get { return ClipChangeable; }
      set { ClipChangeable = value; }
    }

    public AxisPosition Position {
      get { return position; }
      set {
        position = value;
        OnYAxisDescriptorChanged();
      }
    }

    public bool ShowGrid {
      get { return showGrid; }
      set {
        showGrid = value;
        OnYAxisDescriptorChanged();
      }
    }

    public Color GridColor {
      get { return gridColor; }
      set {
        gridColor = value;
        OnYAxisDescriptorChanged();
      }
    }

    public void AddDataRow(IDataRow row) {
      if (row.YAxis != null) {
        row.YAxis.DataRows.Remove(row);
      }
      this.DataRows.Add(row);
      OnYAxisDescriptorChanged();
    }
  }
}