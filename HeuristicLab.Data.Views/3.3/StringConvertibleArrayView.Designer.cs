#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Data.Views {
  partial class StringConvertibleArrayView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.lengthLabel = new System.Windows.Forms.Label();
      this.lengthTextBox = new System.Windows.Forms.TextBox();
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // lengthLabel
      // 
      this.lengthLabel.AutoSize = true;
      this.lengthLabel.Location = new System.Drawing.Point(3, 3);
      this.lengthLabel.Name = "lengthLabel";
      this.lengthLabel.Size = new System.Drawing.Size(43, 13);
      this.lengthLabel.TabIndex = 0;
      this.lengthLabel.Text = "&Length:";
      // 
      // lengthTextBox
      // 
      this.lengthTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lengthTextBox.Location = new System.Drawing.Point(52, 0);
      this.lengthTextBox.Name = "lengthTextBox";
      this.lengthTextBox.Size = new System.Drawing.Size(372, 20);
      this.lengthTextBox.TabIndex = 1;
      this.lengthTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lengthTextBox_KeyDown);
      this.lengthTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.lengthTextBox_Validating);
      this.lengthTextBox.Validated += new System.EventHandler(this.lengthTextBox_Validated);
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.ColumnHeadersVisible = false;
      this.dataGridView.Location = new System.Drawing.Point(0, 26);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
      this.dataGridView.Size = new System.Drawing.Size(424, 378);
      this.dataGridView.TabIndex = 2;
      this.dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);
      this.dataGridView.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.dataGridView_CellParsing);
      this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
      this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // StringConvertibleArrayView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.dataGridView);
      this.Controls.Add(this.lengthTextBox);
      this.Controls.Add(this.lengthLabel);
      this.Name = "StringConvertibleArrayView";
      this.Size = new System.Drawing.Size(424, 404);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.DataGridView dataGridView;
    protected System.Windows.Forms.Label lengthLabel;
    protected System.Windows.Forms.TextBox lengthTextBox;
    protected System.Windows.Forms.ErrorProvider errorProvider;

  }
}
