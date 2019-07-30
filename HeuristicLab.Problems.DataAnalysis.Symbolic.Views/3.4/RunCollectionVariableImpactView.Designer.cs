#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class RunCollectionVariableImpactView {
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
      this.matrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.comboBox = new System.Windows.Forms.ComboBox();
      this.foldsLabel = new System.Windows.Forms.Label();
      this.variableImpactsGroupBox = new System.Windows.Forms.GroupBox();
      this.variableImpactsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // matrixView
      // 
      this.matrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.matrixView.Caption = "StringConvertibleMatrix View";
      this.matrixView.Content = null;
      this.matrixView.Location = new System.Drawing.Point(6, 19);
      this.matrixView.Name = "matrixView";
      this.matrixView.ReadOnly = true;
      this.matrixView.ShowRowsAndColumnsTextBox = true;
      this.matrixView.ShowStatisticalInformation = true;
      this.matrixView.Size = new System.Drawing.Size(294, 174);
      this.matrixView.TabIndex = 0;
      // 
      // comboBox
      // 
      this.comboBox.FormattingEnabled = true;
      this.comboBox.Location = new System.Drawing.Point(39, 6);
      this.comboBox.Name = "comboBox";
      this.comboBox.Size = new System.Drawing.Size(68, 21);
      this.comboBox.TabIndex = 1;
      this.comboBox.SelectedValueChanged += new System.EventHandler(this.comboBox_SelectedValueChanged);
      // 
      // label1
      // 
      this.foldsLabel.AutoSize = true;
      this.foldsLabel.Location = new System.Drawing.Point(3, 9);
      this.foldsLabel.Name = "foldsLabel";
      this.foldsLabel.Size = new System.Drawing.Size(30, 13);
      this.foldsLabel.TabIndex = 2;
      this.foldsLabel.Text = "Fold:";
      // 
      // variableImpactsGroupBox
      // 
      this.variableImpactsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.variableImpactsGroupBox.Controls.Add(this.matrixView);
      this.variableImpactsGroupBox.Location = new System.Drawing.Point(0, 33);
      this.variableImpactsGroupBox.Name = "variableImpactsGroupBox";
      this.variableImpactsGroupBox.Size = new System.Drawing.Size(306, 199);
      this.variableImpactsGroupBox.TabIndex = 3;
      this.variableImpactsGroupBox.TabStop = false;
      this.variableImpactsGroupBox.Text = "Variable impacts:";
      // 
      // RunCollectionVariableImpactView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.variableImpactsGroupBox);
      this.Controls.Add(this.foldsLabel);
      this.Controls.Add(this.comboBox);
      this.Name = "RunCollectionVariableImpactView";
      this.Size = new System.Drawing.Size(309, 235);
      this.variableImpactsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Data.Views.StringConvertibleMatrixView matrixView;
    private System.Windows.Forms.ComboBox comboBox;
    private System.Windows.Forms.Label foldsLabel;
    private System.Windows.Forms.GroupBox variableImpactsGroupBox;
  }
}
