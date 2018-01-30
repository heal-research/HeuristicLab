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
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Variable Impacts")]
  [Content(typeof(IClassificationSolution))]
  public partial class ClassificationSolutionVariableImpactsView : DataAnalysisSolutionEvaluationView {
    private Dictionary<string, double> rawVariableImpacts = new Dictionary<string, double>();

    public new IClassificationSolution Content {
      get { return (IClassificationSolution)base.Content; }
      set {
        base.Content = value;
      }
    }

    public ClassificationSolutionVariableImpactsView()
      : base() {
      InitializeComponent();

      //Little workaround. If you fill the ComboBox-Items in the other partial class, the UI-Designer will moan.
      this.sortByComboBox.Items.AddRange(Enum.GetValues(typeof(ClassificationSolutionVariableImpactsCalculator.SortingCriteria)).Cast<object>().ToArray());
      this.sortByComboBox.SelectedItem = ClassificationSolutionVariableImpactsCalculator.SortingCriteria.ImpactValue;

      this.dataPartitionComboBox.SelectedIndex = 0;
      this.replacementComboBox.SelectedIndex = 0;
      this.factorVarReplComboBox.SelectedIndex = 0;
    }

    #region events
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
        UpdateVariableImpacts();
      }
    }


    private void dataPartitionComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateVariableImpacts();
    }

    private void replacementComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateVariableImpacts();
    }

    private void sortByComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      //Update the default ordering (asc,desc), but remove the eventHandler beforehand (otherwise the data would be ordered twice)
      ascendingCheckBox.CheckedChanged -= ascendingCheckBox_CheckedChanged;
      switch ((ClassificationSolutionVariableImpactsCalculator.SortingCriteria)sortByComboBox.SelectedItem) {
        case ClassificationSolutionVariableImpactsCalculator.SortingCriteria.ImpactValue:
          ascendingCheckBox.Checked = false;
          break;
        case ClassificationSolutionVariableImpactsCalculator.SortingCriteria.Occurrence:
          ascendingCheckBox.Checked = true;
          break;
        case ClassificationSolutionVariableImpactsCalculator.SortingCriteria.VariableName:
          ascendingCheckBox.Checked = true;
          break;
        default:
          throw new Exception("Cannot interpret SortingCriteria");
      }
      ascendingCheckBox.CheckedChanged += ascendingCheckBox_CheckedChanged;

      UpdateDataOrdering();
    }

    private void ascendingCheckBox_CheckedChanged(object sender, EventArgs e) {
      UpdateDataOrdering();
    }
    #endregion

    #region Helper Methods
    private void UpdateVariableImpacts() {
      if (Content == null || replacementComboBox.SelectedIndex < 0
        || factorVarReplComboBox.SelectedIndex < 0
        || dataPartitionComboBox.SelectedIndex < 0) return;
      var mainForm = (MainForm.WindowsForms.MainForm)MainFormManager.MainForm;
      variableImactsArrayView.Caption = Content.Name + " Variable Impacts";
      var replMethod =
         (ClassificationSolutionVariableImpactsCalculator.ReplacementMethodEnum)
           replacementComboBox.Items[replacementComboBox.SelectedIndex];
      var factorReplMethod =
        (ClassificationSolutionVariableImpactsCalculator.FactorReplacementMethodEnum)
          factorVarReplComboBox.Items[factorVarReplComboBox.SelectedIndex];
      var dataPartition =
        (ClassificationSolutionVariableImpactsCalculator.DataPartitionEnum)dataPartitionComboBox.SelectedItem;

      Task.Factory.StartNew(() => {
        try {
          mainForm.AddOperationProgressToView(this, "Calculating variable impacts for " + Content.Name);

          //Remember the original ordering of the variables
          var impacts = ClassificationSolutionVariableImpactsCalculator.CalculateImpacts(Content, dataPartition, replMethod, factorReplMethod);
          var problemData = Content.ProblemData;
          var inputvariables = new HashSet<string>(problemData.AllowedInputVariables.Union(Content.Model.VariablesUsedForPrediction));
          var originalVariableOrdering = problemData.Dataset.VariableNames.Where(v => inputvariables.Contains(v)).Where(problemData.Dataset.VariableHasType<double>).ToList();
          rawVariableImpacts.Clear();
          originalVariableOrdering.ForEach(v => rawVariableImpacts.Add(v, impacts.First(vv => vv.Item1 == v).Item2));
        } finally {
          mainForm.RemoveOperationProgressFromView(this);
          UpdateDataOrdering();
        }
      });
    }

    /// <summary>
    /// Updates the <see cref="variableImactsArrayView"/> according to the selected ordering <see cref="ascendingCheckBox"/> of the selected Column <see cref="sortByComboBox"/>
    /// The default is "Descending" by "VariableImpact" (as in previous versions)
    /// </summary>
    private void UpdateDataOrdering() {
      //Check if valid sortingCriteria is selected and data exists
      if (sortByComboBox.SelectedIndex == -1 || rawVariableImpacts == null || !rawVariableImpacts.Any()) { return; }

      var selectedItem = (ClassificationSolutionVariableImpactsCalculator.SortingCriteria)sortByComboBox.SelectedItem;
      bool ascending = ascendingCheckBox.Checked;

      IEnumerable<KeyValuePair<string, double>> orderedEntries = null;

      //Sort accordingly
      switch (selectedItem) {
        case ClassificationSolutionVariableImpactsCalculator.SortingCriteria.ImpactValue:
          orderedEntries = ascending ? rawVariableImpacts.OrderBy(v => v.Value) : rawVariableImpacts.OrderByDescending(v => v.Value);
          break;
        case ClassificationSolutionVariableImpactsCalculator.SortingCriteria.Occurrence:
          orderedEntries = ascending ? rawVariableImpacts : rawVariableImpacts.Reverse();
          break;
        case ClassificationSolutionVariableImpactsCalculator.SortingCriteria.VariableName:
          orderedEntries = ascending ? rawVariableImpacts.OrderBy(v => v.Key, new NaturalStringComparer()) : rawVariableImpacts.OrderByDescending(v => v.Key, new NaturalStringComparer());
          break;
        default:
          throw new Exception("Cannot interpret SortingCriteria");
      }

      //Write the data back
      var impactArray = new DoubleArray(orderedEntries.Select(i => i.Value).ToArray()) {
        ElementNames = orderedEntries.Select(i => i.Key)
      };
      variableImactsArrayView.Content = (DoubleArray)impactArray.AsReadOnly();
    }
    #endregion   
  }
}
