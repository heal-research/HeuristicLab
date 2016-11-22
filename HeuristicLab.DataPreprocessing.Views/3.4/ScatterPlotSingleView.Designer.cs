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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class ScatterPlotSingleView {
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
      this.scatterPlotView = new HeuristicLab.DataPreprocessing.Views.PreprocessingScatterPlotView();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.comboBoxYVariable = new System.Windows.Forms.ComboBox();
      this.comboBoxXVariable = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.comboBoxColor = new System.Windows.Forms.ComboBox();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // scatterPlotView
      // 
      this.scatterPlotView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scatterPlotView.Caption = "ScatterPlot View";
      this.scatterPlotView.Content = null;
      this.scatterPlotView.Location = new System.Drawing.Point(169, 3);
      this.scatterPlotView.Name = "scatterPlotView";
      this.scatterPlotView.ReadOnly = false;
      this.scatterPlotView.Size = new System.Drawing.Size(689, 509);
      this.scatterPlotView.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.comboBoxColor);
      this.groupBox1.Controls.Add(this.comboBoxYVariable);
      this.groupBox1.Controls.Add(this.comboBoxXVariable);
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(160, 215);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Options";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(17, 80);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Y Variable";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(17, 25);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(55, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "X Variable";
      // 
      // comboBoxYVariable
      // 
      this.comboBoxYVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxYVariable.FormattingEnabled = true;
      this.comboBoxYVariable.Location = new System.Drawing.Point(20, 103);
      this.comboBoxYVariable.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
      this.comboBoxYVariable.Name = "comboBoxYVariable";
      this.comboBoxYVariable.Size = new System.Drawing.Size(121, 21);
      this.comboBoxYVariable.TabIndex = 1;
      this.comboBoxYVariable.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
      // 
      // comboBoxXVariable
      // 
      this.comboBoxXVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxXVariable.FormattingEnabled = true;
      this.comboBoxXVariable.Location = new System.Drawing.Point(20, 48);
      this.comboBoxXVariable.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
      this.comboBoxXVariable.Name = "comboBoxXVariable";
      this.comboBoxXVariable.Size = new System.Drawing.Size(121, 21);
      this.comboBoxXVariable.TabIndex = 0;
      this.comboBoxXVariable.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(17, 141);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(31, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Color";
      // 
      // comboBoxColor
      // 
      this.comboBoxColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxColor.FormattingEnabled = true;
      this.comboBoxColor.Location = new System.Drawing.Point(20, 164);
      this.comboBoxColor.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
      this.comboBoxColor.Name = "comboBoxColor";
      this.comboBoxColor.Size = new System.Drawing.Size(121, 21);
      this.comboBoxColor.TabIndex = 1;
      this.comboBoxColor.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
      // 
      // ScatterPlotSingleView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.scatterPlotView);
      this.Name = "ScatterPlotSingleView";
      this.Size = new System.Drawing.Size(863, 517);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.DataPreprocessing.Views.PreprocessingScatterPlotView scatterPlotView;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox comboBoxYVariable;
    private System.Windows.Forms.ComboBox comboBoxXVariable;
    private System.Windows.Forms.ComboBox comboBoxColor;
    private System.Windows.Forms.Label label3;
  }
}
