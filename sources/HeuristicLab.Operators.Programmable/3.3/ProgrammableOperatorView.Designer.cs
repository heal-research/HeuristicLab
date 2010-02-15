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

namespace HeuristicLab.Operators.Programmable {
  partial class ProgrammableOperatorView {
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
      System.Windows.Forms.SplitContainer splitContainer1;
      System.Windows.Forms.SplitContainer splitContainer2;
      System.Windows.Forms.GroupBox groupBox1;
      System.Windows.Forms.GroupBox groupBox2;
      this.assembliesTreeView = new System.Windows.Forms.TreeView();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.showCodeButton = new System.Windows.Forms.Button();
      this.codeEditor = new HeuristicLab.CodeEditor.CodeEditor();
      this.compileButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.codeTabPage = new System.Windows.Forms.TabPage();
      this.variableInfosTabPage = new System.Windows.Forms.TabPage();
      this.removeVariableInfoButton = new System.Windows.Forms.Button();
      this.addVariableInfoButton = new System.Windows.Forms.Button();
      //this.operatorBaseVariableInfosView = new HeuristicLab.Core.OperatorBaseVariableInfosView();
      this.variablesTabPage = new System.Windows.Forms.TabPage();
      //this.operatorBaseVariablesView = new HeuristicLab.Core.OperatorBaseVariablesView();
      this.constraintsTabPage = new System.Windows.Forms.TabPage();
      //this.constrainedItemBaseView = new HeuristicLab.Core.ConstrainedItemBaseView();
      this.descriptionTabPage = new System.Windows.Forms.TabPage();
      //this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.namespacesTreeView = new System.Windows.Forms.TreeView();
      splitContainer1 = new System.Windows.Forms.SplitContainer();
      splitContainer2 = new System.Windows.Forms.SplitContainer();
      groupBox1 = new System.Windows.Forms.GroupBox();
      groupBox2 = new System.Windows.Forms.GroupBox();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      splitContainer2.Panel1.SuspendLayout();
      splitContainer2.Panel2.SuspendLayout();
      splitContainer2.SuspendLayout();
      groupBox1.SuspendLayout();
      groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.codeTabPage.SuspendLayout();
      this.variableInfosTabPage.SuspendLayout();
      this.variablesTabPage.SuspendLayout();
      this.constraintsTabPage.SuspendLayout();
      this.descriptionTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer1.Location = new System.Drawing.Point(3, 3);
      splitContainer1.Name = "splitContainer1";
      splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(splitContainer2);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(this.groupBox3);
      splitContainer1.Size = new System.Drawing.Size(693, 457);
      splitContainer1.SplitterDistance = 85;
      splitContainer1.TabIndex = 6;
      // 
      // splitContainer2
      // 
      splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer2.Location = new System.Drawing.Point(0, 0);
      splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      splitContainer2.Panel1.Controls.Add(groupBox1);
      // 
      // splitContainer2.Panel2
      // 
      splitContainer2.Panel2.Controls.Add(groupBox2);
      splitContainer2.Size = new System.Drawing.Size(693, 85);
      splitContainer2.SplitterDistance = 340;
      splitContainer2.TabIndex = 2;
      // 
      // groupBox1
      // 
      groupBox1.Controls.Add(this.assembliesTreeView);
      groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      groupBox1.Location = new System.Drawing.Point(0, 0);
      groupBox1.Name = "groupBox1";
      groupBox1.Size = new System.Drawing.Size(340, 85);
      groupBox1.TabIndex = 0;
      groupBox1.TabStop = false;
      groupBox1.Text = "Assemblies";
      // 
      // assembliesTreeView
      // 
      this.assembliesTreeView.CheckBoxes = true;
      this.assembliesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.assembliesTreeView.Location = new System.Drawing.Point(3, 16);
      this.assembliesTreeView.Name = "assembliesTreeView";
      this.assembliesTreeView.Size = new System.Drawing.Size(334, 66);
      this.assembliesTreeView.TabIndex = 1;
      this.assembliesTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.assembliesTreeView_AfterCheck);
      // 
      // groupBox2
      // 
      groupBox2.Controls.Add(this.namespacesTreeView);
      groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      groupBox2.Location = new System.Drawing.Point(0, 0);
      groupBox2.Name = "groupBox2";
      groupBox2.Size = new System.Drawing.Size(349, 85);
      groupBox2.TabIndex = 0;
      groupBox2.TabStop = false;
      groupBox2.Text = "Namespaces";
      // 
      // namespacesTreeView
      // 
      this.namespacesTreeView.CheckBoxes = true;
      this.namespacesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.namespacesTreeView.Location = new System.Drawing.Point(3, 16);
      this.namespacesTreeView.Name = "namespacesTreeView";
      this.namespacesTreeView.PathSeparator = ".";
      this.namespacesTreeView.Size = new System.Drawing.Size(343, 66);
      this.namespacesTreeView.TabIndex = 2;
      this.namespacesTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.namespacesTreeView_AfterCheck);
      // 
      // groupBox3
      // 
      this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox3.Controls.Add(this.showCodeButton);
      this.groupBox3.Controls.Add(this.codeEditor);
      this.groupBox3.Controls.Add(this.compileButton);
      this.groupBox3.Location = new System.Drawing.Point(3, 2);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(687, 363);
      this.groupBox3.TabIndex = 7;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Code";
      // 
      // showCodeButton
      // 
      this.showCodeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.showCodeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.showCodeButton.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ControlDark;
      this.showCodeButton.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientActiveCaption;
      this.showCodeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.showCodeButton.Location = new System.Drawing.Point(467, 333);
      this.showCodeButton.Name = "showCodeButton";
      this.showCodeButton.Size = new System.Drawing.Size(133, 23);
      this.showCodeButton.TabIndex = 4;
      this.showCodeButton.Text = "Show Generated Code";
      this.showCodeButton.UseVisualStyleBackColor = true;
      this.showCodeButton.Click += new System.EventHandler(this.showCodeButton_Click);
      // 
      // compileButton
      // 
      this.compileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.compileButton.Location = new System.Drawing.Point(606, 334);
      this.compileButton.Name = "compileButton";
      this.compileButton.Size = new System.Drawing.Size(75, 23);
      this.compileButton.TabIndex = 3;
      this.compileButton.Text = "&Compile";
      this.compileButton.UseVisualStyleBackColor = true;
      this.compileButton.Click += new System.EventHandler(this.compileButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.codeTabPage);
      this.tabControl.Controls.Add(this.variableInfosTabPage);
      this.tabControl.Controls.Add(this.variablesTabPage);
      this.tabControl.Controls.Add(this.constraintsTabPage);
      this.tabControl.Controls.Add(this.descriptionTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(707, 489);
      this.tabControl.TabIndex = 0;
      // 
      // codeTabPage
      // 
      this.codeTabPage.Controls.Add(splitContainer1);
      this.codeTabPage.Location = new System.Drawing.Point(4, 22);
      this.codeTabPage.Name = "codeTabPage";
      this.codeTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.codeTabPage.Size = new System.Drawing.Size(699, 463);
      this.codeTabPage.TabIndex = 5;
      this.codeTabPage.Text = "Code";
      this.codeTabPage.UseVisualStyleBackColor = true;
      // 
      // variableInfosTabPage
      // 
      this.variableInfosTabPage.Controls.Add(this.removeVariableInfoButton);
      this.variableInfosTabPage.Controls.Add(this.addVariableInfoButton);
      //this.variableInfosTabPage.Controls.Add(this.operatorBaseVariableInfosView);
      this.variableInfosTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableInfosTabPage.Name = "variableInfosTabPage";
      this.variableInfosTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableInfosTabPage.Size = new System.Drawing.Size(699, 463);
      this.variableInfosTabPage.TabIndex = 1;
      this.variableInfosTabPage.Text = "Variable Infos";
      this.variableInfosTabPage.UseVisualStyleBackColor = true;
      // 
      // removeVariableInfoButton
      // 
      this.removeVariableInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeVariableInfoButton.Enabled = false;
      this.removeVariableInfoButton.Location = new System.Drawing.Point(87, 434);
      this.removeVariableInfoButton.Name = "removeVariableInfoButton";
      this.removeVariableInfoButton.Size = new System.Drawing.Size(75, 23);
      this.removeVariableInfoButton.TabIndex = 2;
      this.removeVariableInfoButton.Text = "&Remove";
      this.removeVariableInfoButton.UseVisualStyleBackColor = true;
      //this.removeVariableInfoButton.Click += new System.EventHandler(this.removeVariableInfoButton_Click);
      // 
      // addVariableInfoButton
      // 
      this.addVariableInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addVariableInfoButton.Location = new System.Drawing.Point(6, 434);
      this.addVariableInfoButton.Name = "addVariableInfoButton";
      this.addVariableInfoButton.Size = new System.Drawing.Size(75, 23);
      this.addVariableInfoButton.TabIndex = 1;
      this.addVariableInfoButton.Text = "&Add...";
      this.addVariableInfoButton.UseVisualStyleBackColor = true;
      ///this.addVariableInfoButton.Click += new System.EventHandler(this.addVariableInfoButton_Click);
      // 
      // operatorBaseVariableInfosView
      // 
      /*this.operatorBaseVariableInfosView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorBaseVariableInfosView.Caption = "Operator";
      this.operatorBaseVariableInfosView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseVariableInfosView.Name = "operatorBaseVariableInfosView";
      this.operatorBaseVariableInfosView.Operator = null;
      this.operatorBaseVariableInfosView.Size = new System.Drawing.Size(690, 425);
      this.operatorBaseVariableInfosView.TabIndex = 0;
      this.operatorBaseVariableInfosView.SelectedVariableInfosChanged += new System.EventHandler(this.operatorBaseVariableInfosView_SelectedVariableInfosChanged);
       */
      // 
      // variablesTabPage
      // 
      //this.variablesTabPage.Controls.Add(this.operatorBaseVariablesView);
      this.variablesTabPage.Location = new System.Drawing.Point(4, 22);
      this.variablesTabPage.Name = "variablesTabPage";
      this.variablesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variablesTabPage.Size = new System.Drawing.Size(699, 463);
      this.variablesTabPage.TabIndex = 2;
      this.variablesTabPage.Text = "Local Variables";
      this.variablesTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseVariablesView
      // 
      /*this.operatorBaseVariablesView.Caption = "Operator";
      this.operatorBaseVariablesView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorBaseVariablesView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseVariablesView.Name = "operatorBaseVariablesView";
      this.operatorBaseVariablesView.Operator = null;
      this.operatorBaseVariablesView.Size = new System.Drawing.Size(693, 457);
      this.operatorBaseVariablesView.TabIndex = 0; */
      // 
      // constraintsTabPage
      // 
      //this.constraintsTabPage.Controls.Add(this.constrainedItemBaseView);
      this.constraintsTabPage.Location = new System.Drawing.Point(4, 22);
      this.constraintsTabPage.Name = "constraintsTabPage";
      this.constraintsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.constraintsTabPage.Size = new System.Drawing.Size(699, 463);
      this.constraintsTabPage.TabIndex = 3;
      this.constraintsTabPage.Text = "Constraints";
      this.constraintsTabPage.UseVisualStyleBackColor = true;
      // 
      // constrainedItemBaseView
      // 
      /*this.constrainedItemBaseView.Caption = "Constrained Item";
      this.constrainedItemBaseView.ConstrainedItem = null;
      this.constrainedItemBaseView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.constrainedItemBaseView.Location = new System.Drawing.Point(3, 3);
      this.constrainedItemBaseView.Name = "constrainedItemBaseView";
      this.constrainedItemBaseView.Size = new System.Drawing.Size(693, 457);
      this.constrainedItemBaseView.TabIndex = 0; */
      // 
      // descriptionTabPage
      // 
      this.descriptionTabPage.Controls.Add(this.descriptionTextBox);
      this.descriptionTabPage.Location = new System.Drawing.Point(4, 22);
      this.descriptionTabPage.Name = "descriptionTabPage";
      this.descriptionTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.descriptionTabPage.Size = new System.Drawing.Size(699, 463);
      this.descriptionTabPage.TabIndex = 4;
      this.descriptionTabPage.Text = "Description";
      this.descriptionTabPage.UseVisualStyleBackColor = true;
      // 
      // descriptionTextBox
      // 
      /*this.descriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.descriptionTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.descriptionTextBox.Location = new System.Drawing.Point(3, 3);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.descriptionTextBox.Size = new System.Drawing.Size(693, 457);
      this.descriptionTextBox.TabIndex = 0;
      this.descriptionTextBox.Validated += new System.EventHandler(this.descriptionTextBox_Validated); */
      // 
      // codeEditor
      // 
      this.codeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.codeEditor.Location = new System.Drawing.Point(6, 19);
      this.codeEditor.Name = "codeEditor";
      this.codeEditor.Prefix = "using System;\r\nusing HeuristicLab.Common.Resources;\r\n\r\npublic class Operator {\r\n " +
          " public static void Apply(int arg) {";
      this.codeEditor.Size = new System.Drawing.Size(675, 309);
      this.codeEditor.Suffix = "\n    return null;\n  }\n}";
      this.codeEditor.TabIndex = 0;
      this.codeEditor.UserCode = "\n\n\n";
      this.codeEditor.TextEditorValidated += new System.EventHandler(this.codeEditor_Validated);
      // 
      // ProgrammableOperatorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "ProgrammableOperatorView";
      this.Size = new System.Drawing.Size(707, 489);
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel2.ResumeLayout(false);
      splitContainer1.ResumeLayout(false);
      splitContainer2.Panel1.ResumeLayout(false);
      splitContainer2.Panel2.ResumeLayout(false);
      splitContainer2.ResumeLayout(false);
      groupBox1.ResumeLayout(false);
      groupBox2.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.codeTabPage.ResumeLayout(false);
      this.variableInfosTabPage.ResumeLayout(false);
      this.variablesTabPage.ResumeLayout(false);
      this.constraintsTabPage.ResumeLayout(false);
      this.descriptionTabPage.ResumeLayout(false);
      this.descriptionTabPage.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage codeTabPage;
    private System.Windows.Forms.TabPage variableInfosTabPage;
    //private HeuristicLab.Core.OperatorBaseVariableInfosView operatorBaseVariableInfosView;
    private System.Windows.Forms.TabPage variablesTabPage;
    //private HeuristicLab.Core.OperatorBaseVariablesView operatorBaseVariablesView;
    private System.Windows.Forms.TabPage constraintsTabPage;
    //private HeuristicLab.Core.ConstrainedItemBaseView constrainedItemBaseView;
    private System.Windows.Forms.TabPage descriptionTabPage;
    private System.Windows.Forms.TextBox descriptionTextBox;
    private System.Windows.Forms.Button removeVariableInfoButton;
    private System.Windows.Forms.Button addVariableInfoButton;
    private System.Windows.Forms.TreeView assembliesTreeView;
    private System.Windows.Forms.TreeView namespacesTreeView;
    private System.Windows.Forms.GroupBox groupBox3;
    private HeuristicLab.CodeEditor.CodeEditor codeEditor;
    private System.Windows.Forms.Button compileButton;
    private System.Windows.Forms.Button showCodeButton;


  }
}
