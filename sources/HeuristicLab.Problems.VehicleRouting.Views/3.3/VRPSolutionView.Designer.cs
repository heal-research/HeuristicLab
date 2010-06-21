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
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.qualityViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabPage6 = new System.Windows.Forms.TabPage();
      this.distanceViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.overloadViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabPage3 = new System.Windows.Forms.TabPage();
      this.tardinessViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabPage4 = new System.Windows.Forms.TabPage();
      this.travelTimeViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabPage5 = new System.Windows.Forms.TabPage();
      this.vehicleUtilizationViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.valueTabPage.SuspendLayout();
      this.tourGroupBox.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage6.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.tabPage3.SuspendLayout();
      this.tabPage4.SuspendLayout();
      this.tabPage5.SuspendLayout();
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
      this.pictureBox.Size = new System.Drawing.Size(403, 288);
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      this.pictureBox.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Controls.Add(this.valueTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 3);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(423, 326);
      this.tabControl.TabIndex = 0;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Controls.Add(this.pictureBox);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(415, 300);
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
      this.valueTabPage.Size = new System.Drawing.Size(415, 300);
      this.valueTabPage.TabIndex = 1;
      this.valueTabPage.Text = "Value";
      this.valueTabPage.UseVisualStyleBackColor = true;
      // 
      // tourGroupBox
      // 
      this.tourGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tourGroupBox.Controls.Add(this.tourViewHost);
      this.tourGroupBox.Location = new System.Drawing.Point(6, 6);
      this.tourGroupBox.Name = "tourGroupBox";
      this.tourGroupBox.Size = new System.Drawing.Size(403, 288);
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
      this.tourViewHost.Size = new System.Drawing.Size(391, 263);
      this.tourViewHost.TabIndex = 0;
      this.tourViewHost.ViewType = null;
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.tabControl1);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.tabControl);
      this.splitContainer.Size = new System.Drawing.Size(423, 402);
      this.splitContainer.SplitterDistance = 69;
      this.splitContainer.TabIndex = 0;
      // 
      // tabControl1
      // 
      this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage6);
      this.tabControl1.Controls.Add(this.tabPage5);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Controls.Add(this.tabPage3);
      this.tabControl1.Controls.Add(this.tabPage4);
      this.tabControl1.Location = new System.Drawing.Point(4, 3);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(416, 63);
      this.tabControl1.TabIndex = 0;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.qualityViewHost);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(408, 37);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Quality";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // qualityViewHost
      // 
      this.qualityViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityViewHost.BackColor = System.Drawing.Color.Transparent;
      this.qualityViewHost.Caption = "View";
      this.qualityViewHost.Content = null;
      this.qualityViewHost.Location = new System.Drawing.Point(3, 1);
      this.qualityViewHost.Name = "qualityViewHost";
      this.qualityViewHost.ReadOnly = false;
      this.qualityViewHost.Size = new System.Drawing.Size(402, 35);
      this.qualityViewHost.TabIndex = 5;
      this.qualityViewHost.ViewType = null;
      // 
      // tabPage6
      // 
      this.tabPage6.Controls.Add(this.distanceViewHost);
      this.tabPage6.Location = new System.Drawing.Point(4, 22);
      this.tabPage6.Name = "tabPage6";
      this.tabPage6.Size = new System.Drawing.Size(408, 37);
      this.tabPage6.TabIndex = 5;
      this.tabPage6.Text = "Distance";
      this.tabPage6.UseVisualStyleBackColor = true;
      // 
      // distanceViewHost
      // 
      this.distanceViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.distanceViewHost.BackColor = System.Drawing.Color.Transparent;
      this.distanceViewHost.Caption = "View";
      this.distanceViewHost.Content = null;
      this.distanceViewHost.Location = new System.Drawing.Point(3, 1);
      this.distanceViewHost.Name = "distanceViewHost";
      this.distanceViewHost.ReadOnly = false;
      this.distanceViewHost.Size = new System.Drawing.Size(402, 35);
      this.distanceViewHost.TabIndex = 6;
      this.distanceViewHost.ViewType = null;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.overloadViewHost);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Size = new System.Drawing.Size(408, 37);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Overload";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // overloadViewHost
      // 
      this.overloadViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.overloadViewHost.BackColor = System.Drawing.Color.Transparent;
      this.overloadViewHost.Caption = "View";
      this.overloadViewHost.Content = null;
      this.overloadViewHost.Location = new System.Drawing.Point(3, 2);
      this.overloadViewHost.Name = "overloadViewHost";
      this.overloadViewHost.ReadOnly = false;
      this.overloadViewHost.Size = new System.Drawing.Size(402, 35);
      this.overloadViewHost.TabIndex = 4;
      this.overloadViewHost.ViewType = null;
      // 
      // tabPage3
      // 
      this.tabPage3.Controls.Add(this.tardinessViewHost);
      this.tabPage3.Location = new System.Drawing.Point(4, 22);
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.Size = new System.Drawing.Size(408, 37);
      this.tabPage3.TabIndex = 2;
      this.tabPage3.Text = "Tardiness";
      this.tabPage3.UseVisualStyleBackColor = true;
      // 
      // tardinessViewHost
      // 
      this.tardinessViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tardinessViewHost.Caption = "View";
      this.tardinessViewHost.Content = null;
      this.tardinessViewHost.Location = new System.Drawing.Point(2, 1);
      this.tardinessViewHost.Name = "tardinessViewHost";
      this.tardinessViewHost.ReadOnly = false;
      this.tardinessViewHost.Size = new System.Drawing.Size(404, 35);
      this.tardinessViewHost.TabIndex = 2;
      this.tardinessViewHost.ViewType = null;
      // 
      // tabPage4
      // 
      this.tabPage4.Controls.Add(this.travelTimeViewHost);
      this.tabPage4.Location = new System.Drawing.Point(4, 22);
      this.tabPage4.Name = "tabPage4";
      this.tabPage4.Size = new System.Drawing.Size(408, 37);
      this.tabPage4.TabIndex = 3;
      this.tabPage4.Text = "TravelTime";
      this.tabPage4.UseVisualStyleBackColor = true;
      // 
      // travelTimeViewHost
      // 
      this.travelTimeViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.travelTimeViewHost.Caption = "View";
      this.travelTimeViewHost.Content = null;
      this.travelTimeViewHost.Location = new System.Drawing.Point(2, 1);
      this.travelTimeViewHost.Name = "travelTimeViewHost";
      this.travelTimeViewHost.ReadOnly = false;
      this.travelTimeViewHost.Size = new System.Drawing.Size(404, 35);
      this.travelTimeViewHost.TabIndex = 2;
      this.travelTimeViewHost.ViewType = null;
      // 
      // tabPage5
      // 
      this.tabPage5.Controls.Add(this.vehicleUtilizationViewHost);
      this.tabPage5.Location = new System.Drawing.Point(4, 22);
      this.tabPage5.Name = "tabPage5";
      this.tabPage5.Size = new System.Drawing.Size(408, 37);
      this.tabPage5.TabIndex = 4;
      this.tabPage5.Text = "Vehicle utilization";
      this.tabPage5.UseVisualStyleBackColor = true;
      // 
      // vehicleUtilizationViewHost
      // 
      this.vehicleUtilizationViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.vehicleUtilizationViewHost.Caption = "View";
      this.vehicleUtilizationViewHost.Content = null;
      this.vehicleUtilizationViewHost.Location = new System.Drawing.Point(2, 1);
      this.vehicleUtilizationViewHost.Name = "vehicleUtilizationViewHost";
      this.vehicleUtilizationViewHost.ReadOnly = false;
      this.vehicleUtilizationViewHost.Size = new System.Drawing.Size(404, 35);
      this.vehicleUtilizationViewHost.TabIndex = 2;
      this.vehicleUtilizationViewHost.ViewType = null;
      // 
      // VRPSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "VRPSolutionView";
      this.Size = new System.Drawing.Size(423, 402);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      this.valueTabPage.ResumeLayout(false);
      this.tourGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage6.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.tabPage3.ResumeLayout(false);
      this.tabPage4.ResumeLayout(false);
      this.tabPage5.ResumeLayout(false);
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
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TabPage tabPage3;
    private HeuristicLab.MainForm.WindowsForms.ViewHost tardinessViewHost;
    private System.Windows.Forms.TabPage tabPage4;
    private HeuristicLab.MainForm.WindowsForms.ViewHost travelTimeViewHost;
    private System.Windows.Forms.TabPage tabPage5;
    private HeuristicLab.MainForm.WindowsForms.ViewHost vehicleUtilizationViewHost;
    private HeuristicLab.MainForm.WindowsForms.ViewHost overloadViewHost;
    private HeuristicLab.MainForm.WindowsForms.ViewHost qualityViewHost;
    private System.Windows.Forms.TabPage tabPage6;
    private HeuristicLab.MainForm.WindowsForms.ViewHost distanceViewHost;


  }
}
