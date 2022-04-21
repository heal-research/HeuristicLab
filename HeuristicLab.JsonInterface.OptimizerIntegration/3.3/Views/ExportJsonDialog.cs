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
    private FolderBrowserDialog FolderBrowserDialog { get; } = new FolderBrowserDialog() {
      Description = "Select .json-Template Directory"
    };

    private IDictionary<TreeNode, JsonItemVM> Node2VM { get; } = new Dictionary<TreeNode, JsonItemVM>();

    private IJsonConvertable Convertable { get; }
    //private ICheckedItemList<IRunCollectionModifier> RunCollectionModifiers { get; set; }
    #endregion

    public ExportJsonDialog(IJsonConvertable convertable) {
      InitializeComponent();
      Icon = Common.Resources.HeuristicLab.Icon;
      treeView.AfterCheck += TreeView_AfterCheck;

      var converter = new JsonItemConverter();
      var rootItem = converter.ConvertToJson(convertable);
      Convertable = convertable;

      treeView.Nodes.Add(BuildTree(rootItem));
      treeView.ExpandAll();
    }

    private TreeNode BuildTree(JsonItem rootItem) {
      if (!IsDrawableItem(rootItem)) return null;

      TreeNode node = new TreeNode(rootItem.Id);
      JsonItemVM vm = new JsonItemVM(rootItem);
      Node2VM.Add(node, vm);

      if(!ItemHasProps(rootItem)) {
        node.ForeColor = Color.LightGray;
        node.NodeFont = new Font(SystemFonts.DialogFont, FontStyle.Italic);
      }
      foreach (var kvp in rootItem.Childs) {
        var n = BuildTree(kvp.Value);
        if(n != null) node.Nodes.Add(n);
      }
      return node;

      bool IsDrawableItem(JsonItem item) {
        bool drawable = false;
        foreach(var i in item.Iterate())
          drawable = drawable || ItemHasProps(i);
        return drawable;
      }

      bool ItemHasProps(JsonItem item) =>
        item.Properties
        .Select(x => x.Key)
        .Except(JsonTemplateGenerator.DefaultJsonItemPropertyFilter)
        .Any();
    }

    private void exportButton_Click(object sender, EventArgs e) {
      if (FolderBrowserDialog.ShowDialog() == DialogResult.OK) {
        try {
          JsonTemplateGenerator.GenerateTemplate(
            Path.Combine(FolderBrowserDialog.SelectedPath, textBoxTemplateName.Text), 
            Convertable, Node2VM.Values.Where(x => x.Selected).Select(x => x.Item));
          Close();
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if(Node2VM.TryGetValue(treeView.SelectedNode, out JsonItemVM vm)) {
        SetControlOnPanel(JsonItemBaseControl.Create(vm), panelParameterDetails);
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
        if (Node2VM.TryGetValue(e.Node, out JsonItemVM vm)) {
          vm.Selected = e.Node.Checked;
          e.Node.ForeColor = (e.Node.Checked ? Color.Green : Color.Black);
          treeView.Refresh();
        }
    }

    private void treeViewResults_AfterCheck(object sender, TreeViewEventArgs e) {
      if (e.Action != TreeViewAction.Unknown)
        if (Node2VM.TryGetValue(e.Node, out JsonItemVM vm))
          vm.Selected = e.Node.Checked;
    }
  }
}
