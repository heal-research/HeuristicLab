#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("Gantt Data View")]
  [Content(typeof(GanttData), IsDefaultView = true)]
  public partial class GanttDataView : ItemView {

    public new GanttData Content {
      get { return (GanttData)base.Content; }
      set { base.Content = value; }
    }

    public GanttDataView() {
      InitializeComponent();
    }

    #region De/Register Content Events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Rows.ItemsAdded += GanttRowsOnChanged;
      Content.Rows.ItemsRemoved += GanttRowsOnRemoved;
      Content.Rows.ItemsReplaced += GanttRowsOnChanged;
      Content.Rows.CollectionReset += GanttRowsOnChanged;
      foreach (var row in Content.Rows)
        RegisterGanttRowEvents(row);
    }

    private void RegisterGanttRowEvents(GanttRow row) {
      row.Items.ItemsAdded += GanttRowItemsOnChanged;
      row.Items.ItemsReplaced += GanttRowItemsOnChanged;
      row.Items.ItemsRemoved += GanttRowItemsOnRemoved;
      row.Items.CollectionReset += GanttRowItemsOnChanged;
      foreach (var item in row.Items)
        RegisterGanttRowItemEvents(item);
    }

    private void RegisterGanttRowItemEvents(GanttItem item) {
      item.PropertyChanged += GanttRowItemsItemOnChanged;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Rows.ItemsAdded -= GanttRowsOnChanged;
      Content.Rows.ItemsRemoved -= GanttRowsOnRemoved;
      Content.Rows.ItemsReplaced -= GanttRowsOnChanged;
      Content.Rows.CollectionReset -= GanttRowsOnChanged;
      foreach (var row in Content.Rows)
        DeregisterGanttRowEvents(row);
    }

    private void DeregisterGanttRowEvents(GanttRow row) {
      row.Items.ItemsAdded -= GanttRowItemsOnChanged;
      row.Items.ItemsReplaced -= GanttRowItemsOnChanged;
      row.Items.ItemsRemoved -= GanttRowItemsOnRemoved;
      row.Items.CollectionReset -= GanttRowItemsOnChanged;
      foreach (var item in row.Items)
        DeregisterGanttRowItemEvents(item);
    }

    private void DeregisterGanttRowItemEvents(GanttItem item) {
      item.PropertyChanged -= GanttRowItemsItemOnChanged;
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();

      UpdateVisualization();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      ganttChart.Enabled = !Locked && !ReadOnly && Content != null;
    }

    private void UpdateVisualization() {
      ganttChart.Reset();

      if (Content == null) return;

      var colors = (KnownColor[])Enum.GetValues(typeof(KnownColor));

      var categories = Content.Rows.SelectMany(x => x.Items.Select(y => y.Category ?? string.Empty)).Distinct();
      foreach (var cat in categories) {
        Color color;
        if (!Content.CategoryColors.TryGetValue(cat, out color)) {
          // if no color is defined, a random color is chosen for each category
          var rand = new Random(cat.GetHashCode());
          color = Color.FromKnownColor(colors[rand.Next(colors.Length)]);
        }
        ganttChart.AddCategory(cat, color);
      }
      
      ganttChart.Hide();
      foreach (var row in Content.Rows) {
        foreach (var item in row.Items) {
          ganttChart.AddData(row.Name, item.Category ?? string.Empty, item.StartDate, item.EndDate, item.ToolTip, item.ShowLabel);
        }
      }
      ganttChart.Show();
    }

    #region Content Event Handlers
    private void GanttRowsOnChanged(object sender, CollectionItemsChangedEventArgs<GanttRow> e) {
      foreach (var row in e.Items)
        RegisterGanttRowEvents(row);
      foreach (var row in e.OldItems)
        DeregisterGanttRowEvents(row);

      UpdateVisualization();
    }

    private void GanttRowsOnRemoved(object sender, CollectionItemsChangedEventArgs<GanttRow> e) {
      foreach (var row in e.Items)
        DeregisterGanttRowEvents(row);

      UpdateVisualization();
    }
    private void GanttRowItemsOnChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<GanttItem>> e) {
      foreach (var item in e.Items)
        RegisterGanttRowItemEvents(item.Value);
      foreach (var item in e.OldItems)
        DeregisterGanttRowItemEvents(item.Value);

      UpdateVisualization();
    }
    private void GanttRowItemsOnRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<GanttItem>> e) {
      foreach (var item in e.Items)
        DeregisterGanttRowItemEvents(item.Value);

      UpdateVisualization();
    }
    private void GanttRowItemsItemOnChanged(object sender, EventArgs e) {
      UpdateVisualization();
    }
    #endregion
  }
}
