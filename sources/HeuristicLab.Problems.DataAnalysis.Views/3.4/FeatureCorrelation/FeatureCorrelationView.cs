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

using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using FCE = HeuristicLab.Problems.DataAnalysis.FeatureCorrelationEnums;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Feature Correlation View")]
  [Content(typeof(DataAnalysisProblemData), false)]
  public partial class FeatureCorrelationView : AbstractFeatureCorrelationView {

    private FeatureCorrelationCache correlationCache;

    public FeatureCorrelationView()
      : base() {
      InitializeComponent();
      correlationCache = new FeatureCorrelationCache();
    }

    protected override void OnContentChanged() {
      correlationCache.Reset();
      base.OnContentChanged();
    }

    protected override void CalculateCorrelation() {
      if (CorrelationCalcComboBox.SelectedItem != null && PartitionComboBox.SelectedItem != null) {
        FCE.CorrelationCalculators calc = (FCE.CorrelationCalculators)CorrelationCalcComboBox.SelectedValue;
        FCE.Partitions partition = (FCE.Partitions)PartitionComboBox.SelectedValue;
        DataGridView.Columns.Clear();
        DataGridView.Enabled = false;
        double[,] corr = correlationCache.GetCorrelation(calc, partition);
        if (corr == null) {
          fcc.CalculateElements(calc, partition);
        } else {
          SetNewCorrelation(corr, calc);
          UpdateDataGrid();
        }
      }
    }

    private void SetNewCorrelation(double[,] elements, FCE.CorrelationCalculators calc) {
      DoubleRange range = FCE.calculatorInterval[calc];
      HeatMap hm = new HeatMap(elements, "", range.End, range.Start);
      hm.RowNames = Content.Dataset.DoubleVariables;
      hm.ColumnNames = Content.Dataset.DoubleVariables;
      currentCorrelation = hm;
    }

    protected override void Content_CorrelationCalculationFinished(object sender, FeatureCorrelationCalculator.CorrelationCalculationFinishedArgs e) {
      if (InvokeRequired) {
        Invoke(new FeatureCorrelationCalculator.CorrelationCalculationFinishedHandler(Content_CorrelationCalculationFinished), sender, e);
      } else {
        correlationCache.SetCorrelation(e.Calculcator, e.Partition, e.Correlation);
        SetNewCorrelation(e.Correlation, e.Calculcator);
        UpdateDataGrid();
      }
    }
  }
}