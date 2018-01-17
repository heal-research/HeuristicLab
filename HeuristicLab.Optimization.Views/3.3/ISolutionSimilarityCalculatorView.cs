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

using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimization.Views {
  [View("ISolutionSimilarityCalculator View")]
  [Content(typeof(ISolutionSimilarityCalculator), true)]
  public partial class ISolutionSimilarityCalculatorView : AsynchronousContentView {
    public ISolutionSimilarityCalculatorView() {
      InitializeComponent();
    }

    public new ISolutionSimilarityCalculator Content {
      get { return (ISolutionSimilarityCalculator)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        solutionVariableNameTextBox.Clear();
        qualityVariableNameTextBox.Clear();
      } else {
        solutionVariableNameTextBox.Text = Content.SolutionVariableName;
        qualityVariableNameTextBox.Text = Content.QualityVariableName;
      }
    }

    private void solutionVariableNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (Content != null && Content.SolutionVariableName != solutionVariableNameTextBox.Text) {
        Content.SolutionVariableName = solutionVariableNameTextBox.Text;
      }
    }

    private void qualityVariableNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (Content != null && Content.QualityVariableName != qualityVariableNameTextBox.Text) {
        Content.QualityVariableName = qualityVariableNameTextBox.Text;
      }
    }
  }
}
