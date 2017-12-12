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

namespace HeuristicLab.Clients.Access.Views {
  partial class LightweightUserGroupSelectionDialog {
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
      this.OkButton = new System.Windows.Forms.Button();
      this.AbortButton = new System.Windows.Forms.Button();
      this.lightweightUserGroupSelectionView = new HeuristicLab.Clients.Access.Views.LightweightUserGroupSelectionView();
      this.SuspendLayout();
      // 
      // OkButton
      // 
      this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.OkButton.Location = new System.Drawing.Point(374, 290);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(75, 23);
      this.OkButton.TabIndex = 1;
      this.OkButton.Text = "OK";
      this.OkButton.UseVisualStyleBackColor = true;
      this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
      // 
      // AbortButton
      // 
      this.AbortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.AbortButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.AbortButton.Location = new System.Drawing.Point(455, 290);
      this.AbortButton.Name = "AbortButton";
      this.AbortButton.Size = new System.Drawing.Size(75, 23);
      this.AbortButton.TabIndex = 2;
      this.AbortButton.Text = "Cancel";
      this.AbortButton.UseVisualStyleBackColor = true;
      this.AbortButton.Click += new System.EventHandler(this.CancelButton_Click);
      // 
      // lightweightUserGroupSelectionView
      // 
      this.lightweightUserGroupSelectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lightweightUserGroupSelectionView.Location = new System.Drawing.Point(12, 12);
      this.lightweightUserGroupSelectionView.Name = "lightweightUserGroupSelectionView";
      this.lightweightUserGroupSelectionView.Size = new System.Drawing.Size(518, 272);
      this.lightweightUserGroupSelectionView.TabIndex = 0;
      // 
      // LightweightUserGroupSelectionDialog
      // 
      this.AcceptButton = this.OkButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.AbortButton;
      this.ClientSize = new System.Drawing.Size(542, 325);
      this.Controls.Add(this.AbortButton);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.lightweightUserGroupSelectionView);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "LightweightUserGroupSelectionDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Select users and groups";
      this.TopMost = true;
      this.ResumeLayout(false);

    }

    #endregion

    private LightweightUserGroupSelectionView lightweightUserGroupSelectionView;
    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.Button AbortButton;
  }
}