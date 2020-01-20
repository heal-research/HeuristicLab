using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ExportJsonDialog : Form {
    private IContent content;
    private static SaveFileDialog saveFileDialog;

    private IList<JsonItemVM> vms;
    private JCGenerator generator = new JCGenerator();
    public IContent Content {
      get => content;
      set {
        content = value;

        IEnumerable<JsonItem> items = generator.FetchJsonItems(content as IOptimizer);
        vms = new List<JsonItemVM>();
        tabel.Controls.Clear();

        foreach (var item in items) {
          if (!(item is UnsupportedJsonItem) && item.Value != null && item.Range != null) {
            JsonItemVM vm = new JsonItemVM(item);
            vms.Add(vm);
            UserControl control = null;
            if(item.Value is string) {
              control = new JsonItemValidValuesControl(vm);
            } else if (item.Value is bool) {
              control = new JsonItemBoolControl(vm);
            } else if (item.Value is int) {
              control = new JsonItemValueControl(vm, false);
            } else if (item.Value is double) {
              control = new JsonItemValueControl(vm, true);
            } else if (item.Value is Array) {
              if(((Array)item.Value).GetValue(0) is int)
                control = new JsonItemRangeControl(vm, false);
              else if (((Array)item.Value).GetValue(0) is double)
                control = new JsonItemRangeControl(vm, true);
            }
            if (control != null) {
              control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
              tabel.Controls.Add(control);
            }
          }
        }
      } 
    }

    public ExportJsonDialog() {
      InitializeComponent();
    }

    private void exportButton_Click(object sender, EventArgs e) {
      IList<JsonItem> items = new List<JsonItem>();

      foreach(var x in vms) {
        if (x.Selected)
          items.Add(x.Item);
      }

      if (saveFileDialog == null) {
        saveFileDialog = new SaveFileDialog();
        saveFileDialog.Title = "Export .json-Template";
        saveFileDialog.DefaultExt = "json";
        saveFileDialog.Filter = ".json-Template|*.json|All Files|*.*";
        saveFileDialog.FilterIndex = 1;
      }
      
      saveFileDialog.FileName = "template";

      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        File.WriteAllText(saveFileDialog.FileName, generator.GenerateTemplate(items));
      }

      this.Close();
    }
  }
}
