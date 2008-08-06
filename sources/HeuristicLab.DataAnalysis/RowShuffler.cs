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
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace HeuristicLab.DataAnalysis {
  class RowShuffler : Form, IDatasetManipulator {
    private Label helpLabel;
    private Button nextButton;
    private DataGridView dataGridView;
    private Dataset dataset;
    public RowShuffler()
      : base() {
      InitializeComponent();
    }


    #region IDatasetManipulator Members

    public string Action {
      get { return "Shuffle rows..."; }
    }

    public void Execute(Dataset dataset) {
      this.dataset = dataset;
      dataGridView.ColumnCount = dataset.Columns;
      dataGridView.RowCount = dataset.Rows;
      for(int i = 0; i < dataset.Rows; i++) {
        for(int j = 0; j < dataset.Columns; j++) {
          dataGridView.Rows[i].Cells[j].Value = dataset.GetValue(i, j);
          dataGridView.Rows[i].HeaderCell.Value = i.ToString();
        }
      }
      for(int i = 0; i < dataset.Columns; i++) {
        dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridView.Columns[i].Name = dataset.VariableNames[i];
      }
      dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

      ShowDialog();
    }

    private void ExchangeRows(Dataset dataset, int i, int j) {
      for(int k = 0; k < dataset.Columns; k++) {
        double temp = dataset.GetValue(i, k);
        dataset.SetValue(i, k, dataset.GetValue(j, k));
        dataset.SetValue(j, k, temp);
      }
    }

    #endregion

    private void InitializeComponent() {
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.helpLabel = new System.Windows.Forms.Label();
      this.nextButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Location = new System.Drawing.Point(12, 25);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.ReadOnly = true;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
      this.dataGridView.Size = new System.Drawing.Size(607, 444);
      this.dataGridView.TabIndex = 0;
      this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
      // 
      // helpLabel
      // 
      this.helpLabel.AutoSize = true;
      this.helpLabel.Location = new System.Drawing.Point(12, 9);
      this.helpLabel.Name = "helpLabel";
      this.helpLabel.Size = new System.Drawing.Size(183, 13);
      this.helpLabel.TabIndex = 1;
      this.helpLabel.Text = "Please select the rows to be shuffled.";
      // 
      // nextButton
      // 
      this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.nextButton.Location = new System.Drawing.Point(544, 475);
      this.nextButton.Name = "nextButton";
      this.nextButton.Size = new System.Drawing.Size(75, 23);
      this.nextButton.TabIndex = 2;
      this.nextButton.Text = "Next...";
      this.nextButton.UseVisualStyleBackColor = true;
      this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
      // 
      // RowShuffler
      // 
      this.ClientSize = new System.Drawing.Size(631, 510);
      this.Controls.Add(this.nextButton);
      this.Controls.Add(this.helpLabel);
      this.Controls.Add(this.dataGridView);
      this.Name = "RowShuffler";
      this.Text = "Row shuffler";
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    private void dataGridView_SelectionChanged(object sender, EventArgs e) {
      if(dataGridView.SelectedRows.Count > 1) nextButton.Enabled = true;
      else nextButton.Enabled = false;
    }

    private void nextButton_Click(object sender, EventArgs e) {
      Random random = new Random();
      for(int i = 0; i < dataGridView.SelectedRows.Count - 1; i++) {
        int j = random.Next(i, dataGridView.SelectedRows.Count);
        int col0 = dataGridView.SelectedRows[i].Index;
        int col1 = dataGridView.SelectedRows[j].Index;
        ExchangeRows(dataset, i, j);
      }

      Close();
    }
  }
}
