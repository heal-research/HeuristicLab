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
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage agentsPage;
    private Button connectButton;
    private ComboBox comboBox1;
    private Label projectLabel;
    private Button newButton;
    private Button opLibButton;
    private Label label1;
    private TabPage resultsTabPage;
    private Console console;

    public ConsoleEditor(Console console) {
      InitializeComponent();
      this.console = console;
    }

    private void InitializeComponent() {
      this.uriTextBox = new System.Windows.Forms.TextBox();
      this.uriLabel = new System.Windows.Forms.Label();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.agentsPage = new System.Windows.Forms.TabPage();
      this.resultsTabPage = new System.Windows.Forms.TabPage();
      this.connectButton = new System.Windows.Forms.Button();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.projectLabel = new System.Windows.Forms.Label();
      this.newButton = new System.Windows.Forms.Button();
      this.opLibButton = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.tabControl.SuspendLayout();
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
      this.uriLabel.Location = new System.Drawing.Point(3, 6);
      this.uriLabel.Name = "uriLabel";
      this.uriLabel.Size = new System.Drawing.Size(82, 13);
      this.uriLabel.TabIndex = 1;
      this.uriLabel.Text = "CEDMA Server:";
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.agentsPage);
      this.tabControl.Controls.Add(this.resultsTabPage);
      this.tabControl.Enabled = false;
      this.tabControl.Location = new System.Drawing.Point(6, 85);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(506, 378);
      this.tabControl.TabIndex = 2;
      // 
      // agentsPage
      // 
      this.agentsPage.Location = new System.Drawing.Point(4, 22);
      this.agentsPage.Name = "agentsPage";
      this.agentsPage.Padding = new System.Windows.Forms.Padding(3);
      this.agentsPage.Size = new System.Drawing.Size(498, 352);
      this.agentsPage.TabIndex = 1;
      this.agentsPage.Text = "Agents";
      this.agentsPage.UseVisualStyleBackColor = true;
      // 
      // resultsTabPage
      // 
      this.resultsTabPage.Location = new System.Drawing.Point(4, 22);
      this.resultsTabPage.Name = "resultsTabPage";
      this.resultsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.resultsTabPage.Size = new System.Drawing.Size(498, 352);
      this.resultsTabPage.TabIndex = 2;
      this.resultsTabPage.Text = "Results";
      this.resultsTabPage.UseVisualStyleBackColor = true;
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
      // comboBox1
      // 
      this.comboBox1.Enabled = false;
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Location = new System.Drawing.Point(94, 29);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(121, 21);
      this.comboBox1.TabIndex = 4;
      // 
      // projectLabel
      // 
      this.projectLabel.AutoSize = true;
      this.projectLabel.Enabled = false;
      this.projectLabel.Location = new System.Drawing.Point(42, 32);
      this.projectLabel.Name = "projectLabel";
      this.projectLabel.Size = new System.Drawing.Size(43, 13);
      this.projectLabel.TabIndex = 5;
      this.projectLabel.Text = "Project:";
      // 
      // newButton
      // 
      this.newButton.Enabled = false;
      this.newButton.Location = new System.Drawing.Point(221, 27);
      this.newButton.Name = "newButton";
      this.newButton.Size = new System.Drawing.Size(75, 23);
      this.newButton.TabIndex = 6;
      this.newButton.Text = "New...";
      this.newButton.UseVisualStyleBackColor = true;
      // 
      // opLibButton
      // 
      this.opLibButton.Enabled = false;
      this.opLibButton.Location = new System.Drawing.Point(94, 56);
      this.opLibButton.Name = "opLibButton";
      this.opLibButton.Size = new System.Drawing.Size(75, 23);
      this.opLibButton.TabIndex = 7;
      this.opLibButton.Text = "&Open";
      this.opLibButton.UseVisualStyleBackColor = true;
      this.opLibButton.Click += new System.EventHandler(this.opLibButton_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 61);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(85, 13);
      this.label1.TabIndex = 8;
      this.label1.Text = "Operator Library:";
      // 
      // ConsoleEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.opLibButton);
      this.Controls.Add(this.newButton);
      this.Controls.Add(this.projectLabel);
      this.Controls.Add(this.comboBox1);
      this.Controls.Add(this.connectButton);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.uriLabel);
      this.Controls.Add(this.uriTextBox);
      this.Name = "ConsoleEditor";
      this.Size = new System.Drawing.Size(515, 466);
      this.tabControl.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    private void connectButton_Click(object sender, EventArgs e) {
      try {
        console.Connect(uriTextBox.Text);
        connectButton.Enabled = false;
        tabControl.Enabled = true;
        agentsPage.Controls.Clear();
        agentsPage.Controls.Add((Control)console.AgentList.CreateView());
        agentsPage.Controls[0].Dock = DockStyle.Fill;
        resultsTabPage.Controls.Clear();
        resultsTabPage.Controls.Add((Control)console.ResultsList.CreateView());
        resultsTabPage.Controls[0].Dock = DockStyle.Fill;
        opLibButton.Enabled = true;
        opLibButton.Enabled = true;
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
  }
}
