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

using System;
namespace HeuristicLab.Clients.Access.Views {
  partial class RefreshableLightweightUserInformationView {
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
        lightweightUserInformationView.UserInformationChanged -= new EventHandler(lightweightUserInformationView_Changed);
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RefreshableLightweightUserInformationView));
      this.lightweightUserInformationView = new HeuristicLab.Clients.Access.Views.LightweightUserInformationView();
      this.storeButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // refreshButton
      // 
      this.refreshButton.Location = new System.Drawing.Point(4, 3);
      // 
      // lightweightUserInformationView
      // 
      this.lightweightUserInformationView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lightweightUserInformationView.Caption = "RefreshableLightweightUserInformation View";
      this.lightweightUserInformationView.Content = null;
      this.lightweightUserInformationView.Location = new System.Drawing.Point(3, 33);
      this.lightweightUserInformationView.Name = "lightweightUserInformationView";
      this.lightweightUserInformationView.ReadOnly = false;
      this.lightweightUserInformationView.Size = new System.Drawing.Size(378, 245);
      this.lightweightUserInformationView.TabIndex = 2;
      // 
      // storeButton
      // 
      this.storeButton.Enabled = false;
      this.storeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.PublishToWeb;
      this.storeButton.Location = new System.Drawing.Point(34, 3);
      this.storeButton.Name = "storeButton";
      this.storeButton.Size = new System.Drawing.Size(24, 24);
      this.storeButton.TabIndex = 3;
      this.storeButton.UseVisualStyleBackColor = true;
      this.storeButton.Click += new System.EventHandler(this.storeButton_Click);
      // 
      // RefreshableLightweightUserInformationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lightweightUserInformationView);
      this.Controls.Add(this.storeButton);
      this.Name = "RefreshableLightweightUserInformationView";
      this.Size = new System.Drawing.Size(384, 281);
      this.Controls.SetChildIndex(this.storeButton, 0);
      this.Controls.SetChildIndex(this.lightweightUserInformationView, 0);
      this.Controls.SetChildIndex(this.refreshButton, 0);
      this.ResumeLayout(false);

    }

    #endregion

    private LightweightUserInformationView lightweightUserInformationView;
    private System.Windows.Forms.Button storeButton;

  }
}
