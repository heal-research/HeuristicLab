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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class DataPreprocessingView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataPreprocessingView));
      this.undoButton = new System.Windows.Forms.Button();
      this.applyInNewTabButton = new System.Windows.Forms.Button();
      this.exportProblemButton = new System.Windows.Forms.Button();
      this.lblFilterActive = new System.Windows.Forms.Label();
      this.redoButton = new System.Windows.Forms.Button();
      this.newButton = new System.Windows.Forms.Button();
      this.importButton = new System.Windows.Forms.Button();
      this.newProblemDataTypeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.newRegressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.newClassificationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.newTimeSeriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.importProblemDataTypeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.importRegressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.importClassificationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.importTimeSeriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewShortcutListView = new HeuristicLab.DataPreprocessing.Views.ViewShortcutListView();
      this.applyTypeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.exportTypeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.newProblemDataTypeContextMenuStrip.SuspendLayout();
      this.importProblemDataTypeContextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(755, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(819, 3);
      // 
      // undoButton
      // 
      this.undoButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Undo;
      this.undoButton.Location = new System.Drawing.Point(131, 26);
      this.undoButton.Name = "undoButton";
      this.undoButton.Size = new System.Drawing.Size(24, 24);
      this.undoButton.TabIndex = 5;
      this.toolTip.SetToolTip(this.undoButton, "Undo");
      this.undoButton.UseVisualStyleBackColor = true;
      this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
      // 
      // applyInNewTabButton
      // 
      this.applyInNewTabButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.applyInNewTabButton.Location = new System.Drawing.Point(69, 26);
      this.applyInNewTabButton.Name = "applyInNewTabButton";
      this.applyInNewTabButton.Size = new System.Drawing.Size(24, 24);
      this.applyInNewTabButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.applyInNewTabButton, "Apply in new Tab");
      this.applyInNewTabButton.UseVisualStyleBackColor = true;
      this.applyInNewTabButton.Click += new System.EventHandler(this.applyInNewTabButton_Click);
      // 
      // exportProblemButton
      // 
      this.exportProblemButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.SaveAs;
      this.exportProblemButton.Location = new System.Drawing.Point(99, 26);
      this.exportProblemButton.Name = "exportProblemButton";
      this.exportProblemButton.Size = new System.Drawing.Size(24, 24);
      this.exportProblemButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.exportProblemButton, "Export to File");
      this.exportProblemButton.UseVisualStyleBackColor = true;
      this.exportProblemButton.Click += new System.EventHandler(this.exportProblemButton_Click);
      // 
      // lblFilterActive
      // 
      this.lblFilterActive.AutoSize = true;
      this.lblFilterActive.Location = new System.Drawing.Point(203, 31);
      this.lblFilterActive.Name = "lblFilterActive";
      this.lblFilterActive.Size = new System.Drawing.Size(277, 13);
      this.lblFilterActive.TabIndex = 5;
      this.lblFilterActive.Text = "Attention! The data is read-only, because a filter is active.";
      this.lblFilterActive.Visible = false;
      // 
      // redoButton
      // 
      this.redoButton.Enabled = false;
      this.redoButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Redo;
      this.redoButton.Location = new System.Drawing.Point(161, 26);
      this.redoButton.Name = "redoButton";
      this.redoButton.Size = new System.Drawing.Size(24, 24);
      this.redoButton.TabIndex = 6;
      this.toolTip.SetToolTip(this.redoButton, "Redo (not implemented yet)");
      this.redoButton.UseVisualStyleBackColor = true;
      // 
      // newButton
      // 
      this.newButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.NewDocument;
      this.newButton.Location = new System.Drawing.Point(6, 26);
      this.newButton.Name = "newButton";
      this.newButton.Size = new System.Drawing.Size(24, 24);
      this.newButton.TabIndex = 3;
      this.newButton.UseVisualStyleBackColor = true;
      this.newButton.Click += new System.EventHandler(this.newButton_Click);
      // 
      // importButton
      // 
      this.importButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.importButton.Location = new System.Drawing.Point(36, 26);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(24, 24);
      this.importButton.TabIndex = 3;
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // newProblemDataTypeContextMenuStrip
      // 
      this.newProblemDataTypeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newRegressionToolStripMenuItem,
            this.newClassificationToolStripMenuItem,
            this.newTimeSeriesToolStripMenuItem});
      this.newProblemDataTypeContextMenuStrip.Name = "newProblemDataTypeContextMenuStrip";
      this.newProblemDataTypeContextMenuStrip.Size = new System.Drawing.Size(190, 70);
      // 
      // newRegressionToolStripMenuItem
      // 
      this.newRegressionToolStripMenuItem.Name = "newRegressionToolStripMenuItem";
      this.newRegressionToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
      this.newRegressionToolStripMenuItem.Text = "Regression";
      this.newRegressionToolStripMenuItem.Click += new System.EventHandler(this.newRegressionToolStripMenuItem_Click);
      // 
      // newClassificationToolStripMenuItem
      // 
      this.newClassificationToolStripMenuItem.Name = "newClassificationToolStripMenuItem";
      this.newClassificationToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
      this.newClassificationToolStripMenuItem.Text = "Classification";
      this.newClassificationToolStripMenuItem.Click += new System.EventHandler(this.newClassificationToolStripMenuItem_Click);
      // 
      // newTimeSeriesToolStripMenuItem
      // 
      this.newTimeSeriesToolStripMenuItem.Name = "newTimeSeriesToolStripMenuItem";
      this.newTimeSeriesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
      this.newTimeSeriesToolStripMenuItem.Text = "Time Series Prognosis";
      this.newTimeSeriesToolStripMenuItem.Click += new System.EventHandler(this.newTimeSeriesToolStripMenuItem_Click);
      // 
      // importProblemDataTypeContextMenuStrip
      // 
      this.importProblemDataTypeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importRegressionToolStripMenuItem,
            this.importClassificationToolStripMenuItem,
            this.importTimeSeriesToolStripMenuItem});
      this.importProblemDataTypeContextMenuStrip.Name = "newProblemDataTypeContextMenuStrip";
      this.importProblemDataTypeContextMenuStrip.Size = new System.Drawing.Size(190, 92);
      // 
      // importRegressionToolStripMenuItem
      // 
      this.importRegressionToolStripMenuItem.Name = "importRegressionToolStripMenuItem";
      this.importRegressionToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
      this.importRegressionToolStripMenuItem.Text = "Regression";
      this.importRegressionToolStripMenuItem.Click += new System.EventHandler(this.importRegressionToolStripMenuItem_Click);
      // 
      // importClassificationToolStripMenuItem
      // 
      this.importClassificationToolStripMenuItem.Name = "importClassificationToolStripMenuItem";
      this.importClassificationToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
      this.importClassificationToolStripMenuItem.Text = "Classification";
      this.importClassificationToolStripMenuItem.Click += new System.EventHandler(this.importClassificationToolStripMenuItem_Click);
      // 
      // importTimeSeriesToolStripMenuItem
      // 
      this.importTimeSeriesToolStripMenuItem.Name = "importTimeSeriesToolStripMenuItem";
      this.importTimeSeriesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
      this.importTimeSeriesToolStripMenuItem.Text = "Time Series Prognosis";
      this.importTimeSeriesToolStripMenuItem.Click += new System.EventHandler(this.importTimeSeriesToolStripMenuItem_Click);
      // 
      // viewShortcutListView
      // 
      this.viewShortcutListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.viewShortcutListView.Caption = "ViewShortcutCollection View";
      this.viewShortcutListView.Content = null;
      this.viewShortcutListView.Location = new System.Drawing.Point(4, 56);
      this.viewShortcutListView.Name = "viewShortcutListView";
      this.viewShortcutListView.ReadOnly = false;
      this.viewShortcutListView.Size = new System.Drawing.Size(831, 390);
      this.viewShortcutListView.TabIndex = 7;
      // 
      // applyTypeContextMenuStrip
      // 
      this.applyTypeContextMenuStrip.Name = "newProblemDataTypeContextMenuStrip";
      this.applyTypeContextMenuStrip.Size = new System.Drawing.Size(61, 4);
      // 
      // exportTypeContextMenuStrip
      // 
      this.exportTypeContextMenuStrip.Name = "newProblemDataTypeContextMenuStrip";
      this.exportTypeContextMenuStrip.Size = new System.Drawing.Size(61, 4);
      // 
      // DataPreprocessingView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.newButton);
      this.Controls.Add(this.importButton);
      this.Controls.Add(this.applyInNewTabButton);
      this.Controls.Add(this.exportProblemButton);
      this.Controls.Add(this.undoButton);
      this.Controls.Add(this.redoButton);
      this.Controls.Add(this.lblFilterActive);
      this.Controls.Add(this.viewShortcutListView);
      this.Name = "DataPreprocessingView";
      this.Size = new System.Drawing.Size(838, 449);
      this.Controls.SetChildIndex(this.viewShortcutListView, 0);
      this.Controls.SetChildIndex(this.lblFilterActive, 0);
      this.Controls.SetChildIndex(this.redoButton, 0);
      this.Controls.SetChildIndex(this.undoButton, 0);
      this.Controls.SetChildIndex(this.exportProblemButton, 0);
      this.Controls.SetChildIndex(this.applyInNewTabButton, 0);
      this.Controls.SetChildIndex(this.importButton, 0);
      this.Controls.SetChildIndex(this.newButton, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.newProblemDataTypeContextMenuStrip.ResumeLayout(false);
      this.importProblemDataTypeContextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button exportProblemButton;
    private System.Windows.Forms.Button applyInNewTabButton;
    private System.Windows.Forms.Button undoButton;
    private ViewShortcutListView viewShortcutListView;
    private System.Windows.Forms.Label lblFilterActive;
    private System.Windows.Forms.Button redoButton;
    private System.Windows.Forms.Button newButton;
    private System.Windows.Forms.Button importButton;
    private System.Windows.Forms.ContextMenuStrip newProblemDataTypeContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem newRegressionToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem newClassificationToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem newTimeSeriesToolStripMenuItem;
    private System.Windows.Forms.ContextMenuStrip importProblemDataTypeContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem importRegressionToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem importClassificationToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem importTimeSeriesToolStripMenuItem;
    private System.Windows.Forms.ContextMenuStrip applyTypeContextMenuStrip;
    private System.Windows.Forms.ContextMenuStrip exportTypeContextMenuStrip;
  }
}
