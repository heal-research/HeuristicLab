#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Scripting.Views {
  partial class ScriptView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptView));
      this.compilationLabel = new System.Windows.Forms.Label();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.compileButton = new System.Windows.Forms.Button();
      this.infoTabControl = new System.Windows.Forms.TabControl();
      this.outputTabPage = new System.Windows.Forms.TabPage();
      this.outputTextBox = new System.Windows.Forms.TextBox();
      this.errorListTabPage = new System.Windows.Forms.TabPage();
      this.errorListView = new System.Windows.Forms.ListView();
      this.iconColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.categoryColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.errorNumberColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.lineColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.descriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.codeEditor = new HeuristicLab.CodeEditor.CodeEditor();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.infoTabControl.SuspendLayout();
      this.outputTabPage.SuspendLayout();
      this.errorListTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(69, 0);
      this.nameTextBox.Size = new System.Drawing.Size(741, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(816, 4);
      // 
      // compilationLabel
      // 
      this.compilationLabel.AutoSize = true;
      this.compilationLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
      this.compilationLabel.Location = new System.Drawing.Point(66, 32);
      this.compilationLabel.Name = "compilationLabel";
      this.compilationLabel.Size = new System.Drawing.Size(69, 13);
      this.compilationLabel.TabIndex = 3;
      this.compilationLabel.Text = "Not compiled";
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // compileButton
      // 
      this.compileButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Script;
      this.compileButton.Location = new System.Drawing.Point(6, 26);
      this.compileButton.Name = "compileButton";
      this.compileButton.Size = new System.Drawing.Size(24, 24);
      this.compileButton.TabIndex = 8;
      this.compileButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.compileButton, "Compile (F6)");
      this.compileButton.UseVisualStyleBackColor = true;
      this.compileButton.Click += new System.EventHandler(this.compileButton_Click);
      // 
      // infoTabControl
      // 
      this.infoTabControl.Controls.Add(this.outputTabPage);
      this.infoTabControl.Controls.Add(this.errorListTabPage);
      this.infoTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.infoTabControl.Location = new System.Drawing.Point(0, 0);
      this.infoTabControl.Name = "infoTabControl";
      this.infoTabControl.SelectedIndex = 0;
      this.infoTabControl.Size = new System.Drawing.Size(832, 112);
      this.infoTabControl.TabIndex = 1;
      // 
      // outputTabPage
      // 
      this.outputTabPage.Controls.Add(this.outputTextBox);
      this.outputTabPage.Location = new System.Drawing.Point(4, 22);
      this.outputTabPage.Name = "outputTabPage";
      this.outputTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.outputTabPage.Size = new System.Drawing.Size(824, 86);
      this.outputTabPage.TabIndex = 1;
      this.outputTabPage.Text = "Output";
      this.outputTabPage.UseVisualStyleBackColor = true;
      // 
      // outputTextBox
      // 
      this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.outputTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.outputTextBox.Location = new System.Drawing.Point(3, 3);
      this.outputTextBox.Multiline = true;
      this.outputTextBox.Name = "outputTextBox";
      this.outputTextBox.ReadOnly = true;
      this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.outputTextBox.Size = new System.Drawing.Size(818, 80);
      this.outputTextBox.TabIndex = 0;
      this.outputTextBox.WordWrap = false;
      // 
      // errorListTabPage
      // 
      this.errorListTabPage.Controls.Add(this.errorListView);
      this.errorListTabPage.Location = new System.Drawing.Point(4, 22);
      this.errorListTabPage.Name = "errorListTabPage";
      this.errorListTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.errorListTabPage.Size = new System.Drawing.Size(824, 86);
      this.errorListTabPage.TabIndex = 0;
      this.errorListTabPage.Text = "Error List";
      this.errorListTabPage.UseVisualStyleBackColor = true;
      // 
      // errorListView
      // 
      this.errorListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.iconColumnHeader,
            this.categoryColumnHeader,
            this.errorNumberColumnHeader,
            this.lineColumnHeader,
            this.columnColumnHeader,
            this.descriptionColumnHeader});
      this.errorListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.errorListView.FullRowSelect = true;
      this.errorListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.errorListView.HideSelection = false;
      this.errorListView.Location = new System.Drawing.Point(3, 3);
      this.errorListView.Name = "errorListView";
      this.errorListView.Size = new System.Drawing.Size(818, 80);
      this.errorListView.SmallImageList = this.imageList;
      this.errorListView.TabIndex = 0;
      this.errorListView.UseCompatibleStateImageBehavior = false;
      this.errorListView.View = System.Windows.Forms.View.Details;
      this.errorListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.errorListView_MouseDoubleClick);
      // 
      // iconColumnHeader
      // 
      this.iconColumnHeader.Text = "";
      // 
      // categoryColumnHeader
      // 
      this.categoryColumnHeader.Text = "Category";
      // 
      // errorNumberColumnHeader
      // 
      this.errorNumberColumnHeader.Text = "Error Number";
      // 
      // lineColumnHeader
      // 
      this.lineColumnHeader.Text = "Line";
      // 
      // columnColumnHeader
      // 
      this.columnColumnHeader.Text = "Column";
      // 
      // descriptionColumnHeader
      // 
      this.descriptionColumnHeader.Text = "Description";
      // 
      // codeEditor
      // 
      this.codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.codeEditor.Location = new System.Drawing.Point(0, 0);
      this.codeEditor.Name = "codeEditor";
      this.codeEditor.Prefix = "";
      this.codeEditor.Size = new System.Drawing.Size(832, 430);
      this.codeEditor.Suffix = "";
      this.codeEditor.TabIndex = 0;
      this.codeEditor.UserCode = "";
      this.codeEditor.TextEditorTextChanged += new System.EventHandler(this.codeEditor_TextEditorTextChanged);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point(0, 56);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.codeEditor);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.infoTabControl);
      this.splitContainer1.Size = new System.Drawing.Size(832, 546);
      this.splitContainer1.SplitterDistance = 430;
      this.splitContainer1.TabIndex = 9;
      // 
      // ScriptView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.compileButton);
      this.Controls.Add(this.compilationLabel);
      this.Name = "ScriptView";
      this.Size = new System.Drawing.Size(835, 602);
      this.Controls.SetChildIndex(this.compilationLabel, 0);
      this.Controls.SetChildIndex(this.compileButton, 0);
      this.Controls.SetChildIndex(this.splitContainer1, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.infoTabControl.ResumeLayout(false);
      this.outputTabPage.ResumeLayout(false);
      this.outputTabPage.PerformLayout();
      this.errorListTabPage.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label compilationLabel;
    protected System.Windows.Forms.Button compileButton;
    protected System.Windows.Forms.ImageList imageList;
    protected System.Windows.Forms.TabControl infoTabControl;
    protected System.Windows.Forms.TabPage outputTabPage;
    protected System.Windows.Forms.TextBox outputTextBox;
    protected System.Windows.Forms.TabPage errorListTabPage;
    protected System.Windows.Forms.ListView errorListView;
    protected System.Windows.Forms.ColumnHeader iconColumnHeader;
    protected System.Windows.Forms.ColumnHeader categoryColumnHeader;
    protected System.Windows.Forms.ColumnHeader errorNumberColumnHeader;
    protected System.Windows.Forms.ColumnHeader lineColumnHeader;
    protected System.Windows.Forms.ColumnHeader columnColumnHeader;
    protected System.Windows.Forms.ColumnHeader descriptionColumnHeader;
    protected CodeEditor.CodeEditor codeEditor;
    protected System.Windows.Forms.SplitContainer splitContainer1;
  }
}
