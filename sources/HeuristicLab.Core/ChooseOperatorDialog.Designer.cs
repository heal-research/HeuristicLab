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
  partial class ChooseOperatorDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.builtinOperatorsGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.builtinOperatorsTreeView = new System.Windows.Forms.TreeView();
      this.builtinOperatorsDescriptionTextBox = new System.Windows.Forms.TextBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.operatorLibrariesTabPage = new System.Windows.Forms.TabPage();
      this.operatorLibraryTextBox = new System.Windows.Forms.TextBox();
      this.operatorLibraryLabel = new System.Windows.Forms.Label();
      this.operatorLibraryOperatorsGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.operatorLibraryOperatorsTreeView = new System.Windows.Forms.TreeView();
      this.operatorLibraryOperatorsDescriptionTextBox = new System.Windows.Forms.TextBox();
      this.loadButton = new System.Windows.Forms.Button();
      this.builtinTabPage = new System.Windows.Forms.TabPage();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.builtinOperatorsGroupBox.SuspendLayout();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.operatorLibrariesTabPage.SuspendLayout();
      this.operatorLibraryOperatorsGroupBox.SuspendLayout();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.builtinTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(305, 368);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 1;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(386, 368);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 2;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // builtinOperatorsGroupBox
      // 
      this.builtinOperatorsGroupBox.Controls.Add(this.splitContainer2);
      this.builtinOperatorsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.builtinOperatorsGroupBox.Location = new System.Drawing.Point(3, 3);
      this.builtinOperatorsGroupBox.Name = "builtinOperatorsGroupBox";
      this.builtinOperatorsGroupBox.Size = new System.Drawing.Size(435, 318);
      this.builtinOperatorsGroupBox.TabIndex = 0;
      this.builtinOperatorsGroupBox.TabStop = false;
      this.builtinOperatorsGroupBox.Text = "&Available Operators";
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(3, 16);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.builtinOperatorsTreeView);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.builtinOperatorsDescriptionTextBox);
      this.splitContainer2.Size = new System.Drawing.Size(429, 299);
      this.splitContainer2.SplitterDistance = 220;
      this.splitContainer2.TabIndex = 2;
      // 
      // builtinOperatorsTreeView
      // 
      this.builtinOperatorsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.builtinOperatorsTreeView.HideSelection = false;
      this.builtinOperatorsTreeView.Location = new System.Drawing.Point(0, 0);
      this.builtinOperatorsTreeView.Name = "builtinOperatorsTreeView";
      this.builtinOperatorsTreeView.Size = new System.Drawing.Size(429, 220);
      this.builtinOperatorsTreeView.TabIndex = 0;
      this.builtinOperatorsTreeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
      this.builtinOperatorsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.builtinOperatorsTreeView_AfterSelect);
      // 
      // builtinOperatorsDescriptionTextBox
      // 
      this.builtinOperatorsDescriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.builtinOperatorsDescriptionTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.builtinOperatorsDescriptionTextBox.Location = new System.Drawing.Point(0, 0);
      this.builtinOperatorsDescriptionTextBox.Multiline = true;
      this.builtinOperatorsDescriptionTextBox.Name = "builtinOperatorsDescriptionTextBox";
      this.builtinOperatorsDescriptionTextBox.ReadOnly = true;
      this.builtinOperatorsDescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.builtinOperatorsDescriptionTextBox.Size = new System.Drawing.Size(429, 75);
      this.builtinOperatorsDescriptionTextBox.TabIndex = 0;
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.operatorLibrariesTabPage);
      this.tabControl.Controls.Add(this.builtinTabPage);
      this.tabControl.Location = new System.Drawing.Point(12, 12);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(449, 350);
      this.tabControl.TabIndex = 0;
      // 
      // operatorLibrariesTabPage
      // 
      this.operatorLibrariesTabPage.Controls.Add(this.operatorLibraryTextBox);
      this.operatorLibrariesTabPage.Controls.Add(this.operatorLibraryLabel);
      this.operatorLibrariesTabPage.Controls.Add(this.operatorLibraryOperatorsGroupBox);
      this.operatorLibrariesTabPage.Controls.Add(this.loadButton);
      this.operatorLibrariesTabPage.Location = new System.Drawing.Point(4, 22);
      this.operatorLibrariesTabPage.Name = "operatorLibrariesTabPage";
      this.operatorLibrariesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.operatorLibrariesTabPage.Size = new System.Drawing.Size(441, 324);
      this.operatorLibrariesTabPage.TabIndex = 0;
      this.operatorLibrariesTabPage.Text = "Operator Libraries";
      this.operatorLibrariesTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorLibraryTextBox
      // 
      this.operatorLibraryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorLibraryTextBox.Location = new System.Drawing.Point(97, 8);
      this.operatorLibraryTextBox.Name = "operatorLibraryTextBox";
      this.operatorLibraryTextBox.ReadOnly = true;
      this.operatorLibraryTextBox.Size = new System.Drawing.Size(257, 20);
      this.operatorLibraryTextBox.TabIndex = 2;
      // 
      // operatorLibraryLabel
      // 
      this.operatorLibraryLabel.AutoSize = true;
      this.operatorLibraryLabel.Location = new System.Drawing.Point(6, 11);
      this.operatorLibraryLabel.Name = "operatorLibraryLabel";
      this.operatorLibraryLabel.Size = new System.Drawing.Size(85, 13);
      this.operatorLibraryLabel.TabIndex = 1;
      this.operatorLibraryLabel.Text = "&Operator Library:";
      // 
      // operatorLibraryOperatorsGroupBox
      // 
      this.operatorLibraryOperatorsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorLibraryOperatorsGroupBox.Controls.Add(this.splitContainer1);
      this.operatorLibraryOperatorsGroupBox.Enabled = false;
      this.operatorLibraryOperatorsGroupBox.Location = new System.Drawing.Point(6, 35);
      this.operatorLibraryOperatorsGroupBox.Name = "operatorLibraryOperatorsGroupBox";
      this.operatorLibraryOperatorsGroupBox.Size = new System.Drawing.Size(429, 283);
      this.operatorLibraryOperatorsGroupBox.TabIndex = 3;
      this.operatorLibraryOperatorsGroupBox.TabStop = false;
      this.operatorLibraryOperatorsGroupBox.Text = "&Available Operators";
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(3, 16);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.operatorLibraryOperatorsTreeView);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.operatorLibraryOperatorsDescriptionTextBox);
      this.splitContainer1.Size = new System.Drawing.Size(423, 264);
      this.splitContainer1.SplitterDistance = 187;
      this.splitContainer1.TabIndex = 1;
      // 
      // operatorLibraryOperatorsTreeView
      // 
      this.operatorLibraryOperatorsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorLibraryOperatorsTreeView.HideSelection = false;
      this.operatorLibraryOperatorsTreeView.Location = new System.Drawing.Point(0, 0);
      this.operatorLibraryOperatorsTreeView.Name = "operatorLibraryOperatorsTreeView";
      this.operatorLibraryOperatorsTreeView.ShowNodeToolTips = true;
      this.operatorLibraryOperatorsTreeView.Size = new System.Drawing.Size(423, 187);
      this.operatorLibraryOperatorsTreeView.TabIndex = 0;
      this.operatorLibraryOperatorsTreeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
      this.operatorLibraryOperatorsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.operatorsTreeView_AfterSelect);
      // 
      // operatorLibraryOperatorsDescriptionTextBox
      // 
      this.operatorLibraryOperatorsDescriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorLibraryOperatorsDescriptionTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.operatorLibraryOperatorsDescriptionTextBox.Location = new System.Drawing.Point(0, 0);
      this.operatorLibraryOperatorsDescriptionTextBox.Multiline = true;
      this.operatorLibraryOperatorsDescriptionTextBox.Name = "operatorLibraryOperatorsDescriptionTextBox";
      this.operatorLibraryOperatorsDescriptionTextBox.ReadOnly = true;
      this.operatorLibraryOperatorsDescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.operatorLibraryOperatorsDescriptionTextBox.Size = new System.Drawing.Size(423, 73);
      this.operatorLibraryOperatorsDescriptionTextBox.TabIndex = 0;
      // 
      // loadButton
      // 
      this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.loadButton.Location = new System.Drawing.Point(360, 6);
      this.loadButton.Name = "loadButton";
      this.loadButton.Size = new System.Drawing.Size(75, 23);
      this.loadButton.TabIndex = 0;
      this.loadButton.Text = "&Load...";
      this.loadButton.UseVisualStyleBackColor = true;
      this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
      // 
      // builtinTabPage
      // 
      this.builtinTabPage.Controls.Add(this.builtinOperatorsGroupBox);
      this.builtinTabPage.Location = new System.Drawing.Point(4, 22);
      this.builtinTabPage.Name = "builtinTabPage";
      this.builtinTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.builtinTabPage.Size = new System.Drawing.Size(441, 324);
      this.builtinTabPage.TabIndex = 1;
      this.builtinTabPage.Text = "Built-In";
      this.builtinTabPage.UseVisualStyleBackColor = true;
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "library";
      this.openFileDialog.Filter = "HeuristicLab files|*.hl|All files|*.*";
      this.openFileDialog.Title = "Open file ...";
      // 
      // ChooseOperatorDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(473, 403);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChooseOperatorDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Choose Operator";
      this.Shown += new System.EventHandler(this.ChooseOperatorDialog_Shown);
      this.builtinOperatorsGroupBox.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.Panel2.PerformLayout();
      this.splitContainer2.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.operatorLibrariesTabPage.ResumeLayout(false);
      this.operatorLibrariesTabPage.PerformLayout();
      this.operatorLibraryOperatorsGroupBox.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      this.splitContainer1.ResumeLayout(false);
      this.builtinTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.GroupBox builtinOperatorsGroupBox;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage operatorLibrariesTabPage;
    private System.Windows.Forms.TabPage builtinTabPage;
    private System.Windows.Forms.TextBox operatorLibraryTextBox;
    private System.Windows.Forms.Label operatorLibraryLabel;
    private System.Windows.Forms.GroupBox operatorLibraryOperatorsGroupBox;
    private System.Windows.Forms.TreeView operatorLibraryOperatorsTreeView;
    private System.Windows.Forms.Button loadButton;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.TreeView builtinOperatorsTreeView;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.TextBox builtinOperatorsDescriptionTextBox;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TextBox operatorLibraryOperatorsDescriptionTextBox;
  }
}
