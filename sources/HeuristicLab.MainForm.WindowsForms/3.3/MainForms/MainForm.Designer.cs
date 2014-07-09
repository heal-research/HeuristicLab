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
namespace HeuristicLab.MainForm.WindowsForms {
  partial class MainForm {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.toolStrip = new System.Windows.Forms.ToolStrip();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(624, 24);
      this.menuStrip.TabIndex = 0;
      this.menuStrip.Text = "menuStrip";
      // 
      // toolStrip
      // 
      this.toolStrip.Location = new System.Drawing.Point(0, 24);
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.Size = new System.Drawing.Size(624, 25);
      this.toolStrip.TabIndex = 1;
      // 
      // statusStrip
      // 
      this.statusStrip.Location = new System.Drawing.Point(0, 390);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(624, 22);
      this.statusStrip.TabIndex = 2;
      this.statusStrip.Text = "statusStrip";
      // 
      // MainFormBase
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(624, 412);
      this.Controls.Add(this.statusStrip);
      this.Controls.Add(this.toolStrip);
      this.Controls.Add(this.menuStrip);
      this.Icon = HeuristicLab.Common.Resources.HeuristicLab.Icon;
      this.MainMenuStrip = this.menuStrip;
      this.Name = "MainFormBase";
      this.Load += new System.EventHandler(this.MainFormBase_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.ToolStrip toolStrip;
    protected System.Windows.Forms.StatusStrip statusStrip;
    protected System.Windows.Forms.MenuStrip menuStrip;
  }
}