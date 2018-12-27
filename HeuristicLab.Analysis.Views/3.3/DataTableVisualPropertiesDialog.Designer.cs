#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Analysis.Views {
  partial class DataTableVisualPropertiesDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.seriesTabPage = new System.Windows.Forms.TabPage();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.downButton = new System.Windows.Forms.Button();
      this.upButton = new System.Windows.Forms.Button();
      this.seriesListView = new System.Windows.Forms.ListView();
      this.dataRowVisualPropertiesControl = new HeuristicLab.Analysis.Views.DataRowVisualPropertiesControl();
      this.chartTabPage = new System.Windows.Forms.TabPage();
      this.dataTableVisualPropertiesControl = new HeuristicLab.Analysis.Views.DataTableVisualPropertiesControl();
      this.tabControl.SuspendLayout();
      this.seriesTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.chartTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.okButton.Location = new System.Drawing.Point(325, 454);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(406, 454);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 1;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.seriesTabPage);
      this.tabControl.Controls.Add(this.chartTabPage);
      this.tabControl.Location = new System.Drawing.Point(12, 12);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(469, 436);
      this.tabControl.TabIndex = 2;
      // 
      // seriesTabPage
      // 
      this.seriesTabPage.Controls.Add(this.splitContainer);
      this.seriesTabPage.Location = new System.Drawing.Point(4, 22);
      this.seriesTabPage.Name = "seriesTabPage";
      this.seriesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.seriesTabPage.Size = new System.Drawing.Size(461, 410);
      this.seriesTabPage.TabIndex = 0;
      this.seriesTabPage.Text = "Series";
      this.seriesTabPage.UseVisualStyleBackColor = true;
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.downButton);
      this.splitContainer.Panel1.Controls.Add(this.upButton);
      this.splitContainer.Panel1.Controls.Add(this.seriesListView);
      this.splitContainer.Panel1MinSize = 20;
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.dataRowVisualPropertiesControl);
      this.splitContainer.Panel2MinSize = 50;
      this.splitContainer.Size = new System.Drawing.Size(461, 410);
      this.splitContainer.SplitterDistance = 125;
      this.splitContainer.TabIndex = 0;
      // 
      // downButton
      // 
      this.downButton.Enabled = false;
      this.downButton.Location = new System.Drawing.Point(35, 3);
      this.downButton.Name = "downButton";
      this.downButton.Size = new System.Drawing.Size(26, 23);
      this.downButton.TabIndex = 1;
      this.downButton.Text = "Down";
      this.downButton.UseVisualStyleBackColor = true;
      this.downButton.Click += new System.EventHandler(this.downButton_Click);
      // 
      // upButton
      // 
      this.upButton.Enabled = false;
      this.upButton.Location = new System.Drawing.Point(3, 3);
      this.upButton.Name = "upButton";
      this.upButton.Size = new System.Drawing.Size(26, 23);
      this.upButton.TabIndex = 0;
      this.upButton.Text = "Up";
      this.upButton.UseVisualStyleBackColor = true;
      this.upButton.Click += new System.EventHandler(this.upButton_Click);
      // 
      // seriesListView
      // 
      this.seriesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.seriesListView.HideSelection = false;
      this.seriesListView.Location = new System.Drawing.Point(3, 32);
      this.seriesListView.MultiSelect = false;
      this.seriesListView.Name = "seriesListView";
      this.seriesListView.ShowGroups = false;
      this.seriesListView.Size = new System.Drawing.Size(119, 375);
      this.seriesListView.TabIndex = 2;
      this.seriesListView.UseCompatibleStateImageBehavior = false;
      this.seriesListView.View = System.Windows.Forms.View.List;
      this.seriesListView.SelectedIndexChanged += new System.EventHandler(this.seriesListView_SelectedIndexChanged);
      // 
      // dataRowVisualPropertiesControl
      // 
      this.dataRowVisualPropertiesControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataRowVisualPropertiesControl.Content = null;
      this.dataRowVisualPropertiesControl.Location = new System.Drawing.Point(3, 26);
      this.dataRowVisualPropertiesControl.Name = "dataRowVisualPropertiesControl";
      this.dataRowVisualPropertiesControl.Size = new System.Drawing.Size(326, 381);
      this.dataRowVisualPropertiesControl.TabIndex = 0;
      // 
      // chartTabPage
      // 
      this.chartTabPage.Controls.Add(this.dataTableVisualPropertiesControl);
      this.chartTabPage.Location = new System.Drawing.Point(4, 22);
      this.chartTabPage.Name = "chartTabPage";
      this.chartTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.chartTabPage.Size = new System.Drawing.Size(461, 381);
      this.chartTabPage.TabIndex = 1;
      this.chartTabPage.Text = "Chart";
      this.chartTabPage.UseVisualStyleBackColor = true;
      // 
      // dataTableVisualPropertiesControl
      // 
      this.dataTableVisualPropertiesControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTableVisualPropertiesControl.Content = null;
      this.dataTableVisualPropertiesControl.Location = new System.Drawing.Point(3, 6);
      this.dataTableVisualPropertiesControl.Name = "dataTableVisualPropertiesControl";
      this.dataTableVisualPropertiesControl.Size = new System.Drawing.Size(455, 372);
      this.dataTableVisualPropertiesControl.TabIndex = 0;
      // 
      // DataTableVisualPropertiesDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(493, 489);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "DataTableVisualPropertiesDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Customize Visual Properties";
      this.TopMost = true;
      this.tabControl.ResumeLayout(false);
      this.seriesTabPage.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.chartTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage seriesTabPage;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.ListView seriesListView;
    private System.Windows.Forms.TabPage chartTabPage;
    private DataRowVisualPropertiesControl dataRowVisualPropertiesControl;
    private DataTableVisualPropertiesControl dataTableVisualPropertiesControl;
    private System.Windows.Forms.Button downButton;
    private System.Windows.Forms.Button upButton;
  }
}