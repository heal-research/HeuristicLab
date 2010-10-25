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

    [Storable(DefaultValue = DataRowChartType.Line)]
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
    [Storable(DefaultValue = false)]
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

    public DataRowVisualProperties() {
      chartType = DataRowChartType.Line;
      secondYAxis = false;
    }
    public DataRowVisualProperties(DataRowChartType chartType, bool secondYAxis) {
      this.chartType = chartType;
      this.secondYAxis = secondYAxis;
    }
    [StorableConstructor]
    protected DataRowVisualProperties(bool deserializing) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      DataRowVisualProperties clone = (DataRowVisualProperties)base.Clone(cloner);
      clone.chartType = chartType;
      clone.secondYAxis = secondYAxis;
      return clone;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
