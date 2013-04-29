#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core.Views {
  [View("Clipboard")]
  public sealed partial class Clipboard<T> : HeuristicLab.MainForm.WindowsForms.Sidebar where T : class, IItem {
    private TypeSelectorDialog typeSelectorDialog;
    private Dictionary<T, ListViewItem> itemListViewItemMapping;
    private bool validDragOperation;
    private bool draggedItemsAlreadyContained;

    private string itemsPath;
    public string ItemsPath {
      get { return itemsPath; }
      private set {
        if (string.IsNullOrEmpty(value)) throw new ArgumentException(string.Format("Invalid items path \"{0}\".", value));
        itemsPath = value;
        try {
          if (!Directory.Exists(itemsPath)) {
            Directory.CreateDirectory(itemsPath);
            // directory creation might take some time -> wait until it is definitively created
            while (!Directory.Exists(itemsPath)) {
              Thread.Sleep(100);
              Directory.CreateDirectory(itemsPath);
            }
          }
        }
        catch (Exception ex) {
          throw new ArgumentException(string.Format("Invalid items path \"{0}\".", itemsPath), ex);
        }
      }
    }

    public Clipboard() {
      InitializeComponent();
      ItemsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                  Path.DirectorySeparatorChar + "HeuristicLab" + Path.DirectorySeparatorChar + "Clipboard";
      itemListViewItemMapping = new Dictionary<T, ListViewItem>();
    }
    public Clipboard(string itemsPath) {
      InitializeComponent();
      ItemsPath = itemsPath;
      itemListViewItemMapping = new Dictionary<T, ListViewItem>();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        foreach (T item in itemListViewItemMapping.Keys) {
          item.ItemImageChanged -= new EventHandler(Item_ItemImageChanged);
          item.ToStringChanged -= new EventHandler(Item_ToStringChanged);
        }
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      SetEnabledStateOfControls();
      Enabled = false;
      infoLabel.Text = "Loading ...";
      progressBar.Value = 0;
      infoPanel.Visible = true;
      ThreadPool.QueueUserWorkItem(new WaitCallback(LoadItems));
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      addButton.Enabled = !ReadOnly;
      removeButton.Enabled = !ReadOnly && listView.SelectedItems.Count > 0;
      saveButton.Enabled = !ReadOnly;
    }

    public void AddItem(T item) {
      if (InvokeRequired)
        Invoke(new Action<T>(AddItem), item);
      else {
        if (item == null) throw new ArgumentNullException("item", "Cannot add null item to clipboard.");
        if (!itemListViewItemMapping.ContainsKey(item)) {
          ListViewItem listViewItem = new ListViewItem(item.ToString());
          listViewItem.ToolTipText = item.ItemName + ": " + item.ItemDescription;
          listView.SmallImageList.Images.Add(item.ItemImage);
          listViewItem.ImageIndex = listView.SmallImageList.Images.Count - 1;
          listViewItem.Tag = item;
          listView.Items.Add(listViewItem);
          itemListViewItemMapping.Add(item, listViewItem);
          item.ItemImageChanged += new EventHandler(Item_ItemImageChanged);
          item.ToStringChanged += new EventHandler(Item_ToStringChanged);
          sortAscendingButton.Enabled = sortDescendingButton.Enabled = listView.Items.Count > 1;
          AdjustListViewColumnSizes();
        }
      }
    }

    private void RemoveItem(T item) {
      if (InvokeRequired)
        Invoke(new Action<T>(RemoveItem), item);
      else {
        if (itemListViewItemMapping.ContainsKey(item)) {
          item.ItemImageChanged -= new EventHandler(Item_ItemImageChanged);
          item.ToStringChanged -= new EventHandler(Item_ToStringChanged);
          ListViewItem listViewItem = itemListViewItemMapping[item];
          listViewItem.Remove();
          itemListViewItemMapping.Remove(item);
          sortAscendingButton.Enabled = sortDescendingButton.Enabled = listView.Items.Count > 1;
        }
      }
    }
    private void Save() {
      if (InvokeRequired)
        Invoke(new Action(Save));
      else {
        Enabled = false;
        infoLabel.Text = "Saving ...";
        progressBar.Value = 0;
        infoPanel.Visible = true;
        ThreadPool.QueueUserWorkItem(new WaitCallback(SaveItems));
      }
    }

    #region Loading/Saving Items
    private void LoadItems(object state) {
      string[] items = Directory.GetFiles(ItemsPath);
      foreach (string filename in items) {
        try {
          T item = XmlParser.Deserialize<T>(filename);
          OnItemLoaded(item, progressBar.Maximum / items.Length);
        }
        catch (Exception) { }
      }
      OnAllItemsLoaded();
    }
    private void OnItemLoaded(T item, int progress) {
      if (InvokeRequired)
        Invoke(new Action<T, int>(OnItemLoaded), item, progress);
      else {
        AddItem(item);
        progressBar.Value += progress;
      }
    }
    private void OnAllItemsLoaded() {
      if (InvokeRequired)
        Invoke(new Action(OnAllItemsLoaded));
      else {
        Enabled = true;
        if (listView.Items.Count > 0) {
          for (int i = 0; i < listView.Columns.Count; i++)
            listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
        infoPanel.Visible = false;
      }
    }
    private void SaveItems(object param) {
      Directory.Delete(ItemsPath, true);
      Directory.CreateDirectory(ItemsPath);
      // directory creation might take some time -> wait until it is definitively created
      while (!Directory.Exists(ItemsPath)) {
        Thread.Sleep(100);
        Directory.CreateDirectory(ItemsPath);
      }

      int i = 0;
      T[] items = GetStorableItems(itemListViewItemMapping.Keys);

      foreach (T item in items) {
        try {
          i++;
          SetEnabledStateOfContentViews(item, false);
          XmlGenerator.Serialize(item, ItemsPath + Path.DirectorySeparatorChar + i.ToString("00000000") + ".hl", 9);
          OnItemSaved(item, progressBar.Maximum / listView.Items.Count);
        }
        catch (Exception) { }
        finally {
          SetEnabledStateOfContentViews(item, true);
        }
      }
      OnAllItemsSaved();
    }

    private void OnItemSaved(T item, int progress) {
      if (item != null) {
        if (InvokeRequired)
          Invoke(new Action<T, int>(OnItemSaved), item, progress);
        else {
          progressBar.Value += progress;
        }
      }
    }
    private void OnAllItemsSaved() {
      if (InvokeRequired)
        Invoke(new Action(OnAllItemsLoaded));
      else {
        Enabled = true;
        infoPanel.Visible = false;
      }
    }

    private void SetEnabledStateOfContentViews(IItem item, bool enabled) {
      if (InvokeRequired)
        Invoke((Action<IItem, bool>)SetEnabledStateOfContentViews, item, enabled);
      else {
        var views = MainFormManager.MainForm.Views.OfType<IContentView>().Where(v => v.Content == item).ToList();
        views.ForEach(v => v.Enabled = enabled);
      }
    }

    private static T[] GetStorableItems(IEnumerable<T> items) {
      var query = from item in items
                  let executeable = item as IExecutable
                  let views = MainFormManager.MainForm.Views.OfType<IContentView>().Where(v => v.Content == item)
                  where executeable == null || executeable.ExecutionState != ExecutionState.Started
                  where !views.Any(v => v.Locked)
                  select item;
      T[] itemArray = query.ToArray();
      return itemArray;
    }
    #endregion

    #region ListView Events
    private void listView_SelectedIndexChanged(object sender, EventArgs e) {
      removeButton.Enabled = !ReadOnly && listView.SelectedItems.Count > 0;
    }
    private void listView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if (!ReadOnly && (listView.SelectedItems.Count > 0)) {
          foreach (ListViewItem item in listView.SelectedItems)
            RemoveItem((T)item.Tag);
          RebuildImageList();
        }
      }
    }
    private void listView_DoubleClick(object sender, EventArgs e) {
      if (listView.SelectedItems.Count == 1) {
        T item = (T)listView.SelectedItems[0].Tag;
        IContentView view = MainFormManager.MainForm.ShowContent(item, true);
      }
    }
    private void listView_ItemDrag(object sender, ItemDragEventArgs e) {
      List<T> items = new List<T>();
      foreach (ListViewItem listViewItem in listView.SelectedItems) {
        T item = listViewItem.Tag as T;
        if (item != null) items.Add(item);
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
            foreach (T item in items) RemoveItem(item);
            RebuildImageList();
          }
        }
      }
    }
    private void listView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      draggedItemsAlreadyContained = false;
      if (!ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is T)) {
        validDragOperation = true;
        draggedItemsAlreadyContained = itemListViewItemMapping.ContainsKey((T)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat));
      } else if (!ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable)) {
        validDragOperation = true;
        IEnumerable items = (IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        foreach (object item in items) {
          validDragOperation = validDragOperation && (item is T);
          draggedItemsAlreadyContained = draggedItemsAlreadyContained || itemListViewItemMapping.ContainsKey((T)item);
        }
      }
    }
    private void listView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        if (((e.KeyState & 32) == 32) && !draggedItemsAlreadyContained) e.Effect = DragDropEffects.Link;  // ALT key
        else if (((e.KeyState & 4) == 4) && !draggedItemsAlreadyContained) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move) && !draggedItemsAlreadyContained) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link) && !draggedItemsAlreadyContained) e.Effect = DragDropEffects.Link;
      }
    }
    private void listView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        try {
          if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is T) {
            T item = (T)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
            AddItem(e.Effect.HasFlag(DragDropEffects.Copy) ? (T)item.Clone() : item);
          } else if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable) {
            IEnumerable<T> items = ((IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat)).Cast<T>();
            if (e.Effect.HasFlag(DragDropEffects.Copy)) {
              Cloner cloner = new Cloner();
              items = items.Select(x => cloner.Clone(x));
            }
            foreach (T item in items)
              AddItem(item);
          }
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
    #endregion

    #region Button Events
    private void addButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Item";
        typeSelectorDialog.TypeSelector.Caption = "Available Items";
        typeSelectorDialog.TypeSelector.Configure(typeof(T), false, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          AddItem((T)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType());
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
    private void sortAscendingButton_Click(object sender, EventArgs e) {
      listView.Sorting = SortOrder.None;
      listView.Sorting = SortOrder.Ascending;
    }
    private void sortDescendingButton_Click(object sender, EventArgs e) {
      listView.Sorting = SortOrder.None;
      listView.Sorting = SortOrder.Descending;
    }
    private void removeButton_Click(object sender, EventArgs e) {
      if (listView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in listView.SelectedItems)
          RemoveItem((T)item.Tag);
        RebuildImageList();
      }
    }
    private void saveButton_Click(object sender, EventArgs e) {
      IEnumerable<T> items = itemListViewItemMapping.Keys.Except(GetStorableItems(itemListViewItemMapping.Keys));
      if (items.Any()) {
        string itemNames = string.Join(Environment.NewLine, items.Select(item => item.ToString()).ToArray());
        MessageBox.Show("The following items are not saved, because they are locked (e.g. used in a running algorithm):" + Environment.NewLine + Environment.NewLine +
          itemNames + Environment.NewLine + Environment.NewLine + "All other items will be saved.", "Cannot save all items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      Save();
    }
    #endregion

    #region Item Events
    private void Item_ItemImageChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ItemImageChanged), sender, e);
      else {
        T item = (T)sender;
        ListViewItem listViewItem = itemListViewItemMapping[item];
        int i = listViewItem.ImageIndex;
        listView.SmallImageList.Images[i] = item.ItemImage;
        listViewItem.ImageIndex = -1;
        listViewItem.ImageIndex = i;
      }
    }
    private void Item_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ToStringChanged), sender, e);
      else {
        T item = (T)sender;
        itemListViewItemMapping[item].Text = item.ToString();
        listView.Sort();
        AdjustListViewColumnSizes();
      }
    }
    #endregion

    #region Helpers
    private void AdjustListViewColumnSizes() {
      if (listView.Items.Count > 0) {
        for (int i = 0; i < listView.Columns.Count; i++)
          listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
    }
    private void RebuildImageList() {
      listView.SmallImageList.Images.Clear();
      foreach (ListViewItem item in listView.Items) {
        listView.SmallImageList.Images.Add(((T)item.Tag).ItemImage);
        item.ImageIndex = listView.SmallImageList.Images.Count - 1;
      }
    }
    #endregion
  }
}
