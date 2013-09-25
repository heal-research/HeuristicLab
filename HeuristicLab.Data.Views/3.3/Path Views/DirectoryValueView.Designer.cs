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

namespace HeuristicLab.Data.Views {
  partial class DirectoryValueView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectoryValueView));
      this.openButton = new System.Windows.Forms.Button();
      this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
      this.stringConvertibleValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.SuspendLayout();
      // 
      // openButton
      // 
      this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.openButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.openButton.Location = new System.Drawing.Point(167, 0);
      this.openButton.Name = "openButton";
      this.openButton.Size = new System.Drawing.Size(24, 24);
      this.openButton.TabIndex = 3;
      this.openButton.UseVisualStyleBackColor = true;
      this.openButton.Click += new System.EventHandler(this.openButton_Click);
      // 
      // stringConvertibleValueView
      // 
      this.stringConvertibleValueView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stringConvertibleValueView.Caption = "StringConvertibleValue View";
      this.stringConvertibleValueView.Content = null;
      this.stringConvertibleValueView.LabelVisible = true;
      this.stringConvertibleValueView.Location = new System.Drawing.Point(4, 2);
      this.stringConvertibleValueView.Name = "stringConvertibleValueView";
      this.stringConvertibleValueView.ReadOnly = false;
      this.stringConvertibleValueView.Size = new System.Drawing.Size(157, 21);
      this.stringConvertibleValueView.TabIndex = 4;
      // 
      // DirectoryValueView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.stringConvertibleValueView);
      this.Controls.Add(this.openButton);
      this.Name = "DirectoryValueView";
      this.Size = new System.Drawing.Size(194, 24);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Button openButton;
    protected System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    private StringConvertibleValueView stringConvertibleValueView;

  }
}
