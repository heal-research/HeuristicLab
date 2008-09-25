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
  partial class ProtocolStateView {
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
      HeuristicLab.Data.StringData stringData1 = new HeuristicLab.Data.StringData();
      HeuristicLab.Data.BoolData boolData1 = new HeuristicLab.Data.BoolData();
      this.nameStringDataView = new HeuristicLab.Data.StringDataView();
      this.nameLabel = new System.Windows.Forms.Label();
      this.communicationDataTabControl = new System.Windows.Forms.TabControl();
      this.sendTabPage = new System.Windows.Forms.TabPage();
      this.outboundCommunicationDataView = new HeuristicLab.Data.ConstrainedItemListView();
      this.receiveTabPage = new System.Windows.Forms.TabPage();
      this.inboundCommunicationDataView = new HeuristicLab.Data.ConstrainedItemListView();
      this.transitionTabPage = new System.Windows.Forms.TabPage();
      this.removeTransitionButton = new System.Windows.Forms.Button();
      this.addTransitionButton = new System.Windows.Forms.Button();
      this.stateTransitionTabControl = new System.Windows.Forms.TabControl();
      this.acceptingStateLabel = new System.Windows.Forms.Label();
      this.acceptingStateBoolDataView = new HeuristicLab.Data.BoolDataView();
      this.communicationDataTabControl.SuspendLayout();
      this.sendTabPage.SuspendLayout();
      this.receiveTabPage.SuspendLayout();
      this.transitionTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameStringDataView
      // 
      this.nameStringDataView.Caption = "View (StringData)";
      this.nameStringDataView.Location = new System.Drawing.Point(48, 14);
      this.nameStringDataView.Name = "nameStringDataView";
      this.nameStringDataView.Size = new System.Drawing.Size(178, 26);
      stringData1.Data = "";
      this.nameStringDataView.StringData = stringData1;
      this.nameStringDataView.TabIndex = 1;
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(4, 17);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 3;
      this.nameLabel.Text = "Name:";
      // 
      // communicationDataTabControl
      // 
      this.communicationDataTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.communicationDataTabControl.Controls.Add(this.sendTabPage);
      this.communicationDataTabControl.Controls.Add(this.receiveTabPage);
      this.communicationDataTabControl.Controls.Add(this.transitionTabPage);
      this.communicationDataTabControl.Location = new System.Drawing.Point(3, 46);
      this.communicationDataTabControl.Name = "communicationDataTabControl";
      this.communicationDataTabControl.SelectedIndex = 0;
      this.communicationDataTabControl.Size = new System.Drawing.Size(483, 381);
      this.communicationDataTabControl.TabIndex = 5;
      // 
      // sendTabPage
      // 
      this.sendTabPage.Controls.Add(this.outboundCommunicationDataView);
      this.sendTabPage.Location = new System.Drawing.Point(4, 22);
      this.sendTabPage.Name = "sendTabPage";
      this.sendTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.sendTabPage.Size = new System.Drawing.Size(475, 355);
      this.sendTabPage.TabIndex = 0;
      this.sendTabPage.Text = "Send";
      this.sendTabPage.UseVisualStyleBackColor = true;
      // 
      // outboundCommunicationDataView
      // 
      this.outboundCommunicationDataView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.outboundCommunicationDataView.Caption = "View";
      this.outboundCommunicationDataView.ConstrainedItemList = null;
      this.outboundCommunicationDataView.Location = new System.Drawing.Point(6, 6);
      this.outboundCommunicationDataView.Name = "outboundCommunicationDataView";
      this.outboundCommunicationDataView.Size = new System.Drawing.Size(463, 343);
      this.outboundCommunicationDataView.TabIndex = 0;
      // 
      // receiveTabPage
      // 
      this.receiveTabPage.Controls.Add(this.inboundCommunicationDataView);
      this.receiveTabPage.Location = new System.Drawing.Point(4, 22);
      this.receiveTabPage.Name = "receiveTabPage";
      this.receiveTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.receiveTabPage.Size = new System.Drawing.Size(475, 355);
      this.receiveTabPage.TabIndex = 1;
      this.receiveTabPage.Text = "Receive";
      this.receiveTabPage.UseVisualStyleBackColor = true;
      // 
      // inboundCommunicationDataView
      // 
      this.inboundCommunicationDataView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.inboundCommunicationDataView.Caption = "View";
      this.inboundCommunicationDataView.ConstrainedItemList = null;
      this.inboundCommunicationDataView.Location = new System.Drawing.Point(6, 6);
      this.inboundCommunicationDataView.Name = "inboundCommunicationDataView";
      this.inboundCommunicationDataView.Size = new System.Drawing.Size(463, 343);
      this.inboundCommunicationDataView.TabIndex = 0;
      // 
      // transitionTabPage
      // 
      this.transitionTabPage.Controls.Add(this.removeTransitionButton);
      this.transitionTabPage.Controls.Add(this.addTransitionButton);
      this.transitionTabPage.Controls.Add(this.stateTransitionTabControl);
      this.transitionTabPage.Location = new System.Drawing.Point(4, 22);
      this.transitionTabPage.Name = "transitionTabPage";
      this.transitionTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.transitionTabPage.Size = new System.Drawing.Size(475, 355);
      this.transitionTabPage.TabIndex = 2;
      this.transitionTabPage.Text = "State Transition";
      this.transitionTabPage.UseVisualStyleBackColor = true;
      // 
      // removeTransitionButton
      // 
      this.removeTransitionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.removeTransitionButton.Enabled = false;
      this.removeTransitionButton.Location = new System.Drawing.Point(365, 326);
      this.removeTransitionButton.Name = "removeTransitionButton";
      this.removeTransitionButton.Size = new System.Drawing.Size(104, 23);
      this.removeTransitionButton.TabIndex = 2;
      this.removeTransitionButton.Text = "Remove Transition";
      this.removeTransitionButton.UseVisualStyleBackColor = true;
      this.removeTransitionButton.Click += new System.EventHandler(this.removeTransitionButton_Click);
      // 
      // addTransitionButton
      // 
      this.addTransitionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addTransitionButton.Location = new System.Drawing.Point(6, 326);
      this.addTransitionButton.Name = "addTransitionButton";
      this.addTransitionButton.Size = new System.Drawing.Size(104, 23);
      this.addTransitionButton.TabIndex = 1;
      this.addTransitionButton.Text = "Add Transition";
      this.addTransitionButton.UseVisualStyleBackColor = true;
      this.addTransitionButton.Click += new System.EventHandler(this.addTransitionButton_Click);
      // 
      // stateTransitionTabControl
      // 
      this.stateTransitionTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.stateTransitionTabControl.Location = new System.Drawing.Point(0, 0);
      this.stateTransitionTabControl.Name = "stateTransitionTabControl";
      this.stateTransitionTabControl.SelectedIndex = 0;
      this.stateTransitionTabControl.Size = new System.Drawing.Size(475, 320);
      this.stateTransitionTabControl.TabIndex = 0;
      // 
      // acceptingStateLabel
      // 
      this.acceptingStateLabel.AutoSize = true;
      this.acceptingStateLabel.Location = new System.Drawing.Point(232, 17);
      this.acceptingStateLabel.Name = "acceptingStateLabel";
      this.acceptingStateLabel.Size = new System.Drawing.Size(86, 13);
      this.acceptingStateLabel.TabIndex = 6;
      this.acceptingStateLabel.Text = "Accepting State:";
      // 
      // acceptingStateBoolDataView
      // 
      boolData1.Data = true;
      this.acceptingStateBoolDataView.BoolData = boolData1;
      this.acceptingStateBoolDataView.Caption = "View";
      this.acceptingStateBoolDataView.Enabled = false;
      this.acceptingStateBoolDataView.Location = new System.Drawing.Point(324, 17);
      this.acceptingStateBoolDataView.Name = "acceptingStateBoolDataView";
      this.acceptingStateBoolDataView.Size = new System.Drawing.Size(15, 23);
      this.acceptingStateBoolDataView.TabIndex = 7;
      // 
      // ProtocolStateView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.acceptingStateBoolDataView);
      this.Controls.Add(this.acceptingStateLabel);
      this.Controls.Add(this.communicationDataTabControl);
      this.Controls.Add(this.nameLabel);
      this.Controls.Add(this.nameStringDataView);
      this.Name = "ProtocolStateView";
      this.Size = new System.Drawing.Size(489, 430);
      this.communicationDataTabControl.ResumeLayout(false);
      this.sendTabPage.ResumeLayout(false);
      this.receiveTabPage.ResumeLayout(false);
      this.transitionTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Data.StringDataView nameStringDataView;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.TabControl communicationDataTabControl;
    private System.Windows.Forms.TabPage sendTabPage;
    private System.Windows.Forms.TabPage receiveTabPage;
    private HeuristicLab.Data.ConstrainedItemListView outboundCommunicationDataView;
    private HeuristicLab.Data.ConstrainedItemListView inboundCommunicationDataView;
    private System.Windows.Forms.Label acceptingStateLabel;
    private HeuristicLab.Data.BoolDataView acceptingStateBoolDataView;
    private System.Windows.Forms.TabPage transitionTabPage;
    private System.Windows.Forms.TabControl stateTransitionTabControl;
    private System.Windows.Forms.Button addTransitionButton;
    private System.Windows.Forms.Button removeTransitionButton;

  }
}
