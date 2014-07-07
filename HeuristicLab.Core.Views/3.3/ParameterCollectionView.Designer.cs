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

using System.Windows.Forms;
namespace HeuristicLab.Core.Views {
  partial class ParameterCollectionView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      this.showHiddenParametersCheckBox = new System.Windows.Forms.CheckBox();
      this.itemsListViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.showHideParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.itemsListViewContextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.showHiddenParametersCheckBox);
      // 
      // itemsListView
      // 
      this.itemsListView.ContextMenuStrip = this.itemsListViewContextMenuStrip;
      this.itemsListView.TabIndex = 6;
      // 
      // showHiddenParametersCheckBox
      // 
      this.showHiddenParametersCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showHiddenParametersCheckBox.Checked = false;
      this.showHiddenParametersCheckBox.CheckState = System.Windows.Forms.CheckState.Unchecked;
      this.showHiddenParametersCheckBox.Image = HeuristicLab.Common.Resources.VSImageLibrary.HiddenField;
      this.showHiddenParametersCheckBox.Location = new System.Drawing.Point(153, 3);
      this.showHiddenParametersCheckBox.Name = "showHiddenParametersCheckBox";
      this.showHiddenParametersCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showHiddenParametersCheckBox.TabIndex = 5;
      this.toolTip.SetToolTip(this.showHiddenParametersCheckBox, "Show Hidden Parameters");
      this.showHiddenParametersCheckBox.UseVisualStyleBackColor = true;
      this.showHiddenParametersCheckBox.CheckedChanged += new System.EventHandler(showHiddenParametersCheckBox_CheckedChanged);
      // 
      // itemsListViewContextMenuStrip
      // 
      this.itemsListViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHideParametersToolStripMenuItem});
      this.itemsListViewContextMenuStrip.Name = "itemsListViewContextMenuStrip";
      this.itemsListViewContextMenuStrip.Size = new System.Drawing.Size(161, 26);
      this.itemsListViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(itemsListViewContextMenuStrip_Opening);
      // 
      // showHideParametersToolStripMenuItem
      // 
      this.showHideParametersToolStripMenuItem.Name = "showHideParametersToolStripMenuItem";
      this.showHideParametersToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
      this.showHideParametersToolStripMenuItem.Text = "Show/Hide Parameters";
      this.showHideParametersToolStripMenuItem.Click += new System.EventHandler(showHideParametersToolStripMenuItem_Click);
      // 
      // ParameterCollectionView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Name = "ParameterCollectionView";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.itemsListViewContextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    #endregion

    protected CheckBox showHiddenParametersCheckBox;
    protected ContextMenuStrip itemsListViewContextMenuStrip;
    protected ToolStripMenuItem showHideParametersToolStripMenuItem;
  }
}
