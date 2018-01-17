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
      this.mutationGroupBox = new System.Windows.Forms.GroupBox();
      this.slopeChangeSigmaLabel = new System.Windows.Forms.Label();
      this.slopeChangeSigmaTextBox = new System.Windows.Forms.TextBox();
      this.slopeChangeMuLabel = new System.Windows.Forms.Label();
      this.slopeChangeMuTextBox = new System.Windows.Forms.TextBox();
      this.thresholdChangeSigmaLabel = new System.Windows.Forms.Label();
      this.thresholdChangeSigmaTextBox = new System.Windows.Forms.TextBox();
      this.ThresholdChangeMuLabel = new System.Windows.Forms.Label();
      this.thresholdChangeMuTextBox = new System.Windows.Forms.TextBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.variableNamesTabPage = new System.Windows.Forms.TabPage();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.initializationGroupBox = new System.Windows.Forms.GroupBox();
      this.slopeInitializationSigmaLabel = new System.Windows.Forms.Label();
      this.slopeInitializationSigmaTextBox = new System.Windows.Forms.TextBox();
      this.slopeInitializationMuLabel = new System.Windows.Forms.Label();
      this.slopeInitializationMuTextBox = new System.Windows.Forms.TextBox();
      this.thresholdInitializationSigmaLabel = new System.Windows.Forms.Label();
      this.thresholdInitializationSigmaTextBox = new System.Windows.Forms.TextBox();
      this.thresholdInitializationMuLabel = new System.Windows.Forms.Label();
      this.thresholdInitializationMuTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.mutationGroupBox.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.initializationGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // initialFrequencyLabel
      // 
      this.toolTip.SetToolTip(this.initialFrequencyLabel, "Relative frequency of the symbol in randomly created trees");
      // 
      // initialFrequencyTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initialFrequencyTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(427, 20);
      // 
      // minimumArityLabel
      // 
      this.toolTip.SetToolTip(this.minimumArityLabel, "The minimum arity of the symbol");
      // 
      // maximumArityLabel
      // 
      this.toolTip.SetToolTip(this.maximumArityLabel, "The maximum arity of the symbol");
      // 
      // minimumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.minimumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.minimumArityTextBox.Size = new System.Drawing.Size(421, 20);
      // 
      // maximumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.maximumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.maximumArityTextBox.Size = new System.Drawing.Size(421, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(397, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(495, 3);
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
      this.mutationGroupBox.Location = new System.Drawing.Point(6, 142);
      this.mutationGroupBox.Name = "mutationGroupBox";
      this.mutationGroupBox.Size = new System.Drawing.Size(503, 126);
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
      this.slopeChangeSigmaTextBox.Size = new System.Drawing.Size(345, 20);
      this.slopeChangeSigmaTextBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.slopeChangeSigmaTextBox, "The sigma parameter for the normal distribution to use to sample the change in sl" +
              "ope.");
      this.slopeChangeSigmaTextBox.TextChanged += new System.EventHandler(this.SlopeChangeSigmaTextBox_TextChanged);
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
      this.slopeChangeMuTextBox.Size = new System.Drawing.Size(345, 20);
      this.slopeChangeMuTextBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.slopeChangeMuTextBox, "The mu (mean) parameter for the normal distribution to sample the change in slope" +
              ".");
      this.slopeChangeMuTextBox.TextChanged += new System.EventHandler(this.SlopeChangeMuTextBox_TextChanged);
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
      this.thresholdChangeSigmaTextBox.Size = new System.Drawing.Size(345, 20);
      this.thresholdChangeSigmaTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.thresholdChangeSigmaTextBox, "The sigma parameter for the normal distribution to use to sample the change in th" +
              "reshold.");
      this.thresholdChangeSigmaTextBox.TextChanged += new System.EventHandler(this.ThresholdChangeSigmaTextBox_TextChanged);
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
      this.thresholdChangeMuTextBox.Size = new System.Drawing.Size(345, 20);
      this.thresholdChangeMuTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.thresholdChangeMuTextBox, "The mu (mean) parameter for the normal distribution to sample the change in thres" +
              "hold.");
      this.thresholdChangeMuTextBox.TextChanged += new System.EventHandler(this.ThresholdChangeMuTextBox_TextChanged);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.variableNamesTabPage);
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 127);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(520, 299);
      this.tabControl.TabIndex = 10;
      // 
      // variableNamesTabPage
      // 
      this.variableNamesTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableNamesTabPage.Name = "variableNamesTabPage";
      this.variableNamesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableNamesTabPage.Size = new System.Drawing.Size(512, 273);
      this.variableNamesTabPage.TabIndex = 0;
      this.variableNamesTabPage.Text = "Variable Names";
      this.variableNamesTabPage.UseVisualStyleBackColor = true;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.initializationGroupBox);
      this.parametersTabPage.Controls.Add(this.mutationGroupBox);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(512, 273);
      this.parametersTabPage.TabIndex = 1;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
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
      this.initializationGroupBox.Location = new System.Drawing.Point(6, 6);
      this.initializationGroupBox.Name = "initializationGroupBox";
      this.initializationGroupBox.Size = new System.Drawing.Size(503, 130);
      this.initializationGroupBox.TabIndex = 6;
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
      this.slopeInitializationSigmaTextBox.Size = new System.Drawing.Size(380, 20);
      this.slopeInitializationSigmaTextBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.slopeInitializationSigmaTextBox, "The sigma parameter for the normal distribution from which to sample the initial " +
              "slopes.");
      this.slopeInitializationSigmaTextBox.TextChanged += new System.EventHandler(this.SlopeInitializationSigmaTextBox_TextChanged);
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
      this.slopeInitializationMuTextBox.Size = new System.Drawing.Size(380, 20);
      this.slopeInitializationMuTextBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.slopeInitializationMuTextBox, "The mu (mean) parameter of the normal distribution from which to sample the initi" +
              "al slopes.");
      this.slopeInitializationMuTextBox.TextChanged += new System.EventHandler(this.SlopeInitializationMuTextBox_TextChanged);
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
      this.thresholdInitializationSigmaTextBox.Size = new System.Drawing.Size(380, 20);
      this.thresholdInitializationSigmaTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.thresholdInitializationSigmaTextBox, "The sigma parameter for the normal distribution from which to sample the initial " +
              "thresholds.");
      this.thresholdInitializationSigmaTextBox.TextChanged += new System.EventHandler(this.ThresholdInitializationSigmaTextBox_TextChanged);
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
      this.thresholdInitializationMuTextBox.Size = new System.Drawing.Size(380, 20);
      this.thresholdInitializationMuTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.thresholdInitializationMuTextBox, "The mu (mean) parameter of the normal distribution from which to sample the initi" +
              "al thresholds.");
      this.thresholdInitializationMuTextBox.TextChanged += new System.EventHandler(this.ThresholdInitializationMuTextBox_TextChanged);
      // 
      // VariableConditionView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabControl);
      this.Name = "VariableConditionView";
      this.Size = new System.Drawing.Size(520, 426);
      this.Controls.SetChildIndex(this.maximumArityLabel, 0);
      this.Controls.SetChildIndex(this.maximumArityTextBox, 0);
      this.Controls.SetChildIndex(this.minimumArityLabel, 0);
      this.Controls.SetChildIndex(this.minimumArityTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.mutationGroupBox.ResumeLayout(false);
      this.mutationGroupBox.PerformLayout();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.initializationGroupBox.ResumeLayout(false);
      this.initializationGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.GroupBox mutationGroupBox;
    private System.Windows.Forms.Label thresholdChangeSigmaLabel;
    private System.Windows.Forms.TextBox thresholdChangeSigmaTextBox;
    private System.Windows.Forms.Label ThresholdChangeMuLabel;
    private System.Windows.Forms.TextBox thresholdChangeMuTextBox;
    private System.Windows.Forms.Label slopeChangeSigmaLabel;
    private System.Windows.Forms.TextBox slopeChangeSigmaTextBox;
    private System.Windows.Forms.Label slopeChangeMuLabel;
    private System.Windows.Forms.TextBox slopeChangeMuTextBox;
    protected System.Windows.Forms.TabControl tabControl;
    protected System.Windows.Forms.TabPage variableNamesTabPage;
    protected System.Windows.Forms.TabPage parametersTabPage;
    protected System.Windows.Forms.GroupBox initializationGroupBox;
    private System.Windows.Forms.Label slopeInitializationSigmaLabel;
    private System.Windows.Forms.TextBox slopeInitializationSigmaTextBox;
    private System.Windows.Forms.Label slopeInitializationMuLabel;
    private System.Windows.Forms.TextBox slopeInitializationMuTextBox;
    private System.Windows.Forms.Label thresholdInitializationSigmaLabel;
    private System.Windows.Forms.TextBox thresholdInitializationSigmaTextBox;
    private System.Windows.Forms.Label thresholdInitializationMuLabel;
    private System.Windows.Forms.TextBox thresholdInitializationMuTextBox;

  }
}
