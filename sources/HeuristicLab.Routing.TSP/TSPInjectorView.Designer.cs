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

namespace HeuristicLab.Routing.TSP {
  partial class TSPInjectorView {
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
      this.citiesLabel = new System.Windows.Forms.Label();
      this.bestKnownQualityTextBox = new System.Windows.Forms.TextBox();
      this.bestKnownQualityAvailableCheckBox = new System.Windows.Forms.CheckBox();
      this.citiesTextBox = new System.Windows.Forms.TextBox();
      this.importInstanceButton = new System.Windows.Forms.Button();
      this.bestKnownQualityLabel = new System.Windows.Forms.Label();
      this.bestKnownQualityAvailableLabel = new System.Windows.Forms.Label();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.dataTabPage = new System.Windows.Forms.TabPage();
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
      // citiesLabel
      // 
      this.citiesLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.citiesLabel.AutoSize = true;
      this.citiesLabel.Location = new System.Drawing.Point(6, 36);
      this.citiesLabel.Name = "citiesLabel";
      this.citiesLabel.Size = new System.Drawing.Size(35, 13);
      this.citiesLabel.TabIndex = 0;
      this.citiesLabel.Text = "&Cities:";
      // 
      // bestKnownQualityTextBox
      // 
      this.bestKnownQualityTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.bestKnownQualityTextBox.Location = new System.Drawing.Point(166, 79);
      this.bestKnownQualityTextBox.Name = "bestKnownQualityTextBox";
      this.bestKnownQualityTextBox.Size = new System.Drawing.Size(96, 20);
      this.bestKnownQualityTextBox.TabIndex = 5;
      this.bestKnownQualityTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.bestKnownQualityTextBox_Validating);
      // 
      // bestKnownQualityAvailableCheckBox
      // 
      this.bestKnownQualityAvailableCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.bestKnownQualityAvailableCheckBox.AutoSize = true;
      this.bestKnownQualityAvailableCheckBox.Location = new System.Drawing.Point(166, 59);
      this.bestKnownQualityAvailableCheckBox.Name = "bestKnownQualityAvailableCheckBox";
      this.bestKnownQualityAvailableCheckBox.Size = new System.Drawing.Size(15, 14);
      this.bestKnownQualityAvailableCheckBox.TabIndex = 3;
      this.bestKnownQualityAvailableCheckBox.UseVisualStyleBackColor = true;
      this.bestKnownQualityAvailableCheckBox.CheckedChanged += new System.EventHandler(this.bestKnownQualityAvailableCheckBox_CheckedChanged);
      // 
      // citiesTextBox
      // 
      this.citiesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.citiesTextBox.Location = new System.Drawing.Point(166, 33);
      this.citiesTextBox.Name = "citiesTextBox";
      this.citiesTextBox.ReadOnly = true;
      this.citiesTextBox.Size = new System.Drawing.Size(96, 20);
      this.citiesTextBox.TabIndex = 1;
      // 
      // importInstanceButton
      // 
      this.importInstanceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.importInstanceButton.Location = new System.Drawing.Point(6, 105);
      this.importInstanceButton.Name = "importInstanceButton";
      this.importInstanceButton.Size = new System.Drawing.Size(256, 21);
      this.importInstanceButton.TabIndex = 6;
      this.importInstanceButton.Text = "&Import Instance from TSPLib";
      this.importInstanceButton.UseVisualStyleBackColor = true;
      this.importInstanceButton.Click += new System.EventHandler(this.importInstanceButton_Click);
      // 
      // bestKnownQualityLabel
      // 
      this.bestKnownQualityLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.bestKnownQualityLabel.AutoSize = true;
      this.bestKnownQualityLabel.Location = new System.Drawing.Point(6, 82);
      this.bestKnownQualityLabel.Name = "bestKnownQualityLabel";
      this.bestKnownQualityLabel.Size = new System.Drawing.Size(102, 13);
      this.bestKnownQualityLabel.TabIndex = 4;
      this.bestKnownQualityLabel.Text = "&Best Known Quality:";
      // 
      // bestKnownQualityAvailableLabel
      // 
      this.bestKnownQualityAvailableLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.bestKnownQualityAvailableLabel.AutoSize = true;
      this.bestKnownQualityAvailableLabel.Location = new System.Drawing.Point(6, 59);
      this.bestKnownQualityAvailableLabel.Name = "bestKnownQualityAvailableLabel";
      this.bestKnownQualityAvailableLabel.Size = new System.Drawing.Size(148, 13);
      this.bestKnownQualityAvailableLabel.TabIndex = 2;
      this.bestKnownQualityAvailableLabel.Text = "Best Known Quality &Available:";
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "tsp";
      this.openFileDialog.FileName = "tsp";
      this.openFileDialog.Filter = "TSPLib files|*.tsp|All files|*.*";
      this.openFileDialog.Title = "Import TSPLib File";
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
      this.dataTabPage.Controls.Add(this.citiesLabel);
      this.dataTabPage.Controls.Add(this.bestKnownQualityAvailableLabel);
      this.dataTabPage.Controls.Add(this.bestKnownQualityTextBox);
      this.dataTabPage.Controls.Add(this.bestKnownQualityLabel);
      this.dataTabPage.Controls.Add(this.bestKnownQualityAvailableCheckBox);
      this.dataTabPage.Controls.Add(this.importInstanceButton);
      this.dataTabPage.Controls.Add(this.citiesTextBox);
      this.dataTabPage.Location = new System.Drawing.Point(4, 22);
      this.dataTabPage.Name = "dataTabPage";
      this.dataTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.dataTabPage.Size = new System.Drawing.Size(268, 158);
      this.dataTabPage.TabIndex = 0;
      this.dataTabPage.Text = "TSP Data";
      this.dataTabPage.UseVisualStyleBackColor = true;
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
      // TSPInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "TSPInjectorView";
      this.Size = new System.Drawing.Size(276, 184);
      this.tabControl.ResumeLayout(false);
      this.dataTabPage.ResumeLayout(false);
      this.dataTabPage.PerformLayout();
      this.variableInfosTabPage.ResumeLayout(false);
      this.descriptionTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label citiesLabel;
    private System.Windows.Forms.TextBox bestKnownQualityTextBox;
    private System.Windows.Forms.CheckBox bestKnownQualityAvailableCheckBox;
    private System.Windows.Forms.TextBox citiesTextBox;
    private System.Windows.Forms.Button importInstanceButton;
    private System.Windows.Forms.Label bestKnownQualityLabel;
    private System.Windows.Forms.Label bestKnownQualityAvailableLabel;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage dataTabPage;
    private System.Windows.Forms.TabPage variableInfosTabPage;
    private HeuristicLab.Core.OperatorBaseVariableInfosView operatorBaseVariableInfosView;
    private System.Windows.Forms.TabPage descriptionTabPage;
    private HeuristicLab.Core.OperatorBaseDescriptionView operatorBaseDescriptionView;
  }
}
