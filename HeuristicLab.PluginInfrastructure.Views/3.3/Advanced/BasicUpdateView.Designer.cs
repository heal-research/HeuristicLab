#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class BasicUpdateView {
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
      this.updateAndInstallButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // updateAndInstallButton
      // 
      this.updateAndInstallButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.updateAndInstallButton.Image = HeuristicLab.PluginInfrastructure.Resources.Internet;
      this.updateAndInstallButton.Location = new System.Drawing.Point(3, 2);
      this.updateAndInstallButton.Name = "updateAndInstallButton";
      this.updateAndInstallButton.Size = new System.Drawing.Size(155, 50);
      this.updateAndInstallButton.TabIndex = 0;
      this.updateAndInstallButton.Text = "Find and Install Updates";
      this.updateAndInstallButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
      this.toolTip.SetToolTip(this.updateAndInstallButton, "Find, download and install updates for all installed plugins.");
      this.updateAndInstallButton.UseVisualStyleBackColor = true;
      this.updateAndInstallButton.Click += new System.EventHandler(this.updateAndInstallButton_Click);
      // 
      // BasicUpdateView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.updateAndInstallButton);
      this.Name = "BasicUpdateView";
      this.Size = new System.Drawing.Size(163, 55);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button updateAndInstallButton;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
