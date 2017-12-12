#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Classification;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("RF Model Evaluation")]
  [Content(typeof(IRandomForestRegressionSolution), false)]
  [Content(typeof(IRandomForestClassificationSolution), false)]
  public partial class RandomForestModelEvaluationView : DataAnalysisSolutionEvaluationView {

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      listBox.Enabled = Content != null;
      viewHost.Enabled = Content != null;
    }

    public RandomForestModelEvaluationView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        viewHost.Content = null;
        listBox.Items.Clear();
      } else {
        viewHost.Content = null;
        listBox.Items.Clear();
        var classSol = Content as IRandomForestClassificationSolution;
        var regSol = Content as IRandomForestRegressionSolution;
        var numTrees = classSol != null ? classSol.NumberOfTrees : regSol != null ? regSol.NumberOfTrees : 0;
        for (int i = 0; i < numTrees; i++) {
          listBox.Items.Add(i + 1);
        }
      }
    }

    private void listBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (listBox.SelectedItem == null) viewHost.Content = null;
      else {
        var idx = (int)listBox.SelectedItem;
        viewHost.Content = CreateModel(idx);
      }
    }

    private void listBox_DoubleClick(object sender, System.EventArgs e) {
      var selectedItem = listBox.SelectedItem;
      if (selectedItem == null) return;
      var idx = (int)listBox.SelectedItem;
      MainFormManager.MainForm.ShowContent(CreateModel(idx));
    }

    private IContent CreateModel(int idx) {
      idx -= 1;
      var rfModel = Content.Model as RandomForestModel;
      if (rfModel == null) return null;
      var regProblemData = Content.ProblemData as IRegressionProblemData;
      var classProblemData = Content.ProblemData as IClassificationProblemData;
      if (idx < 0 || idx >= rfModel.NumberOfTrees)
        return null;
      if (regProblemData != null) {
        var syModel = new SymbolicRegressionModel(regProblemData.TargetVariable, rfModel.ExtractTree(idx),
          new SymbolicDataAnalysisExpressionTreeLinearInterpreter());
        return syModel.CreateRegressionSolution(regProblemData);
      } else if (classProblemData != null) {
        var syModel = new SymbolicDiscriminantFunctionClassificationModel(classProblemData.TargetVariable, rfModel.ExtractTree(idx),
          new SymbolicDataAnalysisExpressionTreeLinearInterpreter(), new NormalDistributionCutPointsThresholdCalculator());
        syModel.RecalculateModelParameters(classProblemData, classProblemData.TrainingIndices);
        return syModel.CreateClassificationSolution(classProblemData);
      } else throw new InvalidProgramException();
    }
  }
}
