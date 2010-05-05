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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Data.Views;
using HeuristicLab.Collections;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization.Views {
  [View("RunCollection Tabular View")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionTabularView : StringConvertibleMatrixView {
    public RunCollectionTabularView() {
      InitializeComponent();
      Caption = "Run Collection";
      this.dataGridView.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView_RowHeaderMouseDoubleClick);
      base.ReadOnly = true;
    }

    public override bool ReadOnly {
      get { return base.ReadOnly; }
      set { /*not needed because results are always readonly */}
    }

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        foreach (IRun run in Content)
          UpdateRun(run);
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      RegisterRunEvents(Content);
    }
    protected virtual void RegisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.Changed += new EventHandler(run_Changed);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      DeregisterRunEvents(Content);
    }
    protected virtual void DeregisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.Changed -= new EventHandler(run_Changed);
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
    private void run_Changed(object sender, EventArgs e) {
      if (InvokeRequired)
        this.Invoke(new EventHandler(run_Changed), sender, e);
      else {
        IRun run = (IRun)sender;
        UpdateRun(run);
      }
    }

    private void UpdateRun(IRun run) {
      int rowIndex = Content.ToList().IndexOf(run);
      rowIndex = virtualRowIndizes[rowIndex];
      this.dataGridView.Rows[rowIndex].Visible = run.Visible;
      this.dataGridView.Rows[rowIndex].DefaultCellStyle.ForeColor = run.Color;
      this.rowsTextBox.Text = this.Content.Count(r => r.Visible).ToString();
    }

    private void dataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (e.RowIndex >= 0) {
        IRun run = Content.ElementAt(virtualRowIndizes[e.RowIndex]);
        IContentView view = MainFormManager.MainForm.ShowContent(run);
        if (view != null) {
          view.ReadOnly = this.ReadOnly;
          view.Locked = this.Locked;
        }
      }
    }

    protected override int[] Sort(IEnumerable<KeyValuePair<int, SortOrder>> sortedColumns) {
      int[] newSortedIndex = Enumerable.Range(0, Content.Count).ToArray();
      RunCollectionRowComparer rowComparer = new RunCollectionRowComparer();
      if (sortedColumns.Count() != 0) {
        rowComparer.SortedIndizes = sortedColumns;
        rowComparer.Matrix = Content;
        Array.Sort(newSortedIndex, rowComparer);
      }
      return newSortedIndex;
    }

    public class RunCollectionRowComparer : IComparer<int> {
      public RunCollectionRowComparer() {
      }

      private List<KeyValuePair<int, SortOrder>> sortedIndizes;
      public IEnumerable<KeyValuePair<int, SortOrder>> SortedIndizes {
        get { return this.sortedIndizes; }
        set { sortedIndizes = new List<KeyValuePair<int, SortOrder>>(value); }
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
        if (sortedIndizes == null)
          return 0;

        foreach (KeyValuePair<int, SortOrder> pair in sortedIndizes.Where(p => p.Value != SortOrder.None)) {
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
