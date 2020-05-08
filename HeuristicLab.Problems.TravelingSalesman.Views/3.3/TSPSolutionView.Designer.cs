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

namespace HeuristicLab.Problems.TravelingSalesman.Views {
  partial class TSPSolutionView {
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
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.visualizationTabPage = new System.Windows.Forms.TabPage();
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.routeTabPage = new System.Windows.Forms.TabPage();
      this.tourViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.distanceView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.distanceLabel = new System.Windows.Forms.Label();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.routeTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.AllowDrop = true;
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Controls.Add(this.routeTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 30);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(423, 372);
      this.tabControl.TabIndex = 1;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.visualizationTabPage.Controls.Add(this.pictureBox);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(415, 346);
      this.visualizationTabPage.TabIndex = 0;
      this.visualizationTabPage.Text = "Visualization";
      // 
      // pictureBox
      // 
      this.pictureBox.BackColor = System.Drawing.Color.White;
      this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox.Location = new System.Drawing.Point(3, 3);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(409, 340);
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      // 
      // routeTabPage
      // 
      this.routeTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.routeTabPage.Controls.Add(this.tourViewHost);
      this.routeTabPage.Location = new System.Drawing.Point(4, 22);
      this.routeTabPage.Name = "routeTabPage";
      this.routeTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.routeTabPage.Size = new System.Drawing.Size(415, 346);
      this.routeTabPage.TabIndex = 1;
      this.routeTabPage.Text = "Route";
      // 
      // tourViewHost
      // 
      this.tourViewHost.Caption = "View";
      this.tourViewHost.Content = null;
      this.tourViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tourViewHost.Enabled = false;
      this.tourViewHost.Location = new System.Drawing.Point(3, 3);
      this.tourViewHost.Name = "tourViewHost";
      this.tourViewHost.ReadOnly = false;
      this.tourViewHost.Size = new System.Drawing.Size(409, 340);
      this.tourViewHost.TabIndex = 1;
      this.tourViewHost.ViewsLabelVisible = true;
      this.tourViewHost.ViewType = null;
      // 
      // distanceView
      // 
      this.distanceView.Caption = "StringConvertibleValue View";
      this.distanceView.Content = null;
      this.distanceView.LabelVisible = false;
      this.distanceView.Location = new System.Drawing.Point(62, 3);
      this.distanceView.Name = "distanceView";
      this.distanceView.ReadOnly = false;
      this.distanceView.Size = new System.Drawing.Size(357, 21);
      this.distanceView.TabIndex = 3;
      // 
      // distanceLabel
      // 
      this.distanceLabel.AutoSize = true;
      this.distanceLabel.Location = new System.Drawing.Point(4, 6);
      this.distanceLabel.Name = "distanceLabel";
      this.distanceLabel.Size = new System.Drawing.Size(52, 13);
      this.distanceLabel.TabIndex = 4;
      this.distanceLabel.Text = "Distance:";
      // 
      // TSPSolutionView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.distanceLabel);
      this.Controls.Add(this.distanceView);
      this.Controls.Add(this.tabControl);
      this.Name = "TSPSolutionView";
      this.Size = new System.Drawing.Size(423, 402);
      this.tabControl.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.routeTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected MainForm.WindowsForms.DragOverTabControl tabControl;
    protected System.Windows.Forms.TabPage visualizationTabPage;
    protected System.Windows.Forms.PictureBox pictureBox;
    protected System.Windows.Forms.TabPage routeTabPage;
    protected MainForm.WindowsForms.ViewHost tourViewHost;
    protected Data.Views.StringConvertibleValueView distanceView;
    protected System.Windows.Forms.Label distanceLabel;
  }
}
