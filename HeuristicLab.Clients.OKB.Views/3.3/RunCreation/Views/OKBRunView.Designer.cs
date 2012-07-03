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

namespace HeuristicLab.Clients.OKB.RunCreation.Views {
  partial class OKBRunView {
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
      this.storeButton = new System.Windows.Forms.Button();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.SuspendLayout();
      // 
      // storeButton
      // 
      this.storeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.PublishToWeb;
      this.storeButton.Location = new System.Drawing.Point(0, 0);
      this.storeButton.Name = "storeButton";
      this.storeButton.Size = new System.Drawing.Size(24, 24);
      this.storeButton.TabIndex = 0;
      this.storeButton.UseVisualStyleBackColor = true;
      this.storeButton.Click += new System.EventHandler(this.storeButton_Click);
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(0, 30);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(631, 411);
      this.viewHost.TabIndex = 1;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // OKBRunView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.viewHost);
      this.Controls.Add(this.storeButton);
      this.Name = "OKBRunView";
      this.Size = new System.Drawing.Size(631, 441);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button storeButton;
    private MainForm.WindowsForms.ViewHost viewHost;
  }
}
