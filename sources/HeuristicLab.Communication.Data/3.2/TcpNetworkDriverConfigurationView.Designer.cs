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
  partial class TcpNetworkDriverConfigurationView {
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
      this.ipAddressStringDataView = new HeuristicLab.Data.StringDataView();
      this.ipAddressLabel = new System.Windows.Forms.Label();
      this.targetPortLabel = new System.Windows.Forms.Label();
      this.targetPortIntDataView = new HeuristicLab.Data.IntDataView();
      this.sourcePortIntDataView = new HeuristicLab.Data.IntDataView();
      this.sourcePortLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // ipAddressStringDataView
      // 
      this.ipAddressStringDataView.Caption = "View";
      this.ipAddressStringDataView.Location = new System.Drawing.Point(74, 3);
      this.ipAddressStringDataView.Name = "ipAddressStringDataView";
      this.ipAddressStringDataView.Size = new System.Drawing.Size(200, 25);
      this.ipAddressStringDataView.StringData = null;
      this.ipAddressStringDataView.TabIndex = 0;
      // 
      // ipAddressLabel
      // 
      this.ipAddressLabel.AutoSize = true;
      this.ipAddressLabel.Location = new System.Drawing.Point(7, 7);
      this.ipAddressLabel.Name = "ipAddressLabel";
      this.ipAddressLabel.Size = new System.Drawing.Size(61, 13);
      this.ipAddressLabel.TabIndex = 3;
      this.ipAddressLabel.Text = "IP Address:";
      // 
      // targetPortLabel
      // 
      this.targetPortLabel.AutoSize = true;
      this.targetPortLabel.Location = new System.Drawing.Point(8, 33);
      this.targetPortLabel.Name = "targetPortLabel";
      this.targetPortLabel.Size = new System.Drawing.Size(60, 13);
      this.targetPortLabel.TabIndex = 4;
      this.targetPortLabel.Text = "TargetPort:";
      // 
      // targetPortIntDataView
      // 
      this.targetPortIntDataView.Caption = "View";
      this.targetPortIntDataView.IntData = null;
      this.targetPortIntDataView.Location = new System.Drawing.Point(74, 30);
      this.targetPortIntDataView.Name = "targetPortIntDataView";
      this.targetPortIntDataView.Size = new System.Drawing.Size(96, 26);
      this.targetPortIntDataView.TabIndex = 1;
      // 
      // sourcePortIntDataView
      // 
      this.sourcePortIntDataView.Caption = "View";
      this.sourcePortIntDataView.IntData = null;
      this.sourcePortIntDataView.Location = new System.Drawing.Point(74, 57);
      this.sourcePortIntDataView.Name = "sourcePortIntDataView";
      this.sourcePortIntDataView.Size = new System.Drawing.Size(96, 26);
      this.sourcePortIntDataView.TabIndex = 5;
      // 
      // sourcePortLabel
      // 
      this.sourcePortLabel.AutoSize = true;
      this.sourcePortLabel.Location = new System.Drawing.Point(2, 60);
      this.sourcePortLabel.Name = "sourcePortLabel";
      this.sourcePortLabel.Size = new System.Drawing.Size(66, 13);
      this.sourcePortLabel.TabIndex = 6;
      this.sourcePortLabel.Text = "Source Port:";
      // 
      // TcpNetworkDriverConfigurationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.sourcePortLabel);
      this.Controls.Add(this.sourcePortIntDataView);
      this.Controls.Add(this.targetPortIntDataView);
      this.Controls.Add(this.targetPortLabel);
      this.Controls.Add(this.ipAddressLabel);
      this.Controls.Add(this.ipAddressStringDataView);
      this.Name = "TcpNetworkDriverConfigurationView";
      this.Size = new System.Drawing.Size(278, 81);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Data.StringDataView ipAddressStringDataView;
    private System.Windows.Forms.Label ipAddressLabel;
    private System.Windows.Forms.Label targetPortLabel;
    private HeuristicLab.Data.IntDataView targetPortIntDataView;
    private HeuristicLab.Data.IntDataView sourcePortIntDataView;
    private System.Windows.Forms.Label sourcePortLabel;
  }
}
