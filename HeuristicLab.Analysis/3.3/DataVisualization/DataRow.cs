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
using System.Collections.Generic;
using System.ComponentModel;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// A row of data values.
  /// </summary>
  [Item("DataRow", "A row of data values.")]
  [StorableType("34A66F23-0FBB-414C-81C8-D3AA1C470446")]
  public class DataRow : NamedItem , IDataRow {
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
    private ObservableList<double> values;
    public ObservableList<double> Values {
      get { return values; }
    }

    #region Persistence Properties
    [Storable(Name = "VisualProperties")]
    private DataRowVisualProperties StorableVisualProperties {
      get { return visualProperties; }
      set {
        visualProperties = value;
        visualProperties.PropertyChanged += new PropertyChangedEventHandler(VisualProperties_PropertyChanged);
      }
    }
    [Storable(Name = "values")]
    private IEnumerable<double> StorableValues {
      get { return values; }
      set { values = new ObservableList<double>(value); }
    }
    #endregion

    [StorableConstructor]
    protected DataRow(StorableConstructorFlag _) : base(_) { }
    protected DataRow(DataRow original, Cloner cloner)
      : base(original, cloner) {
      this.VisualProperties = (DataRowVisualProperties)cloner.Clone(original.visualProperties);
      this.values = new ObservableList<double>(original.values);
    }
    public DataRow() : this("DataRow") { }
    public DataRow(string name)
      : base(name) {
      if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name of a DataRow cannot be empty", name);
      VisualProperties = new DataRowVisualProperties(name);
      values = new ObservableList<double>();
    }
    public DataRow(string name, string description)
      : base(name, description) {
      if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name of a DataRow cannot be empty", name);
      VisualProperties = new DataRowVisualProperties(name);
      values = new ObservableList<double>();
    }
    public DataRow(string name, string description, IEnumerable<double> values)
      : base(name, description) {
      if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name of a DataRow cannot be empty", name);
      VisualProperties = new DataRowVisualProperties(name);
      this.values = new ObservableList<double>(values);
    }

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (VisualProperties == null) VisualProperties = new DataRowVisualProperties();
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataRow(this, cloner);
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
