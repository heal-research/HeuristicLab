#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class ComparisonFilterView {
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
      this.components = new System.ComponentModel.Container();
      this.lblAttr = new System.Windows.Forms.Label();
      this.lblFilterOperation = new System.Windows.Forms.Label();
      this.lblFilterData = new System.Windows.Forms.Label();
      this.cbAttr = new System.Windows.Forms.ComboBox();
      this.cbFilterOperation = new System.Windows.Forms.ComboBox();
      this.tbFilterData = new System.Windows.Forms.TextBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // lblAttr
      // 
      this.lblAttr.AutoSize = true;
      this.lblAttr.Location = new System.Drawing.Point(3, 15);
      this.lblAttr.Name = "lblAttr";
      this.lblAttr.Size = new System.Drawing.Size(49, 13);
      this.lblAttr.TabIndex = 0;
      this.lblAttr.Text = "Attribute:";
      // 
      // lblFilterOperation
      // 
      this.lblFilterOperation.AutoSize = true;
      this.lblFilterOperation.Location = new System.Drawing.Point(3, 42);
      this.lblFilterOperation.Name = "lblFilterOperation";
      this.lblFilterOperation.Size = new System.Drawing.Size(81, 13);
      this.lblFilterOperation.TabIndex = 1;
      this.lblFilterOperation.Text = "Filter Operation:";
      // 
      // lblFilterData
      // 
      this.lblFilterData.AutoSize = true;
      this.lblFilterData.Location = new System.Drawing.Point(3, 69);
      this.lblFilterData.Name = "lblFilterData";
      this.lblFilterData.Size = new System.Drawing.Size(58, 13);
      this.lblFilterData.TabIndex = 2;
      this.lblFilterData.Text = "Filter Data:";
      // 
      // cbAttr
      // 
      this.cbAttr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cbAttr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbAttr.FormattingEnabled = true;
      this.cbAttr.Location = new System.Drawing.Point(92, 12);
      this.cbAttr.Name = "cbAttr";
      this.cbAttr.Size = new System.Drawing.Size(235, 21);
      this.cbAttr.TabIndex = 3;
      this.cbAttr.SelectedIndexChanged += new System.EventHandler(this.cbAttr_SelectedIndexChanged);
      // 
      // cbFilterOperation
      // 
      this.cbFilterOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cbFilterOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbFilterOperation.FormattingEnabled = true;
      this.cbFilterOperation.Location = new System.Drawing.Point(92, 39);
      this.cbFilterOperation.Name = "cbFilterOperation";
      this.cbFilterOperation.Size = new System.Drawing.Size(235, 21);
      this.cbFilterOperation.TabIndex = 4;
      this.cbFilterOperation.SelectedIndexChanged += new System.EventHandler(this.cbFilterOperation_SelectedIndexChanged);
      // 
      // tbFilterData
      // 
      this.tbFilterData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbFilterData.Location = new System.Drawing.Point(92, 67);
      this.tbFilterData.Name = "tbFilterData";
      this.tbFilterData.Size = new System.Drawing.Size(235, 20);
      this.tbFilterData.TabIndex = 5;
      this.tbFilterData.Validating += new System.ComponentModel.CancelEventHandler(this.tbFilterData_Validating);
      this.tbFilterData.Validated += new System.EventHandler(this.tbFilterData_Validated);
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // ComparisonFilterView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tbFilterData);
      this.Controls.Add(this.cbFilterOperation);
      this.Controls.Add(this.cbAttr);
      this.Controls.Add(this.lblFilterData);
      this.Controls.Add(this.lblFilterOperation);
      this.Controls.Add(this.lblAttr);
      this.Name = "ComparisonFilterView";
      this.Size = new System.Drawing.Size(348, 97);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblAttr;
    private System.Windows.Forms.Label lblFilterOperation;
    private System.Windows.Forms.Label lblFilterData;
    private System.Windows.Forms.ComboBox cbAttr;
    private System.Windows.Forms.ComboBox cbFilterOperation;
    private System.Windows.Forms.TextBox tbFilterData;
    private System.Windows.Forms.ErrorProvider errorProvider;

  }
}
