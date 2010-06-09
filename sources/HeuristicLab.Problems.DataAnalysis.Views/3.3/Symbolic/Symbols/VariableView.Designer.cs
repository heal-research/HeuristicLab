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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Views {
  partial class VariableView {
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
      this.weightNuTextBox = new System.Windows.Forms.TextBox();
      this.initializationGroupBox = new System.Windows.Forms.GroupBox();
      this.weightSigmaLabel = new System.Windows.Forms.Label();
      this.weightSigmaTextBox = new System.Windows.Forms.TextBox();
      this.mutationGroupBox = new System.Windows.Forms.GroupBox();
      this.stdDevWeightChangeLabel = new System.Windows.Forms.Label();
      this.weightChangeSigmaTextBox = new System.Windows.Forms.TextBox();
      this.meanWeightChangeLabel = new System.Windows.Forms.Label();
      this.weightChangeNuTextBox = new System.Windows.Forms.TextBox();
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
      this.descriptionTextBox.Size = new System.Drawing.Size(203, 20);
      // 
      // weightNuLabel
      // 
      this.weightNuLabel.AutoSize = true;
      this.weightNuLabel.Location = new System.Drawing.Point(6, 22);
      this.weightNuLabel.Name = "weightNuLabel";
      this.weightNuLabel.Size = new System.Drawing.Size(65, 13);
      this.weightNuLabel.TabIndex = 6;
      this.weightNuLabel.Text = "Weight (nu):";
      this.toolTip.SetToolTip(this.weightNuLabel, "The nu (mean) parameter of the normal distribution to use for initial weights.");
      // 
      // weightNuTextBox
      // 
      this.weightNuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.weightNuTextBox.Location = new System.Drawing.Point(92, 19);
      this.weightNuTextBox.Name = "weightNuTextBox";
      this.weightNuTextBox.Size = new System.Drawing.Size(201, 20);
      this.weightNuTextBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.weightNuTextBox, "The nu (mean) parameter of the normal distribution from which to sample the initi" +
              "al weights.");
      this.weightNuTextBox.TextChanged += new System.EventHandler(this.weightNuTextBox_TextChanged);
      // 
      // initializationGroupBox
      // 
      this.initializationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.initializationGroupBox.Controls.Add(this.weightSigmaLabel);
      this.initializationGroupBox.Controls.Add(this.weightSigmaTextBox);
      this.initializationGroupBox.Controls.Add(this.weightNuLabel);
      this.initializationGroupBox.Controls.Add(this.weightNuTextBox);
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
      this.weightSigmaLabel.Size = new System.Drawing.Size(80, 13);
      this.weightSigmaLabel.TabIndex = 8;
      this.weightSigmaLabel.Text = "Weight (sigma):";
      this.toolTip.SetToolTip(this.weightSigmaLabel, "The sigma parameter for the normal distribution to use for the initial weights.");
      // 
      // weightSigmaTextBox
      // 
      this.weightSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.weightSigmaTextBox.Location = new System.Drawing.Point(92, 45);
      this.weightSigmaTextBox.Name = "weightSigmaTextBox";
      this.weightSigmaTextBox.Size = new System.Drawing.Size(201, 20);
      this.weightSigmaTextBox.TabIndex = 9;
      this.toolTip.SetToolTip(this.weightSigmaTextBox, "The sigma parameter for the normal distribution from which to sample the initial " +
              "weights.");
      this.weightSigmaTextBox.TextChanged += new System.EventHandler(this.weightSigmaTextBox_TextChanged);
      // 
      // mutationGroupBox
      // 
      this.mutationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.mutationGroupBox.Controls.Add(this.stdDevWeightChangeLabel);
      this.mutationGroupBox.Controls.Add(this.weightChangeSigmaTextBox);
      this.mutationGroupBox.Controls.Add(this.meanWeightChangeLabel);
      this.mutationGroupBox.Controls.Add(this.weightChangeNuTextBox);
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
      this.stdDevWeightChangeLabel.Size = new System.Drawing.Size(119, 13);
      this.stdDevWeightChangeLabel.TabIndex = 8;
      this.stdDevWeightChangeLabel.Text = "Weight change (sigma):";
      this.toolTip.SetToolTip(this.stdDevWeightChangeLabel, "The sigma parameter for the normal distribution to use to sample the change in we" +
              "ight.");
      // 
      // weightChangeSigmaTextBox
      // 
      this.weightChangeSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.weightChangeSigmaTextBox.Location = new System.Drawing.Point(131, 45);
      this.weightChangeSigmaTextBox.Name = "weightChangeSigmaTextBox";
      this.weightChangeSigmaTextBox.Size = new System.Drawing.Size(162, 20);
      this.weightChangeSigmaTextBox.TabIndex = 9;
      this.toolTip.SetToolTip(this.weightChangeSigmaTextBox, "The sigma parameter for the normal distribution to use to sample the change in we" +
              "ight.");
      this.weightChangeSigmaTextBox.TextChanged += new System.EventHandler(this.weightChangeSigmaTextBox_TextChanged);
      // 
      // meanWeightChangeLabel
      // 
      this.meanWeightChangeLabel.AutoSize = true;
      this.meanWeightChangeLabel.Location = new System.Drawing.Point(6, 22);
      this.meanWeightChangeLabel.Name = "meanWeightChangeLabel";
      this.meanWeightChangeLabel.Size = new System.Drawing.Size(104, 13);
      this.meanWeightChangeLabel.TabIndex = 6;
      this.meanWeightChangeLabel.Text = "Weight change (nu):";
      this.toolTip.SetToolTip(this.meanWeightChangeLabel, "The nu (mean) parameter for the normal distribution to sample the change in weigh" +
              "t.");
      // 
      // weightChangeNuTextBox
      // 
      this.weightChangeNuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.weightChangeNuTextBox.Location = new System.Drawing.Point(131, 19);
      this.weightChangeNuTextBox.Name = "weightChangeNuTextBox";
      this.weightChangeNuTextBox.Size = new System.Drawing.Size(162, 20);
      this.weightChangeNuTextBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.weightChangeNuTextBox, "The nu (mean) parameter for the normal distribution to sample the change in weigh" +
              "t.");
      this.weightChangeNuTextBox.TextChanged += new System.EventHandler(this.weightChangeNuTextBox_TextChanged);
      // 
      // VariableView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.mutationGroupBox);
      this.Controls.Add(this.initializationGroupBox);
      this.Name = "VariableView";
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
    private System.Windows.Forms.TextBox weightNuTextBox;
    protected System.Windows.Forms.GroupBox initializationGroupBox;
    private System.Windows.Forms.Label weightSigmaLabel;
    private System.Windows.Forms.TextBox weightSigmaTextBox;
    protected System.Windows.Forms.GroupBox mutationGroupBox;
    private System.Windows.Forms.Label stdDevWeightChangeLabel;
    private System.Windows.Forms.TextBox weightChangeSigmaTextBox;
    private System.Windows.Forms.Label meanWeightChangeLabel;
    private System.Windows.Forms.TextBox weightChangeNuTextBox;

  }
}
