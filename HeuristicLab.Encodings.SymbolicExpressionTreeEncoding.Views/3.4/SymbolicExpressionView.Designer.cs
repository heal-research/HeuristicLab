#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class SymbolicExpressionView {
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
      this.textBox = new System.Windows.Forms.TextBox();
      this.formattersComboBox = new System.Windows.Forms.ComboBox();
      this.formatterLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // textBox
      // 
      this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox.BackColor = System.Drawing.Color.White;
      this.textBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox.Location = new System.Drawing.Point(3, 30);
      this.textBox.Multiline = true;
      this.textBox.Name = "textBox";
      this.textBox.ReadOnly = true;
      this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.textBox.Size = new System.Drawing.Size(475, 282);
      this.textBox.TabIndex = 0;
      // 
      // formattersComboBox
      // 
      this.formattersComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.formattersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.formattersComboBox.FormattingEnabled = true;
      this.formattersComboBox.Location = new System.Drawing.Point(72, 3);
      this.formattersComboBox.Name = "formattersComboBox";
      this.formattersComboBox.Size = new System.Drawing.Size(406, 21);
      this.formattersComboBox.TabIndex = 1;
      this.formattersComboBox.SelectedIndexChanged += new System.EventHandler(this.formattersComboBox_SelectedIndexChanged);
      // 
      // formatterLabel
      // 
      this.formatterLabel.AutoSize = true;
      this.formatterLabel.Location = new System.Drawing.Point(3, 6);
      this.formatterLabel.Name = "formatterLabel";
      this.formatterLabel.Size = new System.Drawing.Size(54, 13);
      this.formatterLabel.TabIndex = 2;
      this.formatterLabel.Text = "Formatter:";
      // 
      // SymbolicExpressionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.formatterLabel);
      this.Controls.Add(this.formattersComboBox);
      this.Controls.Add(this.textBox);
      this.Name = "SymbolicExpressionView";
      this.Size = new System.Drawing.Size(481, 315);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox textBox;
    private System.Windows.Forms.ComboBox formattersComboBox;
    private System.Windows.Forms.Label formatterLabel;
  }
}
