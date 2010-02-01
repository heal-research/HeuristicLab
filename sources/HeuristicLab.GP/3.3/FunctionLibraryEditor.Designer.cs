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

namespace HeuristicLab.GP {
  partial class FunctionLibraryEditor {
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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.functionsTabPage = new System.Windows.Forms.TabPage();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.availableFunctionsGroupBox = new System.Windows.Forms.GroupBox();
      this.functionsListView = new System.Windows.Forms.ListView();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.functionDetailsGroupBox = new System.Windows.Forms.GroupBox();
      this.functionDetailsPanel = new System.Windows.Forms.Panel();
      this.functionsComboBox = new System.Windows.Forms.ComboBox();
      this.initializationTabPage = new System.Windows.Forms.TabPage();
      this.initSplitContainer = new System.Windows.Forms.SplitContainer();
      this.initListView = new System.Windows.Forms.ListView();
      this.initVariableView = new HeuristicLab.Core.VariableView();
      this.mutationTabPage = new System.Windows.Forms.TabPage();
      this.mutationSplitContainer = new System.Windows.Forms.SplitContainer();
      this.mutationListView = new System.Windows.Forms.ListView();
      this.mutationVariableView = new HeuristicLab.Core.VariableView();
      this.testTabPage = new System.Windows.Forms.TabPage();
      this.outputTextBox = new System.Windows.Forms.TextBox();
      this.tabControl.SuspendLayout();
      this.functionsTabPage.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.availableFunctionsGroupBox.SuspendLayout();
      this.functionDetailsGroupBox.SuspendLayout();
      this.initializationTabPage.SuspendLayout();
      this.initSplitContainer.Panel1.SuspendLayout();
      this.initSplitContainer.Panel2.SuspendLayout();
      this.initSplitContainer.SuspendLayout();
      this.mutationTabPage.SuspendLayout();
      this.mutationSplitContainer.Panel1.SuspendLayout();
      this.mutationSplitContainer.Panel2.SuspendLayout();
      this.mutationSplitContainer.SuspendLayout();
      this.testTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.functionsTabPage);
      this.tabControl.Controls.Add(this.initializationTabPage);
      this.tabControl.Controls.Add(this.mutationTabPage);
      this.tabControl.Controls.Add(this.testTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(677, 561);
      this.tabControl.TabIndex = 1;
      this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_Selected);
      // 
      // functionsTabPage
      // 
      this.functionsTabPage.Controls.Add(this.splitContainer);
      this.functionsTabPage.Location = new System.Drawing.Point(4, 22);
      this.functionsTabPage.Name = "functionsTabPage";
      this.functionsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.functionsTabPage.Size = new System.Drawing.Size(669, 535);
      this.functionsTabPage.TabIndex = 0;
      this.functionsTabPage.Text = "Functions";
      this.functionsTabPage.UseVisualStyleBackColor = true;
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(3, 3);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.availableFunctionsGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.functionDetailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(663, 529);
      this.splitContainer.SplitterDistance = 230;
      this.splitContainer.TabIndex = 4;
      // 
      // availableFunctionsGroupBox
      // 
      this.availableFunctionsGroupBox.Controls.Add(this.functionsListView);
      this.availableFunctionsGroupBox.Controls.Add(this.removeButton);
      this.availableFunctionsGroupBox.Controls.Add(this.addButton);
      this.availableFunctionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.availableFunctionsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.availableFunctionsGroupBox.Name = "availableFunctionsGroupBox";
      this.availableFunctionsGroupBox.Size = new System.Drawing.Size(230, 529);
      this.availableFunctionsGroupBox.TabIndex = 5;
      this.availableFunctionsGroupBox.TabStop = false;
      this.availableFunctionsGroupBox.Text = "Available functions:";
      // 
      // functionsListView
      // 
      this.functionsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.functionsListView.Location = new System.Drawing.Point(6, 19);
      this.functionsListView.MultiSelect = false;
      this.functionsListView.Name = "functionsListView";
      this.functionsListView.Size = new System.Drawing.Size(218, 475);
      this.functionsListView.TabIndex = 3;
      this.functionsListView.UseCompatibleStateImageBehavior = false;
      this.functionsListView.View = System.Windows.Forms.View.List;
      this.functionsListView.SelectedIndexChanged += new System.EventHandler(this.functionsListView_SelectedIndexChanged);
      this.functionsListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.functionsListView_KeyUp);
      this.functionsListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.functionsListView_ItemDrag);
      // 
      // removeButton
      // 
      this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeButton.Location = new System.Drawing.Point(87, 500);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(75, 23);
      this.removeButton.TabIndex = 2;
      this.removeButton.Text = "Remove";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addButton
      // 
      this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addButton.Location = new System.Drawing.Point(6, 500);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(75, 23);
      this.addButton.TabIndex = 1;
      this.addButton.Text = "Add...";
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // functionDetailsGroupBox
      // 
      this.functionDetailsGroupBox.Controls.Add(this.functionDetailsPanel);
      this.functionDetailsGroupBox.Controls.Add(this.functionsComboBox);
      this.functionDetailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.functionDetailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.functionDetailsGroupBox.Name = "functionDetailsGroupBox";
      this.functionDetailsGroupBox.Size = new System.Drawing.Size(429, 529);
      this.functionDetailsGroupBox.TabIndex = 8;
      this.functionDetailsGroupBox.TabStop = false;
      this.functionDetailsGroupBox.Text = "Function details:";
      // 
      // functionDetailsPanel
      // 
      this.functionDetailsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.functionDetailsPanel.Location = new System.Drawing.Point(6, 46);
      this.functionDetailsPanel.Name = "functionDetailsPanel";
      this.functionDetailsPanel.Size = new System.Drawing.Size(417, 477);
      this.functionDetailsPanel.TabIndex = 7;
      // 
      // functionsComboBox
      // 
      this.functionsComboBox.FormattingEnabled = true;
      this.functionsComboBox.Location = new System.Drawing.Point(6, 19);
      this.functionsComboBox.Name = "functionsComboBox";
      this.functionsComboBox.Size = new System.Drawing.Size(178, 21);
      this.functionsComboBox.TabIndex = 0;
      this.functionsComboBox.SelectedIndexChanged += new System.EventHandler(this.functionsComboBox_SelectedIndexChanged);
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
      // testTabPage
      // 
      this.testTabPage.Controls.Add(this.outputTextBox);
      this.testTabPage.Location = new System.Drawing.Point(4, 22);
      this.testTabPage.Name = "testTabPage";
      this.testTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.testTabPage.Size = new System.Drawing.Size(669, 535);
      this.testTabPage.TabIndex = 3;
      this.testTabPage.Text = "Test function library";
      this.testTabPage.UseVisualStyleBackColor = true;
      // 
      // outputTextBox
      // 
      this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.outputTextBox.Location = new System.Drawing.Point(3, 3);
      this.outputTextBox.Multiline = true;
      this.outputTextBox.Name = "outputTextBox";
      this.outputTextBox.Size = new System.Drawing.Size(663, 529);
      this.outputTextBox.TabIndex = 0;
      // 
      // FunctionLibraryEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "FunctionLibraryEditor";
      this.Size = new System.Drawing.Size(677, 561);
      this.tabControl.ResumeLayout(false);
      this.functionsTabPage.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.availableFunctionsGroupBox.ResumeLayout(false);
      this.functionDetailsGroupBox.ResumeLayout(false);
      this.initializationTabPage.ResumeLayout(false);
      this.initSplitContainer.Panel1.ResumeLayout(false);
      this.initSplitContainer.Panel2.ResumeLayout(false);
      this.initSplitContainer.ResumeLayout(false);
      this.mutationTabPage.ResumeLayout(false);
      this.mutationSplitContainer.Panel1.ResumeLayout(false);
      this.mutationSplitContainer.Panel2.ResumeLayout(false);
      this.mutationSplitContainer.ResumeLayout(false);
      this.testTabPage.ResumeLayout(false);
      this.testTabPage.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage functionsTabPage;
    private System.Windows.Forms.TabPage initializationTabPage;
    private System.Windows.Forms.TabPage mutationTabPage;
    private System.Windows.Forms.SplitContainer initSplitContainer;
    private System.Windows.Forms.SplitContainer mutationSplitContainer;
    private HeuristicLab.Core.VariableView initVariableView;
    private HeuristicLab.Core.VariableView mutationVariableView;
    private System.Windows.Forms.ListView initListView;
    private System.Windows.Forms.ListView mutationListView;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.Button addButton;
    private System.Windows.Forms.ListView functionsListView;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Panel functionDetailsPanel;
    private System.Windows.Forms.ComboBox functionsComboBox;
    private System.Windows.Forms.GroupBox availableFunctionsGroupBox;
    private System.Windows.Forms.GroupBox functionDetailsGroupBox;
    private System.Windows.Forms.TabPage testTabPage;
    private System.Windows.Forms.TextBox outputTextBox;
  }
}
