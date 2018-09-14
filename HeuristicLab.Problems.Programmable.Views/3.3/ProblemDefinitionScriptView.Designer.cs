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


namespace HeuristicLab.Problems.Programmable.Views {
  partial class ProblemDefinitionScriptView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProblemDefinitionScriptView));
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.variableStoreView = new HeuristicLab.Scripting.Views.VariableStoreView();
      this.infoTabControl.SuspendLayout();
      this.outputTabPage.SuspendLayout();
      this.errorListTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.SuspendLayout();
      // 
      // compileButton
      // 
      this.toolTip.SetToolTip(this.compileButton, "Compile (F6)");
      // 
      // infoTabControl
      // 
      this.infoTabControl.Size = new System.Drawing.Size(593, 113);
      // 
      // outputTabPage
      // 
      this.outputTabPage.Size = new System.Drawing.Size(585, 87);
      // 
      // outputTextBox
      // 
      this.outputTextBox.Size = new System.Drawing.Size(579, 81);
      // 
      // errorListTabPage
      // 
      this.errorListTabPage.Size = new System.Drawing.Size(621, 86);
      // 
      // errorListView
      // 
      this.errorListView.Size = new System.Drawing.Size(615, 80);
      // 
      // codeEditor
      // 
      this.codeEditor.Size = new System.Drawing.Size(593, 429);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Size = new System.Drawing.Size(593, 546);
      this.splitContainer1.SplitterDistance = 429;
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // splitContainer2
      // 
      this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer2.Location = new System.Drawing.Point(6, 56);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.variableStoreView);
      this.splitContainer2.Size = new System.Drawing.Size(826, 546);
      this.splitContainer2.SplitterDistance = 593;
      this.splitContainer2.TabIndex = 10;
      // 
      // variableStoreView
      // 
      this.variableStoreView.Caption = "ItemCollection View";
      this.variableStoreView.Content = null;
      this.variableStoreView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableStoreView.Location = new System.Drawing.Point(0, 0);
      this.variableStoreView.Name = "variableStoreView";
      this.variableStoreView.ReadOnly = false;
      this.variableStoreView.Size = new System.Drawing.Size(229, 546);
      this.variableStoreView.TabIndex = 10;
      // 
      // ProblemDefinitionScriptView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer2);
      this.Name = "ProblemDefinitionScriptView";
      this.Controls.SetChildIndex(this.infoTextLabel, 0);
      this.Controls.SetChildIndex(this.compileButton, 0);
      this.Controls.SetChildIndex(this.splitContainer2, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.infoTabControl.ResumeLayout(false);
      this.outputTabPage.ResumeLayout(false);
      this.outputTabPage.PerformLayout();
      this.errorListTabPage.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer2;
    private Scripting.Views.VariableStoreView variableStoreView;
  }
}
