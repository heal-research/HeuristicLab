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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Timeframe Feature Correlation View")]
  [Content(typeof(DataAnalysisProblemData), false)]
  public partial class TimeframeFeatureCorrelationView : AbstractFeatureCorrelationView {

    private FeatureCorrelationTimeframeCache correlationTimeframCache;

    public TimeframeFeatureCorrelationView() {
      InitializeComponent();
      correlationTimeframCache = new FeatureCorrelationTimeframeCache();
    }

    protected override void OnContentChanged() {
      correlationTimeframCache.Reset();
      if (Content != null) {
        dataView.RowVisibility = SetInitialVariableVisibility();
        VariableSelectionComboBox.DataSource = Content.Dataset.DoubleVariables.ToList();
      }
      base.OnContentChanged();
    }

    protected void VariableSelectionComboBox_SelectedChangeCommitted(object sender, EventArgs e) {
      CalculateCorrelation();
    }
    protected void TimeframeTextbox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        CalculateCorrelation();
      }
    }

    protected override void CalculateCorrelation() {
      if (CorrelationCalcComboBox.SelectedItem != null && PartitionComboBox.SelectedItem != null
        && VariableSelectionComboBox.SelectedItem != null && ValidateTimeframeTextbox()) {
        string variable = (string)VariableSelectionComboBox.SelectedItem;
        IDependencyCalculator calc = (IDependencyCalculator)CorrelationCalcComboBox.SelectedValue;
        string partition = (string)PartitionComboBox.SelectedValue;
        int frames;
        int.TryParse(TimeframeTextbox.Text, out frames);
        dataView.Enabled = false;
        double[,] corr = correlationTimeframCache.GetTimeframeCorrelation(calc, partition, variable);
        if (corr == null) {
          fcc.CalculateTimeframeElements(calc, partition, variable, frames);
        } else if (corr.GetLength(1) <= frames) {
          fcc.CalculateTimeframeElements(calc, partition, variable, frames, corr);
        } else {
          fcc.TryCancelCalculation();
          SetNewCorrelation(corr, calc, frames);
          UpdateDataView();
        }
      }
    }

    protected bool ValidateTimeframeTextbox() {
      int help;
      if (!int.TryParse(TimeframeTextbox.Text, out help)) {
        MessageBox.Show("Timeframe couldn't be parsed. Enter a valid integer value.", "Parse Error", MessageBoxButtons.OK);
        return false;
      } else {
        if (help > 50) {
          DialogResult dr = MessageBox.Show("The entered value is bigger than 50. Are you sure you want to calculate? " +
                                            "The calculation could take some time.", "Huge Value Warning", MessageBoxButtons.YesNo);
          return dr.Equals(DialogResult.Yes);
        }
      }
      return true;
    }

    private void SetNewCorrelation(double[,] elements, IDependencyCalculator calc, int frames) {
      double[,] neededValues = new double[elements.GetLength(0), frames + 1];
      for (int i = 0; i < elements.GetLength(0); i++) {
        Array.Copy(elements, i * elements.GetLength(1), neededValues, i * neededValues.GetLength(1), frames + 1);
      }
      SetNewCorrelation(neededValues, calc);
    }

    private void SetNewCorrelation(double[,] elements, IDependencyCalculator calc) {
      DoubleRange range = calc.Interval;
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
        UpdateDataView();
      }
    }

    [NonDiscoverableType]
    private class FeatureCorrelationTimeframeCache : Object {
      private Dictionary<Tuple<IDependencyCalculator, string, string>, double[,]> timeFrameCorrelationsCache;

      public FeatureCorrelationTimeframeCache()
        : base() {
        InitializeCaches();
      }

      private void InitializeCaches() {
        timeFrameCorrelationsCache = new Dictionary<Tuple<IDependencyCalculator, string, string>, double[,]>();
      }

      public void Reset() {
        InitializeCaches();
      }

      public double[,] GetTimeframeCorrelation(IDependencyCalculator calc, string partition, string variable) {
        double[,] corr;
        var key = new Tuple<IDependencyCalculator, string, string>(calc, partition, variable);
        timeFrameCorrelationsCache.TryGetValue(key, out corr);
        return corr;
      }

      public void SetTimeframeCorrelation(IDependencyCalculator calc, string partition, string variable, double[,] correlation) {
        var key = new Tuple<IDependencyCalculator, string, string>(calc, partition, variable);
        timeFrameCorrelationsCache[key] = correlation;
      }
    }
  }
}