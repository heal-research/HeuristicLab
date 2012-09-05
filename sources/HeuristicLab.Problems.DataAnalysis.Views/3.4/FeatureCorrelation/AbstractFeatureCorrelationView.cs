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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using FCE = HeuristicLab.Problems.DataAnalysis.FeatureCorrelationEnums;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Feature Correlation View")]
  [Content(typeof(DataAnalysisProblemData), false)]
  public abstract partial class AbstractFeatureCorrelationView : AsynchronousContentView {

    private int[] virtualRowIndices;
    private List<KeyValuePair<int, SortOrder>> sortedColumnIndices;
    private StringConvertibleMatrixView.RowComparer rowComparer;

    protected FeatureCorrelationCalculator fcc;
    protected HeatMap currentCorrelation;

    public new DataAnalysisProblemData Content {
      get { return (DataAnalysisProblemData)base.Content; }
      set { base.Content = value; }
    }

    public AbstractFeatureCorrelationView() {
      InitializeComponent();
      sortedColumnIndices = new List<KeyValuePair<int, SortOrder>>();
      rowComparer = new StringConvertibleMatrixView.RowComparer();
      fcc = new FeatureCorrelationCalculator();
      var calculatorList = FCE.EnumToList<FCE.CorrelationCalculators>().Select(x => new KeyValuePair<FCE.CorrelationCalculators, string>(x, FCE.GetEnumDescription(x))).ToList();
      CorrelationCalcComboBox.ValueMember = "Key";
      CorrelationCalcComboBox.DisplayMember = "Value";
      CorrelationCalcComboBox.DataSource = new BindingList<KeyValuePair<FCE.CorrelationCalculators, string>>(calculatorList);
      var partitionList = FCE.EnumToList<FCE.Partitions>().Select(x => new KeyValuePair<FCE.Partitions, string>(x, FCE.GetEnumDescription(x))).ToList();
      PartitionComboBox.ValueMember = "Key";
      PartitionComboBox.DisplayMember = "Value";
      PartitionComboBox.DataSource = new BindingList<KeyValuePair<FCE.Partitions, string>>(partitionList);
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      fcc.ProgressCalculation += new DataAnalysis.FeatureCorrelationCalculator.ProgressCalculationHandler(Content_ProgressCalculation);
      fcc.CorrelationCalculationFinished += new DataAnalysis.FeatureCorrelationCalculator.CorrelationCalculationFinishedHandler(Content_CorrelationCalculationFinished);
    }

    protected override void DeregisterContentEvents() {
      fcc.CorrelationCalculationFinished += new DataAnalysis.FeatureCorrelationCalculator.CorrelationCalculationFinishedHandler(Content_CorrelationCalculationFinished);
      fcc.ProgressCalculation += new DataAnalysis.FeatureCorrelationCalculator.ProgressCalculationHandler(Content_ProgressCalculation);
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        fcc.ProblemData = Content;
        CalculateCorrelation();
      } else {
        DataGridView.Columns.Clear();
        DataGridView.Rows.Clear();
      }
    }

    protected void CorrelationMeasureComboBox_SelectedChangeCommitted(object sender, System.EventArgs e) {
      CalculateCorrelation();
    }
    protected void PartitionComboBox_SelectedChangeCommitted(object sender, System.EventArgs e) {
      CalculateCorrelation();
    }

    protected abstract void CalculateCorrelation();

    protected void UpdateDataGrid() {
      virtualRowIndices = Enumerable.Range(0, currentCorrelation.Rows).ToArray();
      DataGridViewColumn[] columns = new DataGridViewColumn[currentCorrelation.Columns];
      for (int i = 0; i < columns.Length; ++i) {
        var column = new DataGridViewTextBoxColumn();
        column.FillWeight = 1;
        columns[i] = column;
      }

      DataGridView.Columns.Clear();
      DataGridView.Columns.AddRange(columns);

      DataGridView.RowCount = currentCorrelation.Rows;

      ClearSorting();
      UpdateColumnHeaders();
      UpdateRowHeaders();

      maximumLabel.Text = currentCorrelation.Maximum.ToString();
      minimumLabel.Text = currentCorrelation.Minimum.ToString();

      DataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
      DataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
      DataGridView.Enabled = true;
    }

    protected virtual void UpdateColumnHeaders() {
      for (int i = 0; i < DataGridView.ColumnCount; i++) {
        DataGridView.Columns[i].HeaderText = currentCorrelation.ColumnNames.ElementAt(i);
      }
    }
    protected virtual void UpdateRowHeaders() {
      for (int i = 0; i < DataGridView.RowCount; i++) {
        DataGridView.Rows[i].HeaderCell.Value = currentCorrelation.RowNames.ElementAt(virtualRowIndices[i]);
      }
    }

    protected void Content_ProgressCalculation(object sender, ProgressChangedEventArgs e) {
      if (!CalculatingPanel.Visible && e.ProgressPercentage != HeatMapProgressBar.Maximum) {
        CalculatingPanel.Show();
      } else if (e.ProgressPercentage == HeatMapProgressBar.Maximum) {
        CalculatingPanel.Hide();
      }
      HeatMapProgressBar.Value = e.ProgressPercentage;
    }

    protected abstract void Content_CorrelationCalculationFinished(object sender, FeatureCorrelationCalculator.CorrelationCalculationFinishedArgs e);

    protected void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
      if (Content == null) return;
      int rowIndex = virtualRowIndices[e.RowIndex];
      e.Value = currentCorrelation[rowIndex, e.ColumnIndex];
    }

    protected void DataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
      if (Content == null) return;
      if (e.RowIndex < 0) return;
      if (e.ColumnIndex < 0) return;
      if (e.State.HasFlag(DataGridViewElementStates.Selected)) return;
      if (!e.PaintParts.HasFlag(DataGridViewPaintParts.Background)) return;

      int rowIndex = virtualRowIndices[e.RowIndex];
      Color backColor = GetDataPointColor(currentCorrelation[rowIndex, e.ColumnIndex], currentCorrelation.Minimum, currentCorrelation.Maximum);
      using (Brush backColorBrush = new SolidBrush(backColor)) {
        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
      }
      e.PaintContent(e.CellBounds);
      e.Handled = true;
    }

    protected virtual Color GetDataPointColor(double value, double min, double max) {
      IList<Color> colors = ColorGradient.Colors;
      int index = (int)((colors.Count - 1) * (value - min) / (max - min));
      if (index >= colors.Count) index = colors.Count - 1;
      if (index < 0) index = 0;
      return colors[index];
    }

    #region sort
    protected void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (Content != null) {
        if (e.Button == MouseButtons.Left) {
          bool addToSortedIndices = (Control.ModifierKeys & Keys.Control) == Keys.Control;
          SortOrder newSortOrder = SortOrder.Ascending;
          if (sortedColumnIndices.Any(x => x.Key == e.ColumnIndex)) {
            SortOrder oldSortOrder = sortedColumnIndices.Where(x => x.Key == e.ColumnIndex).First().Value;
            int enumLength = Enum.GetValues(typeof(SortOrder)).Length;
            newSortOrder = oldSortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), ((((int)oldSortOrder) + 1) % enumLength).ToString());
          }

          if (!addToSortedIndices)
            sortedColumnIndices.Clear();

          if (sortedColumnIndices.Any(x => x.Key == e.ColumnIndex)) {
            int sortedIndex = sortedColumnIndices.FindIndex(x => x.Key == e.ColumnIndex);
            if (newSortOrder != SortOrder.None)
              sortedColumnIndices[sortedIndex] = new KeyValuePair<int, SortOrder>(e.ColumnIndex, newSortOrder);
            else
              sortedColumnIndices.RemoveAt(sortedIndex);
          } else
            if (newSortOrder != SortOrder.None)
              sortedColumnIndices.Add(new KeyValuePair<int, SortOrder>(e.ColumnIndex, newSortOrder));
          Sort();
        }
      }
    }

    protected virtual void ClearSorting() {
      virtualRowIndices = Enumerable.Range(0, currentCorrelation.Rows).ToArray();
      sortedColumnIndices.Clear();
      UpdateSortGlyph();
    }

    private void Sort() {
      virtualRowIndices = Sort(sortedColumnIndices);
      UpdateSortGlyph();
      UpdateRowHeaders();
      DataGridView.Invalidate();
    }

    protected virtual int[] Sort(IEnumerable<KeyValuePair<int, SortOrder>> sortedColumns) {
      int[] newSortedIndex = Enumerable.Range(0, currentCorrelation.Rows).ToArray();
      if (sortedColumns.Count() != 0) {
        rowComparer.SortedIndices = sortedColumns;
        rowComparer.Matrix = currentCorrelation;
        Array.Sort(newSortedIndex, rowComparer);
      }
      return newSortedIndex;
    }
    private void UpdateSortGlyph() {
      foreach (DataGridViewColumn col in this.DataGridView.Columns)
        col.HeaderCell.SortGlyphDirection = SortOrder.None;
      foreach (KeyValuePair<int, SortOrder> p in sortedColumnIndices)
        this.DataGridView.Columns[p.Key].HeaderCell.SortGlyphDirection = p.Value;
    }
    #endregion

    #region copy
    protected void DataGridView_KeyDown(object sender, KeyEventArgs e) {
      if (e.Control && e.KeyCode == Keys.C)
        CopyValuesFromDataGridView();
    }

    private void CopyValuesFromDataGridView() {
      if (DataGridView.SelectedCells.Count == 0) return;
      StringBuilder s = new StringBuilder();
      int minRowIndex = DataGridView.SelectedCells[0].RowIndex;
      int maxRowIndex = DataGridView.SelectedCells[DataGridView.SelectedCells.Count - 1].RowIndex;
      int minColIndex = DataGridView.SelectedCells[0].ColumnIndex;
      int maxColIndex = DataGridView.SelectedCells[DataGridView.SelectedCells.Count - 1].ColumnIndex;

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

      bool addRowNames = DataGridView.AreAllCellsSelected(false) && currentCorrelation.RowNames.Count() > 0;
      bool addColumnNames = DataGridView.AreAllCellsSelected(false) && currentCorrelation.ColumnNames.Count() > 0;

      //add colum names
      if (addColumnNames) {
        if (addRowNames)
          s.Append('\t');

        DataGridViewColumn column = DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        while (column != null) {
          s.Append(column.HeaderText);
          s.Append('\t');
          column = DataGridView.Columns.GetNextColumn(column, DataGridViewElementStates.Visible, DataGridViewElementStates.None);
        }
        s.Remove(s.Length - 1, 1); //remove last tab
        s.Append(Environment.NewLine);
      }

      for (int i = minRowIndex; i <= maxRowIndex; i++) {
        int rowIndex = this.virtualRowIndices[i];
        if (addRowNames) {
          s.Append(currentCorrelation.RowNames.ElementAt(rowIndex));
          s.Append('\t');
        }

        DataGridViewColumn column = DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        while (column != null) {
          DataGridViewCell cell = DataGridView[column.Index, i];
          if (cell.Selected) {
            s.Append(currentCorrelation[rowIndex, column.Index]);
            s.Append('\t');
          }

          column = DataGridView.Columns.GetNextColumn(column, DataGridViewElementStates.Visible, DataGridViewElementStates.None);
        }
        s.Remove(s.Length - 1, 1); //remove last tab
        s.Append(Environment.NewLine);
      }
      Clipboard.SetText(s.ToString());
    }
    #endregion
  }
}