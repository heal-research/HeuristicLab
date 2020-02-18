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
    private static FolderBrowserDialog FolderBrowserDialog { get; set; }
    private IDictionary<int, UserControl> Hash2Control { get; set; } = new Dictionary<int, UserControl>();
    private IDictionary<TreeNode, JsonItemVMBase> Node2VM { get; set; } = new Dictionary<TreeNode, JsonItemVMBase>();
    private IJsonItem Root { get; set; }
    private IOptimizer Optimizer { get; set; }
    private IList<JsonItemVMBase> VMs { get; set; }
    private JCGenerator Generator { get; set; } = new JCGenerator();
    private IDictionary<string, IJsonItem> ResultItems { get; set; } = new Dictionary<string, IJsonItem>();

    private IContent content;
    public IContent Content {
      get => content;
      set {
        content = value;

        VMs = new List<JsonItemVMBase>();
        treeView.Nodes.Clear();
        ResultItems.Clear();
        resultItems.Items.Clear();

        Optimizer = content as IOptimizer;
        Root = JsonItemConverter.Extract(Optimizer);
        TreeNode parent = new TreeNode(Root.Name);
        treeView.AfterCheck += TreeView_AfterCheck;
        BuildTreeNode(parent, Root);
        treeView.Nodes.Add(parent);
        treeView.ExpandAll();
        
      } 
    }

    private void TreeView_AfterCheck(object sender, TreeViewEventArgs e) {
      if (e.Action != TreeViewAction.Unknown) {
        if (Node2VM.TryGetValue(e.Node, out JsonItemVMBase vm)) {
          vm.Selected = e.Node.Checked;
        }
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
      foreach (var x in VMs) {
        if (!x.Selected) {
          x.Item.Parent.Children.Remove(x.Item);
        }
      }

      foreach (var x in ResultItems.Values) {
        x.Parent.Children.Remove(x);
      }

      foreach (var x in resultItems.CheckedItems) {
        if (ResultItems.TryGetValue((string)x, out IJsonItem item)) {
          Root.AddChildren(item);
        }
      }

      IList<IJsonItem> faultyItems = new List<IJsonItem>();
      
      if (!Root.GetValidator().Validate(ref faultyItems)) {
        IList<Exception> list = new List<Exception>();
        //print faultyItems
        foreach (var x in faultyItems) {
          list.Add(new Exception($"Combination of value and range is not valid for {x.Name}"));
        }
        ErrorHandling.ShowErrorDialog(this, new AggregateException(list));
      } else {
        if (FolderBrowserDialog == null) {
          FolderBrowserDialog = new FolderBrowserDialog();
          FolderBrowserDialog.Description = "Select .json-Template Dictionary";
        }

        if (FolderBrowserDialog.ShowDialog() == DialogResult.OK) {
          Generator.GenerateTemplate(FolderBrowserDialog.SelectedPath, textBoxTemplateName.Text, Optimizer, Root);
        }

        Close();
      }
    }

    private void BuildTreeNode(TreeNode node, IJsonItem item) {
      if (JI2VM.TryGetValue(item.GetType(), out Type vmType)) {
        JsonItemVMBase vm = (JsonItemVMBase)Activator.CreateInstance(vmType);
        VMs.Add(vm);
        vm.Item = item;
        vm.TreeNode = node;
        vm.TreeView = treeView;
        Node2VM.Add(node, vm);
        node.Checked = vm.Selected;
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
          control.Height = panel.Height;
          control.Dock = DockStyle.Fill;
          control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
        panel.Refresh();
      }
    }

    private void textBoxTemplateName_Validating(object sender, CancelEventArgs e) {
      if (string.IsNullOrWhiteSpace(textBoxTemplateName.Text)) {
        errorProvider.SetError(textBoxTemplateName, "Template name must not be empty.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxTemplateName, null);
      }
    }
  }
}
