#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Windows.Forms;

namespace HeuristicLab.Core.Views {
  partial class OperatorBaseVariableInfosView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      foreach (ListViewItem item in variableInfosListView.Items) {
        ((IVariableInfo)item.Tag).ActualNameChanged -= new EventHandler(VariableInfo_ActualNameChanged);
      }
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.variableInfosGroupBox = new System.Windows.Forms.GroupBox();
      this.variableInfosListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.variableInfoDetailsGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.variableInfosGroupBox.SuspendLayout();
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
      this.splitContainer.Panel1.Controls.Add(this.variableInfosGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.variableInfoDetailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(423, 333);
      this.splitContainer.SplitterDistance = 203;
      this.splitContainer.TabIndex = 0;
      // 
      // variableInfosGroupBox
      // 
      this.variableInfosGroupBox.Controls.Add(this.variableInfosListView);
      this.variableInfosGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableInfosGroupBox.Location = new System.Drawing.Point(0, 0);
      this.variableInfosGroupBox.Name = "variableInfosGroupBox";
      this.variableInfosGroupBox.Size = new System.Drawing.Size(203, 333);
      this.variableInfosGroupBox.TabIndex = 0;
      this.variableInfosGroupBox.TabStop = false;
      this.variableInfosGroupBox.Text = "&Variable Infos";
      // 
      // variableInfosListView
      // 
      this.variableInfosListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.variableInfosListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableInfosListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.variableInfosListView.HideSelection = false;
      this.variableInfosListView.Location = new System.Drawing.Point(3, 16);
      this.variableInfosListView.Name = "variableInfosListView";
      this.variableInfosListView.Size = new System.Drawing.Size(197, 314);
      this.variableInfosListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.variableInfosListView.TabIndex = 0;
      this.variableInfosListView.UseCompatibleStateImageBehavior = false;
      this.variableInfosListView.View = System.Windows.Forms.View.Details;
      this.variableInfosListView.SelectedIndexChanged += new System.EventHandler(this.variableInfosListView_SelectedIndexChanged);
      this.variableInfosListView.SizeChanged += new System.EventHandler(this.variableInfosListView_SizeChanged);
      // 
      // variableInfoDetailsGroupBox
      // 
      this.variableInfoDetailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableInfoDetailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.variableInfoDetailsGroupBox.Name = "variableInfoDetailsGroupBox";
      this.variableInfoDetailsGroupBox.Size = new System.Drawing.Size(216, 333);
      this.variableInfoDetailsGroupBox.TabIndex = 0;
      this.variableInfoDetailsGroupBox.TabStop = false;
      this.variableInfoDetailsGroupBox.Text = "&Details";
      // 
      // OperatorBaseVariableInfosView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "OperatorBaseVariableInfosView";
      this.Size = new System.Drawing.Size(423, 333);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.variableInfosGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ColumnHeader columnHeader1;
    protected SplitContainer splitContainer;
    protected GroupBox variableInfosGroupBox;
    protected ListView variableInfosListView;
    protected GroupBox variableInfoDetailsGroupBox;


  }
}
