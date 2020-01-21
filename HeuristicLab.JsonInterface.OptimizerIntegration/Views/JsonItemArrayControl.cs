using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemArrayControl : JsonItemBaseControl {
    public JsonItemArrayControl(JsonItemVM vm) : base(vm) {
      InitializeComponent();
      dataGridView.Columns.Add("Values", "Values");
      foreach(var val in ((Array)VM.Item.Value)) {
        dataGridView.Rows.Add(val);
      }
    }
  }
}
