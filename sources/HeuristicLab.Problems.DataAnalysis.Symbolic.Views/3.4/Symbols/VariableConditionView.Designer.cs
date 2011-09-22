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


namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class VariableConditionView {
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
      this.thresholdInitializationMuLabel = new System.Windows.Forms.Label();
      this.thresholdInitializationMuTextBox = new System.Windows.Forms.TextBox();
      this.initializationGroupBox = new System.Windows.Forms.GroupBox();
      this.slopeInitializationSigmaLabel = new System.Windows.Forms.Label();
      this.slopeInitializationSigmaTextBox = new System.Windows.Forms.TextBox();
      this.slopeInitializationMuLabel = new System.Windows.Forms.Label();
      this.slopeInitializationMuTextBox = new System.Windows.Forms.TextBox();
      this.thresholdInitializationSigmaLabel = new System.Windows.Forms.Label();
      this.thresholdInitializationSigmaTextBox = new System.Windows.Forms.TextBox();
      this.mutationGroupBox = new System.Windows.Forms.GroupBox();
      this.slopeChangeSigmaLabel = new System.Windows.Forms.Label();
      this.slopeChangeSigmaTextBox = new System.Windows.Forms.TextBox();
      this.slopeChangeMuLabel = new System.Windows.Forms.Label();
      this.slopeChangeMuTextBox = new System.Windows.Forms.TextBox();
      this.thresholdChangeSigmaLabel = new System.Windows.Forms.Label();
      this.thresholdChangeSigmaTextBox = new System.Windows.Forms.TextBox();
      this.ThresholdChangeMuLabel = new System.Windows.Forms.Label();
      this.thresholdChangeMuTextBox = new System.Windows.Forms.TextBox();
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
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(233, 20);
      // 
      // minimumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.minimumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      // 
      // maximumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.maximumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(203, 20);
      // 
      // thresholdInitializationMuLabel
      // 
      this.thresholdInitializationMuLabel.AutoSize = true;
      this.thresholdInitializationMuLabel.Location = new System.Drawing.Point(6, 18);
      this.thresholdInitializationMuLabel.Name = "thresholdInitializationMuLabel";
      this.thresholdInitializationMuLabel.Size = new System.Drawing.Size(80, 13);
      this.thresholdInitializationMuLabel.TabIndex = 0;
      this.thresholdInitializationMuLabel.Text = "Threshold (mu):";
      this.toolTip.SetToolTip(this.thresholdInitializationMuLabel, "The mu (mean) parameter of the normal distribution to use for initial weights.");
      // 
      // thresholdInitializationMuTextBox
      // 
      this.thresholdInitializationMuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.thresholdInitializationMuTextBox.Location = new System.Drawing.Point(114, 15);
      this.thresholdInitializationMuTextBox.Name = "thresholdInitializationMuTextBox";
      this.thresholdInitializationMuTextBox.Size = new System.Drawing.Size(203, 20);
      this.thresholdInitializationMuTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.thresholdInitializationMuTextBox, "The mu (mean) parameter of the normal distribution from which to sample the initi" +
        "al thresholds.");
      this.thresholdInitializationMuTextBox.TextChanged += new System.EventHandler(this.thresholdMuTextBox_TextChanged);
      // 
      // initializationGroupBox
      // 
      this.initializationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.initializationGroupBox.Controls.Add(this.slopeInitializationSigmaLabel);
      this.initializationGroupBox.Controls.Add(this.slopeInitializationSigmaTextBox);
      this.initializationGroupBox.Controls.Add(this.slopeInitializationMuLabel);
      this.initializationGroupBox.Controls.Add(this.slopeInitializationMuTextBox);
      this.initializationGroupBox.Controls.Add(this.thresholdInitializationSigmaLabel);
      this.initializationGroupBox.Controls.Add(this.thresholdInitializationSigmaTextBox);
      this.initializationGroupBox.Controls.Add(this.thresholdInitializationMuLabel);
      this.initializationGroupBox.Controls.Add(this.thresholdInitializationMuTextBox);
      this.initializationGroupBox.Location = new System.Drawing.Point(0, 127);
      this.initializationGroupBox.Name = "initializationGroupBox";
      this.initializationGroupBox.Size = new System.Drawing.Size(326, 127);
      this.initializationGroupBox.TabIndex = 5;
      this.initializationGroupBox.TabStop = false;
      this.initializationGroupBox.Text = "Initialization";
      // 
      // slopeInitializationSigmaLabel
      // 
      this.slopeInitializationSigmaLabel.AutoSize = true;
      this.slopeInitializationSigmaLabel.Location = new System.Drawing.Point(6, 105);
      this.slopeInitializationSigmaLabel.Name = "slopeInitializationSigmaLabel";
      this.slopeInitializationSigmaLabel.Size = new System.Drawing.Size(73, 13);
      this.slopeInitializationSigmaLabel.TabIndex = 6;
      this.slopeInitializationSigmaLabel.Text = "Slope (sigma):";
      this.toolTip.SetToolTip(this.slopeInitializationSigmaLabel, "The sigma parameter for the normal distribution to use for the initial slopes.");
      // 
      // slopeInitializationSigmaTextBox
      // 
      this.slopeInitializationSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.slopeInitializationSigmaTextBox.Location = new System.Drawing.Point(114, 102);
      this.slopeInitializationSigmaTextBox.Name = "slopeInitializationSigmaTextBox";
      this.slopeInitializationSigmaTextBox.Size = new System.Drawing.Size(203, 20);
      this.slopeInitializationSigmaTextBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.slopeInitializationSigmaTextBox, "The sigma parameter for the normal distribution from which to sample the initial " +
        "slopes.");
      this.slopeInitializationSigmaTextBox.TextChanged += new System.EventHandler(this.slopeInitializationSigmaTextBox_TextChanged);
      // 
      // slopeInitializationMuLabel
      // 
      this.slopeInitializationMuLabel.AutoSize = true;
      this.slopeInitializationMuLabel.Location = new System.Drawing.Point(6, 79);
      this.slopeInitializationMuLabel.Name = "slopeInitializationMuLabel";
      this.slopeInitializationMuLabel.Size = new System.Drawing.Size(60, 13);
      this.slopeInitializationMuLabel.TabIndex = 4;
      this.slopeInitializationMuLabel.Text = "Slope (mu):";
      this.toolTip.SetToolTip(this.slopeInitializationMuLabel, "The mu (mean) parameter of the normal distribution to use for initial slopes.");
      // 
      // slopeInitializationMuTextBox
      // 
      this.slopeInitializationMuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.slopeInitializationMuTextBox.Location = new System.Drawing.Point(114, 76);
      this.slopeInitializationMuTextBox.Name = "slopeInitializationMuTextBox";
      this.slopeInitializationMuTextBox.Size = new System.Drawing.Size(203, 20);
      this.slopeInitializationMuTextBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.slopeInitializationMuTextBox, "The mu (mean) parameter of the normal distribution from which to sample the initi" +
        "al slopes.");
      this.slopeInitializationMuTextBox.TextChanged += new System.EventHandler(this.slopeInitializationMuTextBox_TextChanged);
      // 
      // thresholdInitializationSigmaLabel
      // 
      this.thresholdInitializationSigmaLabel.AutoSize = true;
      this.thresholdInitializationSigmaLabel.Location = new System.Drawing.Point(6, 44);
      this.thresholdInitializationSigmaLabel.Name = "thresholdInitializationSigmaLabel";
      this.thresholdInitializationSigmaLabel.Size = new System.Drawing.Size(93, 13);
      this.thresholdInitializationSigmaLabel.TabIndex = 2;
      this.thresholdInitializationSigmaLabel.Text = "Threshold (sigma):";
      this.toolTip.SetToolTip(this.thresholdInitializationSigmaLabel, "The sigma parameter for the normal distribution to use for the initial weights.");
      // 
      // thresholdInitializationSigmaTextBox
      // 
      this.thresholdInitializationSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.thresholdInitializationSigmaTextBox.Location = new System.Drawing.Point(114, 41);
      this.thresholdInitializationSigmaTextBox.Name = "thresholdInitializationSigmaTextBox";
      this.thresholdInitializationSigmaTextBox.Size = new System.Drawing.Size(203, 20);
      this.thresholdInitializationSigmaTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.thresholdInitializationSigmaTextBox, "The sigma parameter for the normal distribution from which to sample the initial " +
        "thresholds.");
      this.thresholdInitializationSigmaTextBox.TextChanged += new System.EventHandler(this.thresholdInitializationSigmaTextBox_TextChanged);
      // 
      // mutationGroupBox
      // 
      this.mutationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.mutationGroupBox.Controls.Add(this.slopeChangeSigmaLabel);
      this.mutationGroupBox.Controls.Add(this.slopeChangeSigmaTextBox);
      this.mutationGroupBox.Controls.Add(this.slopeChangeMuLabel);
      this.mutationGroupBox.Controls.Add(this.slopeChangeMuTextBox);
      this.mutationGroupBox.Controls.Add(this.thresholdChangeSigmaLabel);
      this.mutationGroupBox.Controls.Add(this.thresholdChangeSigmaTextBox);
      this.mutationGroupBox.Controls.Add(this.ThresholdChangeMuLabel);
      this.mutationGroupBox.Controls.Add(this.thresholdChangeMuTextBox);
      this.mutationGroupBox.Location = new System.Drawing.Point(0, 260);
      this.mutationGroupBox.Name = "mutationGroupBox";
      this.mutationGroupBox.Size = new System.Drawing.Size(326, 127);
      this.mutationGroupBox.TabIndex = 6;
      this.mutationGroupBox.TabStop = false;
      this.mutationGroupBox.Text = "Mutation";
      // 
      // slopeChangeSigmaLabel
      // 
      this.slopeChangeSigmaLabel.AutoSize = true;
      this.slopeChangeSigmaLabel.Location = new System.Drawing.Point(6, 102);
      this.slopeChangeSigmaLabel.Name = "slopeChangeSigmaLabel";
      this.slopeChangeSigmaLabel.Size = new System.Drawing.Size(112, 13);
      this.slopeChangeSigmaLabel.TabIndex = 6;
      this.slopeChangeSigmaLabel.Text = "Slope change (sigma):";
      this.toolTip.SetToolTip(this.slopeChangeSigmaLabel, "The sigma parameter for the normal distribution to use to sample the change in sl" +
        "ope.");
      // 
      // slopeChangeSigmaTextBox
      // 
      this.slopeChangeSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.slopeChangeSigmaTextBox.Location = new System.Drawing.Point(149, 99);
      this.slopeChangeSigmaTextBox.Name = "slopeChangeSigmaTextBox";
      this.slopeChangeSigmaTextBox.Size = new System.Drawing.Size(168, 20);
      this.slopeChangeSigmaTextBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.slopeChangeSigmaTextBox, "The sigma parameter for the normal distribution to use to sample the change in sl" +
        "ope.");
      this.slopeChangeSigmaTextBox.TextChanged += new System.EventHandler(this.slopeChangeSigmaTextBox_TextChanged);
      // 
      // slopeChangeMuLabel
      // 
      this.slopeChangeMuLabel.AutoSize = true;
      this.slopeChangeMuLabel.Location = new System.Drawing.Point(6, 76);
      this.slopeChangeMuLabel.Name = "slopeChangeMuLabel";
      this.slopeChangeMuLabel.Size = new System.Drawing.Size(99, 13);
      this.slopeChangeMuLabel.TabIndex = 4;
      this.slopeChangeMuLabel.Text = "Slope change (mu):";
      this.toolTip.SetToolTip(this.slopeChangeMuLabel, "The nu (mean) parameter for the normal distribution to sample the change in slope" +
        ".");
      // 
      // slopeChangeMuTextBox
      // 
      this.slopeChangeMuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.slopeChangeMuTextBox.Location = new System.Drawing.Point(149, 73);
      this.slopeChangeMuTextBox.Name = "slopeChangeMuTextBox";
      this.slopeChangeMuTextBox.Size = new System.Drawing.Size(168, 20);
      this.slopeChangeMuTextBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.slopeChangeMuTextBox, "The mu (mean) parameter for the normal distribution to sample the change in slope" +
        ".");
      this.slopeChangeMuTextBox.TextChanged += new System.EventHandler(this.slopeChangeMuTextBox_TextChanged);
      // 
      // thresholdChangeSigmaLabel
      // 
      this.thresholdChangeSigmaLabel.AutoSize = true;
      this.thresholdChangeSigmaLabel.Location = new System.Drawing.Point(6, 44);
      this.thresholdChangeSigmaLabel.Name = "thresholdChangeSigmaLabel";
      this.thresholdChangeSigmaLabel.Size = new System.Drawing.Size(132, 13);
      this.thresholdChangeSigmaLabel.TabIndex = 2;
      this.thresholdChangeSigmaLabel.Text = "Threshold change (sigma):";
      this.toolTip.SetToolTip(this.thresholdChangeSigmaLabel, "The sigma parameter for the normal distribution to use to sample the change in th" +
        "reshold.");
      // 
      // thresholdChangeSigmaTextBox
      // 
      this.thresholdChangeSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.thresholdChangeSigmaTextBox.Location = new System.Drawing.Point(149, 41);
      this.thresholdChangeSigmaTextBox.Name = "thresholdChangeSigmaTextBox";
      this.thresholdChangeSigmaTextBox.Size = new System.Drawing.Size(168, 20);
      this.thresholdChangeSigmaTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.thresholdChangeSigmaTextBox, "The sigma parameter for the normal distribution to use to sample the change in th" +
        "reshold.");
      this.thresholdChangeSigmaTextBox.TextChanged += new System.EventHandler(this.thresholdChangeSigmaTextBox_TextChanged);
      // 
      // ThresholdChangeMuLabel
      // 
      this.ThresholdChangeMuLabel.AutoSize = true;
      this.ThresholdChangeMuLabel.Location = new System.Drawing.Point(6, 18);
      this.ThresholdChangeMuLabel.Name = "ThresholdChangeMuLabel";
      this.ThresholdChangeMuLabel.Size = new System.Drawing.Size(119, 13);
      this.ThresholdChangeMuLabel.TabIndex = 0;
      this.ThresholdChangeMuLabel.Text = "Threshold change (mu):";
      this.toolTip.SetToolTip(this.ThresholdChangeMuLabel, "The nu (mean) parameter for the normal distribution to sample the change in thres" +
        "hold.");
      // 
      // thresholdChangeMuTextBox
      // 
      this.thresholdChangeMuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.thresholdChangeMuTextBox.Location = new System.Drawing.Point(149, 15);
      this.thresholdChangeMuTextBox.Name = "thresholdChangeMuTextBox";
      this.thresholdChangeMuTextBox.Size = new System.Drawing.Size(168, 20);
      this.thresholdChangeMuTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.thresholdChangeMuTextBox, "The mu (mean) parameter for the normal distribution to sample the change in thres" +
        "hold.");
      this.thresholdChangeMuTextBox.TextChanged += new System.EventHandler(this.thresholdChangeMuTextBox_TextChanged);
      // 
      // VariableConditionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.mutationGroupBox);
      this.Controls.Add(this.initializationGroupBox);
      this.Name = "VariableConditionView";
      this.Size = new System.Drawing.Size(326, 376);
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

    private System.Windows.Forms.Label thresholdInitializationMuLabel;
    private System.Windows.Forms.TextBox thresholdInitializationMuTextBox;
    protected System.Windows.Forms.GroupBox initializationGroupBox;
    private System.Windows.Forms.Label thresholdInitializationSigmaLabel;
    private System.Windows.Forms.TextBox thresholdInitializationSigmaTextBox;
    protected System.Windows.Forms.GroupBox mutationGroupBox;
    private System.Windows.Forms.Label thresholdChangeSigmaLabel;
    private System.Windows.Forms.TextBox thresholdChangeSigmaTextBox;
    private System.Windows.Forms.Label ThresholdChangeMuLabel;
    private System.Windows.Forms.TextBox thresholdChangeMuTextBox;
    private System.Windows.Forms.Label slopeInitializationSigmaLabel;
    private System.Windows.Forms.TextBox slopeInitializationSigmaTextBox;
    private System.Windows.Forms.Label slopeInitializationMuLabel;
    private System.Windows.Forms.TextBox slopeInitializationMuTextBox;
    private System.Windows.Forms.Label slopeChangeSigmaLabel;
    private System.Windows.Forms.TextBox slopeChangeSigmaTextBox;
    private System.Windows.Forms.Label slopeChangeMuLabel;
    private System.Windows.Forms.TextBox slopeChangeMuTextBox;

  }
}
