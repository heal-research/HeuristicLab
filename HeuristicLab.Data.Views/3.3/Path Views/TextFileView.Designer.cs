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


namespace HeuristicLab.Data.Views {
  partial class TextFileView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextFileView));
      this.saveButton = new System.Windows.Forms.Button();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.fileSystemWatcher = new System.IO.FileSystemWatcher();
      this.textBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
      this.SuspendLayout();
      // 
      // openButton
      // 
      this.openButton.Location = new System.Drawing.Point(365, 0);
      // 
      // stringConvertibleValueView
      // 
      this.stringConvertibleValueView.Size = new System.Drawing.Size(355, 21);
      // 
      // saveButton
      // 
      this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.saveButton.CausesValidation = false;
      this.saveButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
      this.saveButton.Location = new System.Drawing.Point(395, 0);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(24, 24);
      this.saveButton.TabIndex = 2;
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // fileSystemWatcher
      // 
      this.fileSystemWatcher.EnableRaisingEvents = true;
      this.fileSystemWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)));
      this.fileSystemWatcher.SynchronizingObject = this;
      this.fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
      this.fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
      this.fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
      this.fileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(this.fileSystemWatcher_Renamed);
      // 
      // textBox
      // 
      this.textBox.AcceptsReturn = true;
      this.textBox.AcceptsTab = true;
      this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox.Location = new System.Drawing.Point(3, 30);
      this.textBox.Multiline = true;
      this.textBox.Name = "textBox";
      this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.textBox.Size = new System.Drawing.Size(415, 368);
      this.textBox.TabIndex = 4;
      this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
      this.textBox.Validated += new System.EventHandler(this.textBox_Validated);
      // 
      // TextFileView
      // 
      this.Controls.Add(this.textBox);
      this.Controls.Add(this.saveButton);
      this.Name = "TextFileView";
      this.Size = new System.Drawing.Size(421, 401);
      this.Controls.SetChildIndex(this.saveButton, 0);
      this.Controls.SetChildIndex(this.textBox, 0);
      this.Controls.SetChildIndex(this.openButton, 0);
      this.Controls.SetChildIndex(this.stringConvertibleValueView, 0);
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;
    private System.IO.FileSystemWatcher fileSystemWatcher;
    private System.Windows.Forms.TextBox textBox;
  }
}
