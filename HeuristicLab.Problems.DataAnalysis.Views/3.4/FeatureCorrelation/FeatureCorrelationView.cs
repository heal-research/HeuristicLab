#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Feature Correlation View")]
  [Content(typeof(DataAnalysisProblemData), false)]
  public partial class FeatureCorrelationView : AbstractFeatureCorrelationView {

    private FeatureCorrelationCache correlationCache;
    private new FeatureCorrelationCalculator CorrelationCalculator {
      get { return (FeatureCorrelationCalculator)base.CorrelationCalculator; }
      set { base.CorrelationCalculator = value; }
    }

    public FeatureCorrelationView()
      : base() {
      InitializeComponent();
      CorrelationCalculator = new FeatureCorrelationCalculator();
      correlationCache = new FeatureCorrelationCache();
    }

    protected override void OnContentChanged() {
      if (Content != null) {
        dataView.ColumnVisibility = dataView.RowVisibility = SetInitialVariableVisibility();
      }
      correlationCache.Reset();
      base.OnContentChanged();
    }

    private void ignoreMissingValuesCheckbox_CheckedChanged(object sender, EventArgs e) {
      CalculateCorrelation();
    }

    protected override void CalculateCorrelation() {
      if (correlationCalcComboBox.SelectedItem == null) return;
      if (partitionComboBox.SelectedItem == null) return;

      IDependencyCalculator calc = (IDependencyCalculator)correlationCalcComboBox.SelectedValue;
      string partition = (string)partitionComboBox.SelectedValue;
      dataView.Enabled = false;
      double[,] corr = correlationCache.GetCorrelation(calc, partition, ignoreMissingValuesCheckbox.Checked);
      if (corr == null) {
        CorrelationCalculator.CalculateElements(Content, calc, partition, ignoreMissingValuesCheckbox.Checked);
      } else {
        CorrelationCalculator.TryCancelCalculation();
        var correlation = new DoubleMatrix(corr, Content.Dataset.DoubleVariables, Content.Dataset.DoubleVariables);
        UpdateDataView(correlation);
      }
    }

    protected override void FeatureCorrelation_CalculationFinished(object sender, AbstractFeatureCorrelationCalculator.CorrelationCalculationFinishedArgs e) {
      if (InvokeRequired) {
        Invoke(new AbstractFeatureCorrelationCalculator.CorrelationCalculationFinishedHandler(FeatureCorrelation_CalculationFinished), sender, e);
        return;
      }
      correlationCache.SetCorrelation(e.Calculcator, e.Partition, e.IgnoreMissingValues, e.Correlation);

      var correlation = new DoubleMatrix(e.Correlation, Content.Dataset.DoubleVariables, Content.Dataset.DoubleVariables);
      UpdateDataView(correlation);
    }

    [NonDiscoverableType]
    private class FeatureCorrelationCache : Object {
      private Dictionary<Tuple<IDependencyCalculator, string, bool>, double[,]> correlationsCache;

      public FeatureCorrelationCache()
        : base() {
        InitializeCaches();
      }

      private void InitializeCaches() {
        correlationsCache = new Dictionary<Tuple<IDependencyCalculator, string, bool>, double[,]>();
      }

      public void Reset() {
        InitializeCaches();
      }

      public double[,] GetCorrelation(IDependencyCalculator calc, string partition, bool ignoreMissingValues) {
        double[,] corr;
        var key = Tuple.Create(calc, partition, ignoreMissingValues);
        correlationsCache.TryGetValue(key, out corr);
        return corr;
      }

      public void SetCorrelation(IDependencyCalculator calc, string partition, bool ignoreMissingValues, double[,] correlation) {
        var key = Tuple.Create(calc, partition, ignoreMissingValues);
        correlationsCache[key] = correlation;
      }
    }
  }
}