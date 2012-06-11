#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
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

    public DataGridView DataGridView {
      get { return dataGridView; }
    }

    public override bool ReadOnly {
      get {
        if ((Content != null) && Content.ReadOnly) return true;
        return base.ReadOnly;
      }
      set { base.ReadOnly = value; }
    }

    private bool showRowsAndColumnsTextBox;
    public bool ShowRowsAndColumnsTextBox {
      get { return showRowsAndColumnsTextBox; }
      set {
        showRowsAndColumnsTextBox = value;
        UpdateVisibilityOfTextBoxes();
      }
    }

    private bool showStatisticalInformation;
    public bool ShowStatisticalInformation {
      get { return showStatisticalInformation; }
      set {
        showStatisticalInformation = value;
        UpdateVisibilityOfStatisticalInformation();
      }
    }

    public StringConvertibleMatrixView() {
      InitializeComponent();
      ShowRowsAndColumnsTextBox = true;
      ShowStatisticalInformation = true;
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
      rowsTextBox.Text = Content.Rows.ToString();
      rowsTextBox.Enabled = true;
      columnsTextBox.Text = Content.Columns.ToString();
      columnsTextBox.Enabled = true;
      virtualRowIndizes = Enumerable.Range(0, Content.Rows).ToArray();

      if (Content.Columns == 0 && dataGridView.ColumnCount != Content.Columns && !Content.ReadOnly)
        Content.Columns = dataGridView.ColumnCount;
      else {
        DataGridViewColumn[] columns = new DataGridViewColumn[Content.Columns];
        for (int i = 0; i < columns.Length; ++i) {
          var column = new DataGridViewTextBoxColumn();
          column.FillWeight = 1;
          columns[i] = column;
        }
        dataGridView.Columns.Clear();
        dataGridView.Columns.AddRange(columns);
      }

      //DataGridViews with rows but no columns are not allowed !
      if (Content.Rows == 0 && dataGridView.RowCount != Content.Rows && !Content.ReadOnly)
        Content.Rows = dataGridView.RowCount;
      else
        dataGridView.RowCount = Content.Rows;


      ClearSorting();
      UpdateColumnHeaders();
      UpdateRowHeaders();

      dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
      dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
      dataGridView.Enabled = true;
    }

    protected virtual void UpdateColumnHeaders() {
      HashSet<string> invisibleColumnNames = new HashSet<string>(dataGridView.Columns.OfType<DataGridViewColumn>()
      .Where(c => !c.Visible && !string.IsNullOrEmpty(c.HeaderText)).Select(c => c.HeaderText));

      for (int i = 0; i < dataGridView.ColumnCount; i++) {
        if (i < Content.ColumnNames.Count())
          dataGridView.Columns[i].HeaderText = Content.ColumnNames.ElementAt(i);
        else
          dataGridView.Columns[i].HeaderText = "Column " + (i + 1);
        dataGridView.Columns[i].Visible = !invisibleColumnNames.Contains(dataGridView.Columns[i].HeaderText);
      }
    }
    protected void UpdateRowHeaders() {
      int index = dataGridView.FirstDisplayedScrollingRowIndex;
      if (index == -1) index = 0;
      int updatedRows = 0;
      int count = dataGridView.DisplayedRowCount(true);

      while (updatedRows < count) {
        if (virtualRowIndizes[index] < Content.RowNames.Count())
          dataGridView.Rows[index].HeaderCell.Value = Content.RowNames.ElementAt(virtualRowIndizes[index]);
        else
          dataGridView.Rows[index].HeaderCell.Value = "Row " + (index + 1);
        if (dataGridView.Rows[index].Visible)
          updatedRows++;
        index++;
      }
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

    private void dataGridView_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
      this.UpdateRowHeaders();
    }
    private void dataGridView_Resize(object sender, EventArgs e) {
      this.UpdateRowHeaders();
    }

    private void dataGridView_KeyDown(object sender, KeyEventArgs e) {
      if (!ReadOnly && e.Control && e.KeyCode == Keys.V)
        PasteValuesToDataGridView();
      else if (e.Control && e.KeyCode == Keys.C)
        CopyValuesFromDataGridView();
    }

    private void CopyValuesFromDataGridView() {
      if (dataGridView.SelectedCells.Count == 0) return;
      StringBuilder s = new StringBuilder();
      int minRowIndex = dataGridView.SelectedCells[0].RowIndex;
      int maxRowIndex = dataGridView.SelectedCells[dataGridView.SelectedCells.Count - 1].RowIndex;
      int minColIndex = dataGridView.SelectedCells[0].ColumnIndex;
      int maxColIndex = dataGridView.SelectedCells[dataGridView.SelectedCells.Count - 1].ColumnIndex;

      if (minRowIndex > maxRowIndex) {
        int temp = minRowIndex;
        minRowIndex = maxRowIndex;
        maxRowIndex = temp;
      }
      if (minColIndex > maxColIndex) {
        int temp = minColIndex;
        minColIndex = maxColIndex;
        maxColIndex = temp;
      }

      bool addRowNames = dataGridView.AreAllCellsSelected(false) && Content.RowNames.Count() > 0;
      bool addColumnNames = dataGridView.AreAllCellsSelected(false) && Content.ColumnNames.Count() > 0;

      //add colum names
      if (addColumnNames) {
        if (addRowNames)
          s.Append('\t');

        DataGridViewColumn column = dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        while (column != null) {
          s.Append(column.HeaderText);
          s.Append('\t');
          column = dataGridView.Columns.GetNextColumn(column, DataGridViewElementStates.Visible, DataGridViewElementStates.None);
        }
        s.Remove(s.Length - 1, 1); //remove last tab
        s.Append(Environment.NewLine);
      }

      for (int i = minRowIndex; i <= maxRowIndex; i++) {
        int rowIndex = this.virtualRowIndizes[i];
        if (addRowNames) {
          s.Append(Content.RowNames.ElementAt(rowIndex));
          s.Append('\t');
        }

        DataGridViewColumn column = dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        while (column != null) {
          DataGridViewCell cell = dataGridView[column.Index, i];
          if (cell.Selected) {
            s.Append(Content.GetValue(rowIndex, column.Index));
            s.Append('\t');
          }

          column = dataGridView.Columns.GetNextColumn(column, DataGridViewElementStates.Visible, DataGridViewElementStates.None);
        }
        s.Remove(s.Length - 1, 1); //remove last tab
        s.Append(Environment.NewLine);
      }
      Clipboard.SetText(s.ToString());
    }

    private void PasteValuesToDataGridView() {
      string[,] values = SplitClipboardString(Clipboard.GetText());
      int rowIndex = 0;
      int columnIndex = 0;
      if (dataGridView.CurrentCell != null) {
        rowIndex = dataGridView.CurrentCell.RowIndex;
        columnIndex = dataGridView.CurrentCell.ColumnIndex;
      }
      if (Content.Rows < values.GetLength(1) + rowIndex) Content.Rows = values.GetLength(1) + rowIndex;
      if (Content.Columns < values.GetLength(0) + columnIndex) Content.Columns = values.GetLength(0) + columnIndex;

      for (int row = 0; row < values.GetLength(1); row++) {
        for (int col = 0; col < values.GetLength(0); col++) {
          Content.SetValue(values[col, row], row + rowIndex, col + columnIndex);
        }
      }
      ClearSorting();
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
        }
      }
    }

    protected virtual void ClearSorting() {
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

    private void dataGridView_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
      if (Content == null) return;
      if (e.Button == MouseButtons.Right && Content.ColumnNames.Count() != 0)
        contextMenu.Show(MousePosition);
    }
    private void ShowHideColumns_Click(object sender, EventArgs e) {
      new ColumnsVisibilityDialog(this.dataGridView.Columns.Cast<DataGridViewColumn>()).ShowDialog();
    }

    private void UpdateVisibilityOfTextBoxes() {
      rowsTextBox.Visible = columnsTextBox.Visible = showRowsAndColumnsTextBox;
      rowsLabel.Visible = columnsLabel.Visible = showRowsAndColumnsTextBox;
      UpdateDataGridViewSizeAndLocation();
    }

    private void UpdateVisibilityOfStatisticalInformation() {
      statisticsTextBox.Visible = showStatisticalInformation;
      UpdateDataGridViewSizeAndLocation();
    }

    private void UpdateDataGridViewSizeAndLocation() {
      int headerSize = columnsTextBox.Location.Y + columnsTextBox.Size.Height +
       columnsTextBox.Margin.Bottom + dataGridView.Margin.Top;

      int offset = showRowsAndColumnsTextBox ? headerSize : 0;
      dataGridView.Location = new Point(0, offset);

      int statisticsTextBoxHeight = showStatisticalInformation ? statisticsTextBox.Height + statisticsTextBox.Margin.Top + statisticsTextBox.Margin.Bottom : 0;
      dataGridView.Size = new Size(Size.Width, Size.Height - offset - statisticsTextBoxHeight);
    }

    private void dataGridView_SelectionChanged(object sender, EventArgs e) {
      string stringFormat = "{0,20:0.0000}";
      statisticsTextBox.Text = string.Empty;
      if (dataGridView.SelectedCells.Count > 1) {
        List<double> selectedValues = new List<double>();
        foreach (DataGridViewCell cell in dataGridView.SelectedCells) {
          double value;
          if (!double.TryParse(cell.Value.ToString(), out value)) return;
          selectedValues.Add(value);
        }
        if (selectedValues.Count > 1) {
          StringBuilder labelText = new StringBuilder();
          labelText.Append("Count: " + string.Format(stringFormat, selectedValues.Count) + "    ");
          labelText.Append("Sum: " + string.Format(stringFormat, selectedValues.Sum()) + "    ");
          labelText.Append("Min: " + string.Format(stringFormat, selectedValues.Min()) + "    ");
          labelText.Append("Max: " + string.Format(stringFormat, selectedValues.Max()) + "    ");
          labelText.Append("Average: " + string.Format(stringFormat, selectedValues.Average()) + "    ");
          labelText.Append("Standard Deviation: " + string.Format(stringFormat, selectedValues.StandardDeviation()) + "    ");

          statisticsTextBox.Text = labelText.ToString();
        }
      }
    }
  }
}
