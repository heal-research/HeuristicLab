#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using HeuristicLab.Common;
using HEAL.Fossil;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Visual properties of a DataRow.
  /// </summary>
  [StorableType("3B28F24F-5CA7-40C2-A81C-65FCAF5B2C66")]
  public class DataRowVisualProperties : DeepCloneable, INotifyPropertyChanged {
    #region ChartType
    [StorableType("488A018F-AF79-4B60-989A-845EF24F4A01")]
    public enum DataRowChartType {
      Line,
      Columns,
      Points,
      Bars,
      Histogram,
      StepLine
    }

    #endregion
    #region LineStyle
    [StorableType("A064C2CE-D2CC-4292-B5FC-8DDCFA55C896")]
    public enum DataRowLineStyle {
      Dash,
      DashDot,
      DashDotDot,
      Dot,
      NotSet,
      Solid
    }
    #endregion

    private DataRowChartType chartType;
    public DataRowChartType ChartType {
      get { return chartType; }
      set {
        if (chartType != value) {
          chartType = value;
          OnPropertyChanged("ChartType");
        }
      }
    }
    private bool secondYAxis;
    public bool SecondYAxis {
      get { return secondYAxis; }
      set {
        if (secondYAxis != value) {
          secondYAxis = value;
          OnPropertyChanged("SecondYAxis");
        }
      }
    }
    private bool secondXAxis;
    public bool SecondXAxis {
      get { return secondXAxis; }
      set {
        if (secondXAxis != value) {
          secondXAxis = value;
          OnPropertyChanged("SecondXAxis");
        }
      }
    }
    private Color color;
    public Color Color {
      get { return color; }
      set {
        if (color != value) {
          color = value;
          OnPropertyChanged("Color");
        }
      }
    }
    private DataRowLineStyle lineStyle;
    public DataRowLineStyle LineStyle {
      get { return lineStyle; }
      set {
        if (lineStyle != value) {
          lineStyle = value;
          OnPropertyChanged("LineStyle");
        }
      }
    }
    private bool startIndexZero;
    public bool StartIndexZero {
      get { return startIndexZero; }
      set {
        if (startIndexZero != value) {
          startIndexZero = value;
          OnPropertyChanged("StartIndexZero");
        }
      }
    }
    private int lineWidth;
    public int LineWidth {
      get { return lineWidth; }
      set {
        if (lineWidth != value) {
          lineWidth = value;
          OnPropertyChanged("LineWidth");
        }
      }
    }

    private double scaleFactor;
    public double ScaleFactor {
      get { return scaleFactor; }
      set {
        if (scaleFactor != value) {
          scaleFactor = value;
          OnPropertyChanged("ScaleFactor");
        }
      }
    }
    private bool isVisibleInLegend;
    public bool IsVisibleInLegend {
      get { return isVisibleInLegend; }
      set {
        if (isVisibleInLegend != value) {
          isVisibleInLegend = value;
          OnPropertyChanged("IsVisibleInLegend");
        }
      }
    }
    private string displayName;
    public string DisplayName {
      get { return displayName == null ? String.Empty : displayName; }
      set {
        if (displayName != value) {
          if (value == null && displayName != String.Empty) {
            displayName = String.Empty;
            OnPropertyChanged("DisplayName");
          } else if (value != null) {
            displayName = value;
            OnPropertyChanged("DisplayName");
          }
        }
      }
    }

    #region Persistence Properties
    [Storable(Name = "ChartType")]
    private DataRowChartType StorableChartType {
      get { return chartType; }
      set { chartType = value; }
    }
    [Storable(Name = "SecondYAxis")]
    private bool StorableSecondYAxis {
      get { return secondYAxis; }
      set { secondYAxis = value; }
    }
    [Storable(Name = "SecondXAxis")]
    private bool StorableSecondXAxis {
      get { return secondXAxis; }
      set { secondXAxis = value; }
    }
    [Storable(Name = "Color")]
    private Color StorableColor {
      get { return color; }
      set { color = value; }
    }
    [Storable(Name = "LineStyle")]
    private DataRowLineStyle StorableLineStyle {
      get { return lineStyle; }
      set { lineStyle = value; }
    }
    [Storable(Name = "StartIndexZero")]
    private bool StorableStartIndexZero {
      get { return startIndexZero; }
      set { startIndexZero = value; }
    }
    [Storable(Name = "LineWidth")]
    private int StorableLineWidth {
      get { return lineWidth; }
      set { lineWidth = value; }
    }
    [Storable(Name = "ScaleFactor")]
    private double StorableScaleFactor {
      get { return scaleFactor; }
      set { scaleFactor = value; }
    }
    [Storable(Name = "IsVisibleInLegend", DefaultValue = true)]
    private bool StorableIsVisibleInLegend {
      get { return isVisibleInLegend; }
      set { isVisibleInLegend = value; }
    }
    [Storable(Name = "DisplayName")]
    private string StorableDisplayName {
      get { return displayName; }
      set { displayName = value; }
    }
    #endregion

    #region Histogram Properties - Backwards Compatability
    internal enum DataRowHistogramAggregation {
      Overlapping,
      SideBySide,
      Stacked
    }

    internal int? Bins { get; private set; }
    internal bool? ExactBins { get; private set; }
    internal DataRowHistogramAggregation? Aggregation { get; private set; }

    [Storable(Name = "Bins", AllowOneWay = true)]
    private int StorableBins { set { Bins = value; } }
    [Storable(Name = "ExactBins", AllowOneWay = true)]
    private bool StorableExactBins { set { ExactBins = value; } }
    [Storable(Name = "Aggregation", AllowOneWay = true)]
    private DataRowHistogramAggregation StorableAggregation { set { Aggregation = value; } }
    #endregion

    [StorableConstructor]
    protected DataRowVisualProperties(StorableConstructorFlag _) { }
    protected DataRowVisualProperties(DataRowVisualProperties original, Cloner cloner)
      : base(original, cloner) {
      this.chartType = original.chartType;
      this.secondYAxis = original.secondYAxis;
      this.secondXAxis = original.secondXAxis;
      this.color = original.color;
      this.lineStyle = original.lineStyle;
      this.startIndexZero = original.startIndexZero;
      this.lineWidth = original.lineWidth;
      this.scaleFactor = original.scaleFactor;
      this.displayName = original.displayName;
      this.isVisibleInLegend = original.isVisibleInLegend;
    }
    public DataRowVisualProperties() {
      chartType = DataRowChartType.Line;
      secondYAxis = false;
      secondXAxis = false;
      color = Color.Empty;
      lineStyle = DataRowLineStyle.Solid;
      startIndexZero = false;
      lineWidth = 1;
      scaleFactor = 1.0;
      displayName = String.Empty;
      isVisibleInLegend = true;
    }
    public DataRowVisualProperties(string displayName)
      : this() {
      this.displayName = displayName;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataRowVisualProperties(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (secondXAxis == default(bool)
        && lineStyle == default(DataRowLineStyle)
        && displayName == default(string)) {
        secondXAxis = false;
        lineStyle = DataRowLineStyle.Solid;
        lineWidth = 1;
        displayName = String.Empty;
      }
      #endregion
    }
  }
}
