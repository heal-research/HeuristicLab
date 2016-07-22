#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class DataAnalysisSolutionView {
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
      this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
      this.loadProblemDataButton = new System.Windows.Forms.Button();
      this.loadProblemDataFileDialog = new System.Windows.Forms.OpenFileDialog();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      //
      // itemslistView
      //
      this.itemsListView.Location = new System.Drawing.Point(this.itemsListView.Location.X, this.itemsListView.Location.Y + 3);
      //
      // detailsGroupBox
      //
      this.detailsGroupBox.Location = new System.Drawing.Point(this.detailsGroupBox.Location.X, this.detailsGroupBox.Location.Y + 3);
      //
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.flowLayoutPanel);
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Text = "Data Analysis Solution";
      //
      // flowLayoutPanel
      //
      this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
      this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.flowLayoutPanel.Size = new System.Drawing.Size(266, 30);
      this.flowLayoutPanel.Controls.Add(this.loadProblemDataButton);
      //
      // loadProblemDataButton
      //
      this.loadProblemDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
      this.loadProblemDataButton.AutoSize = true;
      this.loadProblemDataButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.loadProblemDataButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.loadProblemDataButton.Name = "loadProblemDataButton";
      this.loadProblemDataButton.Size = new System.Drawing.Size(105, 24);
      this.loadProblemDataButton.TabIndex = 6;
      this.loadProblemDataButton.Text = "Load new Data";
      this.loadProblemDataButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.loadProblemDataButton.UseVisualStyleBackColor = true;
      this.loadProblemDataButton.Click += new System.EventHandler(this.loadProblemDataButton_Click);
      this.toolTip.SetToolTip(this.loadProblemDataButton, "Creates a new data analysis solution with the same model and the loaded problem data.");
      // 
      // openFileDialog
      // 
      this.loadProblemDataFileDialog.Filter = "HL files|*.hl";
      this.loadProblemDataFileDialog.Title = "Load new ProblemData or Problem...";
      // 
      // DataAnalysisSolutionView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Name = "DataAnalysisSolutionView";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Button loadProblemDataButton;
    protected System.Windows.Forms.OpenFileDialog loadProblemDataFileDialog;
    protected System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;

  }
}
