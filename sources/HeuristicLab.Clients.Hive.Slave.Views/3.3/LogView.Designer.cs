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

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  partial class LogView {
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
      this.chkShowBalloonTips = new System.Windows.Forms.CheckBox();
      this.hlLogView = new HeuristicLab.Core.Views.LogView();
      this.SuspendLayout();
      // 
      // chkShowBalloonTips
      // 
      this.chkShowBalloonTips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.chkShowBalloonTips.AutoSize = true;
      this.chkShowBalloonTips.Location = new System.Drawing.Point(3, 247);
      this.chkShowBalloonTips.Name = "chkShowBalloonTips";
      this.chkShowBalloonTips.Size = new System.Drawing.Size(291, 17);
      this.chkShowBalloonTips.TabIndex = 3;
      this.chkShowBalloonTips.Text = "Show Ballon Tips in System Tray on important messages";
      this.chkShowBalloonTips.UseVisualStyleBackColor = true;
      this.chkShowBalloonTips.CheckedChanged += new System.EventHandler(this.chkShowBalloonTips_CheckedChanged);
      // 
      // hlLogView
      // 
      this.hlLogView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.hlLogView.Caption = "Log View";
      this.hlLogView.Content = null;
      this.hlLogView.Location = new System.Drawing.Point(3, 3);
      this.hlLogView.Name = "hlLogView";
      this.hlLogView.ReadOnly = false;
      this.hlLogView.Size = new System.Drawing.Size(480, 238);
      this.hlLogView.TabIndex = 4;
      // 
      // LogView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.hlLogView);
      this.Controls.Add(this.chkShowBalloonTips);
      this.Name = "LogView";
      this.Size = new System.Drawing.Size(486, 267);
      this.Load += new System.EventHandler(this.LogView_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox chkShowBalloonTips;
    private HeuristicLab.Core.Views.LogView hlLogView;
  }
}
