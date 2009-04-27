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

namespace HeuristicLab.Operators {
  partial class CombinedOperatorView {
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
      this.operatorGraphTabPage = new System.Windows.Forms.TabPage();
      this.operatorGraphView = new HeuristicLab.Core.OperatorGraphView();
      this.variableInfosTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseVariableInfosView = new HeuristicLab.Core.OperatorBaseVariableInfosView();
      this.variablesTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseVariablesView = new HeuristicLab.Core.OperatorBaseVariablesView();
      this.constraintsTabPage = new System.Windows.Forms.TabPage();
      this.constrainedItemBaseView = new HeuristicLab.Core.ConstrainedItemBaseView();
      this.descriptionTabPage = new System.Windows.Forms.TabPage();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.removeVariableInfoButton = new System.Windows.Forms.Button();
      this.addVariableInfoButton = new System.Windows.Forms.Button();
      this.tabControl.SuspendLayout();
      this.operatorGraphTabPage.SuspendLayout();
      this.variableInfosTabPage.SuspendLayout();
      this.variablesTabPage.SuspendLayout();
      this.constraintsTabPage.SuspendLayout();
      this.descriptionTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.operatorGraphTabPage);
      this.tabControl.Controls.Add(this.variableInfosTabPage);
      this.tabControl.Controls.Add(this.variablesTabPage);
      this.tabControl.Controls.Add(this.constraintsTabPage);
      this.tabControl.Controls.Add(this.descriptionTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(397, 335);
      this.tabControl.TabIndex = 0;
      // 
      // operatorGraphTabPage
      // 
      this.operatorGraphTabPage.Controls.Add(this.operatorGraphView);
      this.operatorGraphTabPage.Location = new System.Drawing.Point(4, 22);
      this.operatorGraphTabPage.Name = "operatorGraphTabPage";
      this.operatorGraphTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.operatorGraphTabPage.Size = new System.Drawing.Size(389, 309);
      this.operatorGraphTabPage.TabIndex = 0;
      this.operatorGraphTabPage.Text = "Operator Graph";
      this.operatorGraphTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorGraphView
      // 
      this.operatorGraphView.Caption = "Operator Graph";
      this.operatorGraphView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorGraphView.Location = new System.Drawing.Point(3, 3);
      this.operatorGraphView.Name = "operatorGraphView";
      this.operatorGraphView.OperatorGraph = null;
      this.operatorGraphView.Size = new System.Drawing.Size(383, 303);
      this.operatorGraphView.TabIndex = 0;
      // 
      // variableInfosTabPage
      // 
      this.variableInfosTabPage.Controls.Add(this.removeVariableInfoButton);
      this.variableInfosTabPage.Controls.Add(this.addVariableInfoButton);
      this.variableInfosTabPage.Controls.Add(this.operatorBaseVariableInfosView);
      this.variableInfosTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableInfosTabPage.Name = "variableInfosTabPage";
      this.variableInfosTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableInfosTabPage.Size = new System.Drawing.Size(389, 309);
      this.variableInfosTabPage.TabIndex = 1;
      this.variableInfosTabPage.Text = "Variable Infos";
      this.variableInfosTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseVariableInfosView
      // 
      this.operatorBaseVariableInfosView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorBaseVariableInfosView.Caption = "Operator";
      this.operatorBaseVariableInfosView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseVariableInfosView.Name = "operatorBaseVariableInfosView";
      this.operatorBaseVariableInfosView.Operator = null;
      this.operatorBaseVariableInfosView.Size = new System.Drawing.Size(383, 274);
      this.operatorBaseVariableInfosView.TabIndex = 0;
      this.operatorBaseVariableInfosView.SelectedVariableInfosChanged += new System.EventHandler(this.operatorBaseVariableInfosView_SelectedVariableInfosChanged);
      // 
      // variablesTabPage
      // 
      this.variablesTabPage.Controls.Add(this.operatorBaseVariablesView);
      this.variablesTabPage.Location = new System.Drawing.Point(4, 22);
      this.variablesTabPage.Name = "variablesTabPage";
      this.variablesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variablesTabPage.Size = new System.Drawing.Size(389, 309);
      this.variablesTabPage.TabIndex = 2;
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
      this.operatorBaseVariablesView.Size = new System.Drawing.Size(383, 303);
      this.operatorBaseVariablesView.TabIndex = 0;
      // 
      // constraintsTabPage
      // 
      this.constraintsTabPage.Controls.Add(this.constrainedItemBaseView);
      this.constraintsTabPage.Location = new System.Drawing.Point(4, 22);
      this.constraintsTabPage.Name = "constraintsTabPage";
      this.constraintsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.constraintsTabPage.Size = new System.Drawing.Size(389, 309);
      this.constraintsTabPage.TabIndex = 3;
      this.constraintsTabPage.Text = "Constraints";
      this.constraintsTabPage.UseVisualStyleBackColor = true;
      // 
      // constrainedItemBaseView
      // 
      this.constrainedItemBaseView.Caption = "Constrained Item";
      this.constrainedItemBaseView.ConstrainedItem = null;
      this.constrainedItemBaseView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.constrainedItemBaseView.Location = new System.Drawing.Point(3, 3);
      this.constrainedItemBaseView.Name = "constrainedItemBaseView";
      this.constrainedItemBaseView.Size = new System.Drawing.Size(383, 303);
      this.constrainedItemBaseView.TabIndex = 0;
      // 
      // descriptionTabPage
      // 
      this.descriptionTabPage.Controls.Add(this.descriptionTextBox);
      this.descriptionTabPage.Location = new System.Drawing.Point(4, 22);
      this.descriptionTabPage.Name = "descriptionTabPage";
      this.descriptionTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.descriptionTabPage.Size = new System.Drawing.Size(389, 309);
      this.descriptionTabPage.TabIndex = 4;
      this.descriptionTabPage.Text = "Description";
      this.descriptionTabPage.UseVisualStyleBackColor = true;
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.descriptionTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.descriptionTextBox.Location = new System.Drawing.Point(3, 3);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.descriptionTextBox.Size = new System.Drawing.Size(383, 303);
      this.descriptionTextBox.TabIndex = 0;
      this.descriptionTextBox.Validated += new System.EventHandler(this.descriptionTextBox_Validated);
      // 
      // removeVariableInfoButton
      // 
      this.removeVariableInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeVariableInfoButton.Enabled = false;
      this.removeVariableInfoButton.Location = new System.Drawing.Point(84, 283);
      this.removeVariableInfoButton.Name = "removeVariableInfoButton";
      this.removeVariableInfoButton.Size = new System.Drawing.Size(75, 23);
      this.removeVariableInfoButton.TabIndex = 2;
      this.removeVariableInfoButton.Text = "&Remove";
      this.removeVariableInfoButton.UseVisualStyleBackColor = true;
      this.removeVariableInfoButton.Click += new System.EventHandler(this.removeVariableInfoButton_Click);
      // 
      // addVariableInfoButton
      // 
      this.addVariableInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addVariableInfoButton.Location = new System.Drawing.Point(3, 283);
      this.addVariableInfoButton.Name = "addVariableInfoButton";
      this.addVariableInfoButton.Size = new System.Drawing.Size(75, 23);
      this.addVariableInfoButton.TabIndex = 1;
      this.addVariableInfoButton.Text = "&Add...";
      this.addVariableInfoButton.UseVisualStyleBackColor = true;
      this.addVariableInfoButton.Click += new System.EventHandler(this.addVariableInfoButton_Click);
      // 
      // CombinedOperatorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "CombinedOperatorView";
      this.Size = new System.Drawing.Size(397, 335);
      this.tabControl.ResumeLayout(false);
      this.operatorGraphTabPage.ResumeLayout(false);
      this.variableInfosTabPage.ResumeLayout(false);
      this.variablesTabPage.ResumeLayout(false);
      this.constraintsTabPage.ResumeLayout(false);
      this.descriptionTabPage.ResumeLayout(false);
      this.descriptionTabPage.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage operatorGraphTabPage;
    private System.Windows.Forms.TabPage variableInfosTabPage;
    private System.Windows.Forms.TabPage variablesTabPage;
    private System.Windows.Forms.TabPage constraintsTabPage;
    private System.Windows.Forms.TabPage descriptionTabPage;
    private System.Windows.Forms.TextBox descriptionTextBox;
    private HeuristicLab.Core.OperatorGraphView operatorGraphView;
    private HeuristicLab.Core.OperatorBaseVariableInfosView operatorBaseVariableInfosView;
    private HeuristicLab.Core.OperatorBaseVariablesView operatorBaseVariablesView;
    private HeuristicLab.Core.ConstrainedItemBaseView constrainedItemBaseView;
    private System.Windows.Forms.Button removeVariableInfoButton;
    private System.Windows.Forms.Button addVariableInfoButton;

  }
}
