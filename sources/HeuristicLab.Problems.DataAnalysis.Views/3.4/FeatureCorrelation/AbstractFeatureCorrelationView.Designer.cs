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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AbstractFeatureCorrelationView));
      this.DataGridView = new System.Windows.Forms.DataGridView();
      this.HeatMapProgressBar = new System.Windows.Forms.ProgressBar();
      this.PartitionComboBox = new System.Windows.Forms.ComboBox();
      this.CorrelationCalcLabel = new System.Windows.Forms.Label();
      this.CorrelationCalcComboBox = new System.Windows.Forms.ComboBox();
      this.PartitionLabel = new System.Windows.Forms.Label();
      this.minimumLabel = new System.Windows.Forms.Label();
      this.maximumLabel = new System.Windows.Forms.Label();
      this.PictureBox = new System.Windows.Forms.PictureBox();
      this.SplitContainer = new System.Windows.Forms.SplitContainer();
      this.CalculatingPanel = new System.Windows.Forms.Panel();
      this.CalculatingLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
      this.SplitContainer.Panel1.SuspendLayout();
      this.SplitContainer.Panel2.SuspendLayout();
      this.SplitContainer.SuspendLayout();
      this.CalculatingPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // DataGridView
      // 
      this.DataGridView.AllowUserToAddRows = false;
      this.DataGridView.AllowUserToDeleteRows = false;
      this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.DataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.DataGridView.Location = new System.Drawing.Point(0, 0);
      this.DataGridView.Name = "DataGridView";
      this.DataGridView.ReadOnly = true;
      this.DataGridView.Size = new System.Drawing.Size(475, 301);
      this.DataGridView.TabIndex = 0;
      this.DataGridView.VirtualMode = true;
      this.DataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.DataGridView_CellPainting);
      this.DataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.DataGridView_CellValueNeeded);
      this.DataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_ColumnHeaderMouseClick);
      this.DataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGridView_KeyDown);
      // 
      // HeatMapProgressBar
      // 
      this.HeatMapProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.HeatMapProgressBar.Location = new System.Drawing.Point(25, 46);
      this.HeatMapProgressBar.Name = "HeatMapProgressBar";
      this.HeatMapProgressBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.HeatMapProgressBar.Size = new System.Drawing.Size(154, 21);
      this.HeatMapProgressBar.TabIndex = 9;
      // 
      // PartitionComboBox
      // 
      this.PartitionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.PartitionComboBox.FormattingEnabled = true;
      this.PartitionComboBox.Location = new System.Drawing.Point(333, 3);
      this.PartitionComboBox.Name = "PartitionComboBox";
      this.PartitionComboBox.Size = new System.Drawing.Size(142, 21);
      this.PartitionComboBox.TabIndex = 8;
      this.PartitionComboBox.SelectionChangeCommitted += new System.EventHandler(this.PartitionComboBox_SelectedChangeCommitted);
      // 
      // CorrelationCalcLabel
      // 
      this.CorrelationCalcLabel.AutoSize = true;
      this.CorrelationCalcLabel.Location = new System.Drawing.Point(0, 6);
      this.CorrelationCalcLabel.Name = "CorrelationCalcLabel";
      this.CorrelationCalcLabel.Size = new System.Drawing.Size(104, 13);
      this.CorrelationCalcLabel.TabIndex = 7;
      this.CorrelationCalcLabel.Text = "Correlation Measure:";
      // 
      // CorrelationCalcComboBox
      // 
      this.CorrelationCalcComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.CorrelationCalcComboBox.FormattingEnabled = true;
      this.CorrelationCalcComboBox.Location = new System.Drawing.Point(110, 3);
      this.CorrelationCalcComboBox.Name = "CorrelationCalcComboBox";
      this.CorrelationCalcComboBox.Size = new System.Drawing.Size(163, 21);
      this.CorrelationCalcComboBox.TabIndex = 6;
      this.CorrelationCalcComboBox.SelectionChangeCommitted += new System.EventHandler(this.CorrelationMeasureComboBox_SelectedChangeCommitted);
      // 
      // PartitionLabel
      // 
      this.PartitionLabel.AutoSize = true;
      this.PartitionLabel.Location = new System.Drawing.Point(279, 6);
      this.PartitionLabel.Name = "PartitionLabel";
      this.PartitionLabel.Size = new System.Drawing.Size(48, 13);
      this.PartitionLabel.TabIndex = 10;
      this.PartitionLabel.Text = "Partition:";
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
      this.PictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.PictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.PictureBox.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox.Image")));
      this.PictureBox.Location = new System.Drawing.Point(507, 30);
      this.PictureBox.Name = "PictureBox";
      this.PictureBox.Size = new System.Drawing.Size(35, 281);
      this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.PictureBox.TabIndex = 15;
      this.PictureBox.TabStop = false;
      // 
      // SplitContainer
      // 
      this.SplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.SplitContainer.IsSplitterFixed = true;
      this.SplitContainer.Location = new System.Drawing.Point(3, 3);
      this.SplitContainer.Name = "SplitContainer";
      this.SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // SplitContainer.Panel1
      // 
      this.SplitContainer.Panel1.Controls.Add(this.CorrelationCalcLabel);
      this.SplitContainer.Panel1.Controls.Add(this.CorrelationCalcComboBox);
      this.SplitContainer.Panel1.Controls.Add(this.PartitionComboBox);
      this.SplitContainer.Panel1.Controls.Add(this.PartitionLabel);
      // 
      // SplitContainer.Panel2
      // 
      this.SplitContainer.Panel2.Controls.Add(this.CalculatingPanel);
      this.SplitContainer.Panel2.Controls.Add(this.DataGridView);
      this.SplitContainer.Size = new System.Drawing.Size(475, 330);
      this.SplitContainer.SplitterDistance = 25;
      this.SplitContainer.TabIndex = 16;
      // 
      // CalculatingPanel
      // 
      this.CalculatingPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.CalculatingPanel.Controls.Add(this.CalculatingLabel);
      this.CalculatingPanel.Controls.Add(this.HeatMapProgressBar);
      this.CalculatingPanel.Location = new System.Drawing.Point(138, 95);
      this.CalculatingPanel.Name = "CalculatingPanel";
      this.CalculatingPanel.Size = new System.Drawing.Size(200, 81);
      this.CalculatingPanel.TabIndex = 10;
      // 
      // CalculatingLabel
      // 
      this.CalculatingLabel.AutoSize = true;
      this.CalculatingLabel.Location = new System.Drawing.Point(42, 19);
      this.CalculatingLabel.Name = "CalculatingLabel";
      this.CalculatingLabel.Size = new System.Drawing.Size(120, 13);
      this.CalculatingLabel.TabIndex = 10;
      this.CalculatingLabel.Text = "Calculating correlation...";
      // 
      // AbstractFeatureCorrelationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.SplitContainer);
      this.Controls.Add(this.minimumLabel);
      this.Controls.Add(this.PictureBox);
      this.Controls.Add(this.maximumLabel);
      this.Name = "AbstractFeatureCorrelationView";
      this.Size = new System.Drawing.Size(569, 336);
      ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
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

    protected System.Windows.Forms.DataGridView DataGridView;
    protected System.Windows.Forms.ProgressBar HeatMapProgressBar;
    protected System.Windows.Forms.ComboBox PartitionComboBox;
    protected System.Windows.Forms.Label CorrelationCalcLabel;
    protected System.Windows.Forms.ComboBox CorrelationCalcComboBox;
    protected System.Windows.Forms.Label PartitionLabel;
    protected System.Windows.Forms.Label minimumLabel;
    protected System.Windows.Forms.Label maximumLabel;
    protected System.Windows.Forms.PictureBox PictureBox;
    protected System.Windows.Forms.SplitContainer SplitContainer;
    protected System.Windows.Forms.Panel CalculatingPanel;
    protected System.Windows.Forms.Label CalculatingLabel;

  }
}
