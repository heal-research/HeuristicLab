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
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.GP.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.GP {
  public partial class FunctionView : ViewBase {

    private const string ALL_SLOTS = "All";
    private string selectedSlot = ALL_SLOTS;

    public Function Function {
      get {
        return (Function)Item;
      }
      set {
        Item = value;
      }
    }

    public FunctionView()
      : base() {
      InitializeComponent();
    }

    public FunctionView(Function function)
      : base() {
      InitializeComponent();
      Function = function;
      UpdateControls();
    }

    protected override void UpdateControls() {
      nameTextBox.Text = Function.Name;
      minSubTreesTextBox.Text = Function.MinSubTrees.ToString();
      maxSubTreesTextBox.Text = Function.MaxSubTrees.ToString();
      ticketsTextBox.Text = Function.Tickets.ToString();
      minTreeHeightTextBox.Text = Function.MinTreeHeight.ToString();
      minTreeSizeTextBox.Text = Function.MinTreeSize.ToString();
      if (Function.Initializer != null) {
        initializerTextBox.Text = Function.Initializer.Name;
      } else {
        initializerTextBox.Enabled = false;
        editInitializerButton.Enabled = false;
      }
      if (Function.Manipulator != null) {
        manipulatorTextBox.Text = Function.Manipulator.Name;
      } else {
        manipulatorTextBox.Enabled = false;
        editManipulatorButton.Enabled = false;
      }

      argumentComboBox.Items.Clear();
      argumentComboBox.Items.Add(ALL_SLOTS);
      for (int i = 0; i < Function.MaxSubTrees; i++) {
        argumentComboBox.Items.Add(i.ToString());
      }

      UpdateAllowedSubFunctionsList();
    }

    private void UpdateAllowedSubFunctionsList() {
      if (Function.MaxSubTrees > 0) {
        subFunctionsListBox.Items.Clear();
        if (selectedSlot == ALL_SLOTS) {
          IEnumerable<IFunction> functionSet = Function.GetAllowedSubFunctions(0);
          for (int i = 1; i < Function.MaxSubTrees; i++) {
            functionSet = functionSet.Intersect(Function.GetAllowedSubFunctions(i));
          }
          foreach (var subFun in functionSet) {
            subFunctionsListBox.Items.Add(subFun);
          }
        } else {
          int slot = int.Parse(selectedSlot);
          foreach (var subFun in Function.GetAllowedSubFunctions(slot)) {
            subFunctionsListBox.Items.Add(subFun);
          }
        }
      } else {
        // no subfunctions allowed
        subTreesGroupBox.Enabled = false;
      }
    }

    private void nameTextBox_TextChanged(object sender, EventArgs e) {
      string name = nameTextBox.Text;
      if (!string.IsNullOrEmpty(name)) {
        Function.Name = name;
        functionPropertiesErrorProvider.SetError(nameTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(nameTextBox, "Name can't be empty.");
      }
    }

    private void minSubTreesTextBox_TextChanged(object sender, EventArgs e) {
      int minSubTrees;
      if (int.TryParse(minSubTreesTextBox.Text, out minSubTrees) && minSubTrees >= 0) {
        Function.MinSubTrees = minSubTrees;
        functionPropertiesErrorProvider.SetError(minSubTreesTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(minSubTreesTextBox, "Min sub-trees must be 0 or a positive integer.");
      }
    }

    private void maxSubTreesTextBox_TextChanged(object sender, EventArgs e) {
      int maxSubTrees;
      if (int.TryParse(maxSubTreesTextBox.Text, out maxSubTrees) && maxSubTrees >= 0) {
        Function.MaxSubTrees = maxSubTrees;
        functionPropertiesErrorProvider.SetError(maxSubTreesTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(maxSubTreesTextBox, "Max sub-trees must be 0 or a positive integer and larger or equal min sub-trees.");
      }
    }

    private void ticketsTextBox_TextChanged(object sender, EventArgs e) {
      double tickets;
      if (double.TryParse(ticketsTextBox.Text, out tickets) && tickets >= 0) {
        Function.Tickets = tickets;
        functionPropertiesErrorProvider.SetError(ticketsTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(ticketsTextBox, "Number of tickets must be 0 or a positive real value.");
      }
    }

    private void editInitializerButton_Click(object sender, EventArgs e) {
      ControlManager.Manager.ShowControl(Function.Initializer.CreateView());
    }

    private void editManipulatorButton_Click(object sender, EventArgs e) {
      ControlManager.Manager.ShowControl(Function.Manipulator.CreateView());
    }

    private void argumentComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      selectedSlot = argumentComboBox.Text;
      UpdateAllowedSubFunctionsList();
    }

    private void subFunctionsListBox_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IFunction"))
        e.Effect = DragDropEffects.Link;
    }
    private void subFunctionsListBox_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (e.Data.GetDataPresent("IFunction"))
        e.Effect = DragDropEffects.Link;
    }

    private void subFunctionsListBox_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        if (e.Data.GetDataPresent("IFunction")) {
          IFunction fun = (IFunction)e.Data.GetData("IFunction");
          try {
            Cursor = Cursors.WaitCursor;
            if (selectedSlot == ALL_SLOTS) {
              for (int slot = 0; slot < Function.MaxSubTrees; slot++)
                Function.AddAllowedSubFunction(fun, slot);
            } else {
              int slot = int.Parse(selectedSlot);
              Function.AddAllowedSubFunction(fun, slot);
            }
          }
          finally {
            Cursor = Cursors.Default;
          }
        }
      }
    }

    private void subFunctionsListBox_KeyUp(object sender, KeyEventArgs e) {
      try {
        Cursor = Cursors.WaitCursor;
        if (subFunctionsListBox.SelectedItems.Count > 0 && e.KeyCode == Keys.Delete) {
          if (selectedSlot == ALL_SLOTS) {
            List<IFunction> removedSubFunctions = new List<IFunction>(subFunctionsListBox.SelectedItems.Cast<IFunction>());
            for (int slot = 0; slot < Function.MaxSubTrees; slot++) {
              foreach (var subFun in removedSubFunctions) {
                Function.RemoveAllowedSubFunction((IFunction)subFun, slot);
              }
            }
          } else {
            int slot = int.Parse(selectedSlot);
            List<IFunction> removedSubFunctions = new List<IFunction>(subFunctionsListBox.SelectedItems.Cast<IFunction>());
            foreach (var subFun in removedSubFunctions) {
              Function.RemoveAllowedSubFunction(subFun, slot);
            }
          }

        }
      }
      finally {
        Cursor = Cursors.Default;
      }
    }
  }
}
