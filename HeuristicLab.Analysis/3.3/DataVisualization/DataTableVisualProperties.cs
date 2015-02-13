#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.ComponentModel;
using System.Drawing;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Visual properties of a DataTable.
  /// </summary>
  [StorableClass]
  public class DataTableVisualProperties : DeepCloneable, INotifyPropertyChanged {
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

    private string secondXAxisTitle;
    public string SecondXAxisTitle {
      get { return secondXAxisTitle; }
      set {
        if (value == null) value = string.Empty;
        if (secondXAxisTitle != value) {
          secondXAxisTitle = value;
          OnPropertyChanged("SecondXAxisTitle");
        }
      }
    }

    private string secondYAxisTitle;
    public string SecondYAxisTitle {
      get { return secondYAxisTitle; }
      set {
        if (value == null) value = string.Empty;
        if (secondYAxisTitle != value) {
          secondYAxisTitle = value;
          OnPropertyChanged("SecondYAxisTitle");
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

    private bool secondXAxisMinimumAuto;
    public bool SecondXAxisMinimumAuto {
      get { return secondXAxisMinimumAuto; }
      set {
        if (secondXAxisMinimumAuto != value) {
          secondXAxisMinimumAuto = value;
          if (value) secondXAxisMinimumFixedValue = double.NaN;
          OnPropertyChanged("SecondXAxisMinimumAuto");
        }
      }
    }

    private bool secondXAxisMaximumAuto;
    public bool SecondXAxisMaximumAuto {
      get { return secondXAxisMaximumAuto; }
      set {
        if (secondXAxisMaximumAuto != value) {
          secondXAxisMaximumAuto = value;
          if (value) secondXAxisMaximumFixedValue = double.NaN;
          OnPropertyChanged("SecondXAxisMaximumAuto");
        }
      }
    }

    private double secondXAxisMinimumFixedValue;
    public double SecondXAxisMinimumFixedValue {
      get { return secondXAxisMinimumFixedValue; }
      set {
        if (secondXAxisMinimumFixedValue != value) {
          secondXAxisMinimumFixedValue = value;
          OnPropertyChanged("SecondXAxisMinimumFixedValue");
        }
      }
    }

    private double secondXAxisMaximumFixedValue;
    public double SecondXAxisMaximumFixedValue {
      get { return secondXAxisMaximumFixedValue; }
      set {
        if (secondXAxisMaximumFixedValue != value) {
          secondXAxisMaximumFixedValue = value;
          OnPropertyChanged("SecondXAxisMaximumFixedValue");
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

    private bool secondYAxisMinimumAuto;
    public bool SecondYAxisMinimumAuto {
      get { return secondYAxisMinimumAuto; }
      set {
        if (secondYAxisMinimumAuto != value) {
          secondYAxisMinimumAuto = value;
          if (value) secondYAxisMinimumFixedValue = double.NaN;
          OnPropertyChanged("SecondYAxisMinimumAuto");
        }
      }
    }

    private bool secondYAxisMaximumAuto;
    public bool SecondYAxisMaximumAuto {
      get { return secondYAxisMaximumAuto; }
      set {
        if (secondYAxisMaximumAuto != value) {
          secondYAxisMaximumAuto = value;
          if (value) secondYAxisMaximumFixedValue = double.NaN;
          OnPropertyChanged("SecondYAxisMaximumAuto");
        }
      }
    }

    private double secondYAxisMinimumFixedValue;
    public double SecondYAxisMinimumFixedValue {
      get { return secondYAxisMinimumFixedValue; }
      set {
        if (secondYAxisMinimumFixedValue != value) {
          secondYAxisMinimumFixedValue = value;
          OnPropertyChanged("SecondYAxisMinimumFixedValue");
        }
      }
    }

    private double secondYAxisMaximumFixedValue;
    public double SecondYAxisMaximumFixedValue {
      get { return secondYAxisMaximumFixedValue; }
      set {
        if (secondYAxisMaximumFixedValue != value) {
          secondYAxisMaximumFixedValue = value;
          OnPropertyChanged("SecondYAxisMaximumFixedValue");
        }
      }
    }

    private bool xAxisLogScale;
    public bool XAxisLogScale {
      get { return xAxisLogScale; }
      set {
        if (xAxisLogScale == value) return;
        xAxisLogScale = value;
        OnPropertyChanged("XAxisLogScale");
      }
    }

    private bool secondXAxisLogScale;
    public bool SecondXAxisLogScale {
      get { return secondXAxisLogScale; }
      set {
        if (secondXAxisLogScale == value) return;
        secondXAxisLogScale = value;
        OnPropertyChanged("SecondXAxisLogScale");
      }
    }

    private bool yAxisLogScale;
    public bool YAxisLogScale {
      get { return yAxisLogScale; }
      set {
        if (yAxisLogScale == value) return;
        yAxisLogScale = value;
        OnPropertyChanged("YAxisLogScale");
      }
    }

    private bool secondYAxisLogScale;
    public bool SecondYAxisLogScale {
      get { return secondYAxisLogScale; }
      set {
        if (secondYAxisLogScale == value) return;
        secondYAxisLogScale = value;
        OnPropertyChanged("SecondYAxisLogScale");
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
    [Storable(Name = "SecondXAxisTitle")]
    private string StorableSecondXAxisTitle {
      get { return secondXAxisTitle; }
      set { secondXAxisTitle = value; }
    }
    [Storable(Name = "SecondYAxisTitle")]
    private string StorableSecondYAxisTitle {
      get { return secondYAxisTitle; }
      set { secondYAxisTitle = value; }
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
    [Storable(Name = "SecondXAxisMinimumAuto")]
    private bool StorableSecondXAxisMinimumAuto {
      get { return secondXAxisMinimumAuto; }
      set { secondXAxisMinimumAuto = value; }
    }
    [Storable(Name = "SecondXAxisMaximumAuto")]
    private bool StorableSecondXAxisMaximumAuto {
      get { return secondXAxisMaximumAuto; }
      set { secondXAxisMaximumAuto = value; }
    }
    [Storable(Name = "SecondXAxisMinimumFixedValue")]
    private double StorableSecondXAxisMinimumFixedValue {
      get { return secondXAxisMinimumFixedValue; }
      set { secondXAxisMinimumFixedValue = value; }
    }
    [Storable(Name = "SecondXAxisMaximumFixedValue")]
    private double StorableSecondXAxisMaximumFixedValue {
      get { return secondXAxisMaximumFixedValue; }
      set { secondXAxisMaximumFixedValue = value; }
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
    [Storable(Name = "SecondYAxisMinimumAuto")]
    private bool StorableSecondYAxisMinimumAuto {
      get { return secondYAxisMinimumAuto; }
      set { secondYAxisMinimumAuto = value; }
    }
    [Storable(Name = "SecondYAxisMaximumAuto")]
    private bool StorableSecondYAxisMaximumAuto {
      get { return secondYAxisMaximumAuto; }
      set { secondYAxisMaximumAuto = value; }
    }
    [Storable(Name = "SecondYAxisMinimumFixedValue")]
    private double StorableSecondYAxisMinimumFixedValue {
      get { return secondYAxisMinimumFixedValue; }
      set { secondYAxisMinimumFixedValue = value; }
    }
    [Storable(Name = "SecondYAxisMaximumFixedValue")]
    private double StorableSecondYAxisMaximumFixedValue {
      get { return secondYAxisMaximumFixedValue; }
      set { secondYAxisMaximumFixedValue = value; }
    }
    [Storable(Name = "XAxisLogScale")]
    private bool StorableXAxisLogScale {
      get { return xAxisLogScale; }
      set { xAxisLogScale = value; }
    }
    [Storable(Name = "SecondXAxisLogScale")]
    private bool StorableSecondXAxisLogScale {
      get { return secondXAxisLogScale; }
      set { secondXAxisLogScale = value; }
    }
    [Storable(Name = "YAxisLogScale")]
    private bool StorableYAxisLogScale {
      get { return yAxisLogScale; }
      set { yAxisLogScale = value; }
    }
    [Storable(Name = "SecondYAxisLogScale")]
    private bool StorableSecondYAxisLogScale {
      get { return secondYAxisLogScale; }
      set { secondYAxisLogScale = value; }
    }
    #endregion

    [StorableConstructor]
    protected DataTableVisualProperties(bool deserializing) : base() { }
    protected DataTableVisualProperties(DataTableVisualProperties original, Cloner cloner)
      : base(original, cloner) {
      if (original.titleFont != null)
        this.titleFont = (Font)original.titleFont.Clone();
      if (original.axisTitleFont != null)
        this.axisTitleFont = (Font)original.axisTitleFont.Clone();
      this.title = original.title;
      this.xAxisTitle = original.xAxisTitle;
      this.yAxisTitle = original.yAxisTitle;
      this.secondXAxisTitle = original.secondXAxisTitle;
      this.secondYAxisTitle = original.secondYAxisTitle;
      this.xAxisMinimumAuto = original.xAxisMinimumAuto;
      this.xAxisMinimumFixedValue = original.xAxisMinimumFixedValue;
      this.xAxisMaximumAuto = original.xAxisMaximumAuto;
      this.xAxisMaximumFixedValue = original.xAxisMaximumFixedValue;
      this.secondXAxisMinimumAuto = original.secondXAxisMinimumAuto;
      this.secondXAxisMinimumFixedValue = original.secondXAxisMinimumFixedValue;
      this.secondXAxisMaximumAuto = original.secondXAxisMaximumAuto;
      this.secondXAxisMaximumFixedValue = original.secondXAxisMaximumFixedValue;
      this.yAxisMinimumAuto = original.yAxisMinimumAuto;
      this.yAxisMinimumFixedValue = original.yAxisMinimumFixedValue;
      this.yAxisMaximumAuto = original.yAxisMaximumAuto;
      this.yAxisMaximumFixedValue = original.yAxisMaximumFixedValue;
      this.secondYAxisMinimumAuto = original.secondYAxisMinimumAuto;
      this.secondYAxisMinimumFixedValue = original.secondYAxisMinimumFixedValue;
      this.secondYAxisMaximumAuto = original.secondYAxisMaximumAuto;
      this.secondYAxisMaximumFixedValue = original.secondYAxisMaximumFixedValue;
      this.xAxisLogScale = original.xAxisLogScale;
      this.secondXAxisLogScale = original.secondXAxisLogScale;
      this.yAxisLogScale = original.yAxisLogScale;
      this.secondYAxisLogScale = original.secondYAxisLogScale;
    }
    public DataTableVisualProperties() {
      this.titleColor = Color.Black;
      this.axisTitleColor = Color.Black;
      this.title = string.Empty;
      this.xAxisTitle = string.Empty;
      this.yAxisTitle = string.Empty;
      this.secondXAxisTitle = string.Empty;
      this.secondYAxisTitle = string.Empty;
      this.xAxisMinimumAuto = true;
      this.xAxisMinimumFixedValue = double.NaN;
      this.xAxisMaximumAuto = true;
      this.xAxisMaximumFixedValue = double.NaN;
      this.secondXAxisMinimumAuto = true;
      this.secondXAxisMinimumFixedValue = double.NaN;
      this.secondXAxisMaximumAuto = true;
      this.secondXAxisMaximumFixedValue = double.NaN;
      this.yAxisMinimumAuto = true;
      this.yAxisMinimumFixedValue = double.NaN;
      this.yAxisMaximumAuto = true;
      this.yAxisMaximumFixedValue = double.NaN;
      this.secondYAxisMinimumAuto = true;
      this.secondYAxisMinimumFixedValue = double.NaN;
      this.secondYAxisMaximumAuto = true;
      this.secondYAxisMaximumFixedValue = double.NaN;
      this.xAxisLogScale = false;
      this.secondXAxisLogScale = false;
      this.yAxisLogScale = false;
      this.secondYAxisLogScale = false;
    }
    public DataTableVisualProperties(string title)
      : this() {
      this.title = title;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataTableVisualProperties(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      // check if most of the properties that were added in Mai'11 were set to their default values, then we want to reset them to our default values
      if (xAxisMinimumAuto == default(bool) && xAxisMaximumAuto == default(bool)
        && yAxisMinimumAuto == default(bool) && yAxisMaximumAuto == default(bool)
        && secondXAxisMinimumAuto == default(bool) && secondXAxisMaximumAuto == default(bool)
        && secondYAxisMinimumAuto == default(bool) && secondYAxisMaximumAuto == default(bool)
        && titleColor == default(Color) && axisTitleColor == default(Color)
        && secondXAxisTitle == default(string)
        && xAxisMinimumFixedValue == default(double) && xAxisMaximumFixedValue == default(double)
        && yAxisMinimumFixedValue == default(double) && yAxisMaximumFixedValue == default(double)
        && secondXAxisMinimumFixedValue == default(double) && secondXAxisMaximumFixedValue == default(double)
        && secondYAxisMinimumFixedValue == default(double) && secondYAxisMaximumFixedValue == default(double)) {
        titleColor = Color.Black;
        axisTitleColor = Color.Black;
        this.secondXAxisTitle = string.Empty;
        this.xAxisMinimumAuto = true;
        this.xAxisMinimumFixedValue = double.NaN;
        this.xAxisMaximumAuto = true;
        this.xAxisMaximumFixedValue = double.NaN;
        this.secondXAxisMinimumAuto = true;
        this.secondXAxisMinimumFixedValue = double.NaN;
        this.secondXAxisMaximumAuto = true;
        this.secondXAxisMaximumFixedValue = double.NaN;
        this.yAxisMinimumAuto = true;
        this.yAxisMinimumFixedValue = double.NaN;
        this.yAxisMaximumAuto = true;
        this.yAxisMaximumFixedValue = double.NaN;
        this.secondYAxisMinimumAuto = true;
        this.secondYAxisMinimumFixedValue = double.NaN;
        this.secondYAxisMaximumAuto = true;
        this.secondYAxisMaximumFixedValue = double.NaN;
      }
      #endregion
    }
  }
}
