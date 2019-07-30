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
#endregion

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  partial class NCADimensionReductionView {
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
      this.messageLabel = new System.Windows.Forms.Label();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.rangeLabel = new System.Windows.Forms.Label();
      this.rangeComboBox = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // messageLabel
      // 
      this.messageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.messageLabel.Location = new System.Drawing.Point(0, 0);
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Size = new System.Drawing.Size(300, 167);
      this.messageLabel.TabIndex = 0;
      this.messageLabel.Text = "Visualization is not supported for more than two dimensions.";
      this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.IsSplitterFixed = true;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.rangeComboBox);
      this.splitContainer.Panel1.Controls.Add(this.rangeLabel);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.messageLabel);
      this.splitContainer.Size = new System.Drawing.Size(300, 198);
      this.splitContainer.SplitterDistance = 27;
      this.splitContainer.TabIndex = 1;
      // 
      // rangeLabel
      // 
      this.rangeLabel.AutoSize = true;
      this.rangeLabel.Location = new System.Drawing.Point(3, 6);
      this.rangeLabel.Name = "rangeLabel";
      this.rangeLabel.Size = new System.Drawing.Size(47, 13);
      this.rangeLabel.TabIndex = 0;
      this.rangeLabel.Text = "Samples";
      // 
      // rangeComboBox
      // 
      this.rangeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rangeComboBox.FormattingEnabled = true;
      this.rangeComboBox.Items.AddRange(new object[] {
            "Training",
            "Test",
            "All samples"});
      this.rangeComboBox.Location = new System.Drawing.Point(56, 3);
      this.rangeComboBox.Name = "rangeComboBox";
      this.rangeComboBox.Size = new System.Drawing.Size(146, 21);
      this.rangeComboBox.TabIndex = 1;
      this.rangeComboBox.SelectedIndexChanged += new System.EventHandler(this.rangeComboBox_SelectedIndexChanged);
      // 
      // NCADimensionalityReductionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "NCADimensionalityReductionView";
      this.Size = new System.Drawing.Size(300, 198);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label messageLabel;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.ComboBox rangeComboBox;
    private System.Windows.Forms.Label rangeLabel;
  }
}
