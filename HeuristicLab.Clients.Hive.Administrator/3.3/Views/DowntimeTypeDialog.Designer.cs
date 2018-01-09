#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class DowntimeTypeDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.okButton = new System.Windows.Forms.Button();
      this.cancleButton = new System.Windows.Forms.Button();
      this.appointmentTypeView = new HeuristicLab.Clients.Hive.Administrator.Views.DowntimeTypeView();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(12, 75);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancleButton
      // 
      this.cancleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancleButton.Location = new System.Drawing.Point(127, 75);
      this.cancleButton.Name = "cancleButton";
      this.cancleButton.Size = new System.Drawing.Size(75, 23);
      this.cancleButton.TabIndex = 1;
      this.cancleButton.Text = "Cancel";
      this.cancleButton.UseVisualStyleBackColor = true;
      this.cancleButton.Click += new System.EventHandler(this.cancleButton_Click);
      // 
      // appointmentTypeView
      // 
      this.appointmentTypeView.DowntimeType = HeuristicLab.Clients.Hive.DowntimeType.Offline;
      this.appointmentTypeView.Location = new System.Drawing.Point(12, 12);
      this.appointmentTypeView.Name = "appointmentTypeView";
      this.appointmentTypeView.Size = new System.Drawing.Size(120, 57);
      this.appointmentTypeView.TabIndex = 2;
      // 
      // AppointmentTypeDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancleButton;
      this.ClientSize = new System.Drawing.Size(214, 108);
      this.Controls.Add(this.appointmentTypeView);
      this.Controls.Add(this.cancleButton);
      this.Controls.Add(this.okButton);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AppointmentTypeDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Select Type of Downtime";
      this.TopMost = true;
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancleButton;
    private DowntimeTypeView appointmentTypeView;
  }
}