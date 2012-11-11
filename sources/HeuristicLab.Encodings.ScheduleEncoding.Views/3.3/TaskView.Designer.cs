#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.ScheduleEncoding.Views {
  partial class TaskView {
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
      this.components = new System.ComponentModel.Container();
      this.taskNrLabel = new System.Windows.Forms.Label();
      this.taskNrTextBox = new System.Windows.Forms.TextBox();
      this.resourceNrLabel = new System.Windows.Forms.Label();
      this.resourceNrTextBox = new System.Windows.Forms.TextBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.jobNrLabel = new System.Windows.Forms.Label();
      this.jobNrTextBox = new System.Windows.Forms.TextBox();
      this.durationLabel = new System.Windows.Forms.Label();
      this.durationTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // taskNrLabel
      // 
      this.taskNrLabel.AutoSize = true;
      this.taskNrLabel.Location = new System.Drawing.Point(3, 6);
      this.taskNrLabel.Name = "taskNrLabel";
      this.taskNrLabel.Size = new System.Drawing.Size(48, 13);
      this.taskNrLabel.TabIndex = 0;
      this.taskNrLabel.Text = "Task Nr:";
      // 
      // taskNrTextBox
      // 
      this.taskNrTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.taskNrTextBox.Location = new System.Drawing.Point(79, 3);
      this.taskNrTextBox.Name = "taskNrTextBox";
      this.taskNrTextBox.Size = new System.Drawing.Size(437, 20);
      this.taskNrTextBox.TabIndex = 1;
      this.taskNrTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.taskNrTextBox_Validating);
      // 
      // resourceNrLabel
      // 
      this.resourceNrLabel.AutoSize = true;
      this.resourceNrLabel.Location = new System.Drawing.Point(3, 32);
      this.resourceNrLabel.Name = "resourceNrLabel";
      this.resourceNrLabel.Size = new System.Drawing.Size(70, 13);
      this.resourceNrLabel.TabIndex = 0;
      this.resourceNrLabel.Text = "Resource Nr:";
      // 
      // resourceNrTextBox
      // 
      this.resourceNrTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.resourceNrTextBox.Location = new System.Drawing.Point(79, 29);
      this.resourceNrTextBox.Name = "resourceNrTextBox";
      this.resourceNrTextBox.Size = new System.Drawing.Size(437, 20);
      this.resourceNrTextBox.TabIndex = 1;
      this.resourceNrTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.resourceNrTextBox_Validating);
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // jobNrLabel
      // 
      this.jobNrLabel.AutoSize = true;
      this.jobNrLabel.Location = new System.Drawing.Point(3, 58);
      this.jobNrLabel.Name = "jobNrLabel";
      this.jobNrLabel.Size = new System.Drawing.Size(41, 13);
      this.jobNrLabel.TabIndex = 0;
      this.jobNrLabel.Text = "Job Nr:";
      // 
      // jobNrTextBox
      // 
      this.jobNrTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.jobNrTextBox.Location = new System.Drawing.Point(79, 55);
      this.jobNrTextBox.Name = "jobNrTextBox";
      this.jobNrTextBox.Size = new System.Drawing.Size(437, 20);
      this.jobNrTextBox.TabIndex = 1;
      this.jobNrTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.jobNrTextBox_Validating);
      // 
      // durationLabel
      // 
      this.durationLabel.AutoSize = true;
      this.durationLabel.Location = new System.Drawing.Point(3, 84);
      this.durationLabel.Name = "durationLabel";
      this.durationLabel.Size = new System.Drawing.Size(50, 13);
      this.durationLabel.TabIndex = 0;
      this.durationLabel.Text = "Duration:";
      // 
      // durationTextBox
      // 
      this.durationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.durationTextBox.Location = new System.Drawing.Point(79, 81);
      this.durationTextBox.Name = "durationTextBox";
      this.durationTextBox.Size = new System.Drawing.Size(437, 20);
      this.durationTextBox.TabIndex = 1;
      this.durationTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.durationTextBox_Validating);
      // 
      // TaskView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.durationTextBox);
      this.Controls.Add(this.durationLabel);
      this.Controls.Add(this.jobNrTextBox);
      this.Controls.Add(this.jobNrLabel);
      this.Controls.Add(this.resourceNrTextBox);
      this.Controls.Add(this.resourceNrLabel);
      this.Controls.Add(this.taskNrTextBox);
      this.Controls.Add(this.taskNrLabel);
      this.Name = "TaskView";
      this.Size = new System.Drawing.Size(519, 108);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label taskNrLabel;
    private System.Windows.Forms.TextBox taskNrTextBox;
    private System.Windows.Forms.Label resourceNrLabel;
    private System.Windows.Forms.TextBox resourceNrTextBox;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.TextBox jobNrTextBox;
    private System.Windows.Forms.Label jobNrLabel;
    private System.Windows.Forms.TextBox durationTextBox;
    private System.Windows.Forms.Label durationLabel;
  }
}
