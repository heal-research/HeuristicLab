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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Feature Correlation View")]
  [Content(typeof(DataAnalysisProblemData), false)]
  public abstract partial class AbstractFeatureCorrelationView : AsynchronousContentView {
    public const string ALLSAMPLES = "All Samples";
    public const string TRAININGSAMPLES = "Training Samples";
    public const string TESTSAMPLES = "Test Samples";

    public static readonly IList<string> Partitions = new List<string>() { ALLSAMPLES, TRAININGSAMPLES, TESTSAMPLES }.AsReadOnly();

    protected AbstractFeatureCorrelationCalculator CorrelationCalculator { get; set; }

    public new DataAnalysisProblemData Content {
      get { return (DataAnalysisProblemData)base.Content; }
      set { base.Content = value; }
    }

    protected AbstractFeatureCorrelationView() {
      InitializeComponent();
      dataView.FormatPattern = "0.000";
      var calculators = ApplicationManager.Manager.GetInstances<IDependencyCalculator>();
      var calcList = calculators.OrderBy(c => c.Name).Select(c => new { Name = c.Name, Calculator = c }).ToList();
      correlationCalcComboBox.ValueMember = "Calculator";
      correlationCalcComboBox.DisplayMember = "Name";
      correlationCalcComboBox.DataSource = calcList;
      correlationCalcComboBox.SelectedItem = calcList.First(c => c.Calculator.GetType() == typeof(PearsonsRDependenceCalculator));
      partitionComboBox.DataSource = Partitions;
      partitionComboBox.SelectedItem = TRAININGSAMPLES;
      progressPanel.Visible = false;
    }

    protected override void OnClosing(FormClosingEventArgs e) {
      base.OnClosing(e);
      CorrelationCalculator.TryCancelCalculation();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      CorrelationCalculator.ProgressChanged += FeatureCorrelation_ProgressChanged;
      CorrelationCalculator.CorrelationCalculationFinished += FeatureCorrelation_CalculationFinished;
    }

    protected override void DeregisterContentEvents() {
      CorrelationCalculator.ProgressChanged -= FeatureCorrelation_ProgressChanged;
      CorrelationCalculator.CorrelationCalculationFinished -= FeatureCorrelation_CalculationFinished;
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      CorrelationCalculator.TryCancelCalculation();
      if (Content != null) {
        CalculateCorrelation();
      } else {
        progressPanel.Visible = false;
        dataView.Maximum = 0;
        dataView.Minimum = 0;
        dataView.Content = null;
        dataView.ResetVisibility();
      }
    }

    protected virtual bool[] SetInitialVariableVisibility() {
      bool[] initialVisibility = new bool[Content.Dataset.DoubleVariables.Count()];
      int i = 0;
      foreach (var variable in Content.Dataset.DoubleVariables) {
        initialVisibility[i] = Content.AllowedInputVariables.Contains(variable);
        i++;
      }
      return initialVisibility;
    }

    protected void CorrelationMeasureComboBox_SelectedChangeCommitted(object sender, System.EventArgs e) {
      CalculateCorrelation();
    }
    protected void PartitionComboBox_SelectedChangeCommitted(object sender, System.EventArgs e) {
      CalculateCorrelation();
    }

    protected abstract void CalculateCorrelation();

    protected abstract void FeatureCorrelation_CalculationFinished(object sender,
      AbstractFeatureCorrelationCalculator.CorrelationCalculationFinishedArgs e);

    protected void UpdateDataView(DoubleMatrix correlation) {
      IDependencyCalculator calc = (IDependencyCalculator)correlationCalcComboBox.SelectedValue;
      maximumLabel.Text = calc.Maximum.ToString();
      minimumLabel.Text = calc.Minimum.ToString();

      correlation.SortableView = true;
      dataView.Maximum = calc.Maximum;
      dataView.Minimum = calc.Minimum;

      dataView.Content = correlation;
      dataView.Enabled = true;
    }

    protected void FeatureCorrelation_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      if (!progressPanel.Visible && e.ProgressPercentage != progressBar.Maximum) {
        progressPanel.Show();
      } else if (e.ProgressPercentage == progressBar.Maximum) {
        progressPanel.Hide();
      }
      progressBar.Value = e.ProgressPercentage;
    }
  }
}