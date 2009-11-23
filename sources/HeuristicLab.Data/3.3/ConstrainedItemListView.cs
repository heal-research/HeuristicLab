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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data {
  /// <summary>
  /// The visual representation of the class <see cref="ConstrainedItemList"/>.
  /// </summary>
  [Content(typeof(ConstrainedItemList), true)]
  public partial class ConstrainedItemListView : ViewBase {
    private ChooseItemDialog chooseItemDialog;


    /// <summary>
    /// Gets or sets the item list to represent.
    /// </summary>
    /// <remarks>Uses property <see cref="HeuristicLab.Core.ViewBase.Item"/> of base class <see cref="ViewBase"/>.</remarks>
    public ConstrainedItemList ConstrainedItemList {
      get { return (ConstrainedItemList)base.Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ConstrainedItemListView"/>.
    /// </summary>
    public ConstrainedItemListView() {
      InitializeComponent();
      //itemsListView.Columns[0].Width = Math.Max(0, itemsListView.Width - 25);
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="ConstrainedItemListView"/> with the given
    /// <paramref name="constrainedItemList"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="constrainedItemList"/> is not copied!</note>
    /// </summary>
    /// <param name="constrainedItemList">The item list to represent visually.</param>
    public ConstrainedItemListView(ConstrainedItemList constrainedItemList)
      : this() {
      ConstrainedItemList = constrainedItemList;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="ConstrainedItemList"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      ConstrainedItemList.ItemAdded -= new EventHandler<EventArgs<IItem, int>>(ConstrainedItemList_ItemAdded);
      ConstrainedItemList.ItemRemoved -= new EventHandler<EventArgs<IItem, int>>(ConstrainedItemList_ItemRemoved);
      ConstrainedItemList.Cleared -= new EventHandler(ConstrainedItemList_Cleared);
      base.RemoveItemEvents();
    }
    
    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="ConstrainedItemList"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ConstrainedItemList.ItemAdded += new EventHandler<EventArgs<IItem, int>>(ConstrainedItemList_ItemAdded);
      ConstrainedItemList.ItemRemoved += new EventHandler<EventArgs<IItem, int>>(ConstrainedItemList_ItemRemoved);
      ConstrainedItemList.Cleared += new EventHandler(ConstrainedItemList_Cleared);
    }

    /// <summary>
    /// Updates all controls with the latest elements in the list.
    /// </summary>
    protected override void UpdateControls() {
      base.UpdateControls();
      detailsGroupBox.Enabled = false;
      detailsGroupBox.Controls.Clear();
      removeItemButton.Enabled = false;

      if (ConstrainedItemList == null) {
        itemsGroupBox.Enabled = false;
        itemsSplitContainer.Enabled = false;
        constraintsConstrainedItemBaseView.Enabled = false;
      } else {
        foreach (ListViewItem item in itemsListView.Items) {
          ((IItem)item.Tag).Changed -= new EventHandler(Item_Changed);
        }
        itemsListView.Items.Clear();
        foreach (IItem data in ConstrainedItemList) {
          ListViewItem item = new ListViewItem();
          item.Text = data.ToString();
          item.Tag = data;
          itemsListView.Items.Add(item);
          data.Changed += new EventHandler(Item_Changed);
        }
        itemsSplitContainer.Enabled = true;
        constraintsConstrainedItemBaseView.ConstrainedItem = ConstrainedItemList;
        constraintsConstrainedItemBaseView.Enabled = true;
      }
    }

    private void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      detailsGroupBox.Enabled = false;
      if (detailsGroupBox.Controls.Count > 0) detailsGroupBox.Controls[0].Dispose();
      detailsGroupBox.Controls.Clear();
      if (itemsListView.SelectedItems.Count == 1) {
        IItem data = (IItem)itemsListView.SelectedItems[0].Tag;
        Control view = (Control)MainFormManager.CreateDefaultView(data);
        detailsGroupBox.Controls.Add(view);
        view.Dock = DockStyle.Fill;
        detailsGroupBox.Enabled = true;
      }
      removeItemButton.Enabled = (itemsListView.SelectedItems.Count == 1);
    }

    #region ConstrainedItemList changes
    void ConstrainedItemList_ItemAdded(object sender, EventArgs<IItem, int> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<IItem, int>>(ConstrainedItemList_ItemAdded), sender, e);
      else {
        ListViewItem item = new ListViewItem();
        item.Text = e.Value.ToString();
        item.Tag = e.Value;
        itemsListView.Items.Insert(e.Value2, item);
        e.Value.Changed += new EventHandler(Item_Changed);
      }
    }

    void ConstrainedItemList_ItemRemoved(object sender, EventArgs<IItem, int> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<IItem, int>>(ConstrainedItemList_ItemRemoved), sender, e);
      else {
        itemsListView.Items.RemoveAt(e.Value2);
        e.Value.Changed -= new EventHandler(Item_Changed);
      }
    }

    void ConstrainedItemList_Cleared(object sender, EventArgs e) {
      Refresh();
    }

    void Item_Changed(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_Changed), sender, e);
      else {
        IItem data = (IItem)sender;
        foreach (ListViewItem item in itemsListView.Items) {
          if (item.Tag == data)
            item.Text = data.ToString();
        }
      }
    }
    #endregion

    #region Button events
    private void addItemButton_Click(object sender, EventArgs e) {
      if (chooseItemDialog == null) {
        chooseItemDialog = new ChooseItemDialog();
        chooseItemDialog.Caption = "Add Item";
      }
      if (chooseItemDialog.ShowDialog(this) == DialogResult.OK) {
        ICollection<IConstraint> violatedConstraints;
        if (!ConstrainedItemList.TryAdd(chooseItemDialog.Item, out violatedConstraints)) {
          HeuristicLab.Core.Views.Auxiliary.ShowConstraintViolationMessageBox(violatedConstraints);
        }
      }
    }

    private void removeItemButton_Click(object sender, EventArgs e) {
      ICollection<IConstraint> violatedConstraints;
      if (!ConstrainedItemList.TryRemoveAt(itemsListView.SelectedIndices[0], out violatedConstraints)) {
        HeuristicLab.Core.Views.Auxiliary.ShowConstraintViolationMessageBox(violatedConstraints);
      }
    }
    #endregion

    #region Key events
    private void itemsListView_KeyUp(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        ICollection<IConstraint> violatedConstraints;
        if (!ConstrainedItemList.TryRemoveAt(itemsListView.SelectedIndices[0], out violatedConstraints)) {
          HeuristicLab.Core.Views.Auxiliary.ShowConstraintViolationMessageBox(violatedConstraints);
        }
      }
    }
    #endregion
  }
}
