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


namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class LaggedVariableView {
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
      this.minTimeOffsetLabel = new System.Windows.Forms.Label();
      this.maxTimeOffsetLabel = new System.Windows.Forms.Label();
      this.minTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.maxTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.initializationGroupBox.SuspendLayout();
      this.mutationGroupBox.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.tabControl.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // weightMuLabel
      // 
      this.toolTip.SetToolTip(this.weightMuLabel, "The mu (mean) parameter of the normal distribution to use for initial weights.");
      // 
      // weightInitializationMuTextBox
      // 
      this.weightInitializationMuTextBox.Size = new System.Drawing.Size(293, 20);
      this.toolTip.SetToolTip(this.weightInitializationMuTextBox, "The mu (mean) parameter of the normal distribution from which to sample the initi" +
        "al weights.");
      // 
      // initializationGroupBox
      // 
      this.initializationGroupBox.Location = new System.Drawing.Point(6, 7);
      this.initializationGroupBox.Size = new System.Drawing.Size(391, 73);
      // 
      // weightSigmaLabel
      // 
      this.toolTip.SetToolTip(this.weightSigmaLabel, "The sigma parameter for the normal distribution to use for the initial weights.");
      // 
      // weightInitializationSigmaTextBox
      // 
      this.weightInitializationSigmaTextBox.Size = new System.Drawing.Size(293, 20);
      this.toolTip.SetToolTip(this.weightInitializationSigmaTextBox, "The sigma parameter for the normal distribution from which to sample the initial " +
        "weights.");
      // 
      // mutationGroupBox
      // 
      this.mutationGroupBox.Location = new System.Drawing.Point(6, 86);
      this.mutationGroupBox.Size = new System.Drawing.Size(391, 95);
      // 
      // multiplicativeWeightChangeLabel
      // 
      this.multiplicativeWeightChangeLabel.TabIndex = 4;
      this.toolTip.SetToolTip(this.multiplicativeWeightChangeLabel, "The sigma parameter for the normal distribution to use to sample a multiplicative" +
        " change in weight.");
      // 
      // multiplicativeWeightChangeSigmaTextBox
      // 
      this.multiplicativeWeightChangeSigmaTextBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.multiplicativeWeightChangeSigmaTextBox, "The sigma (std.dev.) parameter for the normal distribution to sample a multiplica" +
        "tive change in weight.");
      // 
      // additiveWeightChangeLabel
      // 
      this.additiveWeightChangeLabel.TabIndex = 2;
      this.toolTip.SetToolTip(this.additiveWeightChangeLabel, "The sigma (std.dev.) parameter for the normal distribution to sample an additive " +
        "change in weight.");
      // 
      // additiveWeightChangeSigmaTextBox
      // 
      this.additiveWeightChangeSigmaTextBox.TabIndex = 3;
      this.toolTip.SetToolTip(this.additiveWeightChangeSigmaTextBox, "The sigma (std.dev.) parameter for the normal distribution to sample an additive " +
        "change in weight.");
      // 
      // variableNamesTabPage
      // 
      this.variableNamesTabPage.Size = new System.Drawing.Size(400, 149);
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Size = new System.Drawing.Size(412, 208);
      // 
      // tabControl
      // 
      this.tabControl.Location = new System.Drawing.Point(0, 179);
      this.tabControl.Size = new System.Drawing.Size(420, 234);
      this.tabControl.TabIndex = 13;
      // 
      // varChangeProbTextBox
      // 
      this.varChangeProbTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.varChangeProbTextBox, "The sigma (std.dev.) parameter for the normal distribution to sample an additive " +
        "change in weight.");
      // 
      // varChangeProbLabel
      // 
      this.varChangeProbLabel.TabIndex = 0;
      this.toolTip.SetToolTip(this.varChangeProbLabel, "The probability of changing the referenced variable in [0..1]. Variable reference" +
        "s are sampled uniformly.");
      // 
      // initialFrequencyLabel
      // 
      this.initialFrequencyLabel.TabIndex = 2;
      this.toolTip.SetToolTip(this.initialFrequencyLabel, "Relative frequency of the symbol in randomly created trees");
      // 
      // initialFrequencyTextBox
      // 
      this.errorProvider.SetIconAlignment(this.initialFrequencyTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(323, 20);
      this.initialFrequencyTextBox.TabIndex = 3;
      // 
      // enabledCheckBox
      // 
      this.enabledCheckBox.TabIndex = 8;
      // 
      // minimumArityLabel
      // 
      this.minimumArityLabel.TabIndex = 4;
      this.toolTip.SetToolTip(this.minimumArityLabel, "The minimum arity of the symbol");
      // 
      // maximumArityLabel
      // 
      this.toolTip.SetToolTip(this.maximumArityLabel, "The maximum arity of the symbol");
      // 
      // minimumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.minimumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.minimumArityTextBox.Size = new System.Drawing.Size(327, 20);
      this.minimumArityTextBox.TabIndex = 5;
      // 
      // maximumArityTextBox
      // 
      this.errorProvider.SetIconAlignment(this.maximumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.maximumArityTextBox.Size = new System.Drawing.Size(327, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(302, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(401, 3);
      // 
      // minTimeOffsetLabel
      // 
      this.minTimeOffsetLabel.AutoSize = true;
      this.minTimeOffsetLabel.Location = new System.Drawing.Point(3, 130);
      this.minTimeOffsetLabel.Name = "minTimeOffsetLabel";
      this.minTimeOffsetLabel.Size = new System.Drawing.Size(81, 13);
      this.minTimeOffsetLabel.TabIndex = 9;
      this.minTimeOffsetLabel.Text = "Min. time offset:";
      // 
      // maxTimeOffsetLabel
      // 
      this.maxTimeOffsetLabel.AutoSize = true;
      this.maxTimeOffsetLabel.Location = new System.Drawing.Point(3, 156);
      this.maxTimeOffsetLabel.Name = "maxTimeOffsetLabel";
      this.maxTimeOffsetLabel.Size = new System.Drawing.Size(84, 13);
      this.maxTimeOffsetLabel.TabIndex = 11;
      this.maxTimeOffsetLabel.Text = "Max. time offset:";
      // 
      // minTimeOffsetTextBox
      // 
      this.minTimeOffsetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.minTimeOffsetTextBox.Location = new System.Drawing.Point(93, 127);
      this.minTimeOffsetTextBox.Name = "minTimeOffsetTextBox";
      this.minTimeOffsetTextBox.Size = new System.Drawing.Size(327, 20);
      this.minTimeOffsetTextBox.TabIndex = 10;
      this.minTimeOffsetTextBox.TextChanged += new System.EventHandler(this.minTimeOffsetTextBox_TextChanged);
      // 
      // maxTimeOffsetTextBox
      // 
      this.maxTimeOffsetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.maxTimeOffsetTextBox.Location = new System.Drawing.Point(93, 153);
      this.maxTimeOffsetTextBox.Name = "maxTimeOffsetTextBox";
      this.maxTimeOffsetTextBox.Size = new System.Drawing.Size(327, 20);
      this.maxTimeOffsetTextBox.TabIndex = 12;
      this.maxTimeOffsetTextBox.TextChanged += new System.EventHandler(this.maxTimeOffsetTextBox_TextChanged);
      // 
      // LaggedVariableView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.minTimeOffsetTextBox);
      this.Controls.Add(this.maxTimeOffsetLabel);
      this.Controls.Add(this.minTimeOffsetLabel);
      this.Controls.Add(this.maxTimeOffsetTextBox);
      this.Name = "LaggedVariableView";
      this.Size = new System.Drawing.Size(420, 411);
      this.Controls.SetChildIndex(this.enabledCheckBox, 0);
      this.Controls.SetChildIndex(this.maxTimeOffsetTextBox, 0);
      this.Controls.SetChildIndex(this.minTimeOffsetLabel, 0);
      this.Controls.SetChildIndex(this.maxTimeOffsetLabel, 0);
      this.Controls.SetChildIndex(this.minTimeOffsetTextBox, 0);
      this.Controls.SetChildIndex(this.maximumArityLabel, 0);
      this.Controls.SetChildIndex(this.maximumArityTextBox, 0);
      this.Controls.SetChildIndex(this.minimumArityLabel, 0);
      this.Controls.SetChildIndex(this.minimumArityTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.initializationGroupBox.ResumeLayout(false);
      this.initializationGroupBox.PerformLayout();
      this.mutationGroupBox.ResumeLayout(false);
      this.mutationGroupBox.PerformLayout();
      this.parametersTabPage.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label minTimeOffsetLabel;
    private System.Windows.Forms.Label maxTimeOffsetLabel;
    private System.Windows.Forms.TextBox minTimeOffsetTextBox;
    private System.Windows.Forms.TextBox maxTimeOffsetTextBox;

  }
}
