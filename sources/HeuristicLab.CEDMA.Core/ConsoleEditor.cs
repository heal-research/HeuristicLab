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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Windows.Forms;
using System.ServiceModel;
using HeuristicLab.PluginInfrastructure;
using System.Drawing;
using HeuristicLab.CEDMA.DB.Interfaces;

namespace HeuristicLab.CEDMA.Core {
  class ConsoleEditor : EditorBase {
    private System.Windows.Forms.TextBox uriTextBox;
    private System.Windows.Forms.Label uriLabel;
    private Button connectButton;
    private Button openOpLibButton;
    private Label label1;
    private Label resultsLabel;
    private Button openResultsButton;
    private Button openAgentsButton;
    private Label agentsLabel;
    private Console console;

    public ConsoleEditor(Console console) {
      InitializeComponent();
      this.console = console;
    }

    private void InitializeComponent() {
      this.uriTextBox = new System.Windows.Forms.TextBox();
      this.uriLabel = new System.Windows.Forms.Label();
      this.connectButton = new System.Windows.Forms.Button();
      this.openOpLibButton = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.resultsLabel = new System.Windows.Forms.Label();
      this.openResultsButton = new System.Windows.Forms.Button();
      this.openAgentsButton = new System.Windows.Forms.Button();
      this.agentsLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // uriTextBox
      // 
      this.uriTextBox.Location = new System.Drawing.Point(94, 3);
      this.uriTextBox.Name = "uriTextBox";
      this.uriTextBox.Size = new System.Drawing.Size(205, 20);
      this.uriTextBox.TabIndex = 0;
      // 
      // uriLabel
      // 
      this.uriLabel.AutoSize = true;
      this.uriLabel.Location = new System.Drawing.Point(6, 6);
      this.uriLabel.Name = "uriLabel";
      this.uriLabel.Size = new System.Drawing.Size(82, 13);
      this.uriLabel.TabIndex = 1;
      this.uriLabel.Text = "CEDMA Server:";
      // 
      // connectButton
      // 
      this.connectButton.Location = new System.Drawing.Point(305, 1);
      this.connectButton.Name = "connectButton";
      this.connectButton.Size = new System.Drawing.Size(75, 23);
      this.connectButton.TabIndex = 3;
      this.connectButton.Text = "&Connect";
      this.connectButton.UseVisualStyleBackColor = true;
      this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // openOpLibButton
      // 
      this.openOpLibButton.Enabled = false;
      this.openOpLibButton.Location = new System.Drawing.Point(94, 85);
      this.openOpLibButton.Name = "openOpLibButton";
      this.openOpLibButton.Size = new System.Drawing.Size(75, 23);
      this.openOpLibButton.TabIndex = 7;
      this.openOpLibButton.Text = "&Open";
      this.openOpLibButton.UseVisualStyleBackColor = true;
      this.openOpLibButton.Click += new System.EventHandler(this.opLibButton_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 90);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(85, 13);
      this.label1.TabIndex = 8;
      this.label1.Text = "Operator Library:";
      // 
      // resultsLabel
      // 
      this.resultsLabel.AutoSize = true;
      this.resultsLabel.Location = new System.Drawing.Point(43, 61);
      this.resultsLabel.Name = "resultsLabel";
      this.resultsLabel.Size = new System.Drawing.Size(45, 13);
      this.resultsLabel.TabIndex = 9;
      this.resultsLabel.Text = "Results:";
      // 
      // openResultsButton
      // 
      this.openResultsButton.Enabled = false;
      this.openResultsButton.Location = new System.Drawing.Point(94, 56);
      this.openResultsButton.Name = "openResultsButton";
      this.openResultsButton.Size = new System.Drawing.Size(75, 23);
      this.openResultsButton.TabIndex = 10;
      this.openResultsButton.Text = "&Open";
      this.openResultsButton.UseVisualStyleBackColor = true;
      this.openResultsButton.Click += new System.EventHandler(this.openChartButton_Click);
      // 
      // openAgentsButton
      // 
      this.openAgentsButton.Enabled = false;
      this.openAgentsButton.Location = new System.Drawing.Point(94, 27);
      this.openAgentsButton.Name = "openAgentsButton";
      this.openAgentsButton.Size = new System.Drawing.Size(75, 23);
      this.openAgentsButton.TabIndex = 12;
      this.openAgentsButton.Text = "&Open";
      this.openAgentsButton.UseVisualStyleBackColor = true;
      this.openAgentsButton.Click += new System.EventHandler(this.openAgentsButton_Click);
      // 
      // agentsLabel
      // 
      this.agentsLabel.AutoSize = true;
      this.agentsLabel.Location = new System.Drawing.Point(45, 32);
      this.agentsLabel.Name = "agentsLabel";
      this.agentsLabel.Size = new System.Drawing.Size(43, 13);
      this.agentsLabel.TabIndex = 11;
      this.agentsLabel.Text = "Agents:";
      // 
      // ConsoleEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.openAgentsButton);
      this.Controls.Add(this.agentsLabel);
      this.Controls.Add(this.openResultsButton);
      this.Controls.Add(this.resultsLabel);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.openOpLibButton);
      this.Controls.Add(this.connectButton);
      this.Controls.Add(this.uriLabel);
      this.Controls.Add(this.uriTextBox);
      this.Name = "ConsoleEditor";
      this.Size = new System.Drawing.Size(445, 189);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    private void connectButton_Click(object sender, EventArgs e) {
      try {
        console.Connect(uriTextBox.Text);
        connectButton.Enabled = false;
        openOpLibButton.Enabled = true;
        openAgentsButton.Enabled = true;
        openResultsButton.Enabled = true;
      } catch(CommunicationException ex) {
        MessageBox.Show("Exception while trying to connect to " + uriTextBox.Text + "\n" + ex.Message);
      }
    }

    private void opLibButton_Click(object sender, EventArgs e) {
      IOperatorLibrary opLib = console.OperatorLibrary;
      if(opLib != null) {
        IView view = opLib.CreateView();
        if(view != null)
          PluginManager.ControlManager.ShowControl(view);
      }
    }

    private void openChartButton_Click(object sender, EventArgs e) {
      PluginManager.ControlManager.ShowControl(console.ResultsList.CreateView());
    }

    private void openAgentsButton_Click(object sender, EventArgs e) {
      PluginManager.ControlManager.ShowControl(console.AgentList.CreateView());
    }
  }
}
