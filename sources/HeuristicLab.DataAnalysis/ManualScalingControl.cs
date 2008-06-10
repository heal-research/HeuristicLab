using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;

namespace HeuristicLab.DataAnalysis {
  public partial class ManualScalingControl : Form {
    public ManualScalingControl(bool editingAllowed) {
      InitializeComponent();
      dataGridView.ReadOnly = !editingAllowed;
      okButton.Text = editingAllowed ? "Scale" : "Close";
      cancelButton.Visible = editingAllowed;
      cancelButton.Text = "Cancel";
    }

    private double[,] data;
    public double[,] Data {
      get {
        return data;
      }
      set {
        data = value;
        UpdateControls();
      }
    }

    protected void UpdateControls() {
      Refresh();
      dataGridView.ColumnCount = data.GetLength(0);
      dataGridView.RowCount = data.GetLength(1);
      for(int i = 0; i < dataGridView.RowCount; i++) {
        for(int j = 0; j < dataGridView.ColumnCount; j++) {
          dataGridView[j,i].Value = data[j, i];
        }
      }
      dataGridView.Columns[0].HeaderText = "Factor";
      dataGridView.Columns[1].HeaderText = "Offset";
    }

    private void button_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.OK;
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.Cancel;
    }

    // stolen from the internets
    private void dataGridView_KeyDown(object sender, KeyEventArgs e) {
      if(e.Control && e.KeyCode == Keys.C) {
        DataObject d = dataGridView.GetClipboardContent();
        Clipboard.SetDataObject(d);
        e.Handled = true;
      } else if(e.Control && e.KeyCode == Keys.V) {
        string s = Clipboard.GetText();
        string[] lines = s.Split(new char[] {'\n', '\r'},StringSplitOptions.RemoveEmptyEntries);
        int row = dataGridView.CurrentCell.RowIndex;
        int col = dataGridView.CurrentCell.ColumnIndex;
        foreach(string line in lines) {
          if(row < dataGridView.RowCount && line.Length > 0) {
            string[] cells = line.Split(new char[] {'\t',' ',';'},StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < cells.Length; i++) {
              if(col + i < this.dataGridView.ColumnCount) {
                dataGridView[col + i, row].Value = double.Parse(cells[i]);
              } else {
                break;
              }
            }
            row++;
          } else {
            break;
          }
        }
        e.Handled = true;
      }
    }

    private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
      if(e.RowIndex >= 0 && e.ColumnIndex >= 0) {
        data[e.ColumnIndex, e.RowIndex] = (double)dataGridView[e.ColumnIndex, e.RowIndex].Value;
      }
    }
  }
}
