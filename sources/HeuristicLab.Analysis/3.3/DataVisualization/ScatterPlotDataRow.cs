#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.ComponentModel;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// A row of data values for a scatter plot.
  /// </summary>
  [Item("ScatterPlotDataRow", "A row of data values for a scatter plot.")]
  [StorableClass]
  public class ScatterPlotDataRow : NamedItem {
    private ScatterPlotDataRowVisualProperties visualProperties;
    public ScatterPlotDataRowVisualProperties VisualProperties {
      get { return visualProperties; }
      set {
        if (visualProperties != value) {
          if (value == null) throw new ArgumentNullException("VisualProperties");
          if (visualProperties != null) visualProperties.PropertyChanged -= new PropertyChangedEventHandler(VisualProperties_PropertyChanged);
          visualProperties = value;
          visualProperties.PropertyChanged += new PropertyChangedEventHandler(VisualProperties_PropertyChanged);
          OnVisualPropertiesChanged();
        }
      }
    }
    private ObservableList<Point2D<double>> points;
    public ObservableList<Point2D<double>> Points {
      get { return points; }
    }

    #region Persistence Properties
    [Storable(Name = "VisualProperties")]
    private ScatterPlotDataRowVisualProperties StorableVisualProperties {
      get { return visualProperties; }
      set {
        visualProperties = value;
        visualProperties.PropertyChanged += new PropertyChangedEventHandler(VisualProperties_PropertyChanged);
      }
    }
    [Storable(Name = "Points")]
    private IEnumerable<Point2D<double>> StorablePoints {
      get { return points; }
      set { points = new ObservableList<Point2D<double>>(value); }
    }
    #endregion

    [StorableConstructor]
    protected ScatterPlotDataRow(bool deserializing) : base(deserializing) { }
    protected ScatterPlotDataRow(ScatterPlotDataRow original, Cloner cloner)
      : base(original, cloner) {
      VisualProperties = cloner.Clone(original.visualProperties);
      points = new ObservableList<Point2D<double>>(original.points);
    }
    public ScatterPlotDataRow()
      : base() {
      VisualProperties = new ScatterPlotDataRowVisualProperties();
      points = new ObservableList<Point2D<double>>();
    }
    public ScatterPlotDataRow(string name, string description, IEnumerable<Point2D<double>> points)
      : base(name, description) {
      VisualProperties = new ScatterPlotDataRowVisualProperties(name);
      this.points = new ObservableList<Point2D<double>>(points);
    }
    public ScatterPlotDataRow(string name, string description, IEnumerable<Point2D<double>> points, ScatterPlotDataRowVisualProperties visualProperties)
      : base(name, description) {
      VisualProperties = visualProperties;
      this.points = new ObservableList<Point2D<double>>(points);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlotDataRow(this, cloner);
    }

    public event EventHandler VisualPropertiesChanged;
    protected virtual void OnVisualPropertiesChanged() {
      EventHandler handler = VisualPropertiesChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void VisualProperties_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      OnVisualPropertiesChanged();
    }
  }
}
