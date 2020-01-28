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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ExportJsonDialog : Form {
    private IContent content;
    private static SaveFileDialog saveFileDialog;
    private IDictionary<int, UserControl> ctrlCollection = new Dictionary<int, UserControl>();
    private IJsonItem root;
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

    private IDictionary<Type, JsonItemVMBase> VMs { get; set; }


    private void InitCache() {
      VMs = new Dictionary<Type, JsonItemVMBase>();
      foreach (var vm in ApplicationManager.Manager.GetInstances<JsonItemVMBase>()) {
        VMs.Add(vm.JsonItemType, vm);
      }
    }

    public ExportJsonDialog() {
      InitializeComponent();
      InitCache();
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

    private void BuildTreeNode(TreeNode node, IJsonItem item) {

      if (VMs.TryGetValue(item.GetType(), out JsonItemVMBase vm)) {
        //vm.Item = item;
        //UserControl control = vm.GetControl();
        //if (control != null) {
        //  control.Dock = DockStyle.Fill;
        //  control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        //}
        //ctrlCollection.Add(node.GetHashCode(), control);
        if (item.Children != null) {
          foreach (var c in item.Children) {
            if (IsDrawableItem(c)) {
              if (c is ResultItem) {

              } else {
                TreeNode childNode = new TreeNode(c.Name);
                node.Nodes.Add(childNode);
                BuildTreeNode(childNode, c);
                //vm.AddChild(BuildTreeNode(childNode, c));
              }
            }
          }
        }
      } else {
        Console.WriteLine();
      }
    }

    private bool IsDrawableItem(IJsonItem item) {
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
  }
}
