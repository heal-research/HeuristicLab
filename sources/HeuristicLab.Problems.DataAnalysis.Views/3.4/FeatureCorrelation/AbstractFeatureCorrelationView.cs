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

using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Feature Correlation View")]
  [Content(typeof(DataAnalysisProblemData), false)]
  public abstract partial class AbstractFeatureCorrelationView : AsynchronousContentView {
    protected FeatureCorrelationCalculator fcc;
    protected HeatMap currentCorrelation;

    public new DataAnalysisProblemData Content {
      get { return (DataAnalysisProblemData)base.Content; }
      set { base.Content = value; }
    }

    protected AbstractFeatureCorrelationView() {
      InitializeComponent();
      fcc = new FeatureCorrelationCalculator();
      var calculators = ApplicationManager.Manager.GetInstances<IDependencyCalculator>();
      var calcList = calculators.OrderBy(c => c.Name).Select(c => new { Name = c.Name, Calculator = c }).ToList();
      CorrelationCalcComboBox.ValueMember = "Calculator";
      CorrelationCalcComboBox.DisplayMember = "Name";
      CorrelationCalcComboBox.DataSource = calcList;
      CorrelationCalcComboBox.SelectedItem = calcList.First(c => c.Calculator.GetType().Equals(typeof(PearsonsRDependenceCalculator)));
      PartitionComboBox.DataSource = FeatureCorrelationPartitions.Partitions;
      PartitionComboBox.SelectedItem = FeatureCorrelationPartitions.TRAININGSAMPLES;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      fcc.ProgressCalculation += new FeatureCorrelationCalculator.ProgressCalculationHandler(Content_ProgressCalculation);
      fcc.CorrelationCalculationFinished += new FeatureCorrelationCalculator.CorrelationCalculationFinishedHandler(Content_CorrelationCalculationFinished);
    }

    protected override void DeregisterContentEvents() {
      fcc.CorrelationCalculationFinished += new FeatureCorrelationCalculator.CorrelationCalculationFinishedHandler(Content_CorrelationCalculationFinished);
      fcc.ProgressCalculation += new FeatureCorrelationCalculator.ProgressCalculationHandler(Content_ProgressCalculation);
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      fcc.TryCancelCalculation();
      if (Content != null) {
        fcc.ProblemData = Content;
        CalculateCorrelation();
      } else {
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
    protected abstract void Content_CorrelationCalculationFinished(object sender, FeatureCorrelationCalculator.CorrelationCalculationFinishedArgs e);

    protected void UpdateDataView() {
      maximumLabel.Text = currentCorrelation.Maximum.ToString();
      minimumLabel.Text = currentCorrelation.Minimum.ToString();

      currentCorrelation.SortableView = true;
      dataView.Content = currentCorrelation;
      dataView.Enabled = true;
    }

    protected void Content_ProgressCalculation(object sender, ProgressChangedEventArgs e) {
      if (!CalculatingPanel.Visible && e.ProgressPercentage != HeatMapProgressBar.Maximum) {
        CalculatingPanel.Show();
      } else if (e.ProgressPercentage == HeatMapProgressBar.Maximum) {
        CalculatingPanel.Hide();
      }
      HeatMapProgressBar.Value = e.ProgressPercentage;
    }
  }
}