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
    private IDictionary<TreeNode, UserControl> Node2Control { get; set; } = new Dictionary<TreeNode, UserControl>();
    private IDictionary<TreeNode, IJsonItemVM> Node2VM { get; set; } = new Dictionary<TreeNode, IJsonItemVM>();
    private IDictionary<Type, Type> JI2VM { get; set; }
    private IJsonItem Root { get; set; }
    private IOptimizer Optimizer { get; set; }
    private IList<IJsonItemVM> VMs { get; set; }
    private JCGenerator Generator { get; set; } = new JCGenerator();

    private IContent content;
    public IContent Content {
      get => content;
      set {
        content = value;

        VMs = new List<IJsonItemVM>();
        treeView.Nodes.Clear();
        treeViewResults.Nodes.Clear();

        Optimizer = content as IOptimizer;
        Root = JsonItemConverter.Extract(Optimizer);
        TreeNode parent = new TreeNode(Root.Name);
        treeView.AfterCheck += TreeView_AfterCheck;
        BuildTreeNode(parent, Root);
        treeView.Nodes.Add(parent);
        treeView.ExpandAll();
        
      } 
    }

    


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
      // to set default value for disabled items
      JsonItemConverter.Inject(Optimizer, Root);

      // clear all runs
      Optimizer.Runs.Clear();
      
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
          Close();
        }
      }
    }

    private void BuildTreeNode(TreeNode node, IJsonItem item) {
      RegisterItem(node, item, treeView);
      if (item.Children != null) {
        foreach (var c in item.Children) {
          if (IsDrawableItem(c)) {
            if (c is ResultJsonItem) {
              TreeNode childNode = new TreeNode(c.Name);
              treeViewResults.Nodes.Add(childNode);
              RegisterItem(childNode, c, treeViewResults);
              if(Node2VM.TryGetValue(childNode, out IJsonItemVM vm))
                vm.Selected = true;
            } else {
              TreeNode childNode = new TreeNode(c.Name);
              node.Nodes.Add(childNode);
              BuildTreeNode(childNode, c);
            }
          }
        }
      }
    }

    private void RegisterItem(TreeNode node, IJsonItem item, TreeView tv) {
      if (JI2VM.TryGetValue(item.GetType(), out Type vmType)) {
        IJsonItemVM vm = (IJsonItemVM)Activator.CreateInstance(vmType);

        vm.Item = item;
        vm.TreeNode = node;
        vm.TreeView = tv;
        vm.Selected = false;

        VMs.Add(vm);
        Node2VM.Add(node, vm);
        UserControl control = vm.Control;
        Node2Control.Add(node, control);
      }
    }

    private bool IsDrawableItem(IJsonItem item) {
      bool b = false;
      if (item.Children != null) {
        foreach (var c in item.Children) {
          b = b || IsDrawableItem(c);
        }
      }
      
      return b || (item.Value != null || item.Range != null || item.ActualName != null || item is ResultJsonItem);
    }
    
    private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if(Node2Control.TryGetValue(treeView.SelectedNode, out UserControl control)) {
        SetControlOnPanel(control, panelParameterDetails);
      }
    }

    private void treeViewResults_AfterSelect(object sender, TreeViewEventArgs e) {
      if (Node2Control.TryGetValue(treeViewResults.SelectedNode, out UserControl control)) {
        SetControlOnPanel(control, panelResultDetails);
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

    private void SetControlOnPanel(UserControl control, Panel panel) {
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

    private void TreeView_AfterCheck(object sender, TreeViewEventArgs e) {
      if (e.Action != TreeViewAction.Unknown) {
        if (Node2VM.TryGetValue(e.Node, out IJsonItemVM vm)) {
          vm.Selected = e.Node.Checked;
        }
      }
    }

    private void treeViewResults_AfterCheck(object sender, TreeViewEventArgs e) {
      if (e.Action != TreeViewAction.Unknown) {
        if (Node2VM.TryGetValue(e.Node, out IJsonItemVM vm)) {
          vm.Selected = e.Node.Checked;
        }
      }
    }
  }
}
