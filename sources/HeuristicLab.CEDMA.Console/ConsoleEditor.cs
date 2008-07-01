using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Windows.Forms;

namespace HeuristicLab.CEDMA.Console {
  class ConsoleEditor : EditorBase {
    private System.Windows.Forms.TextBox uriTextBox;
    private System.Windows.Forms.Label uriLabel;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage overviewPage;
    private System.Windows.Forms.TabPage agentsPage;
    private System.Windows.Forms.TabPage resultsPage;
    private Console console;

    public ConsoleEditor(Console console) {
      this.console = console;
      agentsPage.Controls.Add((Control)console.AgentList.CreateView());
    }

    private void InitializeComponent() {
      this.uriTextBox = new System.Windows.Forms.TextBox();
      this.uriLabel = new System.Windows.Forms.Label();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.overviewPage = new System.Windows.Forms.TabPage();
      this.agentsPage = new System.Windows.Forms.TabPage();
      this.resultsPage = new System.Windows.Forms.TabPage();
      this.tabControl.SuspendLayout();
      this.agentsPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // uriTextBox
      // 
      this.uriTextBox.Location = new System.Drawing.Point(91, 3);
      this.uriTextBox.Name = "uriTextBox";
      this.uriTextBox.Size = new System.Drawing.Size(205, 20);
      this.uriTextBox.TabIndex = 0;
      this.uriTextBox.Validated += new System.EventHandler(this.uriTextBox_Validated);
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
      this.tabControl.Controls.Add(this.overviewPage);
      this.tabControl.Controls.Add(this.agentsPage);
      this.tabControl.Controls.Add(this.resultsPage);
      this.tabControl.Location = new System.Drawing.Point(6, 29);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(310, 242);
      this.tabControl.TabIndex = 2;
      // 
      // overviewPage
      // 
      this.overviewPage.Location = new System.Drawing.Point(4, 22);
      this.overviewPage.Name = "overviewPage";
      this.overviewPage.Padding = new System.Windows.Forms.Padding(3);
      this.overviewPage.Size = new System.Drawing.Size(288, 194);
      this.overviewPage.TabIndex = 0;
      this.overviewPage.Text = "Overview";
      this.overviewPage.UseVisualStyleBackColor = true;
      // 
      // agentsPage
      // 
      this.agentsPage.Location = new System.Drawing.Point(4, 22);
      this.agentsPage.Name = "agentsPage";
      this.agentsPage.Padding = new System.Windows.Forms.Padding(3);
      this.agentsPage.Size = new System.Drawing.Size(302, 216);
      this.agentsPage.TabIndex = 1;
      this.agentsPage.Text = "Agents";
      this.agentsPage.UseVisualStyleBackColor = true;
      // 
      // resultsPage
      // 
      this.resultsPage.Location = new System.Drawing.Point(4, 22);
      this.resultsPage.Name = "resultsPage";
      this.resultsPage.Padding = new System.Windows.Forms.Padding(3);
      this.resultsPage.Size = new System.Drawing.Size(288, 194);
      this.resultsPage.TabIndex = 2;
      this.resultsPage.Text = "Results";
      this.resultsPage.UseVisualStyleBackColor = true;
      // 
      // ConsoleEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.uriLabel);
      this.Controls.Add(this.uriTextBox);
      this.Name = "ConsoleEditor";
      this.Size = new System.Drawing.Size(319, 274);
      this.tabControl.ResumeLayout(false);
      this.agentsPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    private void uriTextBox_Validated(object sender, EventArgs e) {
      console.ServerUri = uriTextBox.Text;
    }
  }
}
