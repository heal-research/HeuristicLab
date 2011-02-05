#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Visual properties of a DataTable.
  /// </summary>
  [StorableClass]
  public class DataTableVisualProperties : DeepCloneable, INotifyPropertyChanged {
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

    #region Persistence Properties
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
    [Storable(Name = "SecondYAxisTitle")]
    private string StorableSecondYAxisTitle {
      get { return secondYAxisTitle; }
      set { secondYAxisTitle = value; }
    }
    #endregion

    [StorableConstructor]
    protected DataTableVisualProperties(bool deserializing) : base() { }
    protected DataTableVisualProperties(DataTableVisualProperties original, Cloner cloner)
      : base(original, cloner) {
      this.xAxisTitle = original.xAxisTitle;
      this.yAxisTitle = original.yAxisTitle;
      this.secondYAxisTitle = original.secondYAxisTitle;
    }
    public DataTableVisualProperties() {
      this.xAxisTitle = string.Empty;
      this.yAxisTitle = string.Empty;
      this.secondYAxisTitle = string.Empty;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataTableVisualProperties(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
