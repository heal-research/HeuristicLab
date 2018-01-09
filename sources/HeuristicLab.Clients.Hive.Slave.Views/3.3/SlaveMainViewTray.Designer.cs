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

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  partial class SlaveMainViewTray {
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
      this.slaveCmdsWithKill = new HeuristicLab.Clients.Hive.SlaveCore.Views.SlaveCmdsWithKill();
      this.tabSlaveView.SuspendLayout();
      this.mainTabControl.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.slaveCmdsWithKill);
      // 
      // slaveCmdsWithKill
      // 
      this.slaveCmdsWithKill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.slaveCmdsWithKill.Caption = "HeuristicLab Slave Cmds with Kill View";
      this.slaveCmdsWithKill.Content = null;
      this.slaveCmdsWithKill.Location = new System.Drawing.Point(3, 3);
      this.slaveCmdsWithKill.Name = "slaveCmdsWithKill";
      this.slaveCmdsWithKill.ReadOnly = false;
      this.slaveCmdsWithKill.Size = new System.Drawing.Size(92, 35);
      this.slaveCmdsWithKill.TabIndex = 0;
      // 
      // SlaveMainViewTray
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "SlaveMainViewTray";
      this.tabSlaveView.ResumeLayout(false);
      this.mainTabControl.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private SlaveCmdsWithKill slaveCmdsWithKill;



  }
}
