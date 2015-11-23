#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.lblFilterActive = new System.Windows.Forms.Label();
      this.viewShortcutListView = new HeuristicLab.DataPreprocessing.Views.ViewShortcutListView();
      this.SuspendLayout();
      // 
      // undoButton
      // 
      this.undoButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Undo;
      this.undoButton.Location = new System.Drawing.Point(64, 3);
      this.undoButton.Name = "undoButton";
      this.undoButton.Size = new System.Drawing.Size(24, 24);
      this.undoButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.undoButton, "Undo");
      this.undoButton.UseVisualStyleBackColor = true;
      this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
      // 
      // applyInNewTabButton
      // 
      this.applyInNewTabButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.applyInNewTabButton.Location = new System.Drawing.Point(34, 3);
      this.applyInNewTabButton.Name = "applyInNewTabButton";
      this.applyInNewTabButton.Size = new System.Drawing.Size(24, 24);
      this.applyInNewTabButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.applyInNewTabButton, "Apply in new tab");
      this.applyInNewTabButton.UseVisualStyleBackColor = true;
      this.applyInNewTabButton.Click += new System.EventHandler(this.applyInNewTabButton_Click);
      // 
      // exportProblemButton
      // 
      this.exportProblemButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
      this.exportProblemButton.Location = new System.Drawing.Point(4, 3);
      this.exportProblemButton.Name = "exportProblemButton";
      this.exportProblemButton.Size = new System.Drawing.Size(24, 24);
      this.exportProblemButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.exportProblemButton, "Export problem");
      this.exportProblemButton.UseVisualStyleBackColor = true;
      this.exportProblemButton.Click += new System.EventHandler(this.exportProblemButton_Click);
      // 
      // lblFilterActive
      // 
      this.lblFilterActive.AutoSize = true;
      this.lblFilterActive.Location = new System.Drawing.Point(220, 12);
      this.lblFilterActive.Name = "lblFilterActive";
      this.lblFilterActive.Size = new System.Drawing.Size(277, 13);
      this.lblFilterActive.TabIndex = 5;
      this.lblFilterActive.Text = "Attention! The data is read-only, because a filter is active.";
      this.lblFilterActive.Visible = false;
      // 
      // viewShortcutListView
      // 
      this.viewShortcutListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.viewShortcutListView.Caption = "ViewShortcutCollection View";
      this.viewShortcutListView.Content = null;
      this.viewShortcutListView.Location = new System.Drawing.Point(4, 33);
      this.viewShortcutListView.Name = "viewShortcutListView";
      this.viewShortcutListView.ReadOnly = false;
      this.viewShortcutListView.Size = new System.Drawing.Size(831, 413);
      this.viewShortcutListView.TabIndex = 4;
      // 
      // DataPreprocessingView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lblFilterActive);
      this.Controls.Add(this.viewShortcutListView);
      this.Controls.Add(this.undoButton);
      this.Controls.Add(this.applyInNewTabButton);
      this.Controls.Add(this.exportProblemButton);
      this.Name = "DataPreprocessingView";
      this.Size = new System.Drawing.Size(838, 449);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button exportProblemButton;
    private System.Windows.Forms.Button applyInNewTabButton;
    private System.Windows.Forms.Button undoButton;
    private System.Windows.Forms.ToolTip toolTip;
    private ViewShortcutListView viewShortcutListView;
    private System.Windows.Forms.Label lblFilterActive;

  }
}
