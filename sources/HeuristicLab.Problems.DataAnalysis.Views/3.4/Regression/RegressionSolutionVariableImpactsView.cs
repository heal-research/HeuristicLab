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
using System.Linq;
using System.Threading.Tasks;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Variable Impacts")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionVariableImpactsView : DataAnalysisSolutionEvaluationView {

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set {
        base.Content = value;
      }
    }

    public RegressionSolutionVariableImpactsView()
      : base() {
      InitializeComponent();
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

    private void UpdateVariableImpacts() {
      if (Content == null || replacementComboBox.SelectedIndex < 0
        || factorVarReplComboBox.SelectedIndex < 0
        || dataPartitionComboBox.SelectedIndex < 0) return;
      var mainForm = (MainForm.WindowsForms.MainForm)MainFormManager.MainForm;
      variableImactsArrayView.Caption = Content.Name + " Variable Impacts";
      var replMethod =
         (RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum)
           replacementComboBox.Items[replacementComboBox.SelectedIndex];
      var factorReplMethod =
        (RegressionSolutionVariableImpactsCalculator.FactorReplacementMethodEnum)
          factorVarReplComboBox.Items[factorVarReplComboBox.SelectedIndex];
      var dataPartition =
        (RegressionSolutionVariableImpactsCalculator.DataPartitionEnum)dataPartitionComboBox.SelectedItem;

      Task.Factory.StartNew(() => {
        try {
          mainForm.AddOperationProgressToView(this, "Calculating variable impacts for " + Content.Name);

          var impacts = RegressionSolutionVariableImpactsCalculator.CalculateImpacts(Content, dataPartition, replMethod, factorReplMethod);
          var impactArray = new DoubleArray(impacts.Select(i => i.Item2).ToArray());
          impactArray.ElementNames = impacts.Select(i => i.Item1);
          variableImactsArrayView.Content = (DoubleArray)impactArray.AsReadOnly();
        } finally {
          mainForm.RemoveOperationProgressFromView(this);
        }
      });
    }

    #endregion

    private void dataPartitionComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateVariableImpacts();
    }

    private void replacementComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateVariableImpacts();
    }
  }
}
