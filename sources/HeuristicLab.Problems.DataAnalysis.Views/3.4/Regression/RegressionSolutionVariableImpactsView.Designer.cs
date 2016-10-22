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
namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class RegressionSolutionVariableImpactsView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.variableImactsArrayView = new HeuristicLab.Data.Views.StringConvertibleArrayView();
      this.dataPartitionComboBox = new System.Windows.Forms.ComboBox();
      this.dataPartitionLabel = new System.Windows.Forms.Label();
      this.replacementLabel = new System.Windows.Forms.Label();
      this.replacementComboBox = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // variableImactsArrayView
      // 
      this.variableImactsArrayView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.variableImactsArrayView.Caption = "StringConvertibleArray View";
      this.variableImactsArrayView.Content = null;
      this.variableImactsArrayView.Location = new System.Drawing.Point(3, 59);
      this.variableImactsArrayView.Name = "variableImactsArrayView";
      this.variableImactsArrayView.ReadOnly = true;
      this.variableImactsArrayView.Size = new System.Drawing.Size(304, 223);
      this.variableImactsArrayView.TabIndex = 4;
      // 
      // dataPartitionComboBox
      // 
      this.dataPartitionComboBox.FormattingEnabled = true;
      this.dataPartitionComboBox.Items.AddRange(new object[] {
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.DataPartitionEnum.Training,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.DataPartitionEnum.Test,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.DataPartitionEnum.All});
      this.dataPartitionComboBox.Location = new System.Drawing.Point(82, 3);
      this.dataPartitionComboBox.Name = "dataPartitionComboBox";
      this.dataPartitionComboBox.Size = new System.Drawing.Size(121, 21);
      this.dataPartitionComboBox.TabIndex = 1;
      this.dataPartitionComboBox.SelectedIndexChanged += new System.EventHandler(this.dataPartitionComboBox_SelectedIndexChanged);
      // 
      // dataPartitionLabel
      // 
      this.dataPartitionLabel.AutoSize = true;
      this.dataPartitionLabel.Location = new System.Drawing.Point(3, 6);
      this.dataPartitionLabel.Name = "dataPartitionLabel";
      this.dataPartitionLabel.Size = new System.Drawing.Size(73, 13);
      this.dataPartitionLabel.TabIndex = 0;
      this.dataPartitionLabel.Text = "Data partition:";
      // 
      // replacementLabel
      // 
      this.replacementLabel.AutoSize = true;
      this.replacementLabel.Location = new System.Drawing.Point(3, 35);
      this.replacementLabel.Name = "replacementLabel";
      this.replacementLabel.Size = new System.Drawing.Size(73, 13);
      this.replacementLabel.TabIndex = 2;
      this.replacementLabel.Text = "Replacement:";
      // 
      // replacementComboBox
      // 
      this.replacementComboBox.FormattingEnabled = true;
      this.replacementComboBox.Items.AddRange(new object[] {
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum.Median,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum.Average,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum.Noise,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum.Shuffle});
      this.replacementComboBox.Location = new System.Drawing.Point(82, 32);
      this.replacementComboBox.Name = "replacementComboBox";
      this.replacementComboBox.Size = new System.Drawing.Size(121, 21);
      this.replacementComboBox.TabIndex = 3;
      this.replacementComboBox.SelectedIndexChanged += new System.EventHandler(this.replacementComboBox_SelectedIndexChanged);
      // 
      // RegressionSolutionVariableImpactsView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.replacementComboBox);
      this.Controls.Add(this.replacementLabel);
      this.Controls.Add(this.dataPartitionLabel);
      this.Controls.Add(this.dataPartitionComboBox);
      this.Controls.Add(this.variableImactsArrayView);
      this.Name = "RegressionSolutionVariableImpactsView";
      this.Size = new System.Drawing.Size(310, 285);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Data.Views.StringConvertibleArrayView variableImactsArrayView;
    private System.Windows.Forms.ComboBox dataPartitionComboBox;
    private System.Windows.Forms.Label dataPartitionLabel;
    private System.Windows.Forms.Label replacementLabel;
    private System.Windows.Forms.ComboBox replacementComboBox;
  }
}
