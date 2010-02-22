#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization.Views {
  partial class UserDefinedAlgorithmView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.operatorGraphTabPage = new System.Windows.Forms.TabPage();
      this.globalScopeTabPage = new System.Windows.Forms.TabPage();
      this.globalScopeView = new HeuristicLab.Core.Views.ScopeView();
      this.operatorGraphViewHost = new HeuristicLab.Core.Views.ViewHost();
      this.newOperatorGraphButton = new System.Windows.Forms.Button();
      this.openOperatorGraphButton = new System.Windows.Forms.Button();
      this.saveOperatorGraphButton = new System.Windows.Forms.Button();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.operatorGraphTabPage.SuspendLayout();
      this.globalScopeTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.operatorGraphTabPage);
      this.tabControl.Controls.Add(this.globalScopeTabPage);
      this.tabControl.Controls.SetChildIndex(this.globalScopeTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.operatorGraphTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.problemTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.parametersTabPage, 0);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // operatorGraphTabPage
      // 
      this.operatorGraphTabPage.Controls.Add(this.saveOperatorGraphButton);
      this.operatorGraphTabPage.Controls.Add(this.openOperatorGraphButton);
      this.operatorGraphTabPage.Controls.Add(this.newOperatorGraphButton);
      this.operatorGraphTabPage.Controls.Add(this.operatorGraphViewHost);
      this.operatorGraphTabPage.Location = new System.Drawing.Point(4, 22);
      this.operatorGraphTabPage.Name = "operatorGraphTabPage";
      this.operatorGraphTabPage.Size = new System.Drawing.Size(482, 148);
      this.operatorGraphTabPage.TabIndex = 2;
      this.operatorGraphTabPage.Text = "Operator Graph";
      this.operatorGraphTabPage.UseVisualStyleBackColor = true;
      // 
      // globalScopeTabPage
      // 
      this.globalScopeTabPage.Controls.Add(this.globalScopeView);
      this.globalScopeTabPage.Location = new System.Drawing.Point(4, 22);
      this.globalScopeTabPage.Name = "globalScopeTabPage";
      this.globalScopeTabPage.Size = new System.Drawing.Size(482, 148);
      this.globalScopeTabPage.TabIndex = 3;
      this.globalScopeTabPage.Text = "Global Scope";
      this.globalScopeTabPage.UseVisualStyleBackColor = true;
      // 
      // globalScopeView
      // 
      this.globalScopeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.globalScopeView.Caption = "Scope";
      this.globalScopeView.Content = null;
      this.globalScopeView.Location = new System.Drawing.Point(3, 3);
      this.globalScopeView.Name = "globalScopeView";
      this.globalScopeView.Size = new System.Drawing.Size(476, 142);
      this.globalScopeView.TabIndex = 0;
      // 
      // operatorGraphViewHost
      // 
      this.operatorGraphViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorGraphViewHost.Content = null;
      this.operatorGraphViewHost.Location = new System.Drawing.Point(3, 33);
      this.operatorGraphViewHost.Name = "operatorGraphViewHost";
      this.operatorGraphViewHost.Size = new System.Drawing.Size(476, 112);
      this.operatorGraphViewHost.TabIndex = 0;
      this.operatorGraphViewHost.ViewType = null;
      // 
      // newOperatorGraphButton
      // 
      this.newOperatorGraphButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.NewDocument;
      this.newOperatorGraphButton.Location = new System.Drawing.Point(3, 3);
      this.newOperatorGraphButton.Name = "newOperatorGraphButton";
      this.newOperatorGraphButton.Size = new System.Drawing.Size(24, 24);
      this.newOperatorGraphButton.TabIndex = 1;
      this.newOperatorGraphButton.UseVisualStyleBackColor = true;
      this.newOperatorGraphButton.Click += new System.EventHandler(this.newOperatorGraphButton_Click);
      // 
      // openOperatorGraphButton
      // 
      this.openOperatorGraphButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Open;
      this.openOperatorGraphButton.Location = new System.Drawing.Point(33, 3);
      this.openOperatorGraphButton.Name = "openOperatorGraphButton";
      this.openOperatorGraphButton.Size = new System.Drawing.Size(24, 24);
      this.openOperatorGraphButton.TabIndex = 1;
      this.openOperatorGraphButton.UseVisualStyleBackColor = true;
      this.openOperatorGraphButton.Click += new System.EventHandler(this.openOperatorGraphButton_Click);
      // 
      // saveOperatorGraphButton
      // 
      this.saveOperatorGraphButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Save;
      this.saveOperatorGraphButton.Location = new System.Drawing.Point(63, 3);
      this.saveOperatorGraphButton.Name = "saveOperatorGraphButton";
      this.saveOperatorGraphButton.Size = new System.Drawing.Size(24, 24);
      this.saveOperatorGraphButton.TabIndex = 1;
      this.saveOperatorGraphButton.UseVisualStyleBackColor = true;
      this.saveOperatorGraphButton.Click += new System.EventHandler(this.saveOperatorGraphButton_Click);
      // 
      // UserDefinedAlgorithmView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "UserDefinedAlgorithmView";
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.problemTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.operatorGraphTabPage.ResumeLayout(false);
      this.globalScopeTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabPage operatorGraphTabPage;
    private System.Windows.Forms.TabPage globalScopeTabPage;
    private HeuristicLab.Core.Views.ScopeView globalScopeView;
    private System.Windows.Forms.Button saveOperatorGraphButton;
    private System.Windows.Forms.Button openOperatorGraphButton;
    private System.Windows.Forms.Button newOperatorGraphButton;
    private HeuristicLab.Core.Views.ViewHost operatorGraphViewHost;

  }
}
