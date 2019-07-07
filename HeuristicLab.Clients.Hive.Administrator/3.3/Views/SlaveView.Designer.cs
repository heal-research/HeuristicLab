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

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class SlaveView {
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
      this.cpuLabel = new System.Windows.Forms.Label();
      this.cpuTextBox = new System.Windows.Forms.TextBox();
      this.memoryLabel = new System.Windows.Forms.Label();
      this.memoryTextBox = new System.Windows.Forms.TextBox();
      this.operatingSystemLabel = new System.Windows.Forms.Label();
      this.operatingSystemTextBox = new System.Windows.Forms.TextBox();
      this.stateLabel = new System.Windows.Forms.Label();
      this.stateTextBox = new System.Windows.Forms.TextBox();
      this.lastHeartbeatLabel = new System.Windows.Forms.Label();
      this.lastHeartbeatTextBox = new System.Windows.Forms.TextBox();
      this.disposableLabel = new System.Windows.Forms.Label();
      this.disposableCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.heartbeatIntervalNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.nameTextBox.ReadOnly = true;
      // 
      // cpuLabel
      // 
      this.cpuLabel.AutoSize = true;
      this.cpuLabel.Location = new System.Drawing.Point(3, 141);
      this.cpuLabel.Name = "cpuLabel";
      this.cpuLabel.Size = new System.Drawing.Size(32, 13);
      this.cpuLabel.TabIndex = 16;
      this.cpuLabel.Text = "CPU:";
      // 
      // cpuTextBox
      // 
      this.cpuTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cpuTextBox.Location = new System.Drawing.Point(130, 138);
      this.cpuTextBox.Name = "cpuTextBox";
      this.cpuTextBox.ReadOnly = true;
      this.cpuTextBox.Size = new System.Drawing.Size(397, 20);
      this.cpuTextBox.TabIndex = 17;
      // 
      // memoryLabel
      // 
      this.memoryLabel.AutoSize = true;
      this.memoryLabel.Location = new System.Drawing.Point(3, 167);
      this.memoryLabel.Name = "memoryLabel";
      this.memoryLabel.Size = new System.Drawing.Size(47, 13);
      this.memoryLabel.TabIndex = 18;
      this.memoryLabel.Text = "Memory:";
      // 
      // memoryTextBox
      // 
      this.memoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.memoryTextBox.Location = new System.Drawing.Point(130, 164);
      this.memoryTextBox.Name = "memoryTextBox";
      this.memoryTextBox.ReadOnly = true;
      this.memoryTextBox.Size = new System.Drawing.Size(397, 20);
      this.memoryTextBox.TabIndex = 19;
      // 
      // operatingSystemLabel
      // 
      this.operatingSystemLabel.AutoSize = true;
      this.operatingSystemLabel.Location = new System.Drawing.Point(3, 193);
      this.operatingSystemLabel.Name = "operatingSystemLabel";
      this.operatingSystemLabel.Size = new System.Drawing.Size(93, 13);
      this.operatingSystemLabel.TabIndex = 20;
      this.operatingSystemLabel.Text = "Operating System:";
      // 
      // operatingSystemTextBox
      // 
      this.operatingSystemTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.operatingSystemTextBox.Location = new System.Drawing.Point(130, 190);
      this.operatingSystemTextBox.Name = "operatingSystemTextBox";
      this.operatingSystemTextBox.ReadOnly = true;
      this.operatingSystemTextBox.Size = new System.Drawing.Size(397, 20);
      this.operatingSystemTextBox.TabIndex = 21;
      // 
      // stateLabel
      // 
      this.stateLabel.AutoSize = true;
      this.stateLabel.Location = new System.Drawing.Point(3, 219);
      this.stateLabel.Name = "stateLabel";
      this.stateLabel.Size = new System.Drawing.Size(35, 13);
      this.stateLabel.TabIndex = 22;
      this.stateLabel.Text = "State:";
      // 
      // stateTextBox
      // 
      this.stateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stateTextBox.Location = new System.Drawing.Point(130, 216);
      this.stateTextBox.Name = "stateTextBox";
      this.stateTextBox.ReadOnly = true;
      this.stateTextBox.Size = new System.Drawing.Size(397, 20);
      this.stateTextBox.TabIndex = 23;
      // 
      // lastHeartbeatLabel
      // 
      this.lastHeartbeatLabel.AutoSize = true;
      this.lastHeartbeatLabel.Location = new System.Drawing.Point(3, 245);
      this.lastHeartbeatLabel.Name = "lastHeartbeatLabel";
      this.lastHeartbeatLabel.Size = new System.Drawing.Size(80, 13);
      this.lastHeartbeatLabel.TabIndex = 24;
      this.lastHeartbeatLabel.Text = "Last Heartbeat:";
      // 
      // lastHeartbeatTextBox
      // 
      this.lastHeartbeatTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lastHeartbeatTextBox.Location = new System.Drawing.Point(130, 242);
      this.lastHeartbeatTextBox.Name = "lastHeartbeatTextBox";
      this.lastHeartbeatTextBox.ReadOnly = true;
      this.lastHeartbeatTextBox.Size = new System.Drawing.Size(397, 20);
      this.lastHeartbeatTextBox.TabIndex = 25;
      // 
      // disposableLabel
      // 
      this.disposableLabel.AutoSize = true;
      this.disposableLabel.Location = new System.Drawing.Point(3, 271);
      this.disposableLabel.Name = "disposableLabel";
      this.disposableLabel.Size = new System.Drawing.Size(62, 13);
      this.disposableLabel.TabIndex = 28;
      this.disposableLabel.Text = "Disposable:";
      // 
      // disposableCheckBox
      // 
      this.disposableCheckBox.AutoSize = true;
      this.disposableCheckBox.Location = new System.Drawing.Point(130, 270);
      this.disposableCheckBox.Name = "disposableCheckBox";
      this.disposableCheckBox.Size = new System.Drawing.Size(15, 14);
      this.disposableCheckBox.TabIndex = 29;
      this.disposableCheckBox.UseVisualStyleBackColor = true;
      this.disposableCheckBox.CheckedChanged += new System.EventHandler(this.disposableCheckBox_CheckedChanged);
      // 
      // SlaveView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.disposableCheckBox);
      this.Controls.Add(this.disposableLabel);
      this.Controls.Add(this.lastHeartbeatTextBox);
      this.Controls.Add(this.lastHeartbeatLabel);
      this.Controls.Add(this.stateTextBox);
      this.Controls.Add(this.stateLabel);
      this.Controls.Add(this.operatingSystemTextBox);
      this.Controls.Add(this.operatingSystemLabel);
      this.Controls.Add(this.memoryTextBox);
      this.Controls.Add(this.memoryLabel);
      this.Controls.Add(this.cpuTextBox);
      this.Controls.Add(this.cpuLabel);
      this.Name = "SlaveView";
      this.Controls.SetChildIndex(this.idTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.Controls.SetChildIndex(this.heartbeatIntervalNumericUpDown, 0);
      this.Controls.SetChildIndex(this.publicCheckBox, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.cpuLabel, 0);
      this.Controls.SetChildIndex(this.cpuTextBox, 0);
      this.Controls.SetChildIndex(this.memoryLabel, 0);
      this.Controls.SetChildIndex(this.memoryTextBox, 0);
      this.Controls.SetChildIndex(this.operatingSystemLabel, 0);
      this.Controls.SetChildIndex(this.operatingSystemTextBox, 0);
      this.Controls.SetChildIndex(this.stateLabel, 0);
      this.Controls.SetChildIndex(this.stateTextBox, 0);
      this.Controls.SetChildIndex(this.lastHeartbeatLabel, 0);
      this.Controls.SetChildIndex(this.lastHeartbeatTextBox, 0);
      this.Controls.SetChildIndex(this.disposableLabel, 0);
      this.Controls.SetChildIndex(this.disposableCheckBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.heartbeatIntervalNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label cpuLabel;
    private System.Windows.Forms.TextBox cpuTextBox;
    private System.Windows.Forms.Label memoryLabel;
    private System.Windows.Forms.TextBox memoryTextBox;
    private System.Windows.Forms.Label operatingSystemLabel;
    private System.Windows.Forms.TextBox operatingSystemTextBox;
    private System.Windows.Forms.Label stateLabel;
    private System.Windows.Forms.TextBox stateTextBox;
    private System.Windows.Forms.Label lastHeartbeatLabel;
    private System.Windows.Forms.TextBox lastHeartbeatTextBox;
    private System.Windows.Forms.Label disposableLabel;
    private System.Windows.Forms.CheckBox disposableCheckBox;
  }
}
