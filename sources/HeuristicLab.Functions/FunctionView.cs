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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Data;

namespace HeuristicLab.Functions {
  public partial class FunctionTreeView : ViewBase {
    private IFunctionTree functionTree;

    private IFunctionTree selectedBranch;
    private IVariable selectedVariable;

    private FunctionNameVisitor functionNameVisitor;
    public FunctionTreeView() {
      InitializeComponent();
      functionNameVisitor = new FunctionNameVisitor();
    }

    public FunctionTreeView(IFunctionTree functionTree)
      : this() {
      this.functionTree = functionTree;
      Refresh();
    }

    protected override void UpdateControls() {
      functionTreeView.Nodes.Clear();
      functionNameVisitor.Visit(functionTree);
      TreeNode rootNode = new TreeNode();
      rootNode.Name = functionTree.Function.Name;
      rootNode.Text = functionNameVisitor.Name;
      rootNode.Tag = functionTree;
      rootNode.ContextMenuStrip = treeNodeContextMenu;
      functionTreeView.Nodes.Add(rootNode);

      foreach(IFunctionTree subTree in functionTree.SubTrees) {
        CreateTree(rootNode, subTree);
      }
      functionTreeView.ExpandAll();
    }

    private void CreateTree(TreeNode rootNode, IFunctionTree functionTree) {
      TreeNode node = new TreeNode();
      functionNameVisitor.Visit(functionTree);
      node.Name = functionTree.Function.Name;
      node.Text = functionNameVisitor.Name;
      node.Tag = functionTree;
      node.ContextMenuStrip = treeNodeContextMenu;
      rootNode.Nodes.Add(node);
      foreach(IFunctionTree subTree in functionTree.SubTrees) {
        CreateTree(node, subTree);
      }
    }

    private void functionTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      variablesListBox.Items.Clear();
      variablesSplitContainer.Panel2.Controls.Clear();
      templateTextBox.Clear();
      editButton.Enabled = false;
      if(functionTreeView.SelectedNode != null && functionTreeView.SelectedNode.Tag != null) {
        IFunctionTree selectedBranch = (IFunctionTree)functionTreeView.SelectedNode.Tag;
        UpdateVariablesList(selectedBranch);
        templateTextBox.Text = selectedBranch.Function.Name;
        this.selectedBranch = selectedBranch;
        editButton.Enabled = true;
      }
    }

    private void UpdateVariablesList(IFunctionTree functionTree) {
      foreach(IVariable variable in functionTree.LocalVariables) {
        variablesListBox.Items.Add(variable.Name);
      }
    }

    private void variablesListBox_SelectedIndexChanged(object sender, EventArgs e) {
      // in case we had an event-handler registered for a different variable => unregister the event-handler
      if(selectedVariable != null) {
        selectedVariable.Value.Changed -= new EventHandler(selectedVariable_ValueChanged);
      }
      if(variablesListBox.SelectedItem != null) {
        string selectedVariableName = (string)variablesListBox.SelectedItem;
        selectedVariable = selectedBranch.GetLocalVariable(selectedVariableName);
        variablesSplitContainer.Panel2.Controls.Clear();
        Control editor = (Control)selectedVariable.CreateView();
        variablesSplitContainer.Panel2.Controls.Add(editor);
        editor.Dock = DockStyle.Fill;
        // register an event handler that updates the treenode when the value of the variable is changed by the user
        selectedVariable.Value.Changed += new EventHandler(selectedVariable_ValueChanged);
      } else {
        variablesSplitContainer.Panel2.Controls.Clear();
      }
    }

    void selectedVariable_ValueChanged(object sender, EventArgs e) {
      if(functionTreeView.SelectedNode != null && functionTreeView.SelectedNode.Tag != null) {
        TreeNode node = functionTreeView.SelectedNode;
        functionNameVisitor.Visit(selectedBranch);
        node.Text = functionNameVisitor.Name;
      }
    }

    protected virtual void editButton_Click(object sender, EventArgs e) {
      PluginManager.ControlManager.ShowControl(selectedBranch.Function.CreateView());
    }

    private void copyToClipboardMenuItem_Click(object sender, EventArgs e) {
      TreeNode node = functionTreeView.SelectedNode;
      if(node == null || node.Tag == null) return;

      ModelAnalyzerExporter visitor = new ModelAnalyzerExporter();
      visitor.Visit((IFunctionTree)node.Tag);
      Clipboard.SetText(visitor.ModelAnalyzerPrefix);
    }

    private class FunctionNameVisitor : IFunctionVisitor {
      string name;
      IFunctionTree currentBranch;

      public string Name {
        get { return name; }
      }

      public void Visit(IFunctionTree tree) {
        currentBranch = tree;
        tree.Function.Accept(this);
      }

      #region IFunctionVisitor Members
      public void Visit(IFunction function) {
        name = function.Name;
      }

      public void Visit(Addition addition) {
        name = "+";
      }

      public void Visit(Constant constant) {
        name = ((ConstrainedDoubleData)(currentBranch.GetLocalVariable(HeuristicLab.Functions.Constant.VALUE).Value)).Data + "";
      }

      public void Visit(Cosinus cosinus) {
        name = "Sin";
      }

      public void Visit(Differential diff) {
        string timeOffset = "";
        int sampleOffset = ((ConstrainedIntData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Differential.OFFSET).Value).Data;
        int variableIndex = ((ConstrainedIntData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Differential.INDEX).Value).Data;
        double weight = ((ConstrainedDoubleData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Differential.WEIGHT).Value).Data;
        if(sampleOffset < 0) {
          timeOffset = "(t" + sampleOffset + ")";
        } else if(sampleOffset > 0) {
          timeOffset = "(t+" + sampleOffset + ")";
        } else {
          timeOffset = "";
        }
        name = "Diff" + variableIndex + timeOffset + " * " + weight;
      }

      public void Visit(Division division) {
        name = "/";
      }

      public void Visit(Exponential exponential) {
        name = "Exp";
      }

      public void Visit(Logarithm logarithm) {
        name = "Log";
      }

      public void Visit(Multiplication multiplication) {
        name = "*";
      }

      public void Visit(Power power) {
        name = "Pow";
      }

      public void Visit(Signum signum) {
        name = "Sign";
      }

      public void Visit(Sinus sinus) {
        name = "Sin";
      }

      public void Visit(Sqrt sqrt) {
        name = "Sqrt";
      }

      public void Visit(Subtraction substraction) {
        name = "-";
      }

      public void Visit(Tangens tangens) {
        name = "Tan";
      }

      public void Visit(Variable variable) {
        string timeOffset = "";
        int sampleOffset = ((ConstrainedIntData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Variable.OFFSET).Value).Data;
        int variableIndex = ((ConstrainedIntData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Variable.INDEX).Value).Data;
        double weight = ((ConstrainedDoubleData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Variable.WEIGHT).Value).Data;
        if(sampleOffset < 0) {
          timeOffset = "(t" + sampleOffset + ")";
        } else if(sampleOffset > 0) {
          timeOffset = "(t+" + sampleOffset + ")";
        } else {
          timeOffset = "";
        }
        name = "Var" + variableIndex + timeOffset + " * " + weight;
      }

      public void Visit(And and) {
        name = "And";
      }

      public void Visit(Average average) {
        name = "Avg";
      }

      public void Visit(IfThenElse ifThenElse) {
        name = "IFTE";
      }

      public void Visit(Not not) {
        name = "Not";
      }

      public void Visit(Or or) {
        name = "Or";
      }

      public void Visit(Xor xor) {
        name = "Xor";
      }

      public void Visit(Equal equal) {
        name = "eq?";
      }

      public void Visit(LessThan lessThan) {
        name = "<";
      }

      public void Visit(GreaterThan greaterThan) {
        name = ">";
      }
      #endregion
    }
  }
}
