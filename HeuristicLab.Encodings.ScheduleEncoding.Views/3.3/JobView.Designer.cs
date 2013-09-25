#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class JobView {
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
      this.indexLabel = new System.Windows.Forms.Label();
      this.indexTextBox = new System.Windows.Forms.TextBox();
      this.dueDateLabel = new System.Windows.Forms.Label();
      this.dueDateTextBox = new System.Windows.Forms.TextBox();
      this.tasksGroupBox = new System.Windows.Forms.GroupBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // indexLabel
      // 
      this.indexLabel.AutoSize = true;
      this.indexLabel.Location = new System.Drawing.Point(3, 6);
      this.indexLabel.Name = "indexLabel";
      this.indexLabel.Size = new System.Drawing.Size(41, 13);
      this.indexLabel.TabIndex = 0;
      this.indexLabel.Text = "Job Nr:";
      // 
      // indexTextBox
      // 
      this.indexTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.indexTextBox.Location = new System.Drawing.Point(65, 3);
      this.indexTextBox.Name = "indexTextBox";
      this.indexTextBox.Size = new System.Drawing.Size(451, 20);
      this.indexTextBox.TabIndex = 1;
      this.indexTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.indexTextBox_Validating);
      // 
      // dueDateLabel
      // 
      this.dueDateLabel.AutoSize = true;
      this.dueDateLabel.Location = new System.Drawing.Point(3, 32);
      this.dueDateLabel.Name = "dueDateLabel";
      this.dueDateLabel.Size = new System.Drawing.Size(56, 13);
      this.dueDateLabel.TabIndex = 0;
      this.dueDateLabel.Text = "Due Date:";
      // 
      // dueDateTextBox
      // 
      this.dueDateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dueDateTextBox.Location = new System.Drawing.Point(65, 29);
      this.dueDateTextBox.Name = "dueDateTextBox";
      this.dueDateTextBox.Size = new System.Drawing.Size(451, 20);
      this.dueDateTextBox.TabIndex = 1;
      this.dueDateTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.dueDateTextBox_Validating);
      // 
      // tasksGroupBox
      // 
      this.tasksGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tasksGroupBox.Location = new System.Drawing.Point(6, 55);
      this.tasksGroupBox.Name = "tasksGroupBox";
      this.tasksGroupBox.Size = new System.Drawing.Size(510, 214);
      this.tasksGroupBox.TabIndex = 2;
      this.tasksGroupBox.TabStop = false;
      this.tasksGroupBox.Text = "Tasks";
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // JobView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tasksGroupBox);
      this.Controls.Add(this.dueDateTextBox);
      this.Controls.Add(this.dueDateLabel);
      this.Controls.Add(this.indexTextBox);
      this.Controls.Add(this.indexLabel);
      this.Name = "JobView";
      this.Size = new System.Drawing.Size(519, 272);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label indexLabel;
    private System.Windows.Forms.TextBox indexTextBox;
    private System.Windows.Forms.Label dueDateLabel;
    private System.Windows.Forms.TextBox dueDateTextBox;
    private System.Windows.Forms.GroupBox tasksGroupBox;
    private System.Windows.Forms.ErrorProvider errorProvider;
  }
}
