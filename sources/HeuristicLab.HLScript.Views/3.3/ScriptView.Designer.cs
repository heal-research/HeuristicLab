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

namespace HeuristicLab.HLScript.Views {
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
      this.compilationLabel = new System.Windows.Forms.Label();
      this.startStopButton = new System.Windows.Forms.Button();
      this.errorListView = new System.Windows.Forms.ListView();
      this.categoryColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.errorNumberColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.lineColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.descriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.codeEditor = new HeuristicLab.CodeEditor.CodeEditor();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.errorListTabPage = new System.Windows.Forms.TabPage();
      this.outputTabPage = new System.Windows.Forms.TabPage();
      this.outputTextBox = new System.Windows.Forms.TextBox();
      this.variableStoreView = new HeuristicLab.HLScript.Views.VariableStoreView();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.errorListTabPage.SuspendLayout();
      this.outputTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(60, 0);
      this.nameTextBox.Size = new System.Drawing.Size(750, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(816, 4);
      // 
      // compilationLabel
      // 
      this.compilationLabel.AutoSize = true;
      this.compilationLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
      this.compilationLabel.Location = new System.Drawing.Point(33, 32);
      this.compilationLabel.Name = "compilationLabel";
      this.compilationLabel.Size = new System.Drawing.Size(69, 13);
      this.compilationLabel.TabIndex = 3;
      this.compilationLabel.Text = "Not compiled";
      // 
      // startStopButton
      // 
      this.startStopButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.startStopButton.Location = new System.Drawing.Point(3, 26);
      this.startStopButton.Name = "startStopButton";
      this.startStopButton.Size = new System.Drawing.Size(24, 24);
      this.startStopButton.TabIndex = 1;
      this.startStopButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.startStopButton, "Run (F5)");
      this.startStopButton.UseVisualStyleBackColor = true;
      this.startStopButton.Click += new System.EventHandler(this.startStopButton_Click);
      // 
      // errorListView
      // 
      this.errorListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
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
      this.errorListView.Size = new System.Drawing.Size(623, 79);
      this.errorListView.TabIndex = 0;
      this.errorListView.UseCompatibleStateImageBehavior = false;
      this.errorListView.View = System.Windows.Forms.View.Details;
      // 
      // categoryColumnHeader
      // 
      this.categoryColumnHeader.Text = "Category";
      // 
      // errorNumberColumnHeader
      // 
      this.errorNumberColumnHeader.Text = "Error Number";
      this.errorNumberColumnHeader.Width = 78;
      // 
      // lineColumnHeader
      // 
      this.lineColumnHeader.Text = "Line";
      this.lineColumnHeader.Width = 50;
      // 
      // columnColumnHeader
      // 
      this.columnColumnHeader.Text = "Column";
      this.columnColumnHeader.Width = 50;
      // 
      // descriptionColumnHeader
      // 
      this.descriptionColumnHeader.Text = "Description";
      this.descriptionColumnHeader.Width = 600;
      // 
      // codeEditor
      // 
      this.codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.codeEditor.Location = new System.Drawing.Point(0, 0);
      this.codeEditor.Name = "codeEditor";
      this.codeEditor.Prefix = "";
      this.codeEditor.Size = new System.Drawing.Size(637, 428);
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
      this.splitContainer1.Location = new System.Drawing.Point(3, 56);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.variableStoreView);
      this.splitContainer1.Panel2.Controls.Add(this.viewHost);
      this.splitContainer1.Size = new System.Drawing.Size(829, 543);
      this.splitContainer1.SplitterDistance = 637;
      this.splitContainer1.TabIndex = 7;
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
      this.splitContainer2.Panel1.Controls.Add(this.codeEditor);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
      this.splitContainer2.Size = new System.Drawing.Size(637, 543);
      this.splitContainer2.SplitterDistance = 428;
      this.splitContainer2.TabIndex = 5;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.errorListTabPage);
      this.tabControl1.Controls.Add(this.outputTabPage);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(637, 111);
      this.tabControl1.TabIndex = 1;
      // 
      // errorListTabPage
      // 
      this.errorListTabPage.Controls.Add(this.errorListView);
      this.errorListTabPage.Location = new System.Drawing.Point(4, 22);
      this.errorListTabPage.Name = "errorListTabPage";
      this.errorListTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.errorListTabPage.Size = new System.Drawing.Size(629, 85);
      this.errorListTabPage.TabIndex = 0;
      this.errorListTabPage.Text = "Error List";
      this.errorListTabPage.UseVisualStyleBackColor = true;
      // 
      // outputTabPage
      // 
      this.outputTabPage.Controls.Add(this.outputTextBox);
      this.outputTabPage.Location = new System.Drawing.Point(4, 22);
      this.outputTabPage.Name = "outputTabPage";
      this.outputTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.outputTabPage.Size = new System.Drawing.Size(629, 85);
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
      this.outputTextBox.Size = new System.Drawing.Size(623, 79);
      this.outputTextBox.TabIndex = 0;
      this.outputTextBox.WordWrap = false;
      // 
      // variableStoreView
      // 
      this.variableStoreView.Caption = "ItemCollection View";
      this.variableStoreView.Content = null;
      this.variableStoreView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableStoreView.Location = new System.Drawing.Point(0, 0);
      this.variableStoreView.Name = "variableStoreView";
      this.variableStoreView.ReadOnly = false;
      this.variableStoreView.Size = new System.Drawing.Size(188, 543);
      this.variableStoreView.TabIndex = 0;
      // 
      // viewHost
      // 
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(0, 0);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(188, 543);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // ScriptView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.startStopButton);
      this.Controls.Add(this.compilationLabel);
      this.Name = "ScriptView";
      this.Size = new System.Drawing.Size(835, 602);
      this.Controls.SetChildIndex(this.compilationLabel, 0);
      this.Controls.SetChildIndex(this.startStopButton, 0);
      this.Controls.SetChildIndex(this.splitContainer1, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.errorListTabPage.ResumeLayout(false);
      this.outputTabPage.ResumeLayout(false);
      this.outputTabPage.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label compilationLabel;
    private System.Windows.Forms.Button startStopButton;
    private System.Windows.Forms.ListView errorListView;
    private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
    private System.Windows.Forms.ColumnHeader lineColumnHeader;
    private System.Windows.Forms.ColumnHeader columnColumnHeader;
    private CodeEditor.CodeEditor codeEditor;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private MainForm.WindowsForms.ViewHost viewHost;
    private VariableStoreView variableStoreView;
    private System.Windows.Forms.ColumnHeader errorNumberColumnHeader;
    private System.Windows.Forms.ColumnHeader categoryColumnHeader;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage errorListTabPage;
    private System.Windows.Forms.TabPage outputTabPage;
    private System.Windows.Forms.TextBox outputTextBox;
  }
}
