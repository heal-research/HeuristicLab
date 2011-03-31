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
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.VS2010Wizards {
  public partial class ParametersControl : UserControl {
    public ParametersControl() {
      InitializeComponent();
    }

    public void AddParameter(string name, string type, string dataType, string description, string defaultValue) {
      ListViewItem item = new ListViewItem(name);
      item.SubItems.Add(type);
      item.SubItems.Add(dataType);
      item.SubItems.Add(description);
      item.SubItems.Add(defaultValue);
      parametersListView.Items.Add(item);
    }

    public string GetParameterProperties(string accessModifier) {
      StringBuilder builder = new StringBuilder();
      foreach (ListViewItem item in parametersListView.Items) {
        string name = item.Text.Trim();
        string type = item.SubItems[typeColumnHeader.DisplayIndex].Text.Trim();
        string dataType = item.SubItems[dataTypeColumnHeader.DisplayIndex].Text.Trim();
        builder.Append(accessModifier);
        builder.Append(" " + type + "Parameter");
        if (dataType != string.Empty)
          builder.Append("<" + dataType + ">");
        builder.Append(" " + name + "Parameter {" + Environment.NewLine);
        builder.Append("\tget { return ");
        builder.Append("(" + type + "Parameter");
        if (dataType != string.Empty)
          builder.Append("<" + dataType + ">");
        builder.Append(")Parameters[\"" + name + "\"]; }" + Environment.NewLine);
        builder.Append("}" + Environment.NewLine);
      }
      return builder.ToString();
    }

    public string GetProperties(string accessModifier) {
      StringBuilder builder = new StringBuilder();
      foreach (ListViewItem item in parametersListView.Items) {
        string name = item.Text.Trim();
        string type = item.SubItems[typeColumnHeader.DisplayIndex].Text.Trim();
        string dataType = item.SubItems[dataTypeColumnHeader.DisplayIndex].Text.Trim();
        if (type.ToLower().Equals("value")) {
          builder.Append(accessModifier + " " + dataType);
          builder.Append(" " + name + " {" + Environment.NewLine);
          builder.Append("\tget { return " + name + "Parameter.Value; }" + Environment.NewLine);
          builder.Append("\tset { " + name + "Parameter.Value = value; }" + Environment.NewLine);
          builder.Append("}" + Environment.NewLine);
        }
      }
      return builder.ToString();
    }

    public string GetInitializers() {
      StringBuilder builder = new StringBuilder();
      foreach (ListViewItem item in parametersListView.Items) {
        string name = item.Text.Trim();
        string type = item.SubItems[typeColumnHeader.DisplayIndex].Text.Trim();
        string dataType = item.SubItems[dataTypeColumnHeader.DisplayIndex].Text.Trim();
        string description = item.SubItems[descriptionColumnHeader.DisplayIndex].Text.Trim();
        string initialValue = item.SubItems[defaultValueColumnHeader.DisplayIndex].Text.Trim();
        builder.Append("\tParameters.Add(new " + type + "Parameter");
        if (dataType != string.Empty)
          builder.Append("<" + dataType + ">");
        builder.Append("(\"" + name + "\", ");
        builder.Append("\"" + description + "\"");
        if (initialValue != string.Empty)
          builder.Append(", " + initialValue);
        builder.Append("));" + Environment.NewLine);
      }
      return builder.ToString();
    }

    #region Button Event Handlers
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
      item.SubItems.Add("null");
      parametersListView.Items.Add(item);
    }
    #endregion

    private void parametersListView_MouseDoubleClick(object sender, MouseEventArgs e) {
      var hit = parametersListView.HitTest(e.Location);
      if (hit.Item == null || hit.SubItem == null) return;
      if (hit.Item.SubItems[typeColumnHeader.DisplayIndex] == hit.SubItem) {
        parameterTypeComboBox.Left = parametersListView.Left + hit.SubItem.Bounds.Left + 3;
        parameterTypeComboBox.Top = parametersListView.Top + hit.SubItem.Bounds.Top;
        parameterTypeComboBox.Width = hit.SubItem.Bounds.Width;
        parameterTypeComboBox.Height = hit.SubItem.Bounds.Height;
        parameterTypeComboBox.Text = hit.SubItem.Text;
        parameterTypeComboBox.Tag = hit.SubItem;
        parameterTypeComboBox.Visible = true;
        parameterTypeComboBox.BringToFront();
        Refresh();
        parameterTypeComboBox.Focus();
        parameterTypeComboBox.SelectAll();
      } else {
        customInputTextBox.Left = parametersListView.Left + hit.SubItem.Bounds.Left + 3;
        customInputTextBox.Top = parametersListView.Top + hit.SubItem.Bounds.Top;
        if (hit.SubItem != hit.Item.SubItems[0])
          customInputTextBox.Width = hit.SubItem.Bounds.Width;
        else customInputTextBox.Width = hit.Item.SubItems[1].Bounds.Left;
        customInputTextBox.Height = hit.SubItem.Bounds.Height;
        customInputTextBox.Text = hit.SubItem.Text;
        customInputTextBox.Tag = hit.SubItem;
        customInputTextBox.Visible = true;
        customInputTextBox.BringToFront();
        Refresh();
        customInputTextBox.Focus();
        customInputTextBox.SelectAll();
      }
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

    protected override bool ProcessDialogKey(Keys keyData) {
      if (keyData == Keys.Tab) {
        if (customInputTextBox.Focused) {
          customInputTextBox_Leave(customInputTextBox, EventArgs.Empty);
          if (customInputTextBox.Right + 5 < parametersListView.Right) {
            parametersListView_MouseDoubleClick(parametersListView, new MouseEventArgs(MouseButtons.Left, 2, customInputTextBox.Right + 5, customInputTextBox.Top + 3, 0));
            return true;
          } else return base.ProcessDialogKey(keyData);
        } else if (parameterTypeComboBox.Focused) {
          parameterTypeComboBox_Leave(parameterTypeComboBox, EventArgs.Empty);
          parametersListView_MouseDoubleClick(parametersListView, new MouseEventArgs(MouseButtons.Left, 2, parameterTypeComboBox.Right + 5, parameterTypeComboBox.Top + 3, 0));
          return true;
        } else return base.ProcessDialogKey(keyData);
      } else return base.ProcessDialogKey(keyData);
    }

    private void customInputTextBox_Leave(object sender, EventArgs e) {
      TextBox t = (TextBox)sender;
      System.Windows.Forms.ListViewItem.ListViewSubItem subItem = t.Tag as System.Windows.Forms.ListViewItem.ListViewSubItem;
      subItem.Text = t.Text;
      t.Visible = false;
    }

    private void customInputTextBox_KeyUp(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
        customInputTextBox_Leave(sender, EventArgs.Empty);
      e.Handled = (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter);
    }

    private void parameterTypeComboBox_Leave(object sender, EventArgs e) {
      ComboBox c = (ComboBox)sender;
      System.Windows.Forms.ListViewItem.ListViewSubItem subItem = c.Tag as System.Windows.Forms.ListViewItem.ListViewSubItem;
      subItem.Text = c.Text;
      c.Visible = false;
    }

    private void parameterTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (parameterTypeComboBox.Visible)
        parameterTypeComboBox_Leave(sender, e);
    }

    private void parameterTypeComboBox_KeyUp(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
        parameterTypeComboBox_Leave(sender, EventArgs.Empty);
    }
  }
}
