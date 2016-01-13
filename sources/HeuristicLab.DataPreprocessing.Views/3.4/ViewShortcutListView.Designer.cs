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
  partial class ViewShortcutListView {
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
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Size = new System.Drawing.Size(526, 364);
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Size = new System.Drawing.Size(532, 383);
      // 
      // itemsListView
      // 
      this.itemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.itemsListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.itemsListView.Location = new System.Drawing.Point(0, 0);
      this.itemsListView.Size = new System.Drawing.Size(200, 364);
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.detailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Size = new System.Drawing.Size(322, 364);
      // 
      // addButton
      // 
      this.toolTip.SetToolTip(this.addButton, "Add");
      this.addButton.Visible = false;
      // 
      // removeButton
      // 
      this.toolTip.SetToolTip(this.removeButton, "Remove");
      this.removeButton.Visible = false;
      // 
      // moveUpButton
      // 
      this.toolTip.SetToolTip(this.moveUpButton, "Move Up");
      this.moveUpButton.Visible = false;
      // 
      // moveDownButton
      // 
      this.toolTip.SetToolTip(this.moveDownButton, "Move Down");
      this.moveDownButton.Visible = false;
      // 
      // viewHost
      // 
      this.viewHost.Size = new System.Drawing.Size(310, 339);
      // 
      // showDetailsCheckBox
      // 
      this.showDetailsCheckBox.Visible = false;
      // 
      // ViewShortcutListView
      // 
      this.Name = "ViewShortcutListView";
      this.Size = new System.Drawing.Size(532, 383);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
  }
}
