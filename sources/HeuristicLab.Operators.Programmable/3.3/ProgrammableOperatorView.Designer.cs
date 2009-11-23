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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgrammableOperatorView));
      this.tabControl = new System.Windows.Forms.TabControl();
      this.codeTabPage = new System.Windows.Forms.TabPage();
      this.infoTextBox = new System.Windows.Forms.TextBox();
      this.compileButton = new System.Windows.Forms.Button();
      this.codeTextBox = new System.Windows.Forms.TextBox();
      this.variableInfosTabPage = new System.Windows.Forms.TabPage();
      this.removeVariableInfoButton = new System.Windows.Forms.Button();
      this.addVariableInfoButton = new System.Windows.Forms.Button();
      this.operatorBaseVariableInfosView = new HeuristicLab.Core.Views.OperatorBaseVariableInfosView();
      this.variablesTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseVariablesView = new HeuristicLab.Core.Views.OperatorBaseVariablesView();
      this.descriptionTabPage = new System.Windows.Forms.TabPage();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.tabControl.SuspendLayout();
      this.codeTabPage.SuspendLayout();
      this.variableInfosTabPage.SuspendLayout();
      this.variablesTabPage.SuspendLayout();
      this.descriptionTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.codeTabPage);
      this.tabControl.Controls.Add(this.variableInfosTabPage);
      this.tabControl.Controls.Add(this.variablesTabPage);
      this.tabControl.Controls.Add(this.descriptionTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(435, 351);
      this.tabControl.TabIndex = 0;
      // 
      // codeTabPage
      // 
      this.codeTabPage.Controls.Add(this.infoTextBox);
      this.codeTabPage.Controls.Add(this.compileButton);
      this.codeTabPage.Controls.Add(this.codeTextBox);
      this.codeTabPage.Location = new System.Drawing.Point(4, 22);
      this.codeTabPage.Name = "codeTabPage";
      this.codeTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.codeTabPage.Size = new System.Drawing.Size(427, 325);
      this.codeTabPage.TabIndex = 5;
      this.codeTabPage.Text = "Code";
      this.codeTabPage.UseVisualStyleBackColor = true;
      // 
      // infoTextBox
      // 
      this.infoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.infoTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
      this.infoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.infoTextBox.Location = new System.Drawing.Point(6, 253);
      this.infoTextBox.Multiline = true;
      this.infoTextBox.Name = "infoTextBox";
      this.infoTextBox.ReadOnly = true;
      this.infoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.infoTextBox.Size = new System.Drawing.Size(334, 66);
      this.infoTextBox.TabIndex = 2;
      this.infoTextBox.Text = resources.GetString("infoTextBox.Text");
      // 
      // compileButton
      // 
      this.compileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.compileButton.Location = new System.Drawing.Point(346, 296);
      this.compileButton.Name = "compileButton";
      this.compileButton.Size = new System.Drawing.Size(75, 23);
      this.compileButton.TabIndex = 1;
      this.compileButton.Text = "&Compile";
      this.compileButton.UseVisualStyleBackColor = true;
      this.compileButton.Click += new System.EventHandler(this.compileButton_Click);
      // 
      // codeTextBox
      // 
      this.codeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.codeTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.codeTextBox.Location = new System.Drawing.Point(3, 3);
      this.codeTextBox.Multiline = true;
      this.codeTextBox.Name = "codeTextBox";
      this.codeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.codeTextBox.Size = new System.Drawing.Size(421, 244);
      this.codeTextBox.TabIndex = 0;
      this.codeTextBox.Validated += new System.EventHandler(this.codeTextBox_Validated);
      // 
      // variableInfosTabPage
      // 
      this.variableInfosTabPage.Controls.Add(this.removeVariableInfoButton);
      this.variableInfosTabPage.Controls.Add(this.addVariableInfoButton);
      this.variableInfosTabPage.Controls.Add(this.operatorBaseVariableInfosView);
      this.variableInfosTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableInfosTabPage.Name = "variableInfosTabPage";
      this.variableInfosTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableInfosTabPage.Size = new System.Drawing.Size(427, 325);
      this.variableInfosTabPage.TabIndex = 1;
      this.variableInfosTabPage.Text = "Variable Infos";
      this.variableInfosTabPage.UseVisualStyleBackColor = true;
      // 
      // removeVariableInfoButton
      // 
      this.removeVariableInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeVariableInfoButton.Enabled = false;
      this.removeVariableInfoButton.Location = new System.Drawing.Point(84, 299);
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
      this.addVariableInfoButton.Location = new System.Drawing.Point(3, 299);
      this.addVariableInfoButton.Name = "addVariableInfoButton";
      this.addVariableInfoButton.Size = new System.Drawing.Size(75, 23);
      this.addVariableInfoButton.TabIndex = 1;
      this.addVariableInfoButton.Text = "&Add...";
      this.addVariableInfoButton.UseVisualStyleBackColor = true;
      this.addVariableInfoButton.Click += new System.EventHandler(this.addVariableInfoButton_Click);
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
      this.operatorBaseVariableInfosView.Size = new System.Drawing.Size(421, 290);
      this.operatorBaseVariableInfosView.TabIndex = 0;
      this.operatorBaseVariableInfosView.SelectedVariableInfosChanged += new System.EventHandler(this.operatorBaseVariableInfosView_SelectedVariableInfosChanged);
      // 
      // variablesTabPage
      // 
      this.variablesTabPage.Controls.Add(this.operatorBaseVariablesView);
      this.variablesTabPage.Location = new System.Drawing.Point(4, 22);
      this.variablesTabPage.Name = "variablesTabPage";
      this.variablesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variablesTabPage.Size = new System.Drawing.Size(427, 325);
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
      this.operatorBaseVariablesView.Size = new System.Drawing.Size(421, 319);
      this.operatorBaseVariablesView.TabIndex = 0;
      // 
      // descriptionTabPage
      // 
      this.descriptionTabPage.Controls.Add(this.descriptionTextBox);
      this.descriptionTabPage.Location = new System.Drawing.Point(4, 22);
      this.descriptionTabPage.Name = "descriptionTabPage";
      this.descriptionTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.descriptionTabPage.Size = new System.Drawing.Size(427, 325);
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
      this.descriptionTextBox.Size = new System.Drawing.Size(421, 319);
      this.descriptionTextBox.TabIndex = 0;
      this.descriptionTextBox.Validated += new System.EventHandler(this.descriptionTextBox_Validated);
      // 
      // ProgrammableOperatorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "ProgrammableOperatorView";
      this.Size = new System.Drawing.Size(435, 351);
      this.tabControl.ResumeLayout(false);
      this.codeTabPage.ResumeLayout(false);
      this.codeTabPage.PerformLayout();
      this.variableInfosTabPage.ResumeLayout(false);
      this.variablesTabPage.ResumeLayout(false);
      this.descriptionTabPage.ResumeLayout(false);
      this.descriptionTabPage.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage codeTabPage;
    private System.Windows.Forms.TextBox codeTextBox;
    private System.Windows.Forms.TabPage variableInfosTabPage;
    private HeuristicLab.Core.Views.OperatorBaseVariableInfosView operatorBaseVariableInfosView;
    private System.Windows.Forms.TabPage variablesTabPage;
    private HeuristicLab.Core.Views.OperatorBaseVariablesView operatorBaseVariablesView;
    private System.Windows.Forms.TabPage descriptionTabPage;
    private System.Windows.Forms.TextBox descriptionTextBox;
    private System.Windows.Forms.Button compileButton;
    private System.Windows.Forms.Button removeVariableInfoButton;
    private System.Windows.Forms.Button addVariableInfoButton;
    private System.Windows.Forms.TextBox infoTextBox;

  }
}
