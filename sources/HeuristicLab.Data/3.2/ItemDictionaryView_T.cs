using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  /// <summary>
  /// The visual representation of the class <see cref="HeuristicLab.Data.ItemDictionary&lt;K,V&gt;"/>.
  /// </summary>
  /// <typeparam name="K">The type of the keys of the dictionary.</typeparam>
  /// <typeparam name="V">The type of the values of the dictionary.</typeparam>
  public partial class ItemDictionaryView<K, V> : ViewBase
    where K : IItem
    where V : IItem {

    private EditKeyValueDialog editKeyValueDialog;

    /// <summary>
    /// Gets or sets the dictionary to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="HeuristicLab.Core.ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public ItemDictionary<K, V> ItemDictionary {
      get { return (ItemDictionary<K, V>) Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Creates a new list view item with the given <paramref name="key"/> and <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key to show in the list item.</param>
    /// <param name="value">The value to show in the list item.</param>
    /// <returns>The created list item as <see cref="ListViewItem"/>.</returns>
    private ListViewItem CreateListViewItem(K key, V value) {
      ListViewItem item = new ListViewItem(key.ToString());
      item.Name = key.ToString();
      item.SubItems.Add(value.ToString());
      item.SubItems[0].Tag = key;
      item.SubItems[1].Tag = value;
      return item;
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ItemDictionaryView&lt;K,V&gt;"/>.
    /// </summary>
    public ItemDictionaryView() {
      InitializeComponent();
      listView.View = View.Details;
      listView.Columns[0].Text = "Key";
      listView.Columns[1].Text = "Value";
      valueTypeTextBox.Text = typeof(V).ToString();
      keyTypeTextBox.Text = typeof(K).ToString();
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ItemDictionaryView&lt;K,V&gt;"/> with the given
    /// <paramref name="dictionary"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="dictionary"/> is not copied!</note>
    /// </summary>
    /// <param name="dictionary">The dictionary to represent visually.</param>
    public ItemDictionaryView(ItemDictionary<K, V> dictionary)
      : this() {
      ItemDictionary = dictionary;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying 
    /// <see cref="HeuristicLab.Data.ItemDictionary&lt;K,V&gt;"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      ItemDictionary.ItemAdded -= new EventHandler<KeyValueEventArgs>(ItemDictionary_ItemInserted);
      ItemDictionary.ItemRemoved -= new EventHandler<KeyValueEventArgs>(ItemDictionary_ItemRemoved);
      ItemDictionary.Cleared -= new EventHandler(ItemDictionary_Cleared);
      base.RemoveItemEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="HeuristicLab.Data.ItemDictionary&lt;K,V&gt;"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ItemDictionary.ItemAdded += new EventHandler<KeyValueEventArgs>(ItemDictionary_ItemInserted);
      ItemDictionary.ItemRemoved += new EventHandler<KeyValueEventArgs>(ItemDictionary_ItemRemoved);
      ItemDictionary.Cleared += new EventHandler(ItemDictionary_Cleared);
    }

    private void listView_SelectedIndexChanged(object sender, EventArgs e) {
      if (detailsPanel.Controls.Count > 0)
        detailsPanel.Controls[0].Dispose();
      if (keyPanel.Controls.Count > 0)
        keyPanel.Controls[0].Dispose();
      detailsPanel.Controls.Clear();
      keyPanel.Controls.Clear();
      detailsPanel.Enabled = false;
      keyPanel.Enabled = false;
      removeButton.Enabled = false;
      if (listView.SelectedItems.Count > 0) {
        removeButton.Enabled = true;
      }
      if (listView.SelectedItems.Count == 1) {
        K key = (K) listView.SelectedItems[0].SubItems[0].Tag;
        V data = (V) listView.SelectedItems[0].SubItems[1].Tag;
        Control keyView = (Control) key.CreateView();
        Control dataView = (Control) data.CreateView();
        keyPanel.Controls.Add(keyView);
        detailsPanel.Controls.Add(dataView);
        detailsPanel.Enabled = true;
      }
      keyPanel.Enabled = false;
    }

    #region Item and ItemDictionary Events
    private void ItemDictionary_ItemInserted(object sender, KeyValueEventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler<KeyValueEventArgs>(ItemDictionary_ItemInserted), sender, e);
      else {
        ListViewItem item = CreateListViewItem((K) e.Key, (V) e.Value);
        listView.Items.Insert(listView.Items.Count, item);
        item.Name = e.Key.ToString();
        e.Value.Changed += new EventHandler(Item_Changed);
        e.Key.Changed += new EventHandler(Item_Changed);
      }
    }

    private void ItemDictionary_ItemRemoved(object sender, KeyValueEventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler<KeyValueEventArgs>(ItemDictionary_ItemRemoved), sender, e);
      else {
        int index = listView.Items.IndexOfKey(e.Key.ToString());
        listView.Items.RemoveAt(index);
        e.Key.Changed -= new EventHandler(Item_Changed);
        e.Value.Changed += new EventHandler(Item_Changed);
      }
    }

    private void ItemDictionary_Cleared(object sender, EventArgs e) {
      Refresh();
    }

    private void Item_Changed(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_Changed), sender, e);
      else {
        IItem data = (IItem) sender;
        foreach (ListViewItem item in listView.Items) {
          if (item.SubItems[0].Tag == data) {
            item.SubItems[0].Text = data.ToString();
            item.Name = data.ToString();
          } else if (item.SubItems[1].Tag == data) {
            item.SubItems[1].Text = data.ToString();
          }
        }
      }
    }
    #endregion

    #region Update Controls
    /// <summary>
    /// Updates the controls with the latest elements of the dictionary.
    /// </summary>
    protected override void UpdateControls() {
      base.UpdateControls();
      detailsPanel.Controls.Clear();
      keyPanel.Controls.Clear();
      detailsPanel.Enabled = false;
      keyPanel.Enabled = false;
      removeButton.Enabled = false;
      if (ItemDictionary != null) {
        foreach (ListViewItem item in listView.Items) {
          ((IItem) item.SubItems[0]).Changed -= new EventHandler(Item_Changed);
          ((IItem) item.SubItems[1]).Changed -= new EventHandler(Item_Changed);
        }
        listView.Items.Clear();
        foreach (KeyValuePair<K, V> data in ItemDictionary) {
          ListViewItem item = CreateListViewItem(data.Key, data.Value);
          listView.Items.Add(item);
          data.Key.Changed += new EventHandler(Item_Changed);
          data.Value.Changed += new EventHandler(Item_Changed);
        }
        addButton.Enabled = true; 
      } else {
        addButton.Enabled = false; 
      }
    }
    #endregion

    #region Button Events
    private void addButton_Click(object sender, EventArgs e) {
      editKeyValueDialog = new EditKeyValueDialog(typeof(K), typeof(V));
      if (editKeyValueDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          if (!ItemDictionary.ContainsKey((K) editKeyValueDialog.Key)) {
            ItemDictionary.Add((K) editKeyValueDialog.Key, (V) editKeyValueDialog.Value);
          }
        } catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
    }

    private void removeButton_Click(object sender, EventArgs e) {
      while (listView.SelectedIndices.Count > 0)
        ItemDictionary.Remove((K) listView.SelectedItems[0].SubItems[0].Tag);
    }
    #endregion
  }
}
