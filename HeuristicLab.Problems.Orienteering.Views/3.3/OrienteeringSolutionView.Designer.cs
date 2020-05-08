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
      this.scoreLabel = new System.Windows.Forms.Label();
      this.qualityLabel = new System.Windows.Forms.Label();
      this.qualityValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.scoreValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.routeTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Location = new System.Drawing.Point(3, 88);
      this.tabControl.Size = new System.Drawing.Size(417, 314);
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Size = new System.Drawing.Size(409, 288);
      // 
      // pictureBox
      // 
      this.pictureBox.Size = new System.Drawing.Size(403, 282);
      // 
      // routeTabPage
      // 
      this.routeTabPage.Size = new System.Drawing.Size(409, 234);
      // 
      // tourViewHost
      // 
      this.tourViewHost.Size = new System.Drawing.Size(403, 228);
      // 
      // distanceView
      // 
      this.distanceView.Location = new System.Drawing.Point(62, 61);
      // 
      // distanceLabel
      // 
      this.distanceLabel.Location = new System.Drawing.Point(4, 64);
      // 
      // scoreLabel
      // 
      this.scoreLabel.AutoSize = true;
      this.scoreLabel.Location = new System.Drawing.Point(7, 38);
      this.scoreLabel.Name = "scoreLabel";
      this.scoreLabel.Size = new System.Drawing.Size(38, 13);
      this.scoreLabel.TabIndex = 1;
      this.scoreLabel.Text = "Score:";
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
      // qualityValueView
      // 
      this.qualityValueView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityValueView.Caption = "StringConvertibleValue View";
      this.qualityValueView.Content = null;
      this.qualityValueView.LabelVisible = false;
      this.qualityValueView.Location = new System.Drawing.Point(62, 7);
      this.qualityValueView.Name = "qualityValueView";
      this.qualityValueView.ReadOnly = false;
      this.qualityValueView.Size = new System.Drawing.Size(357, 21);
      this.qualityValueView.TabIndex = 0;
      // 
      // scoreValueView
      // 
      this.scoreValueView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scoreValueView.Caption = "StringConvertibleValue View";
      this.scoreValueView.Content = null;
      this.scoreValueView.LabelVisible = false;
      this.scoreValueView.Location = new System.Drawing.Point(62, 34);
      this.scoreValueView.Name = "scoreValueView";
      this.scoreValueView.ReadOnly = false;
      this.scoreValueView.Size = new System.Drawing.Size(357, 21);
      this.scoreValueView.TabIndex = 0;
      // 
      // OrienteeringSolutionView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.scoreLabel);
      this.Controls.Add(this.qualityLabel);
      this.Controls.Add(this.qualityValueView);
      this.Controls.Add(this.scoreValueView);
      this.Name = "OrienteeringSolutionView";
      this.Controls.SetChildIndex(this.scoreValueView, 0);
      this.Controls.SetChildIndex(this.qualityValueView, 0);
      this.Controls.SetChildIndex(this.qualityLabel, 0);
      this.Controls.SetChildIndex(this.scoreLabel, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.distanceView, 0);
      this.Controls.SetChildIndex(this.distanceLabel, 0);
      this.tabControl.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.routeTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label scoreLabel;
    private System.Windows.Forms.Label qualityLabel;
    private Data.Views.StringConvertibleValueView qualityValueView;
    private Data.Views.StringConvertibleValueView scoreValueView;
  }
}
