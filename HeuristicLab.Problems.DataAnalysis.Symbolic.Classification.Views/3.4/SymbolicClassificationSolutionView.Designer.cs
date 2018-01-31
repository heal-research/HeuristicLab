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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification.Views {
  partial class SymbolicClassificationSolutionView {
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
      this.exportButton = new System.Windows.Forms.Button();
      this.exportFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.btnSimplify = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // flowLayoutPanel
      // 
      this.flowLayoutPanel.Controls.Add(this.btnSimplify);
      this.flowLayoutPanel.Controls.Add(this.exportButton);
      // 
      // btnSimplify
      // 
      this.btnSimplify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
      this.btnSimplify.Image = HeuristicLab.Common.Resources.VSImageLibrary.FormulaEvaluator;
      this.btnSimplify.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSimplify.Name = "btnSimplify";
      this.btnSimplify.Size = new System.Drawing.Size(105, 24);
      this.btnSimplify.TabIndex = 7;
      this.btnSimplify.Text = "Simplify Model";
      this.btnSimplify.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.btnSimplify.UseVisualStyleBackColor = true;
      this.btnSimplify.Click += new System.EventHandler(this.btn_SimplifyModel_Click);
      this.toolTip.SetToolTip(this.btnSimplify, "Simplify solution");
      // 
      // exportButton
      // 
      this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
      this.exportButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Statistics;
      this.exportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.exportButton.Name = "exportButton";
      this.exportButton.Size = new System.Drawing.Size(105, 24);
      this.exportButton.TabIndex = 8;
      this.exportButton.Text = "Export to Excel ";
      this.exportButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.exportButton.UseVisualStyleBackColor = true;
      this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
      this.toolTip.SetToolTip(this.exportButton, "Exports the symbolic regression solution to Excel.");
      // 
      // SymbolicClassificationSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Name = "SymbolicDiscriminantFunctionClassificationSolutionView";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

    protected System.Windows.Forms.SaveFileDialog exportFileDialog;
    protected System.Windows.Forms.Button exportButton;
    protected System.Windows.Forms.Button btnSimplify;
  }
}
