#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.HLScript.Views {
  [View("ItemCollection View")]
  [Content(typeof(VariableStore), true)]
  public partial class VariableStoreView : AsynchronousContentView {
    protected Dictionary<string, ListViewItem> itemListViewItemMapping;
    protected TypeSelectorDialog typeSelectorDialog;
    protected bool validDragOperation;

    public new VariableStore Content {
      get { return (VariableStore)base.Content; }
      set { base.Content = value; }
    }

    public ListView ItemsListView {
      get { return variableListView; }
    }

    public VariableStoreView() {
      InitializeComponent();
      itemListViewItemMapping = new Dictionary<string, ListViewItem>();
      variableListView.SmallImageList.Images.AddRange(new Image[] {
        HeuristicLab.Common.Resources.VSImageLibrary.Error,
        HeuristicLab.Common.Resources.VSImageLibrary.Object,
        HeuristicLab.Common.Resources.VSImageLibrary.Nothing
      });
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= Content_ItemsAdded;
      Content.ItemsReplaced -= Content_ItemsReplaced;
      Content.ItemsRemoved -= Content_ItemsRemoved;
      Content.CollectionReset -= Content_CollectionReset;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += Content_ItemsAdded;
      Content.ItemsReplaced += Content_ItemsReplaced;
      Content.ItemsRemoved += Content_ItemsRemoved;
      Content.CollectionReset += Content_CollectionReset;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      variableListView.Items.Clear();
      itemListViewItemMapping.Clear();
      RebuildImageList();
      if (Content != null) {
        Caption += " (" + Content.GetType().Name + ")";
        foreach (var item in Content)
          AddVariable(item);
        AdjustListViewColumnSizes();
        SortItemsListView(SortOrder.Ascending);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null) {
        addButton.Enabled = false;
        sortAscendingButton.Enabled = false;
        sortDescendingButton.Enabled = false;
        removeButton.Enabled = false;
        variableListView.Enabled = false;
      } else {
        addButton.Enabled = !Locked && !ReadOnly;
        sortAscendingButton.Enabled = variableListView.Items.Count > 1;
        sortDescendingButton.Enabled = variableListView.Items.Count > 1;
        removeButton.Enabled = !Locked && !ReadOnly && variableListView.SelectedItems.Count > 0;
        variableListView.Enabled = true;
        variableListView.LabelEdit = !Locked && !ReadOnly;
      }
    }

    protected virtual object CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog { Caption = "Select Item" };
        typeSelectorDialog.TypeSelector.Caption = "Available Items";
        typeSelectorDialog.TypeSelector.Configure(typeof(IItem), false, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          return (object)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
      return null;
    }

    protected virtual void AddVariable(KeyValuePair<string, object> variable) {
      if (string.IsNullOrEmpty(variable.Key)) throw new ArgumentException("The variable must have a name.", "variable");
      string value = (variable.Value ?? "null").ToString();
      string type = variable.Value == null ? "null" : variable.Value.GetType().ToString();
      bool serializable = IsSerializable(variable);
      var listViewItem = new ListViewItem(new[] { variable.Key, value, type }) { ToolTipText = GetToolTipText(variable, serializable), Tag = variable };
      if (serializable) {
        listViewItem.ImageIndex = variable.Value == null ? 2 : 1;
      } else listViewItem.ImageIndex = 0;
      variableListView.Items.Add(listViewItem);
      itemListViewItemMapping[variable.Key] = listViewItem;
      sortAscendingButton.Enabled = variableListView.Items.Count > 1;
      sortDescendingButton.Enabled = variableListView.Items.Count > 1;
      var item = variable.Value as IItem;
      if (item != null) item.ToStringChanged += item_ToStringChanged;
    }

    protected virtual void RemoveVariable(KeyValuePair<string, object> variable) {
      if (string.IsNullOrEmpty(variable.Key)) throw new ArgumentException("The variable must have a name.", "variable");
      ListViewItem listViewItem;
      if (itemListViewItemMapping.TryGetValue(variable.Key, out listViewItem)) {
        itemListViewItemMapping.Remove(variable.Key);
        variableListView.Items.Remove(listViewItem);
        sortAscendingButton.Enabled = variableListView.Items.Count > 1;
        sortDescendingButton.Enabled = variableListView.Items.Count > 1;
        var item = variable.Value as IItem;
        if (item != null) item.ToStringChanged -= item_ToStringChanged;
      }
    }

    protected virtual void UpdateVariable(KeyValuePair<string, object> variable) {
      if (string.IsNullOrEmpty(variable.Key)) throw new ArgumentException("The variable must have a name.", "variable");
      ListViewItem listViewItem;
      if (itemListViewItemMapping.TryGetValue(variable.Key, out listViewItem)) {
        string value = (variable.Value ?? "null").ToString();
        string type = variable.Value == null ? "null" : variable.Value.GetType().ToString();
        bool serializable = IsSerializable(variable);
        if (serializable) {
          listViewItem.ImageIndex = variable.Value == null ? 2 : 1;
        } else listViewItem.ImageIndex = 0;
        listViewItem.SubItems[1].Text = value;
        listViewItem.SubItems[2].Text = type;
        listViewItem.ToolTipText = GetToolTipText(variable, serializable);
        listViewItem.Tag = variable;
      } else throw new ArgumentException("A variable with the specified name does not exist.", "variable");
    }

    #region ListView Events
    protected virtual void variableListView_SelectedIndexChanged(object sender, EventArgs e) {
      removeButton.Enabled = (Content != null) && !Locked && !ReadOnly && variableListView.SelectedItems.Count > 0;
      AdjustListViewColumnSizes();
    }
    protected virtual void variableListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if ((variableListView.SelectedItems.Count > 0) && !Locked && !ReadOnly) {
          foreach (ListViewItem item in variableListView.SelectedItems)
            Content.Remove(item.Text);
        }
      }
    }
    protected virtual void variableListView_DoubleClick(object sender, EventArgs e) {
      if (variableListView.SelectedItems.Count == 1) {
        var item = variableListView.SelectedItems[0].Tag as KeyValuePair<string, object>?;
        if (item != null) {
          var value = item.Value.Value as IContent;
          if (value != null) {
            IContentView view = MainFormManager.MainForm.ShowContent(value);
            if (view != null) {
              view.ReadOnly = ReadOnly;
              view.Locked = Locked;
            }
          }
        }
      }
    }
    protected virtual void variableListView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!Locked) {
        var items = new List<object>();
        foreach (ListViewItem listViewItem in variableListView.SelectedItems) {
          var item = (KeyValuePair<string, object>)listViewItem.Tag as KeyValuePair<string, object>?;
          if (item != null) items.Add(item.Value.Value);
        }

        if (items.Count > 0) {
          DataObject data = new DataObject();
          if (items.Count == 1) data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, items[0]);
          else data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, items);
          if (ReadOnly) {
            DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
          } else {
            DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
            if ((result & DragDropEffects.Move) == DragDropEffects.Move) {
              foreach (string item in items) Content.Remove(item);
            }
          }
        }
      }
    }
    protected virtual void variableListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (!Locked && !ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is object)) {
        validDragOperation = true;
      } else if (!Locked && !ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable)) {
        validDragOperation = true;
        IEnumerable items = (IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        foreach (object item in items)
          validDragOperation = validDragOperation && (item is object);
      }
    }
    protected virtual void variableListView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    protected virtual void variableListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable) {
          IEnumerable<object> items = ((IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat)).Cast<object>();
          if (e.Effect.HasFlag(DragDropEffects.Copy)) {
            var cloner = new Cloner();
            var clonedItems = new List<object>();
            foreach (var item in items) {
              var dc = item as IDeepCloneable;
              clonedItems.Add(dc != null ? cloner.Clone(dc) : item);
            }
            items = clonedItems;
          }
          foreach (var item in items) {
            string name = GenerateNewVariableName();
            Content.Add(name, item);
            var listViewItem = variableListView.FindItemWithText(name);
            listViewItem.BeginEdit();
          }
        } else {
          object item = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
          if (e.Effect.HasFlag(DragDropEffects.Copy)) {
            var cloner = new Cloner();
            var dc = item as IDeepCloneable;
            if (dc != null) item = cloner.Clone(dc);
          }
          string name = GenerateNewVariableName();
          Content.Add(name, item);
          var listViewItem = variableListView.FindItemWithText(name);
          listViewItem.BeginEdit();
        }
      }
    }

    private void variableListView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
      if (!string.IsNullOrEmpty(e.Label)) {
        var variable = (KeyValuePair<string, object>)variableListView.Items[e.Item].Tag;
        if (!Content.ContainsKey(e.Label)) {
          Content.Remove(variable.Key);
          Content.Add(e.Label, variable.Value);
        }
      }
      e.CancelEdit = true;
    }
    #endregion

    #region Button Events
    protected virtual void addButton_Click(object sender, EventArgs e) {
      object newVar = CreateItem();
      if (newVar != null) {
        string name = GenerateNewVariableName();
        Content.Add(name, newVar);
        var item = variableListView.FindItemWithText(name);
        item.BeginEdit();
      }
    }
    protected virtual void sortAscendingButton_Click(object sender, EventArgs e) {
      SortItemsListView(SortOrder.Ascending);
    }
    protected virtual void sortDescendingButton_Click(object sender, EventArgs e) {
      SortItemsListView(SortOrder.Descending);
    }
    protected virtual void removeButton_Click(object sender, EventArgs e) {
      if (variableListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in variableListView.SelectedItems)
          Content.Remove(item.Text);
        variableListView.SelectedItems.Clear();
      }
    }
    #endregion

    #region Content Events
    protected virtual void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<KeyValuePair<string, object>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<KeyValuePair<string, object>>(Content_ItemsAdded), sender, e);
      else {
        foreach (var item in e.Items)
          AddVariable(item);
        AdjustListViewColumnSizes();
      }
    }

    protected virtual void Content_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<KeyValuePair<string, object>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<KeyValuePair<string, object>>(Content_ItemsReplaced), sender, e);
      else {
        foreach (var item in e.Items)
          UpdateVariable(item);
        AdjustListViewColumnSizes();
      }
    }

    protected virtual void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<KeyValuePair<string, object>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<KeyValuePair<string, object>>(Content_ItemsRemoved), sender, e);
      else {
        foreach (var item in e.Items)
          RemoveVariable(item);
        RebuildImageList();
        AdjustListViewColumnSizes();
      }
    }
    protected virtual void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<KeyValuePair<string, object>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<KeyValuePair<string, object>>(Content_CollectionReset), sender, e);
      else {
        foreach (var item in e.OldItems)
          RemoveVariable(item);
        RebuildImageList();
        foreach (var item in e.Items)
          AddVariable(item);
        AdjustListViewColumnSizes();
      }
    }

    private void item_ToStringChanged(object sender, EventArgs e) {
      foreach (ListViewItem item in variableListView.Items) {
        var variable = item.Tag as KeyValuePair<string, object>?;
        if (variable != null && variable.Value.Value == sender) {
          string value = (variable.Value.Value ?? "null").ToString();
          item.SubItems[1].Text = value;
          item.SubItems[2].Text = variable.Value.Value.GetType().ToString();
          item.ToolTipText = GetToolTipText(variable.Value, item.ImageIndex == 0);
          return;
        }
      }
    }
    #endregion

    #region Helpers
    protected virtual void SortItemsListView(SortOrder sortOrder) {
      variableListView.Sorting = SortOrder.None;
      variableListView.Sorting = sortOrder;
      variableListView.Sorting = SortOrder.None;
    }
    protected virtual void AdjustListViewColumnSizes() {
      if (variableListView.Items.Count > 0) {
        for (int i = 0; i < variableListView.Columns.Count; i++)
          variableListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
    }
    protected virtual void RebuildImageList() {
      variableListView.SmallImageList.Images.Clear();
      variableListView.SmallImageList.Images.AddRange(new Image[] {
        HeuristicLab.Common.Resources.VSImageLibrary.Error,
        HeuristicLab.Common.Resources.VSImageLibrary.Object,
        HeuristicLab.Common.Resources.VSImageLibrary.Nothing
      });
      foreach (ListViewItem listViewItem in variableListView.Items) {
        var variable = (KeyValuePair<string, object>)listViewItem.Tag;
        bool serializable = IsSerializable(variable);
        if (serializable) {
          listViewItem.ImageIndex = variable.Value == null ? 2 : 1;
        } else listViewItem.ImageIndex = 0;
      }
    }

    private string GetToolTipText(KeyValuePair<string, object> variable, bool serializable) {
      if (string.IsNullOrEmpty(variable.Key)) throw new ArgumentException("The variable must have a name.", "variable");
      string value = (variable.Value ?? "null").ToString();
      string type = variable.Value == null ? "null" : variable.Value.GetType().ToString();
      string[] lines = {
        "Name: " + variable.Key,
        "Value: " + value,
        "Type: " + type
      };
      string toolTipText = string.Join(Environment.NewLine, lines);
      if (!serializable)
        toolTipText += Environment.NewLine + "CAUTION: Type is not serializable!";
      return toolTipText;
    }

    private string GenerateNewVariableName(string defaultName = "enter_name") {
      if (Content.ContainsKey(defaultName)) {
        int i = 1;
        string newName;
        do {
          newName = defaultName + i++;
        } while (Content.ContainsKey(newName));
        return newName;
      }
      return defaultName;
    }

    private bool IsSerializable(KeyValuePair<string, object> variable) {
      var ser = new Serializer(variable, ConfigurationService.Instance.GetDefaultConfig(new XmlFormat()), "ROOT", true);
      try {
        return ser.Count() > 0;
      } catch (PersistenceException) {
        return false;
      }
    }
    #endregion
  }
}
