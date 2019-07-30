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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class HistogramView {
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
      this.components = new System.ComponentModel.Container();
      this.optionsBox = new System.Windows.Forms.GroupBox();
      this.groupingTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.variableLabel = new System.Windows.Forms.Label();
      this.orderLabel = new System.Windows.Forms.Label();
      this.aggregationLabel = new System.Windows.Forms.Label();
      this.orderComboBox = new System.Windows.Forms.ComboBox();
      this.groupingComboBox = new System.Windows.Forms.ComboBox();
      this.aggregationComboBox = new System.Windows.Forms.ComboBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.optionsBox.SuspendLayout();
      this.groupingTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // sizeGroupBox
      // 
      this.sizeGroupBox.Location = new System.Drawing.Point(0, 258);
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.optionsBox);
      // 
      // optionsBox
      // 
      this.optionsBox.Controls.Add(this.groupingTableLayoutPanel);
      this.optionsBox.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.optionsBox.Location = new System.Drawing.Point(0, 302);
      this.optionsBox.Name = "optionsBox";
      this.optionsBox.Size = new System.Drawing.Size(180, 101);
      this.optionsBox.TabIndex = 7;
      this.optionsBox.TabStop = false;
      this.optionsBox.Text = "Grouping Options";
      // 
      // groupingTableLayoutPanel
      // 
      this.groupingTableLayoutPanel.ColumnCount = 2;
      this.groupingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.groupingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.groupingTableLayoutPanel.Controls.Add(this.variableLabel, 0, 0);
      this.groupingTableLayoutPanel.Controls.Add(this.orderLabel, 0, 2);
      this.groupingTableLayoutPanel.Controls.Add(this.aggregationLabel, 0, 1);
      this.groupingTableLayoutPanel.Controls.Add(this.orderComboBox, 1, 2);
      this.groupingTableLayoutPanel.Controls.Add(this.groupingComboBox, 1, 0);
      this.groupingTableLayoutPanel.Controls.Add(this.aggregationComboBox, 1, 1);
      this.groupingTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupingTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
      this.groupingTableLayoutPanel.Name = "groupingTableLayoutPanel";
      this.groupingTableLayoutPanel.RowCount = 3;
      this.groupingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.groupingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.groupingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.groupingTableLayoutPanel.Size = new System.Drawing.Size(174, 82);
      this.groupingTableLayoutPanel.TabIndex = 3;
      // 
      // variableLabel
      // 
      this.variableLabel.AutoSize = true;
      this.variableLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableLabel.Location = new System.Drawing.Point(3, 0);
      this.variableLabel.Name = "variableLabel";
      this.variableLabel.Size = new System.Drawing.Size(48, 27);
      this.variableLabel.TabIndex = 2;
      this.variableLabel.Text = "Variable:";
      this.variableLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // orderLabel
      // 
      this.orderLabel.AutoSize = true;
      this.orderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.orderLabel.Location = new System.Drawing.Point(3, 54);
      this.orderLabel.Name = "orderLabel";
      this.orderLabel.Size = new System.Drawing.Size(48, 28);
      this.orderLabel.TabIndex = 2;
      this.orderLabel.Text = "Order:";
      this.orderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.toolTip.SetToolTip(this.orderLabel, "Order of Legend Entries");
      // 
      // aggregationLabel
      // 
      this.aggregationLabel.AutoSize = true;
      this.aggregationLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.aggregationLabel.Location = new System.Drawing.Point(3, 27);
      this.aggregationLabel.Name = "aggregationLabel";
      this.aggregationLabel.Size = new System.Drawing.Size(48, 27);
      this.aggregationLabel.TabIndex = 2;
      this.aggregationLabel.Text = "Aggr.:";
      this.aggregationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.toolTip.SetToolTip(this.aggregationLabel, "Aggregation");
      // 
      // orderComboBox
      // 
      this.orderComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.orderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.orderComboBox.FormattingEnabled = true;
      this.orderComboBox.Location = new System.Drawing.Point(57, 57);
      this.orderComboBox.Name = "orderComboBox";
      this.orderComboBox.Size = new System.Drawing.Size(114, 21);
      this.orderComboBox.TabIndex = 1;
      this.orderComboBox.SelectedIndexChanged += new System.EventHandler(this.orderComboBox_SelectedIndexChanged);
      // 
      // groupingComboBox
      // 
      this.groupingComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.groupingComboBox.FormattingEnabled = true;
      this.groupingComboBox.Location = new System.Drawing.Point(57, 3);
      this.groupingComboBox.Name = "groupingComboBox";
      this.groupingComboBox.Size = new System.Drawing.Size(114, 21);
      this.groupingComboBox.TabIndex = 1;
      this.groupingComboBox.SelectedIndexChanged += new System.EventHandler(this.classifierComboBox_SelectedIndexChanged);
      // 
      // aggregationComboBox
      // 
      this.aggregationComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.aggregationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.aggregationComboBox.FormattingEnabled = true;
      this.aggregationComboBox.Location = new System.Drawing.Point(57, 30);
      this.aggregationComboBox.Name = "aggregationComboBox";
      this.aggregationComboBox.Size = new System.Drawing.Size(114, 21);
      this.aggregationComboBox.TabIndex = 1;
      this.aggregationComboBox.SelectedIndexChanged += new System.EventHandler(this.aggregationComboBox_SelectedIndexChanged);
      // 
      // HistogramView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "HistogramView";
      this.splitContainer.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.optionsBox.ResumeLayout(false);
      this.groupingTableLayoutPanel.ResumeLayout(false);
      this.groupingTableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox optionsBox;
    private System.Windows.Forms.ComboBox groupingComboBox;
    private System.Windows.Forms.Label variableLabel;
    private System.Windows.Forms.Label aggregationLabel;
    private System.Windows.Forms.ComboBox aggregationComboBox;
    private System.Windows.Forms.TableLayoutPanel groupingTableLayoutPanel;
    private System.Windows.Forms.Label orderLabel;
    private System.Windows.Forms.ComboBox orderComboBox;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
