#region License Information

/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

# endregion

namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class ShapeConstraintView {
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
      this.components = new System.ComponentModel.Container();
      this.lowerboundLabel = new System.Windows.Forms.Label();
      this.upperboundLabel = new System.Windows.Forms.Label();
      this.variableLabel = new System.Windows.Forms.Label();
      this.numberOfDerivationLabel = new System.Windows.Forms.Label();
      this.upperboundInput = new System.Windows.Forms.TextBox();
      this.lowerboundInput = new System.Windows.Forms.TextBox();
      this.variableInput = new System.Windows.Forms.TextBox();
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.weightTextBox = new System.Windows.Forms.TextBox();
      this.weightLabel = new System.Windows.Forms.Label();
      this.regionLabel = new System.Windows.Forms.Label();
      this.regionView = new HeuristicLab.Problems.DataAnalysis.Views.IntervalCollectionView();
      this.numberOfDerivationsComboBox = new System.Windows.Forms.ComboBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.groupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // lowerboundLabel
      // 
      this.lowerboundLabel.AutoSize = true;
      this.lowerboundLabel.Location = new System.Drawing.Point(8, 75);
      this.lowerboundLabel.Name = "lowerboundLabel";
      this.lowerboundLabel.Size = new System.Drawing.Size(72, 13);
      this.lowerboundLabel.TabIndex = 4;
      this.lowerboundLabel.Text = "Lower bound:";
      // 
      // upperboundLabel
      // 
      this.upperboundLabel.AutoSize = true;
      this.upperboundLabel.Location = new System.Drawing.Point(8, 101);
      this.upperboundLabel.Name = "upperboundLabel";
      this.upperboundLabel.Size = new System.Drawing.Size(72, 13);
      this.upperboundLabel.TabIndex = 6;
      this.upperboundLabel.Text = "Upper bound:";
      // 
      // variableLabel
      // 
      this.variableLabel.AutoSize = true;
      this.variableLabel.Location = new System.Drawing.Point(8, 22);
      this.variableLabel.Name = "variableLabel";
      this.variableLabel.Size = new System.Drawing.Size(48, 13);
      this.variableLabel.TabIndex = 0;
      this.variableLabel.Text = "Variable:";
      // 
      // numberderivationLabel
      // 
      this.numberOfDerivationLabel.AutoSize = true;
      this.numberOfDerivationLabel.Location = new System.Drawing.Point(8, 48);
      this.numberOfDerivationLabel.Name = "numberderivationLabel";
      this.numberOfDerivationLabel.Size = new System.Drawing.Size(58, 13);
      this.numberOfDerivationLabel.TabIndex = 2;
      this.numberOfDerivationLabel.Text = "Derivative:";
      // 
      // upperboundInput
      // 
      this.upperboundInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.upperboundInput.Location = new System.Drawing.Point(86, 98);
      this.upperboundInput.Name = "upperboundInput";
      this.upperboundInput.Size = new System.Drawing.Size(495, 20);
      this.upperboundInput.TabIndex = 7;
      this.upperboundInput.Validating += new System.ComponentModel.CancelEventHandler(this.upperboundInput_Validating);
      this.upperboundInput.Validated += new System.EventHandler(this.upperboundInput_Validated);
      // 
      // lowerboundInput
      // 
      this.lowerboundInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lowerboundInput.Location = new System.Drawing.Point(86, 72);
      this.lowerboundInput.Name = "lowerboundInput";
      this.lowerboundInput.Size = new System.Drawing.Size(495, 20);
      this.lowerboundInput.TabIndex = 5;
      this.lowerboundInput.Validating += new System.ComponentModel.CancelEventHandler(this.lowerboundInput_Validating);
      this.lowerboundInput.Validated += new System.EventHandler(this.lowerboundInput_Validated);
      // 
      // variableInput
      // 
      this.variableInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.variableInput.Location = new System.Drawing.Point(86, 19);
      this.variableInput.Name = "variableInput";
      this.variableInput.Size = new System.Drawing.Size(495, 20);
      this.variableInput.TabIndex = 1;
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.weightTextBox);
      this.groupBox.Controls.Add(this.weightLabel);
      this.groupBox.Controls.Add(this.regionLabel);
      this.groupBox.Controls.Add(this.regionView);
      this.groupBox.Controls.Add(this.numberOfDerivationsComboBox);
      this.groupBox.Controls.Add(this.variableInput);
      this.groupBox.Controls.Add(this.lowerboundInput);
      this.groupBox.Controls.Add(this.upperboundInput);
      this.groupBox.Controls.Add(this.numberOfDerivationLabel);
      this.groupBox.Controls.Add(this.variableLabel);
      this.groupBox.Controls.Add(this.upperboundLabel);
      this.groupBox.Controls.Add(this.lowerboundLabel);
      this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox.Location = new System.Drawing.Point(0, 0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(587, 380);
      this.groupBox.TabIndex = 0;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "Shape Constraint Detail";
      // 
      // weightInput
      // 
      this.weightTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.weightTextBox.Location = new System.Drawing.Point(86, 124);
      this.weightTextBox.Name = "weightInput";
      this.weightTextBox.Size = new System.Drawing.Size(495, 20);
      this.weightTextBox.TabIndex = 9;
      this.weightTextBox.TextChanged += new System.EventHandler(this.weightInput_TextChanged);
      // 
      // WeightLab
      // 
      this.weightLabel.AutoSize = true;
      this.weightLabel.Location = new System.Drawing.Point(8, 127);
      this.weightLabel.Name = "WeightLab";
      this.weightLabel.Size = new System.Drawing.Size(44, 13);
      this.weightLabel.TabIndex = 8;
      this.weightLabel.Text = "Weight:";
      // 
      // regionLab
      // 
      this.regionLabel.AutoSize = true;
      this.regionLabel.Location = new System.Drawing.Point(8, 150);
      this.regionLabel.Name = "regionLab";
      this.regionLabel.Size = new System.Drawing.Size(49, 13);
      this.regionLabel.TabIndex = 10;
      this.regionLabel.Text = "Regions:";
      // 
      // regionView
      // 
      this.regionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.regionView.Caption = "IntervalCollection View";
      this.regionView.Content = null;
      this.regionView.Location = new System.Drawing.Point(86, 150);
      this.regionView.Name = "regionView";
      this.regionView.ReadOnly = false;
      this.regionView.Size = new System.Drawing.Size(495, 224);
      this.regionView.TabIndex = 11;
      this.regionView.TabStop = false;
      // 
      // numberderivationInput
      // 
      this.numberOfDerivationsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.numberOfDerivationsComboBox.FormattingEnabled = true;
      this.numberOfDerivationsComboBox.Location = new System.Drawing.Point(86, 45);
      this.numberOfDerivationsComboBox.Name = "numberderivationInput";
      this.numberOfDerivationsComboBox.Size = new System.Drawing.Size(495, 21);
      this.numberOfDerivationsComboBox.TabIndex = 3;
      this.numberOfDerivationsComboBox.SelectedIndexChanged += new System.EventHandler(this.numberderivationInput_SelectedIndexChanged);
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      this.errorProvider.RightToLeft = true;
      // 
      // ShapeConstraintView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.groupBox);
      this.Name = "ShapeConstraintView";
      this.Size = new System.Drawing.Size(587, 380);
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lowerboundLabel;
    private System.Windows.Forms.Label upperboundLabel;
    private System.Windows.Forms.Label variableLabel;
    private System.Windows.Forms.Label numberOfDerivationLabel;
    private System.Windows.Forms.TextBox upperboundInput;
    private System.Windows.Forms.TextBox lowerboundInput;
    private System.Windows.Forms.TextBox variableInput;
    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.ComboBox numberOfDerivationsComboBox;
    private IntervalCollectionView regionView;
    private System.Windows.Forms.Label regionLabel;
    private System.Windows.Forms.TextBox weightTextBox;
    private System.Windows.Forms.Label weightLabel;
  }
}
