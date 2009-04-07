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

namespace HeuristicLab.Communication.Data {
  partial class ProtocolEditor {
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
      if (Protocol != null) Protocol.Changed -= new System.EventHandler(Protocol_Changed);
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.protocolNameLabel = new System.Windows.Forms.Label();
      this.protocolStatesLabel = new System.Windows.Forms.Label();
      this.statesListBox = new System.Windows.Forms.ListBox();
      this.stateChartPictureBox = new System.Windows.Forms.PictureBox();
      this.addStateButton = new System.Windows.Forms.Button();
      this.removeStateButton = new System.Windows.Forms.Button();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.setAsInitialStateButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.stateChartPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // protocolNameLabel
      // 
      this.protocolNameLabel.AutoSize = true;
      this.protocolNameLabel.Location = new System.Drawing.Point(3, 6);
      this.protocolNameLabel.Name = "protocolNameLabel";
      this.protocolNameLabel.Size = new System.Drawing.Size(80, 13);
      this.protocolNameLabel.TabIndex = 3;
      this.protocolNameLabel.Text = "Protocol Name:";
      // 
      // protocolStatesLabel
      // 
      this.protocolStatesLabel.AutoSize = true;
      this.protocolStatesLabel.Location = new System.Drawing.Point(3, 32);
      this.protocolStatesLabel.Name = "protocolStatesLabel";
      this.protocolStatesLabel.Size = new System.Drawing.Size(82, 13);
      this.protocolStatesLabel.TabIndex = 8;
      this.protocolStatesLabel.Text = "Protocol States:";
      // 
      // statesListBox
      // 
      this.statesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.statesListBox.FormattingEnabled = true;
      this.statesListBox.Location = new System.Drawing.Point(6, 48);
      this.statesListBox.MultiColumn = true;
      this.statesListBox.Name = "statesListBox";
      this.statesListBox.Size = new System.Drawing.Size(159, 303);
      this.statesListBox.TabIndex = 10;
      this.statesListBox.SelectedIndexChanged += new System.EventHandler(this.statesListBox_SelectedIndexChanged);
      this.statesListBox.DoubleClick += new System.EventHandler(this.statesListBox_DoubleClick);
      // 
      // stateChartPictureBox
      // 
      this.stateChartPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.stateChartPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.stateChartPictureBox.Location = new System.Drawing.Point(172, 48);
      this.stateChartPictureBox.Name = "stateChartPictureBox";
      this.stateChartPictureBox.Size = new System.Drawing.Size(444, 303);
      this.stateChartPictureBox.TabIndex = 11;
      this.stateChartPictureBox.TabStop = false;
      // 
      // addStateButton
      // 
      this.addStateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addStateButton.Location = new System.Drawing.Point(6, 357);
      this.addStateButton.Name = "addStateButton";
      this.addStateButton.Size = new System.Drawing.Size(77, 23);
      this.addStateButton.TabIndex = 12;
      this.addStateButton.Text = "Add";
      this.addStateButton.UseVisualStyleBackColor = true;
      this.addStateButton.Click += new System.EventHandler(this.addStateButton_Click);
      // 
      // removeStateButton
      // 
      this.removeStateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeStateButton.Location = new System.Drawing.Point(89, 357);
      this.removeStateButton.Name = "removeStateButton";
      this.removeStateButton.Size = new System.Drawing.Size(75, 23);
      this.removeStateButton.TabIndex = 13;
      this.removeStateButton.Text = "Remove";
      this.removeStateButton.UseVisualStyleBackColor = true;
      this.removeStateButton.Click += new System.EventHandler(this.removeStateButton_Click);
      // 
      // nameTextBox
      // 
      this.nameTextBox.Location = new System.Drawing.Point(89, 3);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(142, 20);
      this.nameTextBox.TabIndex = 14;
      // 
      // setAsInitialStateButton
      // 
      this.setAsInitialStateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.setAsInitialStateButton.Location = new System.Drawing.Point(170, 357);
      this.setAsInitialStateButton.Name = "setAsInitialStateButton";
      this.setAsInitialStateButton.Size = new System.Drawing.Size(75, 23);
      this.setAsInitialStateButton.TabIndex = 15;
      this.setAsInitialStateButton.Text = "Set Initital";
      this.setAsInitialStateButton.UseVisualStyleBackColor = true;
      this.setAsInitialStateButton.Click += new System.EventHandler(this.setAsInitialStateButton_Click);
      // 
      // ProtocolEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.setAsInitialStateButton);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.removeStateButton);
      this.Controls.Add(this.addStateButton);
      this.Controls.Add(this.stateChartPictureBox);
      this.Controls.Add(this.statesListBox);
      this.Controls.Add(this.protocolStatesLabel);
      this.Controls.Add(this.protocolNameLabel);
      this.Name = "ProtocolEditor";
      this.Size = new System.Drawing.Size(619, 389);
      ((System.ComponentModel.ISupportInitialize)(this.stateChartPictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label protocolNameLabel;
    private System.Windows.Forms.Label protocolStatesLabel;
    private System.Windows.Forms.ListBox statesListBox;
    private System.Windows.Forms.PictureBox stateChartPictureBox;
    private System.Windows.Forms.Button addStateButton;
    private System.Windows.Forms.Button removeStateButton;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Button setAsInitialStateButton;
  }
}