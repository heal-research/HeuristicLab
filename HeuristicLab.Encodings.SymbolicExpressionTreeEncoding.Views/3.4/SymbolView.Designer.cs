#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  partial class SymbolView {
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
      this.initialFrequencyLabel = new System.Windows.Forms.Label();
      this.initialFrequencyTextBox = new System.Windows.Forms.TextBox();
      this.enabledCheckBox = new System.Windows.Forms.CheckBox();
      this.minimumArityLabel = new System.Windows.Forms.Label();
      this.maximumArityLabel = new System.Windows.Forms.Label();
      this.minimumArityTextBox = new System.Windows.Forms.TextBox();
      this.maximumArityTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(93, 0);
      this.nameTextBox.Size = new System.Drawing.Size(202, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(301, 3);
      // 
      // initialFrequencyLabel
      // 
      this.initialFrequencyLabel.AutoSize = true;
      this.initialFrequencyLabel.Location = new System.Drawing.Point(3, 29);
      this.initialFrequencyLabel.Name = "initialFrequencyLabel";
      this.initialFrequencyLabel.Size = new System.Drawing.Size(84, 13);
      this.initialFrequencyLabel.TabIndex = 3;
      this.initialFrequencyLabel.Text = "Initial frequency:";
      this.toolTip.SetToolTip(this.initialFrequencyLabel, "Relative frequency of the symbol in randomly created trees");
      // 
      // initialFrequencyTextBox
      // 
      this.initialFrequencyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.initialFrequencyTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.initialFrequencyTextBox.Location = new System.Drawing.Point(93, 26);
      this.initialFrequencyTextBox.Name = "initialFrequencyTextBox";
      this.initialFrequencyTextBox.Size = new System.Drawing.Size(227, 20);
      this.initialFrequencyTextBox.TabIndex = 4;
      this.initialFrequencyTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.initialFrequencyTextBox_KeyDown);
      this.initialFrequencyTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.initialFrequencyTextBox_Validating);
      this.initialFrequencyTextBox.Validated += new System.EventHandler(this.initialFrequencyTextBox_Validated);
      // 
      // enabledCheckBox
      // 
      this.enabledCheckBox.AutoSize = true;
      this.enabledCheckBox.Location = new System.Drawing.Point(93, 104);
      this.enabledCheckBox.Name = "enabledCheckBox";
      this.enabledCheckBox.Size = new System.Drawing.Size(65, 17);
      this.enabledCheckBox.TabIndex = 5;
      this.enabledCheckBox.Text = "Enabled";
      this.enabledCheckBox.UseVisualStyleBackColor = true;
      this.enabledCheckBox.CheckedChanged += new System.EventHandler(this.checkBoxEnabled_CheckedChanged);
      // 
      // minimumArityLabel
      // 
      this.minimumArityLabel.AutoSize = true;
      this.minimumArityLabel.Location = new System.Drawing.Point(3, 55);
      this.minimumArityLabel.Name = "minimumArityLabel";
      this.minimumArityLabel.Size = new System.Drawing.Size(74, 13);
      this.minimumArityLabel.TabIndex = 8;
      this.minimumArityLabel.Text = "Minimum Arity:";
      this.toolTip.SetToolTip(this.minimumArityLabel, "The minimum arity of the symbol");
      // 
      // maximumArityLabel
      // 
      this.maximumArityLabel.AutoSize = true;
      this.maximumArityLabel.Location = new System.Drawing.Point(3, 81);
      this.maximumArityLabel.Name = "maximumArityLabel";
      this.maximumArityLabel.Size = new System.Drawing.Size(74, 13);
      this.maximumArityLabel.TabIndex = 6;
      this.maximumArityLabel.Text = "Maximum Arity";
      this.toolTip.SetToolTip(this.maximumArityLabel, "The maximum arity of the symbol");
      // 
      // minimumArityTextBox
      // 
      this.minimumArityTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.minimumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.minimumArityTextBox.Location = new System.Drawing.Point(93, 52);
      this.minimumArityTextBox.Name = "minimumArityTextBox";
      this.minimumArityTextBox.ReadOnly = true;
      this.minimumArityTextBox.Size = new System.Drawing.Size(227, 20);
      this.minimumArityTextBox.TabIndex = 9;
      // 
      // maximumArityTextBox
      // 
      this.maximumArityTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.maximumArityTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.maximumArityTextBox.Location = new System.Drawing.Point(93, 78);
      this.maximumArityTextBox.Name = "maximumArityTextBox";
      this.maximumArityTextBox.ReadOnly = true;
      this.maximumArityTextBox.Size = new System.Drawing.Size(227, 20);
      this.maximumArityTextBox.TabIndex = 7;
      // 
      // SymbolView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.minimumArityTextBox);
      this.Controls.Add(this.minimumArityLabel);
      this.Controls.Add(this.maximumArityTextBox);
      this.Controls.Add(this.maximumArityLabel);
      this.Controls.Add(this.initialFrequencyTextBox);
      this.Controls.Add(this.initialFrequencyLabel);
      this.Controls.Add(this.enabledCheckBox);
      this.Name = "SymbolView";
      this.Size = new System.Drawing.Size(320, 123);
      this.Controls.SetChildIndex(this.enabledCheckBox, 0);
      this.Controls.SetChildIndex(this.initialFrequencyLabel, 0);
      this.Controls.SetChildIndex(this.initialFrequencyTextBox, 0);
      this.Controls.SetChildIndex(this.maximumArityLabel, 0);
      this.Controls.SetChildIndex(this.maximumArityTextBox, 0);
      this.Controls.SetChildIndex(this.minimumArityLabel, 0);
      this.Controls.SetChildIndex(this.minimumArityTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label initialFrequencyLabel;
    protected System.Windows.Forms.TextBox initialFrequencyTextBox;
    protected System.Windows.Forms.CheckBox enabledCheckBox;
    protected System.Windows.Forms.Label minimumArityLabel;
    protected System.Windows.Forms.Label maximumArityLabel;
    protected System.Windows.Forms.TextBox minimumArityTextBox;
    protected System.Windows.Forms.TextBox maximumArityTextBox;

  }
}
