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
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Data.Views {
  [View("StringConvertibleMatrix View")]
  [Content(typeof(IStringConvertibleMatrix), true)]
  public partial class StringConvertibleMatrixView : AsynchronousContentView {
    private int[] virtualRowIndizes;
    private List<KeyValuePair<int, SortOrder>> sortedColumnIndizes;
    private RowComparer rowComparer;

    public new IStringConvertibleMatrix Content {
      get { return (IStringConvertibleMatrix)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get {
        if ((Content != null) && Content.ReadOnly) return true;
        return base.ReadOnly;
      }
      set { base.ReadOnly = value; }
    }

    public StringConvertibleMatrixView() {
      InitializeComponent();
      Caption = "StringConvertibleMatrix View";
      errorProvider.SetIconAlignment(rowsTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(rowsTextBox, 2);
      errorProvider.SetIconAlignment(columnsTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(columnsTextBox, 2);
      sortedColumnIndizes = new List<KeyValuePair<int, SortOrder>>();
      rowComparer = new RowComparer();
    }
    public StringConvertibleMatrixView(IStringConvertibleMatrix content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ItemChanged -= new EventHandler<EventArgs<int, int>>(Content_ItemChanged);
      Content.Reset -= new EventHandler(Content_Reset);
      Content.ColumnNamesChanged -= new EventHandler(Content_ColumnNamesChanged);
      Content.RowNamesChanged -= new EventHandler(Content_RowNamesChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemChanged += new EventHandler<EventArgs<int, int>>(Content_ItemChanged);
      Content.Reset += new EventHandler(Content_Reset);
      Content.ColumnNamesChanged += new EventHandler(Content_ColumnNamesChanged);
      Content.RowNamesChanged += new EventHandler(Content_RowNamesChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "StringConvertibleMatrix View";
        rowsTextBox.Text = "";
        columnsTextBox.Text = "";
        dataGridView.Rows.Clear();
        dataGridView.Columns.Clear();
        virtualRowIndizes = new int[0];
      } else {
        Caption = "StringConvertibleMatrix (" + Content.GetType().Name + ")";
        UpdateData();
      }
      SetEnabledStateOfControls();
    }
    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }
    private void SetEnabledStateOfControls() {
      if (Content == null) {
        rowsTextBox.Enabled = false;
        columnsTextBox.Enabled = false;
        dataGridView.Enabled = false;
      } else {
        rowsTextBox.Enabled = true;
        columnsTextBox.Enabled = true;
        dataGridView.Enabled = true;
        rowsTextBox.ReadOnly = ReadOnly;
        columnsTextBox.ReadOnly = ReadOnly;
        dataGridView.ReadOnly = ReadOnly;
      }
    }

    private void UpdateData() {
      sortedColumnIndizes.Clear();
      Sort();
      rowsTextBox.Text = Content.Rows.ToString();
      rowsTextBox.Enabled = true;
      columnsTextBox.Text = Content.Columns.ToString();
      columnsTextBox.Enabled = true;
      virtualRowIndizes = Enumerable.Range(0, Content.Rows).ToArray();
      //DataGridViews with Rows but no columns are not allowed !
      if (Content.Rows == 0 && dataGridView.RowCount != Content.Rows && !Content.ReadOnly)
        Content.Rows = dataGridView.RowCount;
      else
        dataGridView.RowCount = Content.Rows;
      if (Content.Columns == 0 && dataGridView.ColumnCount != Content.Columns && !Content.ReadOnly)
        Content.Columns = dataGridView.ColumnCount;
      else
        dataGridView.ColumnCount = Content.Columns;

      UpdateRowHeaders();
      UpdateColumnHeaders();
      dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
      dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
      dataGridView.Enabled = true;
    }

    private void UpdateColumnHeaders() {
      for (int i = 0; i < Content.Columns; i++) {
        if (Content.ColumnNames.Count() != 0)
          dataGridView.Columns[i].HeaderText = Content.ColumnNames.ElementAt(i);
        else
          dataGridView.Columns[i].HeaderText = "Column " + (i + 1);
      }
      dataGridView.Invalidate();
    }

    private void UpdateRowHeaders() {
      for (int i = dataGridView.FirstDisplayedScrollingRowIndex; i < dataGridView.FirstDisplayedScrollingRowIndex + dataGridView.DisplayedRowCount(true); i++) {
        if (Content.RowNames.Count() != 0)
          dataGridView.Rows[i].HeaderCell.Value = Content.RowNames.ElementAt(i);
        else
          dataGridView.Rows[i].HeaderCell.Value = "Row " + (i + 1);
      }
      dataGridView.Invalidate();
    }

    private void Content_RowNamesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_RowNamesChanged), sender, e);
      else
        UpdateRowHeaders();
    }
    private void Content_ColumnNamesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ColumnNamesChanged), sender, e);
      else
        UpdateColumnHeaders();
    }
    private void Content_ItemChanged(object sender, EventArgs<int, int> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<int, int>>(Content_ItemChanged), sender, e);
      else
        dataGridView.InvalidateCell(e.Value2, e.Value);
    }
    private void Content_Reset(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Reset), sender, e);
      else
        UpdateData();
    }

    #region TextBox Events
    private void rowsTextBox_Validating(object sender, CancelEventArgs e) {
      int i = 0;
      if (!int.TryParse(rowsTextBox.Text, out i) || (i <= 0)) {
        e.Cancel = true;
        errorProvider.SetError(rowsTextBox, "Invalid Number of Rows (Valid values are positive integers larger than 0)");
        rowsTextBox.SelectAll();
      }
    }
    private void rowsTextBox_Validated(object sender, EventArgs e) {
      if (!Content.ReadOnly) Content.Rows = int.Parse(rowsTextBox.Text);
      errorProvider.SetError(rowsTextBox, string.Empty);
    }
    private void rowsTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        rowsLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        rowsTextBox.Text = Content.Rows.ToString();
        rowsLabel.Focus();  // set focus on label to validate data
      }
    }
    private void columnsTextBox_Validating(object sender, CancelEventArgs e) {
      int i = 0;
      if (!int.TryParse(columnsTextBox.Text, out i) || (i <= 0)) {
        e.Cancel = true;
        errorProvider.SetError(columnsTextBox, "Invalid Number of Columns (Valid values are positive integers larger than 0)");
        columnsTextBox.SelectAll();
      }
    }
    private void columnsTextBox_Validated(object sender, EventArgs e) {
      if (!Content.ReadOnly) Content.Columns = int.Parse(columnsTextBox.Text);
      errorProvider.SetError(columnsTextBox, string.Empty);
    }
    private void columnsTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        columnsLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        columnsTextBox.Text = Content.Columns.ToString();
        columnsLabel.Focus();  // set focus on label to validate data
      }
    }
    #endregion

    #region DataGridView Events
    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (!dataGridView.ReadOnly) {
        string errorMessage;
        if (!Content.Validate(e.FormattedValue.ToString(), out errorMessage)) {
          e.Cancel = true;
          dataGridView.Rows[e.RowIndex].ErrorText = errorMessage;
        }
      }
    }
    private void dataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e) {
      if (!dataGridView.ReadOnly) {
        string value = e.Value.ToString();
        int rowIndex = virtualRowIndizes[e.RowIndex];
        e.ParsingApplied = Content.SetValue(value, rowIndex, e.ColumnIndex);
        if (e.ParsingApplied) e.Value = Content.GetValue(rowIndex, e.ColumnIndex);
      }
    }
    private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
    }
    private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
      if (e.RowIndex < Content.Rows && e.ColumnIndex < Content.Columns) {
        int rowIndex = virtualRowIndizes[e.RowIndex];
        e.Value = Content.GetValue(rowIndex, e.ColumnIndex);
      }
    }
    private void dataGridView_Scroll(object sender, ScrollEventArgs e) {
      UpdateRowHeaders();
    }
    private void dataGridView_Resize(object sender, EventArgs e) {
      UpdateRowHeaders();
    }

    private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (Content != null) {
        if (e.Button == MouseButtons.Left && Content.SortableView) {
          bool addToSortedIndizes = (Control.ModifierKeys & Keys.Control) == Keys.Control;
          SortOrder newSortOrder = SortOrder.Ascending;
          if (sortedColumnIndizes.Any(x => x.Key == e.ColumnIndex)) {
            SortOrder oldSortOrder = sortedColumnIndizes.Where(x => x.Key == e.ColumnIndex).First().Value;
            int enumLength = Enum.GetValues(typeof(SortOrder)).Length;
            newSortOrder = oldSortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), ((((int)oldSortOrder) + 1) % enumLength).ToString());
          }

          if (!addToSortedIndizes)
            sortedColumnIndizes.Clear();

          if (sortedColumnIndizes.Any(x => x.Key == e.ColumnIndex)) {
            int sortedIndex = sortedColumnIndizes.FindIndex(x => x.Key == e.ColumnIndex);
            if (newSortOrder != SortOrder.None)
              sortedColumnIndizes[sortedIndex] = new KeyValuePair<int, SortOrder>(e.ColumnIndex, newSortOrder);
            else
              sortedColumnIndizes.RemoveAt(sortedIndex);
          } else
            if (newSortOrder != SortOrder.None)
              sortedColumnIndizes.Add(new KeyValuePair<int, SortOrder>(e.ColumnIndex, newSortOrder));
          Sort();
        } else if (e.Button == MouseButtons.Right) {
          if (Content.ColumnNames.Count() != 0)
            contextMenu.Show(MousePosition);
        }
      }
    }

    private void Sort() {
      int[] newSortedIndex = Enumerable.Range(0, Content.Rows).ToArray();
      if (sortedColumnIndizes.Count != 0) {
        rowComparer.SortedIndizes = sortedColumnIndizes;
        rowComparer.Matrix = Content;
        Array.Sort(newSortedIndex, rowComparer);
      }
      virtualRowIndizes = newSortedIndex;
      UpdateSortGlyph();
      dataGridView.Invalidate();
    }

    private void UpdateSortGlyph() {
      foreach (DataGridViewColumn col in this.dataGridView.Columns)
        col.HeaderCell.SortGlyphDirection = SortOrder.None;
      foreach (KeyValuePair<int, SortOrder> p in sortedColumnIndizes)
        this.dataGridView.Columns[p.Key].HeaderCell.SortGlyphDirection = p.Value;
    }
    #endregion

    public class RowComparer : IComparer<int> {
      public RowComparer() {
      }

      private List<KeyValuePair<int, SortOrder>> sortedIndizes;
      public IEnumerable<KeyValuePair<int, SortOrder>> SortedIndizes {
        get { return this.sortedIndizes; }
        set { sortedIndizes = new List<KeyValuePair<int, SortOrder>>(value); }
      }
      private IStringConvertibleMatrix matrix;
      public IStringConvertibleMatrix Matrix {
        get { return this.matrix; }
        set { this.matrix = value; }
      }

      public int Compare(int x, int y) {
        int result = 0;
        double double1, double2;
        DateTime dateTime1, dateTime2;
        string string1, string2;

        if (matrix == null)
          throw new InvalidOperationException("Could not sort IStringConvertibleMatrix if the matrix member is null.");
        if (sortedIndizes == null)
          return 0;

        foreach (KeyValuePair<int, SortOrder> pair in sortedIndizes.Where(p => p.Value != SortOrder.None)) {
          string1 = matrix.GetValue(x, pair.Key);
          string2 = matrix.GetValue(y, pair.Key);
          if (double.TryParse(string1, out double1) && double.TryParse(string2, out double2))
            result = double1.CompareTo(double2);
          else if (DateTime.TryParse(string1, out dateTime1) && DateTime.TryParse(string2, out dateTime2))
            result = dateTime1.CompareTo(dateTime2);
          else {
            if (string1 != null)
              result = string1.CompareTo(string2);
            else if (string2 != null)
              result = string2.CompareTo(string1) * -1;
          }
          if (pair.Value == SortOrder.Descending)
            result *= -1;
          if (result != 0)
            return result;
        }
        return result;
      }
    }

    private void ShowHideColumns_Click(object sender, EventArgs e) {
      new ColumnsVisibilityDialog(this.dataGridView.Columns.Cast<DataGridViewColumn>()).ShowDialog();
    }
  }
}
