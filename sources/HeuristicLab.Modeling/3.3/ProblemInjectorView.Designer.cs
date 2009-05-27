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

namespace HeuristicLab.Modeling {
  partial class ProblemInjectorView {
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
      this.importInstanceButton = new System.Windows.Forms.Button();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.dataTabPage = new System.Windows.Forms.TabPage();
      this.datasetView = new HeuristicLab.DataAnalysis.DatasetView();
      this.variableInfosTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseVariableInfosView = new HeuristicLab.Core.OperatorBaseVariableInfosView();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.variablesView = new HeuristicLab.Core.OperatorBaseVariablesView();
      this.descriptionTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseDescriptionView = new HeuristicLab.Core.OperatorBaseDescriptionView();
      this.tabControl.SuspendLayout();
      this.dataTabPage.SuspendLayout();
      this.variableInfosTabPage.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.descriptionTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // importInstanceButton
      // 
      this.importInstanceButton.Location = new System.Drawing.Point(6, 6);
      this.importInstanceButton.Name = "importInstanceButton";
      this.importInstanceButton.Size = new System.Drawing.Size(129, 21);
      this.importInstanceButton.TabIndex = 6;
      this.importInstanceButton.Text = "&Import...";
      this.importInstanceButton.UseVisualStyleBackColor = true;
      this.importInstanceButton.Click += new System.EventHandler(this.importInstanceButton_Click);
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "txt";
      this.openFileDialog.FileName = "txt";
      this.openFileDialog.Filter = "Text files|*.txt|All files|*.*";
      this.openFileDialog.Title = "Import StructId File";
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.dataTabPage);
      this.tabControl.Controls.Add(this.variableInfosTabPage);
      this.tabControl.Controls.Add(this.tabPage1);
      this.tabControl.Controls.Add(this.descriptionTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(507, 451);
      this.tabControl.TabIndex = 0;
      // 
      // dataTabPage
      // 
      this.dataTabPage.Controls.Add(this.datasetView);
      this.dataTabPage.Controls.Add(this.importInstanceButton);
      this.dataTabPage.Location = new System.Drawing.Point(4, 22);
      this.dataTabPage.Name = "dataTabPage";
      this.dataTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.dataTabPage.Size = new System.Drawing.Size(499, 425);
      this.dataTabPage.TabIndex = 0;
      this.dataTabPage.Text = "Data";
      this.dataTabPage.UseVisualStyleBackColor = true;
      // 
      // datasetView
      // 
      this.datasetView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.datasetView.Caption = "Editor";
      this.datasetView.Dataset = null;
      this.datasetView.Filename = null;
      this.datasetView.Location = new System.Drawing.Point(6, 33);
      this.datasetView.Name = "datasetView";
      this.datasetView.Size = new System.Drawing.Size(487, 386);
      this.datasetView.TabIndex = 7;
      // 
      // variableInfosTabPage
      // 
      this.variableInfosTabPage.Controls.Add(this.operatorBaseVariableInfosView);
      this.variableInfosTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableInfosTabPage.Name = "variableInfosTabPage";
      this.variableInfosTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableInfosTabPage.Size = new System.Drawing.Size(499, 425);
      this.variableInfosTabPage.TabIndex = 1;
      this.variableInfosTabPage.Text = "Variable Infos";
      this.variableInfosTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseVariableInfosView
      // 
      this.operatorBaseVariableInfosView.Caption = "Operator";
      this.operatorBaseVariableInfosView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorBaseVariableInfosView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseVariableInfosView.Name = "operatorBaseVariableInfosView";
      this.operatorBaseVariableInfosView.Operator = null;
      this.operatorBaseVariableInfosView.Size = new System.Drawing.Size(493, 419);
      this.operatorBaseVariableInfosView.TabIndex = 0;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.variablesView);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(499, 425);
      this.tabPage1.TabIndex = 3;
      this.tabPage1.Text = "Variables";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // variablesView
      // 
      this.variablesView.Caption = "Operator";
      this.variablesView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variablesView.Location = new System.Drawing.Point(3, 3);
      this.variablesView.Name = "variablesView";
      this.variablesView.Operator = null;
      this.variablesView.Size = new System.Drawing.Size(493, 419);
      this.variablesView.TabIndex = 0;
      // 
      // descriptionTabPage
      // 
      this.descriptionTabPage.Controls.Add(this.operatorBaseDescriptionView);
      this.descriptionTabPage.Location = new System.Drawing.Point(4, 22);
      this.descriptionTabPage.Name = "descriptionTabPage";
      this.descriptionTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.descriptionTabPage.Size = new System.Drawing.Size(499, 425);
      this.descriptionTabPage.TabIndex = 2;
      this.descriptionTabPage.Text = "Description";
      this.descriptionTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseDescriptionView
      // 
      this.operatorBaseDescriptionView.Caption = "Operator";
      this.operatorBaseDescriptionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorBaseDescriptionView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseDescriptionView.Name = "operatorBaseDescriptionView";
      this.operatorBaseDescriptionView.Operator = null;
      this.operatorBaseDescriptionView.Size = new System.Drawing.Size(493, 419);
      this.operatorBaseDescriptionView.TabIndex = 0;
      // 
      // StructIdProblemInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "StructIdProblemInjectorView";
      this.Size = new System.Drawing.Size(507, 451);
      this.tabControl.ResumeLayout(false);
      this.dataTabPage.ResumeLayout(false);
      this.variableInfosTabPage.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.descriptionTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button importInstanceButton;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage dataTabPage;
    private System.Windows.Forms.TabPage variableInfosTabPage;
    private HeuristicLab.Core.OperatorBaseVariableInfosView operatorBaseVariableInfosView;
    private System.Windows.Forms.TabPage descriptionTabPage;
    private HeuristicLab.Core.OperatorBaseDescriptionView operatorBaseDescriptionView;
    private System.Windows.Forms.TabPage tabPage1;
    private HeuristicLab.Core.OperatorBaseVariablesView variablesView;
    private HeuristicLab.DataAnalysis.DatasetView datasetView;
  }
}
