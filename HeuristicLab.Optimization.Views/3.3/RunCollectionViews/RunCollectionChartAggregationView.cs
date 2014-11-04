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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("Chart Aggregation")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionChartAggregationView : ItemView {
    private const string AllDataRows = "All DataRows";

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    private int rowNumber = 0;
    private bool suppressUpdates;
    private readonly Dictionary<IRun, IEnumerable<DataRow>> runMapping;
    private readonly DataTable combinedDataTable;
    public DataTable CombinedDataTable {
      get { return combinedDataTable; }
    }

    public RunCollectionChartAggregationView() {
      InitializeComponent();
      runMapping = new Dictionary<IRun, IEnumerable<DataRow>>();
      combinedDataTable = new DataTable("Combined DataTable", "A data table containing data rows from multiple runs.");
      viewHost.Content = combinedDataTable;
      suppressUpdates = false;
    }

    #region Content events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.UpdateOfRunsInProgressChanged += new EventHandler(Content_UpdateOfRunsInProgressChanged);
      Content.OptimizerNameChanged += new EventHandler(Content_AlgorithmNameChanged);
    }
    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.UpdateOfRunsInProgressChanged -= new EventHandler(Content_UpdateOfRunsInProgressChanged);
      Content.OptimizerNameChanged -= new EventHandler(Content_AlgorithmNameChanged);
      base.DeregisterContentEvents();
    }

    private void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded), sender, e);
        return;
      }
      AddRuns(e.Items);
    }
    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved), sender, e);
        return;
      }
      RemoveRuns(e.Items);
    }
    private void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset), sender, e);
        return;
      }
      RemoveRuns(e.OldItems);
      AddRuns(e.Items);
    }
    private void Content_AlgorithmNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_AlgorithmNameChanged), sender, e);
      else UpdateCaption();
    }
    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_UpdateOfRunsInProgressChanged), sender, e);
        return;
      }
      suppressUpdates = Content.UpdateOfRunsInProgress;
      if (!suppressUpdates) UpdateRuns(Content);
    }

    private void RegisterRunEvents(IRun run) {
      run.PropertyChanged += run_PropertyChanged;
    }
    private void DeregisterRunEvents(IRun run) {
      run.PropertyChanged -= run_PropertyChanged;
    }
    private void run_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired) {
        Invoke((Action<object, PropertyChangedEventArgs>)run_PropertyChanged, sender, e);
      } else {
        var run = (IRun)sender;
        if (e.PropertyName == "Color" || e.PropertyName == "Visible")
          UpdateRuns(new IRun[] { run });
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      dataTableComboBox.Items.Clear();
      dataRowComboBox.Items.Clear();
      combinedDataTable.Rows.Clear();
      runMapping.Clear();

      UpdateCaption();
      if (Content != null) {
        UpdateDataTableComboBox();
      }
    }

    private void RebuildCombinedDataTable() {
      RemoveRuns(Content);
      rowNumber = 0;
      AddRuns(Content);
    }

    private void AddRuns(IEnumerable<IRun> runs) {
      foreach (var run in runs) {
        runMapping[run] = ExtractDataRowsFromRun(run).ToList();
        RegisterRunEvents(run);
      }
      var dataRows = runs.Where(r => r.Visible && runMapping.ContainsKey(r)).SelectMany(r => runMapping[r]);
      combinedDataTable.Rows.AddRange(dataRows);
    }

    private void RemoveRuns(IEnumerable<IRun> runs) {
      var dataRows = runs.Where(r => runMapping.ContainsKey(r)).SelectMany(r => runMapping[r]).ToList();
      foreach (var run in runs) {
        if (!runMapping.ContainsKey(run)) continue;
        runMapping.Remove(run);
        DeregisterRunEvents(run);
      }
      combinedDataTable.Rows.RemoveRange(dataRows);
    }

    private void UpdateRuns(IEnumerable<IRun> runs) {
      if (suppressUpdates) return;
      foreach (var run in runs) {
        //update color
        foreach (var dataRow in runMapping[run]) {
          dataRow.VisualProperties.Color = run.Color;
        }
        //update visibility - remove and add all rows to keep the same order as before
        combinedDataTable.Rows.Clear();
        combinedDataTable.Rows.AddRange(runMapping.Where(mapping => mapping.Key.Visible).SelectMany(mapping => mapping.Value));
      }
    }

    private IEnumerable<DataRow> ExtractDataRowsFromRun(IRun run) {
      var resultName = (string)dataTableComboBox.SelectedItem;
      var rowName = (string)dataRowComboBox.SelectedItem;
      if (!run.Results.ContainsKey(resultName)) yield break;

      var dataTable = (DataTable)run.Results[resultName];
      foreach (var dataRow in dataTable.Rows) {
        if (dataRow.Name != rowName && rowName != AllDataRows) continue;
        rowNumber++;
        var clonedRow = (DataRow)dataRow.Clone();
        //row names must be unique -> add incremented number to the row name
        clonedRow.Name = run.Name + "." + dataRow.Name + rowNumber;
        clonedRow.VisualProperties.DisplayName = run.Name + "." + dataRow.Name;
        clonedRow.VisualProperties.Color = run.Color;
        yield return clonedRow;
      }
    }

    private void UpdateDataTableComboBox() {
      dataTableComboBox.Items.Clear();
      var dataTables = (from run in Content
                        from result in run.Results
                        where result.Value is DataTable
                        select result.Key).Distinct().ToArray();

      dataTableComboBox.Items.AddRange(dataTables);
      if (dataTableComboBox.Items.Count > 0) dataTableComboBox.SelectedItem = dataTableComboBox.Items[0];
    }

    private void UpdateCaption() {
      Caption = Content != null ? Content.OptimizerName + " Chart Aggregation" : ViewAttribute.GetViewName(GetType());
    }

    private void UpdateDataRowComboBox() {
      dataRowComboBox.Items.Clear();
      var resultName = (string)dataTableComboBox.SelectedItem;
      var dataTables = from run in Content
                       where run.Results.ContainsKey(resultName)
                       select run.Results[resultName] as DataTable;
      var rowNames = (from dataTable in dataTables
                      from row in dataTable.Rows
                      select row.Name).Distinct().ToArray();

      dataRowComboBox.Items.AddRange(rowNames);
      dataRowComboBox.Items.Add(AllDataRows);
      if (dataRowComboBox.Items.Count > 0) dataRowComboBox.SelectedItem = dataRowComboBox.Items[0];
    }

    private void dataTableComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      UpdateDataRowComboBox();
    }
    private void dataRowComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      RebuildCombinedDataTable();
    }
  }
}
