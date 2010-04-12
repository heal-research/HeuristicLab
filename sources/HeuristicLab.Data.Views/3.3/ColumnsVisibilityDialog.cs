using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Data.Views {
  public partial class ColumnsVisibilityDialog : Form {
    public ColumnsVisibilityDialog() {
      InitializeComponent();
    }

    public ColumnsVisibilityDialog(IEnumerable<DataGridViewColumn> columns) :this() {
      this.Columns = columns;
    }

    private List<DataGridViewColumn> columns;
    public IEnumerable<DataGridViewColumn> Columns {
      get { return this.columns; }
      set {
        this.columns = new List<DataGridViewColumn>(value);
        UpdateCheckBoxes();
      }
    }

    private void UpdateCheckBoxes() {
      this.checkedListBox.Items.Clear();
      foreach (DataGridViewColumn column in columns)
        checkedListBox.Items.Add(column.HeaderText, column.Visible);
    }

    private void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
      this.columns[e.Index].Visible = e.NewValue == CheckState.Checked;
    }
  }
}
