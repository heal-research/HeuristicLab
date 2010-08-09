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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.VS2010Wizards {
  public partial class ParametersControl : UserControl {
    public ParametersControl() {
      InitializeComponent();
    }

    public string GetParameterProperties(string accessModifier) {
      return string.Empty;
    }

    public string GetProperties(string accessModifier) {
      return string.Empty;
    }

    public string GetInitializers() {
      return string.Empty;
    }

    private void parametersListView_MouseDoubleClick(object sender, MouseEventArgs e) {
      var hit = parametersListView.HitTest(e.Location);
      TextBox dynamicUserInput = new TextBox();
      dynamicUserInput.Left = parametersListView.Left + hit.SubItem.Bounds.Left + 3;
      dynamicUserInput.Top = parametersListView.Top + hit.SubItem.Bounds.Top;
      dynamicUserInput.Width = hit.SubItem.Bounds.Width;
      dynamicUserInput.Height = hit.SubItem.Bounds.Height;
      dynamicUserInput.Text = hit.SubItem.Text;
      dynamicUserInput.Tag = hit.SubItem;
      Controls.Add(dynamicUserInput);
      dynamicUserInput.BringToFront();
      Refresh();
      dynamicUserInput.Focus();
      dynamicUserInput.SelectAll();
      dynamicUserInput.KeyUp += new KeyEventHandler(dynamicUserInput_KeyUp);
      dynamicUserInput.LostFocus += new EventHandler(dynamicUserInput_LostFocus);
    }

    private void dynamicUserInput_LostFocus(object sender, EventArgs e) {
      TextBox t = (TextBox)sender;
      System.Windows.Forms.ListViewItem.ListViewSubItem subItem = t.Tag as System.Windows.Forms.ListViewItem.ListViewSubItem;
      subItem.Text = t.Text;
      t.KeyUp -= new KeyEventHandler(dynamicUserInput_KeyUp);
      t.LostFocus -= new EventHandler(dynamicUserInput_LostFocus);
      Controls.Remove(t);
    }

    private void dynamicUserInput_KeyUp(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
        dynamicUserInput_LostFocus(sender, EventArgs.Empty);
    }

    private void parametersListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (parametersListView.SelectedIndices.Count > 0) {
        removeButton.Enabled = true;
        upButton.Enabled = true;
        downButton.Enabled = true;
      } else {
        removeButton.Enabled = false;
        upButton.Enabled = false;
        downButton.Enabled = false;
      }
    }

    private void removeButton_Click(object sender, EventArgs e) {
      if (parametersListView.SelectedIndices.Count > 0) {
        int row = parametersListView.SelectedIndices[0];
        parametersListView.Items.RemoveAt(row);
        if (parametersListView.Items.Count > 0) {
          int index = Math.Max(Math.Min(row, parametersListView.Items.Count - 1), 0);
          parametersListView.Items[index].Selected = true;
        }
      }
    }

    private void upButton_Click(object sender, EventArgs e) {
      if (parametersListView.SelectedIndices.Count > 0) {
        ListViewItem selected = parametersListView.SelectedItems[0];
        int row = parametersListView.SelectedIndices[0];
        parametersListView.Items.Remove(selected);
        parametersListView.Items.Insert(Math.Max(row - 1, 0), selected);
      }
    }

    private void downButton_Click(object sender, EventArgs e) {
      if (parametersListView.SelectedIndices.Count > 0) {
        ListViewItem selected = parametersListView.SelectedItems[0];
        int row = parametersListView.SelectedIndices[0];
        parametersListView.Items.Remove(selected);
        if (row == parametersListView.Items.Count)
          parametersListView.Items.Add(selected);
        else
          parametersListView.Items.Insert(row + 1, selected);
      }
    }

    private void addButton_Click(object sender, EventArgs e) {
      string name = "FormalName";
      bool uniqueNameFound;
      int i = 1;
      do {
        uniqueNameFound = true;
        foreach (ListViewItem li in parametersListView.Items) {
          if (li.Text.Equals(name)) {
            name = "FormalName" + i.ToString();
            i++;
            uniqueNameFound = false;
          }
        }
      } while (!uniqueNameFound);
      ListViewItem item = new ListViewItem();
      item.Text = name;
      item.SubItems.Add("Value");
      item.SubItems.Add("IItem");
      item.SubItems.Add("Add a description.");
      item.SubItems.Add("");
      parametersListView.Items.Add(item);
    }
  }
}
