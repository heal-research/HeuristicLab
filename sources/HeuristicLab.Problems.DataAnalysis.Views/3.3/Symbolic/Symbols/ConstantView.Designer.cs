#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Views.Symbolic.Symbols {
  partial class ConstantView {
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
      this.weightNuLabel = new System.Windows.Forms.Label();
      this.minValueTextBox = new System.Windows.Forms.TextBox();
      this.initializationGroupBox = new System.Windows.Forms.GroupBox();
      this.weightSigmaLabel = new System.Windows.Forms.Label();
      this.maxValueTextBox = new System.Windows.Forms.TextBox();
      this.mutationGroupBox = new System.Windows.Forms.GroupBox();
      this.stdDevWeightChangeLabel = new System.Windows.Forms.Label();
      this.valueChangeSigmaTextBox = new System.Windows.Forms.TextBox();
      this.meanWeightChangeLabel = new System.Windows.Forms.Label();
      this.valueChangeMuTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.initializationGroupBox.SuspendLayout();
      this.mutationGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // initialFrequencyLabel
      // 
      this.toolTip.SetToolTip(this.initialFrequencyLabel, "Relative frequency of the symbol in randomly created trees");
      // 
      // initialFrequencyTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initialFrequencyTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(203, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(203, 20);
      // 
      // descriptionTextBox
      // 
      this.errorProvider.SetIconAlignment(this.descriptionTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.descriptionTextBox.Size = new System.Drawing.Size(203, 20);
      // 
      // weightNuLabel
      // 
      this.weightNuLabel.AutoSize = true;
      this.weightNuLabel.Location = new System.Drawing.Point(6, 22);
      this.weightNuLabel.Name = "weightNuLabel";
      this.weightNuLabel.Size = new System.Drawing.Size(56, 13);
      this.weightNuLabel.TabIndex = 6;
      this.weightNuLabel.Text = "Min value:";
      this.toolTip.SetToolTip(this.weightNuLabel, "The minimal value to use for random initialization of constants.");
      // 
      // minValueTextBox
      // 
      this.minValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.minValueTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.minValueTextBox.Location = new System.Drawing.Point(92, 19);
      this.minValueTextBox.Name = "minValueTextBox";
      this.minValueTextBox.Size = new System.Drawing.Size(222, 20);
      this.minValueTextBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.minValueTextBox, "The minimal value to use for random initialization of constants.");
      this.minValueTextBox.TextChanged += new System.EventHandler(this.minValueTextBox_TextChanged);
      // 
      // initializationGroupBox
      // 
      this.initializationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.initializationGroupBox.Controls.Add(this.weightSigmaLabel);
      this.initializationGroupBox.Controls.Add(this.maxValueTextBox);
      this.initializationGroupBox.Controls.Add(this.weightNuLabel);
      this.initializationGroupBox.Controls.Add(this.minValueTextBox);
      this.initializationGroupBox.Location = new System.Drawing.Point(3, 79);
      this.initializationGroupBox.Name = "initializationGroupBox";
      this.initializationGroupBox.Size = new System.Drawing.Size(320, 73);
      this.initializationGroupBox.TabIndex = 8;
      this.initializationGroupBox.TabStop = false;
      this.initializationGroupBox.Text = "Initialization";
      // 
      // weightSigmaLabel
      // 
      this.weightSigmaLabel.AutoSize = true;
      this.weightSigmaLabel.Location = new System.Drawing.Point(6, 48);
      this.weightSigmaLabel.Name = "weightSigmaLabel";
      this.weightSigmaLabel.Size = new System.Drawing.Size(59, 13);
      this.weightSigmaLabel.TabIndex = 8;
      this.weightSigmaLabel.Text = "Max value:";
      this.toolTip.SetToolTip(this.weightSigmaLabel, "The maximal value to use for random initialization of constants.");
      // 
      // maxValueTextBox
      // 
      this.maxValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.maxValueTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.maxValueTextBox.Location = new System.Drawing.Point(92, 45);
      this.maxValueTextBox.Name = "maxValueTextBox";
      this.maxValueTextBox.Size = new System.Drawing.Size(222, 20);
      this.maxValueTextBox.TabIndex = 9;
      this.toolTip.SetToolTip(this.maxValueTextBox, "The maximal value to use for random initialization of constants.");
      this.maxValueTextBox.TextChanged += new System.EventHandler(this.maxValueTextBox_TextChanged);
      // 
      // mutationGroupBox
      // 
      this.mutationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.mutationGroupBox.Controls.Add(this.stdDevWeightChangeLabel);
      this.mutationGroupBox.Controls.Add(this.valueChangeSigmaTextBox);
      this.mutationGroupBox.Controls.Add(this.meanWeightChangeLabel);
      this.mutationGroupBox.Controls.Add(this.valueChangeMuTextBox);
      this.mutationGroupBox.Location = new System.Drawing.Point(3, 158);
      this.mutationGroupBox.Name = "mutationGroupBox";
      this.mutationGroupBox.Size = new System.Drawing.Size(320, 73);
      this.mutationGroupBox.TabIndex = 9;
      this.mutationGroupBox.TabStop = false;
      this.mutationGroupBox.Text = "Mutation";
      // 
      // stdDevWeightChangeLabel
      // 
      this.stdDevWeightChangeLabel.AutoSize = true;
      this.stdDevWeightChangeLabel.Location = new System.Drawing.Point(6, 48);
      this.stdDevWeightChangeLabel.Name = "stdDevWeightChangeLabel";
      this.stdDevWeightChangeLabel.Size = new System.Drawing.Size(112, 13);
      this.stdDevWeightChangeLabel.TabIndex = 8;
      this.stdDevWeightChangeLabel.Text = "Value change (sigma):";
      this.toolTip.SetToolTip(this.stdDevWeightChangeLabel, "The sigma (std. dev.) parameter for the normal distribution to use to sample the " +
              "value change.");
      // 
      // valueChangeSigmaTextBox
      // 
      this.valueChangeSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.valueChangeSigmaTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.valueChangeSigmaTextBox.Location = new System.Drawing.Point(136, 45);
      this.valueChangeSigmaTextBox.Name = "valueChangeSigmaTextBox";
      this.valueChangeSigmaTextBox.Size = new System.Drawing.Size(178, 20);
      this.valueChangeSigmaTextBox.TabIndex = 9;
      this.toolTip.SetToolTip(this.valueChangeSigmaTextBox, "The sigma (std. dev.) parameter for the normal distribution to use to sample the " +
              "value change.");
      this.valueChangeSigmaTextBox.TextChanged += new System.EventHandler(this.valueChangeSigmaTextBox_TextChanged);
      // 
      // meanWeightChangeLabel
      // 
      this.meanWeightChangeLabel.AutoSize = true;
      this.meanWeightChangeLabel.Location = new System.Drawing.Point(6, 22);
      this.meanWeightChangeLabel.Name = "meanWeightChangeLabel";
      this.meanWeightChangeLabel.Size = new System.Drawing.Size(99, 13);
      this.meanWeightChangeLabel.TabIndex = 6;
      this.meanWeightChangeLabel.Text = "Value change (mu):";
      this.toolTip.SetToolTip(this.meanWeightChangeLabel, "The mu (mean) parameter for the normal distribution to sample the value change.");
      // 
      // valueChangeMuTextBox
      // 
      this.valueChangeMuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.valueChangeMuTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.valueChangeMuTextBox.Location = new System.Drawing.Point(136, 19);
      this.valueChangeMuTextBox.Name = "valueChangeMuTextBox";
      this.valueChangeMuTextBox.Size = new System.Drawing.Size(178, 20);
      this.valueChangeMuTextBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.valueChangeMuTextBox, "The mu (mean) parameter for the normal distribution to sample the value change.");
      this.valueChangeMuTextBox.TextChanged += new System.EventHandler(this.valueChangeNuTextBox_TextChanged);
      // 
      // ConstantView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.mutationGroupBox);
      this.Controls.Add(this.initializationGroupBox);
      this.Name = "ConstantView";
      this.Size = new System.Drawing.Size(326, 235);
      this.Controls.SetChildIndex(this.initializationGroupBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.Controls.SetChildIndex(this.mutationGroupBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.initializationGroupBox.ResumeLayout(false);
      this.initializationGroupBox.PerformLayout();
      this.mutationGroupBox.ResumeLayout(false);
      this.mutationGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label weightNuLabel;
    private System.Windows.Forms.TextBox minValueTextBox;
    protected System.Windows.Forms.GroupBox initializationGroupBox;
    private System.Windows.Forms.Label weightSigmaLabel;
    private System.Windows.Forms.TextBox maxValueTextBox;
    protected System.Windows.Forms.GroupBox mutationGroupBox;
    private System.Windows.Forms.Label stdDevWeightChangeLabel;
    private System.Windows.Forms.TextBox valueChangeSigmaTextBox;
    private System.Windows.Forms.Label meanWeightChangeLabel;
    private System.Windows.Forms.TextBox valueChangeMuTextBox;

  }
}
