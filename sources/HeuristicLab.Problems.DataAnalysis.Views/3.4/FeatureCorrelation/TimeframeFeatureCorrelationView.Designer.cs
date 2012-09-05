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
      this.VariableSelectionLabel = new System.Windows.Forms.Label();
      this.VariableSelectionComboBox = new System.Windows.Forms.ComboBox();
      this.TimeFrameLabel = new System.Windows.Forms.Label();
      this.TimeframeComboBox = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
      this.SplitContainer.Panel1.SuspendLayout();
      this.SplitContainer.Panel2.SuspendLayout();
      this.SplitContainer.SuspendLayout();
      this.CalculatingPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // PartitionComboBox
      // 
      this.PartitionComboBox.Location = new System.Drawing.Point(344, 3);
      this.PartitionComboBox.Size = new System.Drawing.Size(131, 21);
      // 
      // maximumLabel
      // 
      this.maximumLabel.Location = new System.Drawing.Point(487, 3);
      // 
      // PictureBox
      // 
      this.PictureBox.Location = new System.Drawing.Point(506, 31);
      this.PictureBox.Size = new System.Drawing.Size(35, 280);
      // 
      // SplitContainer
      // 
      // 
      // SplitContainer.Panel1
      // 
      this.SplitContainer.Panel1.Controls.Add(this.VariableSelectionComboBox);
      this.SplitContainer.Panel1.Controls.Add(this.TimeframeComboBox);
      this.SplitContainer.Panel1.Controls.Add(this.TimeFrameLabel);
      this.SplitContainer.Panel1.Controls.Add(this.VariableSelectionLabel);
      this.SplitContainer.SplitterDistance = 52;
      // 
      // CalculatingPanel
      // 
      this.CalculatingPanel.Location = new System.Drawing.Point(138, 82);
      // 
      // VariableSelectionLabel
      // 
      this.VariableSelectionLabel.AutoSize = true;
      this.VariableSelectionLabel.Location = new System.Drawing.Point(0, 33);
      this.VariableSelectionLabel.Name = "VariableSelectionLabel";
      this.VariableSelectionLabel.Size = new System.Drawing.Size(81, 13);
      this.VariableSelectionLabel.TabIndex = 16;
      this.VariableSelectionLabel.Text = "Select Variable:";
      // 
      // VariableSelectionComboBox
      // 
      this.VariableSelectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.VariableSelectionComboBox.FormattingEnabled = true;
      this.VariableSelectionComboBox.Location = new System.Drawing.Point(110, 30);
      this.VariableSelectionComboBox.Name = "VariableSelectionComboBox";
      this.VariableSelectionComboBox.Size = new System.Drawing.Size(163, 21);
      this.VariableSelectionComboBox.TabIndex = 17;
      this.VariableSelectionComboBox.SelectionChangeCommitted += new System.EventHandler(this.VariableSelectionComboBox_SelectedChangeCommitted);
      // 
      // TimeFrameLabel
      // 
      this.TimeFrameLabel.AutoSize = true;
      this.TimeFrameLabel.Location = new System.Drawing.Point(279, 33);
      this.TimeFrameLabel.Name = "TimeFrameLabel";
      this.TimeFrameLabel.Size = new System.Drawing.Size(59, 13);
      this.TimeFrameLabel.TabIndex = 18;
      this.TimeFrameLabel.Text = "Timeframe:";
      // 
      // TimeframeComboBox
      // 
      this.TimeframeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.TimeframeComboBox.FormattingEnabled = true;
      this.TimeframeComboBox.Location = new System.Drawing.Point(344, 30);
      this.TimeframeComboBox.Name = "TimeframeComboBox";
      this.TimeframeComboBox.Size = new System.Drawing.Size(131, 21);
      this.TimeframeComboBox.TabIndex = 19;
      this.TimeframeComboBox.SelectionChangeCommitted += new System.EventHandler(this.TimeframeComboBox_SelectedChangeCommitted);
      // 
      // TimeframeFeatureCorrelationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "TimeframeFeatureCorrelationView";
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
      this.SplitContainer.Panel1.ResumeLayout(false);
      this.SplitContainer.Panel1.PerformLayout();
      this.SplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
      this.SplitContainer.ResumeLayout(false);
      this.CalculatingPanel.ResumeLayout(false);
      this.CalculatingPanel.PerformLayout();
      this.ResumeLayout(false);

    }
    #endregion

    protected System.Windows.Forms.Label VariableSelectionLabel;
    protected System.Windows.Forms.ComboBox VariableSelectionComboBox;
    protected System.Windows.Forms.Label TimeFrameLabel;
    protected System.Windows.Forms.ComboBox TimeframeComboBox;
  }
}
