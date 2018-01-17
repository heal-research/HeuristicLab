#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class PreprocessingFeatureCorrelationView {
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
      this.checkInputsTargetButton = new System.Windows.Forms.Button();
      this.uncheckAllButton = new System.Windows.Forms.Button();
      this.checkAllButton = new System.Windows.Forms.Button();
      this.correlationView = new HeuristicLab.Problems.DataAnalysis.Views.FeatureCorrelationView();
      this.variablesLabel = new System.Windows.Forms.Label();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // checkInputsTargetButton
      // 
      this.checkInputsTargetButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.Inputs;
      this.checkInputsTargetButton.Location = new System.Drawing.Point(736, 5);
      this.checkInputsTargetButton.Name = "checkInputsTargetButton";
      this.checkInputsTargetButton.Size = new System.Drawing.Size(24, 24);
      this.checkInputsTargetButton.TabIndex = 14;
      this.toolTip.SetToolTip(this.checkInputsTargetButton, "Inputs & Target Variables");
      this.checkInputsTargetButton.UseVisualStyleBackColor = true;
      this.checkInputsTargetButton.Click += new System.EventHandler(this.checkInputsTargetButton_Click);
      // 
      // uncheckAllButton
      // 
      this.uncheckAllButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.None;
      this.uncheckAllButton.Location = new System.Drawing.Point(766, 5);
      this.uncheckAllButton.Name = "uncheckAllButton";
      this.uncheckAllButton.Size = new System.Drawing.Size(24, 24);
      this.uncheckAllButton.TabIndex = 12;
      this.toolTip.SetToolTip(this.uncheckAllButton, "None");
      this.uncheckAllButton.UseVisualStyleBackColor = true;
      this.uncheckAllButton.Click += new System.EventHandler(this.uncheckAllButton_Click);
      // 
      // checkAllButton
      // 
      this.checkAllButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.All;
      this.checkAllButton.Location = new System.Drawing.Point(706, 5);
      this.checkAllButton.Name = "checkAllButton";
      this.checkAllButton.Size = new System.Drawing.Size(24, 24);
      this.checkAllButton.TabIndex = 13;
      this.toolTip.SetToolTip(this.checkAllButton, "All");
      this.checkAllButton.UseVisualStyleBackColor = true;
      this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
      // 
      // correlationView
      // 
      this.correlationView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.correlationView.Caption = "Feature Correlation View";
      this.correlationView.Content = null;
      this.correlationView.Location = new System.Drawing.Point(0, 0);
      this.correlationView.Name = "correlationView";
      this.correlationView.ReadOnly = false;
      this.correlationView.Size = new System.Drawing.Size(1013, 640);
      this.correlationView.TabIndex = 18;
      // 
      // variablesLabel
      // 
      this.variablesLabel.AutoSize = true;
      this.variablesLabel.Location = new System.Drawing.Point(650, 11);
      this.variablesLabel.Name = "variablesLabel";
      this.variablesLabel.Size = new System.Drawing.Size(50, 13);
      this.variablesLabel.TabIndex = 15;
      this.variablesLabel.Text = "Variables";
      // 
      // PreprocessingFeatureCorrelationView
      // 
      this.Controls.Add(this.variablesLabel);
      this.Controls.Add(this.checkInputsTargetButton);
      this.Controls.Add(this.uncheckAllButton);
      this.Controls.Add(this.checkAllButton);
      this.Controls.Add(this.correlationView);
      this.Name = "PreprocessingFeatureCorrelationView";
      this.Size = new System.Drawing.Size(1013, 640);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Button checkInputsTargetButton;
    private System.Windows.Forms.Button uncheckAllButton;
    private System.Windows.Forms.Button checkAllButton;
    private HeuristicLab.Problems.DataAnalysis.Views.FeatureCorrelationView correlationView;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Label variablesLabel;
  }
}
