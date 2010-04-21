#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using System.CodeDom.Compiler;
using System.Reflection;
using HeuristicLab.CodeEditor;
using HeuristicLab.Core.Views;
using HeuristicLab.Operators.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Operators.Programmable {
  [View("ProgrammableOperator View")]
  [Content(typeof(ProgrammableOperator), true)]
  public partial class ProgrammableOperatorView : NamedItemView {

    public ProgrammableOperator ProgrammableOperator {
      get { return (ProgrammableOperator)base.Content; }
      set { base.Content = (ProgrammableOperator)value; }
    }

    public ProgrammableOperatorView() {
      InitializeComponent();
    }

    public ProgrammableOperatorView(ProgrammableOperator programmableOperator)
      : this() {
      ProgrammableOperator = programmableOperator;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      ProgrammableOperator.CodeChanged += ProgrammableOperator_CodeChanged;
      ProgrammableOperator.SignatureChanged += ProgrammableOperator_SignatureChanged;
    }

    protected override void DeregisterContentEvents() {
      ProgrammableOperator.CodeChanged -= ProgrammableOperator_CodeChanged;
      ProgrammableOperator.SignatureChanged -= ProgrammableOperator_SignatureChanged;
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (ProgrammableOperator == null) {
        codeEditor.Text = "";
        assembliesTreeView.Nodes.Clear();
        parameterCollectionView.Content = null;
      } else {
        codeEditor.Prefix = GetGeneratedPrefix();
        codeEditor.Suffix = @"
    return null;
  }
}";
        codeEditor.UserCode = ProgrammableOperator.Code;
        if (codeEditor.UserCode == "")
          codeEditor.UserCode = "\n\n\n";
        InitializeAssemblyList();
        InitializeNamespacesList();
        foreach (var a in ProgrammableOperator.SelectedAssemblies) {
          codeEditor.AddAssembly(a);
        }
        codeEditor.ScrollAfterPrefix();
        codeEditor.ShowCompileErrors(ProgrammableOperator.CompileErrors, "ProgrammableOperator");
        showCodeButton.Enabled = 
          ProgrammableOperator.CompilationUnitCode != null && 
          ProgrammableOperator.CompilationUnitCode.Length > 0;
        parameterCollectionView.Content = ProgrammableOperator.Parameters;
      }
      SetEnabledStateOfControls();
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      parameterCollectionView.Enabled = Content != null;
      parameterCollectionView.ReadOnly = ReadOnly;
      assembliesTreeView.Enabled = Content != null && !ReadOnly;
      namespacesTreeView.Enabled = Content != null && !ReadOnly;
      compileButton.Enabled = Content != null && !ReadOnly;
      codeEditor.Enabled = Content != null && !ReadOnly;
    }


    private string GetGeneratedPrefix() {
      StringBuilder prefix = new StringBuilder();
      foreach (var ns in ProgrammableOperator.GetSelectedAndValidNamespaces()) {
        prefix.Append("using ").Append(ns).AppendLine(";");
      }
      prefix.AppendLine();
      prefix.Append("public class ").Append(ProgrammableOperator.CompiledTypeName).AppendLine(" {");
      prefix.Append("  ").Append(ProgrammableOperator.Signature).AppendLine(" {");
      return prefix.ToString();
    }

    private void codeEditor_Validated(object sender, EventArgs e) {
      ProgrammableOperator.Code = codeEditor.UserCode;
    }

    private void Recompile() {
      this.Enabled = false;
      try {
        ProgrammableOperator.Compile();
        MessageBox.Show("Compilation successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } catch (Exception ex) {
        Auxiliary.ShowErrorMessageBox(ex);
      }
      OnContentChanged();
      this.Enabled = true;
    }

    private void compileButton_Click(object sender, EventArgs e) {
      Recompile();
    }

    #region ProgrammableOperator Events
    private void ProgrammableOperator_CodeChanged(object sender, EventArgs e) {
      codeEditor.Text = ProgrammableOperator.Code;
    }
    private void ProgrammableOperator_SignatureChanged(object sender, EventArgs args) {
      codeEditor.Prefix = GetGeneratedPrefix();
    }
    #endregion

    private void assembliesTreeView_AfterCheck(object sender, TreeViewEventArgs e) {
      if (initializing)
        return;
      Assembly a = e.Node.Tag as Assembly;
      if (a == null && e.Node.Nodes.Count > 0) {
        foreach (TreeNode n in e.Node.Nodes)
          n.Checked = e.Node.Checked;
        return;
      } else {
        if (e.Node.Checked) {
          ProgrammableOperator.SelectAssembly(a);
          codeEditor.AddAssembly(a);
        } else {
          ProgrammableOperator.UnselectAssembly(a);
          codeEditor.RemoveAssembly(a);
        }
      }
      InitializeNamespacesList();
      codeEditor.Prefix = GetGeneratedPrefix();
    }

    private bool initializing = false;
    private void InitializeAssemblyList() {
      initializing = true;
      assembliesTreeView.Enabled = false;
      namespacesTreeView.Enabled = false;
      assembliesTreeView.BeginUpdate();
      assembliesTreeView.Nodes.Clear();
      var selectedAssemblies = new HashSet<Assembly>(ProgrammableOperator.SelectedAssemblies);
      foreach (var p in ProgrammableOperator.Plugins) {
        var node = assembliesTreeView.Nodes.Add(p.Key);
        node.Tag = p;
        foreach (var a in p.Value) {
          var aNode = node.Nodes.Add(a.GetName().Name);
          aNode.Tag = a;
          if (selectedAssemblies.Contains(a))
            aNode.Checked = true;
        }
        if (node.Nodes.Count == 1 && node.Nodes[0].Name == node.Nodes[0].Name) {
          node.Tag = node.Nodes[0].Tag;
          node.Nodes.Clear();
        } else if (node.Nodes.Count > 0 && node.Nodes.Cast<TreeNode>().All(n => n.Checked)) {
          node.Checked = true;
        }
      }
      assembliesTreeView.EndUpdate();
      assembliesTreeView.Enabled = true;
      namespacesTreeView.Enabled = true;
      initializing = false;
    }

    private void InitializeNamespacesList() {
      initializing = true;
      namespacesTreeView.Enabled = false;
      namespacesTreeView.BeginUpdate();
      TreeNode oldTree = new TreeNode("root");
      CloneTreeNodeCollection(oldTree, namespacesTreeView.Nodes);
      namespacesTreeView.Nodes.Clear();
      var selectedNamespaces = new HashSet<string>(ProgrammableOperator.Namespaces);
      foreach (var ns in ProgrammableOperator.GetAllNamespaces(true))
        AddNamespace(namespacesTreeView.Nodes, ns, selectedNamespaces.Contains(ns), oldTree);
      codeEditor.Prefix = GetGeneratedPrefix();
      namespacesTreeView.EndUpdate();
      namespacesTreeView.Enabled = true;
      initializing = false;
    }

    private void CloneTreeNodeCollection(TreeNode root, TreeNodeCollection nodes) {
      foreach (TreeNode n in nodes) {
        TreeNode newNode = root.Nodes.Add(n.Text, n.Text);
        newNode.Checked = n.Checked;
        CloneTreeNodeCollection(newNode, n.Nodes);
        if (n.IsExpanded)
          newNode.Expand();
      }
    }

    private bool AddNamespace(TreeNodeCollection parentNodes, string ns, bool isSelected, TreeNode oldTree) {
      int dotIndex = ns.IndexOf('.');
      string prefix = ns;
      if (dotIndex != -1)
        prefix = ns.Substring(0, dotIndex);
      TreeNode node = GetOrCreateNode(parentNodes, prefix);
      TreeNode oldNode = MaybeGetNode(oldTree, prefix);
      bool isNew = oldNode == null;
      if (dotIndex != -1 && dotIndex + 1 < ns.Length) {
        isNew = AddNamespace(node.Nodes, ns.Substring(dotIndex + 1, ns.Length - (dotIndex + 1)), isSelected, oldNode);
      } else {
        node.Checked = isSelected;
      }
      if (isNew || oldNode != null && oldNode.IsExpanded)
        node.Expand();
      if (isNew)
        namespacesTreeView.SelectedNode = node;
      return isNew;
    }

    private static TreeNode MaybeGetNode(TreeNode parentNode, string key) {
      if (parentNode == null)
        return null;
      if (parentNode.Nodes.ContainsKey(key))
        return parentNode.Nodes[key];
      return null;
    }

    private static TreeNode GetOrCreateNode(TreeNodeCollection parentNodes, string key) {
      TreeNode node = null;
      if (parentNodes.ContainsKey(key)) {
        node = parentNodes[key];
      } else {
        node = parentNodes.Add(key, key);
      }
      return node;
    }

    private void namespacesTreeView_AfterCheck(object sender, TreeViewEventArgs e) {
      if (initializing)
        return;
      if (e.Node.Checked) {
        ProgrammableOperator.SelectNamespace(e.Node.FullPath);
      } else {
        ProgrammableOperator.UnselectNamespace(e.Node.FullPath);
      }
      codeEditor.Prefix = GetGeneratedPrefix();
    }

    private void showCodeButton_Click(object sender, EventArgs e) {
      new CodeViewer(ProgrammableOperator.CompilationUnitCode).ShowDialog(this);
    }
  }
}