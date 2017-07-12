#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HeuristicLab.Analysis {
  [Item("IndexedDataRow", "A data row that contains a series of points.")]
  [StorableClass]
  public class IndexedDataRow<T> : NamedItem {

    private DataRowVisualProperties visualProperties;
    public DataRowVisualProperties VisualProperties {
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
    private ObservableList<Tuple<T, double>> values;
    public ObservableList<Tuple<T, double>> Values {
      get { return values; }
    }

    #region Persistence Properties
    [Storable(Name = "visualProperties")]
    private DataRowVisualProperties StorableVisualProperties {
      get { return visualProperties; }
      set { visualProperties = value; }
    }
    [Storable(Name = "values")]
    private IEnumerable<Tuple<T, double>> StorableValues {
      get { return values; }
      set { values = new ObservableList<Tuple<T, double>>(value); }
    }
    #endregion

    [StorableConstructor]
    protected IndexedDataRow(bool deserializing) : base(deserializing) { }
    protected IndexedDataRow(IndexedDataRow<T> original, Cloner cloner)
      : base(original, cloner) {
      values = new ObservableList<Tuple<T, double>>(original.values.Select(x => Tuple.Create<T, double>(x.Item1, x.Item2)).ToList());
      VisualProperties = cloner.Clone(original.visualProperties);
    }
    public IndexedDataRow() {
      values = new ObservableList<Tuple<T, double>>();
      VisualProperties = new DataRowVisualProperties();
    }
    public IndexedDataRow(string name)
      : base(name) {
      values = new ObservableList<Tuple<T, double>>();
      VisualProperties = new DataRowVisualProperties(name);
    }
    public IndexedDataRow(string name, string description)
      : base(name, description) {
      values = new ObservableList<Tuple<T, double>>();
      VisualProperties = new DataRowVisualProperties(name);
    }
    public IndexedDataRow(string name, string description, IEnumerable<Tuple<T, double>> values)
      : base(name, description) {
      this.values = new ObservableList<Tuple<T, double>>(values);
      VisualProperties = new DataRowVisualProperties(name);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IndexedDataRow<T>(this, cloner);
    }

    public event EventHandler VisualPropertiesChanged;
    protected virtual void OnVisualPropertiesChanged() {
      EventHandler handler = VisualPropertiesChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void VisualProperties_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      OnVisualPropertiesChanged();
    }
    protected override void OnNameChanged() {
      base.OnNameChanged();
      VisualProperties.DisplayName = Name;
    }
  }
}
