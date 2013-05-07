#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Visual properties of a ScatterPlotDataRow.
  /// </summary>
  [StorableClass]
  public class ScatterPlotDataRowVisualProperties : DeepCloneable, INotifyPropertyChanged {
    #region PointStyle
    public enum ScatterPlotDataRowPointStyle {
      Circle,
      Cross,
      Diamond,
      Square,
      Star4,
      Star5,
      Star6,
      Star10,
      Triangle
    }
    #endregion

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
    private ScatterPlotDataRowPointStyle pointStyle;
    public ScatterPlotDataRowPointStyle PointStyle {
      get { return pointStyle; }
      set {
        if (pointStyle != value) {
          pointStyle = value;
          OnPropertyChanged("PointStyle");
        }
      }
    }
    private int pointSize;
    public int PointSize {
      get { return pointSize; }
      set {
        if (pointSize != value) {
          pointSize = value;
          OnPropertyChanged("PointSize");
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
    [Storable(Name = "Color")]
    private Color StorableColor {
      get { return color; }
      set { color = value; }
    }
    [Storable(Name = "PointStyle")]
    private ScatterPlotDataRowPointStyle StorablePointStyle {
      get { return pointStyle; }
      set { pointStyle = value; }
    }
    [Storable(Name = "PointSize")]
    private int StorablePointSize {
      get { return pointSize; }
      set { pointSize = value; }
    }
    [Storable(Name = "IsVisibleInLegend")]
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

    [StorableConstructor]
    protected ScatterPlotDataRowVisualProperties(bool deserializing) : base() { }
    protected ScatterPlotDataRowVisualProperties(ScatterPlotDataRowVisualProperties original, Cloner cloner)
      : base(original, cloner) {
      this.color = original.color;
      this.pointStyle = original.pointStyle;
      this.pointSize = original.pointSize;
      this.displayName = original.displayName;
      this.isVisibleInLegend = original.isVisibleInLegend;
    }
    public ScatterPlotDataRowVisualProperties() {
      color = Color.Empty;
      pointStyle = ScatterPlotDataRowPointStyle.Circle;
      pointSize = 3;
      displayName = String.Empty;
      isVisibleInLegend = true;
    }
    public ScatterPlotDataRowVisualProperties(string displayName)
      : this() {
      this.displayName = displayName;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlotDataRowVisualProperties(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
