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
      this.ProviderExportSplitContainer = new System.Windows.Forms.SplitContainer();
      this.ProviderImportSplitContainer = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(this.ProviderExportSplitContainer)).BeginInit();
      this.ProviderExportSplitContainer.Panel1.SuspendLayout();
      this.ProviderExportSplitContainer.Panel2.SuspendLayout();
      this.ProviderExportSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ProviderImportSplitContainer)).BeginInit();
      this.ProviderImportSplitContainer.Panel1.SuspendLayout();
      this.ProviderImportSplitContainer.Panel2.SuspendLayout();
      this.ProviderImportSplitContainer.SuspendLayout();
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
      this.problemInstanceProviderComboBox.Size = new System.Drawing.Size(174, 21);
      this.problemInstanceProviderComboBox.TabIndex = 16;
      this.problemInstanceProviderComboBox.SelectedIndexChanged += new System.EventHandler(this.problemInstanceProviderComboBox_SelectedIndexChanged);
      // 
      // libraryInfoButton
      // 
      this.libraryInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.libraryInfoButton.Location = new System.Drawing.Point(230, -1);
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
      this.libraryLabel.Location = new System.Drawing.Point(3, 5);
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
      this.importButton.Location = new System.Drawing.Point(1, -1);
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
      // ProviderExportSplitContainer
      // 
      this.ProviderExportSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.ProviderExportSplitContainer.IsSplitterFixed = true;
      this.ProviderExportSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.ProviderExportSplitContainer.Name = "ProviderExportSplitContainer";
      // 
      // ProviderExportSplitContainer.Panel1
      // 
      this.ProviderExportSplitContainer.Panel1.Controls.Add(this.ProviderImportSplitContainer);
      // 
      // ProviderExportSplitContainer.Panel2
      // 
      this.ProviderExportSplitContainer.Panel2.Controls.Add(this.exportButton);
      this.ProviderExportSplitContainer.Size = new System.Drawing.Size(312, 22);
      this.ProviderExportSplitContainer.SplitterDistance = 283;
      this.ProviderExportSplitContainer.TabIndex = 20;
      // 
      // ProviderImportSplitContainer
      // 
      this.ProviderImportSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ProviderImportSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.ProviderImportSplitContainer.IsSplitterFixed = true;
      this.ProviderImportSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.ProviderImportSplitContainer.Name = "ProviderImportSplitContainer";
      // 
      // ProviderImportSplitContainer.Panel1
      // 
      this.ProviderImportSplitContainer.Panel1.Controls.Add(this.libraryLabel);
      this.ProviderImportSplitContainer.Panel1.Controls.Add(this.problemInstanceProviderComboBox);
      this.ProviderImportSplitContainer.Panel1.Controls.Add(this.libraryInfoButton);
      // 
      // ProviderImportSplitContainer.Panel2
      // 
      this.ProviderImportSplitContainer.Panel2.Controls.Add(this.importButton);
      this.ProviderImportSplitContainer.Size = new System.Drawing.Size(282, 23);
      this.ProviderImportSplitContainer.SplitterDistance = 253;
      this.ProviderImportSplitContainer.TabIndex = 21;
      // 
      // ProblemInstanceConsumerViewGeneric
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.ProviderExportSplitContainer);
      this.Controls.Add(this.problemInstanceProviderViewHost);
      this.Name = "ProblemInstanceConsumerViewGeneric";
      this.ProviderExportSplitContainer.Panel1.ResumeLayout(false);
      this.ProviderExportSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ProviderExportSplitContainer)).EndInit();
      this.ProviderExportSplitContainer.ResumeLayout(false);
      this.ProviderImportSplitContainer.Panel1.ResumeLayout(false);
      this.ProviderImportSplitContainer.Panel1.PerformLayout();
      this.ProviderImportSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ProviderImportSplitContainer)).EndInit();
      this.ProviderImportSplitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.ComboBox problemInstanceProviderComboBox;
    protected System.Windows.Forms.Button libraryInfoButton;
    protected System.Windows.Forms.Label libraryLabel;
    protected MainForm.WindowsForms.ViewHost problemInstanceProviderViewHost;
    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.Button importButton;
    protected System.Windows.Forms.Button exportButton;
    protected System.Windows.Forms.SaveFileDialog saveFileDialog;
    protected System.Windows.Forms.OpenFileDialog openFileDialog;
    protected System.Windows.Forms.SplitContainer ProviderExportSplitContainer;
    protected System.Windows.Forms.SplitContainer ProviderImportSplitContainer;
  }
}
