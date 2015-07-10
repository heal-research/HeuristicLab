#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class ClusteringSolutionVisualizationView {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.rangeComboBox = new System.Windows.Forms.ComboBox();
      this.rangeLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
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
      this.splitContainer.Size = new System.Drawing.Size(438, 198);
      this.splitContainer.SplitterDistance = 27;
      this.splitContainer.TabIndex = 1;
      // 
      // rangeComboBox
      // 
      this.rangeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rangeComboBox.FormattingEnabled = true;
      this.rangeComboBox.Items.AddRange(new object[] {
            "Training samples",
            "Test samples",
            "All samples"});
      this.rangeComboBox.Location = new System.Drawing.Point(56, 3);
      this.rangeComboBox.Name = "rangeComboBox";
      this.rangeComboBox.Size = new System.Drawing.Size(146, 21);
      this.rangeComboBox.TabIndex = 1;
      this.rangeComboBox.SelectedIndexChanged += new System.EventHandler(this.rangeComboBox_SelectedIndexChanged);
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
      // ClusteringSolutionVisualizationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "ClusteringSolutionVisualizationView";
      this.Size = new System.Drawing.Size(438, 198);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.ComboBox rangeComboBox;
    private System.Windows.Forms.Label rangeLabel;
  }
}
