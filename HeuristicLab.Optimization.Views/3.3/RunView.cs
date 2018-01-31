#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The visual representation of a <see cref="Variable"/>.
  /// </summary>
  [View("Run View")]
  [Content(typeof(IRun), true)]
  public sealed partial class RunView : NamedItemView {
    private readonly Dictionary<string, ListViewItem> parametersItemToListViewItem;
    private readonly Dictionary<string, ListViewItem> resultsItemToListViewItem;
    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new IRun Content {
      get { return (IRun)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public RunView() {
      InitializeComponent();
      parametersItemToListViewItem = new Dictionary<string, ListViewItem>();
      resultsItemToListViewItem = new Dictionary<string, ListViewItem>();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += Content_PropertyChanged;
      RegisterContentParametersEvents();
      RegisterContentResultsEents();
    }
    private void RegisterContentParametersEvents() {
      Content.Parameters.ItemsAdded += ParametersOnItemsChanged;
      Content.Parameters.ItemsRemoved += ParametersOnItemsRemoved;
      Content.Parameters.ItemsReplaced += ParametersOnItemsChanged;
      Content.Parameters.CollectionReset += ParametersOnItemsChanged;
    }
    private void RegisterContentResultsEents() {
      Content.Results.ItemsAdded += ResultsOnItemsChanged;
      Content.Results.ItemsRemoved += ResultsOnItemsRemoved;
      Content.Results.ItemsReplaced += ResultsOnItemsChanged;
      Content.Results.CollectionReset += ResultsOnItemsChanged;
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.PropertyChanged -= Content_PropertyChanged;
      DeregisterContentParametersEvents();
      DeregisterContentResultsEvents();
    }
    private void DeregisterContentParametersEvents() {
      Content.Parameters.ItemsAdded -= ParametersOnItemsChanged;
      Content.Parameters.ItemsRemoved -= ParametersOnItemsRemoved;
      Content.Parameters.ItemsReplaced -= ParametersOnItemsChanged;
      Content.Parameters.CollectionReset -= ParametersOnItemsChanged;
    }
    private void DeregisterContentResultsEvents() {
      Content.Results.ItemsAdded -= ResultsOnItemsChanged;
      Content.Results.ItemsRemoved -= ResultsOnItemsRemoved;
      Content.Results.ItemsReplaced -= ResultsOnItemsChanged;
      Content.Results.CollectionReset -= ResultsOnItemsChanged;
    }

    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName == "Color") {
        if (InvokeRequired) this.Invoke((Action)UpdateColor, null);
        else UpdateColor();
      } else if (e.PropertyName == "Parameters") {
        if (InvokeRequired) this.Invoke((Action<bool>)FillParametersListView, true);
        else FillParametersListView();
        RegisterContentParametersEvents();
      } else if (e.PropertyName == "Results") {
        if (InvokeRequired) this.Invoke((Action<bool>)FillResultsListView, true);
        else FillResultsListView();
        RegisterContentResultsEents();
      }
    }

    private void ParametersOnItemsChanged(object sender, CollectionItemsChangedEventArgs<KeyValuePair<string, IItem>> e) {
      foreach (var item in e.OldItems) {
        listView.Items.Remove(parametersItemToListViewItem[item.Key]);
        parametersItemToListViewItem.Remove(item.Key);
      }
      foreach (var item in e.Items) {
        var listViewItem = CreateListViewItem(item.Key, Content.Parameters[item.Key], listView.Groups["parametersGroup"]);
        listView.Items.Add(listViewItem);
        parametersItemToListViewItem[item.Key] = listViewItem;
      }
    }

    private void ParametersOnItemsRemoved(object sender, CollectionItemsChangedEventArgs<KeyValuePair<string, IItem>> e) {
      foreach (var item in e.Items) {
        listView.Items.Remove(parametersItemToListViewItem[item.Key]);
        parametersItemToListViewItem.Remove(item.Key);
      }
    }

    private void ResultsOnItemsChanged(object sender, CollectionItemsChangedEventArgs<KeyValuePair<string, IItem>> e) {
      foreach (var item in e.OldItems) {
        listView.Items.Remove(resultsItemToListViewItem[item.Key]);
        resultsItemToListViewItem.Remove(item.Key);
      }
      foreach (var item in e.Items) {
        var listViewItem = CreateListViewItem(item.Key, Content.Results[item.Key], listView.Groups["resultsGroup"]);
        listView.Items.Add(listViewItem);
        resultsItemToListViewItem[item.Key] = listViewItem;
      }
    }

    private void ResultsOnItemsRemoved(object sender, CollectionItemsChangedEventArgs<KeyValuePair<string, IItem>> e) {
      foreach (var item in e.Items) {
        listView.Items.Remove(resultsItemToListViewItem[item.Key]);
        resultsItemToListViewItem.Remove(item.Key);
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      viewHost.Content = null;
      if (Content != null)
        UpdateColor();
      FillListView();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      listView.Enabled = Content != null;
      detailsGroupBox.Enabled = (Content != null) && (listView.SelectedItems.Count == 1);
      changeColorButton.Enabled = Content != null;
      showAlgorithmButton.Enabled = Content != null && Content.Algorithm != null && !Locked;
    }

    protected override void PropagateStateChanges(Control control, Type type, System.Reflection.PropertyInfo propertyInfo) {
      if (propertyInfo.Name == "ReadOnly") return;
      base.PropagateStateChanges(control, type, propertyInfo);
    }

    private void changeColorButton_Click(object sender, EventArgs e) {
      if (colorDialog.ShowDialog(this) == DialogResult.OK) {
        this.Content.Color = this.colorDialog.Color;
      }
    }
    private void UpdateColor() {
      this.colorDialog.Color = this.Content.Color;
      this.colorArea.BackColor = this.Content.Color;
    }

    private string selectedName;
    private void FillListView() {
      if (listView.SelectedItems.Count == 1) selectedName = listView.SelectedItems[0].SubItems[0].Text;

      FillParametersListView(false);
      FillResultsListView(false);
      if (listView.Items.Count > 0) {
        for (int i = 0; i < listView.Columns.Count; i++)
          listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        selectedName = null;
      }
    }

    private void FillParametersListView(bool resize = true) {
      listView.BeginUpdate();
      foreach (var item in listView.Groups["parametersGroup"].Items.OfType<ListViewItem>().OrderByDescending(x => x.ImageIndex).ToList()) {
        listView.SmallImageList.Images.RemoveAt(item.ImageIndex);
        listView.Items.Remove(item);
      }
      parametersItemToListViewItem.Clear();

      var counter = 0;
      foreach (var item in listView.Items.OfType<ListViewItem>().OrderBy(x => x.ImageIndex).ToList())
        item.ImageIndex = counter++;
      listView.EndUpdate();

      if (Content != null) {
        foreach (string key in Content.Parameters.Keys) {
          var listViewItem = CreateListViewItem(key, Content.Parameters[key], listView.Groups["parametersGroup"]);
          listView.Items.Add(listViewItem);
          if ((selectedName != null) && key.Equals(selectedName)) listViewItem.Selected = true;
          parametersItemToListViewItem[key] = listViewItem;
        }
        if (resize && listView.Items.Count > 0) {
          for (int i = 0; i < listView.Columns.Count; i++)
            listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }

    private void FillResultsListView(bool resize = true) {
      listView.BeginUpdate();
      foreach (var item in listView.Groups["resultsGroup"].Items.OfType<ListViewItem>().OrderByDescending(x => x.ImageIndex).ToList()) {
        listView.SmallImageList.Images.RemoveAt(item.ImageIndex);
        listView.Items.Remove(item);
      }
      resultsItemToListViewItem.Clear();

      var counter = 0;
      foreach (var item in listView.Items.OfType<ListViewItem>().OrderBy(x => x.ImageIndex).ToList())
        item.ImageIndex = counter++;
      listView.EndUpdate();

      if (Content != null) {
        foreach (string key in Content.Results.Keys) {
          var listViewItem = CreateListViewItem(key, Content.Results[key], listView.Groups["resultsGroup"]);
          listView.Items.Add(listViewItem);
          if ((selectedName != null) && key.Equals(selectedName)) listViewItem.Selected = true;
          resultsItemToListViewItem[key] = listViewItem;
        }
        if (resize && listView.Items.Count > 0) {
          for (int i = 0; i < listView.Columns.Count; i++)
            listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }

    private ListViewItem CreateListViewItem(string name, IItem value, ListViewGroup group) {
      var item = new ListViewItem(new string[] { name, value != null ? value.ToString() : "-" });
      item.Tag = value;
      item.Group = group;
      listView.SmallImageList.Images.Add(value == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : value.ItemImage);
      item.ImageIndex = listView.SmallImageList.Images.Count - 1;
      return item;
    }

    private void listView_SelectedIndexChanged(object sender, EventArgs e) {
      if (showDetailsCheckBox.Checked) {
        if (listView.SelectedItems.Count == 1) {
          detailsGroupBox.Enabled = true;
          viewHost.Content = listView.SelectedItems[0].Tag as IContent;
        } else {
          viewHost.Content = null;
          detailsGroupBox.Enabled = false;
        }
      }
    }
    private void listView_DoubleClick(object sender, EventArgs e) {
      if (listView.SelectedItems.Count == 1) {
        IItem item = (IItem)listView.SelectedItems[0].Tag;
        IContentView view = MainFormManager.MainForm.ShowContent(item);
        if (view != null) {
          view.ReadOnly = true;
          view.Locked = Locked;
        }
      }
    }
    private void listView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!Locked) {
        ListViewItem listViewItem = (ListViewItem)e.Item;
        IItem item = (IItem)listViewItem.Tag;
        if (item != null) {
          DataObject data = new DataObject();
          data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, item);
          DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy);
        }
      }
    }
    private void showDetailsCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (showDetailsCheckBox.Checked) {
        splitContainer.Panel2Collapsed = false;
        detailsGroupBox.Enabled = listView.SelectedItems.Count == 1;
        viewHost.Content = listView.SelectedItems.Count == 1 ? (IContent)listView.SelectedItems[0].Tag : null;
      } else {
        splitContainer.Panel2Collapsed = true;
        viewHost.Content = null;
      }
    }
    private void showAlgorithmButton_Click(object sender, EventArgs e) {
      if (!Locked) {
        MainFormManager.MainForm.ShowContent((IContent)Content.Algorithm.Clone());
      }
    }
  }
}
