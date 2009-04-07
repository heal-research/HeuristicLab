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
namespace HeuristicLab.GP {
  partial class FunctionTreeView {
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
      if(selectedVariable != null) {
        selectedVariable.Value.Changed -= new EventHandler(selectedVariable_ValueChanged);
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
      this.funTreeView = new System.Windows.Forms.TreeView();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.variablesGroupBox = new System.Windows.Forms.GroupBox();
      this.variablesSplitContainer = new System.Windows.Forms.SplitContainer();
      this.variablesListBox = new System.Windows.Forms.ListBox();
      this.label1 = new System.Windows.Forms.Label();
      this.templateTextBox = new System.Windows.Forms.TextBox();
      this.editButton = new System.Windows.Forms.Button();
      this.treeNodeContextMenu = new System.Windows.Forms.ContextMenu();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.variablesGroupBox.SuspendLayout();
      this.variablesSplitContainer.Panel1.SuspendLayout();
      this.variablesSplitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // funTreeView
      // 
      this.funTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.funTreeView.HideSelection = false;
      this.funTreeView.Location = new System.Drawing.Point(0, 0);
      this.funTreeView.Name = "funTreeView";
      this.funTreeView.Size = new System.Drawing.Size(182, 532);
      this.funTreeView.TabIndex = 0;
      this.funTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.funTreeView_MouseUp);
      this.funTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.functionTreeView_AfterSelect);
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.funTreeView);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.variablesGroupBox);
      this.splitContainer.Panel2.Controls.Add(this.label1);
      this.splitContainer.Panel2.Controls.Add(this.templateTextBox);
      this.splitContainer.Panel2.Controls.Add(this.editButton);
      this.splitContainer.Size = new System.Drawing.Size(735, 532);
      this.splitContainer.SplitterDistance = 182;
      this.splitContainer.TabIndex = 1;
      // 
      // variablesGroupBox
      // 
      this.variablesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.variablesGroupBox.Controls.Add(this.variablesSplitContainer);
      this.variablesGroupBox.Location = new System.Drawing.Point(3, 31);
      this.variablesGroupBox.Name = "variablesGroupBox";
      this.variablesGroupBox.Size = new System.Drawing.Size(543, 498);
      this.variablesGroupBox.TabIndex = 5;
      this.variablesGroupBox.TabStop = false;
      this.variablesGroupBox.Text = "Local variables";
      // 
      // variablesSplitContainer
      // 
      this.variablesSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variablesSplitContainer.Location = new System.Drawing.Point(3, 16);
      this.variablesSplitContainer.Name = "variablesSplitContainer";
      // 
      // variablesSplitContainer.Panel1
      // 
      this.variablesSplitContainer.Panel1.Controls.Add(this.variablesListBox);
      this.variablesSplitContainer.Size = new System.Drawing.Size(537, 479);
      this.variablesSplitContainer.SplitterDistance = 179;
      this.variablesSplitContainer.TabIndex = 0;
      // 
      // variablesListBox
      // 
      this.variablesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variablesListBox.FormattingEnabled = true;
      this.variablesListBox.Location = new System.Drawing.Point(0, 0);
      this.variablesListBox.Name = "variablesListBox";
      this.variablesListBox.Size = new System.Drawing.Size(179, 472);
      this.variablesListBox.TabIndex = 0;
      this.variablesListBox.SelectedIndexChanged += new System.EventHandler(this.variablesListBox_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 10);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(96, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Function definition:";
      // 
      // templateTextBox
      // 
      this.templateTextBox.Location = new System.Drawing.Point(105, 7);
      this.templateTextBox.Name = "templateTextBox";
      this.templateTextBox.ReadOnly = true;
      this.templateTextBox.Size = new System.Drawing.Size(190, 20);
      this.templateTextBox.TabIndex = 2;
      // 
      // editButton
      // 
      this.editButton.Enabled = false;
      this.editButton.Location = new System.Drawing.Point(301, 5);
      this.editButton.Name = "editButton";
      this.editButton.Size = new System.Drawing.Size(49, 23);
      this.editButton.TabIndex = 1;
      this.editButton.Text = "Edit...";
      this.editButton.UseVisualStyleBackColor = true;
      this.editButton.Click += new System.EventHandler(this.editButton_Click);
      // 
      // treeNodeContextMenu
      // 
      this.treeNodeContextMenu.Name = "treeNodeContextMenu";
      // 
      // FunctionTreeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "FunctionTreeView";
      this.Size = new System.Drawing.Size(735, 532);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      this.splitContainer.ResumeLayout(false);
      this.variablesGroupBox.ResumeLayout(false);
      this.variablesSplitContainer.Panel1.ResumeLayout(false);
      this.variablesSplitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView funTreeView;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.TextBox templateTextBox;
    private System.Windows.Forms.Button editButton;
    private System.Windows.Forms.GroupBox variablesGroupBox;
    private System.Windows.Forms.SplitContainer variablesSplitContainer;
    private System.Windows.Forms.ListBox variablesListBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ContextMenu treeNodeContextMenu;
  }
}
