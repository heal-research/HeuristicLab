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

namespace HeuristicLab.Operators.Views {
  partial class AlgorithmOperatorView {
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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.parameterCollectionView = new HeuristicLab.Core.Views.ParameterCollectionView();
      this.operatorGraphTabPage = new System.Windows.Forms.TabPage();
      this.operatorGraphViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.breakpointCheckBox = new System.Windows.Forms.CheckBox();
      this.breakpointLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.operatorGraphTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(414, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Size = new System.Drawing.Size(414, 20);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Controls.Add(this.operatorGraphTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 72);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(486, 410);
      this.tabControl.TabIndex = 6;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.parameterCollectionView);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(478, 384);
      this.parametersTabPage.TabIndex = 0;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parameterCollectionView.Content = null;
      this.parameterCollectionView.Location = new System.Drawing.Point(6, 6);
      this.parameterCollectionView.Name = "parameterCollectionView";
      this.parameterCollectionView.Size = new System.Drawing.Size(466, 372);
      this.parameterCollectionView.TabIndex = 0;
      // 
      // operatorGraphTabPage
      // 
      this.operatorGraphTabPage.Controls.Add(this.operatorGraphViewHost);
      this.operatorGraphTabPage.Location = new System.Drawing.Point(4, 22);
      this.operatorGraphTabPage.Name = "operatorGraphTabPage";
      this.operatorGraphTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.operatorGraphTabPage.Size = new System.Drawing.Size(478, 384);
      this.operatorGraphTabPage.TabIndex = 1;
      this.operatorGraphTabPage.Text = "Operator Graph";
      this.operatorGraphTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorGraphViewHost
      // 
      this.operatorGraphViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorGraphViewHost.Content = null;
      this.operatorGraphViewHost.Location = new System.Drawing.Point(6, 6);
      this.operatorGraphViewHost.Name = "operatorGraphViewHost";
      this.operatorGraphViewHost.Size = new System.Drawing.Size(466, 372);
      this.operatorGraphViewHost.TabIndex = 0;
      this.operatorGraphViewHost.ViewType = null;
      // 
      // breakpointCheckBox
      // 
      this.breakpointCheckBox.AutoSize = true;
      this.breakpointCheckBox.Location = new System.Drawing.Point(72, 52);
      this.breakpointCheckBox.Name = "breakpointCheckBox";
      this.breakpointCheckBox.Size = new System.Drawing.Size(15, 14);
      this.breakpointCheckBox.TabIndex = 5;
      this.breakpointCheckBox.UseVisualStyleBackColor = true;
      this.breakpointCheckBox.CheckedChanged += new System.EventHandler(this.breakpointCheckBox_CheckedChanged);
      this.toolTip.SetToolTip(this.breakpointCheckBox, "Check if an engine should stop execution each time after this operator has been processed.");
      // 
      // breakpointLabel
      // 
      this.breakpointLabel.AutoSize = true;
      this.breakpointLabel.Location = new System.Drawing.Point(3, 52);
      this.breakpointLabel.Name = "breakpointLabel";
      this.breakpointLabel.Size = new System.Drawing.Size(61, 13);
      this.breakpointLabel.TabIndex = 4;
      this.breakpointLabel.Text = "&Breakpoint:";
      // 
      // AlgorithmOperatorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.breakpointCheckBox);
      this.Controls.Add(this.breakpointLabel);
      this.Controls.Add(this.tabControl);
      this.Name = "AlgorithmOperatorView";
      this.Size = new System.Drawing.Size(486, 482);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.Controls.SetChildIndex(this.breakpointLabel, 0);
      this.Controls.SetChildIndex(this.breakpointCheckBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.operatorGraphTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.TabControl tabControl;
    protected System.Windows.Forms.TabPage parametersTabPage;
    protected System.Windows.Forms.TabPage operatorGraphTabPage;
    protected HeuristicLab.Core.Views.ParameterCollectionView parameterCollectionView;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost operatorGraphViewHost;
    protected System.Windows.Forms.CheckBox breakpointCheckBox;
    protected System.Windows.Forms.Label breakpointLabel;
  }
}
