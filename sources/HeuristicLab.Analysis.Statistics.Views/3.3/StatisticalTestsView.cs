#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Views;

namespace HeuristicLab.Analysis.Statistics.Views {
  [View("Statistical Tests", "HeuristicLab.Analysis.Statistics.Views.InfoResources.StatisticalTestsInfo.rtf")]
  [Content(typeof(RunCollection), false)]
  public sealed partial class StatisticalTestsView : ItemView, IConfigureableView {
    private double significanceLevel = 0.05;
    private const int requiredSampleSize = 5;
    private double[][] data;
    private bool suppressUpdates;
    private bool initializing;

    public double SignificanceLevel {
      get { return significanceLevel; }
      set {
        if (!significanceLevel.IsAlmost(value)) {
          significanceLevel = value;
          ResetUI();
          CalculateValues();
        }
      }
    }

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get { return true; }
      set { /*not needed because results are always readonly */}
    }

    public StatisticalTestsView() {
      InitializeComponent();
    }

    public void ShowConfiguration() {
      using (StatisticalTestsConfigurationDialog dlg = new StatisticalTestsConfigurationDialog(this)) {
        dlg.ShowDialog(this);
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content != null) {
        UpdateUI();
      } else {
        ResetUI();
      }
      UpdateCaption();
    }

    private void UpdateUI() {
      initializing = true;
      UpdateResultComboBox();
      UpdateGroupsComboBox();
      RebuildDataTable();
      FillCompComboBox();
      ResetUI();
      CalculateValues();
      initializing = false;
    }

    private void UpdateCaption() {
      Caption = Content != null ? Content.OptimizerName + " Statistical Tests" : ViewAttribute.GetViewName(GetType());
    }

    #region events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ColumnsChanged += Content_ColumnsChanged;
      Content.RowsChanged += Content_RowsChanged;
      Content.CollectionReset += Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged += Content_UpdateOfRunsInProgressChanged;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ColumnsChanged -= Content_ColumnsChanged;
      Content.RowsChanged -= Content_RowsChanged;
      Content.CollectionReset -= Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged -= Content_UpdateOfRunsInProgressChanged;
    }

    void Content_RowsChanged(object sender, EventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_RowsChanged, sender, e);
      else {
        UpdateUI();
      }
    }

    void Content_ColumnsChanged(object sender, EventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_ColumnsChanged, sender, e);
      else {
        UpdateUI();
      }
    }

    private void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired) Invoke((Action<object, CollectionItemsChangedEventArgs<IRun>>)Content_CollectionReset, sender, e);
      else {
        UpdateUI();
      }
    }

    void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_UpdateOfRunsInProgressChanged, sender, e);
      else {
        suppressUpdates = Content.UpdateOfRunsInProgress;
        if (!suppressUpdates) UpdateUI();
      }
    }

    private void openBoxPlotToolStripMenuItem_Click(object sender, EventArgs e) {
      RunCollectionBoxPlotView boxplotView = new RunCollectionBoxPlotView();
      boxplotView.Content = Content;
      boxplotView.SetXAxis(groupComboBox.SelectedItem.ToString());
      boxplotView.SetYAxis(resultComboBox.SelectedItem.ToString());

      boxplotView.Show();
    }

    private void groupCompComboBox_SelectedValueChanged(object sender, EventArgs e) {
      if (initializing || suppressUpdates) return;
      string curItem = (string)groupCompComboBox.SelectedItem;
      CalculatePairwise(curItem);
    }

    private void resultComboBox_SelectedValueChanged(object sender, EventArgs e) {
      if (initializing || suppressUpdates) return;
      RebuildDataTable();
      ResetUI();
      CalculateValues();
    }

    private void groupComboBox_SelectedValueChanged(object sender, EventArgs e) {
      if (initializing || suppressUpdates) return;
      RebuildDataTable();
      FillCompComboBox();
      ResetUI();
      CalculateValues();
    }
    #endregion

    private void UpdateGroupsComboBox() {
      string selectedItem = (string)groupComboBox.SelectedItem;

      groupComboBox.Items.Clear();
      var parameters = (from run in Content
                        where run.Visible
                        from param in run.Parameters
                        select param.Key).Distinct().ToArray();

      foreach (var p in parameters) {
        var variations = (from run in Content
                          where run.Visible && run.Parameters.ContainsKey(p) &&
                          (run.Parameters[p] is IntValue || run.Parameters[p] is DoubleValue ||
                          run.Parameters[p] is StringValue || run.Parameters[p] is BoolValue)
                          select ((dynamic)run.Parameters[p]).Value).Distinct();

        if (variations.Count() > 1) {
          groupComboBox.Items.Add(p);
        }
      }

      if (groupComboBox.Items.Count > 0) {
        //try to select something different than "Seed" or "Algorithm Name" as this makes no sense
        //and takes a long time to group
        List<int> possibleIndizes = new List<int>();
        for (int i = 0; i < groupComboBox.Items.Count; i++) {
          if (groupComboBox.Items[i].ToString() != "Seed"
            && groupComboBox.Items[i].ToString() != "Algorithm Name") {
            possibleIndizes.Add(i);
          }
        }

        if (selectedItem != null && groupComboBox.Items.Contains(selectedItem)) {
          groupComboBox.SelectedItem = selectedItem;
        } else if (possibleIndizes.Count > 0) {
          groupComboBox.SelectedItem = groupComboBox.Items[possibleIndizes.First()];
        }
      }
    }

    private string[] GetColumnNames(IEnumerable<IRun> runs) {
      string parameterName = (string)groupComboBox.SelectedItem;
      var r = runs.Where(x => x.Parameters.ContainsKey(parameterName));
      return r.Select(x => ((dynamic)x.Parameters[parameterName]).Value).Distinct().Select(x => (string)x.ToString()).ToArray();
    }

    private void UpdateResultComboBox() {
      string selectedItem = (string)resultComboBox.SelectedItem;

      resultComboBox.Items.Clear();
      var results = (from run in Content
                     where run.Visible
                     from result in run.Results
                     where result.Value is IntValue || result.Value is DoubleValue
                     select result.Key).Distinct().ToArray();

      resultComboBox.Items.AddRange(results);

      if (selectedItem != null && resultComboBox.Items.Contains(selectedItem)) {
        resultComboBox.SelectedItem = selectedItem;
      } else if (resultComboBox.Items.Count > 0) {
        resultComboBox.SelectedItem = resultComboBox.Items[0];
      }
    }

    private void FillCompComboBox() {
      string selectedItem = (string)groupCompComboBox.SelectedItem;
      string parameterName = (string)groupComboBox.SelectedItem;
      if (parameterName != null) {
        string resultName = (string)resultComboBox.SelectedItem;
        if (resultName != null) {
          var runs = Content.Where(x => x.Results.ContainsKey(resultName) && x.Visible);
          var columnNames = GetColumnNames(runs).ToList();
          groupCompComboBox.Items.Clear();
          columnNames.ForEach(x => groupCompComboBox.Items.Add(x));
          if (selectedItem != null && groupCompComboBox.Items.Contains(selectedItem)) {
            groupCompComboBox.SelectedItem = selectedItem;
          } else if (groupCompComboBox.Items.Count > 0) {
            groupCompComboBox.SelectedItem = groupCompComboBox.Items[0];
          }
        }
      }
    }

    private void RebuildDataTable() {
      string parameterName = (string)groupComboBox.SelectedItem;
      if (parameterName != null) {
        string resultName = (string)resultComboBox.SelectedItem;

        var runs = Content.Where(x => x.Results.ContainsKey(resultName) && x.Visible);
        var columnNames = GetColumnNames(runs);
        var groups = GetGroups(columnNames, runs);
        data = new double[columnNames.Count()][];

        if (!groups.Any() || !columnNames.Any()) {
          return;
        }

        DoubleMatrix dt = new DoubleMatrix(groups.Select(x => x.Count()).Max(), columnNames.Count());
        dt.ColumnNames = columnNames;
        DataTable histogramDataTable = new DataTable(resultName);

        for (int i = 0; i < columnNames.Count(); i++) {
          int j = 0;
          data[i] = new double[groups[i].Count()];
          DataRow row = new DataRow(columnNames[i]);
          row.VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Histogram;
          histogramDataTable.Rows.Add(row);

          foreach (IRun run in groups[i]) {
            dt[j, i] = (double)((dynamic)run.Results[resultName]).Value;
            data[i][j] = dt[j, i];
            row.Values.Add(dt[j, i]);
            j++;
          }
        }

        GenerateChart(histogramDataTable);
        stringConvertibleMatrixView.Content = dt;
      }
    }

    private void GenerateChart(DataTable histogramTable) {
      histogramControl.ClearPoints();
      foreach (var row in histogramTable.Rows) {
        histogramControl.AddPoints(row.Name, row.Values, true);
      }
    }

    private List<IEnumerable<IRun>> GetGroups(string[] columnNames, IEnumerable<IRun> runs) {
      List<IEnumerable<IRun>> runCols = new List<IEnumerable<IRun>>();
      string parameterName = (string)groupComboBox.SelectedItem;

      foreach (string cn in columnNames) {
        var tmpRuns = runs.Where(x =>
        x.Parameters.ContainsKey(parameterName) &&
        (((string)((dynamic)x.Parameters[parameterName]).Value.ToString()) == cn));
        runCols.Add(tmpRuns);
      }

      return runCols;
    }

    private void ResetUI() {
      normalityLabel.Image = null;
      normalityTextLabel.Text = string.Empty;
      groupCompLabel.Image = null;
      groupComTextLabel.Text = string.Empty;
      pairwiseLabel.Image = null;
      pairwiseTextLabel.Text = string.Empty;

      pValTextBox.Text = string.Empty;
      equalDistsTextBox.Text = string.Empty;
    }

    private bool VerifyDataLength(bool showMessage) {
      if (data == null || data.Length < 2)
        return false;

      //alglib needs at least 5 samples for computation
      if (data.Any(x => x.Length < requiredSampleSize)) {
        if (showMessage)
          MessageBox.Show(this, "You need at least " + requiredSampleSize
            + " samples per group for computing hypothesis tests.", "HeuristicLab", MessageBoxButtons.OK,
            MessageBoxIcon.Error);
        return false;
      }
      return true;
    }

    private void CalculateValues() {
      if (!VerifyDataLength(true))
        return;

      if (data != null && data.All(x => x != null)) {
        MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>()
          .AddOperationProgressToView(this, "Calculating...");

        string curItem = (string)groupCompComboBox.SelectedItem;
        Task.Factory.StartNew(() => CalculateValuesAsync(curItem));
      }
    }

    private void CalculateValuesAsync(string groupName) {
      CalculateAllGroupsTest();
      CalculateNormalityTest();
      CalculatePairwiseTest(groupName);

      MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
    }

    private void CalculatePairwise(string groupName) {
      if (groupName == null) return;
      if (!VerifyDataLength(false))
        return;

      MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().AddOperationProgressToView(pairwiseTestGroupBox, "Calculating...");
      Task.Factory.StartNew(() => CalculatePairwiseAsync(groupName));
    }

    private void CalculatePairwiseAsync(string groupName) {
      CalculatePairwiseTest(groupName);

      MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(pairwiseTestGroupBox);
    }

    private void CalculateAllGroupsTest() {
      double pval = KruskalWallisTest.Test(data);
      DisplayAllGroupsTextResults(pval);
    }

    private void DisplayAllGroupsTextResults(double pval) {
      if (InvokeRequired) {
        Invoke((Action<double>)DisplayAllGroupsTextResults, pval);
      } else {
        pValTextBox.Text = pval.ToString();
        if (pval < significanceLevel) {
          groupCompLabel.Image = VSImageLibrary.Default;
          groupComTextLabel.Text = "There are groups with different distributions";
        } else {
          groupCompLabel.Image = VSImageLibrary.Warning;
          groupComTextLabel.Text = "Groups have an equal distribution";
        }
      }
    }

    private void CalculateNormalityTest() {
      double val;
      List<double> res = new List<double>();
      DoubleMatrix pValsMatrix = new DoubleMatrix(1, stringConvertibleMatrixView.Content.Columns);
      pValsMatrix.ColumnNames = stringConvertibleMatrixView.Content.ColumnNames;
      pValsMatrix.RowNames = new[] { "p-Value" };

      for (int i = 0; i < data.Length; i++) {
        alglib.jarqueberatest(data[i], data[i].Length, out val);
        res.Add(val);
        pValsMatrix[0, i] = val;
      }

      // p-value is below significance level and thus the null hypothesis (data is normally distributed) is rejected
      if (res.Any(x => x < significanceLevel)) {
        Invoke(new Action(() => {
          normalityLabel.Image = VSImageLibrary.Warning;
          normalityTextLabel.Text = "Some groups may not be normally distributed";
        }));
      } else {
        Invoke(new Action(() => {
          normalityLabel.Image = VSImageLibrary.Default;
          normalityTextLabel.Text = "All sample data is normally distributed";
        }));
      }

      Invoke(new Action(() => {
        normalityStringConvertibleMatrixView.Content = pValsMatrix;
        normalityStringConvertibleMatrixView.DataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
      }));
    }

    private void ShowPairwiseResult(int nrOfEqualDistributions) {
      double ratio = ((double)nrOfEqualDistributions) / (data.Length - 1) * 100.0;
      equalDistsTextBox.Text = ratio + " %";

      if (nrOfEqualDistributions == 0) {
        Invoke(new Action(() => {
          pairwiseLabel.Image = VSImageLibrary.Default;
          pairwiseTextLabel.Text = "All groups have different distributions";
        }));
      } else {
        Invoke(new Action(() => {
          pairwiseLabel.Image = VSImageLibrary.Warning;
          pairwiseTextLabel.Text = "Some groups have equal distributions";
        }));
      }
    }

    private void CalculatePairwiseTest(string groupName) {
      var columnNames = stringConvertibleMatrixView.Content.ColumnNames.ToList();
      int colIndex = columnNames.IndexOf(groupName);
      columnNames = columnNames.Where(x => x != groupName).ToList();

      double[][] newData = FilterDataForPairwiseTest(colIndex);

      var rowNames = new[] { "p-Value of Mann-Whitney U", "Adjusted p-Value of Mann-Whitney U",
            "p-Value of T-Test", "Adjusted p-Value of T-Test", "Cohen's d", "Hedges' g" };

      DoubleMatrix pValsMatrix = new DoubleMatrix(rowNames.Length, columnNames.Count());
      pValsMatrix.ColumnNames = columnNames;
      pValsMatrix.RowNames = rowNames;

      double mwuBothTails;
      double tTestBothTails;
      double[] mwuPValues = new double[newData.Length];
      double[] tTestPValues = new double[newData.Length];
      bool[] decision = null;
      double[] adjustedMwuPValues = null;
      double[] adjustedTtestPValues = null;
      int cnt = 0;

      for (int i = 0; i < newData.Length; i++) {
        mwuBothTails = PairwiseTest.MannWhitneyUTest(data[colIndex], newData[i]);
        tTestBothTails = PairwiseTest.TTest(data[colIndex], newData[i]);
        mwuPValues[i] = mwuBothTails;
        tTestPValues[i] = tTestBothTails;

        if (mwuBothTails > significanceLevel) {
          cnt++;
        }
      }

      adjustedMwuPValues = BonferroniHolm.Calculate(significanceLevel, mwuPValues, out decision);
      adjustedTtestPValues = BonferroniHolm.Calculate(significanceLevel, tTestPValues, out decision);

      for (int i = 0; i < newData.Length; i++) {
        pValsMatrix[0, i] = mwuPValues[i];
        pValsMatrix[1, i] = adjustedMwuPValues[i];
        pValsMatrix[2, i] = tTestPValues[i];
        pValsMatrix[3, i] = adjustedTtestPValues[i];
        pValsMatrix[4, i] = SampleSizeDetermination.CalculateCohensD(data[colIndex], newData[i]);
        pValsMatrix[5, i] = SampleSizeDetermination.CalculateHedgesG(data[colIndex], newData[i]);
      }

      Invoke(new Action(() => {
        pairwiseStringConvertibleMatrixView.Content = pValsMatrix;
        pairwiseStringConvertibleMatrixView.DataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
      }));

      ShowPairwiseResult(cnt);
    }

    private double[][] FilterDataForPairwiseTest(int columnToRemove) {
      double[][] newData = new double[data.Length - 1][];

      int i = 0;
      int l = 0;
      while (i < data.Length) {
        if (i != columnToRemove) {
          double[] row = new double[data[i].Length - 1];
          newData[l] = row;

          int j = 0, k = 0;
          while (j < row.Length) {
            if (i != columnToRemove) {
              newData[l][j] = data[i][k];
              j++;
              k++;
            } else {
              k++;
            }
          }
          i++;
          l++;
        } else {
          i++;
        }
      }
      return newData;
    }
  }
}
