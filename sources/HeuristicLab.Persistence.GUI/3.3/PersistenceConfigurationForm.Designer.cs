#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Persistence.GUI {
  partial class PersistenceConfigurationForm {
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
      this.updateButton = new System.Windows.Forms.Button();
      this.configurationTabs = new System.Windows.Forms.TabControl();
      this.buttonPanel = new System.Windows.Forms.TableLayoutPanel();
      this.resetButton = new System.Windows.Forms.Button();
      this.buttonPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // updateButton
      // 
      this.updateButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.updateButton.Location = new System.Drawing.Point(296, 6);
      this.updateButton.Name = "updateButton";
      this.updateButton.Size = new System.Drawing.Size(282, 25);
      this.updateButton.TabIndex = 1;
      this.updateButton.Text = "&Define";
      this.updateButton.UseVisualStyleBackColor = true;
      this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
      // 
      // configurationTabs
      // 
      this.configurationTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.configurationTabs.Location = new System.Drawing.Point(0, -1);
      this.configurationTabs.Name = "configurationTabs";
      this.configurationTabs.SelectedIndex = 0;
      this.configurationTabs.Size = new System.Drawing.Size(584, 573);
      this.configurationTabs.TabIndex = 0;
      // 
      // buttonPanel
      // 
      this.buttonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonPanel.AutoSize = true;
      this.buttonPanel.CausesValidation = false;
      this.buttonPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
      this.buttonPanel.ColumnCount = 2;
      this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.buttonPanel.Controls.Add(this.updateButton, 1, 0);
      this.buttonPanel.Controls.Add(this.resetButton, 0, 0);
      this.buttonPanel.Location = new System.Drawing.Point(0, 572);
      this.buttonPanel.Name = "buttonPanel";
      this.buttonPanel.RowCount = 1;
      this.buttonPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.buttonPanel.Size = new System.Drawing.Size(584, 37);
      this.buttonPanel.TabIndex = 2;
      // 
      // resetButton
      // 
      this.resetButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resetButton.Location = new System.Drawing.Point(6, 6);
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size(281, 25);
      this.resetButton.TabIndex = 2;
      this.resetButton.Text = "&Reset All";
      this.resetButton.UseVisualStyleBackColor = true;
      this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
      // 
      // PersistenceConfigurationForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(582, 609);
      this.Controls.Add(this.buttonPanel);
      this.Controls.Add(this.configurationTabs);
      this.Icon = HeuristicLab.Common.Resources.HeuristicLab.Icon;
      this.Name = "PersistenceConfigurationForm";
      this.Text = "PersistenceConfigurationForm";
      this.buttonPanel.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl configurationTabs;
    private System.Windows.Forms.TableLayoutPanel buttonPanel;
    private System.Windows.Forms.Button resetButton;
    private System.Windows.Forms.Button updateButton;
  }
}