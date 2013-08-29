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

namespace HeuristicLab.MainForm.WindowsForms {
  partial class View {
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
      this.helpLabel = new System.Windows.Forms.Label();
      this.helpToolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // helpLabel
      // 
      this.helpLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.helpLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Help;
      this.helpLabel.Location = new System.Drawing.Point(131, 0);
      this.helpLabel.Name = "helpLabel";
      this.helpLabel.Size = new System.Drawing.Size(16, 16);
      this.helpLabel.TabIndex = 0;
      this.helpToolTip.SetToolTip(this.helpLabel, "Double Click to Show Help");
      this.helpLabel.Visible = false;
      this.helpLabel.DoubleClick += new System.EventHandler(this.helpLabel_DoubleClick);
      // 
      // View
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.helpLabel);
      this.Name = "View";
      this.Load += new System.EventHandler(this.View_Load);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Label helpLabel;
    protected System.Windows.Forms.ToolTip helpToolTip;

  }
}
