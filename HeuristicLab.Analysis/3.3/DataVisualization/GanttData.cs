#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  [Item("Gantt Data", "Data of a Gantt visualization")]
  [StorableClass]
  public class GanttData : NamedItem {

    [Storable]
    private NamedItemCollection<GanttRow> rows;
    public NamedItemCollection<GanttRow> Rows { get { return rows; } }

    [Storable]
    private ObservableDictionary<string, Color> categoryColors;
    public ObservableDictionary<string, Color> CategoryColors { get { return categoryColors; } }

    [StorableConstructor]
    protected GanttData(bool deserializing) : base(deserializing) {}
    protected GanttData(GanttData original, Cloner cloner)
      : base(original, cloner) {
      rows = cloner.Clone(original.rows);
      categoryColors = new ObservableDictionary<string, Color>(original.categoryColors);
    }
    public GanttData() : this("Gantt Data", "Data of a Gantt visualization") { }
    public GanttData(string name) : this(name, string.Empty) { }
    public GanttData(string name, string description) : base(name, description) {
      rows = new NamedItemCollection<GanttRow>();
      categoryColors = new ObservableDictionary<string, Color>();
    }
    public GanttData(IEnumerable<GanttRow> rows) : this("Gantt Data", rows) { }
    public GanttData(string name, IEnumerable<GanttRow> rows) : this(name, string.Empty, rows) { }
    public GanttData(string name, string description, IEnumerable<GanttRow> rows) : base(name, description) {
      this.rows = new NamedItemCollection<GanttRow>(rows);
      categoryColors = new ObservableDictionary<string, Color>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GanttData(this, cloner);
    }
  }

  [Item("Gantt Row", "Row of a Gantt chart")]
  [StorableClass]
  public class GanttRow : NamedItem {
    [Storable]
    private ItemList<GanttItem> items;
    public ItemList<GanttItem> Items { get { return items; } }

    [StorableConstructor]
    protected GanttRow(bool deserializing) : base(deserializing) {}
    protected GanttRow(GanttRow original, Cloner cloner)
      : base(original, cloner) {
      items = cloner.Clone(original.items);
    }
    public GanttRow() : this("Gantt Row", "Row of a Gantt chart") { }
    public GanttRow(string name) : this(name, string.Empty) { }
    public GanttRow(string name, string description) : base(name, description) {
      items = new ItemList<GanttItem>();
    }
    public GanttRow(IEnumerable<GanttItem> items) : this("Gantt Row", items) { }
    public GanttRow(string name, IEnumerable<GanttItem> items) : this(name, string.Empty, items) { }
    public GanttRow(string name, string description, IEnumerable<GanttItem> items) : base(name, description) {
      this.items = new ItemList<GanttItem>(items);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GanttRow(this, cloner);
    }
  }

  [Item("Gantt Item", "Item of a Gantt chart row")]
  [StorableClass]
  public class GanttItem : Item, INotifyPropertyChanged {

    [Storable]
    private DateTime startDate;
    public DateTime StartDate {
      get { return startDate; }
      set {
        if (startDate == value) return;
        startDate = value;
        OnPropertyChanged("StartDate");
      }
    }

    [Storable]
    private DateTime endDate;
    public DateTime EndDate {
      get { return endDate; }
      set {
        if (endDate == value) return;
        endDate = value;
        OnPropertyChanged("EndDate");
      }
    }

    [Storable]
    private string toolTip;
    public string ToolTip {
      get { return toolTip; }
      set {
        if (toolTip == value) return;
        toolTip = value;
        OnPropertyChanged("ToolTip");
      }
    }

    [Storable]
    private string category;
    public string Category {
      get { return category; }
      set {
        if (category == value) return;
        category = value;
        OnPropertyChanged("Category");
      }
    }

    [Storable]
    private bool showLabel;
    public bool ShowLabel {
      get { return showLabel; }
      set {
        if (showLabel == value) return;
        showLabel = value;
        OnPropertyChanged("ShowLabel");
      }
    }

    [StorableConstructor]
    protected GanttItem(bool deserializing) : base(deserializing) { }
    protected GanttItem(GanttItem original, Cloner cloner)
      : base(original, cloner) {
      startDate = original.startDate;
      endDate = original.endDate;
      toolTip = original.toolTip;
      category = original.category;
      showLabel = original.showLabel;
    }
    public GanttItem() : this(DateTime.Now, DateTime.Now) { }
    public GanttItem(DateTime start, DateTime end) {
      startDate = start;
      endDate = end;
      toolTip = string.Empty;
      category = string.Empty;
      showLabel = false;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GanttItem(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
