#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class AbstractFeatureCorrelationView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AbstractFeatureCorrelationView));
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.partitionComboBox = new System.Windows.Forms.ComboBox();
      this.correlationCalcLabel = new System.Windows.Forms.Label();
      this.correlationCalcComboBox = new System.Windows.Forms.ComboBox();
      this.partitionLabel = new System.Windows.Forms.Label();
      this.minimumLabel = new System.Windows.Forms.Label();
      this.maximumLabel = new System.Windows.Forms.Label();
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.progressPanel = new System.Windows.Forms.Panel();
      this.progressLabel = new System.Windows.Forms.Label();
      this.dataView = new HeuristicLab.Data.Views.EnhancedStringConvertibleMatrixView();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.progressPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // HeatMapProgressBar
      // 
      this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar.Location = new System.Drawing.Point(25, 46);
      this.progressBar.Name = "HeatMapProgressBar";
      this.progressBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.progressBar.Size = new System.Drawing.Size(154, 21);
      this.progressBar.TabIndex = 9;
      // 
      // PartitionComboBox
      // 
      this.partitionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.partitionComboBox.FormattingEnabled = true;
      this.partitionComboBox.Location = new System.Drawing.Point(333, 3);
      this.partitionComboBox.Name = "PartitionComboBox";
      this.partitionComboBox.Size = new System.Drawing.Size(142, 21);
      this.partitionComboBox.TabIndex = 8;
      this.partitionComboBox.SelectionChangeCommitted += new System.EventHandler(this.PartitionComboBox_SelectedChangeCommitted);
      // 
      // CorrelationCalcLabel
      // 
      this.correlationCalcLabel.AutoSize = true;
      this.correlationCalcLabel.Location = new System.Drawing.Point(0, 6);
      this.correlationCalcLabel.Name = "CorrelationCalcLabel";
      this.correlationCalcLabel.Size = new System.Drawing.Size(104, 13);
      this.correlationCalcLabel.TabIndex = 7;
      this.correlationCalcLabel.Text = "Correlation Measure:";
      // 
      // CorrelationCalcComboBox
      // 
      this.correlationCalcComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.correlationCalcComboBox.FormattingEnabled = true;
      this.correlationCalcComboBox.Location = new System.Drawing.Point(110, 3);
      this.correlationCalcComboBox.Name = "CorrelationCalcComboBox";
      this.correlationCalcComboBox.Size = new System.Drawing.Size(163, 21);
      this.correlationCalcComboBox.TabIndex = 6;
      this.correlationCalcComboBox.SelectionChangeCommitted += new System.EventHandler(this.CorrelationMeasureComboBox_SelectedChangeCommitted);
      // 
      // PartitionLabel
      // 
      this.partitionLabel.AutoSize = true;
      this.partitionLabel.Location = new System.Drawing.Point(279, 6);
      this.partitionLabel.Name = "PartitionLabel";
      this.partitionLabel.Size = new System.Drawing.Size(48, 13);
      this.partitionLabel.TabIndex = 10;
      this.partitionLabel.Text = "Partition:";
      // 
      // minimumLabel
      // 
      this.minimumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.minimumLabel.BackColor = System.Drawing.Color.Transparent;
      this.minimumLabel.Location = new System.Drawing.Point(487, 314);
      this.minimumLabel.Name = "minimumLabel";
      this.minimumLabel.Size = new System.Drawing.Size(73, 19);
      this.minimumLabel.TabIndex = 13;
      this.minimumLabel.Text = "0.0";
      this.minimumLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // maximumLabel
      // 
      this.maximumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.maximumLabel.BackColor = System.Drawing.Color.Transparent;
      this.maximumLabel.Location = new System.Drawing.Point(487, 2);
      this.maximumLabel.Name = "maximumLabel";
      this.maximumLabel.Size = new System.Drawing.Size(73, 25);
      this.maximumLabel.TabIndex = 12;
      this.maximumLabel.Text = "1.0";
      this.maximumLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      // 
      // PictureBox
      // 
      this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox.Image")));
      this.pictureBox.Location = new System.Drawing.Point(507, 30);
      this.pictureBox.Name = "PictureBox";
      this.pictureBox.Size = new System.Drawing.Size(35, 281);
      this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox.TabIndex = 15;
      this.pictureBox.TabStop = false;
      // 
      // SplitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.IsSplitterFixed = true;
      this.splitContainer.Location = new System.Drawing.Point(3, 3);
      this.splitContainer.Name = "SplitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // SplitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.correlationCalcLabel);
      this.splitContainer.Panel1.Controls.Add(this.correlationCalcComboBox);
      this.splitContainer.Panel1.Controls.Add(this.partitionComboBox);
      this.splitContainer.Panel1.Controls.Add(this.partitionLabel);
      // 
      // SplitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.progressPanel);
      this.splitContainer.Panel2.Controls.Add(this.dataView);
      this.splitContainer.Size = new System.Drawing.Size(475, 330);
      this.splitContainer.SplitterDistance = 25;
      this.splitContainer.TabIndex = 16;
      // 
      // CalculatingPanel
      // 
      this.progressPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.progressPanel.Controls.Add(this.progressLabel);
      this.progressPanel.Controls.Add(this.progressBar);
      this.progressPanel.Location = new System.Drawing.Point(138, 95);
      this.progressPanel.Name = "CalculatingPanel";
      this.progressPanel.Size = new System.Drawing.Size(200, 81);
      this.progressPanel.TabIndex = 10;
      // 
      // CalculatingLabel
      // 
      this.progressLabel.AutoSize = true;
      this.progressLabel.Location = new System.Drawing.Point(42, 19);
      this.progressLabel.Name = "CalculatingLabel";
      this.progressLabel.Size = new System.Drawing.Size(120, 13);
      this.progressLabel.TabIndex = 10;
      this.progressLabel.Text = "Calculating correlation...";
      // 
      // DataGridView
      // 
      this.dataView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataView.Location = new System.Drawing.Point(0, 0);
      this.dataView.Name = "DataView";
      this.dataView.ReadOnly = true;
      this.dataView.Size = new System.Drawing.Size(475, 301);
      this.dataView.TabIndex = 0;
      // 
      // AbstractFeatureCorrelationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.minimumLabel);
      this.Controls.Add(this.pictureBox);
      this.Controls.Add(this.maximumLabel);
      this.Name = "AbstractFeatureCorrelationView";
      this.Size = new System.Drawing.Size(569, 336);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.progressPanel.ResumeLayout(false);
      this.progressPanel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.ProgressBar progressBar;
    protected System.Windows.Forms.ComboBox partitionComboBox;
    protected System.Windows.Forms.Label correlationCalcLabel;
    protected System.Windows.Forms.ComboBox correlationCalcComboBox;
    protected System.Windows.Forms.Label partitionLabel;
    protected System.Windows.Forms.Label minimumLabel;
    protected System.Windows.Forms.Label maximumLabel;
    protected System.Windows.Forms.PictureBox pictureBox;
    protected System.Windows.Forms.SplitContainer splitContainer;
    protected System.Windows.Forms.Panel progressPanel;
    protected System.Windows.Forms.Label progressLabel;
    protected HeuristicLab.Data.Views.EnhancedStringConvertibleMatrixView dataView;

  }
}
