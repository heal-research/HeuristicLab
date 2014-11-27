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

namespace HeuristicLab.Core.Views {
  partial class NestingLevelErrorControl {
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
      this.infoLabel = new System.Windows.Forms.Label();
      this.showButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // infoLabel
      // 
      this.infoLabel.AutoSize = true;
      this.infoLabel.Location = new System.Drawing.Point(3, 3);
      this.infoLabel.Name = "infoLabel";
      this.infoLabel.Size = new System.Drawing.Size(382, 13);
      this.infoLabel.TabIndex = 0;
      this.infoLabel.Text = "Please click the button below to open a new tab for displaying the desired view.";
      this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // showButton
      // 
      this.showButton.Location = new System.Drawing.Point(7, 21);
      this.showButton.Name = "showButton";
      this.showButton.Size = new System.Drawing.Size(132, 23);
      this.showButton.TabIndex = 1;
      this.showButton.Text = "Show view in new tab";
      this.showButton.UseVisualStyleBackColor = true;
      this.showButton.Click += new System.EventHandler(this.showButton_Click);
      // 
      // NestingLevelErrorControl
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.showButton);
      this.Controls.Add(this.infoLabel);
      this.Name = "NestingLevelErrorControl";
      this.Size = new System.Drawing.Size(387, 50);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label infoLabel;
    private System.Windows.Forms.Button showButton;
  }
}
