#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Analysis.Statistics.Views {
  partial class ChartAnalysisView {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.fittingComboBox = new System.Windows.Forms.ComboBox();
      this.addValuesButton = new System.Windows.Forms.Button();
      this.addLineToChart = new System.Windows.Forms.Button();
      this.dataRowComboBox = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.dataTableComboBox = new System.Windows.Forms.ComboBox();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.stringConvertibleMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // fittingComboBox
      // 
      this.fittingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.fittingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.fittingComboBox.FormattingEnabled = true;
      this.fittingComboBox.Location = new System.Drawing.Point(76, 3);
      this.fittingComboBox.Name = "fittingComboBox";
      this.fittingComboBox.Size = new System.Drawing.Size(214, 21);
      this.fittingComboBox.TabIndex = 12;
      // 
      // addValuesButton
      // 
      this.addValuesButton.Location = new System.Drawing.Point(3, 3);
      this.addValuesButton.Name = "addValuesButton";
      this.addValuesButton.Size = new System.Drawing.Size(133, 23);
      this.addValuesButton.TabIndex = 10;
      this.addValuesButton.Text = "Add Values to Results";
      this.addValuesButton.UseVisualStyleBackColor = true;
      this.addValuesButton.Click += new System.EventHandler(this.addValuesButton_Click);
      // 
      // addLineToChart
      // 
      this.addLineToChart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.addLineToChart.Location = new System.Drawing.Point(296, 3);
      this.addLineToChart.Name = "addLineToChart";
      this.addLineToChart.Size = new System.Drawing.Size(100, 23);
      this.addLineToChart.TabIndex = 9;
      this.addLineToChart.Text = "Fit Line to Chart";
      this.addLineToChart.UseVisualStyleBackColor = true;
      this.addLineToChart.Click += new System.EventHandler(this.addLineToChart_Click);
      // 
      // dataRowComboBox
      // 
      this.dataRowComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataRowComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.dataRowComboBox.FormattingEnabled = true;
      this.dataRowComboBox.Location = new System.Drawing.Point(69, 36);
      this.dataRowComboBox.Name = "dataRowComboBox";
      this.dataRowComboBox.Size = new System.Drawing.Size(540, 21);
      this.dataRowComboBox.TabIndex = 8;
      this.dataRowComboBox.SelectedIndexChanged += new System.EventHandler(this.dataRowComboBox_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 39);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "DataRow:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(60, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "DataTable:";
      // 
      // dataTableComboBox
      // 
      this.dataTableComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTableComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.dataTableComboBox.FormattingEnabled = true;
      this.dataTableComboBox.Location = new System.Drawing.Point(69, 6);
      this.dataTableComboBox.Name = "dataTableComboBox";
      this.dataTableComboBox.Size = new System.Drawing.Size(540, 21);
      this.dataTableComboBox.TabIndex = 5;
      this.dataTableComboBox.SelectedIndexChanged += new System.EventHandler(this.dataTableComboBox_SelectedIndexChanged);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point(6, 391);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.addValuesButton);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.fittingComboBox);
      this.splitContainer1.Panel2.Controls.Add(this.addLineToChart);
      this.splitContainer1.Size = new System.Drawing.Size(603, 29);
      this.splitContainer1.SplitterDistance = 200;
      this.splitContainer1.TabIndex = 13;
      // 
      // stringConvertibleMatrixView
      // 
      this.stringConvertibleMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stringConvertibleMatrixView.Caption = "StringConvertibleMatrix View";
      this.stringConvertibleMatrixView.Content = null;
      this.stringConvertibleMatrixView.Location = new System.Drawing.Point(2, 63);
      this.stringConvertibleMatrixView.Name = "stringConvertibleMatrixView";
      this.stringConvertibleMatrixView.ReadOnly = true;
      this.stringConvertibleMatrixView.ShowRowsAndColumnsTextBox = true;
      this.stringConvertibleMatrixView.ShowStatisticalInformation = true;
      this.stringConvertibleMatrixView.Size = new System.Drawing.Size(607, 322);
      this.stringConvertibleMatrixView.TabIndex = 0;
      // 
      // ChartAnalysisView
      // 
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.stringConvertibleMatrixView);
      this.Controls.Add(this.dataTableComboBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.dataRowComboBox);
      this.Controls.Add(this.label2);
      this.Name = "ChartAnalysisView";
      this.Size = new System.Drawing.Size(612, 423);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private HeuristicLab.Data.Views.StringConvertibleMatrixView stringConvertibleMatrixView;
    private System.Windows.Forms.ComboBox dataRowComboBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox dataTableComboBox;
    private System.Windows.Forms.Button addLineToChart;
    private System.Windows.Forms.Button addValuesButton;
    private System.Windows.Forms.ComboBox fittingComboBox;
    private System.Windows.Forms.SplitContainer splitContainer1;
  }
}
