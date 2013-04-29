#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class InsertNodeDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.nodeSymbolLabel = new System.Windows.Forms.Label();
      this.allowedSymbolsCombo = new System.Windows.Forms.ComboBox();
      this.variableWeightLabel = new System.Windows.Forms.Label();
      this.variableNameLabel = new System.Windows.Forms.Label();
      this.variableNamesCombo = new System.Windows.Forms.ComboBox();
      this.variableWeightTextBox = new System.Windows.Forms.TextBox();
      this.constantValueTextBox = new System.Windows.Forms.TextBox();
      this.constantValueLabel = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // nodeSymbolLabel
      // 
      this.nodeSymbolLabel.AutoSize = true;
      this.nodeSymbolLabel.Location = new System.Drawing.Point(13, 13);
      this.nodeSymbolLabel.Name = "nodeSymbolLabel";
      this.nodeSymbolLabel.Size = new System.Drawing.Size(41, 13);
      this.nodeSymbolLabel.TabIndex = 0;
      this.nodeSymbolLabel.Text = "Symbol";
      // 
      // allowedSymbolsCombo
      // 
      this.allowedSymbolsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.allowedSymbolsCombo.FormattingEnabled = true;
      this.allowedSymbolsCombo.Location = new System.Drawing.Point(120, 10);
      this.allowedSymbolsCombo.Name = "allowedSymbolsCombo";
      this.allowedSymbolsCombo.Size = new System.Drawing.Size(108, 21);
      this.allowedSymbolsCombo.TabIndex = 1;
      this.allowedSymbolsCombo.SelectedIndexChanged += new System.EventHandler(this.allowedSymbolsCombo_SelectedIndexChanged);
      this.allowedSymbolsCombo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.childControl_KeyDown);
      // 
      // variableWeightLabel
      // 
      this.variableWeightLabel.AutoSize = true;
      this.variableWeightLabel.Location = new System.Drawing.Point(13, 44);
      this.variableWeightLabel.Name = "variableWeightLabel";
      this.variableWeightLabel.Size = new System.Drawing.Size(41, 13);
      this.variableWeightLabel.TabIndex = 2;
      this.variableWeightLabel.Text = "Weight";
      this.variableWeightLabel.Visible = false;
      // 
      // variableNameLabel
      // 
      this.variableNameLabel.AutoSize = true;
      this.variableNameLabel.Location = new System.Drawing.Point(13, 74);
      this.variableNameLabel.Name = "variableNameLabel";
      this.variableNameLabel.Size = new System.Drawing.Size(35, 13);
      this.variableNameLabel.TabIndex = 3;
      this.variableNameLabel.Text = "Name";
      this.variableNameLabel.Visible = false;
      // 
      // variableNamesCombo
      // 
      this.variableNamesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.variableNamesCombo.FormattingEnabled = true;
      this.variableNamesCombo.Location = new System.Drawing.Point(120, 71);
      this.variableNamesCombo.Name = "variableNamesCombo";
      this.variableNamesCombo.Size = new System.Drawing.Size(108, 21);
      this.variableNamesCombo.TabIndex = 5;
      this.variableNamesCombo.Visible = false;
      // 
      // variableWeightTextBox
      // 
      this.variableWeightTextBox.Location = new System.Drawing.Point(120, 41);
      this.variableWeightTextBox.Name = "variableWeightTextBox";
      this.variableWeightTextBox.Size = new System.Drawing.Size(108, 20);
      this.variableWeightTextBox.TabIndex = 6;
      this.variableWeightTextBox.Text = "0.0";
      this.variableWeightTextBox.Visible = false;
      this.variableWeightTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.variableWeightTextBox_Validating);
      // 
      // constantValueTextBox
      // 
      this.constantValueTextBox.Location = new System.Drawing.Point(120, 41);
      this.constantValueTextBox.Name = "constantValueTextBox";
      this.constantValueTextBox.Size = new System.Drawing.Size(108, 20);
      this.constantValueTextBox.TabIndex = 7;
      this.constantValueTextBox.Text = "0.0";
      this.constantValueTextBox.Visible = false;
      this.constantValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.constantValueTextBox_Validating);
      // 
      // constantValueLabel
      // 
      this.constantValueLabel.AutoSize = true;
      this.constantValueLabel.Location = new System.Drawing.Point(13, 44);
      this.constantValueLabel.Name = "constantValueLabel";
      this.constantValueLabel.Size = new System.Drawing.Size(34, 13);
      this.constantValueLabel.TabIndex = 8;
      this.constantValueLabel.Text = "Value";
      this.constantValueLabel.Visible = false;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      this.errorProvider.RightToLeft = true;
      // 
      // okButton
      // 
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(12, 112);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 9;
      this.okButton.Text = "OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(153, 112);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 10;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // InsertNodeDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(240, 147);
      this.ControlBox = false;
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.Controls.Add(this.constantValueLabel);
      this.Controls.Add(this.constantValueTextBox);
      this.Controls.Add(this.variableWeightTextBox);
      this.Controls.Add(this.variableNamesCombo);
      this.Controls.Add(this.variableNameLabel);
      this.Controls.Add(this.variableWeightLabel);
      this.Controls.Add(this.allowedSymbolsCombo);
      this.Controls.Add(this.nodeSymbolLabel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "InsertNodeDialog";
      this.Text = "SymbolicExpressionTreeNodeInsertDialog";
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.insertNodeDialog_KeyDown);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label nodeSymbolLabel;
    private System.Windows.Forms.ComboBox allowedSymbolsCombo;
    private System.Windows.Forms.Label variableWeightLabel;
    private System.Windows.Forms.Label variableNameLabel;
    private System.Windows.Forms.Label constantValueLabel;
    private System.Windows.Forms.ErrorProvider errorProvider;
    internal System.Windows.Forms.TextBox constantValueTextBox;
    internal System.Windows.Forms.TextBox variableWeightTextBox;
    internal System.Windows.Forms.ComboBox variableNamesCombo;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button okButton;
  }
}