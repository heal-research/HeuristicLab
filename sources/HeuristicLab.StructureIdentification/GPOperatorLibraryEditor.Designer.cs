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

namespace HeuristicLab.StructureIdentification {
  partial class GPOperatorLibraryEditor {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.operatorLibraryEditor = new HeuristicLab.Core.OperatorLibraryEditor();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.operatorLibraryTabPage = new System.Windows.Forms.TabPage();
      this.initializationTabPage = new System.Windows.Forms.TabPage();
      this.initSplitContainer = new System.Windows.Forms.SplitContainer();
      this.initListView = new System.Windows.Forms.ListView();
      this.initVariableView = new HeuristicLab.Core.VariableView();
      this.mutationTabPage = new System.Windows.Forms.TabPage();
      this.mutationSplitContainer = new System.Windows.Forms.SplitContainer();
      this.mutationListView = new System.Windows.Forms.ListView();
      this.mutationVariableView = new HeuristicLab.Core.VariableView();
      this.tabControl.SuspendLayout();
      this.operatorLibraryTabPage.SuspendLayout();
      this.initializationTabPage.SuspendLayout();
      this.initSplitContainer.Panel1.SuspendLayout();
      this.initSplitContainer.Panel2.SuspendLayout();
      this.initSplitContainer.SuspendLayout();
      this.mutationTabPage.SuspendLayout();
      this.mutationSplitContainer.Panel1.SuspendLayout();
      this.mutationSplitContainer.Panel2.SuspendLayout();
      this.mutationSplitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // operatorLibraryEditor
      // 
      this.operatorLibraryEditor.Caption = "Operator Library";
      this.operatorLibraryEditor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorLibraryEditor.Filename = null;
      this.operatorLibraryEditor.Location = new System.Drawing.Point(3, 3);
      this.operatorLibraryEditor.Name = "operatorLibraryEditor";
      this.operatorLibraryEditor.OperatorLibrary = null;
      this.operatorLibraryEditor.Size = new System.Drawing.Size(663, 529);
      this.operatorLibraryEditor.TabIndex = 0;
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.operatorLibraryTabPage);
      this.tabControl.Controls.Add(this.initializationTabPage);
      this.tabControl.Controls.Add(this.mutationTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(677, 561);
      this.tabControl.TabIndex = 1;
      // 
      // operatorLibraryTabPage
      // 
      this.operatorLibraryTabPage.Controls.Add(this.operatorLibraryEditor);
      this.operatorLibraryTabPage.Location = new System.Drawing.Point(4, 22);
      this.operatorLibraryTabPage.Name = "operatorLibraryTabPage";
      this.operatorLibraryTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.operatorLibraryTabPage.Size = new System.Drawing.Size(669, 535);
      this.operatorLibraryTabPage.TabIndex = 0;
      this.operatorLibraryTabPage.Text = "Operator Library";
      this.operatorLibraryTabPage.UseVisualStyleBackColor = true;
      // 
      // initializationTabPage
      // 
      this.initializationTabPage.Controls.Add(this.initSplitContainer);
      this.initializationTabPage.Location = new System.Drawing.Point(4, 22);
      this.initializationTabPage.Name = "initializationTabPage";
      this.initializationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.initializationTabPage.Size = new System.Drawing.Size(669, 535);
      this.initializationTabPage.TabIndex = 1;
      this.initializationTabPage.Text = "Initialization";
      this.initializationTabPage.UseVisualStyleBackColor = true;
      // 
      // initSplitContainer
      // 
      this.initSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.initSplitContainer.Location = new System.Drawing.Point(3, 3);
      this.initSplitContainer.Name = "initSplitContainer";
      // 
      // initSplitContainer.Panel1
      // 
      this.initSplitContainer.Panel1.Controls.Add(this.initListView);
      // 
      // initSplitContainer.Panel2
      // 
      this.initSplitContainer.Panel2.Controls.Add(this.initVariableView);
      this.initSplitContainer.Size = new System.Drawing.Size(663, 529);
      this.initSplitContainer.SplitterDistance = 221;
      this.initSplitContainer.TabIndex = 0;
      // 
      // initListView
      // 
      this.initListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.initListView.Location = new System.Drawing.Point(0, 0);
      this.initListView.Name = "initListView";
      this.initListView.Size = new System.Drawing.Size(221, 529);
      this.initListView.TabIndex = 0;
      this.initListView.UseCompatibleStateImageBehavior = false;
      this.initListView.View = System.Windows.Forms.View.List;
      this.initListView.SelectedIndexChanged += new System.EventHandler(this.initListView_SelectedIndexChanged);
      // 
      // initVariableView
      // 
      this.initVariableView.Caption = "Variable";
      this.initVariableView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.initVariableView.Location = new System.Drawing.Point(0, 0);
      this.initVariableView.Name = "initVariableView";
      this.initVariableView.Size = new System.Drawing.Size(438, 529);
      this.initVariableView.TabIndex = 0;
      this.initVariableView.Variable = null;
      // 
      // mutationTabPage
      // 
      this.mutationTabPage.Controls.Add(this.mutationSplitContainer);
      this.mutationTabPage.Location = new System.Drawing.Point(4, 22);
      this.mutationTabPage.Name = "mutationTabPage";
      this.mutationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.mutationTabPage.Size = new System.Drawing.Size(669, 535);
      this.mutationTabPage.TabIndex = 2;
      this.mutationTabPage.Text = "Mutation";
      this.mutationTabPage.UseVisualStyleBackColor = true;
      // 
      // mutationSplitContainer
      // 
      this.mutationSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.mutationSplitContainer.Location = new System.Drawing.Point(3, 3);
      this.mutationSplitContainer.Name = "mutationSplitContainer";
      // 
      // mutationSplitContainer.Panel1
      // 
      this.mutationSplitContainer.Panel1.Controls.Add(this.mutationListView);
      // 
      // mutationSplitContainer.Panel2
      // 
      this.mutationSplitContainer.Panel2.Controls.Add(this.mutationVariableView);
      this.mutationSplitContainer.Size = new System.Drawing.Size(663, 529);
      this.mutationSplitContainer.SplitterDistance = 221;
      this.mutationSplitContainer.TabIndex = 0;
      // 
      // mutationListView
      // 
      this.mutationListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.mutationListView.Location = new System.Drawing.Point(0, 0);
      this.mutationListView.MultiSelect = false;
      this.mutationListView.Name = "mutationListView";
      this.mutationListView.Size = new System.Drawing.Size(221, 529);
      this.mutationListView.TabIndex = 0;
      this.mutationListView.UseCompatibleStateImageBehavior = false;
      this.mutationListView.View = System.Windows.Forms.View.List;
      this.mutationListView.SelectedIndexChanged += new System.EventHandler(this.mutationListView_SelectedIndexChanged);
      // 
      // mutationVariableView
      // 
      this.mutationVariableView.Caption = "Variable";
      this.mutationVariableView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.mutationVariableView.Location = new System.Drawing.Point(0, 0);
      this.mutationVariableView.Name = "mutationVariableView";
      this.mutationVariableView.Size = new System.Drawing.Size(438, 529);
      this.mutationVariableView.TabIndex = 0;
      this.mutationVariableView.Variable = null;
      // 
      // GPOperatorLibraryEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "GPOperatorLibraryEditor";
      this.Size = new System.Drawing.Size(677, 561);
      this.tabControl.ResumeLayout(false);
      this.operatorLibraryTabPage.ResumeLayout(false);
      this.initializationTabPage.ResumeLayout(false);
      this.initSplitContainer.Panel1.ResumeLayout(false);
      this.initSplitContainer.Panel2.ResumeLayout(false);
      this.initSplitContainer.ResumeLayout(false);
      this.mutationTabPage.ResumeLayout(false);
      this.mutationSplitContainer.Panel1.ResumeLayout(false);
      this.mutationSplitContainer.Panel2.ResumeLayout(false);
      this.mutationSplitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.Core.OperatorLibraryEditor operatorLibraryEditor;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage operatorLibraryTabPage;
    private System.Windows.Forms.TabPage initializationTabPage;
    private System.Windows.Forms.TabPage mutationTabPage;
    private System.Windows.Forms.SplitContainer initSplitContainer;
    private System.Windows.Forms.SplitContainer mutationSplitContainer;
    private HeuristicLab.Core.VariableView initVariableView;
    private HeuristicLab.Core.VariableView mutationVariableView;
    private System.Windows.Forms.ListView initListView;
    private System.Windows.Forms.ListView mutationListView;
  }
}
