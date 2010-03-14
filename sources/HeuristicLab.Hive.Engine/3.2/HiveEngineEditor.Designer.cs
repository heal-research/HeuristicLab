#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Hive.Engine {
  partial class HiveEngineEditor {
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
      this.urlLabel = new System.Windows.Forms.Label();
      this.snapshotButton = new System.Windows.Forms.Button();
      this.multiSubmitTextbox = new System.Windows.Forms.TextBox();
      this.urlTextBox = new System.Windows.Forms.TextBox();
      this.assignedRessourceTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.operatorGraphGroupBox.SuspendLayout();
      this.globalScopeGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Location = new System.Drawing.Point(686, 460);
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Location = new System.Drawing.Point(683, 444);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Size = new System.Drawing.Size(827, 441);
      this.splitContainer1.SplitterDistance = 639;
      // 
      // operatorGraphGroupBox
      // 
      this.operatorGraphGroupBox.Size = new System.Drawing.Size(639, 441);
      // 
      // globalScopeGroupBox
      // 
      this.globalScopeGroupBox.Size = new System.Drawing.Size(184, 441);
      // 
      // operatorGraphView
      // 
      this.operatorGraphView.Size = new System.Drawing.Size(633, 422);
      // 
      // scopeView
      // 
      this.scopeView.Size = new System.Drawing.Size(178, 422);
      // 
      // urlLabel
      // 
      this.urlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.urlLabel.AutoSize = true;
      this.urlLabel.Location = new System.Drawing.Point(523, 444);
      this.urlLabel.Name = "urlLabel";
      this.urlLabel.Size = new System.Drawing.Size(82, 13);
      this.urlLabel.TabIndex = 8;
      this.urlLabel.Text = "Hive Server Url:";
      // 
      // snapshotButton
      // 
      this.snapshotButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.snapshotButton.Enabled = false;
      this.snapshotButton.Location = new System.Drawing.Point(294, 457);
      this.snapshotButton.Name = "snapshotButton";
      this.snapshotButton.Size = new System.Drawing.Size(75, 23);
      this.snapshotButton.TabIndex = 9;
      this.snapshotButton.Text = "Sna&pshot";
      this.snapshotButton.UseVisualStyleBackColor = true;
      this.snapshotButton.Click += new System.EventHandler(this.snapshotButton_Click);
      // 
      // multiSubmitTextbox
      // 
      this.multiSubmitTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.multiSubmitTextbox.Location = new System.Drawing.Point(375, 460);
      this.multiSubmitTextbox.Name = "multiSubmitTextbox";
      this.multiSubmitTextbox.Size = new System.Drawing.Size(31, 20);
      this.multiSubmitTextbox.TabIndex = 10;
      this.multiSubmitTextbox.Text = "1";
      // 
      // urlTextBox
      // 
      this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.urlTextBox.Location = new System.Drawing.Point(526, 459);
      this.urlTextBox.Name = "urlTextBox";
      this.urlTextBox.Size = new System.Drawing.Size(154, 20);
      this.urlTextBox.TabIndex = 7;
      // 
      // assignedRessourceTextBox
      // 
      this.assignedRessourceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.assignedRessourceTextBox.Location = new System.Drawing.Point(412, 457);
      this.assignedRessourceTextBox.Name = "assignedRessourceTextBox";
      this.assignedRessourceTextBox.Size = new System.Drawing.Size(100, 20);
      this.assignedRessourceTextBox.TabIndex = 11;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(409, 444);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(85, 13);
      this.label1.TabIndex = 12;
      this.label1.Text = "Ressource GIDs";
      // 
      // HiveEngineEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.multiSubmitTextbox);
      this.Controls.Add(this.snapshotButton);
      this.Controls.Add(this.urlTextBox);
      this.Controls.Add(this.urlLabel);
      this.Controls.Add(this.assignedRessourceTextBox);
      this.Controls.Add(this.label1);
      this.Name = "HiveEngineEditor";
      this.Size = new System.Drawing.Size(827, 480);
      this.Controls.SetChildIndex(this.label1, 0);
      this.Controls.SetChildIndex(this.assignedRessourceTextBox, 0);
      this.Controls.SetChildIndex(this.urlLabel, 0);
      this.Controls.SetChildIndex(this.urlTextBox, 0);
      this.Controls.SetChildIndex(this.snapshotButton, 0);
      this.Controls.SetChildIndex(this.multiSubmitTextbox, 0);
      this.Controls.SetChildIndex(this.executeButton, 0);
      this.Controls.SetChildIndex(this.abortButton, 0);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.splitContainer1, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.operatorGraphGroupBox.ResumeLayout(false);
      this.globalScopeGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label urlLabel;
    private System.Windows.Forms.Button snapshotButton;
    private System.Windows.Forms.TextBox multiSubmitTextbox;
    private System.Windows.Forms.TextBox urlTextBox;
    private System.Windows.Forms.TextBox assignedRessourceTextBox;
    private System.Windows.Forms.Label label1;
  }
}
