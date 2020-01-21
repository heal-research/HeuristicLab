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
    private IDictionary<int, UserControl> ctrlCollection = new Dictionary<int, UserControl>();
    private JsonItem root;
    private IOptimizer optimizer;
    private IList<JsonItemVM> vms;
    private JCGenerator generator = new JCGenerator();
    public IContent Content {
      get => content;
      set {
        content = value;

        //IEnumerable<JsonItem> items = generator.FetchJsonItems(content as IOptimizer);
        vms = new List<JsonItemVM>();
        treeView.Nodes.Clear();

        optimizer = content as IOptimizer;
        root = JsonItemConverter.Extract(optimizer);
        TreeNode parent = new TreeNode(root.Name);
        BuildTreeNode(parent, root);
        treeView.Nodes.Add(parent);
      } 
    }

    public ExportJsonDialog() {
      InitializeComponent();
    }

    private void exportButton_Click(object sender, EventArgs e) {
      foreach(var x in vms) {
        if (!x.Selected) {
          x.Item.Parent.Children.Remove(x.Item);
        }
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
        File.WriteAllText(saveFileDialog.FileName, generator.GenerateTemplate(root, optimizer));
      }

      this.Close();
    }

    private JsonItemVM BuildTreeNode(TreeNode node, JsonItem item) {
      JsonItemVM vm = new JsonItemVM(item);

      vms.Add(vm);
      ctrlCollection.Add(node.GetHashCode(), GenerateControl(vm));
      if (item.Children != null) {
        foreach (var c in item.Children) {
          if (IsDrawableItem(c)) {
            if (c is ResultItem) {

            } else {
              TreeNode childNode = new TreeNode(c.Name);
              node.Nodes.Add(childNode);
              vm.AddChild(BuildTreeNode(childNode, c));
            }
          }
        }
      }
      
      return vm;
    }

    private bool IsDrawableItem(JsonItem item) {
      bool b = false;
      if (item.Children != null) {
        foreach (var c in item.Children) {
          b = b || IsDrawableItem(c);
        }
      }
      
      return b || (item.Value != null || item.Range != null || item.ActualName != null);
    }
    
    private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if(ctrlCollection.TryGetValue(treeView.SelectedNode.GetHashCode(), out UserControl ctrl)) {
        panel.Controls.Clear();
        if (ctrl != null) {
          panel.Controls.Add(ctrl);
        }
        panel.Refresh();
      }
    }

    private UserControl GenerateControl(JsonItemVM vm) {
      JsonItem item = vm.Item;
      UserControl control = null;
      if (!(item is UnsupportedJsonItem)) {
        if (item.Value is string && item.Range != null) {
          control = new JsonItemValidValuesControl(vm);
        } else if (item.Value is bool && item.Range != null) {
          control = new JsonItemBoolControl(vm);
        } else if (item.Value is int && item.Range != null) {
          control = new JsonItemValueControl(vm, false);
        } else if (item.Value is double && item.Range != null) {
          control = new JsonItemValueControl(vm, true);
        } else if (item.Value is Array) {
          Array arr = (Array)item.Value;
          if (arr.Length == 2 && arr.GetValue(0) is int && item.Range != null)
            control = new JsonItemRangeControl(vm, false);
          else if (arr.Length == 2 && arr.GetValue(0) is double && item.Range != null)
            control = new JsonItemRangeControl(vm, true);
          else if (arr.Rank == 1 && arr.GetValue(0) is double) {
            control = new JsonItemArrayControl(vm);
          }
        } else {
          control = new JsonItemBaseControl(vm);
        }
        if (control != null) {
          control.Dock = DockStyle.Fill;
          control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
      }
      return control;
    }
  }
}
