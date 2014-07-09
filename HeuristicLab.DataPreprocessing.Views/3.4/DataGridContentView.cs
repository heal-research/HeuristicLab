#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.Data.Views;
using HeuristicLab.DataPreprocessing.Filter;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Data Grid Content View")]
  [Content(typeof(IDataGridContent), true)]
  public partial class DataGridContentView : StringConvertibleMatrixView {

    private bool isSearching = false;
    private bool updateOnMouseUp = false;
    private SearchAndReplaceDialog findAndReplaceDialog;
    private IFindPreprocessingItemsIterator searchIterator;
    private string currentSearchText;
    private ComparisonOperation currentComparisonOperation;
    private Tuple<int, int> currentCell;

    public new IDataGridContent Content {
      get { return (IDataGridContent)base.Content; }
      set { base.Content = value; }
    }

    private IDictionary<int, IList<int>> _highlightedCellsBackground;
    public IDictionary<int, IList<int>> HightlightedCellsBackground {
      get { return _highlightedCellsBackground; }
      set {
        _highlightedCellsBackground = value;
        Refresh();
      }
    }

    public DataGridContentView() {
      InitializeComponent();
      dataGridView.CellMouseClick += dataGridView_CellMouseClick;
      dataGridView.KeyDown += dataGridView_KeyDown;
      dataGridView.MouseUp += dataGridView_MouseUp;
      dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
      contextMenuCell.Items.Add(ShowHideColumns);
      _highlightedCellsBackground = new Dictionary<int, IList<int>>();
      currentCell = null;
    }

    protected override void OnContentChanged() {
      List<KeyValuePair<int, SortOrder>> order = new List<KeyValuePair<int, SortOrder>>(base.sortedColumnIndices);
      base.OnContentChanged();

      DataGridView.RowHeadersWidth = 70;

      if (Content == null && findAndReplaceDialog != null) {
        findAndReplaceDialog.Close();
      }

      if (Content != null) {
        base.sortedColumnIndices = order;
        base.Sort();
      }

    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += Content_Changed;
      Content.FilterLogic.FilterChanged += FilterLogic_FilterChanged;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Changed -= Content_Changed;
      Content.FilterLogic.FilterChanged -= FilterLogic_FilterChanged;
    }

    private void FilterLogic_FilterChanged(object sender, EventArgs e) {
      OnContentChanged();
      searchIterator = null;
      if (findAndReplaceDialog != null && !findAndReplaceDialog.IsDisposed) {
        if (Content.FilterLogic.IsFiltered) {
          findAndReplaceDialog.DisableReplace();
        } else {
          findAndReplaceDialog.EnableReplace();
        }
      }
      btnReplace.Enabled = !Content.FilterLogic.IsFiltered;
    }

    private void Content_Changed(object sender, DataPreprocessingChangedEventArgs e) {
      OnContentChanged();
      searchIterator = null;
    }

    protected override void dataGridView_SelectionChanged(object sender, EventArgs e) {
      base.dataGridView_SelectionChanged(sender, e);
      if (Content != null && dataGridView.RowCount != 0 && dataGridView.ColumnCount != 0)
        Content.Selection = GetSelectedCells();
    }

    //couldn't use base.dataGridView_CellValidating as the values have to be validated per column individually
    protected override void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (dataGridView.ReadOnly) return;
      if (Content == null) return;
      if (Content.Rows == 0 || Content.Columns == 0) return;

      string errorMessage;
      if (Content != null) {
        if (dataGridView.IsCurrentCellInEditMode && Content.FilterLogic.IsFiltered) {
          errorMessage = "A filter is active, you cannot modify data. Press ESC to exit edit mode.";
        } else {
          Content.Validate(e.FormattedValue.ToString(), out errorMessage, e.ColumnIndex);
        }

        if (!String.IsNullOrEmpty(errorMessage)) {
          e.Cancel = true;
          dataGridView.Rows[e.RowIndex].ErrorText = errorMessage;
        }

      }
    }


    //protected override void PasteValuesToDataGridView() {
    //  base.PasteValuesToDataGridView();
    //  dataGridView.Refresh();
    //}

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      rowsTextBox.ReadOnly = true;
      columnsTextBox.ReadOnly = true;
    }

    protected override int[] Sort(IEnumerable<KeyValuePair<int, SortOrder>> sortedColumns) {
      btnApplySort.Enabled = sortedColumns.Any();
      return base.Sort(sortedColumns);
    }

    protected override void ClearSorting() {
      btnApplySort.Enabled = false;
      base.ClearSorting();
    }

    private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
      searchIterator = null;
    }

    private void dataGridView_MouseUp(object sender, MouseEventArgs e) {
      if (!updateOnMouseUp)
        return;

      updateOnMouseUp = false;
      dataGridView_SelectionChanged(sender, e);
    }

    private void btnApplySort_Click(object sender, System.EventArgs e) {
      Content.ManipulationLogic.ReOrderToIndices(virtualRowIndices);
      OnContentChanged();
    }

    #region FindAndReplaceDialog

    private void CreateFindAndReplaceDialog() {
      if (findAndReplaceDialog == null || findAndReplaceDialog.IsDisposed) {
        findAndReplaceDialog = new SearchAndReplaceDialog();
        findAndReplaceDialog.Show(this);
        if (AreMultipleCellsSelected()) {
          ResetHighlightedCellsBackground();
          HightlightedCellsBackground = GetSelectedCells();
          dataGridView.ClearSelection();
        }
        findAndReplaceDialog.FindAllEvent += findAndReplaceDialog_FindAllEvent;
        findAndReplaceDialog.FindNextEvent += findAndReplaceDialog_FindNextEvent;
        findAndReplaceDialog.ReplaceAllEvent += findAndReplaceDialog_ReplaceAllEvent;
        findAndReplaceDialog.ReplaceNextEvent += findAndReplaceDialog_ReplaceEvent;
        findAndReplaceDialog.FormClosing += findAndReplaceDialog_FormClosing;
        searchIterator = null;
        DataGridView.SelectionChanged += DataGridView_SelectionChanged_FindAndReplace;
        if (Content.FilterLogic.IsFiltered) {
          findAndReplaceDialog.DisableReplace();
        }
      }
    }

    private void DataGridView_SelectionChanged_FindAndReplace(object sender, EventArgs e) {
      if (Content != null) {
        if (!isSearching && AreMultipleCellsSelected()) {
          ResetHighlightedCellsBackground();
          HightlightedCellsBackground = GetSelectedCells();
          searchIterator = null;
        }
      }
    }

    void findAndReplaceDialog_FormClosing(object sender, FormClosingEventArgs e) {
      ResetHighlightedCellsBackground();
      searchIterator = null;
      DataGridView.SelectionChanged -= DataGridView_SelectionChanged_FindAndReplace;
    }

    void findAndReplaceDialog_ReplaceEvent(object sender, EventArgs e) {
      if (searchIterator != null && searchIterator.GetCurrent() != null) {
        Replace(TransformToDictionary(currentCell));
      }
    }

    void findAndReplaceDialog_ReplaceAllEvent(object sender, EventArgs e) {
      Replace(FindAll(findAndReplaceDialog.GetSearchText()));
    }

    void findAndReplaceDialog_FindNextEvent(object sender, EventArgs e) {
      if (searchIterator == null
        || currentSearchText != findAndReplaceDialog.GetSearchText()
        || currentComparisonOperation != findAndReplaceDialog.GetComparisonOperation()) {

        searchIterator = new FindPreprocessingItemsIterator(FindAll(findAndReplaceDialog.GetSearchText()));
        currentSearchText = findAndReplaceDialog.GetSearchText();
        currentComparisonOperation = findAndReplaceDialog.GetComparisonOperation();
      }

      if (IsOneCellSelected()) {
        var first = GetSelectedCells().First();
        searchIterator.SetStartCell(first.Key, first.Value[0]);
      }

      bool moreOccurences = false;
      currentCell = searchIterator.GetCurrent();
      moreOccurences = searchIterator.MoveNext();
      if (IsOneCellSelected() && currentCell != null) {
        var first = GetSelectedCells().First();
        if (currentCell.Item1 == first.Key && currentCell.Item2 == first.Value[0]) {
          if (!moreOccurences) {
            searchIterator.Reset();
          }
          currentCell = searchIterator.GetCurrent();
          moreOccurences = searchIterator.MoveNext();
          if (!moreOccurences) {
            searchIterator.Reset();
          }
        }
      }

      dataGridView.ClearSelection();

      if (currentCell != null) {
        dataGridView[currentCell.Item1, currentCell.Item2].Selected = true;
        dataGridView.CurrentCell = dataGridView[currentCell.Item1, currentCell.Item2];
      }
    }

    private bool AreMultipleCellsSelected() {
      return GetSelectedCellCount() > 1;
    }

    private bool IsOneCellSelected() {
      return GetSelectedCellCount() == 1;
    }

    private int GetSelectedCellCount() {
      int count = 0;
      foreach (var column in GetSelectedCells()) {
        count += column.Value.Count();
      }
      return count;
    }

    void findAndReplaceDialog_FindAllEvent(object sender, EventArgs e) {
      dataGridView.ClearSelection();
      isSearching = true;
      SuspendRepaint();
      var selectedCells = FindAll(findAndReplaceDialog.GetSearchText());
      foreach (var column in selectedCells) {
        foreach (var cell in column.Value) {
          dataGridView[column.Key, cell].Selected = true;
        }
      }
      ResumeRepaint(true);
      isSearching = false;
      Content.Selection = selectedCells;
      //update statistic in base
      base.dataGridView_SelectionChanged(sender, e);
    }

    private Core.ConstraintOperation GetConstraintOperation(ComparisonOperation comparisonOperation) {
      Core.ConstraintOperation constraintOperation = Core.ConstraintOperation.Equal;
      switch (comparisonOperation) {
        case ComparisonOperation.Equal:
          constraintOperation = Core.ConstraintOperation.Equal;
          break;
        case ComparisonOperation.Greater:
          constraintOperation = Core.ConstraintOperation.Greater;
          break;
        case ComparisonOperation.GreaterOrEqual:
          constraintOperation = Core.ConstraintOperation.GreaterOrEqual;
          break;
        case ComparisonOperation.Less:
          constraintOperation = Core.ConstraintOperation.Less;
          break;
        case ComparisonOperation.LessOrEqual:
          constraintOperation = Core.ConstraintOperation.LessOrEqual;
          break;
        case ComparisonOperation.NotEqual:
          constraintOperation = Core.ConstraintOperation.NotEqual;
          break;
      }
      return constraintOperation;
    }

    private IDictionary<int, IList<int>> FindAll(string match) {
      bool searchInSelection = HightlightedCellsBackground.Values.Sum(list => list.Count) > 1;
      ComparisonOperation comparisonOperation = findAndReplaceDialog.GetComparisonOperation();
      var foundCells = new Dictionary<int, IList<int>>();
      for (int i = 0; i < Content.FilterLogic.PreprocessingData.Columns; i++) {
        var filters = CreateFilters(match, comparisonOperation, i);

        bool[] filteredRows = Content.FilterLogic.GetFilterResult(filters, true);
        var foundIndices = new List<int>();
        for (int idx = 0; idx < filteredRows.Length; ++idx) {
          var notFilteredThusFound = !filteredRows[idx];
          if (notFilteredThusFound) {
            foundIndices.Add(idx);
          }
        }
        foundCells[i] = foundIndices;
        IList<int> selectedList;
        if (searchInSelection && HightlightedCellsBackground.TryGetValue(i, out selectedList)) {
          foundCells[i] = foundCells[i].Intersect(selectedList).ToList<int>();
        } else if (searchInSelection) {
          foundCells[i].Clear();
        }
      }
      return MapToSorting(foundCells);
    }

    private List<IFilter> CreateFilters(string match, ComparisonOperation comparisonOperation, int columnIndex) {
      IPreprocessingData preprocessingData = Content.FilterLogic.PreprocessingData;
      IStringConvertibleValue value;
      if (preprocessingData.VariableHasType<double>(columnIndex)) {
        value = new DoubleValue();
      } else if (preprocessingData.VariableHasType<String>(columnIndex)) {
        value = new StringValue();
      } else if (preprocessingData.VariableHasType<DateTime>(columnIndex)) {
        value = new DateTimeValue();
      } else {
        throw new ArgumentException("unsupported type");
      }
      value.SetValue(match);
      var comparisonFilter = new ComparisonFilter(preprocessingData, GetConstraintOperation(comparisonOperation), value, true);
      comparisonFilter.ConstraintColumn = columnIndex;
      return new List<Filter.IFilter>() { comparisonFilter };
    }

    private IDictionary<int, IList<int>> MapToSorting(Dictionary<int, IList<int>> foundCells) {
      if (sortedColumnIndices.Count == 0) {
        return foundCells;
      } else {
        var sortedFoundCells = new Dictionary<int, IList<int>>();

        var indicesToVirtual = new Dictionary<int, int>();
        for (int i = 0; i < virtualRowIndices.Length; ++i) {
          indicesToVirtual.Add(virtualRowIndices[i], i);
        }

        foreach (var entry in foundCells) {
          var cells = new List<int>();
          foreach (var cell in entry.Value) {
            cells.Add(indicesToVirtual[cell]);
          }
          cells.Sort();
          sortedFoundCells.Add(entry.Key, cells);
        }
        return sortedFoundCells;
      }
    }

    private void Replace(IDictionary<int, IList<int>> cells) {
      if (findAndReplaceDialog != null) {
        ReplaceTransaction(() => {
          switch (findAndReplaceDialog.GetReplaceAction()) {
            case ReplaceAction.Value:
              Content.ManipulationLogic.ReplaceIndicesByValue(cells, findAndReplaceDialog.GetReplaceText());
              break;
            case ReplaceAction.Average:
              Content.ManipulationLogic.ReplaceIndicesByAverageValue(cells, false);
              break;
            case ReplaceAction.Median:
              Content.ManipulationLogic.ReplaceIndicesByMedianValue(cells, false);
              break;
            case ReplaceAction.Random:
              Content.ManipulationLogic.ReplaceIndicesByRandomValue(cells, false);
              break;
            case ReplaceAction.MostCommon:
              Content.ManipulationLogic.ReplaceIndicesByMostCommonValue(cells, false);
              break;
            case ReplaceAction.Interpolation:
              Content.ManipulationLogic.ReplaceIndicesByLinearInterpolationOfNeighbours(cells);
              break;
          }
        });
      }
    }

    private IDictionary<int, IList<int>> TransformToDictionary(Tuple<int, int> tuple) {
      var highlightCells = new Dictionary<int, IList<int>>();
      highlightCells.Add(tuple.Item1, new List<int>() { tuple.Item2 });
      return highlightCells;
    }

    private void ResetHighlightedCellsBackground() {
      HightlightedCellsBackground = new Dictionary<int, IList<int>>();
    }

    #endregion FindAndReplaceDialog

    private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (Content == null) return;
      if (e.Button == System.Windows.Forms.MouseButtons.Right) {
        if (e.ColumnIndex == -1 || e.RowIndex == -1) {
          replaceValueOverColumnToolStripMenuItem.Visible = false;
          contextMenuCell.Show(MousePosition);
        } else {
          if (!dataGridView.SelectedCells.Contains(dataGridView[e.ColumnIndex, e.RowIndex])) {
            dataGridView.ClearSelection();
            dataGridView[e.ColumnIndex, e.RowIndex].Selected = true;
          }

          var columnIndices = new HashSet<int>();
          for (int i = 0; i < dataGridView.SelectedCells.Count; i++) {
            columnIndices.Add(dataGridView.SelectedCells[i].ColumnIndex);
          }

          replaceValueOverSelectionToolStripMenuItem.Enabled = AreMultipleCellsSelected();

          averageToolStripMenuItem_Column.Enabled =
            averageToolStripMenuItem_Selection.Enabled =
            medianToolStripMenuItem_Column.Enabled =
            medianToolStripMenuItem_Selection.Enabled =
            randomToolStripMenuItem_Column.Enabled =
            randomToolStripMenuItem_Selection.Enabled = !Content.PreProcessingData.AreAllStringColumns(columnIndices);

          smoothingToolStripMenuItem_Column.Enabled =
            interpolationToolStripMenuItem_Column.Enabled = !dataGridView.SelectedCells.Contains(dataGridView[e.ColumnIndex, 0])
            && !dataGridView.SelectedCells.Contains(dataGridView[e.ColumnIndex, Content.Rows - 1])
            && !Content.PreProcessingData.AreAllStringColumns(columnIndices);

          replaceValueOverColumnToolStripMenuItem.Visible = true;
          contextMenuCell.Show(MousePosition);
        }
      }
    }

    private void dataGridView_KeyDown(object sender, KeyEventArgs e) {
      var selectedRows = dataGridView.SelectedRows;
      if (e.KeyCode == Keys.Delete && selectedRows.Count > 0) {
        List<int> rows = new List<int>();
        for (int i = 0; i < selectedRows.Count; ++i) {
          rows.Add(selectedRows[i].Index);
        }
        Content.DeleteRow(rows);
      } else if (e.Control && e.KeyCode == Keys.F) {
        CreateFindAndReplaceDialog();
        findAndReplaceDialog.ActivateSearch();
      } else if (e.Control && e.KeyCode == Keys.R) {
        CreateFindAndReplaceDialog();
        findAndReplaceDialog.ActivateReplace();
      }
    }

    private IDictionary<int, IList<int>> GetSelectedCells() {
      IDictionary<int, IList<int>> selectedCells = new Dictionary<int, IList<int>>();

      //special case if all cells are selected
      if (dataGridView.AreAllCellsSelected(true)) {
        for (int i = 0; i < Content.Columns; i++)
          selectedCells[i] = Enumerable.Range(0, Content.Rows).ToList();
        return selectedCells;
      }

      foreach (var selectedCell in dataGridView.SelectedCells) {
        var cell = (DataGridViewCell)selectedCell;
        if (!selectedCells.ContainsKey(cell.ColumnIndex))
          selectedCells.Add(cell.ColumnIndex, new List<int>(1024));
        selectedCells[cell.ColumnIndex].Add(cell.RowIndex);
      }

      return selectedCells;
    }

    private void StartReplacing() {
      SuspendRepaint();
    }

    private void StopReplacing() {
      ResumeRepaint(true);
    }

    private void ReplaceTransaction(Action action) {
      StartReplacing();
      action();
      StopReplacing();
    }

    private void btnSearch_Click(object sender, EventArgs e) {
      CreateFindAndReplaceDialog();
      findAndReplaceDialog.ActivateSearch();
    }

    private void btnReplace_Click(object sender, EventArgs e) {
      CreateFindAndReplaceDialog();
      findAndReplaceDialog.ActivateReplace();
    }

    #region ContextMenu Events

    private void ReplaceWithAverage_Column_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesByAverageValue(GetSelectedCells(), false);
      });
    }
    private void ReplaceWithAverage_Selection_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesByAverageValue(GetSelectedCells(), true);
      });
    }

    private void ReplaceWithMedian_Column_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesByMedianValue(GetSelectedCells(), false);
      });
    }
    private void ReplaceWithMedian_Selection_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesByMedianValue(GetSelectedCells(), true);
      });
    }

    private void ReplaceWithRandom_Column_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesByRandomValue(GetSelectedCells(), false);
      });
    }
    private void ReplaceWithRandom_Selection_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesByRandomValue(GetSelectedCells(), true);
      });
    }

    private void ReplaceWithMostCommon_Column_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesByMostCommonValue(GetSelectedCells(), false);
      });
    }
    private void ReplaceWithMostCommon_Selection_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesByMostCommonValue(GetSelectedCells(), true);
      });
    }

    private void ReplaceWithInterpolation_Column_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesByLinearInterpolationOfNeighbours(GetSelectedCells());
      });
    }

    private void ReplaceWithSmoothing_Selection_Click(object sender, EventArgs e) {
      ReplaceTransaction(() => {
        Content.ManipulationLogic.ReplaceIndicesBySmoothing(GetSelectedCells());
      });
    }
    #endregion

  }
}
