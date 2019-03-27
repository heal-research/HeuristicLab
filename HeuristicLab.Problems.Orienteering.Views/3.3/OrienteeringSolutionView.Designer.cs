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

namespace HeuristicLab.Problems.Orienteering.Views {
  partial class OrienteeringSolutionView {
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
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.visualizationTabPage = new System.Windows.Forms.TabPage();
      this.valueTabPage = new System.Windows.Forms.TabPage();
      this.tourGroupBox = new System.Windows.Forms.GroupBox();
      this.tourViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.qualityValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.distanceValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.penaltyValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.qualityLabel = new System.Windows.Forms.Label();
      this.distanceLabel = new System.Windows.Forms.Label();
      this.penaltyLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.valueTabPage.SuspendLayout();
      this.tourGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // pictureBox
      // 
      this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox.BackColor = System.Drawing.Color.White;
      this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox.Location = new System.Drawing.Point(6, 6);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(403, 271);
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      this.pictureBox.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
      // 
      // tabControl
      // 
      this.tabControl.AllowDrop = true;
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Controls.Add(this.valueTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 3);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(423, 305);
      this.tabControl.TabIndex = 0;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.visualizationTabPage.Controls.Add(this.pictureBox);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(415, 279);
      this.visualizationTabPage.TabIndex = 0;
      this.visualizationTabPage.Text = "Visualization";
      // 
      // valueTabPage
      // 
      this.valueTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.valueTabPage.Controls.Add(this.tourGroupBox);
      this.valueTabPage.Location = new System.Drawing.Point(4, 22);
      this.valueTabPage.Name = "valueTabPage";
      this.valueTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.valueTabPage.Size = new System.Drawing.Size(211, 370);
      this.valueTabPage.TabIndex = 1;
      this.valueTabPage.Text = "Value";
      // 
      // tourGroupBox
      // 
      this.tourGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tourGroupBox.Controls.Add(this.tourViewHost);
      this.tourGroupBox.Location = new System.Drawing.Point(6, 6);
      this.tourGroupBox.Name = "tourGroupBox";
      this.tourGroupBox.Size = new System.Drawing.Size(199, 358);
      this.tourGroupBox.TabIndex = 0;
      this.tourGroupBox.TabStop = false;
      this.tourGroupBox.Text = "Tour";
      // 
      // tourViewHost
      // 
      this.tourViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tourViewHost.Caption = "View";
      this.tourViewHost.Content = null;
      this.tourViewHost.Enabled = false;
      this.tourViewHost.Location = new System.Drawing.Point(6, 19);
      this.tourViewHost.Name = "tourViewHost";
      this.tourViewHost.ReadOnly = false;
      this.tourViewHost.Size = new System.Drawing.Size(187, 333);
      this.tourViewHost.TabIndex = 0;
      this.tourViewHost.ViewsLabelVisible = true;
      this.tourViewHost.ViewType = null;
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.penaltyLabel);
      this.splitContainer.Panel1.Controls.Add(this.distanceLabel);
      this.splitContainer.Panel1.Controls.Add(this.qualityLabel);
      this.splitContainer.Panel1.Controls.Add(this.qualityValueView);
      this.splitContainer.Panel1.Controls.Add(this.distanceValueView);
      this.splitContainer.Panel1.Controls.Add(this.penaltyValueView);
      this.splitContainer.Panel1.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
      this.splitContainer.Panel1MinSize = 0;
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.tabControl);
      this.splitContainer.Size = new System.Drawing.Size(423, 402);
      this.splitContainer.SplitterDistance = 87;
      this.splitContainer.TabIndex = 0;
      // 
      // qualityValueView
      // 
      this.qualityValueView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityValueView.Caption = "StringConvertibleValue View";
      this.qualityValueView.Content = null;
      this.qualityValueView.LabelVisible = false;
      this.qualityValueView.Location = new System.Drawing.Point(65, 7);
      this.qualityValueView.Name = "qualityValueView";
      this.qualityValueView.ReadOnly = false;
      this.qualityValueView.Size = new System.Drawing.Size(354, 21);
      this.qualityValueView.TabIndex = 0;
      // 
      // distanceValueView
      // 
      this.distanceValueView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.distanceValueView.Caption = "StringConvertibleValue View";
      this.distanceValueView.Content = null;
      this.distanceValueView.LabelVisible = false;
      this.distanceValueView.Location = new System.Drawing.Point(65, 34);
      this.distanceValueView.Name = "distanceValueView";
      this.distanceValueView.ReadOnly = false;
      this.distanceValueView.Size = new System.Drawing.Size(354, 21);
      this.distanceValueView.TabIndex = 0;
      // 
      // penaltyValueView
      // 
      this.penaltyValueView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.penaltyValueView.Caption = "StringConvertibleValue View";
      this.penaltyValueView.Content = null;
      this.penaltyValueView.LabelVisible = false;
      this.penaltyValueView.Location = new System.Drawing.Point(65, 61);
      this.penaltyValueView.Name = "penaltyValueView";
      this.penaltyValueView.ReadOnly = false;
      this.penaltyValueView.Size = new System.Drawing.Size(354, 21);
      this.penaltyValueView.TabIndex = 0;
      // 
      // qualityLabel
      // 
      this.qualityLabel.AutoSize = true;
      this.qualityLabel.Location = new System.Drawing.Point(7, 11);
      this.qualityLabel.Name = "qualityLabel";
      this.qualityLabel.Size = new System.Drawing.Size(42, 13);
      this.qualityLabel.TabIndex = 1;
      this.qualityLabel.Text = "Quality:";
      // 
      // distanceLabel
      // 
      this.distanceLabel.AutoSize = true;
      this.distanceLabel.Location = new System.Drawing.Point(7, 38);
      this.distanceLabel.Name = "distanceLabel";
      this.distanceLabel.Size = new System.Drawing.Size(52, 13);
      this.distanceLabel.TabIndex = 1;
      this.distanceLabel.Text = "Distance:";
      // 
      // penaltyLabel
      // 
      this.penaltyLabel.AutoSize = true;
      this.penaltyLabel.Location = new System.Drawing.Point(7, 64);
      this.penaltyLabel.Name = "penaltyLabel";
      this.penaltyLabel.Size = new System.Drawing.Size(45, 13);
      this.penaltyLabel.TabIndex = 1;
      this.penaltyLabel.Text = "Penalty:";
      // 
      // OrienteeringSolutionView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "OrienteeringSolutionView";
      this.Size = new System.Drawing.Size(423, 402);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      this.valueTabPage.ResumeLayout(false);
      this.tourGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox;
    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage visualizationTabPage;
    private System.Windows.Forms.TabPage valueTabPage;
    private System.Windows.Forms.GroupBox tourGroupBox;
    private HeuristicLab.MainForm.WindowsForms.ViewHost tourViewHost;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Label penaltyLabel;
    private System.Windows.Forms.Label distanceLabel;
    private System.Windows.Forms.Label qualityLabel;
    private Data.Views.StringConvertibleValueView qualityValueView;
    private Data.Views.StringConvertibleValueView distanceValueView;
    private Data.Views.StringConvertibleValueView penaltyValueView;
  }
}
