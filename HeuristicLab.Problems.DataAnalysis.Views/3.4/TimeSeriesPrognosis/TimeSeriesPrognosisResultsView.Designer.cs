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

namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class TimeSeriesPrognosisResultsView {
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
      this.components = new System.ComponentModel.Container();
      this.TrainingHorizonLabel = new System.Windows.Forms.Label();
      this.TestHorizonLabel = new System.Windows.Forms.Label();
      this.TrainingHorizonTextBox = new System.Windows.Forms.TextBox();
      this.TestHorizonTextBox = new System.Windows.Forms.TextBox();
      this.TrainingHorizonErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.TestHorizonErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.TrainingHorizonErrorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.TestHorizonErrorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.TestHorizonTextBox);
      this.splitContainer.Panel1.Controls.Add(this.TrainingHorizonTextBox);
      this.splitContainer.Panel1.Controls.Add(this.TrainingHorizonLabel);
      this.splitContainer.Panel1.Controls.Add(this.TestHorizonLabel);
      // 
      // itemsListView
      // 
      this.itemsListView.Location = new System.Drawing.Point(3, 82);
      this.itemsListView.Size = new System.Drawing.Size(244, 279);
      // 
      // addButton
      // 
      this.toolTip.SetToolTip(this.addButton, "Add");
      // 
      // removeButton
      // 
      this.toolTip.SetToolTip(this.removeButton, "Remove");
      // 
      // TrainingHorizonLabel
      // 
      this.TrainingHorizonLabel.AutoSize = true;
      this.TrainingHorizonLabel.Location = new System.Drawing.Point(3, 34);
      this.TrainingHorizonLabel.Name = "TrainingHorizonLabel";
      this.TrainingHorizonLabel.Size = new System.Drawing.Size(87, 13);
      this.TrainingHorizonLabel.TabIndex = 7;
      this.TrainingHorizonLabel.Text = "Training Horizon:";
      // 
      // TestHorizonLabel
      // 
      this.TestHorizonLabel.AutoSize = true;
      this.TestHorizonLabel.Location = new System.Drawing.Point(3, 60);
      this.TestHorizonLabel.Name = "TestHorizonLabel";
      this.TestHorizonLabel.Size = new System.Drawing.Size(73, 13);
      this.TestHorizonLabel.TabIndex = 8;
      this.TestHorizonLabel.Text = "Test Horizon: ";
      // 
      // TrainingHorizonTextBox
      // 
      this.TrainingHorizonTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TrainingHorizonTextBox.Location = new System.Drawing.Point(96, 31);
      this.TrainingHorizonTextBox.Name = "TrainingHorizonTextBox";
      this.TrainingHorizonTextBox.Size = new System.Drawing.Size(151, 20);
      this.TrainingHorizonTextBox.TabIndex = 1;
      this.TrainingHorizonTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TrainingHorizonTextBox_KeyDown);
      this.TrainingHorizonTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TrainingHorizonTextBox_Validating);
      this.TrainingHorizonTextBox.Validated += new System.EventHandler(this.TrainingHorizonTextBox_Validated);
      // 
      // TestHorizonTextBox
      // 
      this.TestHorizonTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TestHorizonTextBox.Location = new System.Drawing.Point(96, 57);
      this.TestHorizonTextBox.Name = "TestHorizonTextBox";
      this.TestHorizonTextBox.Size = new System.Drawing.Size(151, 20);
      this.TestHorizonTextBox.TabIndex = 1;
      this.TestHorizonTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TestHorizonTextBox_KeyDown);
      this.TestHorizonTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TestHorizonTextBox_Validating);
      this.TestHorizonTextBox.Validated += new System.EventHandler(this.TestHorizonTextBox_Validated);
      // 
      // TrainingHorizonErrorProvider
      // 
      this.TrainingHorizonErrorProvider.BlinkRate = 0;
      this.TrainingHorizonErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.TrainingHorizonErrorProvider.ContainerControl = this;
      // 
      // TestHorizonErrorProvider
      // 
      this.TestHorizonErrorProvider.BlinkRate = 0;
      this.TestHorizonErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.TestHorizonErrorProvider.ContainerControl = this;
      // 
      // TimeSeriesPrognosisResultsView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "TimeSeriesPrognosisResultsView";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.TrainingHorizonErrorProvider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.TestHorizonErrorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox TestHorizonTextBox;
    private System.Windows.Forms.TextBox TrainingHorizonTextBox;
    private System.Windows.Forms.Label TrainingHorizonLabel;
    private System.Windows.Forms.Label TestHorizonLabel;
    private System.Windows.Forms.ErrorProvider TrainingHorizonErrorProvider;
    private System.Windows.Forms.ErrorProvider TestHorizonErrorProvider;

  }
}
