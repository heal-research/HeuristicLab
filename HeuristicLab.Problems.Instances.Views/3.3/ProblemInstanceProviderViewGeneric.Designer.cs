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


namespace HeuristicLab.Problems.Instances.Views {
  partial class ProblemInstanceProviderView<T> {
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
      this.instancesComboBox = new System.Windows.Forms.ComboBox();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.instanceLabel = new System.Windows.Forms.Label();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.exportButton = new System.Windows.Forms.Button();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.importButton = new System.Windows.Forms.Button();
      this.libraryInfoButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // instancesComboBox
      // 
      this.instancesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.instancesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.instancesComboBox.FormattingEnabled = true;
      this.instancesComboBox.Location = new System.Drawing.Point(60, 2);
      this.instancesComboBox.Name = "instancesComboBox";
      this.instancesComboBox.Size = new System.Drawing.Size(540, 21);
      this.instancesComboBox.TabIndex = 7;
      this.instancesComboBox.SelectionChangeCommitted += new System.EventHandler(this.instancesComboBox_SelectionChangeCommitted);
      this.instancesComboBox.DataSourceChanged += new System.EventHandler(this.instancesComboBox_DataSourceChanged);
      // 
      // openFileDialog
      // 
      this.openFileDialog.Filter = "All files|*.*";
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.Filter = "CSV files|*.csv|All files|*.*";
      this.saveFileDialog.Title = "Save RegressionInstance...";
      // 
      // instanceLabel
      // 
      this.instanceLabel.AutoSize = true;
      this.instanceLabel.Location = new System.Drawing.Point(3, 5);
      this.instanceLabel.Name = "instanceLabel";
      this.instanceLabel.Size = new System.Drawing.Size(51, 13);
      this.instanceLabel.TabIndex = 4;
      this.instanceLabel.Text = "Instance:";
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer2.IsSplitterFixed = true;
      this.splitContainer2.Location = new System.Drawing.Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.exportButton);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.instanceLabel);
      this.splitContainer2.Panel2.Controls.Add(this.instancesComboBox);
      this.splitContainer2.Size = new System.Drawing.Size(632, 23);
      this.splitContainer2.SplitterDistance = 25;
      this.splitContainer2.TabIndex = 21;
      // 
      // exportButton
      // 
      this.exportButton.Location = new System.Drawing.Point(1, 0);
      this.exportButton.Name = "exportButton";
      this.exportButton.Size = new System.Drawing.Size(24, 24);
      this.exportButton.TabIndex = 20;
      this.exportButton.Text = "Export";
      this.exportButton.UseVisualStyleBackColor = true;
      this.exportButton.Click += new System.EventHandler(exportButton_Click);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer1.IsSplitterFixed = true;
      this.splitContainer1.Location = new System.Drawing.Point(33, -1);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.importButton);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
      this.splitContainer1.Size = new System.Drawing.Size(661, 23);
      this.splitContainer1.SplitterDistance = 25;
      this.splitContainer1.TabIndex = 21;
      // 
      // importButton
      // 
      this.importButton.Location = new System.Drawing.Point(0, 0);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(24, 24);
      this.importButton.TabIndex = 19;
      this.importButton.Text = "Import";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(importButton_Click);
      // 
      // libraryInfoButton
      // 
      this.libraryInfoButton.Location = new System.Drawing.Point(3, -1);
      this.libraryInfoButton.Name = "libraryInfoButton";
      this.libraryInfoButton.Size = new System.Drawing.Size(24, 24);
      this.libraryInfoButton.TabIndex = 22;
      this.libraryInfoButton.Text = "Info";
      this.libraryInfoButton.UseVisualStyleBackColor = true;
      this.libraryInfoButton.Click += new System.EventHandler(this.libraryInfoButton_Click);
      // 
      // ProblemInstanceProviderViewGeneric
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.libraryInfoButton);
      this.Controls.Add(this.splitContainer1);
      this.Name = "ProblemInstanceProviderViewGeneric";
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

    protected System.Windows.Forms.OpenFileDialog openFileDialog;
    protected System.Windows.Forms.ComboBox instancesComboBox;
    protected System.Windows.Forms.SaveFileDialog saveFileDialog;
    protected System.Windows.Forms.Label instanceLabel;
    protected System.Windows.Forms.SplitContainer splitContainer2;
    protected System.Windows.Forms.SplitContainer splitContainer1;
    protected System.Windows.Forms.Button libraryInfoButton;
    protected System.Windows.Forms.Button importButton;
    protected System.Windows.Forms.Button exportButton;
  }
}
