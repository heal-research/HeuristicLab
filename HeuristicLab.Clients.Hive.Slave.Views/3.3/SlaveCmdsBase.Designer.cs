#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class SlaveCmdsBase {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlaveCmdsBase));
      this.btnStart = new System.Windows.Forms.Button();
      this.btnStop = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // btnStart
      // 
      this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
      this.btnStart.Location = new System.Drawing.Point(3, 3);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(24, 24);
      this.btnStart.TabIndex = 10;
      this.toolTip.SetToolTip(this.btnStart, "Start/Resume the slave");
      this.btnStart.UseVisualStyleBackColor = true;
      this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
      // 
      // btnStop
      // 
      this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
      this.btnStop.Location = new System.Drawing.Point(33, 3);
      this.btnStop.Name = "btnStop";
      this.btnStop.Size = new System.Drawing.Size(24, 24);
      this.btnStop.TabIndex = 11;
      this.toolTip.SetToolTip(this.btnStop, "Pause slave");
      this.btnStop.UseVisualStyleBackColor = true;
      this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
      // 
      // SlaveCmdsBase
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.btnStop);
      this.Controls.Add(this.btnStart);
      this.Name = "SlaveCmdsBase";
      this.Size = new System.Drawing.Size(92, 30);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.Button btnStop;
    protected System.Windows.Forms.ToolTip toolTip;
  }
}
