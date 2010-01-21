#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Operators;
using System.CodeDom.Compiler;
using System.Reflection;
using HeuristicLab.CodeEditor;

namespace HeuristicLab.Operators.Programmable {

  public partial class ProgrammableOperatorView : ViewBase {

    public ProgrammableOperator ProgrammableOperator {
      get { return (ProgrammableOperator)Item; }
      set { base.Item = value; }
    }

    public ProgrammableOperatorView() {
      InitializeComponent();
    }

    public ProgrammableOperatorView(ProgrammableOperator programmableOperator)
      : this() {
      ProgrammableOperator = programmableOperator;
    }

    protected override void RemoveItemEvents() {
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseVariablesView.Operator = null;
      constrainedItemBaseView.ConstrainedItem = null;
      ProgrammableOperator.CodeChanged -= new EventHandler(ProgrammableOperator_CodeChanged);
      ProgrammableOperator.DescriptionChanged -= new EventHandler(ProgrammableOperator_DescriptionChanged);
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorBaseVariableInfosView.Operator = ProgrammableOperator;
      operatorBaseVariablesView.Operator = ProgrammableOperator;
      constrainedItemBaseView.ConstrainedItem = ProgrammableOperator;
      ProgrammableOperator.CodeChanged += new EventHandler(ProgrammableOperator_CodeChanged);
      ProgrammableOperator.DescriptionChanged += new EventHandler(ProgrammableOperator_DescriptionChanged);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ProgrammableOperator == null) {
        codeEditor.Text = "";
        codeEditor.Enabled = false;
        addVariableInfoButton.Enabled = false;
        removeVariableInfoButton.Enabled = false;
        descriptionTextBox.Text = "";
        descriptionTextBox.Enabled = false;
        codeEditor.Prefix = @"using System

public class Operator {
  public static IOperation Execute(IOperator op, IScope scope, parameters ...) {
";
        codeEditor.Suffix = @"
    return null;
  }
}";
        assembliesTreeView.Nodes.Clear();
      } else {
        codeEditor.Enabled = true;
        addVariableInfoButton.Enabled = true;
        removeVariableInfoButton.Enabled = operatorBaseVariableInfosView.SelectedVariableInfos.Count > 0;
        descriptionTextBox.Text = ProgrammableOperator.Description;
        descriptionTextBox.Enabled = true;
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
      }
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

    private void operatorBaseVariableInfosView_SelectedVariableInfosChanged(object sender, EventArgs e) {
      removeVariableInfoButton.Enabled = operatorBaseVariableInfosView.SelectedVariableInfos.Count > 0;
    }
    private void codeEditor_Validated(object sender, EventArgs e) {
      ProgrammableOperator.Code = codeEditor.UserCode;
    }
    private void descriptionTextBox_Validated(object sender, EventArgs e) {
      ProgrammableOperator.SetDescription(descriptionTextBox.Text);
    }

    private void addVariableInfoButton_Click(object sender, EventArgs e) {
      AddVariableInfoDialog dialog = new AddVariableInfoDialog();
      if (dialog.ShowDialog(this) == DialogResult.OK) {
        if (ProgrammableOperator.GetVariableInfo(dialog.VariableInfo.FormalName) != null) {
          Auxiliary.ShowErrorMessageBox("A variable info with the same formal name already exists.");
        } else {
          ProgrammableOperator.AddVariableInfo(dialog.VariableInfo);
          Recompile();
        }
      }
      dialog.Dispose();
    }

    private void removeVariableInfoButton_Click(object sender, EventArgs e) {
      IVariableInfo[] selected = new IVariableInfo[operatorBaseVariableInfosView.SelectedVariableInfos.Count];
      operatorBaseVariableInfosView.SelectedVariableInfos.CopyTo(selected, 0);
      for (int i = 0; i < selected.Length; i++)
        ProgrammableOperator.RemoveVariableInfo(selected[i].FormalName);
      Recompile();
    }

    private void Recompile() {
      try {
        ProgrammableOperator.Compile();
        MessageBox.Show("Compilation successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } catch (Exception ex) {
        Auxiliary.ShowErrorMessageBox(ex);
      }
      UpdateControls();
      codeEditor.ShowCompileErrors(ProgrammableOperator.CompileErrors, "ProgrammableOperator");
    }

    private void compileButton_Click(object sender, EventArgs e) {
      Recompile();
    }

    #region ProgrammableOperator Events
    private void ProgrammableOperator_CodeChanged(object sender, EventArgs e) {
      codeEditor.Text = ProgrammableOperator.Code;
    }
    private void ProgrammableOperator_DescriptionChanged(object sender, EventArgs e) {
      descriptionTextBox.Text = ProgrammableOperator.Description;
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
      initializing = false;
    }

    private void InitializeNamespacesList() {
      initializing = true;
      namespacesTreeView.Nodes.Clear();
      var selectedNamespaces = new HashSet<string>(ProgrammableOperator.Namespaces);
      foreach (var ns in ProgrammableOperator.GetAllNamespaces(true))
        AddNamespace(namespacesTreeView.Nodes, ns, selectedNamespaces.Contains(ns));
      codeEditor.Prefix = GetGeneratedPrefix();
      initializing = false;
    }

    private void AddNamespace(TreeNodeCollection parentNodes, string ns, bool isSelected) {
      int dotIndex = ns.IndexOf('.');
      string prefix = ns;
      if (dotIndex != -1)
        prefix = ns.Substring(0, dotIndex);
      TreeNode node = null;
      if (parentNodes.ContainsKey(prefix)) {
        node = parentNodes[prefix];
      } else {
        node = parentNodes.Add(prefix, prefix);
      }
      if (dotIndex != -1 && dotIndex + 1 < ns.Length) {
        AddNamespace(node.Nodes, ns.Substring(dotIndex + 1, ns.Length - (dotIndex + 1)), isSelected);
        if (isSelected)
          node.Expand();
      }  else
        node.Checked = isSelected;
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