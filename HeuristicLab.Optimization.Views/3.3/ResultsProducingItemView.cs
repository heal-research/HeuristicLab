#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimization.Views {

  [View("Results Producing Item View")]
  [Content(typeof(IResultsProducingItem), true)]
  public partial class ResultsProducingItemView : AsynchronousContentView {
    public ResultsProducingItemView() {
      InitializeComponent();

      parameterGroup = listView.Groups["parametersGroup"];
      resultsGroup = listView.Groups["resultsGroup"];
    }

    public new IResultsProducingItem Content {
      get { return (IResultsProducingItem)base.Content; }
      set { base.Content = value; }
    }

    private Dictionary<IItem, ListViewItem> itemToListViewItem = new Dictionary<IItem, ListViewItem>();
    private ListViewGroup parameterGroup;
    private ListViewGroup resultsGroup;

    protected override void OnContentChanged() {
      base.OnContentChanged();
      listView.Items.Clear();
      itemToListViewItem.Clear();
      RebuildImageList();

      viewHost.Content = null;

      if (Content != null) {
        FillListView();
        AdjustListViewColumnSizes();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();

      if (Content == null) {
        addButton.Enabled = false;
        sortAscendingButton.Enabled = false;
        sortDescendingButton.Enabled = false;
        removeButton.Enabled = false;
        listView.Enabled = false;
        detailsGroupBox.Enabled = false;
        return;
      }

      addButton.Enabled = !ReadOnly && !Content.Parameters.IsReadOnly && !((IKeyedItemCollection<string, IResult>)Content.Results).IsReadOnly;
      sortAscendingButton.Enabled = listView.Items.Count > 1;
      sortDescendingButton.Enabled = listView.Items.Count > 1;
      removeButton.Enabled = !ReadOnly && !Content.Parameters.IsReadOnly && !((IKeyedItemCollection<string, IResult>)Content.Results).IsReadOnly;
      listView.Enabled = true;
      detailsGroupBox.Enabled = listView.SelectedItems.Count == 1;
    }

    #region Content event registration
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      RegisterContentParametersEvents();
      RegisterContentResultsEents();
    }
    private void RegisterContentParametersEvents() {
      Content.Parameters.ItemsAdded += ParametersOnItemsChanged;
      Content.Parameters.ItemsRemoved += ParametersOnItemsChanged;
      Content.Parameters.ItemsReplaced += ParametersOnItemsChanged;
      Content.Parameters.CollectionReset += ParametersOnItemsChanged;
    }
    private void RegisterContentResultsEents() {
      Content.Results.ItemsAdded += ResultsOnItemsChanged;
      Content.Results.ItemsRemoved += ResultsOnItemsChanged;
      Content.Results.ItemsReplaced += ResultsOnItemsChanged;
      Content.Results.CollectionReset += ResultsOnItemsChanged;
    }
    protected virtual void RegisterItemEvents(IItem item) {
      item.ItemImageChanged += new EventHandler(Item_ItemImageChanged);
      item.ToStringChanged += new EventHandler(Item_ToStringChanged);
    }

    protected override void DeregisterContentEvents() {
      DeregisterContentParametersEvents();
      DeregisterContentResultsEvents();

      foreach (var item in itemToListViewItem.Keys) {
        DeregisterItemEvents(item);
      }
      base.DeregisterContentEvents();
    }
    private void DeregisterContentParametersEvents() {
      Content.Parameters.ItemsAdded -= ParametersOnItemsChanged;
      Content.Parameters.ItemsRemoved -= ParametersOnItemsChanged;
      Content.Parameters.ItemsReplaced -= ParametersOnItemsChanged;
      Content.Parameters.CollectionReset -= ParametersOnItemsChanged;
    }
    private void DeregisterContentResultsEvents() {
      Content.Results.ItemsAdded -= ResultsOnItemsChanged;
      Content.Results.ItemsRemoved -= ResultsOnItemsChanged;
      Content.Results.ItemsReplaced -= ResultsOnItemsChanged;
      Content.Results.CollectionReset -= ResultsOnItemsChanged;
    }
    protected virtual void DeregisterItemEvents(IItem item) {
      item.ItemImageChanged -= new EventHandler(Item_ItemImageChanged);
      item.ToStringChanged -= new EventHandler(Item_ToStringChanged);
    }
    #endregion



    private string selectedName;
    private void FillListView() {
      if (listView.SelectedItems.Count == 1) selectedName = listView.SelectedItems[0].SubItems[0].Text;

      FillListView(false);
      if (listView.Items.Count > 0)
        selectedName = null;
    }


    private void FillListView(bool resize = true) {
      listView.BeginUpdate();
      foreach (var item in listView.Items.OfType<ListViewItem>().OrderByDescending(x => x.ImageIndex).ToList()) {
        listView.SmallImageList.Images.RemoveAt(item.ImageIndex);
        listView.Items.Remove(item);
      }
      itemToListViewItem.Clear();

      var counter = 0;
      foreach (var item in listView.Items.OfType<ListViewItem>().OrderBy(x => x.ImageIndex).ToList())
        item.ImageIndex = counter++;
      listView.EndUpdate();

      if (Content == null) return;

      //Fill list view with new content
      foreach (var parameter in Content.Parameters) {
        var listViewItem = CreateListViewItem(parameter, parameterGroup);
        listView.Items.Add(listViewItem);
        if ((selectedName != null) && parameter.Name.Equals(selectedName)) listViewItem.Selected = true;
        itemToListViewItem[parameter] = listViewItem;
      }

      foreach (var result in Content.Results) {
        var listViewItem = CreateListViewItem(result, resultsGroup);
        listView.Items.Add(listViewItem);
        if ((selectedName != null) && result.Name.Equals(selectedName)) listViewItem.Selected = true;
        itemToListViewItem[result] = listViewItem;
      }

      if (resize && listView.Items.Count > 0) {
        for (int i = 0; i < listView.Columns.Count; i++)
          listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
    }

    private void ParametersOnItemsChanged(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      foreach (var item in e.OldItems) {
        listView.Items.Remove(itemToListViewItem[item]);
        itemToListViewItem.Remove(item);
      }
      foreach (var item in e.Items) {
        var listViewItem = CreateListViewItem(item, parameterGroup);
        listView.Items.Add(listViewItem);
        itemToListViewItem[item] = listViewItem;
      }
      AdjustListViewColumnSizes();
    }

    private void ResultsOnItemsChanged(object sender, CollectionItemsChangedEventArgs<IResult> e) {
      foreach (var item in e.OldItems) {
        listView.Items.Remove(itemToListViewItem[item]);
        itemToListViewItem.Remove(item);
      }
      foreach (var item in e.Items) {
        var listViewItem = CreateListViewItem(item, resultsGroup);
        listView.Items.Add(listViewItem);
        itemToListViewItem[item] = listViewItem;
      }
      AdjustListViewColumnSizes();
    }


    private ListViewItem CreateListViewItem(INamedItem value, ListViewGroup group) {
      var item = new ListViewItem();
      item.Text = value.ToString();
      item.ToolTipText = value.Name + ": " + value.Description;
      item.Tag = value;
      item.Group = group;
      listView.SmallImageList.Images.Add(value == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : value.ItemImage);
      item.ImageIndex = listView.SmallImageList.Images.Count - 1;
      return item;
    }

    private void UpdateListViewItemImage(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      var item = listViewItem.Tag as IItem;
      int i = listViewItem.ImageIndex;
      listView.SmallImageList.Images[i] = item == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : item.ItemImage;
      listViewItem.ImageIndex = -1;
      listViewItem.ImageIndex = i;
    }
    private void UpdateListViewItemText(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      var item = listViewItem.Tag as INamedItem;
      listViewItem.Text = item.ToString();
      listViewItem.ToolTipText = item.Name + ": " + item.Description;
    }

    #region listView events
    private void listView_SelectedIndexChanged(object sender, EventArgs e) {
      if (!showDetailsCheckBox.Checked) return;

      if (listView.SelectedItems.Count == 1) {
        detailsGroupBox.Enabled = true;
        viewHost.Content = listView.SelectedItems[0].Tag as IContent;
      } else {
        viewHost.Content = null;
        detailsGroupBox.Enabled = false;
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

    protected virtual void listView_Layout(object sender, LayoutEventArgs e) {
      AdjustListViewColumnSizes();
    }
    protected virtual void listView_Resize(object sender, EventArgs e) {
      AdjustListViewColumnSizes();
    }
    #endregion

    #region Item Events
    protected virtual void Item_ItemImageChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ItemImageChanged), sender, e);
      else {
        IItem item = (IItem)sender;
        var listViewItem = itemToListViewItem[item];
        UpdateListViewItemImage(listViewItem);
      }
    }

    protected virtual void Item_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ToStringChanged), sender, e);
      else {
        IItem item = (IItem)sender;
        var listViewItem = itemToListViewItem[item];
        UpdateListViewItemText(listViewItem);
        if (listView.Columns.Count > 1)
          AdjustListViewColumnSizes();
      }
    }
    #endregion

    #region Helper Methods
    protected virtual void AdjustListViewColumnSizes() {
      if (listView.Columns.Count == 1)
        listView.Columns[0].Width = listView.ClientSize.Width;
      else {
        if (listView.Items.Count > 0) {
          for (int i = 0; i < listView.Columns.Count; i++)
            listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }
    protected virtual void RebuildImageList() {
      listView.SmallImageList.Images.Clear();
      foreach (ListViewItem listViewItem in listView.Items) {
        var item = listViewItem.Tag as IItem;
        listView.SmallImageList.Images.Add(item == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : item.ItemImage);
        listViewItem.ImageIndex = listView.SmallImageList.Images.Count - 1;
      }
    }
    #endregion
  }
}

