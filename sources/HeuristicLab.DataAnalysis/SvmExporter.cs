using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace HeuristicLab.DataAnalysis {
  class SvmExporter : Form, IDatasetManipulator {
    private Label helpLabel;
    private Button nextButton;
    private DataGridView dataGridView;
    private Dataset dataset;
    public SvmExporter() : base() {
      InitializeComponent();
    }

    #region IDatasetManipulator Members
    public string Action {
      get { return "Export to libsvm..."; }
    }

    public void Execute(Dataset dataset) {
      this.dataset = dataset;
      dataGridView.ColumnCount = dataset.Columns;
      dataGridView.RowCount = dataset.Rows;
      for(int i = 0; i < dataset.Rows; i++) {
        for(int j = 0; j < dataset.Columns; j++) {
          dataGridView.Rows[i].Cells[j].Value = dataset.GetValue(i, j);
        }
      }
      for(int i = 0; i < dataset.Columns; i++) {
        dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridView.Columns[i].Name = dataset.VariableNames[i];
      }
      dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

      ShowDialog();
    }

    #endregion

    private void InitializeComponent() {
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
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Location = new System.Drawing.Point(12, 25);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.ReadOnly = true;
      this.dataGridView.Size = new System.Drawing.Size(568, 200);
      this.dataGridView.TabIndex = 0;
      this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
      // 
      // helpLabel
      // 
      this.helpLabel.AutoSize = true;
      this.helpLabel.Location = new System.Drawing.Point(12, 9);
      this.helpLabel.Name = "helpLabel";
      this.helpLabel.Size = new System.Drawing.Size(231, 13);
      this.helpLabel.TabIndex = 1;
      this.helpLabel.Text = "Please select the column of  the target variable.";
      // 
      // nextButton
      // 
      this.nextButton.Location = new System.Drawing.Point(505, 231);
      this.nextButton.Name = "nextButton";
      this.nextButton.Size = new System.Drawing.Size(75, 23);
      this.nextButton.TabIndex = 2;
      this.nextButton.Text = "Next...";
      this.nextButton.UseVisualStyleBackColor = true;
      this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
      // 
      // SvmExporter
      // 
      this.ClientSize = new System.Drawing.Size(592, 266);
      this.Controls.Add(this.nextButton);
      this.Controls.Add(this.helpLabel);
      this.Controls.Add(this.dataGridView);
      this.Name = "SvmExporter";
      this.Text = "Execute to libSVM";
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    private void dataGridView_SelectionChanged(object sender, EventArgs e) {
      if(dataGridView.SelectedColumns.Count == 1) nextButton.Enabled = true;
      else nextButton.Enabled = false;
    }

    private void nextButton_Click(object sender, EventArgs e) {
      int targetColumn = dataGridView.SelectedColumns[0].Index;

      SaveFileDialog saveDialog = new SaveFileDialog();
      if(saveDialog.ShowDialog() == DialogResult.OK) {
        string filename = saveDialog.FileName;
        StreamWriter writer = new StreamWriter(filename);
        for(int i = 0; i < dataset.Rows; i++) {
          writer.Write(dataset.GetValue(i, targetColumn).ToString("r", CultureInfo.InvariantCulture) + "\t");
          for(int j = 0; j < dataset.Columns; j++) {
            if(j != targetColumn) {
              double val = dataset.GetValue(i, j);
              if(!double.IsInfinity(val) && !double.IsNaN(val))
                writer.Write((j + 1) + ":" + val.ToString("r", CultureInfo.InvariantCulture) + "\t");
            }
          }
          writer.WriteLine();
        }
        writer.Close();
      }
      Close();
    }
  }
}
