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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using FCE = HeuristicLab.Problems.DataAnalysis.FeatureCorrelationEnums;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Timeframe Feature Correlation View")]
  [Content(typeof(DataAnalysisProblemData), false)]
  public partial class TimeframeFeatureCorrelationView : AbstractFeatureCorrelationView {

    private FeatureCorrelationTimeframeCache correlationTimeframCache;

    public TimeframeFeatureCorrelationView() {
      InitializeComponent();
      TimeframeComboBox.DataSource = Enumerable.Range(0, 16).ToList<int>();
      correlationTimeframCache = new FeatureCorrelationTimeframeCache();
    }

    protected override void OnContentChanged() {
      correlationTimeframCache.Reset();
      if (Content != null) {
        VariableSelectionComboBox.DataSource = Content.Dataset.DoubleVariables.ToList();
      }
      base.OnContentChanged();
    }

    protected void VariableSelectionComboBox_SelectedChangeCommitted(object sender, EventArgs e) {
      CalculateCorrelation();
    }
    protected void TimeframeComboBox_SelectedChangeCommitted(object sender, EventArgs e) {
      CalculateCorrelation();
    }

    protected override void CalculateCorrelation() {
      string variable = (string)VariableSelectionComboBox.SelectedItem;
      if (CorrelationCalcComboBox.SelectedItem != null && PartitionComboBox.SelectedItem != null && variable != null) {
        FCE.CorrelationCalculators calc = (FCE.CorrelationCalculators)CorrelationCalcComboBox.SelectedValue;
        FCE.Partitions partition = (FCE.Partitions)PartitionComboBox.SelectedValue;
        DataGridView.Columns.Clear();
        DataGridView.Enabled = false;
        int frames = (int)TimeframeComboBox.SelectedItem;
        double[,] corr = correlationTimeframCache.GetTimeframeCorrelation(calc, partition, variable);
        if (corr == null) {
          fcc.CalculateTimeframeElements(calc, partition, variable, frames);
        } else if (corr.GetLength(1) <= frames) {
          fcc.CalculateTimeframeElements(calc, partition, variable, frames, corr);
        } else {
          SetNewCorrelation(corr, calc, frames);
          UpdateDataGrid();
        }
      }
    }

    private void SetNewCorrelation(double[,] elements, FCE.CorrelationCalculators calc, int frames) {
      double[,] neededValues = new double[elements.GetLength(0), frames + 1];
      for (int i = 0; i < elements.GetLength(0); i++) {
        Array.Copy(elements, i * elements.GetLength(1), neededValues, i * neededValues.GetLength(1), frames + 1);
      }
      SetNewCorrelation(neededValues, calc);
    }

    private void SetNewCorrelation(double[,] elements, FCE.CorrelationCalculators calc) {
      DoubleRange range = FCE.calculatorInterval[calc];
      HeatMap hm = new HeatMap(elements, "", range.End, range.Start);
      hm.RowNames = Content.Dataset.DoubleVariables;
      hm.ColumnNames = Enumerable.Range(0, elements.GetLength(1)).Select(x => x.ToString());
      currentCorrelation = hm;
    }

    protected override void Content_CorrelationCalculationFinished(object sender, FeatureCorrelationCalculator.CorrelationCalculationFinishedArgs e) {
      if (InvokeRequired) {
        Invoke(new FeatureCorrelationCalculator.CorrelationCalculationFinishedHandler(Content_CorrelationCalculationFinished), sender, e);
      } else {
        correlationTimeframCache.SetTimeframeCorrelation(e.Calculcator, e.Partition, e.Variable, e.Correlation);
        SetNewCorrelation(e.Correlation, e.Calculcator);
        UpdateDataGrid();
      }
    }

    protected override void UpdateColumnHeaders() {
      for (int i = 0; i < DataGridView.ColumnCount; i++) {
        DataGridView.Columns[i].HeaderText = i.ToString();
      }
    }
  }
}