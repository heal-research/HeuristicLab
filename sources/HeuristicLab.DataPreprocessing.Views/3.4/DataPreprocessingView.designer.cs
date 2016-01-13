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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataPreprocessingView));
      this.undoButton = new System.Windows.Forms.Button();
      this.applyInNewTabButton = new System.Windows.Forms.Button();
      this.exportProblemButton = new System.Windows.Forms.Button();
      this.lblFilterActive = new System.Windows.Forms.Label();
      this.applyComboBox = new System.Windows.Forms.ComboBox();
      this.exportLabel = new System.Windows.Forms.Label();
      this.redoButton = new System.Windows.Forms.Button();
      this.viewShortcutListView = new HeuristicLab.DataPreprocessing.Views.ViewShortcutListView();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
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
      this.undoButton.Location = new System.Drawing.Point(6, 56);
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
      this.applyInNewTabButton.Location = new System.Drawing.Point(57, 26);
      this.applyInNewTabButton.Name = "applyInNewTabButton";
      this.applyInNewTabButton.Size = new System.Drawing.Size(24, 24);
      this.applyInNewTabButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.applyInNewTabButton, "Apply in new Tab");
      this.applyInNewTabButton.UseVisualStyleBackColor = true;
      this.applyInNewTabButton.Click += new System.EventHandler(this.applyInNewTabButton_Click);
      // 
      // exportProblemButton
      // 
      this.exportProblemButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
      this.exportProblemButton.Location = new System.Drawing.Point(87, 26);
      this.exportProblemButton.Name = "exportProblemButton";
      this.exportProblemButton.Size = new System.Drawing.Size(24, 24);
      this.exportProblemButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.exportProblemButton, "Save");
      this.exportProblemButton.UseVisualStyleBackColor = true;
      this.exportProblemButton.Click += new System.EventHandler(this.exportProblemButton_Click);
      // 
      // lblFilterActive
      // 
      this.lblFilterActive.AutoSize = true;
      this.lblFilterActive.Location = new System.Drawing.Point(84, 62);
      this.lblFilterActive.Name = "lblFilterActive";
      this.lblFilterActive.Size = new System.Drawing.Size(277, 13);
      this.lblFilterActive.TabIndex = 5;
      this.lblFilterActive.Text = "Attention! The data is read-only, because a filter is active.";
      this.lblFilterActive.Visible = false;
      // 
      // applyComboBox
      // 
      this.applyComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.applyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.applyComboBox.FormattingEnabled = true;
      this.applyComboBox.Items.AddRange(new object[] {
            "Algorithm",
            "Problem",
            "ProblemData"});
      this.applyComboBox.Location = new System.Drawing.Point(117, 28);
      this.applyComboBox.Name = "applyComboBox";
      this.applyComboBox.Size = new System.Drawing.Size(696, 21);
      this.applyComboBox.TabIndex = 4;
      // 
      // exportLabel
      // 
      this.exportLabel.AutoSize = true;
      this.exportLabel.Location = new System.Drawing.Point(5, 31);
      this.exportLabel.Name = "exportLabel";
      this.exportLabel.Size = new System.Drawing.Size(40, 13);
      this.exportLabel.TabIndex = 8;
      this.exportLabel.Text = "Export:";
      // 
      // redoButton
      // 
      this.redoButton.Enabled = false;
      this.redoButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Redo;
      this.redoButton.Location = new System.Drawing.Point(36, 56);
      this.redoButton.Name = "redoButton";
      this.redoButton.Size = new System.Drawing.Size(24, 24);
      this.redoButton.TabIndex = 6;
      this.toolTip.SetToolTip(this.redoButton, "Redo (not implemented yet)");
      this.redoButton.UseVisualStyleBackColor = true;
      // 
      // viewShortcutListView
      // 
      this.viewShortcutListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.viewShortcutListView.Caption = "ViewShortcutCollection View";
      this.viewShortcutListView.Content = null;
      this.viewShortcutListView.Location = new System.Drawing.Point(4, 86);
      this.viewShortcutListView.Name = "viewShortcutListView";
      this.viewShortcutListView.ReadOnly = false;
      this.viewShortcutListView.Size = new System.Drawing.Size(831, 360);
      this.viewShortcutListView.TabIndex = 7;
      // 
      // DataPreprocessingView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.applyComboBox);
      this.Controls.Add(this.exportLabel);
      this.Controls.Add(this.exportProblemButton);
      this.Controls.Add(this.applyInNewTabButton);
      this.Controls.Add(this.lblFilterActive);
      this.Controls.Add(this.viewShortcutListView);
      this.Controls.Add(this.redoButton);
      this.Controls.Add(this.undoButton);
      this.Name = "DataPreprocessingView";
      this.Size = new System.Drawing.Size(838, 449);
      this.Controls.SetChildIndex(this.undoButton, 0);
      this.Controls.SetChildIndex(this.redoButton, 0);
      this.Controls.SetChildIndex(this.viewShortcutListView, 0);
      this.Controls.SetChildIndex(this.lblFilterActive, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.applyInNewTabButton, 0);
      this.Controls.SetChildIndex(this.exportProblemButton, 0);
      this.Controls.SetChildIndex(this.exportLabel, 0);
      this.Controls.SetChildIndex(this.applyComboBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button exportProblemButton;
    private System.Windows.Forms.Button applyInNewTabButton;
    private System.Windows.Forms.Button undoButton;
    private ViewShortcutListView viewShortcutListView;
    private System.Windows.Forms.Label lblFilterActive;
    private System.Windows.Forms.Label exportLabel;
    private System.Windows.Forms.Button redoButton;
    private System.Windows.Forms.ComboBox applyComboBox;
  }
}
