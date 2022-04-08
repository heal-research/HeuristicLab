using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ExportJsonDialog : Form {

    #region Private Properties
    private static FolderBrowserDialog FolderBrowserDialog { get; set; }
    private IDictionary<TreeNode, UserControl> Node2Control { get; set; } = new Dictionary<TreeNode, UserControl>();
    private IDictionary<TreeNode, JsonItemVMBase> Node2VM { get; } = new Dictionary<TreeNode, JsonItemVMBase>();
    private IDictionary<Type, Type> JI2VM { get; set; }
    private JsonItem Root { get; set; }
    private IJsonConvertable Convertable { get; set; }
    private IList<JsonItemVMBase> VMs { get; } = new List<JsonItemVMBase>();
    //private ICheckedItemList<IRunCollectionModifier> RunCollectionModifiers { get; set; }
    #endregion
    /*
    private IContent content;
    public IContent Content {
      get => content;
      set {
        content = value;
        #region Clear
        VMs = new List<JsonItemVMBase>();
        treeView.Nodes.Clear();
        #endregion
        Convertable = content as IJsonConvertable;
        if(Convertable != null) {
          //Convertable = (IOptimizer)Convertable.Clone(); // clone the optimizer
          var converter = new JsonItemConverter();
          Root = converter.ConvertToJson(Convertable);
          TreeNode parent = new TreeNode(Root.Name);
          treeView.AfterCheck += TreeView_AfterCheck;
          BuildTreeNode(parent, Root);
          treeView.Nodes.Add(parent);
          treeView.ExpandAll();
          panelParameterDetails.Controls.Clear();
        }
      } 
    }*/



    //private void InitCache() {
    //  JI2VM = new Dictionary<Type, Type>();
    //  foreach (var vmType in ApplicationManager.Manager.GetTypes(typeof(IJsonItemVM))) {
    //    IJsonItemVM vm = (IJsonItemVM)Activator.CreateInstance(vmType);
    //    JI2VM.Add(vm.TargetedJsonItemType, vmType);
    //  }
    //}

    public ExportJsonDialog() {
      InitializeComponent();
      Icon = Common.Resources.HeuristicLab.Icon;
      
      //RunCollectionModifiers = postProcessorListControl.Content;
      //treeView.AfterCheck += TreeView_AfterCheck;
      //InitCache();
    }

    public void SetJsonConvertable(IJsonConvertable convertable) {
      var converter = new JsonItemConverter();
      var rootItem = converter.ConvertToJson(convertable);

      treeView.Nodes.Clear();
      Node2VM.Clear();
      treeView.Nodes.Add(BuildTree(rootItem));
    }

    private TreeNode BuildTree(JsonItem rootItem) {
      TreeNode node = new TreeNode(rootItem.Name);
      Node2VM.Add(node, new JsonItemVMBase(rootItem));
      foreach (var kvp in rootItem.Childs)
        node.Nodes.Add(BuildTree(kvp.Value));
      return node;
    }


    private void exportButton_Click(object sender, EventArgs e) {
      if (FolderBrowserDialog == null) {
        FolderBrowserDialog = new FolderBrowserDialog();
        FolderBrowserDialog.Description = "Select .json-Template Directory";
      }

      if (FolderBrowserDialog.ShowDialog() == DialogResult.OK) {
        try {
          /*
          JsonTemplateGenerator.GenerateTemplate(
            Path.Combine(FolderBrowserDialog.SelectedPath, textBoxTemplateName.Text), 
            Convertable, Root, RunCollectionModifiers.CheckedItems.Select(x => x.Value));*/
          Close();
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    //private void BuildTreeNode(TreeNode node, JsonItem item) {
    //  RegisterItem(node, item, treeView);
    //  if (item.Children != null) {
    //    foreach (var c in item.Children) {
    //      if (IsDrawableItem(c)) {
    //        TreeNode childNode = new TreeNode(c.Name);
    //        node.Nodes.Add(childNode);
    //        BuildTreeNode(childNode, c);
    //      }
    //    }
    //  }
    //}

    //private IJsonItemVM RegisterItem(TreeNode node, IJsonItem item, TreeView tv) {
    //  if (JI2VM.TryGetValue(item.GetType(), out Type vmType)) {
    //    IJsonItemVM vm = (IJsonItemVM)Activator.CreateInstance(vmType);

    //    vm.Item = item;
    //    vm.TreeNode = node;
    //    vm.TreeView = tv;
    //    vm.Selected = false;

    //    VMs.Add(vm);
    //    Node2VM.Add(node, vm);
    //    UserControl control = JsonItemBaseControl.Create(vm, vm.Control);
    //    Node2Control.Add(node, control);
    //    return vm;
    //  } else {
    //    node.ForeColor = Color.LightGray;
    //    node.NodeFont = new Font(SystemFonts.DialogFont, FontStyle.Italic);
    //  }
    //  return null;
    //}

    //private bool IsDrawableItem(IJsonItem item) {
    //  bool b = false;
    //  if (item.Children != null)
    //    foreach (var c in item.Children)
    //      b = b || IsDrawableItem(c);
      
    //  return b || !(item is EmptyJsonItem) || !(item is UnsupportedJsonItem);
    //}
    
    private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if(Node2Control.TryGetValue(treeView.SelectedNode, out UserControl control)) {
        SetControlOnPanel(control, panelParameterDetails);
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
      if (e.Action != TreeViewAction.Unknown)
        if (Node2VM.TryGetValue(e.Node, out JsonItemVMBase vm))
          vm.Selected = e.Node.Checked;
    }

    private void treeViewResults_AfterCheck(object sender, TreeViewEventArgs e) {
      if (e.Action != TreeViewAction.Unknown)
        if (Node2VM.TryGetValue(e.Node, out JsonItemVMBase vm))
          vm.Selected = e.Node.Checked;
    }
  }
}
