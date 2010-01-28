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

namespace HeuristicLab.GP {
  public partial class FunctionLibraryEditor : EditorBase {
    private ChooseItemDialog chooseFunctionDialog;
    public FunctionLibrary FunctionLibrary {
      get { return (FunctionLibrary)Item; }
      set { base.Item = value; }
    }

    public FunctionLibraryEditor(FunctionLibrary library)
      : base() {
      InitializeComponent();
      FunctionLibrary = library;
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      base.Item.Changed += (sender, args) => UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      functionsListView.Clear();
      functionsComboBox.Items.Clear();
      foreach (IFunction fun in FunctionLibrary.Functions) {
        functionsListView.Items.Add(CreateListViewItem(fun));
        functionsComboBox.Items.Add(fun);
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
      foreach (int selectedIndex in removeIndices) {
        FunctionLibrary.RemoveFunction((IFunction)functionsListView.Items[selectedIndex].Tag);        
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

    private void functionsComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (functionsComboBox.SelectedItem != null) {
        IFunction selectedFun = (IFunction)functionsComboBox.SelectedItem;
        Control funView = (Control)selectedFun.CreateView();
        funView.Dock = DockStyle.Fill;
        functionDetailsPanel.Controls.Clear();
        functionDetailsPanel.Controls.Add(funView);
      }
    }
  }
}
