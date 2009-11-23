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
using System.Windows.Forms;

namespace HeuristicLab.Core.Views {
  partial class OperatorBaseView {
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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.variableInfosTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseVariableInfosView = new HeuristicLab.Core.Views.OperatorBaseVariableInfosView();
      this.variablesTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseVariablesView = new HeuristicLab.Core.Views.OperatorBaseVariablesView();
      this.descriptionTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseDescriptionView = new HeuristicLab.Core.Views.OperatorBaseDescriptionView();
      this.tabControl.SuspendLayout();
      this.variableInfosTabPage.SuspendLayout();
      this.variablesTabPage.SuspendLayout();
      this.descriptionTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.variableInfosTabPage);
      this.tabControl.Controls.Add(this.variablesTabPage);
      this.tabControl.Controls.Add(this.descriptionTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(423, 333);
      this.tabControl.TabIndex = 0;
      // 
      // variableInfosTabPage
      // 
      this.variableInfosTabPage.Controls.Add(this.operatorBaseVariableInfosView);
      this.variableInfosTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableInfosTabPage.Name = "variableInfosTabPage";
      this.variableInfosTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableInfosTabPage.Size = new System.Drawing.Size(415, 307);
      this.variableInfosTabPage.TabIndex = 0;
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
      this.operatorBaseVariableInfosView.Size = new System.Drawing.Size(409, 301);
      this.operatorBaseVariableInfosView.TabIndex = 0;
      // 
      // variablesTabPage
      // 
      this.variablesTabPage.Controls.Add(this.operatorBaseVariablesView);
      this.variablesTabPage.Location = new System.Drawing.Point(4, 22);
      this.variablesTabPage.Name = "variablesTabPage";
      this.variablesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variablesTabPage.Size = new System.Drawing.Size(415, 307);
      this.variablesTabPage.TabIndex = 1;
      this.variablesTabPage.Text = "Local Variables";
      this.variablesTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseVariablesView
      // 
      this.operatorBaseVariablesView.Caption = "Operator";
      this.operatorBaseVariablesView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorBaseVariablesView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseVariablesView.Name = "operatorBaseVariablesView";
      this.operatorBaseVariablesView.Operator = null;
      this.operatorBaseVariablesView.Size = new System.Drawing.Size(409, 301);
      this.operatorBaseVariablesView.TabIndex = 0;
      // 
      // descriptionTabPage
      // 
      this.descriptionTabPage.Controls.Add(this.operatorBaseDescriptionView);
      this.descriptionTabPage.Location = new System.Drawing.Point(4, 22);
      this.descriptionTabPage.Name = "descriptionTabPage";
      this.descriptionTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.descriptionTabPage.Size = new System.Drawing.Size(415, 307);
      this.descriptionTabPage.TabIndex = 3;
      this.descriptionTabPage.Text = "Description";
      this.descriptionTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseDescriptionView
      // 
      this.operatorBaseDescriptionView.Caption = "Operator";
      this.operatorBaseDescriptionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorBaseDescriptionView.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.operatorBaseDescriptionView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseDescriptionView.Name = "operatorBaseDescriptionView";
      this.operatorBaseDescriptionView.Operator = null;
      this.operatorBaseDescriptionView.Size = new System.Drawing.Size(409, 301);
      this.operatorBaseDescriptionView.TabIndex = 0;
      // 
      // OperatorBaseView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "OperatorBaseView";
      this.Size = new System.Drawing.Size(423, 333);
      this.tabControl.ResumeLayout(false);
      this.variableInfosTabPage.ResumeLayout(false);
      this.variablesTabPage.ResumeLayout(false);
      this.descriptionTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected TabControl tabControl;
    protected TabPage variableInfosTabPage;
    protected TabPage variablesTabPage;
    protected OperatorBaseVariableInfosView operatorBaseVariableInfosView;
    protected OperatorBaseVariablesView operatorBaseVariablesView;
    private TabPage descriptionTabPage;
    private OperatorBaseDescriptionView operatorBaseDescriptionView;


  }
}
