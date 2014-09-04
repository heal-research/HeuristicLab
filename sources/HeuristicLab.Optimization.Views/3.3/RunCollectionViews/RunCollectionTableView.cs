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
using HeuristicLab.Core;
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("Table")]
  [Content(typeof(RunCollection), false)]
  public sealed partial class RunCollectionTableView : StringConvertibleMatrixView {
    private int[] runToRowMapping;
    private bool suppressUpdates = false;
    public RunCollectionTableView() {
      InitializeComponent();
      dataGridView.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView_RowHeaderMouseDoubleClick);
    }

    public override bool ReadOnly {
      get { return true; }
      set { /*not needed because results are always readonly */}
    }

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        runToRowMapping = Enumerable.Range(0, Content.Count).ToArray();
        UpdateRowAttributes();
      }
      UpdateCaption();
    }

    #region events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.UpdateOfRunsInProgressChanged += new EventHandler(Content_UpdateOfRunsInProgressChanged);
      Content.OptimizerNameChanged += new EventHandler(Content_AlgorithmNameChanged);
      RegisterRunEvents(Content);
    }
    private void RegisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.PropertyChanged += run_PropertyChanged;
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.UpdateOfRunsInProgressChanged -= new EventHandler(Content_UpdateOfRunsInProgressChanged);
      Content.OptimizerNameChanged -= new EventHandler(Content_AlgorithmNameChanged);
      DeregisterRunEvents(Content);
    }
    private void DeregisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.PropertyChanged -= run_PropertyChanged;
    }
    private void Content_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      DeregisterRunEvents(e.OldItems);
      RegisterRunEvents(e.Items);
    }
    private void Content_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      DeregisterRunEvents(e.Items);
    }
    private void Content_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      RegisterRunEvents(e.Items);
    }
    private void Content_AlgorithmNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_AlgorithmNameChanged), sender, e);
      else UpdateCaption();
    }
    private void run_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired)
        this.Invoke((Action<object, PropertyChangedEventArgs>)run_PropertyChanged, sender, e);
      else {
        IRun run = (IRun)sender;
        if (e.PropertyName == "Color" || e.PropertyName == "Visible")
          UpdateRun(run);
      }
    }
    #endregion

    private void UpdateCaption() {
      Caption = Content != null ? Content.OptimizerName + " Table" : ViewAttribute.GetViewName(GetType());
    }

    protected override void UpdateColumnHeaders() {
      HashSet<string> visibleColumnNames = new HashSet<string>(dataGridView.Columns.OfType<DataGridViewColumn>()
       .Where(c => c.Visible && !string.IsNullOrEmpty(c.HeaderText)).Select(c => c.HeaderText));

      for (int i = 0; i < dataGridView.ColumnCount; i++) {
        if (i < base.Content.ColumnNames.Count())
          dataGridView.Columns[i].HeaderText = base.Content.ColumnNames.ElementAt(i);
        else
          dataGridView.Columns[i].HeaderText = "Column " + (i + 1);
        dataGridView.Columns[i].Visible = visibleColumnNames.Count == 0 || visibleColumnNames.Contains(dataGridView.Columns[i].HeaderText);
      }
    }

    private void UpdateRun(IRun run) {
      foreach (int runIndex in GetIndexOfRun(run)) {
        int rowIndex = runToRowMapping[runIndex];
        this.dataGridView.Rows[rowIndex].Visible = run.Visible;
        this.dataGridView.Rows[rowIndex].DefaultCellStyle.ForeColor = run.Color;
      }
      this.UpdateRowHeaders();
    }


    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_UpdateOfRunsInProgressChanged), sender, e);
      else {
        suppressUpdates = Content.UpdateOfRunsInProgress;
        if (!suppressUpdates) UpdateRowAttributes();
      }
    }

    private IEnumerable<int> GetIndexOfRun(IRun run) {
      int i = 0;
      foreach (IRun actualRun in Content) {
        if (actualRun == run)
          yield return i;
        i++;
      }
    }

    private void dataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (e.RowIndex >= 0) {
        IRun run = Content.ElementAt(runToRowMapping.ToList().IndexOf(e.RowIndex));
        IContentView view = MainFormManager.MainForm.ShowContent(run);
        if (view != null) {
          view.ReadOnly = this.ReadOnly;
          view.Locked = this.Locked;
        }
      }
    }

    protected override void ClearSorting() {
      base.ClearSorting();
      runToRowMapping = Enumerable.Range(0, Content.Count).ToArray();
      UpdateRowAttributes();
    }

    protected override int[] Sort(IEnumerable<KeyValuePair<int, SortOrder>> sortedColumns) {
      int[] newSortedIndex = Enumerable.Range(0, Content.Count).ToArray();
      RunCollectionRowComparer rowComparer = new RunCollectionRowComparer();
      if (sortedColumns.Count() != 0) {
        rowComparer.SortedIndices = sortedColumns;
        rowComparer.Matrix = Content;
        Array.Sort(newSortedIndex, rowComparer);
      }

      runToRowMapping = new int[newSortedIndex.Length];
      int i = 0;
      foreach (int runIndex in newSortedIndex) {
        runToRowMapping[runIndex] = i;
        i++;
      }
      UpdateRowAttributes();
      return newSortedIndex;
    }

    private void UpdateRowAttributes() {
      int runIndex = 0;
      foreach (IRun run in Content) {
        int rowIndex = this.runToRowMapping[runIndex];
        this.dataGridView.Rows[rowIndex].Visible = run.Visible;
        this.dataGridView.Rows[rowIndex].DefaultCellStyle.ForeColor = run.Color;
        runIndex++;
      }
      rowsTextBox.Text = dataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible).ToString();
      UpdateRowHeaders();
    }

    public class RunCollectionRowComparer : IComparer<int> {
      public RunCollectionRowComparer() {
      }

      private List<KeyValuePair<int, SortOrder>> sortedIndices;
      public IEnumerable<KeyValuePair<int, SortOrder>> SortedIndices {
        get { return this.sortedIndices; }
        set { sortedIndices = new List<KeyValuePair<int, SortOrder>>(value); }
      }
      private RunCollection matrix;
      public RunCollection Matrix {
        get { return this.matrix; }
        set { this.matrix = value; }
      }

      public int Compare(int x, int y) {
        int result = 0;
        IItem value1, value2;
        IComparable comparable1, comparable2;

        if (matrix == null)
          throw new InvalidOperationException("Could not sort IStringConvertibleMatrix if the matrix member is null.");
        if (sortedIndices == null)
          return 0;

        foreach (KeyValuePair<int, SortOrder> pair in sortedIndices.Where(p => p.Value != SortOrder.None)) {
          value1 = matrix.GetValue(x, pair.Key);
          value2 = matrix.GetValue(y, pair.Key);
          comparable1 = value1 as IComparable;
          comparable2 = value2 as IComparable;
          if (comparable1 != null)
            result = comparable1.CompareTo(comparable2);
          else {
            string string1 = value1 != null ? value1.ToString() : string.Empty;
            string string2 = value2 != null ? value2.ToString() : string.Empty;
            result = string1.CompareTo(string2);
          }
          if (pair.Value == SortOrder.Descending)
            result *= -1;
          if (result != 0)
            return result;
        }
        return result;
      }
    }
  }
}
