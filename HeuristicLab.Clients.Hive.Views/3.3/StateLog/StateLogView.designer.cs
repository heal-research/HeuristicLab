#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive.Views {
  partial class StateLogView {
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
      this.stateLabel = new System.Windows.Forms.Label();
      this.stateTextBox = new System.Windows.Forms.TextBox();
      this.dateLabel = new System.Windows.Forms.Label();
      this.dateTextBox = new System.Windows.Forms.TextBox();
      this.exceptionTextBox = new System.Windows.Forms.TextBox();
      this.userIdTextBox = new System.Windows.Forms.TextBox();
      this.slaveIdTextBox = new System.Windows.Forms.TextBox();
      this.slaveIdLabel = new System.Windows.Forms.Label();
      this.userIdLabel = new System.Windows.Forms.Label();
      this.exceptionLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // stateLabel
      // 
      this.stateLabel.AutoSize = true;
      this.stateLabel.Location = new System.Drawing.Point(3, 11);
      this.stateLabel.Name = "stateLabel";
      this.stateLabel.Size = new System.Drawing.Size(35, 13);
      this.stateLabel.TabIndex = 0;
      this.stateLabel.Text = "State:";
      // 
      // stateTextBox
      // 
      this.stateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.stateTextBox.Location = new System.Drawing.Point(72, 4);
      this.stateTextBox.Name = "stateTextBox";
      this.stateTextBox.Size = new System.Drawing.Size(451, 20);
      this.stateTextBox.TabIndex = 1;
      // 
      // dateLabel
      // 
      this.dateLabel.AutoSize = true;
      this.dateLabel.Location = new System.Drawing.Point(3, 33);
      this.dateLabel.Name = "dateLabel";
      this.dateLabel.Size = new System.Drawing.Size(33, 13);
      this.dateLabel.TabIndex = 2;
      this.dateLabel.Text = "Date:";
      // 
      // dateTextBox
      // 
      this.dateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dateTextBox.Location = new System.Drawing.Point(72, 30);
      this.dateTextBox.Name = "dateTextBox";
      this.dateTextBox.Size = new System.Drawing.Size(451, 20);
      this.dateTextBox.TabIndex = 3;
      // 
      // exceptionTextBox
      // 
      this.exceptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.exceptionTextBox.Location = new System.Drawing.Point(72, 56);
      this.exceptionTextBox.Name = "exceptionTextBox";
      this.exceptionTextBox.Size = new System.Drawing.Size(451, 20);
      this.exceptionTextBox.TabIndex = 4;
      // 
      // userIdTextBox
      // 
      this.userIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.userIdTextBox.Location = new System.Drawing.Point(72, 82);
      this.userIdTextBox.Name = "userIdTextBox";
      this.userIdTextBox.Size = new System.Drawing.Size(451, 20);
      this.userIdTextBox.TabIndex = 5;
      // 
      // slaveIdTextBox
      // 
      this.slaveIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.slaveIdTextBox.Location = new System.Drawing.Point(72, 108);
      this.slaveIdTextBox.Name = "slaveIdTextBox";
      this.slaveIdTextBox.Size = new System.Drawing.Size(451, 20);
      this.slaveIdTextBox.TabIndex = 6;
      // 
      // slaveIdLabel
      // 
      this.slaveIdLabel.AutoSize = true;
      this.slaveIdLabel.Location = new System.Drawing.Point(3, 111);
      this.slaveIdLabel.Name = "slaveIdLabel";
      this.slaveIdLabel.Size = new System.Drawing.Size(46, 13);
      this.slaveIdLabel.TabIndex = 7;
      this.slaveIdLabel.Text = "SlaveId:";
      // 
      // userIdLabel
      // 
      this.userIdLabel.AutoSize = true;
      this.userIdLabel.Location = new System.Drawing.Point(3, 85);
      this.userIdLabel.Name = "userIdLabel";
      this.userIdLabel.Size = new System.Drawing.Size(41, 13);
      this.userIdLabel.TabIndex = 8;
      this.userIdLabel.Text = "UserId:";
      // 
      // exceptionLabel
      // 
      this.exceptionLabel.AutoSize = true;
      this.exceptionLabel.Location = new System.Drawing.Point(3, 59);
      this.exceptionLabel.Name = "exceptionLabel";
      this.exceptionLabel.Size = new System.Drawing.Size(57, 13);
      this.exceptionLabel.TabIndex = 9;
      this.exceptionLabel.Text = "Exception:";
      // 
      // StateLogView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.exceptionLabel);
      this.Controls.Add(this.userIdLabel);
      this.Controls.Add(this.slaveIdLabel);
      this.Controls.Add(this.slaveIdTextBox);
      this.Controls.Add(this.userIdTextBox);
      this.Controls.Add(this.exceptionTextBox);
      this.Controls.Add(this.dateTextBox);
      this.Controls.Add(this.dateLabel);
      this.Controls.Add(this.stateTextBox);
      this.Controls.Add(this.stateLabel);
      this.Name = "StateLogView";
      this.Size = new System.Drawing.Size(523, 192);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label stateLabel;
    private System.Windows.Forms.TextBox stateTextBox;
    private System.Windows.Forms.Label dateLabel;
    private System.Windows.Forms.TextBox dateTextBox;
    private System.Windows.Forms.TextBox exceptionTextBox;
    private System.Windows.Forms.TextBox userIdTextBox;
    private System.Windows.Forms.TextBox slaveIdTextBox;
    private System.Windows.Forms.Label slaveIdLabel;
    private System.Windows.Forms.Label userIdLabel;
    private System.Windows.Forms.Label exceptionLabel;

  }
}
