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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
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
      this.weightMuLabel = new System.Windows.Forms.Label();
      this.weightInitializationMuTextBox = new System.Windows.Forms.TextBox();
      this.initializationGroupBox = new System.Windows.Forms.GroupBox();
      this.weightSigmaLabel = new System.Windows.Forms.Label();
      this.weightInitializationSigmaTextBox = new System.Windows.Forms.TextBox();
      this.mutationGroupBox = new System.Windows.Forms.GroupBox();
      this.multiplicativeWeightChangeLabel = new System.Windows.Forms.Label();
      this.multiplicativeWeightChangeSigmaTextBox = new System.Windows.Forms.TextBox();
      this.additiveWeightChangeLabel = new System.Windows.Forms.Label();
      this.additiveWeightChangeSigmaTextBox = new System.Windows.Forms.TextBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.variableNamesTabPage = new System.Windows.Forms.TabPage();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.initializationGroupBox.SuspendLayout();
      this.mutationGroupBox.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // initialFrequencyLabel
      // 
      this.toolTip.SetToolTip(this.initialFrequencyLabel, "Relative frequency of the symbol in randomly created trees");
      // 
      // initialFrequencyTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initialFrequencyTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(311, 20);
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
      this.minimumArityTextBox.Size = new System.Drawing.Size(311, 20);
      // 
      // maximumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.maximumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.maximumArityTextBox.Size = new System.Drawing.Size(311, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(290, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(389, 3);
      // 
      // weightMuLabel
      // 
      this.weightMuLabel.AutoSize = true;
      this.weightMuLabel.Location = new System.Drawing.Point(6, 22);
      this.weightMuLabel.Name = "weightMuLabel";
      this.weightMuLabel.Size = new System.Drawing.Size(67, 13);
      this.weightMuLabel.TabIndex = 0;
      this.weightMuLabel.Text = "Weight (mu):";
      this.toolTip.SetToolTip(this.weightMuLabel, "The mu (mean) parameter of the normal distribution to use for initial weights.");
      // 
      // weightInitializationMuTextBox
      // 
      this.weightInitializationMuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.weightInitializationMuTextBox.Location = new System.Drawing.Point(92, 19);
      this.weightInitializationMuTextBox.Name = "weightInitializationMuTextBox";
      this.weightInitializationMuTextBox.Size = new System.Drawing.Size(290, 20);
      this.weightInitializationMuTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.weightInitializationMuTextBox, "The mu (mean) parameter of the normal distribution from which to sample the initi" +
        "al weights.");
      this.weightInitializationMuTextBox.TextChanged += new System.EventHandler(this.weightMuTextBox_TextChanged);
      // 
      // initializationGroupBox
      // 
      this.initializationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.initializationGroupBox.Controls.Add(this.weightSigmaLabel);
      this.initializationGroupBox.Controls.Add(this.weightInitializationSigmaTextBox);
      this.initializationGroupBox.Controls.Add(this.weightMuLabel);
      this.initializationGroupBox.Controls.Add(this.weightInitializationMuTextBox);
      this.initializationGroupBox.Location = new System.Drawing.Point(6, 6);
      this.initializationGroupBox.Name = "initializationGroupBox";
      this.initializationGroupBox.Size = new System.Drawing.Size(388, 73);
      this.initializationGroupBox.TabIndex = 0;
      this.initializationGroupBox.TabStop = false;
      this.initializationGroupBox.Text = "Initialization";
      // 
      // weightSigmaLabel
      // 
      this.weightSigmaLabel.AutoSize = true;
      this.weightSigmaLabel.Location = new System.Drawing.Point(6, 48);
      this.weightSigmaLabel.Name = "weightSigmaLabel";
      this.weightSigmaLabel.Size = new System.Drawing.Size(80, 13);
      this.weightSigmaLabel.TabIndex = 2;
      this.weightSigmaLabel.Text = "Weight (sigma):";
      this.toolTip.SetToolTip(this.weightSigmaLabel, "The sigma parameter for the normal distribution to use for the initial weights.");
      // 
      // weightInitializationSigmaTextBox
      // 
      this.weightInitializationSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.weightInitializationSigmaTextBox.Location = new System.Drawing.Point(92, 45);
      this.weightInitializationSigmaTextBox.Name = "weightInitializationSigmaTextBox";
      this.weightInitializationSigmaTextBox.Size = new System.Drawing.Size(290, 20);
      this.weightInitializationSigmaTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.weightInitializationSigmaTextBox, "The sigma parameter for the normal distribution from which to sample the initial " +
        "weights.");
      this.weightInitializationSigmaTextBox.TextChanged += new System.EventHandler(this.weightSigmaTextBox_TextChanged);
      // 
      // mutationGroupBox
      // 
      this.mutationGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.mutationGroupBox.Controls.Add(this.multiplicativeWeightChangeLabel);
      this.mutationGroupBox.Controls.Add(this.multiplicativeWeightChangeSigmaTextBox);
      this.mutationGroupBox.Controls.Add(this.additiveWeightChangeLabel);
      this.mutationGroupBox.Controls.Add(this.additiveWeightChangeSigmaTextBox);
      this.mutationGroupBox.Location = new System.Drawing.Point(6, 85);
      this.mutationGroupBox.Name = "mutationGroupBox";
      this.mutationGroupBox.Size = new System.Drawing.Size(391, 73);
      this.mutationGroupBox.TabIndex = 1;
      this.mutationGroupBox.TabStop = false;
      this.mutationGroupBox.Text = "Mutation";
      // 
      // multiplicativeWeightChangeLabel
      // 
      this.multiplicativeWeightChangeLabel.AutoSize = true;
      this.multiplicativeWeightChangeLabel.Location = new System.Drawing.Point(6, 48);
      this.multiplicativeWeightChangeLabel.Name = "multiplicativeWeightChangeLabel";
      this.multiplicativeWeightChangeLabel.Size = new System.Drawing.Size(180, 13);
      this.multiplicativeWeightChangeLabel.TabIndex = 2;
      this.multiplicativeWeightChangeLabel.Text = "Multiplicative weight change (sigma):";
      this.toolTip.SetToolTip(this.multiplicativeWeightChangeLabel, "The sigma parameter for the normal distribution to use to sample a multiplicative" +
        " change in weight.");
      // 
      // multiplicativeWeightChangeSigmaTextBox
      // 
      this.multiplicativeWeightChangeSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.multiplicativeWeightChangeSigmaTextBox.Location = new System.Drawing.Point(201, 45);
      this.multiplicativeWeightChangeSigmaTextBox.Name = "multiplicativeWeightChangeSigmaTextBox";
      this.multiplicativeWeightChangeSigmaTextBox.Size = new System.Drawing.Size(184, 20);
      this.multiplicativeWeightChangeSigmaTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.multiplicativeWeightChangeSigmaTextBox, "The sigma (std.dev.) parameter for the normal distribution to sample a multiplica" +
        "tive change in weight.");
      this.multiplicativeWeightChangeSigmaTextBox.TextChanged += new System.EventHandler(this.multiplicativeWeightChangeSigmaTextBox_TextChanged);
      // 
      // additiveWeightChangeLabel
      // 
      this.additiveWeightChangeLabel.AutoSize = true;
      this.additiveWeightChangeLabel.Location = new System.Drawing.Point(6, 22);
      this.additiveWeightChangeLabel.Name = "additiveWeightChangeLabel";
      this.additiveWeightChangeLabel.Size = new System.Drawing.Size(157, 13);
      this.additiveWeightChangeLabel.TabIndex = 0;
      this.additiveWeightChangeLabel.Text = "Additive weight change (sigma):";
      this.toolTip.SetToolTip(this.additiveWeightChangeLabel, "The sigma (std.dev.) parameter for the normal distribution to sample an additive " +
        "change in weight.");
      // 
      // additiveWeightChangeSigmaTextBox
      // 
      this.additiveWeightChangeSigmaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.additiveWeightChangeSigmaTextBox.Location = new System.Drawing.Point(201, 19);
      this.additiveWeightChangeSigmaTextBox.Name = "additiveWeightChangeSigmaTextBox";
      this.additiveWeightChangeSigmaTextBox.Size = new System.Drawing.Size(184, 20);
      this.additiveWeightChangeSigmaTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.additiveWeightChangeSigmaTextBox, "The sigma (std.dev.) parameter for the normal distribution to sample an additive " +
        "change in weight.");
      this.additiveWeightChangeSigmaTextBox.TextChanged += new System.EventHandler(this.additiveWeightChangeSigmaTextBox_TextChanged);
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
      this.tabControl.Size = new System.Drawing.Size(408, 190);
      this.tabControl.TabIndex = 5;
      // 
      // variableNamesTabPage
      // 
      this.variableNamesTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableNamesTabPage.Name = "variableNamesTabPage";
      this.variableNamesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableNamesTabPage.Size = new System.Drawing.Size(400, 164);
      this.variableNamesTabPage.TabIndex = 0;
      this.variableNamesTabPage.Text = "Variable Names";
      this.variableNamesTabPage.UseVisualStyleBackColor = true;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.mutationGroupBox);
      this.parametersTabPage.Controls.Add(this.initializationGroupBox);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(400, 164);
      this.parametersTabPage.TabIndex = 1;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // VariableView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabControl);
      this.Name = "VariableView";
      this.Size = new System.Drawing.Size(408, 317);
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
      this.initializationGroupBox.ResumeLayout(false);
      this.initializationGroupBox.PerformLayout();
      this.mutationGroupBox.ResumeLayout(false);
      this.mutationGroupBox.PerformLayout();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label weightMuLabel;
    protected System.Windows.Forms.TextBox weightInitializationMuTextBox;
    protected System.Windows.Forms.GroupBox initializationGroupBox;
    protected System.Windows.Forms.Label weightSigmaLabel;
    protected System.Windows.Forms.TextBox weightInitializationSigmaTextBox;
    protected System.Windows.Forms.GroupBox mutationGroupBox;
    protected System.Windows.Forms.Label multiplicativeWeightChangeLabel;
    protected System.Windows.Forms.TextBox multiplicativeWeightChangeSigmaTextBox;
    protected System.Windows.Forms.Label additiveWeightChangeLabel;
    protected System.Windows.Forms.TextBox additiveWeightChangeSigmaTextBox;
    protected System.Windows.Forms.TabPage variableNamesTabPage;
    protected System.Windows.Forms.TabPage parametersTabPage;
    protected System.Windows.Forms.TabControl tabControl;
  }
}
