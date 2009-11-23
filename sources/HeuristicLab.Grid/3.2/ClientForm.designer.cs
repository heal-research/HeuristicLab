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

namespace HeuristicLab.Grid {
  partial class ClientForm {
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
      this.label1 = new System.Windows.Forms.Label();
      this.stopButton = new System.Windows.Forms.Button();
      this.startButton = new System.Windows.Forms.Button();
      this.addressTextBox = new System.Windows.Forms.TextBox();
      this.timer = new System.Windows.Forms.Timer(this.components);
      this.nClientsControl = new System.Windows.Forms.NumericUpDown();
      this.clientGrid = new System.Windows.Forms.DataGridView();
      this.clientControllerBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.clientDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.messageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.runningDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.nClientsControl)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.clientGrid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.clientControllerBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(81, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "&Server address:";
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Enabled = false;
      this.stopButton.Location = new System.Drawing.Point(93, 142);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(75, 23);
      this.stopButton.TabIndex = 6;
      this.stopButton.Text = "St&op All";
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Location = new System.Drawing.Point(12, 142);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(75, 23);
      this.startButton.TabIndex = 5;
      this.startButton.Text = "St&art All";
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // addressTextBox
      // 
      this.addressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.addressTextBox.Location = new System.Drawing.Point(96, 6);
      this.addressTextBox.Name = "addressTextBox";
      this.addressTextBox.Size = new System.Drawing.Size(263, 20);
      this.addressTextBox.TabIndex = 4;
      // 
      // timer
      // 
      this.timer.Enabled = true;
      this.timer.Interval = 1000;
      this.timer.Tick += new System.EventHandler(this.timer_Tick);
      // 
      // nClientsControl
      // 
      this.nClientsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.nClientsControl.Location = new System.Drawing.Point(365, 6);
      this.nClientsControl.Name = "nClientsControl";
      this.nClientsControl.Size = new System.Drawing.Size(48, 20);
      this.nClientsControl.TabIndex = 11;
      this.nClientsControl.ValueChanged += new System.EventHandler(this.nClientsControl_ValueChanged);
      // 
      // clientGrid
      // 
      this.clientGrid.AllowUserToAddRows = false;
      this.clientGrid.AllowUserToDeleteRows = false;
      this.clientGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.clientGrid.AutoGenerateColumns = false;
      this.clientGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.clientGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clientDataGridViewTextBoxColumn,
            this.statusDataGridViewTextBoxColumn,
            this.messageDataGridViewTextBoxColumn,
            this.runningDataGridViewCheckBoxColumn});
      this.clientGrid.DataSource = this.clientControllerBindingSource;
      this.clientGrid.Location = new System.Drawing.Point(12, 32);
      this.clientGrid.Name = "clientGrid";
      this.clientGrid.RowHeadersVisible = false;
      this.clientGrid.Size = new System.Drawing.Size(401, 104);
      this.clientGrid.TabIndex = 12;
      // 
      // clientControllerBindingSource
      // 
      this.clientControllerBindingSource.DataSource = typeof(HeuristicLab.Grid.ClientController);
      // 
      // clientDataGridViewTextBoxColumn
      // 
      this.clientDataGridViewTextBoxColumn.DataPropertyName = "Client";
      this.clientDataGridViewTextBoxColumn.HeaderText = "Client";
      this.clientDataGridViewTextBoxColumn.Name = "clientDataGridViewTextBoxColumn";
      this.clientDataGridViewTextBoxColumn.ReadOnly = true;
      this.clientDataGridViewTextBoxColumn.Visible = false;
      // 
      // statusDataGridViewTextBoxColumn
      // 
      this.statusDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
      this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
      this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
      this.statusDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // messageDataGridViewTextBoxColumn
      // 
      this.messageDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.messageDataGridViewTextBoxColumn.DataPropertyName = "Message";
      this.messageDataGridViewTextBoxColumn.HeaderText = "Message";
      this.messageDataGridViewTextBoxColumn.Name = "messageDataGridViewTextBoxColumn";
      this.messageDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // runningDataGridViewCheckBoxColumn
      // 
      this.runningDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.runningDataGridViewCheckBoxColumn.DataPropertyName = "Running";
      this.runningDataGridViewCheckBoxColumn.HeaderText = "Running";
      this.runningDataGridViewCheckBoxColumn.Name = "runningDataGridViewCheckBoxColumn";
      this.runningDataGridViewCheckBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      // 
      // ClientForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(425, 177);
      this.Controls.Add(this.clientGrid);
      this.Controls.Add(this.nClientsControl);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.addressTextBox);
      this.Name = "ClientForm";
      this.Text = "Grid Client";
      ((System.ComponentModel.ISupportInitialize)(this.nClientsControl)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.clientGrid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.clientControllerBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.TextBox addressTextBox;
    private System.Windows.Forms.Timer timer;
    private System.Windows.Forms.NumericUpDown nClientsControl;
    private System.Windows.Forms.DataGridView clientGrid;
    private System.Windows.Forms.BindingSource clientControllerBindingSource;
    private System.Windows.Forms.DataGridViewTextBoxColumn clientDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn messageDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewCheckBoxColumn runningDataGridViewCheckBoxColumn;
  }
}
