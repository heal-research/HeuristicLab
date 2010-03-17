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

using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  /// <summary>
  /// The visual representation of a <see cref="Variable"/>.
  /// </summary>
  [View("DataTable View")]
  [Content(typeof(DataTable), true)]
  public sealed partial class DataTableView : NamedItemView {
    Dictionary<IObservableList<double>, DataRow> valuesRowsTable;
    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new DataTable Content {
      get { return (DataTable)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public DataTableView() {
      InitializeComponent();
      Caption = "DataTable";
      valuesRowsTable = new Dictionary<IObservableList<double>, DataRow>();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with the given <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariableView()"/>.</remarks>
    /// <param name="variable">The variable to represent visually.</param>
    public DataTableView(DataTable content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="Variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      foreach (DataRow row in Content.Rows)
        DeregisterDataRowEvents(row);
      Content.Rows.ItemsAdded -= new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsAdded);
      Content.Rows.ItemsRemoved -= new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsRemoved);
      Content.Rows.ItemsReplaced -= new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsReplaced);
      Content.Rows.CollectionReset -= new CollectionItemsChangedEventHandler<DataRow>(Rows_CollectionReset);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="Variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Rows.ItemsAdded += new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsAdded);
      Content.Rows.ItemsRemoved += new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsRemoved);
      Content.Rows.ItemsReplaced += new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsReplaced);
      Content.Rows.CollectionReset += new CollectionItemsChangedEventHandler<DataRow>(Rows_CollectionReset);
      foreach (DataRow row in Content.Rows)
        RegisterDataRowEvents(row);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      chart.Titles.Clear();
      chart.Series.Clear();
      if (Content == null) {
        Caption = "DataTable";
        chart.Enabled = false;
      } else {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        chart.Enabled = true;
        chart.Titles.Add(new Title(Content.Name, Docking.Top));
        foreach (DataRow row in Content.Rows)
          AddDataRow(row);
      }
    }

    private void AddDataRow(DataRow row) {
      Series series = new Series(row.Name);
      series.ChartType = SeriesChartType.FastLine;
      series.ToolTip = "#VAL";
      for (int i = 0; i < row.Values.Count; i++) {
        if (double.IsNaN(row.Values[i])) {
          DataPoint point = new DataPoint();
          point.IsEmpty = true;
          series.Points.Add(point);
        } else {
          series.Points.Add(row.Values[i]);
        }
      }
      chart.Series.Add(series);
    }
    private void RemoveDataRow(DataRow row) {
      Series series = chart.Series[row.Name];
      chart.Series.Remove(series);
    }

    #region Content Events
    private void RegisterDataRowEvents(DataRow row) {
      row.NameChanged += new EventHandler(Row_NameChanged);
      valuesRowsTable.Add(row.Values, row);
      row.Values.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsAdded);
      row.Values.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsRemoved);
      row.Values.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsReplaced);
      row.Values.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsMoved);
      row.Values.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_CollectionReset);
    }
    private void DeregisterDataRowEvents(DataRow row) {
      row.Values.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsAdded);
      row.Values.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsRemoved);
      row.Values.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsReplaced);
      row.Values.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsMoved);
      row.Values.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_CollectionReset);
      valuesRowsTable.Remove(row.Values);
      row.NameChanged -= new EventHandler(Row_NameChanged);
    }
    protected override void Content_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_NameChanged), sender, e);
      else {
        chart.Titles[0].Text = Content.Name;
        base.Content_NameChanged(sender, e);
      }
    }
    private void Rows_ItemsAdded(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsAdded), sender, e);
      else {
        foreach (DataRow row in e.Items) {
          AddDataRow(row);
          RegisterDataRowEvents(row);
        }
      }
    }
    private void Rows_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsRemoved), sender, e);
      else {
        foreach (DataRow row in e.Items) {
          DeregisterDataRowEvents(row);
          RemoveDataRow(row);
        }
      }
    }
    private void Rows_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsReplaced), sender, e);
      else {
        foreach (DataRow row in e.OldItems) {
          DeregisterDataRowEvents(row);
          RemoveDataRow(row);
        }
        foreach (DataRow row in e.Items) {
          AddDataRow(row);
          RegisterDataRowEvents(row);
        }
      }
    }
    private void Rows_CollectionReset(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<DataRow>(Rows_CollectionReset), sender, e);
      else {
        foreach (DataRow row in e.OldItems) {
          DeregisterDataRowEvents(row);
          RemoveDataRow(row);
        }
        foreach (DataRow row in e.Items) {
          AddDataRow(row);
          RegisterDataRowEvents(row);
        }
      }
    }
    private void Row_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Row_NameChanged), sender, e);
      else {
        DataRow row = (DataRow)sender;
        chart.Series[row.Name].Name = row.Name;
      }
    }
    private void Values_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsAdded), sender, e);
      else {
        DataRow row = valuesRowsTable[(IObservableList<double>)sender];
        foreach (IndexedItem<double> item in e.Items) {
          if (double.IsNaN(item.Value)) {
            DataPoint point = new DataPoint();
            point.IsEmpty = true;
            chart.Series[row.Name].Points.Insert(item.Index, point);
          } else {
            chart.Series[row.Name].Points.InsertY(item.Index, item.Value);
          }
        }
      }
    }
    private void Values_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsRemoved), sender, e);
      else {
        DataRow row = valuesRowsTable[(IObservableList<double>)sender];
        List<DataPoint> points = new List<DataPoint>();
        foreach (IndexedItem<double> item in e.Items)
          points.Add(chart.Series[row.Name].Points[item.Index]);
        foreach (DataPoint point in points)
          chart.Series[row.Name].Points.Remove(point);
      }
    }
    private void Values_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsReplaced), sender, e);
      else {
        DataRow row = valuesRowsTable[(IObservableList<double>)sender];
        foreach (IndexedItem<double> item in e.Items) {
          if (double.IsNaN(item.Value))
            chart.Series[row.Name].Points[item.Index].IsEmpty = true;
          else
            chart.Series[row.Name].Points[item.Index].YValues = new double[] { item.Value };
        }
      }
    }
    private void Values_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsMoved), sender, e);
      else {
        DataRow row = valuesRowsTable[(IObservableList<double>)sender];
        foreach (IndexedItem<double> item in e.Items) {
          if (double.IsNaN(item.Value))
            chart.Series[row.Name].Points[item.Index].IsEmpty = true;
          else
            chart.Series[row.Name].Points[item.Index].YValues = new double[] { item.Value };
        }
      }
    }
    private void Values_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_CollectionReset), sender, e);
      else {
        DataRow row = valuesRowsTable[(IObservableList<double>)sender];
        chart.Series[row.Name].Points.Clear();
        foreach (IndexedItem<double> item in e.Items) {
          if (double.IsNaN(item.Value))
            chart.Series[row.Name].Points[item.Index].IsEmpty = true;
          else
            chart.Series[row.Name].Points[item.Index].YValues = new double[] { item.Value };
        }
      }
    }
    #endregion
  }
}
