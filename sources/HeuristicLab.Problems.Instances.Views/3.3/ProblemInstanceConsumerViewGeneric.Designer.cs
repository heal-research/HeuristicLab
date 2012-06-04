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

namespace HeuristicLab.Problems.Instances.Views {
  partial class ProblemInstanceConsumerViewGeneric<T> {
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
      this.problemInstanceProviderComboBox = new System.Windows.Forms.ComboBox();
      this.libraryInfoButton = new System.Windows.Forms.Button();
      this.libraryLabel = new System.Windows.Forms.Label();
      this.problemInstanceProviderViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.importButton = new System.Windows.Forms.Button();
      this.exportButton = new System.Windows.Forms.Button();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.problemInstanceProviderSplitContainer = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceProviderSplitContainer)).BeginInit();
      this.problemInstanceProviderSplitContainer.Panel1.SuspendLayout();
      this.problemInstanceProviderSplitContainer.Panel2.SuspendLayout();
      this.problemInstanceProviderSplitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // problemInstanceProviderComboBox
      // 
      this.problemInstanceProviderComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemInstanceProviderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.problemInstanceProviderComboBox.FormattingEnabled = true;
      this.problemInstanceProviderComboBox.Location = new System.Drawing.Point(50, 1);
      this.problemInstanceProviderComboBox.Name = "problemInstanceProviderComboBox";
      this.problemInstanceProviderComboBox.Size = new System.Drawing.Size(171, 21);
      this.problemInstanceProviderComboBox.TabIndex = 16;
      this.problemInstanceProviderComboBox.SelectedIndexChanged += new System.EventHandler(this.problemInstanceProviderComboBox_SelectedIndexChanged);
      // 
      // libraryInfoButton
      // 
      this.libraryInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.libraryInfoButton.Location = new System.Drawing.Point(227, -1);
      this.libraryInfoButton.Name = "libraryInfoButton";
      this.libraryInfoButton.Size = new System.Drawing.Size(24, 24);
      this.libraryInfoButton.TabIndex = 17;
      this.libraryInfoButton.Text = "Info";
      this.libraryInfoButton.UseVisualStyleBackColor = true;
      this.libraryInfoButton.Click += new System.EventHandler(this.libraryInfoButton_Click);
      // 
      // libraryLabel
      // 
      this.libraryLabel.AutoSize = true;
      this.libraryLabel.Location = new System.Drawing.Point(4, 4);
      this.libraryLabel.Name = "libraryLabel";
      this.libraryLabel.Size = new System.Drawing.Size(41, 13);
      this.libraryLabel.TabIndex = 15;
      this.libraryLabel.Text = "Library:";
      // 
      // problemInstanceProviderViewHost
      // 
      this.problemInstanceProviderViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemInstanceProviderViewHost.Caption = "View";
      this.problemInstanceProviderViewHost.Content = null;
      this.problemInstanceProviderViewHost.Enabled = false;
      this.problemInstanceProviderViewHost.Location = new System.Drawing.Point(318, 0);
      this.problemInstanceProviderViewHost.Name = "problemInstanceProviderViewHost";
      this.problemInstanceProviderViewHost.ReadOnly = false;
      this.problemInstanceProviderViewHost.Size = new System.Drawing.Size(373, 22);
      this.problemInstanceProviderViewHost.TabIndex = 15;
      this.problemInstanceProviderViewHost.ViewsLabelVisible = false;
      this.problemInstanceProviderViewHost.ViewType = null;
      // 
      // importButton
      // 
      this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.importButton.Location = new System.Drawing.Point(257, -1);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(24, 24);
      this.importButton.TabIndex = 18;
      this.importButton.Text = "Import";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // exportButton
      // 
      this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.exportButton.Location = new System.Drawing.Point(1, -1);
      this.exportButton.Name = "exportButton";
      this.exportButton.Size = new System.Drawing.Size(24, 24);
      this.exportButton.TabIndex = 19;
      this.exportButton.Text = "Export";
      this.exportButton.UseVisualStyleBackColor = true;
      this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
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
      // problemInstanceProviderSplitContainer
      // 
      this.problemInstanceProviderSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.problemInstanceProviderSplitContainer.IsSplitterFixed = true;
      this.problemInstanceProviderSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.problemInstanceProviderSplitContainer.Name = "problemInstanceProviderSplitContainer";
      // 
      // problemInstanceProviderSplitContainer.Panel1
      // 
      this.problemInstanceProviderSplitContainer.Panel1.Controls.Add(this.problemInstanceProviderComboBox);
      this.problemInstanceProviderSplitContainer.Panel1.Controls.Add(this.importButton);
      this.problemInstanceProviderSplitContainer.Panel1.Controls.Add(this.libraryLabel);
      this.problemInstanceProviderSplitContainer.Panel1.Controls.Add(this.libraryInfoButton);
      // 
      // problemInstanceProviderSplitContainer.Panel2
      // 
      this.problemInstanceProviderSplitContainer.Panel2.Controls.Add(this.exportButton);
      this.problemInstanceProviderSplitContainer.Size = new System.Drawing.Size(312, 22);
      this.problemInstanceProviderSplitContainer.SplitterDistance = 280;
      this.problemInstanceProviderSplitContainer.TabIndex = 20;
      // 
      // ProblemInstanceConsumerViewGeneric
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.problemInstanceProviderSplitContainer);
      this.Controls.Add(this.problemInstanceProviderViewHost);
      this.Name = "ProblemInstanceConsumerViewGeneric";
      this.problemInstanceProviderSplitContainer.Panel1.ResumeLayout(false);
      this.problemInstanceProviderSplitContainer.Panel1.PerformLayout();
      this.problemInstanceProviderSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceProviderSplitContainer)).EndInit();
      this.problemInstanceProviderSplitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.ComboBox problemInstanceProviderComboBox;
    private System.Windows.Forms.Button libraryInfoButton;
    private System.Windows.Forms.Label libraryLabel;
    protected MainForm.WindowsForms.ViewHost problemInstanceProviderViewHost;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Button importButton;
    private System.Windows.Forms.Button exportButton;
    protected System.Windows.Forms.SaveFileDialog saveFileDialog;
    protected System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.SplitContainer problemInstanceProviderSplitContainer;
  }
}
