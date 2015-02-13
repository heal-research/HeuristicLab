#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text.RegularExpressions;
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

namespace HeuristicLab.Scripting.Views {
  [View("ItemCollection View")]
  [Content(typeof(VariableStore), true)]
  public partial class VariableStoreView : AsynchronousContentView {
    #region Image Names
    private const string ErrorImageName = "Error";
    private const string WarningImageName = "Warning";
    private const string HeuristicLabObjectImageName = "HeuristicLabObject";
    private const string ObjectImageName = "Object";
    private const string NothingImageName = "Nothing";
    #endregion

    private readonly Regex SafeVariableNameRegex = new Regex("^[@]?[_a-zA-Z][_a-zA-Z0-9]*$");
    private const string DefaultVariableName = "enter_name";
    protected readonly Dictionary<string, ListViewItem> itemListViewItemMapping;
    protected readonly Dictionary<Type, bool> serializableLookup;
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
      serializableLookup = new Dictionary<Type, bool>();

      var images = variableListView.SmallImageList.Images;
      images.Add(ErrorImageName, Common.Resources.VSImageLibrary.Error);
      images.Add(WarningImageName, Common.Resources.VSImageLibrary.Warning);
      images.Add(HeuristicLabObjectImageName, Common.Resources.HeuristicLab.Icon.ToBitmap());
      images.Add(ObjectImageName, Common.Resources.VSImageLibrary.Object);
      images.Add(NothingImageName, Common.Resources.VSImageLibrary.Nothing);
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
        variableListView.LabelEdit = false;
      } else {
        bool enabled = !Locked && !ReadOnly;
        addButton.Enabled = enabled;
        sortAscendingButton.Enabled = variableListView.Items.Count > 1;
        sortDescendingButton.Enabled = variableListView.Items.Count > 1;
        removeButton.Enabled = enabled && variableListView.SelectedItems.Count > 0;
        variableListView.LabelEdit = enabled;
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
      bool serializable = IsSerializable(variable);

      var listViewItem = new ListViewItem();
      AssignVariableToListViewItem(listViewItem, variable);
      SetImageKey(listViewItem, serializable);
      SetToolTipText(listViewItem, serializable);
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
      if (!itemListViewItemMapping.TryGetValue(variable.Key, out listViewItem)) return;

      itemListViewItemMapping.Remove(variable.Key);
      variableListView.Items.Remove(listViewItem);
      sortAscendingButton.Enabled = variableListView.Items.Count > 1;
      sortDescendingButton.Enabled = variableListView.Items.Count > 1;
      var item = variable.Value as IItem;
      if (item != null) item.ToStringChanged -= item_ToStringChanged;
    }

    protected virtual void UpdateVariable(KeyValuePair<string, object> variable) {
      if (string.IsNullOrEmpty(variable.Key)) throw new ArgumentException("The variable must have a name.", "variable");

      ListViewItem listViewItem;
      if (!itemListViewItemMapping.TryGetValue(variable.Key, out listViewItem))
        throw new ArgumentException("A variable with the specified name does not exist.", "variable");

      bool serializable = IsSerializable(variable);
      AssignVariableToListViewItem(listViewItem, variable);
      SetImageKey(listViewItem, serializable);
      SetToolTipText(listViewItem, serializable);

    }

    #region ListView Events
    protected virtual void variableListView_SelectedIndexChanged(object sender, EventArgs e) {
      removeButton.Enabled = (Content != null) && !Locked && !ReadOnly && variableListView.SelectedItems.Count > 0;
      AdjustListViewColumnSizes();
    }
    protected virtual void variableListView_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
        case Keys.Delete:
          if ((variableListView.SelectedItems.Count > 0) && !Locked && !ReadOnly) {
            foreach (ListViewItem item in variableListView.SelectedItems)
              Content.Remove(item.Text);
          }
          break;
        case Keys.F2:
          if (variableListView.SelectedItems.Count != 1) return;
          var selectedItem = variableListView.SelectedItems[0];
          if (variableListView.LabelEdit)
            selectedItem.BeginEdit();
          break;
        case Keys.A:
          if (e.Modifiers.HasFlag(Keys.Control)) {
            foreach (ListViewItem item in variableListView.Items)
              item.Selected = true;
          }
          break;
      }
    }
    protected virtual void variableListView_DoubleClick(object sender, EventArgs e) {
      if (variableListView.SelectedItems.Count != 1) return;
      var item = variableListView.SelectedItems[0].Tag as KeyValuePair<string, object>?;
      if (item == null) return;

      var value = item.Value.Value as IContent;
      if (value == null) return;

      IContentView view = MainFormManager.MainForm.ShowContent(value);
      if (view == null) return;

      view.ReadOnly = ReadOnly;
      view.Locked = Locked;
    }
    protected virtual void variableListView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (Locked || variableListView.SelectedItems.Count != 1) return;

      var listViewItem = variableListView.SelectedItems[0];
      var item = (KeyValuePair<string, object>)listViewItem.Tag;
      if (!(item.Value is IDeepCloneable)) return;
      var data = new DataObject(HeuristicLab.Common.Constants.DragDropDataFormat, item);
      DoDragDrop(data, DragDropEffects.Copy);
    }
    protected virtual void variableListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = !Locked && !ReadOnly;

      object item = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      if (item is KeyValuePair<string, object>) {
        var variable = (KeyValuePair<string, object>)item;
        validDragOperation &= variable.Value is IDeepCloneable;
      } else {
        validDragOperation &= item is IDeepCloneable;
      }
    }
    protected virtual void variableListView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        if (e.AllowedEffect.HasFlag(DragDropEffects.Copy))
          e.Effect = DragDropEffects.Copy;
      }
    }
    protected virtual void variableListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.Copy) return;
      object item = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);

      string variableName;
      bool editLabel;

      if (item is KeyValuePair<string, object>) {
        var variable = (KeyValuePair<string, object>)item;
        variableName = GenerateNewVariableName(out editLabel, variable.Key, false);
        item = variable.Value;
      } else {
        var namedItem = item as INamedItem;
        if (namedItem != null)
          variableName = GenerateNewVariableName(out editLabel, namedItem.Name, false);
        else
          variableName = GenerateNewVariableName(out editLabel);
      }

      var cloneable = item as IDeepCloneable;
      if (cloneable == null) return;

      var clonedItem = cloneable.Clone();
      Content.Add(variableName, clonedItem);

      var listViewItem = variableListView.FindItemWithText(variableName);
      variableListView.SelectedItems.Clear();
      if (editLabel) listViewItem.BeginEdit();
    }

    private void variableListView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
      string name = e.Label;
      if (!string.IsNullOrEmpty(name)) {
        var variable = (KeyValuePair<string, object>)variableListView.Items[e.Item].Tag;
        if (!Content.ContainsKey(name)) {
          Content.Remove(variable.Key);
          Content.Add(name, variable.Value);
        }
      }
      e.CancelEdit = true;
    }
    #endregion

    #region Button Events
    protected virtual void addButton_Click(object sender, EventArgs e) {
      object variableValue = CreateItem();
      if (variableValue == null) return;

      string variableName;
      var namedItem = variableValue as INamedItem;
      if (namedItem != null)
        variableName = GenerateNewVariableName(namedItem.Name, false);
      else
        variableName = GenerateNewVariableName();

      Content.Add(variableName, variableValue);

      var item = variableListView.FindItemWithText(variableName);
      variableListView.SelectedItems.Clear();
      item.BeginEdit();
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
        AdjustListViewColumnSizes();
      }
    }
    protected virtual void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<KeyValuePair<string, object>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<KeyValuePair<string, object>>(Content_CollectionReset), sender, e);
      else {
        foreach (var item in e.OldItems)
          RemoveVariable(item);
        foreach (var item in e.Items)
          AddVariable(item);
        AdjustListViewColumnSizes();
      }
    }

    private void item_ToStringChanged(object sender, EventArgs e) {
      foreach (ListViewItem item in variableListView.Items) {
        var variable = item.Tag as KeyValuePair<string, object>?;
        if (variable == null || variable.Value.Value != sender) continue;

        string value = (variable.Value.Value ?? "null").ToString();
        item.SubItems[1].Text = value;
        item.SubItems[2].Text = variable.Value.Value.GetType().ToString();
        SetToolTipText(item, item.ImageIndex != 0);
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
      foreach (ColumnHeader ch in variableListView.Columns)
        ch.Width = -2;
    }

    protected virtual void AssignVariableToListViewItem(ListViewItem listViewItem, KeyValuePair<string, object> variable) {
      string value = (variable.Value ?? "null").ToString();
      string type = variable.Value == null ? "null" : variable.Value.GetType().ToString();

      listViewItem.Tag = variable;

      var subItems = listViewItem.SubItems;
      subItems[0].Text = variable.Key;
      if (subItems.Count == 1) { // variable information is added; subitems do not exist yet
        subItems.AddRange(new[] { value, type });
      } else { // variable information is updated; subitems are changed
        subItems[1].Text = value;
        subItems[2].Text = type;
      }
    }

    protected virtual void SetImageKey(ListViewItem listViewItem, bool serializable) {
      var variable = (KeyValuePair<string, object>)listViewItem.Tag;
      if (!serializable) listViewItem.ImageKey = ErrorImageName;
      else if (!SafeVariableNameRegex.IsMatch(variable.Key)) listViewItem.ImageKey = WarningImageName;
      else if (variable.Value is IItem) listViewItem.ImageKey = HeuristicLabObjectImageName;
      else if (variable.Value != null) listViewItem.ImageKey = ObjectImageName;
      else listViewItem.ImageKey = NothingImageName;
    }

    protected virtual void SetToolTipText(ListViewItem listViewItem, bool serializable) {
      var variable = (KeyValuePair<string, object>)listViewItem.Tag;
      if (string.IsNullOrEmpty(variable.Key)) throw new ArgumentException("The variable must have a name.", "variable");
      string value = listViewItem.SubItems[1].Text;
      string type = listViewItem.SubItems[2].Text;

      string[] lines = {
        "Name: " + variable.Key,
        "Value: " + value,
        "Type: " + type
      };

      string toolTipText = string.Join(Environment.NewLine, lines);
      if (!SafeVariableNameRegex.IsMatch(variable.Key))
        toolTipText = "Caution: Identifier is no valid C# identifier!" + Environment.NewLine + toolTipText;
      if (!serializable)
        toolTipText = "Caution: Type is not serializable!" + Environment.NewLine + toolTipText;
      listViewItem.ToolTipText = toolTipText;
    }

    private string GenerateNewVariableName(string defaultName = DefaultVariableName, bool generateValidIdentifier = true) {
      bool editLabel;
      return GenerateNewVariableName(out editLabel, defaultName, generateValidIdentifier);
    }

    private string GenerateNewVariableName(out bool defaultNameExists, string defaultName = DefaultVariableName, bool generateValidIdentifier = true) {
      if (string.IsNullOrEmpty(defaultName) || generateValidIdentifier && !SafeVariableNameRegex.IsMatch(defaultName))
        defaultName = DefaultVariableName;
      if (Content.ContainsKey(defaultName)) {
        int i = 1;
        string formatString = generateValidIdentifier ? "{0}{1}" : "{0} ({1})";
        string newName;
        do {
          newName = string.Format(formatString, defaultName, i++);
        } while (Content.ContainsKey(newName));
        defaultNameExists = true;
        return newName;
      }
      defaultNameExists = false;
      return defaultName;
    }

    private bool IsSerializable(KeyValuePair<string, object> variable) {
      Type type = null;
      bool serializable;

      if (variable.Value != null) {
        type = variable.Value.GetType();
        if (serializableLookup.TryGetValue(type, out serializable))
          return serializable;
      }

      var ser = new Serializer(variable, ConfigurationService.Instance.GetDefaultConfig(new XmlFormat()), "ROOT", true);
      try {
        serializable = ser.Count() > 0; // try to create all serialization tokens
      } catch (PersistenceException) {
        serializable = false;
      }

      if (type != null)
        serializableLookup[type] = serializable;
      return serializable;
    }
    #endregion
  }
}
