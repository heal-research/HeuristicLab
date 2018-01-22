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
      this.ascendingCheckBox = new System.Windows.Forms.CheckBox();
      this.sortByLabel = new System.Windows.Forms.Label();
      this.sortByComboBox = new System.Windows.Forms.ComboBox();
      this.factorVarReplComboBox = new System.Windows.Forms.ComboBox();
      this.factorVarReplacementLabel = new System.Windows.Forms.Label();
      this.replacementComboBox = new System.Windows.Forms.ComboBox();
      this.numericVarReplacementLabel = new System.Windows.Forms.Label();
      this.dataPartitionLabel = new System.Windows.Forms.Label();
      this.dataPartitionComboBox = new System.Windows.Forms.ComboBox();
      this.variableImactsArrayView = new HeuristicLab.Data.Views.StringConvertibleArrayView();
      this.SuspendLayout();
      // 
      // ascendingCheckBox
      // 
      this.ascendingCheckBox.AutoSize = true;
      this.ascendingCheckBox.Location = new System.Drawing.Point(534, 6);
      this.ascendingCheckBox.Name = "ascendingCheckBox";
      this.ascendingCheckBox.Size = new System.Drawing.Size(76, 17);
      this.ascendingCheckBox.TabIndex = 7;
      this.ascendingCheckBox.Text = "Ascending";
      this.ascendingCheckBox.UseVisualStyleBackColor = true;
      this.ascendingCheckBox.CheckedChanged += new System.EventHandler(this.ascendingCheckBox_CheckedChanged);
      // 
      // sortByLabel
      // 
      this.sortByLabel.AutoSize = true;
      this.sortByLabel.Location = new System.Drawing.Point(324, 6);
      this.sortByLabel.Name = "sortByLabel";
      this.sortByLabel.Size = new System.Drawing.Size(77, 13);
      this.sortByLabel.TabIndex = 4;
      this.sortByLabel.Text = "Sorting criteria:";
      // 
      // sortByComboBox
      // 
      this.sortByComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.sortByComboBox.FormattingEnabled = true;
      this.sortByComboBox.Items.AddRange(new object[] {
            "Impact Value",
            "Occurrence",
            "Variable Name"});
      this.sortByComboBox.Location = new System.Drawing.Point(407, 3);
      this.sortByComboBox.Name = "sortByComboBox";
      this.sortByComboBox.Size = new System.Drawing.Size(121, 21);
      this.sortByComboBox.TabIndex = 5;
      this.sortByComboBox.SelectedIndexChanged += new System.EventHandler(this.sortByComboBox_SelectedIndexChanged);
      // 
      // factorVarReplComboBox
      // 
      this.factorVarReplComboBox.FormattingEnabled = true;
      this.factorVarReplComboBox.Items.AddRange(new object[] {
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.FactorReplacementMethodEnum.Best,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.FactorReplacementMethodEnum.Mode,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.FactorReplacementMethodEnum.Shuffle});
      this.factorVarReplComboBox.Location = new System.Drawing.Point(197, 57);
      this.factorVarReplComboBox.Name = "factorVarReplComboBox";
      this.factorVarReplComboBox.Size = new System.Drawing.Size(121, 21);
      this.factorVarReplComboBox.TabIndex = 1;
      this.factorVarReplComboBox.SelectedIndexChanged += new System.EventHandler(this.replacementComboBox_SelectedIndexChanged);
      // 
      // factorVarReplacementLabel
      // 
      this.factorVarReplacementLabel.AutoSize = true;
      this.factorVarReplacementLabel.Location = new System.Drawing.Point(3, 60);
      this.factorVarReplacementLabel.Name = "factorVarReplacementLabel";
      this.factorVarReplacementLabel.Size = new System.Drawing.Size(188, 13);
      this.factorVarReplacementLabel.TabIndex = 0;
      this.factorVarReplacementLabel.Text = "Replacement for categorical variables:";
      // 
      // replacementComboBox
      // 
      this.replacementComboBox.FormattingEnabled = true;
      this.replacementComboBox.Items.AddRange(new object[] {
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum.Median,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum.Average,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum.Noise,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.ReplacementMethodEnum.Shuffle});
      this.replacementComboBox.Location = new System.Drawing.Point(197, 30);
      this.replacementComboBox.Name = "replacementComboBox";
      this.replacementComboBox.Size = new System.Drawing.Size(121, 21);
      this.replacementComboBox.TabIndex = 3;
      this.replacementComboBox.SelectedIndexChanged += new System.EventHandler(this.replacementComboBox_SelectedIndexChanged);
      // 
      // numericVarReplacementLabel
      // 
      this.numericVarReplacementLabel.AutoSize = true;
      this.numericVarReplacementLabel.Location = new System.Drawing.Point(3, 33);
      this.numericVarReplacementLabel.Name = "numericVarReplacementLabel";
      this.numericVarReplacementLabel.Size = new System.Drawing.Size(173, 13);
      this.numericVarReplacementLabel.TabIndex = 2;
      this.numericVarReplacementLabel.Text = "Replacement for numeric variables:";
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
      // dataPartitionComboBox
      // 
      this.dataPartitionComboBox.FormattingEnabled = true;
      this.dataPartitionComboBox.Items.AddRange(new object[] {
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.DataPartitionEnum.Training,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.DataPartitionEnum.Test,
            HeuristicLab.Problems.DataAnalysis.RegressionSolutionVariableImpactsCalculator.DataPartitionEnum.All});
      this.dataPartitionComboBox.Location = new System.Drawing.Point(197, 3);
      this.dataPartitionComboBox.Name = "dataPartitionComboBox";
      this.dataPartitionComboBox.Size = new System.Drawing.Size(121, 21);
      this.dataPartitionComboBox.TabIndex = 1;
      this.dataPartitionComboBox.SelectedIndexChanged += new System.EventHandler(this.dataPartitionComboBox_SelectedIndexChanged);
      // 
      // variableImactsArrayView
      // 
      this.variableImactsArrayView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.variableImactsArrayView.Caption = "StringConvertibleArray View";
      this.variableImactsArrayView.Content = null;
      this.variableImactsArrayView.Location = new System.Drawing.Point(3, 84);
      this.variableImactsArrayView.Name = "variableImactsArrayView";
      this.variableImactsArrayView.ReadOnly = true;
      this.variableImactsArrayView.Size = new System.Drawing.Size(706, 278);
      this.variableImactsArrayView.TabIndex = 2;
      // 
      // RegressionSolutionVariableImpactsView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.ascendingCheckBox);
      this.Controls.Add(this.sortByLabel);
      this.Controls.Add(this.sortByComboBox);
      this.Controls.Add(this.factorVarReplComboBox);
      this.Controls.Add(this.factorVarReplacementLabel);
      this.Controls.Add(this.replacementComboBox);
      this.Controls.Add(this.numericVarReplacementLabel);
      this.Controls.Add(this.dataPartitionLabel);
      this.Controls.Add(this.dataPartitionComboBox);
      this.Controls.Add(this.variableImactsArrayView);
      this.Name = "RegressionSolutionVariableImpactsView";
      this.Size = new System.Drawing.Size(712, 365);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Data.Views.StringConvertibleArrayView variableImactsArrayView;
    private System.Windows.Forms.ComboBox dataPartitionComboBox;
    private System.Windows.Forms.Label dataPartitionLabel;
    private System.Windows.Forms.Label numericVarReplacementLabel;
    private System.Windows.Forms.ComboBox replacementComboBox;
    private System.Windows.Forms.Label factorVarReplacementLabel;
    private System.Windows.Forms.ComboBox factorVarReplComboBox;
    private System.Windows.Forms.Label sortByLabel;
    private System.Windows.Forms.ComboBox sortByComboBox;
    private System.Windows.Forms.CheckBox ascendingCheckBox;
  }
}
