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
    private Function function;
    private const string ALL_SLOTS = "All";
    private string selectedSlot = ALL_SLOTS;

    public FunctionView() {
      InitializeComponent();
    }

    public FunctionView(Function function)
      : this() {
      this.function = function;
      function.Changed += (sender, args) => UpdateControls();
      Refresh();
    }

    protected override void UpdateControls() {
      nameTextBox.Text = function.Name;
      minSubTreesTextBox.Text = function.MinSubTrees.ToString();
      maxSubTreesTextBox.Text = function.MaxSubTrees.ToString();
      ticketsTextBox.Text = function.Tickets.ToString();
      minTreeHeightTextBox.Text = function.MinTreeHeight.ToString();
      minTreeSizeTextBox.Text = function.MinTreeSize.ToString();
      if (function.Initializer != null) {
        initializerTextBox.Text = function.Initializer.Name;
      } else {
        initializerTextBox.Enabled = false;
        editInitializerButton.Enabled = false;
      }
      if (function.Manipulator != null) {
        manipulatorTextBox.Text = function.Manipulator.Name;
      } else {
        manipulatorTextBox.Enabled = false;
        editManipulatorButton.Enabled = false;
      }

      argumentComboBox.Items.Clear();
      argumentComboBox.Items.Add(ALL_SLOTS);
      for (int i = 0; i < function.MaxSubTrees; i++) {
        argumentComboBox.Items.Add(i.ToString());
      }

      UpdateAllowedSubFunctionsList();
    }

    private void UpdateAllowedSubFunctionsList() {
      if (function.MaxSubTrees > 0) {
        subFunctionsListBox.Items.Clear();
        if (selectedSlot == ALL_SLOTS) {
          IEnumerable<IFunction> functionSet = function.GetAllowedSubFunctions(0);
          for (int i = 1; i < function.MaxSubTrees; i++) {
            functionSet = functionSet.Intersect(function.GetAllowedSubFunctions(i));
          }
          foreach (var subFun in functionSet) {
            subFunctionsListBox.Items.Add(subFun);
          }
        } else {
          int slot = int.Parse(selectedSlot);
          foreach (var subFun in function.GetAllowedSubFunctions(slot)) {
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
        function.Name = name;
        functionPropertiesErrorProvider.SetError(nameTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(nameTextBox, "Name can't be empty.");
      }
    }

    private void minSubTreesTextBox_TextChanged(object sender, EventArgs e) {
      int minSubTrees;
      if (int.TryParse(minSubTreesTextBox.Text, out minSubTrees) && minSubTrees >= 0) {
        function.MinSubTrees = minSubTrees;
        functionPropertiesErrorProvider.SetError(minSubTreesTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(minSubTreesTextBox, "Min sub-trees must be 0 or a positive integer.");
      }
    }

    private void maxSubTreesTextBox_TextChanged(object sender, EventArgs e) {
      int maxSubTrees;
      if (int.TryParse(maxSubTreesTextBox.Text, out maxSubTrees) && maxSubTrees >= 0) {
        function.MaxSubTrees = maxSubTrees;
        functionPropertiesErrorProvider.SetError(maxSubTreesTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(maxSubTreesTextBox, "Max sub-trees must be 0 or a positive integer and larger or equal min sub-trees.");
      }
    }

    private void ticketsTextBox_TextChanged(object sender, EventArgs e) {
      double tickets;
      if (double.TryParse(ticketsTextBox.Text, out tickets) && tickets >= 0) {
        function.Tickets = tickets;
        functionPropertiesErrorProvider.SetError(ticketsTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(ticketsTextBox, "Number of tickets must be 0 or a positive real value.");
      }
    }

    private void editInitializerButton_Click(object sender, EventArgs e) {
      ControlManager.Manager.ShowControl(function.Initializer.CreateView());
    }

    private void editManipulatorButton_Click(object sender, EventArgs e) {
      ControlManager.Manager.ShowControl(function.Manipulator.CreateView());
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
              for (int slot = 0; slot < function.MaxSubTrees; slot++)
                function.AddAllowedSubFunction(fun, slot);
            } else {
              int slot = int.Parse(selectedSlot);
              function.AddAllowedSubFunction(fun, slot);
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
            for (int slot = 0; slot < function.MaxSubTrees; slot++) {
              foreach (var subFun in subFunctionsListBox.SelectedItems) {
                function.RemoveAllowedSubFunction((IFunction)subFun, slot);
              }
            }
          } else {
            int slot = int.Parse(selectedSlot);
            foreach (var subFun in subFunctionsListBox.SelectedItems) {
              function.RemoveAllowedSubFunction((IFunction)subFun, slot);
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
