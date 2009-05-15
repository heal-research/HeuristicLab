using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Hive.Server.ServerConsole {

  public delegate void AddGroupDelegate(string name);

  public partial class AddGroup : Form {

    public event AddGroupDelegate addGroupEvent;

    public AddGroup() {
      InitializeComponent();
    }

    private void btnAdd_Click(object sender, EventArgs e) {
      if (addGroupEvent != null) {
        addGroupEvent(tbName.Text);
      }
      this.Close();
    }

    private void btnClose_Click(object sender, EventArgs e) {
      this.Close();
    }

    private void tbName_TextChanged(object sender, EventArgs e) {
      if (string.IsNullOrEmpty(tbName.Text)) {
        btnAdd.Enabled = false;
      } else {
        btnAdd.Enabled = true;
      }
    }
  }
}
