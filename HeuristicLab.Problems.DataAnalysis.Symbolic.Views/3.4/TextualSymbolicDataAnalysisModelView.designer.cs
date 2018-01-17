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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class TextualSymbolicDataAnalysisModelView {
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
      this.symbolicExpressionTreeView = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionView();
      this.SuspendLayout();
      // 
      // symbolicExpressionTreeView
      // 
      this.symbolicExpressionTreeView.AllowDrop = true;
      this.symbolicExpressionTreeView.Caption = "Graphical SymbolicExpressionTree View";
      this.symbolicExpressionTreeView.Content = null;
      this.symbolicExpressionTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.symbolicExpressionTreeView.Location = new System.Drawing.Point(0, 0);
      this.symbolicExpressionTreeView.Name = "symbolicExpressionTreeView";
      this.symbolicExpressionTreeView.ReadOnly = false;
      this.symbolicExpressionTreeView.Size = new System.Drawing.Size(352, 413);
      this.symbolicExpressionTreeView.TabIndex = 0;
      // 
      // TextualSymbolicDataAnalysisModelView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.symbolicExpressionTreeView);
      this.Name = "TextualSymbolicDataAnalysisModelView";
      this.Size = new System.Drawing.Size(352, 413);
      this.ResumeLayout(false);

    }

    #endregion
    private HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionView symbolicExpressionTreeView;

  }
}
