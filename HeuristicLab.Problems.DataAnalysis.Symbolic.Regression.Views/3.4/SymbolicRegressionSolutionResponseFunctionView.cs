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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression.Views {
  [View("Response Function View")]
  [Content(typeof(ISymbolicRegressionSolution), false)]
  public partial class SymbolicRegressionSolutionResponseFunctionView : ItemView {
    private Dictionary<string, List<ISymbolicExpressionTreeNode>> variableNodes;
    private ISymbolicExpressionTree clonedTree;
    private Dictionary<string, double> medianValues;
    public SymbolicRegressionSolutionResponseFunctionView() {
      InitializeComponent();
      variableNodes = new Dictionary<string, List<ISymbolicExpressionTreeNode>>();
      medianValues = new Dictionary<string, double>();
      Caption = "Response Function View";
    }

    public new ISymbolicRegressionSolution Content {
      get { return (ISymbolicRegressionSolution)base.Content; }
      set { base.Content = value; }
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

    private void Content_ModelChanged(object sender, EventArgs e) {
      OnModelChanged();
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      OnProblemDataChanged();
    }

    protected virtual void OnModelChanged() {
      this.UpdateView();
    }

    protected virtual void OnProblemDataChanged() {
      this.UpdateView();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.UpdateView();
    }

    private void UpdateView() {
      if (Content != null && Content.Model != null && Content.ProblemData != null) {
        var referencedVariables =
       (from varNode in Content.Model.SymbolicExpressionTree.IterateNodesPrefix().OfType<VariableTreeNode>()
        select varNode.VariableName)
         .Distinct()
         .OrderBy(x => x, new NaturalStringComparer())
         .ToList();

        medianValues.Clear();
        foreach (var variableName in referencedVariables) {
          medianValues.Add(variableName, Content.ProblemData.Dataset.GetDoubleValues(variableName).Median());
        }

        comboBox.Items.Clear();
        comboBox.Items.AddRange(referencedVariables.ToArray());
        comboBox.SelectedIndex = 0;
      }
    }

    private void CreateSliders(IEnumerable<string> variableNames) {
      flowLayoutPanel.Controls.Clear();

      foreach (var variableName in variableNames) {
        var variableTrackbar = new VariableTrackbar(variableName,
                                                    Content.ProblemData.Dataset.GetDoubleValues(variableName));
        variableTrackbar.Size = new Size(variableTrackbar.Size.Width, flowLayoutPanel.Size.Height - 23);
        variableTrackbar.ValueChanged += TrackBarValueChanged;
        flowLayoutPanel.Controls.Add(variableTrackbar);
      }
    }

    private void TrackBarValueChanged(object sender, EventArgs e) {
      var trackBar = (VariableTrackbar)sender;
      string variableName = trackBar.VariableName;
      ChangeVariableValue(variableName, trackBar.Value);
    }

    private void ChangeVariableValue(string variableName, double value) {
      foreach (var constNode in variableNodes[variableName].Cast<ConstantTreeNode>())
        constNode.Value = value;

      UpdateResponseSeries();
    }

    private void UpdateScatterPlot() {
      string freeVariable = (string)comboBox.SelectedItem;
      IEnumerable<string> fixedVariables = comboBox.Items.OfType<string>()
        .Except(new string[] { freeVariable });

      // scatter plots for subset of samples that have values near the median values for all variables
      Func<int, bool> NearMedianValue = (r) => {
        foreach (var fixedVar in fixedVariables) {
          double med = medianValues[fixedVar];
          if (!(Content.ProblemData.Dataset.GetDoubleValue(fixedVar, r) < med + 0.1 * Math.Abs(med) &&
            Content.ProblemData.Dataset.GetDoubleValue(fixedVar, r) > med - 0.1 * Math.Abs(med)))
            return false;
        }
        return true;
      };

      var mainTrainingIndices = (from row in Content.ProblemData.TrainingIndices
                                 where NearMedianValue(row)
                                 select row)
        .ToArray();
      var mainTestIndices = (from row in Content.ProblemData.TestIndices
                             where NearMedianValue(row)
                             select row)
        .ToArray();

      var freeVariableValues = Content.ProblemData.Dataset.GetDoubleValues(freeVariable, mainTrainingIndices).ToArray();
      var trainingValues = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable,
                                                                     mainTrainingIndices).ToArray();
      Array.Sort(freeVariableValues, trainingValues);
      responseChart.Series["Training Data"].Points.DataBindXY(freeVariableValues, trainingValues);

      freeVariableValues = Content.ProblemData.Dataset.GetDoubleValues(freeVariable, mainTestIndices).ToArray();
      var testValues = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable,
                                                                     mainTestIndices).ToArray();
      Array.Sort(freeVariableValues, testValues);
      responseChart.Series["Test Data"].Points.DataBindXY(freeVariableValues, testValues);

      // draw scatter plots of remaining values
      freeVariableValues = Content.ProblemData.Dataset.GetDoubleValues(freeVariable, Content.ProblemData.TrainingIndices).ToArray();
      trainingValues = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable,
                                                                     Content.ProblemData.TrainingIndices).ToArray();
      Array.Sort(freeVariableValues, trainingValues);
      responseChart.Series["Training Data (edge)"].Points.DataBindXY(freeVariableValues, trainingValues);

      freeVariableValues = Content.ProblemData.Dataset.GetDoubleValues(freeVariable, Content.ProblemData.TestIndices).ToArray();
      testValues = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable,
                                                                     Content.ProblemData.TestIndices).ToArray();
      Array.Sort(freeVariableValues, testValues);
      responseChart.Series["Test Data (edge)"].Points.DataBindXY(freeVariableValues, testValues);



      responseChart.ChartAreas[0].AxisX.Maximum = Math.Ceiling(freeVariableValues.Max());
      responseChart.ChartAreas[0].AxisX.Minimum = Math.Floor(freeVariableValues.Min());
      responseChart.ChartAreas[0].AxisY.Maximum = Math.Ceiling(Math.Max(testValues.Max(), trainingValues.Max()));
      responseChart.ChartAreas[0].AxisY.Minimum = Math.Floor(Math.Min(testValues.Min(), trainingValues.Min()));
    }

    private void UpdateResponseSeries() {
      string freeVariable = (string)comboBox.SelectedItem;

      var freeVariableValues = Content.ProblemData.Dataset.GetDoubleValues(freeVariable, Content.ProblemData.TrainingIndices).ToArray();
      var responseValues = Content.Model.Interpreter.GetSymbolicExpressionTreeValues(clonedTree,
                                                                              Content.ProblemData.Dataset,
                                                                              Content.ProblemData.TrainingIndices)
                                                                              .ToArray();
      Array.Sort(freeVariableValues, responseValues);
      responseChart.Series["Model Response"].Points.DataBindXY(freeVariableValues, responseValues);
    }

    private void ComboBoxSelectedIndexChanged(object sender, EventArgs e) {
      string freeVariable = (string)comboBox.SelectedItem;
      IEnumerable<string> fixedVariables = comboBox.Items.OfType<string>()
        .Except(new string[] { freeVariable });

      variableNodes.Clear();
      clonedTree = (ISymbolicExpressionTree)Content.Model.SymbolicExpressionTree.Clone();

      foreach (var varNode in clonedTree.IterateNodesPrefix().OfType<VariableTreeNode>()) {
        if (fixedVariables.Contains(varNode.VariableName)) {
          if (!variableNodes.ContainsKey(varNode.VariableName))
            variableNodes.Add(varNode.VariableName, new List<ISymbolicExpressionTreeNode>());

          int childIndex = varNode.Parent.IndexOfSubtree(varNode);
          var replacementNode = MakeConstantTreeNode(medianValues[varNode.VariableName]);
          var parent = varNode.Parent;
          parent.RemoveSubtree(childIndex);
          parent.InsertSubtree(childIndex, MakeProduct(replacementNode, varNode.Weight));
          variableNodes[varNode.VariableName].Add(replacementNode);
        }
      }

      CreateSliders(fixedVariables);
      UpdateScatterPlot();
      UpdateResponseSeries();
    }

    private ISymbolicExpressionTreeNode MakeProduct(ConstantTreeNode c, double weight) {
      var mul = new Multiplication();
      var prod = mul.CreateTreeNode();
      prod.AddSubtree(MakeConstantTreeNode(weight));
      prod.AddSubtree(c);
      return prod;
    }

    private ConstantTreeNode MakeConstantTreeNode(double value) {
      Constant constant = new Constant();
      constant.MinValue = value - 1;
      constant.MaxValue = value + 1;
      ConstantTreeNode constantTreeNode = (ConstantTreeNode)constant.CreateTreeNode();
      constantTreeNode.Value = value;
      return constantTreeNode;
    }
  }
}
