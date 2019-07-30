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
  partial class StatisticsView {
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
      this.lblRows = new System.Windows.Forms.Label();
      this.lblColumns = new System.Windows.Forms.Label();
      this.lblMissingValuesTotal = new System.Windows.Forms.Label();
      this.lblNumericColumns = new System.Windows.Forms.Label();
      this.lblNominalColumns = new System.Windows.Forms.Label();
      this.stringMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.showVariablesGroupBox = new System.Windows.Forms.GroupBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.lblTNumValues = new System.Windows.Forms.Label();
      this.orientationGroupBox = new System.Windows.Forms.GroupBox();
      this.verticalRadioButton = new System.Windows.Forms.RadioButton();
      this.horizontalRadioButton = new System.Windows.Forms.RadioButton();
      this.rowsTextBox = new System.Windows.Forms.TextBox();
      this.columnsTextBox = new System.Windows.Forms.TextBox();
      this.totalValuesTextBox = new System.Windows.Forms.TextBox();
      this.overviewTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.numericColumnsTextBox = new System.Windows.Forms.TextBox();
      this.nominalColumnsTextBox5 = new System.Windows.Forms.TextBox();
      this.missingValuesTextBox = new System.Windows.Forms.TextBox();
      this.overviewGroupBox = new System.Windows.Forms.GroupBox();
      this.checkInputsTargetButton = new System.Windows.Forms.Button();
      this.uncheckAllButton = new System.Windows.Forms.Button();
      this.checkAllButton = new System.Windows.Forms.Button();
      this.showVariablesGroupBox.SuspendLayout();
      this.orientationGroupBox.SuspendLayout();
      this.overviewTableLayoutPanel.SuspendLayout();
      this.overviewGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblRows
      // 
      this.lblRows.AutoSize = true;
      this.lblRows.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lblRows.Location = new System.Drawing.Point(3, 0);
      this.lblRows.Name = "lblRows";
      this.lblRows.Size = new System.Drawing.Size(52, 26);
      this.lblRows.TabIndex = 0;
      this.lblRows.Text = "Datarows";
      this.lblRows.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblColumns
      // 
      this.lblColumns.AutoSize = true;
      this.lblColumns.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lblColumns.Location = new System.Drawing.Point(3, 26);
      this.lblColumns.Name = "lblColumns";
      this.lblColumns.Size = new System.Drawing.Size(52, 26);
      this.lblColumns.TabIndex = 2;
      this.lblColumns.Text = "Variables";
      this.lblColumns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblMissingValuesTotal
      // 
      this.lblMissingValuesTotal.AutoSize = true;
      this.lblMissingValuesTotal.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lblMissingValuesTotal.Location = new System.Drawing.Point(275, 26);
      this.lblMissingValuesTotal.Name = "lblMissingValuesTotal";
      this.lblMissingValuesTotal.Size = new System.Drawing.Size(77, 26);
      this.lblMissingValuesTotal.TabIndex = 3;
      this.lblMissingValuesTotal.Text = "Missing Values";
      this.lblMissingValuesTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblNumericColumns
      // 
      this.lblNumericColumns.AutoSize = true;
      this.lblNumericColumns.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lblNumericColumns.Location = new System.Drawing.Point(119, 0);
      this.lblNumericColumns.Name = "lblNumericColumns";
      this.lblNumericColumns.Size = new System.Drawing.Size(92, 26);
      this.lblNumericColumns.TabIndex = 3;
      this.lblNumericColumns.Text = "Numeric Variables";
      this.lblNumericColumns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblNominalColumns
      // 
      this.lblNominalColumns.AutoSize = true;
      this.lblNominalColumns.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lblNominalColumns.Location = new System.Drawing.Point(119, 26);
      this.lblNominalColumns.Name = "lblNominalColumns";
      this.lblNominalColumns.Size = new System.Drawing.Size(92, 26);
      this.lblNominalColumns.TabIndex = 3;
      this.lblNominalColumns.Text = "Nominal Variables";
      this.lblNominalColumns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // stringMatrixView
      // 
      this.stringMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stringMatrixView.Caption = "StringConvertibleMatrix View";
      this.stringMatrixView.Content = null;
      this.stringMatrixView.Location = new System.Drawing.Point(3, 74);
      this.stringMatrixView.Name = "stringMatrixView";
      this.stringMatrixView.ReadOnly = true;
      this.stringMatrixView.ShowRowsAndColumnsTextBox = false;
      this.stringMatrixView.ShowStatisticalInformation = true;
      this.stringMatrixView.Size = new System.Drawing.Size(655, 374);
      this.stringMatrixView.TabIndex = 4;
      // 
      // showVariablesGroupBox
      // 
      this.showVariablesGroupBox.Controls.Add(this.checkInputsTargetButton);
      this.showVariablesGroupBox.Controls.Add(this.uncheckAllButton);
      this.showVariablesGroupBox.Controls.Add(this.checkAllButton);
      this.showVariablesGroupBox.Location = new System.Drawing.Point(458, 0);
      this.showVariablesGroupBox.Name = "showVariablesGroupBox";
      this.showVariablesGroupBox.Size = new System.Drawing.Size(97, 71);
      this.showVariablesGroupBox.TabIndex = 16;
      this.showVariablesGroupBox.TabStop = false;
      this.showVariablesGroupBox.Text = "Show Variables";
      // 
      // lblTNumValues
      // 
      this.lblTNumValues.AutoSize = true;
      this.lblTNumValues.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lblTNumValues.Location = new System.Drawing.Point(275, 0);
      this.lblTNumValues.Name = "lblTNumValues";
      this.lblTNumValues.Size = new System.Drawing.Size(77, 26);
      this.lblTNumValues.TabIndex = 2;
      this.lblTNumValues.Text = "Total Values";
      this.lblTNumValues.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.toolTip.SetToolTip(this.lblTNumValues, "Valid Values (excl. missing Values)");
      // 
      // orientationGroupBox
      // 
      this.orientationGroupBox.Controls.Add(this.verticalRadioButton);
      this.orientationGroupBox.Controls.Add(this.horizontalRadioButton);
      this.orientationGroupBox.Location = new System.Drawing.Point(561, 0);
      this.orientationGroupBox.Name = "orientationGroupBox";
      this.orientationGroupBox.Size = new System.Drawing.Size(94, 71);
      this.orientationGroupBox.TabIndex = 17;
      this.orientationGroupBox.TabStop = false;
      this.orientationGroupBox.Text = "Orientation";
      // 
      // verticalRadioButton
      // 
      this.verticalRadioButton.AutoSize = true;
      this.verticalRadioButton.Checked = true;
      this.verticalRadioButton.Location = new System.Drawing.Point(6, 32);
      this.verticalRadioButton.Name = "verticalRadioButton";
      this.verticalRadioButton.Size = new System.Drawing.Size(60, 17);
      this.verticalRadioButton.TabIndex = 1;
      this.verticalRadioButton.TabStop = true;
      this.verticalRadioButton.Text = "Vertical";
      this.verticalRadioButton.UseVisualStyleBackColor = true;
      this.verticalRadioButton.CheckedChanged += new System.EventHandler(this.verticalRadioButton_CheckedChanged);
      // 
      // horizontalRadioButton
      // 
      this.horizontalRadioButton.AutoSize = true;
      this.horizontalRadioButton.Location = new System.Drawing.Point(6, 14);
      this.horizontalRadioButton.Name = "horizontalRadioButton";
      this.horizontalRadioButton.Size = new System.Drawing.Size(72, 17);
      this.horizontalRadioButton.TabIndex = 0;
      this.horizontalRadioButton.Text = "Horizontal";
      this.horizontalRadioButton.UseVisualStyleBackColor = true;
      this.horizontalRadioButton.CheckedChanged += new System.EventHandler(this.horizontalRadioButton_CheckedChanged);
      // 
      // rowsTextBox
      // 
      this.rowsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rowsTextBox.Location = new System.Drawing.Point(61, 3);
      this.rowsTextBox.Name = "rowsTextBox";
      this.rowsTextBox.ReadOnly = true;
      this.rowsTextBox.Size = new System.Drawing.Size(52, 20);
      this.rowsTextBox.TabIndex = 18;
      this.rowsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // columnsTextBox
      // 
      this.columnsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.columnsTextBox.Location = new System.Drawing.Point(61, 29);
      this.columnsTextBox.Name = "columnsTextBox";
      this.columnsTextBox.ReadOnly = true;
      this.columnsTextBox.Size = new System.Drawing.Size(52, 20);
      this.columnsTextBox.TabIndex = 18;
      this.columnsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // totalValuesTextBox
      // 
      this.totalValuesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.totalValuesTextBox.Location = new System.Drawing.Point(358, 3);
      this.totalValuesTextBox.Name = "totalValuesTextBox";
      this.totalValuesTextBox.ReadOnly = true;
      this.totalValuesTextBox.Size = new System.Drawing.Size(82, 20);
      this.totalValuesTextBox.TabIndex = 18;
      this.totalValuesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // overviewTableLayoutPanel
      // 
      this.overviewTableLayoutPanel.ColumnCount = 6;
      this.overviewTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.overviewTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
      this.overviewTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.overviewTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
      this.overviewTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.overviewTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.85715F));
      this.overviewTableLayoutPanel.Controls.Add(this.lblRows, 0, 0);
      this.overviewTableLayoutPanel.Controls.Add(this.lblColumns, 0, 1);
      this.overviewTableLayoutPanel.Controls.Add(this.columnsTextBox, 1, 1);
      this.overviewTableLayoutPanel.Controls.Add(this.lblNominalColumns, 2, 1);
      this.overviewTableLayoutPanel.Controls.Add(this.lblNumericColumns, 2, 0);
      this.overviewTableLayoutPanel.Controls.Add(this.rowsTextBox, 1, 0);
      this.overviewTableLayoutPanel.Controls.Add(this.numericColumnsTextBox, 3, 0);
      this.overviewTableLayoutPanel.Controls.Add(this.nominalColumnsTextBox5, 3, 1);
      this.overviewTableLayoutPanel.Controls.Add(this.missingValuesTextBox, 5, 1);
      this.overviewTableLayoutPanel.Controls.Add(this.lblTNumValues, 4, 0);
      this.overviewTableLayoutPanel.Controls.Add(this.totalValuesTextBox, 5, 0);
      this.overviewTableLayoutPanel.Controls.Add(this.lblMissingValuesTotal, 4, 1);
      this.overviewTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.overviewTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
      this.overviewTableLayoutPanel.Name = "overviewTableLayoutPanel";
      this.overviewTableLayoutPanel.RowCount = 3;
      this.overviewTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.overviewTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.overviewTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.overviewTableLayoutPanel.Size = new System.Drawing.Size(443, 52);
      this.overviewTableLayoutPanel.TabIndex = 19;
      // 
      // numericColumnsTextBox
      // 
      this.numericColumnsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.numericColumnsTextBox.Location = new System.Drawing.Point(217, 3);
      this.numericColumnsTextBox.Name = "numericColumnsTextBox";
      this.numericColumnsTextBox.ReadOnly = true;
      this.numericColumnsTextBox.Size = new System.Drawing.Size(52, 20);
      this.numericColumnsTextBox.TabIndex = 18;
      this.numericColumnsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // nominalColumnsTextBox5
      // 
      this.nominalColumnsTextBox5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.nominalColumnsTextBox5.Location = new System.Drawing.Point(217, 29);
      this.nominalColumnsTextBox5.Name = "nominalColumnsTextBox5";
      this.nominalColumnsTextBox5.ReadOnly = true;
      this.nominalColumnsTextBox5.Size = new System.Drawing.Size(52, 20);
      this.nominalColumnsTextBox5.TabIndex = 18;
      this.nominalColumnsTextBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // missingValuesTextBox
      // 
      this.missingValuesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.missingValuesTextBox.Location = new System.Drawing.Point(358, 29);
      this.missingValuesTextBox.Name = "missingValuesTextBox";
      this.missingValuesTextBox.ReadOnly = true;
      this.missingValuesTextBox.Size = new System.Drawing.Size(82, 20);
      this.missingValuesTextBox.TabIndex = 18;
      this.missingValuesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // overviewGroupBox
      // 
      this.overviewGroupBox.Controls.Add(this.overviewTableLayoutPanel);
      this.overviewGroupBox.Location = new System.Drawing.Point(3, 0);
      this.overviewGroupBox.Name = "overviewGroupBox";
      this.overviewGroupBox.Size = new System.Drawing.Size(449, 71);
      this.overviewGroupBox.TabIndex = 20;
      this.overviewGroupBox.TabStop = false;
      this.overviewGroupBox.Text = "Overview";
      // 
      // checkInputsTargetButton
      // 
      this.checkInputsTargetButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.Inputs;
      this.checkInputsTargetButton.Location = new System.Drawing.Point(36, 19);
      this.checkInputsTargetButton.Name = "checkInputsTargetButton";
      this.checkInputsTargetButton.Size = new System.Drawing.Size(24, 24);
      this.checkInputsTargetButton.TabIndex = 14;
      this.toolTip.SetToolTip(this.checkInputsTargetButton, "Show Inputs & Target");
      this.checkInputsTargetButton.UseVisualStyleBackColor = true;
      this.checkInputsTargetButton.Click += new System.EventHandler(this.checkInputsTargetButton_Click);
      // 
      // uncheckAllButton
      // 
      this.uncheckAllButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.None;
      this.uncheckAllButton.Location = new System.Drawing.Point(66, 19);
      this.uncheckAllButton.Name = "uncheckAllButton";
      this.uncheckAllButton.Size = new System.Drawing.Size(24, 24);
      this.uncheckAllButton.TabIndex = 12;
      this.toolTip.SetToolTip(this.uncheckAllButton, "Show None");
      this.uncheckAllButton.UseVisualStyleBackColor = true;
      this.uncheckAllButton.Click += new System.EventHandler(this.uncheckAllButton_Click);
      // 
      // checkAllButton
      // 
      this.checkAllButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.All;
      this.checkAllButton.Location = new System.Drawing.Point(6, 19);
      this.checkAllButton.Name = "checkAllButton";
      this.checkAllButton.Size = new System.Drawing.Size(24, 24);
      this.checkAllButton.TabIndex = 13;
      this.toolTip.SetToolTip(this.checkAllButton, "Show All");
      this.checkAllButton.UseVisualStyleBackColor = true;
      this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
      // 
      // StatisticsView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.overviewGroupBox);
      this.Controls.Add(this.orientationGroupBox);
      this.Controls.Add(this.showVariablesGroupBox);
      this.Controls.Add(this.stringMatrixView);
      this.Name = "StatisticsView";
      this.Size = new System.Drawing.Size(661, 451);
      this.showVariablesGroupBox.ResumeLayout(false);
      this.orientationGroupBox.ResumeLayout(false);
      this.orientationGroupBox.PerformLayout();
      this.overviewTableLayoutPanel.ResumeLayout(false);
      this.overviewTableLayoutPanel.PerformLayout();
      this.overviewGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblRows;
    private System.Windows.Forms.Label lblColumns;
    private System.Windows.Forms.Label lblMissingValuesTotal;
    private System.Windows.Forms.Label lblNumericColumns;
    private System.Windows.Forms.Label lblNominalColumns;
    private HeuristicLab.Data.Views.StringConvertibleMatrixView stringMatrixView;
    private System.Windows.Forms.GroupBox showVariablesGroupBox;
    private System.Windows.Forms.Button checkInputsTargetButton;
    private System.Windows.Forms.Button uncheckAllButton;
    private System.Windows.Forms.Button checkAllButton;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.GroupBox orientationGroupBox;
    private System.Windows.Forms.RadioButton verticalRadioButton;
    private System.Windows.Forms.RadioButton horizontalRadioButton;
    private System.Windows.Forms.Label lblTNumValues;
    private System.Windows.Forms.TextBox rowsTextBox;
    private System.Windows.Forms.TextBox columnsTextBox;
    private System.Windows.Forms.TextBox totalValuesTextBox;
    private System.Windows.Forms.TableLayoutPanel overviewTableLayoutPanel;
    private System.Windows.Forms.TextBox numericColumnsTextBox;
    private System.Windows.Forms.TextBox nominalColumnsTextBox5;
    private System.Windows.Forms.TextBox missingValuesTextBox;
    private System.Windows.Forms.GroupBox overviewGroupBox;
  }
}
