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
      this.ScoreLabel = new System.Windows.Forms.Label();
      this.qualityLabel = new System.Windows.Forms.Label();
      this.qualityValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.scoreValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.valueTabPage.SuspendLayout();
      this.tourGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.qualityGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // pictureBox
      // 
      this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox.Location = new System.Drawing.Point(3, 3);
      this.pictureBox.Size = new System.Drawing.Size(403, 228);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Location = new System.Drawing.Point(3, 72);
      this.tabControl.Size = new System.Drawing.Size(417, 260);
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Size = new System.Drawing.Size(409, 234);
      // 
      // valueTabPage
      // 
      this.valueTabPage.Size = new System.Drawing.Size(409, 234);
      // 
      // tourGroupBox
      // 
      this.tourGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.tourGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tourGroupBox.Location = new System.Drawing.Point(3, 3);
      this.tourGroupBox.Size = new System.Drawing.Size(403, 228);
      // 
      // tourViewHost
      // 
      this.tourViewHost.Size = new System.Drawing.Size(391, 203);
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.ScoreLabel);
      this.splitContainer.Panel1.Controls.Add(this.qualityLabel);
      this.splitContainer.Panel1.Controls.Add(this.qualityValueView);
      this.splitContainer.Panel1.Controls.Add(this.scoreValueView);
      this.splitContainer.Panel1.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
      this.splitContainer.Panel1MinSize = 0;
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.qualityGroupBox);
      this.splitContainer.SplitterDistance = 63;
      // 
      // qualityGroupBox
      // 
      this.qualityGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityGroupBox.Location = new System.Drawing.Point(3, 3);
      this.qualityGroupBox.Size = new System.Drawing.Size(417, 63);
      // 
      // qualityViewHost
      // 
      this.qualityViewHost.Size = new System.Drawing.Size(405, 38);
      // 
      // ScoreLabel
      // 
      this.ScoreLabel.AutoSize = true;
      this.ScoreLabel.Location = new System.Drawing.Point(7, 38);
      this.ScoreLabel.Name = "ScoreLabel";
      this.ScoreLabel.Size = new System.Drawing.Size(38, 13);
      this.ScoreLabel.TabIndex = 1;
      this.ScoreLabel.Text = "Score:";
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
      this.qualityValueView.Location = new System.Drawing.Point(65, 7);
      this.qualityValueView.Name = "qualityValueView";
      this.qualityValueView.ReadOnly = false;
      this.qualityValueView.Size = new System.Drawing.Size(354, 21);
      this.qualityValueView.TabIndex = 0;
      // 
      // scoreValueView
      // 
      this.scoreValueView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scoreValueView.Caption = "StringConvertibleValue View";
      this.scoreValueView.Content = null;
      this.scoreValueView.LabelVisible = false;
      this.scoreValueView.Location = new System.Drawing.Point(65, 34);
      this.scoreValueView.Name = "scoreValueView";
      this.scoreValueView.ReadOnly = false;
      this.scoreValueView.Size = new System.Drawing.Size(354, 21);
      this.scoreValueView.TabIndex = 0;
      // 
      // OrienteeringSolutionView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Name = "OrienteeringSolutionView";
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
      this.qualityGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Label ScoreLabel;
    private System.Windows.Forms.Label qualityLabel;
    private Data.Views.StringConvertibleValueView qualityValueView;
    private Data.Views.StringConvertibleValueView scoreValueView;
  }
}
