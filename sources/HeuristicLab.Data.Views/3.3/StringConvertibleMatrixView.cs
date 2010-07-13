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
    protected int[] virtualRowIndizes;
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
      errorProvider.SetIconAlignment(rowsTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(rowsTextBox, 2);
      errorProvider.SetIconAlignment(columnsTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(columnsTextBox, 2);
      sortedColumnIndizes = new List<KeyValuePair<int, SortOrder>>();
      rowComparer = new RowComparer();
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
        rowsTextBox.Text = "";
        columnsTextBox.Text = "";
        dataGridView.Rows.Clear();
        dataGridView.Columns.Clear();
        virtualRowIndizes = new int[0];
      } else
        UpdateData();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      rowsTextBox.Enabled = Content != null;
      columnsTextBox.Enabled = Content != null;
      dataGridView.Enabled = Content != null;
      rowsTextBox.ReadOnly = ReadOnly;
      columnsTextBox.ReadOnly = ReadOnly;
      dataGridView.ReadOnly = ReadOnly;
    }

    private void UpdateData() {
      ClearSorting();
      rowsTextBox.Text = Content.Rows.ToString();
      rowsTextBox.Enabled = true;
      columnsTextBox.Text = Content.Columns.ToString();
      columnsTextBox.Enabled = true;
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
      dataGridView.Enabled = true;
    }

    private void UpdateColumnHeaders() {
      int firstDisplayedColumnIndex = this.dataGridView.FirstDisplayedScrollingColumnIndex;
      if (firstDisplayedColumnIndex == -1)
        firstDisplayedColumnIndex = 0;
      int lastDisplayedColumnIndex = firstDisplayedColumnIndex + dataGridView.DisplayedColumnCount(true);
      for (int i = firstDisplayedColumnIndex; i < lastDisplayedColumnIndex; i++) {
        if (Content.ColumnNames.Count() != 0)
          dataGridView.Columns[i].HeaderText = Content.ColumnNames.ElementAt(i);
        else
          dataGridView.Columns[i].HeaderText = "Column " + (i + 1);
      }
      dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
    }

    private void UpdateRowHeaders() {
      int firstDisplayedRowIndex = dataGridView.FirstDisplayedScrollingRowIndex;
      if (firstDisplayedRowIndex == -1)
        firstDisplayedRowIndex = 0;
      int lastDisplaydRowIndex = firstDisplayedRowIndex + dataGridView.DisplayedRowCount(true);
      for (int i = firstDisplayedRowIndex; i < lastDisplaydRowIndex; i++) {
        if (Content.RowNames.Count() != 0)
          dataGridView.Rows[i].HeaderCell.Value = Content.RowNames.ElementAt(virtualRowIndizes[i]);
        else
          dataGridView.Rows[i].HeaderCell.Value = "Row " + (i + 1);
      }
      dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
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
      if (ReadOnly || Locked)
        return;
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
      if (ReadOnly || Locked)
        return;
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
        if (Content != null && !Content.Validate(e.FormattedValue.ToString(), out errorMessage)) {
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
      if (Content != null && e.RowIndex < Content.Rows && e.ColumnIndex < Content.Columns) {
        int rowIndex = virtualRowIndizes[e.RowIndex];
        e.Value = Content.GetValue(rowIndex, e.ColumnIndex);
      }
    }
    private void dataGridView_Scroll(object sender, ScrollEventArgs e) {
      UpdateRowHeaders();
      UpdateColumnHeaders();
    }
    private void dataGridView_Resize(object sender, EventArgs e) {
      UpdateRowHeaders();
      UpdateColumnHeaders();
    }

    private void dataGridView_KeyDown(object sender, KeyEventArgs e) {
      if (!ReadOnly && e.Control && e.KeyCode == Keys.V) { //shortcut for values paste
        string[,] values = SplitClipboardString(Clipboard.GetText());

        int rowIndex = 0;
        int columnIndex = 0;
        if (dataGridView.CurrentCell != null) {
          rowIndex = dataGridView.CurrentCell.RowIndex;
          columnIndex = dataGridView.CurrentCell.ColumnIndex;
        }

        for (int row = 0; row < values.GetLength(1); row++) {
          if (row + rowIndex >= Content.Rows)
            Content.Rows = Content.Rows + 1;
          for (int col = 0; col < values.GetLength(0); col++) {
            if (col + columnIndex >= Content.Columns)
              Content.Columns = Content.Columns + 1;
            Content.SetValue(values[col, row], row + rowIndex, col + columnIndex);
          }
        }

        ClearSorting();
      }
    }

    private string[,] SplitClipboardString(string clipboardText) {
      clipboardText = clipboardText.Remove(clipboardText.Length - Environment.NewLine.Length);  //remove last newline constant
      string[,] values = null;
      string[] lines = clipboardText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
      string[] cells;
      for (int i = 0; i < lines.Length; i++) {
        cells = lines[i].Split('\t');
        if (values == null)
          values = new string[cells.Length, lines.Length];
        for (int j = 0; j < cells.Length; j++)
          values[j, i] = string.IsNullOrEmpty(cells[j]) ? string.Empty : cells[j];
      }
      return values;
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

    protected void ClearSorting() {
      virtualRowIndizes = Enumerable.Range(0, Content.Rows).ToArray();
      sortedColumnIndizes.Clear();
      UpdateSortGlyph();
    }

    private void Sort() {
      virtualRowIndizes = Sort(sortedColumnIndizes);
      UpdateSortGlyph();
      UpdateRowHeaders();
      dataGridView.Invalidate();
    }
    protected virtual int[] Sort(IEnumerable<KeyValuePair<int, SortOrder>> sortedColumns) {
      int[] newSortedIndex = Enumerable.Range(0, Content.Rows).ToArray();
      if (sortedColumns.Count() != 0) {
        rowComparer.SortedIndizes = sortedColumns;
        rowComparer.Matrix = Content;
        Array.Sort(newSortedIndex, rowComparer);
      }
      return newSortedIndex;
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
        TimeSpan timeSpan1, timeSpan2;
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
          else if (TimeSpan.TryParse(string1, out timeSpan1) && TimeSpan.TryParse(string2, out timeSpan2))
            result = timeSpan1.CompareTo(timeSpan2);
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
