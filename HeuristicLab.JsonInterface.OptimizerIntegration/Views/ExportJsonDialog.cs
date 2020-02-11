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
    private static SaveFileDialog SaveFileDialog { get; set; }
    private IDictionary<int, UserControl> Hash2Control { get; set; } = new Dictionary<int, UserControl>();
    private IJsonItem Root { get; set; }
    private IOptimizer Optimizer { get; set; }
    private IList<JsonItemVMBase> VMs { get; set; }
    private JCGenerator Generator { get; set; } = new JCGenerator();
    private IDictionary<string, IJsonItem> ResultItems = new Dictionary<string, IJsonItem>();

    private IContent content;
    public IContent Content {
      get => content;
      set {
        content = value;

        VMs = new List<JsonItemVMBase>();
        treeView.Nodes.Clear();
        ResultItems.Clear();
        
        Optimizer = content as IOptimizer;
        Root = JsonItemConverter.Extract(Optimizer);
        TreeNode parent = new TreeNode(Root.Name);

        BuildTreeNode(parent, Root);
        treeView.Nodes.Add(parent);
        treeView.ExpandAll();
        
      } 
    }

    private IDictionary<Type, Type> JI2VM { get; set; }

    private void InitCache() {
      JI2VM = new Dictionary<Type, Type>();
      foreach (var vmType in ApplicationManager.Manager.GetTypes(typeof(JsonItemVMBase))) {
        JsonItemVMBase vm = (JsonItemVMBase)Activator.CreateInstance(vmType);
        JI2VM.Add(vm.JsonItemType, vmType);
      }
    }

    public ExportJsonDialog() {
      InitializeComponent();
      InitCache();
    }

    private void exportButton_Click(object sender, EventArgs e) {
      foreach(var x in VMs) {
        if (!x.Selected) {
          x.Item.Parent.Children.Remove(x.Item);
        }
      }

      foreach(var x in ResultItems.Values) {
        x.Parent.Children.Remove(x);
      }

      foreach(var x in resultItems.CheckedItems) {
        if(ResultItems.TryGetValue((string)x, out IJsonItem item)) {
          Root.AddChildren(item);
        }
      }

      IList<IJsonItem> faultyItems = new List<IJsonItem>();
      if(!Root.GetValidator().Validate(ref faultyItems)) {
        //print faultyItems
      }

      if (SaveFileDialog == null) {
        SaveFileDialog = new SaveFileDialog();
        SaveFileDialog.Title = "Export .json-Template";
        SaveFileDialog.DefaultExt = "json";
        SaveFileDialog.Filter = ".json-Template|*.json|All Files|*.*";
        SaveFileDialog.FilterIndex = 1;
      }
      
      SaveFileDialog.FileName = "template";

      if (SaveFileDialog.ShowDialog() == DialogResult.OK) {
        File.WriteAllText(SaveFileDialog.FileName, Generator.GenerateTemplate(Root, Optimizer));
      }

      Close();
    }

    private void BuildTreeNode(TreeNode node, IJsonItem item) {
      if (JI2VM.TryGetValue(item.GetType(), out Type vmType)) {
        JsonItemVMBase vm = (JsonItemVMBase)Activator.CreateInstance(vmType);
        VMs.Add(vm);
        vm.Item = item;
        vm.TreeNode = node;
        vm.TreeView = treeView;
        UserControl control = vm.GetControl();
        Hash2Control.Add(node.GetHashCode(), control);
        if (item.Children != null) {
          foreach (var c in item.Children) {
            if (IsDrawableItem(c)) {
              if (c is ResultItem) {
                resultItems.Items.Add(c.Name, true);
                ResultItems.Add(c.Name, c);
              } else {
                TreeNode childNode = new TreeNode(c.Name);
                node.Nodes.Add(childNode);
                BuildTreeNode(childNode, c);
              }
            }
          }
        }
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
      if(Hash2Control.TryGetValue(treeView.SelectedNode.GetHashCode(), out UserControl control)) {
        panel.Controls.Clear();
        if (control != null) {
          panel.Controls.Add(control);
          control.Width = panel.Width;
          control.Dock = DockStyle.Fill;
          control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
        panel.Refresh();
      }
    }
  }
}
