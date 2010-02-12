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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.GP.Interfaces;
using System.Text;
using HeuristicLab.Random;

namespace HeuristicLab.GP {
  public partial class FunctionLibraryEditor : EditorBase {
    private ChooseItemDialog chooseFunctionDialog;
    public FunctionLibrary FunctionLibrary {
      get { return (FunctionLibrary)Item; }
      set { base.Item = value; }
    }

    public FunctionLibraryEditor()
      : base() {
      InitializeComponent();
    }

    public FunctionLibraryEditor(FunctionLibrary library)
      : this() {
      FunctionLibrary = library;
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      base.Item.Changed += (sender, args) => UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      mutationListView.Items.Clear();
      initListView.Items.Clear();
      functionsListView.Clear();
      foreach (IFunction fun in FunctionLibrary.Functions) {
        functionsListView.Items.Add(CreateListViewItem(fun));
        if (fun.Manipulator != null) {
          mutationListView.Items.Add(CreateListViewItem(fun));
        }
        if (fun.Initializer != null) {
          initListView.Items.Add(CreateListViewItem(fun));
        }
      }
    }

    private void mutationListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (mutationListView.SelectedItems.Count > 0 && mutationListView.SelectedItems[0].Tag != null) {
        IOperator manipulator = ((IFunction)mutationListView.SelectedItems[0].Tag).Manipulator;
        mutationVariableView.Enabled = true;
        mutationVariableView.Variable = new Variable("Manipulator", manipulator);
      } else {
        mutationVariableView.Enabled = false;
      }
    }

    private void initListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (initListView.SelectedItems.Count > 0 && initListView.SelectedItems[0].Tag != null) {
        IOperator initializer = ((IFunction)initListView.SelectedItems[0].Tag).Initializer;
        initVariableView.Enabled = true;
        initVariableView.Variable = new Variable("Initializer", initializer);
      } else {
        initVariableView.Enabled = false;
      }
    }

    private void addButton_Click(object sender, EventArgs e) {
      if (chooseFunctionDialog == null) chooseFunctionDialog = new ChooseItemDialog(typeof(IFunction));
      if (chooseFunctionDialog.ShowDialog(this) == DialogResult.OK) {
        FunctionLibrary.AddFunction((IFunction)chooseFunctionDialog.Item);
      }
    }

    private void removeButton_Click(object sender, EventArgs e) {
      // delete from the end of the list
      List<int> removeIndices = functionsListView.SelectedIndices.OfType<int>().OrderBy(x => 1.0 / x).ToList();
      try {
        Cursor = Cursors.WaitCursor;
        foreach (int selectedIndex in removeIndices) {
          FunctionLibrary.RemoveFunction((IFunction)functionsListView.Items[selectedIndex].Tag);
        }
      }
      finally {
        Cursor = Cursors.Default;
      }
    }

    private void functionsListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (functionsListView.SelectedIndices.Count > 0) {
        removeButton.Enabled = true;
      } else {
        removeButton.Enabled = false;
      }
    }

    private ListViewItem CreateListViewItem(IFunction function) {
      ListViewItem item = new ListViewItem();
      item.Name = function.Name;
      item.Text = function.Name;
      item.Tag = function;
      return item;
    }

    private void functionsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      ListViewItem item = (ListViewItem)e.Item;
      IFunction fun = (IFunction)item.Tag;
      DataObject data = new DataObject();
      data.SetData("IFunction", fun);
      data.SetData("DragSource", functionsListView);
      DoDragDrop(data, DragDropEffects.Link);
    }

    private void functionsListView_KeyUp(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete && functionsListView.SelectedItems.Count > 0) {
        List<IFunction> removedFunctions = new List<IFunction>(from x in functionsListView.SelectedItems.OfType<ListViewItem>()
                                                               select (IFunction)x.Tag);
        try {
          Cursor = Cursors.WaitCursor;
          foreach (var fun in removedFunctions) {
            FunctionLibrary.RemoveFunction(fun);
          }
        }
        finally {
          Cursor = Cursors.Default;
        }
      }
    }
    #region fun lib test
    private string TestFunctionLibrary() {
      int n = 1000;
      try {
        IFunctionTree[] randomTrees = CreateRandomTrees(n, 1, 100);
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Function symbol frequencies:");
        builder.AppendLine(CalculateFunctionFrequencies(randomTrees));
        builder.AppendLine("-----------------------------------------");
        builder.AppendLine("Terminal symbol frequencies:");
        builder.AppendLine(CalculateTerminalFrequencies(randomTrees));
        builder.AppendLine("-----------------------------------------");
        builder.AppendLine("Function arity frequencies:");
        builder.AppendLine(CalculateFunctionArityFrequencies(randomTrees));
        builder.AppendLine("-----------------------------------------");
        builder.AppendLine("Tree size frequencies:");
        builder.AppendLine(CalculateTreeSizeFrequencies(randomTrees));
        builder.AppendLine("-----------------------------------------");
        builder.AppendLine("Tree height frequencies:");
        builder.AppendLine(CalculateTreeHeightFrequencies(randomTrees));
        return builder.ToString();
      }
      catch (ArgumentException ex) {
        return "Could not create random trees:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
      }
    }

    private string CalculateFunctionFrequencies(IFunctionTree[] randomTrees) {
      Dictionary<IFunction, int> occurances = new Dictionary<IFunction, int>();
      double n = 0.0;
      for (int i = 0; i < randomTrees.Length; i++) {
        foreach (var node in FunctionTreeIterator.IteratePrefix(randomTrees[i])) {
          if (node.SubTrees.Count > 0) {
            if (!occurances.ContainsKey(node.Function))
              occurances[node.Function] = 0;
            occurances[node.Function]++;
            n++;
          }
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var function in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(function.Name); strBuilder.Append(": ");
        strBuilder.AppendFormat("{0:#0.00%}", occurances[function] / n);
      }
      return strBuilder.ToString();
    }

    public string CalculateTreeSizeFrequencies(IFunctionTree[] randomTrees) {
      int[] histogram = new int[105 / 5];
      for (int i = 0; i < randomTrees.Length; i++) {
        histogram[randomTrees[i].GetSize() / 5]++;
      }
      StringBuilder strBuilder = new StringBuilder();
      for (int i = 0; i < histogram.Length; i++) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append("< "); strBuilder.Append((i + 1) * 5);
        strBuilder.Append(": "); strBuilder.AppendFormat("{0:#0.00%}", histogram[i] / (double)randomTrees.Length);
      }
      return strBuilder.ToString();
    }

    public string CalculateTreeHeightFrequencies(IFunctionTree[] randomTrees) {
      int[] histogram = new int[100];
      for (int i = 0; i < randomTrees.Length; i++) {
        histogram[randomTrees[i].GetHeight()]++;
      }
      StringBuilder strBuilder = new StringBuilder();
      for (int i = 0; i < histogram.Length; i++) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append("< "); strBuilder.Append((i + 1));
        strBuilder.Append(": "); strBuilder.AppendFormat("{0:#0.00%}", histogram[i] / (double)randomTrees.Length);
      }
      return strBuilder.ToString();
    }

    public string CalculateFunctionArityFrequencies(IFunctionTree[] randomTrees) {
      Dictionary<int, int> occurances = new Dictionary<int, int>();
      double n = 0.0;
      for (int i = 0; i < randomTrees.Length; i++) {
        foreach (var node in FunctionTreeIterator.IteratePrefix(randomTrees[i])) {
          if (!occurances.ContainsKey(node.SubTrees.Count))
            occurances[node.SubTrees.Count] = 0;
          occurances[node.SubTrees.Count]++;
          n++;
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var arity in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(arity); strBuilder.Append(": ");
        strBuilder.AppendFormat("{0:#0.00%}", occurances[arity] / n);
      }
      return strBuilder.ToString();
    }

    public string CalculateTerminalFrequencies(IFunctionTree[] randomTrees) {
      Dictionary<IFunction, int> occurances = new Dictionary<IFunction, int>();
      double n = 0.0;
      for (int i = 0; i < randomTrees.Length; i++) {
        foreach (var node in FunctionTreeIterator.IteratePrefix(randomTrees[i])) {
          if (node.SubTrees.Count == 0) {
            if (!occurances.ContainsKey(node.Function))
              occurances[node.Function] = 0;
            occurances[node.Function]++;
            n++;
          }
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var function in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(function.Name); strBuilder.Append(": ");
        strBuilder.AppendFormat("{0:#0.00%}", occurances[function] / n);
      }
      return strBuilder.ToString();
    }

    private IFunctionTree[] CreateRandomTrees(int popSize, int minSize, int maxSize) {
      int maxHeight = 10;
      int maxTries = 100;
      IFunctionTree[] randomTrees = new IFunctionTree[popSize];
      MersenneTwister twister = new MersenneTwister();
      for (int i = 0; i < randomTrees.Length; i++) {
        int treeSize = twister.Next(minSize, maxSize);
        IFunctionTree root = null;
        int tries = 0;
        TreeGardener gardener = new TreeGardener(twister, FunctionLibrary);
        do {
          try {
            root = gardener.PTC2(treeSize, maxSize);
          }
          catch (ArgumentException) {
            // try a different size
            treeSize = twister.Next(minSize, maxSize);
            tries = 0;
          }
          if (tries++ >= maxTries) {
            // try a different size
            treeSize = twister.Next(minSize, maxSize);
            tries = 0;
          }
        } while (root == null || root.GetSize() > maxSize || root.GetHeight() > maxHeight);
        randomTrees[i] = root;
      }
      return randomTrees;
    }

    private void testButton_Click(object sender, EventArgs e) {
      try {
        Cursor = Cursors.WaitCursor;
        outputTextBox.Text = TestFunctionLibrary();
      }
      finally {
        Cursor = Cursors.Default;
      }
    }
    #endregion

    private void functionsListView_MouseUp(object sender, MouseEventArgs e) {
      if (functionsListView.SelectedItems.Count > 0) {
        IFunction selectedFun = (IFunction)functionsListView.SelectedItems[0].Tag;
        Control funView = (Control)selectedFun.CreateView();
        funView.Dock = DockStyle.Fill;
        functionDetailsPanel.Controls.Clear();
        functionDetailsPanel.Controls.Add(funView);
      }
    }
  }
}
