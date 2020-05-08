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

namespace HeuristicLab.Problems.Orienteering.Views {
  partial class OrienteeringProblemDataView {
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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.routingDataTabPage = new System.Windows.Forms.TabPage();
      this.routingDataViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.OrienteeringDataTabPage = new System.Windows.Forms.TabPage();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.scoresArrayView = new HeuristicLab.Data.Views.StringConvertibleArrayView();
      this.scoresLabel = new System.Windows.Forms.Label();
      this.pointVisitingCostsTextBox = new System.Windows.Forms.TextBox();
      this.pointVisitingCostsLabel = new System.Windows.Forms.Label();
      this.maximumTravelCostsTextBox = new System.Windows.Forms.TextBox();
      this.maximumTravelCostsLabel = new System.Windows.Forms.Label();
      this.terminalPointTextBox = new System.Windows.Forms.TextBox();
      this.terminalPointLabel = new System.Windows.Forms.Label();
      this.startingPointTextBox = new System.Windows.Forms.TextBox();
      this.startingPointLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.routingDataTabPage.SuspendLayout();
      this.OrienteeringDataTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(454, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(518, 3);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.routingDataTabPage);
      this.tabControl.Controls.Add(this.OrienteeringDataTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 26);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(541, 401);
      this.tabControl.TabIndex = 0;
      // 
      // routingDataTabPage
      // 
      this.routingDataTabPage.Controls.Add(this.routingDataViewHost);
      this.routingDataTabPage.Location = new System.Drawing.Point(4, 22);
      this.routingDataTabPage.Name = "routingDataTabPage";
      this.routingDataTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.routingDataTabPage.Size = new System.Drawing.Size(533, 375);
      this.routingDataTabPage.TabIndex = 0;
      this.routingDataTabPage.Text = "Routing Data";
      this.routingDataTabPage.UseVisualStyleBackColor = true;
      // 
      // routingDataViewHost
      // 
      this.routingDataViewHost.Caption = "View";
      this.routingDataViewHost.Content = null;
      this.routingDataViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.routingDataViewHost.Enabled = false;
      this.routingDataViewHost.Location = new System.Drawing.Point(3, 3);
      this.routingDataViewHost.Name = "routingDataViewHost";
      this.routingDataViewHost.ReadOnly = false;
      this.routingDataViewHost.Size = new System.Drawing.Size(527, 369);
      this.routingDataViewHost.TabIndex = 0;
      this.routingDataViewHost.ViewsLabelVisible = true;
      this.routingDataViewHost.ViewType = null;
      // 
      // OrienteeringDataTabPage
      // 
      this.OrienteeringDataTabPage.Controls.Add(this.splitContainer1);
      this.OrienteeringDataTabPage.Location = new System.Drawing.Point(4, 22);
      this.OrienteeringDataTabPage.Name = "OrienteeringDataTabPage";
      this.OrienteeringDataTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.OrienteeringDataTabPage.Size = new System.Drawing.Size(533, 375);
      this.OrienteeringDataTabPage.TabIndex = 1;
      this.OrienteeringDataTabPage.Text = "Orienteering";
      this.OrienteeringDataTabPage.UseVisualStyleBackColor = true;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(3, 3);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.scoresArrayView);
      this.splitContainer1.Panel1.Controls.Add(this.scoresLabel);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.pointVisitingCostsTextBox);
      this.splitContainer1.Panel2.Controls.Add(this.pointVisitingCostsLabel);
      this.splitContainer1.Panel2.Controls.Add(this.maximumTravelCostsTextBox);
      this.splitContainer1.Panel2.Controls.Add(this.maximumTravelCostsLabel);
      this.splitContainer1.Panel2.Controls.Add(this.terminalPointTextBox);
      this.splitContainer1.Panel2.Controls.Add(this.terminalPointLabel);
      this.splitContainer1.Panel2.Controls.Add(this.startingPointTextBox);
      this.splitContainer1.Panel2.Controls.Add(this.startingPointLabel);
      this.splitContainer1.Size = new System.Drawing.Size(527, 369);
      this.splitContainer1.SplitterDistance = 175;
      this.splitContainer1.TabIndex = 0;
      // 
      // scoresArrayView
      // 
      this.scoresArrayView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scoresArrayView.Caption = "StringConvertibleArray View";
      this.scoresArrayView.Content = null;
      this.scoresArrayView.Location = new System.Drawing.Point(3, 25);
      this.scoresArrayView.Name = "scoresArrayView";
      this.scoresArrayView.ReadOnly = false;
      this.scoresArrayView.Size = new System.Drawing.Size(169, 341);
      this.scoresArrayView.TabIndex = 1;
      // 
      // scoresLabel
      // 
      this.scoresLabel.AutoSize = true;
      this.scoresLabel.Location = new System.Drawing.Point(5, 9);
      this.scoresLabel.Name = "scoresLabel";
      this.scoresLabel.Size = new System.Drawing.Size(43, 13);
      this.scoresLabel.TabIndex = 0;
      this.scoresLabel.Text = "Scores:";
      // 
      // pointVisitingCostsTextBox
      // 
      this.pointVisitingCostsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pointVisitingCostsTextBox.Location = new System.Drawing.Point(126, 58);
      this.pointVisitingCostsTextBox.Name = "pointVisitingCostsTextBox";
      this.pointVisitingCostsTextBox.ReadOnly = true;
      this.pointVisitingCostsTextBox.Size = new System.Drawing.Size(219, 20);
      this.pointVisitingCostsTextBox.TabIndex = 1;
      // 
      // pointVisitingCostsLabel
      // 
      this.pointVisitingCostsLabel.AutoSize = true;
      this.pointVisitingCostsLabel.Location = new System.Drawing.Point(4, 61);
      this.pointVisitingCostsLabel.Name = "pointVisitingCostsLabel";
      this.pointVisitingCostsLabel.Size = new System.Drawing.Size(99, 13);
      this.pointVisitingCostsLabel.TabIndex = 0;
      this.pointVisitingCostsLabel.Text = "Point Visiting Costs:";
      // 
      // maximumTravelCostsTextBox
      // 
      this.maximumTravelCostsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.maximumTravelCostsTextBox.Location = new System.Drawing.Point(126, 84);
      this.maximumTravelCostsTextBox.Name = "maximumTravelCostsTextBox";
      this.maximumTravelCostsTextBox.ReadOnly = true;
      this.maximumTravelCostsTextBox.Size = new System.Drawing.Size(219, 20);
      this.maximumTravelCostsTextBox.TabIndex = 1;
      // 
      // maximumTravelCostsLabel
      // 
      this.maximumTravelCostsLabel.AutoSize = true;
      this.maximumTravelCostsLabel.Location = new System.Drawing.Point(4, 87);
      this.maximumTravelCostsLabel.Name = "maximumTravelCostsLabel";
      this.maximumTravelCostsLabel.Size = new System.Drawing.Size(116, 13);
      this.maximumTravelCostsLabel.TabIndex = 0;
      this.maximumTravelCostsLabel.Text = "Maximum Travel Costs:";
      // 
      // terminalPointTextBox
      // 
      this.terminalPointTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.terminalPointTextBox.Location = new System.Drawing.Point(126, 32);
      this.terminalPointTextBox.Name = "terminalPointTextBox";
      this.terminalPointTextBox.ReadOnly = true;
      this.terminalPointTextBox.Size = new System.Drawing.Size(219, 20);
      this.terminalPointTextBox.TabIndex = 1;
      // 
      // terminalPointLabel
      // 
      this.terminalPointLabel.AutoSize = true;
      this.terminalPointLabel.Location = new System.Drawing.Point(4, 35);
      this.terminalPointLabel.Name = "terminalPointLabel";
      this.terminalPointLabel.Size = new System.Drawing.Size(77, 13);
      this.terminalPointLabel.TabIndex = 0;
      this.terminalPointLabel.Text = "Terminal Point:";
      // 
      // startingPointTextBox
      // 
      this.startingPointTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.startingPointTextBox.Location = new System.Drawing.Point(126, 6);
      this.startingPointTextBox.Name = "startingPointTextBox";
      this.startingPointTextBox.ReadOnly = true;
      this.startingPointTextBox.Size = new System.Drawing.Size(219, 20);
      this.startingPointTextBox.TabIndex = 1;
      // 
      // startingPointLabel
      // 
      this.startingPointLabel.AutoSize = true;
      this.startingPointLabel.Location = new System.Drawing.Point(4, 9);
      this.startingPointLabel.Name = "startingPointLabel";
      this.startingPointLabel.Size = new System.Drawing.Size(73, 13);
      this.startingPointLabel.TabIndex = 0;
      this.startingPointLabel.Text = "Starting Point:";
      // 
      // OrienteeringProblemDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "OrienteeringProblemDataView";
      this.Size = new System.Drawing.Size(541, 427);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.routingDataTabPage.ResumeLayout(false);
      this.OrienteeringDataTabPage.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel1.PerformLayout();
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage routingDataTabPage;
    private MainForm.WindowsForms.ViewHost routingDataViewHost;
    private System.Windows.Forms.TabPage OrienteeringDataTabPage;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private Data.Views.StringConvertibleArrayView scoresArrayView;
    private System.Windows.Forms.Label scoresLabel;
    private System.Windows.Forms.TextBox startingPointTextBox;
    private System.Windows.Forms.Label startingPointLabel;
    private System.Windows.Forms.TextBox pointVisitingCostsTextBox;
    private System.Windows.Forms.Label pointVisitingCostsLabel;
    private System.Windows.Forms.TextBox maximumTravelCostsTextBox;
    private System.Windows.Forms.Label maximumTravelCostsLabel;
    private System.Windows.Forms.TextBox terminalPointTextBox;
    private System.Windows.Forms.Label terminalPointLabel;
  }
}
