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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  partial class SymbolicExpressionGrammarEditorView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SymbolicExpressionGrammarEditorView));
      this.grpSymbols = new System.Windows.Forms.GroupBox();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.copyButton = new System.Windows.Forms.Button();
      this.showDetailsCheckBox = new System.Windows.Forms.CheckBox();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.symbolsTreeView = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.CheckBoxTreeView();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.allowedChildSymbolsControl = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionGrammarAllowedChildSymbolsControl();
      this.symbolDetailsGroupBox = new System.Windows.Forms.GroupBox();
      this.symbolDetailsViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.showSampleTreeButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.grpSymbols.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.symbolDetailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(686, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(750, 3);
      // 
      // grpSymbols
      // 
      this.grpSymbols.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grpSymbols.Controls.Add(this.splitContainer1);
      this.grpSymbols.Location = new System.Drawing.Point(0, 26);
      this.grpSymbols.Name = "grpSymbols";
      this.grpSymbols.Size = new System.Drawing.Size(769, 378);
      this.grpSymbols.TabIndex = 3;
      this.grpSymbols.TabStop = false;
      this.grpSymbols.Text = "Symbols";
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(3, 16);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.showSampleTreeButton);
      this.splitContainer1.Panel1.Controls.Add(this.copyButton);
      this.splitContainer1.Panel1.Controls.Add(this.showDetailsCheckBox);
      this.splitContainer1.Panel1.Controls.Add(this.removeButton);
      this.splitContainer1.Panel1.Controls.Add(this.addButton);
      this.splitContainer1.Panel1.Controls.Add(this.symbolsTreeView);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
      this.splitContainer1.Size = new System.Drawing.Size(763, 359);
      this.splitContainer1.SplitterDistance = 191;
      this.splitContainer1.TabIndex = 0;
      // 
      // copyButton
      // 
      this.copyButton.Enabled = false;
      this.copyButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Copy;
      this.copyButton.Location = new System.Drawing.Point(33, 3);
      this.copyButton.Name = "copyButton";
      this.copyButton.Size = new System.Drawing.Size(24, 24);
      this.copyButton.TabIndex = 8;
      this.toolTip.SetToolTip(this.copyButton, "Copy Symbol");
      this.copyButton.UseVisualStyleBackColor = true;
      this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
      // 
      // showDetailsCheckBox
      // 
      this.showDetailsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showDetailsCheckBox.Checked = true;
      this.showDetailsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showDetailsCheckBox.Image = HeuristicLab.Common.Resources.VSImageLibrary.Properties;
      this.showDetailsCheckBox.Location = new System.Drawing.Point(93, 3);
      this.showDetailsCheckBox.Name = "showDetailsCheckBox";
      this.showDetailsCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showDetailsCheckBox.TabIndex = 7;
      this.toolTip.SetToolTip(this.showDetailsCheckBox, "Show/Hide Details");
      this.showDetailsCheckBox.UseVisualStyleBackColor = true;
      this.showDetailsCheckBox.CheckedChanged += new System.EventHandler(this.showDetailsCheckBox_CheckedChanged);
      // 
      // removeButton
      // 
      this.removeButton.Enabled = false;
      this.removeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.removeButton.Location = new System.Drawing.Point(63, 3);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(24, 24);
      this.removeButton.TabIndex = 6;
      this.toolTip.SetToolTip(this.removeButton, "Remove Symbol");
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addButton
      // 
      this.addButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Add;
      this.addButton.Location = new System.Drawing.Point(3, 3);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(24, 24);
      this.addButton.TabIndex = 5;
      this.toolTip.SetToolTip(this.addButton, "Add Symbol");
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // symbolsTreeView
      // 
      this.symbolsTreeView.AllowDrop = true;
      this.symbolsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.symbolsTreeView.CheckBoxes = true;
      this.symbolsTreeView.HideSelection = false;
      this.symbolsTreeView.ImageIndex = 0;
      this.symbolsTreeView.ImageList = this.imageList;
      this.symbolsTreeView.Location = new System.Drawing.Point(3, 28);
      this.symbolsTreeView.Name = "symbolsTreeView";
      this.symbolsTreeView.SelectedImageIndex = 0;
      this.symbolsTreeView.Size = new System.Drawing.Size(185, 299);
      this.symbolsTreeView.TabIndex = 0;
      this.symbolsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.symbolsTreeView_AfterCheck);
      this.symbolsTreeView.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(symbolsTreeView_BeforeCheck);
      this.symbolsTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.symbolsTreeView_ItemDrag);
      this.symbolsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.symbolsTreeView_AfterSelect);
      this.symbolsTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.symbolsTreeView_DragDrop);
      this.symbolsTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.symbolsTreeView_DragEnter);
      this.symbolsTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.symbolsTreeView_DragOver);
      this.symbolsTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.symbolsTreeView_KeyDown);
      this.symbolsTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.symbolsTreeView_MouseDown);
      this.symbolsTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(symbolsTreeView_NodeMouseDoubleClick);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.allowedChildSymbolsControl);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.symbolDetailsGroupBox);
      this.splitContainer2.Size = new System.Drawing.Size(568, 359);
      this.splitContainer2.SplitterDistance = 165;
      this.splitContainer2.TabIndex = 0;
      // 
      // allowedChildSymbolsControl
      // 
      this.allowedChildSymbolsControl.AllowDrop = true;
      this.allowedChildSymbolsControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.allowedChildSymbolsControl.Grammar = null;
      this.allowedChildSymbolsControl.Location = new System.Drawing.Point(0, 0);
      this.allowedChildSymbolsControl.Name = "allowedChildSymbolsControl";
      this.allowedChildSymbolsControl.Size = new System.Drawing.Size(568, 165);
      this.allowedChildSymbolsControl.Symbol = null;
      this.allowedChildSymbolsControl.TabIndex = 0;
      // 
      // symbolDetailsGroupBox
      // 
      this.symbolDetailsGroupBox.Controls.Add(this.symbolDetailsViewHost);
      this.symbolDetailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.symbolDetailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.symbolDetailsGroupBox.Name = "symbolDetailsGroupBox";
      this.symbolDetailsGroupBox.Size = new System.Drawing.Size(568, 190);
      this.symbolDetailsGroupBox.TabIndex = 1;
      this.symbolDetailsGroupBox.TabStop = false;
      this.symbolDetailsGroupBox.Text = "Symbol Details";
      // 
      // symbolDetailsViewHost
      // 
      this.symbolDetailsViewHost.Caption = "View";
      this.symbolDetailsViewHost.Content = null;
      this.symbolDetailsViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.symbolDetailsViewHost.Enabled = false;
      this.symbolDetailsViewHost.Location = new System.Drawing.Point(3, 16);
      this.symbolDetailsViewHost.Name = "symbolDetailsViewHost";
      this.symbolDetailsViewHost.ReadOnly = false;
      this.symbolDetailsViewHost.Size = new System.Drawing.Size(562, 171);
      this.symbolDetailsViewHost.TabIndex = 1;
      this.symbolDetailsViewHost.ViewsLabelVisible = true;
      this.symbolDetailsViewHost.ViewType = null;
      // 
      // showSampleTreeButton
      // 
      this.showSampleTreeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.showSampleTreeButton.Location = new System.Drawing.Point(3, 333);
      this.showSampleTreeButton.Name = "showSampleTreeButton";
      this.showSampleTreeButton.Size = new System.Drawing.Size(185, 23);
      this.showSampleTreeButton.TabIndex = 9;
      this.showSampleTreeButton.Text = "Show Sample Tree";
      this.showSampleTreeButton.UseVisualStyleBackColor = true;
      this.showSampleTreeButton.Click += new System.EventHandler(this.showSampleTreeButton_Click);
      // 
      // SymbolicExpressionGrammarEditorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.grpSymbols);
      this.Name = "SymbolicExpressionGrammarEditorView";
      this.Size = new System.Drawing.Size(769, 404);
      this.Controls.SetChildIndex(this.grpSymbols, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.grpSymbols.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.symbolDetailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private System.Windows.Forms.GroupBox grpSymbols;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private CheckBoxTreeView symbolsTreeView;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.ImageList imageList;
    protected System.Windows.Forms.CheckBox showDetailsCheckBox;
    protected System.Windows.Forms.Button removeButton;
    protected System.Windows.Forms.Button addButton;
    private System.Windows.Forms.GroupBox symbolDetailsGroupBox;
    private MainForm.WindowsForms.ViewHost symbolDetailsViewHost;
    protected System.Windows.Forms.Button copyButton;
    private SymbolicExpressionGrammarAllowedChildSymbolsControl allowedChildSymbolsControl;
    private System.Windows.Forms.Button showSampleTreeButton;
  }
}
