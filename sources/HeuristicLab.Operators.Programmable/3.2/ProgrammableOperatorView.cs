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
  public static IOperation Execute(IOperator op, IScope scope, parameters ...) {";
        codeEditor.Suffix = @"
    return null;
  }
}";   
        assembliesListBox.DataSource = null;
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
      prefix.Append("  ").Append(ProgrammableOperator.Signature).Append(" {");
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

    public static Assembly GetAssembly(CheckedListBox box, int index) {
      return (Assembly)(((CheckedListBoxItem)box.Items[index]).Tag);
    }

    private void assembliesListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
      if (initializing)
        return;
      Assembly a = GetAssembly(assembliesListBox, e.Index);
      if (e.NewValue == CheckState.Checked) {                
        ProgrammableOperator.SelectAssembly(a);
        codeEditor.AddAssembly(a);
      } else if (e.NewValue == CheckState.Unchecked) {
        ProgrammableOperator.UnselectAssembly(a);
        codeEditor.RemoveAssembly(a);
      } else {
        return;
      }
      InitializeNamespacesList();      
      codeEditor.Prefix = GetGeneratedPrefix();
    }

    private bool initializing = false;
    private void InitializeAssemblyList() {
      assembliesListBox.Items.Clear();
      var selectedAssemblies = new HashSet<Assembly>(ProgrammableOperator.SelectedAssemblies);
      initializing = true;
      foreach (var a in ProgrammableOperator.AvailableAssemblies.ToList()) {
        assembliesListBox.Items.Add(
          new CheckedListBoxItem(a.GetName().Name, a),
          selectedAssemblies.Contains(a));
      }
      initializing = false;
    }

    private void InitializeNamespacesList() {
      initializing = true;
      namespacesListBox.Items.Clear();
      var selectedNamespaces = new HashSet<string>(ProgrammableOperator.Namespaces);
      foreach (var ns in ProgrammableOperator.GetAllNamespaces(true)) {
        namespacesListBox.Items.Add(ns, selectedNamespaces.Contains(ns));
      }
      codeEditor.Prefix = GetGeneratedPrefix();
      initializing = false;
    }

    private void namespacesListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
      if (initializing)
        return;
      if (e.NewValue == CheckState.Checked) {
        ProgrammableOperator.SelectNamespace((string)namespacesListBox.Items[e.Index]);
      } else if (e.NewValue == CheckState.Unchecked) {
        ProgrammableOperator.UnselectNamespace((string)namespacesListBox.Items[e.Index]);
      }
      codeEditor.Prefix = GetGeneratedPrefix();
    }

    private void showCodeButton_Click(object sender, EventArgs e) {
      new CodeViewer(ProgrammableOperator.CompilationUnitCode).ShowDialog(this);
    }

  }

  public class CheckedListBoxItem : IComparable {

    public object Tag { get; private set; }
    public string Text { get; private set; }

    public CheckedListBoxItem(string text, object tag) {
      Text = text;
      Tag = tag;
    }

    public override string ToString() {
      return Text;
    }

    public int CompareTo(object obj) {
      if (obj == null)
        throw new ArgumentException("cannot compare to null");
      if (!(obj is CheckedListBoxItem))
        throw new ArgumentException(string.Format(
          "cannot compare CheckedListBoxItem to {0}",
          obj.GetType().Name));
      return Text.CompareTo(((CheckedListBoxItem)obj).Text);
    }
  }
}
