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
using System.ComponentModel;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Variable Impacts")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionVariableImpactsView : DataAnalysisSolutionEvaluationView {
    private class BackgroundworkerArguments {
      internal MainForm.WindowsForms.MainForm mainForm;
      internal RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum replMethod;
      internal RegressionSolutionVariableImpactsCalculator.FactorReplacementMethodEnum factorReplMethod;
      internal RegressionSolutionVariableImpactsCalculator.DataPartitionEnum dataPartition;
    }
    private enum SortingCriteria {
      ImpactValue,
      Occurrence,
      VariableName
    }
    private IProgress progress;
    private Dictionary<string, double> rawVariableImpacts = new Dictionary<string, double>();
    private BackgroundWorker worker = new BackgroundWorker();

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set {
        base.Content = value;
      }
    }

    public RegressionSolutionVariableImpactsView()
      : base() {
      InitializeComponent();

      //Little workaround. If you fill the ComboBox-Items in the other partial class, the UI-Designer will moan.
      this.sortByComboBox.Items.AddRange(Enum.GetValues(typeof(SortingCriteria)).Cast<object>().ToArray());
      this.sortByComboBox.SelectedItem = SortingCriteria.ImpactValue;

      //Set the default values
      this.dataPartitionComboBox.SelectedIndex = 0;
      this.replacementComboBox.SelectedIndex = 0;
      this.factorVarReplComboBox.SelectedIndex = 0;

      //Worker magic
      worker.WorkerSupportsCancellation = true;
      worker.WorkerReportsProgress = true;
      worker.DoWork += Worker_DoWork;
      worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
      worker.ProgressChanged += Worker_ProgressChanged;
    }


    private void Worker_DoWork(object sender, DoWorkEventArgs e) {
      var args = e.Argument as BackgroundworkerArguments;

      //Remember the original ordering of the variables
      var impacts = RegressionSolutionVariableImpactsCalculator.CalculateImpacts(Content, args.dataPartition, args.replMethod, args.factorReplMethod,
        (i) => {
          var worker = (sender as BackgroundWorker);
          worker.ReportProgress(0, i);
          return worker.CancellationPending;
        });

      if ((sender as BackgroundWorker).CancellationPending) { return; }
      var problemData = Content.ProblemData;
      var inputvariables = new HashSet<string>(problemData.AllowedInputVariables.Union(Content.Model.VariablesUsedForPrediction));
      var originalVariableOrdering = problemData.Dataset.VariableNames.Where(v => inputvariables.Contains(v)).Where(problemData.Dataset.VariableHasType<double>).ToList();

      rawVariableImpacts.Clear();
      originalVariableOrdering.ForEach(v => rawVariableImpacts.Add(v, impacts.First(vv => vv.Item1 == v).Item2));
    }
    private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      progress.ProgressValue = (double)e.UserState;
    }
    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) { throw e.Error; }
      if (e.Error == null && !e.Cancelled) { UpdateDataOrdering(); }
      ((MainForm.WindowsForms.MainForm)MainFormManager.MainForm).RemoveOperationProgressFromView(this);
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }

    protected virtual void Content_ProblemDataChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    protected virtual void Content_ModelChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        variableImactsArrayView.Content = null;
      } else {
        UpdateVariableImpact();
      }
    }

    private void RegressionSolutionVariableImpactsView_VisibleChanged(object sender, EventArgs e) {
      if (!worker.IsBusy) { return; }
      worker.CancelAsync();
    }


    private void dataPartitionComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateVariableImpact();
    }

    private void replacementComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateVariableImpact();
    }

    private void sortByComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      //Update the default ordering (asc,desc), but remove the eventHandler beforehand (otherwise the data would be ordered twice)
      ascendingCheckBox.CheckedChanged -= ascendingCheckBox_CheckedChanged;
      switch ((SortingCriteria)sortByComboBox.SelectedItem) {
        case SortingCriteria.ImpactValue:
          ascendingCheckBox.Checked = false;
          break;
        case SortingCriteria.Occurrence:
          ascendingCheckBox.Checked = true;
          break;
        case SortingCriteria.VariableName:
          ascendingCheckBox.Checked = true;
          break;
        default:
          throw new NotImplementedException("Ordering for selected SortingCriteria not implemented");
      }
      ascendingCheckBox.CheckedChanged += ascendingCheckBox_CheckedChanged;

      UpdateDataOrdering();
    }

    private void ascendingCheckBox_CheckedChanged(object sender, EventArgs e) {
      UpdateDataOrdering();
    }


    private void UpdateVariableImpact() {
      //Check if the selection is valid
      if (Content == null) { return; }
      if (replacementComboBox.SelectedIndex < 0) { return; }
      if (dataPartitionComboBox.SelectedIndex < 0) { return; }
      if (factorVarReplComboBox.SelectedIndex < 0) { return; }

      //Prepare arguments
      var mainForm = (MainForm.WindowsForms.MainForm)MainFormManager.MainForm;
      var replMethod = (RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum)replacementComboBox.Items[replacementComboBox.SelectedIndex];
      var factorReplMethod = (RegressionSolutionVariableImpactsCalculator.FactorReplacementMethodEnum)factorVarReplComboBox.Items[factorVarReplComboBox.SelectedIndex];
      var dataPartition = (RegressionSolutionVariableImpactsCalculator.DataPartitionEnum)dataPartitionComboBox.SelectedItem;

      variableImactsArrayView.Caption = Content.Name + " Variable Impacts";
      progress = mainForm.AddOperationProgressToView(this, "Calculating variable impacts for " + Content.Name);
      progress.ProgressValue = 0;

      if (!worker.IsBusy) {
        worker.RunWorkerAsync(new BackgroundworkerArguments() {
          mainForm = mainForm,
          dataPartition = dataPartition,
          factorReplMethod = factorReplMethod,
          replMethod = replMethod
        });
      }
    }

    /// <summary>
    /// Updates the <see cref="variableImactsArrayView"/> according to the selected ordering <see cref="ascendingCheckBox"/> of the selected Column <see cref="sortByComboBox"/>
    /// The default is "Descending" by "VariableImpact" (as in previous versions)
    /// </summary>
    private void UpdateDataOrdering() {
      //Check if valid sortingCriteria is selected and data exists
      if (sortByComboBox.SelectedIndex == -1) { return; }
      if (rawVariableImpacts == null) { return; }
      if (!rawVariableImpacts.Any()) { return; }

      var selectedItem = (SortingCriteria)sortByComboBox.SelectedItem;
      bool ascending = ascendingCheckBox.Checked;

      IEnumerable<KeyValuePair<string, double>> orderedEntries = null;

      //Sort accordingly
      switch (selectedItem) {
        case SortingCriteria.ImpactValue:
          orderedEntries = rawVariableImpacts.OrderBy(v => v.Value);
          break;
        case SortingCriteria.Occurrence:
          orderedEntries = rawVariableImpacts;
          break;
        case SortingCriteria.VariableName:
          orderedEntries = rawVariableImpacts.OrderBy(v => v.Key, new NaturalStringComparer());
          break;
        default:
          throw new NotImplementedException("Ordering for selected SortingCriteria not implemented");
      }

      if (!ascending) { orderedEntries = orderedEntries.Reverse(); }

      //Write the data back
      var impactArray = new DoubleArray(orderedEntries.Select(i => i.Value).ToArray()) {
        ElementNames = orderedEntries.Select(i => i.Key)
      };

      //Could be, if the View was closed
      if (!variableImactsArrayView.IsDisposed) {
        variableImactsArrayView.Content = (DoubleArray)impactArray.AsReadOnly();
      }
    }
  }
}
