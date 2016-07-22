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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  partial class SymbolicExpressionGrammarAllowedChildSymbolsControl {
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
      this.allowedChildSymbolsGroupBox = new System.Windows.Forms.GroupBox();
      this.symbolicExpressionTreeChart = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionTreeChart();
      this.allowedChildSymbolsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // allowedChildSymbolsGroupBox
      // 
      this.allowedChildSymbolsGroupBox.Controls.Add(this.symbolicExpressionTreeChart);
      this.allowedChildSymbolsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.allowedChildSymbolsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.allowedChildSymbolsGroupBox.Name = "allowedChildSymbolsGroupBox";
      this.allowedChildSymbolsGroupBox.Size = new System.Drawing.Size(651, 405);
      this.allowedChildSymbolsGroupBox.TabIndex = 2;
      this.allowedChildSymbolsGroupBox.TabStop = false;
      this.allowedChildSymbolsGroupBox.Text = "Allowed Child Symbols";
      // 
      // allowedChildSymbolsControl
      // 
      this.symbolicExpressionTreeChart.AllowDrop = true;
      this.symbolicExpressionTreeChart.BackgroundColor = System.Drawing.Color.White;
      this.symbolicExpressionTreeChart.Dock = System.Windows.Forms.DockStyle.Fill;
      this.symbolicExpressionTreeChart.LineColor = System.Drawing.Color.Black;
      this.symbolicExpressionTreeChart.Location = new System.Drawing.Point(3, 16);
      this.symbolicExpressionTreeChart.Name = "allowedChildSymbolsControl";
      this.symbolicExpressionTreeChart.Size = new System.Drawing.Size(645, 386);
      this.symbolicExpressionTreeChart.Spacing = 5;
      this.symbolicExpressionTreeChart.SuspendRepaint = false;
      this.symbolicExpressionTreeChart.TabIndex = 0;
      this.symbolicExpressionTreeChart.TextFont = new System.Drawing.Font("Times New Roman", 8F);
      this.symbolicExpressionTreeChart.Tree = null;
      this.symbolicExpressionTreeChart.SymbolicExpressionTreeNodeClicked += new System.Windows.Forms.MouseEventHandler(this.symbolicExpressionTreeChart_SymbolicExpressionTreeNodeClicked);
      this.symbolicExpressionTreeChart.DragDrop += new System.Windows.Forms.DragEventHandler(this.symbolicExpressionTreeChart_DragDrop);
      this.symbolicExpressionTreeChart.DragEnter += new System.Windows.Forms.DragEventHandler(this.symbolicExpressionTreeChart_DragEnter);
      this.symbolicExpressionTreeChart.DragOver += new System.Windows.Forms.DragEventHandler(this.symbolicExpressionTreeChart_DragOver);
      this.symbolicExpressionTreeChart.Paint += new System.Windows.Forms.PaintEventHandler(this.allowedChildSymbolsControl_Paint);
      this.symbolicExpressionTreeChart.KeyDown += new System.Windows.Forms.KeyEventHandler(this.symbolicExpressionTreeChart_KeyDown);
      this.symbolicExpressionTreeChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.allowedChildSymbolsControl_MouseDown);
      // 
      // SymbolicExpressionGrammarAllowedChildSymbolsControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.allowedChildSymbolsGroupBox);
      this.Name = "SymbolicExpressionGrammarAllowedChildSymbolsControl";
      this.Size = new System.Drawing.Size(651, 405);
      this.allowedChildSymbolsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox allowedChildSymbolsGroupBox;
    private SymbolicExpressionTreeChart symbolicExpressionTreeChart;
  }
}
