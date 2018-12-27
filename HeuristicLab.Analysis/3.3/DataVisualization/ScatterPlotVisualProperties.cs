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

using System.ComponentModel;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Visual properties of a ScatterPlot.
  /// </summary>
  [StorableClass]
  public class ScatterPlotVisualProperties : DeepCloneable, INotifyPropertyChanged {
    private Font titleFont;
    public Font TitleFont {
      get { return titleFont; }
      set {
        if (titleFont == null || value == null
          || titleFont.Name != value.Name || titleFont.Size != value.Size || titleFont.Style != value.Style) {
          titleFont = value;
          OnPropertyChanged("TitleFont");
        }
      }
    }
    private Color titleColor;
    public Color TitleColor {
      get { return titleColor; }
      set {
        if (titleColor != value) {
          titleColor = value;
          OnPropertyChanged("TitleFontColor");
        }
      }
    }
    private string title;
    public string Title {
      get { return title; }
      set {
        if (title != value) {
          title = value;
          OnPropertyChanged("Title");
        }
      }
    }
    private Font axisTitleFont;
    public Font AxisTitleFont {
      get { return axisTitleFont; }
      set {
        if (axisTitleFont == null || axisTitleFont == null
          || axisTitleFont.Name != value.Name || axisTitleFont.Size != value.Size || axisTitleFont.Style != value.Style) {
          axisTitleFont = value;
          OnPropertyChanged("AxisTitleFont");
        }
      }
    }
    private Color axisTitleColor;
    public Color AxisTitleColor {
      get { return axisTitleColor; }
      set {
        if (axisTitleColor != value) {
          axisTitleColor = value;
          OnPropertyChanged("AxisTitleColor");
        }
      }
    }
    private string xAxisTitle;
    public string XAxisTitle {
      get { return xAxisTitle; }
      set {
        if (value == null) value = string.Empty;
        if (xAxisTitle != value) {
          xAxisTitle = value;
          OnPropertyChanged("XAxisTitle");
        }
      }
    }

    private string yAxisTitle;
    public string YAxisTitle {
      get { return yAxisTitle; }
      set {
        if (value == null) value = string.Empty;
        if (yAxisTitle != value) {
          yAxisTitle = value;
          OnPropertyChanged("YAxisTitle");
        }
      }
    }

    private bool xAxisGrid;
    public bool XAxisGrid {
      get { return xAxisGrid; }
      set {
        if (xAxisGrid != value) {
          xAxisGrid = value;
          OnPropertyChanged("XAxisGrid");
        }
      }
    }

    private bool yAxisGrid;
    public bool YAxisGrid {
      get { return yAxisGrid; }
      set {
        if (yAxisGrid != value) {
          yAxisGrid = value;
          OnPropertyChanged("YAxisGrid");
        }
      }
    }

    private bool xAxisMinimumAuto;
    public bool XAxisMinimumAuto {
      get { return xAxisMinimumAuto; }
      set {
        if (xAxisMinimumAuto != value) {
          xAxisMinimumAuto = value;
          if (value) xAxisMinimumFixedValue = double.NaN;
          OnPropertyChanged("XAxisMinimumAuto");
        }
      }
    }

    private bool xAxisMaximumAuto;
    public bool XAxisMaximumAuto {
      get { return xAxisMaximumAuto; }
      set {
        if (xAxisMaximumAuto != value) {
          xAxisMaximumAuto = value;
          if (value) xAxisMaximumFixedValue = double.NaN;
          OnPropertyChanged("XAxisMaximumAuto");
        }
      }
    }

    private double xAxisMinimumFixedValue;
    public double XAxisMinimumFixedValue {
      get { return xAxisMinimumFixedValue; }
      set {
        if (xAxisMinimumFixedValue != value) {
          xAxisMinimumFixedValue = value;
          OnPropertyChanged("XAxisMinimumFixedValue");
        }
      }
    }

    private double xAxisMaximumFixedValue;
    public double XAxisMaximumFixedValue {
      get { return xAxisMaximumFixedValue; }
      set {
        if (xAxisMaximumFixedValue != value) {
          xAxisMaximumFixedValue = value;
          OnPropertyChanged("XAxisMaximumFixedValue");
        }
      }
    }

    private bool yAxisMinimumAuto;
    public bool YAxisMinimumAuto {
      get { return yAxisMinimumAuto; }
      set {
        if (yAxisMinimumAuto != value) {
          yAxisMinimumAuto = value;
          if (value) yAxisMinimumFixedValue = double.NaN;
          OnPropertyChanged("YAxisMinimumAuto");
        }
      }
    }

    private bool yAxisMaximumAuto;
    public bool YAxisMaximumAuto {
      get { return yAxisMaximumAuto; }
      set {
        if (yAxisMaximumAuto != value) {
          yAxisMaximumAuto = value;
          if (value) yAxisMaximumFixedValue = double.NaN;
          OnPropertyChanged("YAxisMaximumAuto");
        }
      }
    }

    private double yAxisMinimumFixedValue;
    public double YAxisMinimumFixedValue {
      get { return yAxisMinimumFixedValue; }
      set {
        if (yAxisMinimumFixedValue != value) {
          yAxisMinimumFixedValue = value;
          OnPropertyChanged("YAxisMinimumFixedValue");
        }
      }
    }

    private double yAxisMaximumFixedValue;
    public double YAxisMaximumFixedValue {
      get { return yAxisMaximumFixedValue; }
      set {
        if (yAxisMaximumFixedValue != value) {
          yAxisMaximumFixedValue = value;
          OnPropertyChanged("YAxisMaximumFixedValue");
        }
      }
    }

    #region Persistence Properties
    [Storable(Name = "TitleFont")]
    private Font StorableTitleFont {
      get { return titleFont; }
      set { titleFont = value; }
    }
    [Storable(Name = "TitleColor")]
    private Color StorableTitleColor {
      get { return titleColor; }
      set { titleColor = value; }
    }
    [Storable(Name = "Title")]
    private string StorableTitle {
      get { return title; }
      set { title = value; }
    }
    [Storable(Name = "AxisTitleFont")]
    private Font StorableAxisTitleFont {
      get { return axisTitleFont; }
      set { axisTitleFont = value; }
    }
    [Storable(Name = "AxisTitleColor")]
    private Color StorableAxisTitleColor {
      get { return axisTitleColor; }
      set { axisTitleColor = value; }
    }
    [Storable(Name = "XAxisTitle")]
    private string StorableXAxisTitle {
      get { return xAxisTitle; }
      set { xAxisTitle = value; }
    }
    [Storable(Name = "YAxisTitle")]
    private string StorableYAxisTitle {
      get { return yAxisTitle; }
      set { yAxisTitle = value; }
    }
    [Storable(Name = "XAxisGrid")]
    private bool StorableXAxisGrid {
      get { return xAxisGrid; }
      set { xAxisGrid = value; }
    }
    [Storable(Name = "YAxisGrid")]
    private bool StorableYAxisGrid {
      get { return yAxisGrid; }
      set { yAxisGrid = value; }
    }
    [Storable(Name = "XAxisMinimumAuto")]
    private bool StorableXAxisMinimumAuto {
      get { return xAxisMinimumAuto; }
      set { xAxisMinimumAuto = value; }
    }
    [Storable(Name = "XAxisMaximumAuto")]
    private bool StorableXAxisMaximumAuto {
      get { return xAxisMaximumAuto; }
      set { xAxisMaximumAuto = value; }
    }
    [Storable(Name = "XAxisMinimumFixedValue")]
    private double StorableXAxisMinimumFixedValue {
      get { return xAxisMinimumFixedValue; }
      set { xAxisMinimumFixedValue = value; }
    }
    [Storable(Name = "XAxisMaximumFixedValue")]
    private double StorableXAxisMaximumFixedValue {
      get { return xAxisMaximumFixedValue; }
      set { xAxisMaximumFixedValue = value; }
    }
    [Storable(Name = "YAxisMinimumAuto")]
    private bool StorableYAxisMinimumAuto {
      get { return yAxisMinimumAuto; }
      set { yAxisMinimumAuto = value; }
    }
    [Storable(Name = "YAxisMaximumAuto")]
    private bool StorableYAxisMaximumAuto {
      get { return yAxisMaximumAuto; }
      set { yAxisMaximumAuto = value; }
    }
    [Storable(Name = "YAxisMinimumFixedValue")]
    private double StorableYAxisMinimumFixedValue {
      get { return yAxisMinimumFixedValue; }
      set { yAxisMinimumFixedValue = value; }
    }
    [Storable(Name = "YAxisMaximumFixedValue")]
    private double StorableYAxisMaximumFixedValue {
      get { return yAxisMaximumFixedValue; }
      set { yAxisMaximumFixedValue = value; }
    }
    #endregion

    [StorableConstructor]
    protected ScatterPlotVisualProperties(bool deserializing) : base() { }
    protected ScatterPlotVisualProperties(ScatterPlotVisualProperties original, Cloner cloner)
      : base(original, cloner) {
      if (original.titleFont != null)
        this.titleFont = (Font)original.titleFont.Clone();
      if (original.axisTitleFont != null)
        this.axisTitleFont = (Font)original.axisTitleFont.Clone();
      this.title = original.title;
      this.xAxisTitle = original.xAxisTitle;
      this.yAxisTitle = original.yAxisTitle;
      this.xAxisGrid = original.xAxisGrid;
      this.yAxisGrid = original.yAxisGrid;
      this.xAxisMinimumAuto = original.xAxisMinimumAuto;
      this.xAxisMinimumFixedValue = original.xAxisMinimumFixedValue;
      this.xAxisMaximumAuto = original.xAxisMaximumAuto;
      this.xAxisMaximumFixedValue = original.xAxisMaximumFixedValue;
      this.yAxisMinimumAuto = original.yAxisMinimumAuto;
      this.yAxisMinimumFixedValue = original.yAxisMinimumFixedValue;
      this.yAxisMaximumAuto = original.yAxisMaximumAuto;
      this.yAxisMaximumFixedValue = original.yAxisMaximumFixedValue;
    }
    public ScatterPlotVisualProperties() {
      this.titleColor = Color.Black;
      this.axisTitleColor = Color.Black;
      this.title = string.Empty;
      this.xAxisTitle = string.Empty;
      this.yAxisTitle = string.Empty;
      this.xAxisGrid = true;
      this.yAxisGrid = true;
      this.xAxisMinimumAuto = true;
      this.xAxisMinimumFixedValue = double.NaN;
      this.xAxisMaximumAuto = true;
      this.xAxisMaximumFixedValue = double.NaN;
      this.yAxisMinimumAuto = true;
      this.yAxisMinimumFixedValue = double.NaN;
      this.yAxisMaximumAuto = true;
      this.yAxisMaximumFixedValue = double.NaN;
    }
    public ScatterPlotVisualProperties(string title)
      : this() {
      this.title = title;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlotVisualProperties(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
