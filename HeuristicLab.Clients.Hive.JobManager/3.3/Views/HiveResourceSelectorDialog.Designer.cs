#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  partial class HiveResourceSelectorDialog {
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
      this.components = new System.ComponentModel.Container();
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.refreshButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.hiveResourceSelector = new HeuristicLab.Clients.Hive.JobManager.Views.HiveProjectSelector();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(405, 609);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 1;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.errorProvider.SetIconAlignment(this.okButton, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.okButton, 2);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(486, 609);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 2;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(12, 608);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh data");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // hiveResourceSelector
      // 
      this.hiveResourceSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.hiveResourceSelector.Caption = "View";
      this.hiveResourceSelector.Content = null;
      this.hiveResourceSelector.JobId = new System.Guid("00000000-0000-0000-0000-000000000000");
      this.hiveResourceSelector.Location = new System.Drawing.Point(12, 12);
      this.hiveResourceSelector.Name = "hiveResourceSelector";
      this.hiveResourceSelector.ProjectId = null;
      this.hiveResourceSelector.ReadOnly = false;
      this.hiveResourceSelector.SelectedProject = null;
      this.hiveResourceSelector.SelectedProjectId = null;
      this.hiveResourceSelector.SelectedResourceIds = null;
      this.hiveResourceSelector.Size = new System.Drawing.Size(549, 591);
      this.hiveResourceSelector.TabIndex = 0;
      this.hiveResourceSelector.SelectedProjectChanged += new System.EventHandler(this.hiveResourceSelector_SelectedProjectChanged);
      this.hiveResourceSelector.AssignedResourcesChanged += new System.EventHandler(this.hiveResourceSelector_SelectedResourcesChanged);
      this.hiveResourceSelector.ProjectsTreeViewDoubleClicked += new System.EventHandler(this.hiveResourceSelector_ProjectsTreeViewDoubleClicked);
      // 
      // HiveResourceSelectorDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(573, 644);
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.hiveResourceSelector);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "HiveResourceSelectorDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Select Project";
      this.Load += new System.EventHandler(this.HiveResourceSelectorDialog_Load);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Button okButton;
    protected System.Windows.Forms.Button cancelButton;
    protected System.Windows.Forms.Button refreshButton;
    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.ErrorProvider errorProvider;
    protected HiveProjectSelector hiveResourceSelector;
  }
}