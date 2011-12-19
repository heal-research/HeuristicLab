#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization.Views {
  [View("RunCollection View")]
  [Content(typeof(RunCollection), true)]
  [Content(typeof(IItemCollection<IRun>), false)]
  public sealed partial class RunCollectionView : ItemView {
    private Dictionary<IRun, List<ListViewItem>> itemListViewItemMapping;
    private bool validDragOperation;

    public new IItemCollection<IRun> Content {
      get { return (IItemCollection<IRun>)base.Content; }
      set { base.Content = value; }
    }

    public RunCollection RunCollection {
      get { return Content as RunCollection; }
    }

    public ListView ItemsListView {
      get { return itemsListView; }
    }

    public RunCollectionView() {
      InitializeComponent();
      itemsGroupBox.Text = "Runs";
      itemListViewItemMapping = new Dictionary<IRun, List<ListViewItem>>();
      runCollectionModifiersListView.Evaluator = EvaluateModifications;
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      foreach (IRun run in itemListViewItemMapping.Keys) {
        DeregisterItemEvents(run);
      }
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
    }
    private void DeregisterItemEvents(IRun item) {
      item.ItemImageChanged -= new EventHandler(Item_ItemImageChanged);
      item.ToStringChanged -= new EventHandler(Item_ToStringChanged);
      item.Changed -= new EventHandler(Item_Changed);
    }
    private void RegisterItemEvents(IRun item) {
      item.ItemImageChanged += new EventHandler(Item_ItemImageChanged);
      item.ToStringChanged += new EventHandler(Item_ToStringChanged);
      item.Changed += new EventHandler(Item_Changed);
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      var viewTypes = MainFormManager.GetViewTypes(typeof(RunCollection), true);
      foreach (Type viewType in viewTypes.OrderBy(x => ViewAttribute.GetViewName(x))) {
        if ((viewType != typeof(ItemCollectionView<IRun>)) && (viewType != typeof(ViewHost))) {
          ToolStripMenuItem menuItem = new ToolStripMenuItem();
          menuItem.Text = ViewAttribute.GetViewName(viewType);
          menuItem.Tag = viewType;
          menuItem.Click += new EventHandler(menuItem_Click);
          analyzeRunsToolStripDropDownButton.DropDownItems.Add(menuItem);
        }
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      string selectedName = null;
      if ((itemsListView.SelectedItems.Count == 1) && (itemsListView.SelectedItems[0].Tag != null))
        selectedName = ((IRun)itemsListView.SelectedItems[0].Tag).Name;

      itemsListView.Items.Clear();
      itemListViewItemMapping.Clear();
      RebuildImageList();
      viewHost.Content = null;

      if (Content != null) {
        if (RunCollection != null) {
          if (!tabControl.TabPages.Contains(constraintPage))
            tabControl.TabPages.Add(constraintPage);
          runCollectionConstraintCollectionView.Content = RunCollection.Constraints;
          runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
          if (!tabControl.TabPages.Contains(modifiersPage))
            tabControl.TabPages.Add(modifiersPage);
          runCollectionModifiersListView.Content = RunCollection.Modifiers;
        }
        foreach (IRun item in Content) {
          ListViewItem listViewItem = CreateListViewItem(item);
          AddListViewItem(listViewItem);
          if ((selectedName != null) && item.Name.Equals(selectedName))
            listViewItem.Selected = true;
        }
        AdjustListViewColumnSizes();
      } else {
        runCollectionConstraintCollectionView.Content = null;
        if (tabControl.TabPages.Contains(constraintPage))
          tabControl.TabPages.Remove(constraintPage);
        if (tabControl.TabPages.Contains(modifiersPage))
          tabControl.TabPages.Remove(modifiersPage);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null) {
        analyzeRunsToolStripDropDownButton.Enabled = false;
        runCollectionConstraintCollectionView.ReadOnly = true;
        itemsListView.Enabled = false;
        detailsGroupBox.Enabled = false;
        viewHost.Enabled = false;
        removeButton.Enabled = false;
        clearButton.Enabled = false;
      } else {
        analyzeRunsToolStripDropDownButton.Enabled = itemsListView.Items.Count > 0;
        runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
        itemsListView.Enabled = true;
        detailsGroupBox.Enabled = itemsListView.SelectedItems.Count == 1;
        removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        clearButton.Enabled = itemsListView.Items.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        viewHost.Enabled = true;
      }
    }

    private ListViewItem CreateListViewItem(IRun item) {
      ListViewItem listViewItem = new ListViewItem();
      if (item == null) {
        listViewItem.Text = "null";
        itemsListView.SmallImageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Nothing);
        listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      } else {
        listViewItem.Text = item.ToString();
        listViewItem.ToolTipText = item.ItemName + ": " + item.ItemDescription;
        itemsListView.SmallImageList.Images.Add(item.ItemImage);
        listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
        listViewItem.Tag = item;

        if (item.Visible) {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
          listViewItem.ForeColor = item.Color;
        } else {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Italic);
          listViewItem.ForeColor = Color.LightGray;
        }
      }
      return listViewItem;
    }
    private void AddListViewItem(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      itemsListView.Items.Add(listViewItem);
      IRun run = listViewItem.Tag as IRun;
      if (run != null) {
        if (!itemListViewItemMapping.ContainsKey(run)) {
          itemListViewItemMapping.Add(run, new List<ListViewItem>());
          RegisterItemEvents(run);
        }
        itemListViewItemMapping[run].Add(listViewItem);
      }
    }
    private void RemoveListViewItem(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      IRun run = listViewItem.Tag as IRun;
      if (run != null) {
        itemListViewItemMapping[run].Remove(listViewItem);
        if (itemListViewItemMapping[run].Count == 0) {
          DeregisterItemEvents(run);
          itemListViewItemMapping.Remove(run);
        }
      }
      listViewItem.Remove();
    }
    private void UpdateListViewItemImage(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      IRun item = listViewItem.Tag as IRun;
      int i = listViewItem.ImageIndex;
      itemsListView.SmallImageList.Images[i] = item == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : item.ItemImage;
      listViewItem.ImageIndex = -1;
      listViewItem.ImageIndex = i;
    }
    private void UpdateListViewItemText(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      IRun item = listViewItem.Tag as IRun;
      listViewItem.Text = item == null ? "null" : item.ToString();
      listViewItem.ToolTipText = item == null ? string.Empty : item.ItemName + ": " + item.ItemDescription;
    }
    private IEnumerable<ListViewItem> GetListViewItemsForItem(IRun item) {
      if (item == null) {
        List<ListViewItem> listViewItems = new List<ListViewItem>();
        foreach (ListViewItem listViewItem in itemsListView.Items) {
          if (listViewItem.Tag == null) listViewItems.Add(listViewItem);
        }
        return listViewItems;
      } else {
        List<ListViewItem> listViewItems = null;
        itemListViewItemMapping.TryGetValue(item, out listViewItems);
        return listViewItems == null ? Enumerable.Empty<ListViewItem>() : listViewItems;
      }
    }

    #region ListView Events
    private void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && (Content != null) && !Content.IsReadOnly && !ReadOnly;
      // for performance reason (multiple selection fires this handler for every selected item)
      if (itemsListView.SelectedIndices.Count <= 1)
        AdjustListViewColumnSizes();
      if (showDetailsCheckBox.Checked) {
        if (itemsListView.SelectedItems.Count == 1) {
          IRun item = (IRun)itemsListView.SelectedItems[0].Tag;
          detailsGroupBox.Enabled = true;
          viewHost.Content = item;
        } else {
          viewHost.Content = null;
          detailsGroupBox.Enabled = false;
        }
      }
    }
    private void itemsListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if ((itemsListView.SelectedItems.Count > 0) && !Content.IsReadOnly && !ReadOnly) {
          foreach (ListViewItem item in itemsListView.SelectedItems)
            Content.Remove((IRun)item.Tag);
        }
      }
    }
    private void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        IRun item = itemsListView.SelectedItems[0].Tag as IRun;
        if (item != null) {
          IContentView view = MainFormManager.MainForm.ShowContent(item);
          if (view != null) {
            view.ReadOnly = ReadOnly;
            view.Locked = Locked;
          }
        }
      }
    }
    private void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!Locked) {
        List<IRun> items = new List<IRun>();
        foreach (ListViewItem listViewItem in itemsListView.SelectedItems) {
          IRun item = listViewItem.Tag as IRun;
          if (item != null) items.Add(item);
        }

        if (items.Count > 0) {
          DataObject data = new DataObject();
          if (items.Count == 1) data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, items[0]);
          else data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, items);
          if (Content.IsReadOnly || ReadOnly) {
            DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
          } else {
            DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
            if (result.HasFlag(DragDropEffects.Move)) {
              foreach (IRun item in items) Content.Remove(item);
            }
          }
        }
      }
    }
    private void itemsListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (!Content.IsReadOnly && !ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IRun)) {
        validDragOperation = true;
      } else if (!Content.IsReadOnly && !ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable)) {
        validDragOperation = true;
        IEnumerable items = (IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        foreach (object item in items)
          validDragOperation = validDragOperation && (item is IRun);
      }
    }
    private void itemsListView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    private void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IRun) {
          IRun item = (IRun)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
          Content.Add(e.Effect.HasFlag(DragDropEffects.Copy) ? (IRun)item.Clone() : item);
        } else if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable) {
          IEnumerable<IRun> items = ((IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat)).Cast<IRun>();
          if (e.Effect.HasFlag(DragDropEffects.Copy)) {
            Cloner cloner = new Cloner();
            items = items.Select(x => cloner.Clone(x));
          }
          if (RunCollection != null) {
            RunCollection.AddRange(items);
          } else { // the content is an IItemCollection<IRun>
            foreach (IRun item in items)
              Content.Add(item);
          }
        }
      }
    }
    #endregion

    #region Button Events
    private void menuItem_Click(object sender, EventArgs e) {
      ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
      Type viewType = (Type)menuItem.Tag;
      IContentView view = MainFormManager.MainForm.ShowContent(Content, viewType);
      if (view != null) {
        view.Locked = Locked;
        view.ReadOnly = ReadOnly;
      }
    }
    private void removeButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in itemsListView.SelectedItems)
          Content.Remove((IRun)item.Tag);
        itemsListView.SelectedItems.Clear();
      }
    }
    private void clearButton_Click(object sender, EventArgs e) {
      Content.Clear();
    }
    #endregion

    #region Control Events
    private void showDetailsCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (showDetailsCheckBox.Checked) {
        splitContainer.Panel2Collapsed = false;
        detailsGroupBox.Enabled = itemsListView.SelectedItems.Count == 1;
        viewHost.Content = itemsListView.SelectedItems.Count == 1 ? (IRun)itemsListView.SelectedItems[0].Tag : null;
      } else {
        splitContainer.Panel2Collapsed = true;
        viewHost.Content = null;
      }
    }
    private void EvaluateModifications() {
      if (RunCollection == null)
        return;
      ReadOnly = true;
      try {
        RunCollection.Modify();
      } finally {
        ReadOnly = false;
      }
    }
    #endregion

    #region Content Events
    private void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded), sender, e);
      else {
        foreach (IRun item in e.Items)
          AddListViewItem(CreateListViewItem(item));

        AdjustListViewColumnSizes();
        analyzeRunsToolStripDropDownButton.Enabled = itemsListView.Items.Count > 0;
        clearButton.Enabled = itemsListView.Items.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
      }
    }
    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved), sender, e);
      else {
        foreach (IRun item in e.Items) {
          //remove only the first matching ListViewItem, because the IRun could be contained multiple times in the ItemCollection
          ListViewItem listViewItem = GetListViewItemsForItem(item).FirstOrDefault();
          if (listViewItem != null) RemoveListViewItem(listViewItem);
        }
        RebuildImageList();
        analyzeRunsToolStripDropDownButton.Enabled = itemsListView.Items.Count > 0;
        clearButton.Enabled = itemsListView.Items.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
      }
    }
    private void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset), sender, e);
      else {
        foreach (IRun item in e.OldItems) {
          //remove only the first matching ListViewItem, because the IRun could be contained multiple times in the ItemCollection
          ListViewItem listViewItem = GetListViewItemsForItem(item).FirstOrDefault();
          if (listViewItem != null) RemoveListViewItem(listViewItem);
        }
        RebuildImageList();
        foreach (IRun item in e.Items)
          AddListViewItem(CreateListViewItem(item));

        AdjustListViewColumnSizes();
        analyzeRunsToolStripDropDownButton.Enabled = itemsListView.Items.Count > 0;
        clearButton.Enabled = itemsListView.Items.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
      }
    }
    #endregion

    #region Item Events
    private void Item_ItemImageChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ItemImageChanged), sender, e);
      else {
        IRun item = (IRun)sender;
        foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
          UpdateListViewItemImage(listViewItem);
      }
    }
    private void Item_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ToStringChanged), sender, e);
      else {
        IRun item = (IRun)sender;
        foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
          UpdateListViewItemText(listViewItem);
        AdjustListViewColumnSizes();
      }
    }
    private void Item_Changed(object sender, EventArgs e) {
      if (InvokeRequired)
        this.Invoke(new EventHandler(Item_Changed), sender, e);
      else {
        IRun run = (IRun)sender;
        UpdateRun(run);
      }
    }

    private void UpdateRun(IRun run) {
      foreach (ListViewItem listViewItem in GetListViewItemsForItem(run)) {
        if (run.Visible) {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
          listViewItem.ForeColor = run.Color;
        } else {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Italic);
          listViewItem.ForeColor = Color.LightGray;
        }
      }
    }
    #endregion

    #region Helpers
    private void AdjustListViewColumnSizes() {
      if (itemsListView.Items.Count > 0) {
        for (int i = 0; i < itemsListView.Columns.Count; i++) {
          itemsListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }
    private void RebuildImageList() {
      itemsListView.SmallImageList.Images.Clear();
      foreach (ListViewItem listViewItem in itemsListView.Items) {
        IRun item = listViewItem.Tag as IRun;
        itemsListView.SmallImageList.Images.Add(item == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : item.ItemImage);
        listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      }
    }
    #endregion
  }
}
