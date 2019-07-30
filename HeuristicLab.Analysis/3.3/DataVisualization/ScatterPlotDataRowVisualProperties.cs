#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Visual properties of a ScatterPlotDataRow.
  /// </summary>
  [StorableType("3336A12E-A464-438E-9A37-B87790AE963A")]
  public class ScatterPlotDataRowVisualProperties : DeepCloneable, INotifyPropertyChanged {
    #region PointStyle
    [StorableType("45ED097C-3523-46B7-8D04-DA193833A899")]
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
    #region
    [StorableType("4EFF1DC9-C74C-474C-81E4-0AF8E336438E")]
    public enum ScatterPlotDataRowRegressionType {
      None,
      Linear,
      Polynomial,
      Exponential,
      Logarithmic,
      Power
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
    private ScatterPlotDataRowRegressionType regressionType;
    public ScatterPlotDataRowRegressionType RegressionType {
      get { return regressionType; }
      set {
        if (regressionType != value) {
          regressionType = value;
          OnPropertyChanged("RegressionType");
        }
      }
    }
    private int polynomialRegressionOrder;
    public int PolynomialRegressionOrder {
      get { return polynomialRegressionOrder; }
      set {
        if (polynomialRegressionOrder != value) {
          polynomialRegressionOrder = value;
          OnPropertyChanged("PolynomialRegressionOrder");
        }
      }
    }
    private bool isRegressionVisibleInLegend;
    public bool IsRegressionVisibleInLegend {
      get { return isRegressionVisibleInLegend; }
      set {
        if (isRegressionVisibleInLegend != value) {
          isRegressionVisibleInLegend = value;
          OnPropertyChanged("IsRegressionVisibleInLegend");
        }
      }
    }
    private string regressionDisplayName;
    public string RegressionDisplayName {
      get { return regressionDisplayName ?? string.Empty; }
      set {
        if (regressionDisplayName != value) {
          if (value == null && regressionDisplayName != string.Empty) {
            regressionDisplayName = string.Empty;
            OnPropertyChanged("RegressionDisplayName");
          } else if (value != null) {
            regressionDisplayName = value;
            OnPropertyChanged("RegressionDisplayName");
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
    [Storable(Name = "RegressionType")]
    private ScatterPlotDataRowRegressionType StorableRegressionType {
      get { return regressionType; }
      set { regressionType = value; }
    }
    [Storable(Name = "PolynomialRegressionOrder", DefaultValue = 2)]
    private int StorablePolynomialRegressionOrder {
      get { return polynomialRegressionOrder; }
      set { polynomialRegressionOrder = value; }
    }
    [Storable(Name = "IsRegressionVisibleInLegend", DefaultValue = true)]
    private bool StorableIsRegressionVisibleInLegend {
      get { return isRegressionVisibleInLegend; }
      set { isRegressionVisibleInLegend = value; }
    }
    [Storable(Name = "RegressionDisplayName")]
    private string StorableRegressionDisplayName {
      get { return regressionDisplayName; }
      set { regressionDisplayName = value; }
    }
    #endregion

    [StorableConstructor]
    protected ScatterPlotDataRowVisualProperties(StorableConstructorFlag _) { }
    protected ScatterPlotDataRowVisualProperties(ScatterPlotDataRowVisualProperties original, Cloner cloner)
      : base(original, cloner) {
      this.color = original.color;
      this.pointStyle = original.pointStyle;
      this.pointSize = original.pointSize;
      this.displayName = original.displayName;
      this.isVisibleInLegend = original.isVisibleInLegend;
      this.regressionType = original.regressionType;
      this.polynomialRegressionOrder = original.polynomialRegressionOrder;
      this.isRegressionVisibleInLegend = original.isRegressionVisibleInLegend;
      this.regressionDisplayName = original.regressionDisplayName;
    }
    public ScatterPlotDataRowVisualProperties() {
      color = Color.Empty;
      pointStyle = ScatterPlotDataRowPointStyle.Circle;
      pointSize = 3;
      displayName = String.Empty;
      isVisibleInLegend = true;
      regressionType = ScatterPlotDataRowRegressionType.None;
      polynomialRegressionOrder = 2;
      isRegressionVisibleInLegend = true;
      regressionDisplayName = string.Empty;
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
