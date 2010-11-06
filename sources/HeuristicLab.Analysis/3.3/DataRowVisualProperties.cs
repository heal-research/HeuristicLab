#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Visual properties of a DataRow.
  /// </summary>
  [StorableClass]
  public class DataRowVisualProperties : DeepCloneable, INotifyPropertyChanged {
    #region ChartType
    public enum DataRowChartType {
      Line,
      Columns,
      Points,
      Bars
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
    [Storable(Name = "Color")]
    private Color StorableColor {
      get { return color; }
      set { color = value; }
    }
    [Storable(Name = "StartIndexZero")]
    private bool StorableStartIndexZero {
      get { return startIndexZero; }
      set { startIndexZero = value; }
    }
    #endregion

    #region Storing & Cloning
    [StorableConstructor]
    protected DataRowVisualProperties(bool deserializing) : base() { }
    protected DataRowVisualProperties(DataRowVisualProperties original, Cloner cloner)
      : base(original, cloner) {
      this.chartType = original.chartType;
      this.secondYAxis = original.secondYAxis;
      this.color = original.color;
      this.startIndexZero = original.startIndexZero;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataRowVisualProperties(this, cloner);
    }
    #endregion
    public DataRowVisualProperties() {
      chartType = DataRowChartType.Line;
      secondYAxis = false;
      color = Color.Empty;
      startIndexZero = false;
    }
    public DataRowVisualProperties(DataRowChartType chartType, bool secondYAxis, Color color, bool startIndexZero) {
      this.chartType = chartType;
      this.secondYAxis = secondYAxis;
      this.color = color;
      this.startIndexZero = startIndexZero;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
