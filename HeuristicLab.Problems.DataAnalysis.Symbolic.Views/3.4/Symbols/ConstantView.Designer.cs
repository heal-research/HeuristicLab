#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
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
      this.multiplicativeChangeLabel = new System.Windows.Forms.Label();
      this.multiplicativeChangeSigmaTextBox = new System.Windows.Forms.TextBox();
      this.additiveChangeLabel = new System.Windows.Forms.Label();
      this.additiveChangeSigmaTextBox = new System.Windows.Forms.TextBox();
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
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // minimumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.minimumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.minimumArityTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // maximumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.maximumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.maximumArityTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(280, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(379, 3);
      // 
      // weightNuLabel
      // 
      this.weightNuLabel.AutoSize = true;
      this.weightNuLabel.Location = new System.Drawing.Point(6, 22);
      this.weightNuLabel.Name = "weightNuLabel";
      this.weightNuLabel.Size = new System.Drawing.Size(56, 13);
      this.weightNuLabel.TabIndex = 0;
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
      this.minValueTextBox.Size = new System.Drawing.Size(300, 20);
      this.minValueTextBox.TabIndex = 1;
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
      this.initializationGroupBox.Location = new System.Drawing.Point(0, 127);
      this.initializationGroupBox.Name = "initializationGroupBox";
      this.initializationGroupBox.Size = new System.Drawing.Size(398, 73);
      this.initializationGroupBox.TabIndex = 5;
      this.initializationGroupBox.TabStop = false;
      this.initializationGroupBox.Text = "Initialization";
      // 
      // weightSigmaLabel
      // 
      this.weightSigmaLabel.AutoSize = true;
      this.weightSigmaLabel.Location = new System.Drawing.Point(6, 48);
      this.weightSigmaLabel.Name = "weightSigmaLabel";
      this.weightSigmaLabel.Size = new System.Drawing.Size(59, 13);
      this.weightSigmaLabel.TabIndex = 2;
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
      this.maxValueTextBox.Size = new System.Drawing.Size(300, 20);
      this.maxValueTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.maxValueTextBox, "The maximal value to use for random initialization of constants.");
      this.maxValueTextBox.TextChanged += new System.EventHandler(this.maxValueTextBox_TextChanged);
      // 
      // mutationGroupBox
      // 
      this.mutationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.mutationGroupBox.Controls.Add(this.multiplicativeChangeLabel);
      this.mutationGroupBox.Controls.Add(this.multiplicativeChangeSigmaTextBox);
      this.mutationGroupBox.Controls.Add(this.additiveChangeLabel);
      this.mutationGroupBox.Controls.Add(this.additiveChangeSigmaTextBox);
      this.mutationGroupBox.Location = new System.Drawing.Point(0, 206);
      this.mutationGroupBox.Name = "mutationGroupBox";
      this.mutationGroupBox.Size = new System.Drawing.Size(398, 73);
      this.mutationGroupBox.TabIndex = 6;
      this.mutationGroupBox.TabStop = false;
      this.mutationGroupBox.Text = "Mutation";
      // 
      // multiplicativeChangeLabel
      // 
      this.multiplicativeChangeLabel.AutoSize = true;
      this.multiplicativeChangeLabel.Location = new System.Drawing.Point(6, 48);
      this.multiplicativeChangeLabel.Name = "multiplicativeChangeLabel";
      this.multiplicativeChangeLabel.Size = new System.Drawing.Size(146, 13);
      this.multiplicativeChangeLabel.TabIndex = 2;
      this.multiplicativeChangeLabel.Text = "Multiplicative change (sigma):";
      this.toolTip.SetToolTip(this.multiplicativeChangeLabel, "The sigma (std. dev.) parameter for the normal distribution to use to sample the " +
        "multiplicative change.");
      // 
      // multiplicativeChangeSigmaTextBox
      // 
      this.multiplicativeChangeSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.multiplicativeChangeSigmaTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.multiplicativeChangeSigmaTextBox.Location = new System.Drawing.Point(168, 45);
      this.multiplicativeChangeSigmaTextBox.Name = "multiplicativeChangeSigmaTextBox";
      this.multiplicativeChangeSigmaTextBox.Size = new System.Drawing.Size(224, 20);
      this.multiplicativeChangeSigmaTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.multiplicativeChangeSigmaTextBox, "The sigma (std. dev.) parameter for the normal distribution to use to sample a mu" +
        "ltiplicative change.");
      this.multiplicativeChangeSigmaTextBox.TextChanged += new System.EventHandler(this.multiplicativeChangeSigmaTextBox_TextChanged);
      // 
      // additiveChangeLabel
      // 
      this.additiveChangeLabel.AutoSize = true;
      this.additiveChangeLabel.Location = new System.Drawing.Point(6, 22);
      this.additiveChangeLabel.Name = "additiveChangeLabel";
      this.additiveChangeLabel.Size = new System.Drawing.Size(123, 13);
      this.additiveChangeLabel.TabIndex = 0;
      this.additiveChangeLabel.Text = "Additive change (sigma):";
      this.toolTip.SetToolTip(this.additiveChangeLabel, "The sigma (std. dev.) parameter for the normal distribution to sample the additiv" +
        "e change.");
      // 
      // additiveChangeSigmaTextBox
      // 
      this.additiveChangeSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.additiveChangeSigmaTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.additiveChangeSigmaTextBox.Location = new System.Drawing.Point(168, 19);
      this.additiveChangeSigmaTextBox.Name = "additiveChangeSigmaTextBox";
      this.additiveChangeSigmaTextBox.Size = new System.Drawing.Size(224, 20);
      this.additiveChangeSigmaTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.additiveChangeSigmaTextBox, "The sigma (std. dev.) parameter for the normal distribution to use to sample an a" +
        "dditive change.");
      this.additiveChangeSigmaTextBox.TextChanged += new System.EventHandler(this.additiveChangeSigmaTextBox_TextChanged);
      // 
      // ConstantView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.mutationGroupBox);
      this.Controls.Add(this.initializationGroupBox);
      this.Name = "ConstantView";
      this.Size = new System.Drawing.Size(398, 284);
      this.Controls.SetChildIndex(this.maximumArityLabel, 0);
      this.Controls.SetChildIndex(this.maximumArityTextBox, 0);
      this.Controls.SetChildIndex(this.minimumArityLabel, 0);
      this.Controls.SetChildIndex(this.minimumArityTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.initializationGroupBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
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
    private System.Windows.Forms.Label multiplicativeChangeLabel;
    private System.Windows.Forms.TextBox multiplicativeChangeSigmaTextBox;
    private System.Windows.Forms.Label additiveChangeLabel;
    private System.Windows.Forms.TextBox additiveChangeSigmaTextBox;

  }
}
