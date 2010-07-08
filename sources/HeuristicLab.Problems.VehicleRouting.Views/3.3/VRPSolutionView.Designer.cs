#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Views {
  partial class VRPSolutionView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.visualizationTabPage = new System.Windows.Forms.TabPage();
      this.valueTabPage = new System.Windows.Forms.TabPage();
      this.tourGroupBox = new System.Windows.Forms.GroupBox();
      this.tourViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.travelTimeViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.label6 = new System.Windows.Forms.Label();
      this.tardinessViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.label5 = new System.Windows.Forms.Label();
      this.overloadViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.label4 = new System.Windows.Forms.Label();
      this.vehicleUtilizationViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.qualityViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.label1 = new System.Windows.Forms.Label();
      this.distanceViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.valueTabPage.SuspendLayout();
      this.tourGroupBox.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
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
      this.pictureBox.Size = new System.Drawing.Size(403, 386);
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      this.pictureBox.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Controls.Add(this.valueTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(423, 424);
      this.tabControl.TabIndex = 0;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Controls.Add(this.pictureBox);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(415, 398);
      this.visualizationTabPage.TabIndex = 0;
      this.visualizationTabPage.Text = "Visualization";
      this.visualizationTabPage.UseVisualStyleBackColor = true;
      // 
      // valueTabPage
      // 
      this.valueTabPage.Controls.Add(this.tourGroupBox);
      this.valueTabPage.Location = new System.Drawing.Point(4, 22);
      this.valueTabPage.Name = "valueTabPage";
      this.valueTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.valueTabPage.Size = new System.Drawing.Size(415, 398);
      this.valueTabPage.TabIndex = 1;
      this.valueTabPage.Text = "Value";
      this.valueTabPage.UseVisualStyleBackColor = true;
      // 
      // tourGroupBox
      // 
      this.tourGroupBox.Controls.Add(this.tourViewHost);
      this.tourGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tourGroupBox.Location = new System.Drawing.Point(3, 3);
      this.tourGroupBox.Name = "tourGroupBox";
      this.tourGroupBox.Size = new System.Drawing.Size(409, 392);
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
      this.tourViewHost.Location = new System.Drawing.Point(6, 19);
      this.tourViewHost.Name = "tourViewHost";
      this.tourViewHost.ReadOnly = false;
      this.tourViewHost.Size = new System.Drawing.Size(397, 367);
      this.tourViewHost.TabIndex = 0;
      this.tourViewHost.ViewType = null;
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
      this.splitContainer.Panel1.Controls.Add(this.tableLayoutPanel1);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.tabControl);
      this.splitContainer.Size = new System.Drawing.Size(423, 558);
      this.splitContainer.SplitterDistance = 130;
      this.splitContainer.TabIndex = 0;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.64066F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.35934F));
      this.tableLayoutPanel1.Controls.Add(this.travelTimeViewHost, 1, 5);
      this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
      this.tableLayoutPanel1.Controls.Add(this.tardinessViewHost, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.overloadViewHost, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.vehicleUtilizationViewHost, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.qualityViewHost, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.distanceViewHost, 1, 1);
      this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 6;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(423, 121);
      this.tableLayoutPanel1.TabIndex = 2;
      // 
      // travelTimeViewHost
      // 
      this.travelTimeViewHost.Caption = "View";
      this.travelTimeViewHost.Content = null;
      this.travelTimeViewHost.Dock = System.Windows.Forms.DockStyle.Left;
      this.travelTimeViewHost.Location = new System.Drawing.Point(102, 103);
      this.travelTimeViewHost.Name = "travelTimeViewHost";
      this.travelTimeViewHost.ReadOnly = false;
      this.travelTimeViewHost.Size = new System.Drawing.Size(281, 15);
      this.travelTimeViewHost.TabIndex = 17;
      this.travelTimeViewHost.ViewType = null;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label6.Location = new System.Drawing.Point(3, 100);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(93, 21);
      this.label6.TabIndex = 16;
      this.label6.Text = "Travel Time";
      this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      // 
      // tardinessViewHost
      // 
      this.tardinessViewHost.Caption = "View";
      this.tardinessViewHost.Content = null;
      this.tardinessViewHost.Dock = System.Windows.Forms.DockStyle.Left;
      this.tardinessViewHost.Location = new System.Drawing.Point(102, 83);
      this.tardinessViewHost.Name = "tardinessViewHost";
      this.tardinessViewHost.ReadOnly = false;
      this.tardinessViewHost.Size = new System.Drawing.Size(281, 14);
      this.tardinessViewHost.TabIndex = 15;
      this.tardinessViewHost.ViewType = null;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label5.Location = new System.Drawing.Point(3, 80);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(93, 20);
      this.label5.TabIndex = 14;
      this.label5.Text = "Tardiness";
      this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      // 
      // overloadViewHost
      // 
      this.overloadViewHost.BackColor = System.Drawing.Color.Transparent;
      this.overloadViewHost.Caption = "View";
      this.overloadViewHost.Content = null;
      this.overloadViewHost.Dock = System.Windows.Forms.DockStyle.Left;
      this.overloadViewHost.Location = new System.Drawing.Point(102, 63);
      this.overloadViewHost.Name = "overloadViewHost";
      this.overloadViewHost.ReadOnly = false;
      this.overloadViewHost.Size = new System.Drawing.Size(281, 14);
      this.overloadViewHost.TabIndex = 13;
      this.overloadViewHost.ViewType = null;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label4.Location = new System.Drawing.Point(3, 60);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(93, 20);
      this.label4.TabIndex = 12;
      this.label4.Text = "Overload";
      this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      // 
      // vehicleUtilizationViewHost
      // 
      this.vehicleUtilizationViewHost.Caption = "View";
      this.vehicleUtilizationViewHost.Content = null;
      this.vehicleUtilizationViewHost.Dock = System.Windows.Forms.DockStyle.Left;
      this.vehicleUtilizationViewHost.Location = new System.Drawing.Point(102, 43);
      this.vehicleUtilizationViewHost.Name = "vehicleUtilizationViewHost";
      this.vehicleUtilizationViewHost.ReadOnly = false;
      this.vehicleUtilizationViewHost.Size = new System.Drawing.Size(281, 14);
      this.vehicleUtilizationViewHost.TabIndex = 11;
      this.vehicleUtilizationViewHost.ViewType = null;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label3.Location = new System.Drawing.Point(3, 40);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(93, 20);
      this.label3.TabIndex = 10;
      this.label3.Text = "Vehicle utilization";
      this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label2.Location = new System.Drawing.Point(3, 20);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(93, 20);
      this.label2.TabIndex = 9;
      this.label2.Text = "Distance";
      this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      // 
      // qualityViewHost
      // 
      this.qualityViewHost.BackColor = System.Drawing.Color.Transparent;
      this.qualityViewHost.Caption = "View";
      this.qualityViewHost.Content = null;
      this.qualityViewHost.Dock = System.Windows.Forms.DockStyle.Left;
      this.qualityViewHost.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.qualityViewHost.Location = new System.Drawing.Point(102, 3);
      this.qualityViewHost.Name = "qualityViewHost";
      this.qualityViewHost.ReadOnly = false;
      this.qualityViewHost.Size = new System.Drawing.Size(281, 14);
      this.qualityViewHost.TabIndex = 8;
      this.qualityViewHost.ViewType = null;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(93, 20);
      this.label1.TabIndex = 0;
      this.label1.Text = "Quality";
      this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      // 
      // distanceViewHost
      // 
      this.distanceViewHost.BackColor = System.Drawing.Color.Transparent;
      this.distanceViewHost.Caption = "View";
      this.distanceViewHost.Content = null;
      this.distanceViewHost.Dock = System.Windows.Forms.DockStyle.Left;
      this.distanceViewHost.Location = new System.Drawing.Point(102, 23);
      this.distanceViewHost.Name = "distanceViewHost";
      this.distanceViewHost.ReadOnly = false;
      this.distanceViewHost.Size = new System.Drawing.Size(281, 14);
      this.distanceViewHost.TabIndex = 7;
      this.distanceViewHost.ViewType = null;
      // 
      // VRPSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "VRPSolutionView";
      this.Size = new System.Drawing.Size(423, 558);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      this.valueTabPage.ResumeLayout(false);
      this.tourGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage visualizationTabPage;
    private System.Windows.Forms.TabPage valueTabPage;
    private System.Windows.Forms.GroupBox tourGroupBox;
    private HeuristicLab.MainForm.WindowsForms.ViewHost tourViewHost;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private HeuristicLab.MainForm.WindowsForms.ViewHost travelTimeViewHost;
    private System.Windows.Forms.Label label6;
    private HeuristicLab.MainForm.WindowsForms.ViewHost tardinessViewHost;
    private System.Windows.Forms.Label label5;
    private HeuristicLab.MainForm.WindowsForms.ViewHost overloadViewHost;
    private System.Windows.Forms.Label label4;
    private HeuristicLab.MainForm.WindowsForms.ViewHost vehicleUtilizationViewHost;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private HeuristicLab.MainForm.WindowsForms.ViewHost qualityViewHost;
    private HeuristicLab.MainForm.WindowsForms.ViewHost distanceViewHost;


  }
}
