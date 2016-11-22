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

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class DowntimeTypeView {
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
      this.OfflineRadioButton = new System.Windows.Forms.RadioButton();
      this.shutdownRadioButton = new System.Windows.Forms.RadioButton();
      this.panel1 = new System.Windows.Forms.Panel();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // OfflineRadioButton
      // 
      this.OfflineRadioButton.AutoSize = true;
      this.OfflineRadioButton.Checked = true;
      this.OfflineRadioButton.Location = new System.Drawing.Point(3, 3);
      this.OfflineRadioButton.Name = "OfflineRadioButton";
      this.OfflineRadioButton.Size = new System.Drawing.Size(55, 17);
      this.OfflineRadioButton.TabIndex = 0;
      this.OfflineRadioButton.TabStop = true;
      this.OfflineRadioButton.Tag = "Offline";
      this.OfflineRadioButton.Text = "Offline";
      this.OfflineRadioButton.UseVisualStyleBackColor = true;
      this.OfflineRadioButton.CheckedChanged += new System.EventHandler(this.OfflineRadioButton_CheckedChanged);
      // 
      // shutdownRadioButton
      // 
      this.shutdownRadioButton.AutoSize = true;
      this.shutdownRadioButton.Location = new System.Drawing.Point(3, 26);
      this.shutdownRadioButton.Name = "shutdownRadioButton";
      this.shutdownRadioButton.Size = new System.Drawing.Size(105, 17);
      this.shutdownRadioButton.TabIndex = 1;
      this.shutdownRadioButton.TabStop = true;
      this.shutdownRadioButton.Tag = "Shutdown";
      this.shutdownRadioButton.Text = "Shutdown Signal";
      this.shutdownRadioButton.UseVisualStyleBackColor = true;
      this.shutdownRadioButton.CheckedChanged += new System.EventHandler(this.shutdownRadioButton_CheckedChanged);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.shutdownRadioButton);
      this.panel1.Controls.Add(this.OfflineRadioButton);
      this.panel1.Location = new System.Drawing.Point(3, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(114, 51);
      this.panel1.TabIndex = 2;
      // 
      // AppointmentTypeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.panel1);
      this.Name = "AppointmentTypeView";
      this.Size = new System.Drawing.Size(120, 57);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RadioButton OfflineRadioButton;
    private System.Windows.Forms.RadioButton shutdownRadioButton;
    private System.Windows.Forms.Panel panel1;
  }
}
