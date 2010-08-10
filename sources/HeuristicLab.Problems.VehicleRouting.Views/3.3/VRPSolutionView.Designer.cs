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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.visualizationTabPage = new System.Windows.Forms.TabPage();
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.valueTabPage = new System.Windows.Forms.TabPage();
      this.tourGroupBox = new System.Windows.Forms.GroupBox();
      this.tourViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.valueTabPage.SuspendLayout();
      this.tourGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Controls.Add(this.valueTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(423, 558);
      this.tabControl.TabIndex = 1;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Controls.Add(this.pictureBox);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(415, 532);
      this.visualizationTabPage.TabIndex = 0;
      this.visualizationTabPage.Text = "Visualization";
      this.visualizationTabPage.UseVisualStyleBackColor = true;
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
      this.pictureBox.Size = new System.Drawing.Size(403, 520);
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      this.pictureBox.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
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
      // VRPSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "VRPSolutionView";
      this.Size = new System.Drawing.Size(423, 558);
      this.tabControl.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.valueTabPage.ResumeLayout(false);
      this.tourGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage visualizationTabPage;
    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.TabPage valueTabPage;
    private System.Windows.Forms.GroupBox tourGroupBox;
    private MainForm.WindowsForms.ViewHost tourViewHost;



  }
}
