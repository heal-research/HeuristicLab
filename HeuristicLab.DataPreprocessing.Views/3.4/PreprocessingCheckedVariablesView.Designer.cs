#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class PreprocessingCheckedVariablesView {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.checkedItemList = new HeuristicLab.DataPreprocessing.Views.PreprocessingCheckedItemListView();
      this.variablesListcontextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.checkInputsTargetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.checkOnlyInputsTargetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.checkAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.uncheckAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.variablesListcontextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.checkedItemList);
      this.splitContainer.Size = new System.Drawing.Size(654, 403);
      this.splitContainer.SplitterDistance = 91;
      this.splitContainer.TabIndex = 7;
      // 
      // checkedItemList
      // 
      this.checkedItemList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.checkedItemList.Caption = "View";
      this.checkedItemList.Content = null;
      this.checkedItemList.ContextMenuStrip = this.variablesListcontextMenuStrip;
      this.checkedItemList.Location = new System.Drawing.Point(4, 4);
      this.checkedItemList.Name = "checkedItemList";
      this.checkedItemList.ReadOnly = false;
      this.checkedItemList.Size = new System.Drawing.Size(84, 252);
      this.checkedItemList.TabIndex = 4;
      // 
      // variablesListcontextMenuStrip
      // 
      this.variablesListcontextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkInputsTargetToolStripMenuItem,
            this.checkOnlyInputsTargetToolStripMenuItem,
            this.checkAllToolStripMenuItem,
            this.uncheckAllToolStripMenuItem});
      this.variablesListcontextMenuStrip.Name = "variablesListcontextMenuStrip";
      this.variablesListcontextMenuStrip.Size = new System.Drawing.Size(211, 92);
      this.variablesListcontextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.variablesListcontextMenuStrip_Opening);
      // 
      // checkInputsTargetToolStripMenuItem
      // 
      this.checkInputsTargetToolStripMenuItem.Name = "checkInputsTargetToolStripMenuItem";
      this.checkInputsTargetToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
      this.checkInputsTargetToolStripMenuItem.Text = "Check Inputs+Target";
      this.checkInputsTargetToolStripMenuItem.Click += new System.EventHandler(this.checkInputsTargetToolStripMenuItem_Click);
      // 
      // checkOnlyInputsTargetToolStripMenuItem
      // 
      this.checkOnlyInputsTargetToolStripMenuItem.Name = "checkOnlyInputsTargetToolStripMenuItem";
      this.checkOnlyInputsTargetToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
      this.checkOnlyInputsTargetToolStripMenuItem.Text = "Check only Inputs+Target";
      this.checkOnlyInputsTargetToolStripMenuItem.Click += new System.EventHandler(this.checkOnlyInputsTargetToolStripMenuItem_Click);
      // 
      // checkAllToolStripMenuItem
      // 
      this.checkAllToolStripMenuItem.Name = "checkAllToolStripMenuItem";
      this.checkAllToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
      this.checkAllToolStripMenuItem.Text = "Check All";
      this.checkAllToolStripMenuItem.Click += new System.EventHandler(this.checkAllToolStripMenuItem_Click);
      // 
      // uncheckAllToolStripMenuItem
      // 
      this.uncheckAllToolStripMenuItem.Name = "uncheckAllToolStripMenuItem";
      this.uncheckAllToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
      this.uncheckAllToolStripMenuItem.Text = "Uncheck All";
      this.uncheckAllToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllToolStripMenuItem_Click);
      // 
      // PreprocessingCheckedVariablesView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "PreprocessingCheckedVariablesView";
      this.Size = new System.Drawing.Size(654, 403);
      this.splitContainer.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.variablesListcontextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected PreprocessingCheckedItemListView checkedItemList;
    protected System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.ContextMenuStrip variablesListcontextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem checkInputsTargetToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem checkAllToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem uncheckAllToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem checkOnlyInputsTargetToolStripMenuItem;
  }
}
