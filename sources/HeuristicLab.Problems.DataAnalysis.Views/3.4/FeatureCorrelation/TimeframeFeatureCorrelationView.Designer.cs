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

namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class TimeframeFeatureCorrelationView {
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
      this.variableSelectionLabel = new System.Windows.Forms.Label();
      this.variableSelectionComboBox = new System.Windows.Forms.ComboBox();
      this.timeFrameLabel = new System.Windows.Forms.Label();
      this.timeframeTextbox = new System.Windows.Forms.TextBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.calculatingPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // PartitionComboBox
      // 
      this.partitionComboBox.Location = new System.Drawing.Point(344, 3);
      this.partitionComboBox.Size = new System.Drawing.Size(131, 21);
      // 
      // maximumLabel
      // 
      this.maximumLabel.Location = new System.Drawing.Point(487, 3);
      // 
      // PictureBox
      // 
      this.pictureBox.Location = new System.Drawing.Point(506, 31);
      this.pictureBox.Size = new System.Drawing.Size(35, 280);
      // 
      // SplitContainer
      // 
      // 
      // SplitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.variableSelectionComboBox);
      this.splitContainer.Panel1.Controls.Add(this.timeframeTextbox);
      this.splitContainer.Panel1.Controls.Add(this.timeFrameLabel);
      this.splitContainer.Panel1.Controls.Add(this.variableSelectionLabel);
      this.splitContainer.SplitterDistance = 52;
      // 
      // CalculatingPanel
      // 
      this.calculatingPanel.Location = new System.Drawing.Point(138, 82);
      // 
      // VariableSelectionLabel
      // 
      this.variableSelectionLabel.AutoSize = true;
      this.variableSelectionLabel.Location = new System.Drawing.Point(0, 33);
      this.variableSelectionLabel.Name = "VariableSelectionLabel";
      this.variableSelectionLabel.Size = new System.Drawing.Size(81, 13);
      this.variableSelectionLabel.TabIndex = 16;
      this.variableSelectionLabel.Text = "Select Variable:";
      // 
      // VariableSelectionComboBox
      // 
      this.variableSelectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.variableSelectionComboBox.FormattingEnabled = true;
      this.variableSelectionComboBox.Location = new System.Drawing.Point(110, 30);
      this.variableSelectionComboBox.Name = "VariableSelectionComboBox";
      this.variableSelectionComboBox.Size = new System.Drawing.Size(163, 21);
      this.variableSelectionComboBox.TabIndex = 17;
      this.variableSelectionComboBox.SelectionChangeCommitted += new System.EventHandler(this.VariableSelectionComboBox_SelectedChangeCommitted);
      // 
      // TimeFrameLabel
      // 
      this.timeFrameLabel.AutoSize = true;
      this.timeFrameLabel.Location = new System.Drawing.Point(279, 33);
      this.timeFrameLabel.Name = "TimeFrameLabel";
      this.timeFrameLabel.Size = new System.Drawing.Size(59, 13);
      this.timeFrameLabel.TabIndex = 18;
      this.timeFrameLabel.Text = "Timeframe:";
      // 
      // TimeframeComboBox
      // 
      this.timeframeTextbox.Location = new System.Drawing.Point(344, 30);
      this.timeframeTextbox.Name = "TimeframeTextbox";
      this.timeframeTextbox.Size = new System.Drawing.Size(131, 21);
      this.timeframeTextbox.TabIndex = 19;
      this.timeframeTextbox.Text = "5";
      this.timeframeTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(TimeframeTextbox_KeyDown);
      this.timeframeTextbox.Validating += new System.ComponentModel.CancelEventHandler(TimeframeTextbox_Validating);
      this.timeframeTextbox.Validated += new System.EventHandler(TimeframeTextbox_Validated);
      // 
      // TimeframeFeatureCorrelationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "TimeframeFeatureCorrelationView";
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.calculatingPanel.ResumeLayout(false);
      this.calculatingPanel.PerformLayout();
      this.ResumeLayout(false);

    }
    #endregion

    protected System.Windows.Forms.Label variableSelectionLabel;
    protected System.Windows.Forms.ComboBox variableSelectionComboBox;
    protected System.Windows.Forms.Label timeFrameLabel;
    protected System.Windows.Forms.TextBox timeframeTextbox;
    protected System.Windows.Forms.ErrorProvider errorProvider;
  }
}
