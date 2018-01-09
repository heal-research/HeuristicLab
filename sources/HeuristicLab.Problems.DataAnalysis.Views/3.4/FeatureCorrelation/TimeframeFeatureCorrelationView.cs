#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Timeframe Feature Correlation View")]
  [Content(typeof(DataAnalysisProblemData), false)]
  public partial class TimeframeFeatureCorrelationView : AbstractFeatureCorrelationView {

    private FeatureCorrelationTimeframeCache correlationTimeframCache;
    private string lastFramesValue;

    private new TimeframeFeatureCorrelationCalculator CorrelationCalculator {
      get { return (TimeframeFeatureCorrelationCalculator)base.CorrelationCalculator; }
      set { base.CorrelationCalculator = value; }
    }


    public TimeframeFeatureCorrelationView() {
      InitializeComponent();
      CorrelationCalculator = new TimeframeFeatureCorrelationCalculator();
      correlationTimeframCache = new FeatureCorrelationTimeframeCache();
      errorProvider.SetIconAlignment(timeframeTextbox, ErrorIconAlignment.MiddleRight);
      errorProvider.SetIconPadding(timeframeTextbox, 2);
      lastFramesValue = timeframeTextbox.Text;
    }

    protected override void OnContentChanged() {
      correlationTimeframCache.Reset();
      if (Content != null) {
        dataView.RowVisibility = SetInitialVariableVisibility();
        SetVariableSelectionComboBox();
      }
      base.OnContentChanged();
    }

    protected virtual void SetVariableSelectionComboBox() {
      variableSelectionComboBox.DataSource = Content.Dataset.DoubleVariables.ToList();
    }

    private void VariableSelectionComboBox_SelectedChangeCommitted(object sender, EventArgs e) {
      CalculateCorrelation();
    }
    private void TimeframeTextbox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
        timeFrameLabel.Select();  // select label to validate data
      }

      if (e.KeyCode == Keys.Escape) {
        timeframeTextbox.Text = lastFramesValue;
        timeFrameLabel.Select();  // select label to validate data
      }
    }
    private void TimeframeTextbox_Validated(object sender, System.EventArgs e) {
      lastFramesValue = timeframeTextbox.Text;
      errorProvider.SetError(timeframeTextbox, string.Empty);
      CalculateCorrelation();
    }
    private void TimeframeTextbox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      int help;
      if (!int.TryParse(timeframeTextbox.Text, out help)) {
        errorProvider.SetError(timeframeTextbox, "Timeframe couldn't be parsed. Enter a valid integer value.");
        e.Cancel = true;
      } else {
        if (help > 50) {
          DialogResult dr = MessageBox.Show("The entered value is bigger than 50. Are you sure you want to calculate? " +
                                            "The calculation could take some time.", "Huge Value Warning", MessageBoxButtons.YesNo);
          e.Cancel = !dr.Equals(DialogResult.Yes);
        } else if (help < 0) {
          errorProvider.SetError(timeframeTextbox, "The entered value can't be negative!");
          e.Cancel = true;
        }
      }
    }

    protected override void CalculateCorrelation() {
      if (correlationCalcComboBox.SelectedItem == null) return;
      if (partitionComboBox.SelectedItem == null) return;
      if (variableSelectionComboBox.SelectedItem == null) return;

      string variable = (string)variableSelectionComboBox.SelectedItem;
      IDependencyCalculator calc = (IDependencyCalculator)correlationCalcComboBox.SelectedValue;
      string partition = (string)partitionComboBox.SelectedValue;
      int frames;
      int.TryParse(timeframeTextbox.Text, out frames);
      dataView.Enabled = false;
      double[,] corr = correlationTimeframCache.GetTimeframeCorrelation(calc, partition, variable);
      if (corr == null) {
        CorrelationCalculator.CalculateTimeframeElements(Content, calc, partition, variable, frames);
      } else if (corr.GetLength(1) <= frames) {
        CorrelationCalculator.CalculateTimeframeElements(Content, calc, partition, variable, frames, corr);
      } else {
        CorrelationCalculator.TryCancelCalculation();
        var columnNames = Enumerable.Range(0, corr.GetLength(1)).Select(x => x.ToString());
        var correlation = new DoubleMatrix(corr, columnNames, Content.Dataset.DoubleVariables);
        ((IStringConvertibleMatrix)correlation).Columns = frames + 1;
        UpdateDataView(correlation);
      }
    }

    protected override void FeatureCorrelation_CalculationFinished(object sender, AbstractFeatureCorrelationCalculator.CorrelationCalculationFinishedArgs e) {
      if (InvokeRequired) {
        Invoke(new AbstractFeatureCorrelationCalculator.CorrelationCalculationFinishedHandler(FeatureCorrelation_CalculationFinished), sender, e);
      } else {
        correlationTimeframCache.SetTimeframeCorrelation(e.Calculcator, e.Partition, e.Variable, e.Correlation);
        var columnNames = Enumerable.Range(0, e.Correlation.GetLength(1)).Select(x => x.ToString());
        var correlation = new DoubleMatrix(e.Correlation, columnNames, Content.Dataset.DoubleVariables);
        UpdateDataView(correlation);
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