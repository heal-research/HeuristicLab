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

namespace HeuristicLab.Core {
  partial class OperatorLibraryEditor {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (chooseOperatorDialog != null) chooseOperatorDialog.Dispose();
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
      this.operatorsGroupBox = new System.Windows.Forms.GroupBox();
      this.operatorsTreeView = new System.Windows.Forms.TreeView();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.removeButton = new System.Windows.Forms.Button();
      this.addGroupButton = new System.Windows.Forms.Button();
      this.addOperatorButton = new System.Windows.Forms.Button();
      this.importButton = new System.Windows.Forms.Button();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.operatorsGroupBox.SuspendLayout();
      this.contextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // operatorsGroupBox
      // 
      this.operatorsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorsGroupBox.Controls.Add(this.operatorsTreeView);
      this.operatorsGroupBox.Enabled = false;
      this.operatorsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.operatorsGroupBox.Name = "operatorsGroupBox";
      this.operatorsGroupBox.Size = new System.Drawing.Size(522, 366);
      this.operatorsGroupBox.TabIndex = 0;
      this.operatorsGroupBox.TabStop = false;
      this.operatorsGroupBox.Text = "&Operators";
      // 
      // operatorsTreeView
      // 
      this.operatorsTreeView.AllowDrop = true;
      this.operatorsTreeView.ContextMenuStrip = this.contextMenuStrip;
      this.operatorsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorsTreeView.HideSelection = false;
      this.operatorsTreeView.LabelEdit = true;
      this.operatorsTreeView.Location = new System.Drawing.Point(3, 16);
      this.operatorsTreeView.Name = "operatorsTreeView";
      this.operatorsTreeView.ShowNodeToolTips = true;
      this.operatorsTreeView.Size = new System.Drawing.Size(516, 347);
      this.operatorsTreeView.TabIndex = 0;
      this.operatorsTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.operatorsTreeView_AfterLabelEdit);
      this.operatorsTreeView.DoubleClick += new System.EventHandler(this.operatorsTreeView_DoubleClick);
      this.operatorsTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.operatorsTreeView_DragDrop);
      this.operatorsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.operatorsTreeView_AfterSelect);
      this.operatorsTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operatorsTreeView_MouseDown);
      this.operatorsTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.operatorsTreeView_DragEnter);
      this.operatorsTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.operatorsTreeView_KeyDown);
      this.operatorsTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.operatorsTreeView_ItemDrag);
      this.operatorsTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.operatorsTreeView_DragOver);
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(120, 26);
      this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
      // 
      // viewToolStripMenuItem
      // 
      this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      this.viewToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
      this.viewToolStripMenuItem.Text = "&View...";
      this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
      // 
      // removeButton
      // 
      this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeButton.Enabled = false;
      this.removeButton.Location = new System.Drawing.Point(220, 372);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(104, 23);
      this.removeButton.TabIndex = 3;
      this.removeButton.Text = "&Remove";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addGroupButton
      // 
      this.addGroupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addGroupButton.Enabled = false;
      this.addGroupButton.Location = new System.Drawing.Point(0, 372);
      this.addGroupButton.Name = "addGroupButton";
      this.addGroupButton.Size = new System.Drawing.Size(104, 23);
      this.addGroupButton.TabIndex = 1;
      this.addGroupButton.Text = "&Add &Group...";
      this.addGroupButton.UseVisualStyleBackColor = true;
      this.addGroupButton.Click += new System.EventHandler(this.addGroupButton_Click);
      // 
      // addOperatorButton
      // 
      this.addOperatorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addOperatorButton.Enabled = false;
      this.addOperatorButton.Location = new System.Drawing.Point(110, 372);
      this.addOperatorButton.Name = "addOperatorButton";
      this.addOperatorButton.Size = new System.Drawing.Size(104, 23);
      this.addOperatorButton.TabIndex = 2;
      this.addOperatorButton.Text = "Add &Operator...";
      this.addOperatorButton.UseVisualStyleBackColor = true;
      this.addOperatorButton.Click += new System.EventHandler(this.addOperatorButton_Click);
      // 
      // importButton
      // 
      this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.importButton.Location = new System.Drawing.Point(330, 372);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(104, 23);
      this.importButton.TabIndex = 4;
      this.importButton.Text = "&Import...";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "item";
      this.openFileDialog.Filter = "HeuristicLab files|*.hl|All files|*.*";
      this.openFileDialog.Title = "Import ...";
      // 
      // OperatorLibraryEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.importButton);
      this.Controls.Add(this.operatorsGroupBox);
      this.Controls.Add(this.addGroupButton);
      this.Controls.Add(this.removeButton);
      this.Controls.Add(this.addOperatorButton);
      this.Name = "OperatorLibraryEditor";
      this.Size = new System.Drawing.Size(522, 395);
      this.operatorsGroupBox.ResumeLayout(false);
      this.contextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox operatorsGroupBox;
    private System.Windows.Forms.TreeView operatorsTreeView;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.Button addGroupButton;
    private System.Windows.Forms.Button addOperatorButton;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.Button importButton;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
  }
}
