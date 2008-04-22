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
  partial class StructIdProblemInjectorView {
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
      this.nameLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.importInstanceButton = new System.Windows.Forms.Button();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.dataTabPage = new System.Windows.Forms.TabPage();
      this.variablesLabel = new System.Windows.Forms.Label();
      this.variablesTextBox = new System.Windows.Forms.TextBox();
      this.samplesLabel = new System.Windows.Forms.Label();
      this.samplesTextBox = new System.Windows.Forms.TextBox();
      this.variableInfosTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseVariableInfosView = new HeuristicLab.Core.OperatorBaseVariableInfosView();
      this.descriptionTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseDescriptionView = new HeuristicLab.Core.OperatorBaseDescriptionView();
      this.tabControl.SuspendLayout();
      this.dataTabPage.SuspendLayout();
      this.variableInfosTabPage.SuspendLayout();
      this.descriptionTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameLabel
      // 
      this.nameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(6, 9);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 0;
      this.nameLabel.Text = "Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(62, 6);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.ReadOnly = true;
      this.nameTextBox.Size = new System.Drawing.Size(166, 20);
      this.nameTextBox.TabIndex = 1;
      // 
      // importInstanceButton
      // 
      this.importInstanceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.importInstanceButton.Location = new System.Drawing.Point(6, 105);
      this.importInstanceButton.Name = "importInstanceButton";
      this.importInstanceButton.Size = new System.Drawing.Size(256, 21);
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
      this.tabControl.Controls.Add(this.descriptionTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(276, 184);
      this.tabControl.TabIndex = 0;
      // 
      // dataTabPage
      // 
      this.dataTabPage.Controls.Add(this.variablesLabel);
      this.dataTabPage.Controls.Add(this.variablesTextBox);
      this.dataTabPage.Controls.Add(this.samplesLabel);
      this.dataTabPage.Controls.Add(this.samplesTextBox);
      this.dataTabPage.Controls.Add(this.nameLabel);
      this.dataTabPage.Controls.Add(this.importInstanceButton);
      this.dataTabPage.Controls.Add(this.nameTextBox);
      this.dataTabPage.Location = new System.Drawing.Point(4, 22);
      this.dataTabPage.Name = "dataTabPage";
      this.dataTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.dataTabPage.Size = new System.Drawing.Size(268, 158);
      this.dataTabPage.TabIndex = 0;
      this.dataTabPage.Text = "Data";
      this.dataTabPage.UseVisualStyleBackColor = true;
      // 
      // variablesLabel
      // 
      this.variablesLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.variablesLabel.AutoSize = true;
      this.variablesLabel.Location = new System.Drawing.Point(6, 61);
      this.variablesLabel.Name = "variablesLabel";
      this.variablesLabel.Size = new System.Drawing.Size(53, 13);
      this.variablesLabel.TabIndex = 9;
      this.variablesLabel.Text = "Variables:";
      // 
      // variablesTextBox
      // 
      this.variablesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.variablesTextBox.Location = new System.Drawing.Point(62, 58);
      this.variablesTextBox.Name = "variablesTextBox";
      this.variablesTextBox.ReadOnly = true;
      this.variablesTextBox.Size = new System.Drawing.Size(81, 20);
      this.variablesTextBox.TabIndex = 10;
      // 
      // samplesLabel
      // 
      this.samplesLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.samplesLabel.AutoSize = true;
      this.samplesLabel.Location = new System.Drawing.Point(6, 35);
      this.samplesLabel.Name = "samplesLabel";
      this.samplesLabel.Size = new System.Drawing.Size(50, 13);
      this.samplesLabel.TabIndex = 7;
      this.samplesLabel.Text = "Samples:";
      // 
      // samplesTextBox
      // 
      this.samplesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.samplesTextBox.Location = new System.Drawing.Point(62, 32);
      this.samplesTextBox.Name = "samplesTextBox";
      this.samplesTextBox.ReadOnly = true;
      this.samplesTextBox.Size = new System.Drawing.Size(81, 20);
      this.samplesTextBox.TabIndex = 8;
      // 
      // variableInfosTabPage
      // 
      this.variableInfosTabPage.Controls.Add(this.operatorBaseVariableInfosView);
      this.variableInfosTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableInfosTabPage.Name = "variableInfosTabPage";
      this.variableInfosTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableInfosTabPage.Size = new System.Drawing.Size(268, 158);
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
      this.operatorBaseVariableInfosView.Size = new System.Drawing.Size(262, 152);
      this.operatorBaseVariableInfosView.TabIndex = 0;
      // 
      // descriptionTabPage
      // 
      this.descriptionTabPage.Controls.Add(this.operatorBaseDescriptionView);
      this.descriptionTabPage.Location = new System.Drawing.Point(4, 22);
      this.descriptionTabPage.Name = "descriptionTabPage";
      this.descriptionTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.descriptionTabPage.Size = new System.Drawing.Size(268, 158);
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
      this.operatorBaseDescriptionView.Size = new System.Drawing.Size(262, 152);
      this.operatorBaseDescriptionView.TabIndex = 0;
      // 
      // StructIdProblemInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "StructIdProblemInjectorView";
      this.Size = new System.Drawing.Size(276, 184);
      this.tabControl.ResumeLayout(false);
      this.dataTabPage.ResumeLayout(false);
      this.dataTabPage.PerformLayout();
      this.variableInfosTabPage.ResumeLayout(false);
      this.descriptionTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Button importInstanceButton;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage dataTabPage;
    private System.Windows.Forms.TabPage variableInfosTabPage;
    private HeuristicLab.Core.OperatorBaseVariableInfosView operatorBaseVariableInfosView;
    private System.Windows.Forms.TabPage descriptionTabPage;
    private HeuristicLab.Core.OperatorBaseDescriptionView operatorBaseDescriptionView;
    private System.Windows.Forms.Label variablesLabel;
    private System.Windows.Forms.TextBox variablesTextBox;
    private System.Windows.Forms.Label samplesLabel;
    private System.Windows.Forms.TextBox samplesTextBox;
  }
}
