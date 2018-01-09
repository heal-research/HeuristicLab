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


namespace HeuristicLab.Scripting.Views {
  partial class CSharpScriptView {
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
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.variableStoreView = new HeuristicLab.Scripting.Views.VariableStoreView();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
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
      // executionTimeLabel
      // 
      this.executionTimeLabel.Location = new System.Drawing.Point(409, 3);
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Location = new System.Drawing.Point(498, 0);
      // 
      // infoTabControl
      // 
      this.infoTabControl.Size = new System.Drawing.Size(637, 110);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Size = new System.Drawing.Size(637, 543);
      this.splitContainer1.SplitterDistance = 427;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer2.Location = new System.Drawing.Point(3, 56);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.variableStoreView);
      this.splitContainer2.Panel2.Controls.Add(this.viewHost);
      this.splitContainer2.Size = new System.Drawing.Size(829, 543);
      this.splitContainer2.SplitterDistance = 637;
      this.splitContainer2.TabIndex = 7;
      // 
      // variableStoreView
      // 
      this.variableStoreView.Caption = "ItemCollection View";
      this.variableStoreView.Content = null;
      this.variableStoreView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableStoreView.Location = new System.Drawing.Point(0, 0);
      this.variableStoreView.Name = "variableStoreView";
      this.variableStoreView.ReadOnly = false;
      this.variableStoreView.Size = new System.Drawing.Size(188, 543);
      this.variableStoreView.TabIndex = 0;
      // 
      // viewHost
      // 
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(0, 0);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(188, 543);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // CSharpScriptView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer2);
      this.Name = "CSharpScriptView";
      this.Controls.SetChildIndex(this.infoTextLabel, 0);
      this.Controls.SetChildIndex(this.compileButton, 0);
      this.Controls.SetChildIndex(this.startStopButton, 0);
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
      this.splitContainer1.Panel2.PerformLayout();
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

    protected System.Windows.Forms.SplitContainer splitContainer2;
    protected MainForm.WindowsForms.ViewHost viewHost;
    protected VariableStoreView variableStoreView;
  }
}
