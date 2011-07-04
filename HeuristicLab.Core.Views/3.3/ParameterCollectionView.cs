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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("ParameterCollection View")]
  [Content(typeof(ParameterCollection), true)]
  [Content(typeof(IKeyedItemCollection<string, IParameter>), false)]
  public partial class ParameterCollectionView : NamedItemCollectionView<IParameter> {
    protected CreateParameterDialog createParameterDialog;

    protected bool allowEditingOfHiddenParameters;
    public virtual bool AllowEditingOfHiddenParameters {
      get { return allowEditingOfHiddenParameters; }
      set {
        if (value != allowEditingOfHiddenParameters) {
          allowEditingOfHiddenParameters = value;
          SetEnabledStateOfControls();
        }
      }
    }

    public ParameterCollectionView() {
      InitializeComponent();
      itemsGroupBox.Text = "Parameters";
      allowEditingOfHiddenParameters = true;
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (createParameterDialog != null) createParameterDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void DeregisterItemEvents(IParameter item) {
      item.HiddenChanged -= new EventHandler(Item_HiddenChanged);
      base.DeregisterItemEvents(item);
    }
    protected override void RegisterItemEvents(IParameter item) {
      base.RegisterItemEvents(item);
      item.HiddenChanged += new EventHandler(Item_HiddenChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if ((Content == null) || !Content.Any(x => x.Hidden))
        showHiddenParametersCheckBox.Checked = false;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      showHiddenParametersCheckBox.Enabled = (Content != null) && Content.Any(x => x.Hidden);
      viewHost.ReadOnly = ReadOnly || ((viewHost.Content is IParameter) && (((IParameter)viewHost.Content).Hidden) && !AllowEditingOfHiddenParameters);
    }

    protected override IParameter CreateItem() {
      if (createParameterDialog == null) createParameterDialog = new CreateParameterDialog();

      if (createParameterDialog.ShowDialog(this) == DialogResult.OK) {
        IParameter param = createParameterDialog.Parameter;
        if ((param != null) && Content.ContainsKey(param.Name))
          param = (IParameter)Activator.CreateInstance(param.GetType(), GetUniqueName(param.Name), param.Description);
        return param;
      }
      return null;
    }

    protected override ListViewItem CreateListViewItem(IParameter item) {
      ListViewItem listViewItem = base.CreateListViewItem(item);
      if ((item != null) && item.Hidden) {
        listViewItem.Font = new Font(listViewItem.Font, FontStyle.Italic);
        listViewItem.ForeColor = Color.LightGray;
      }
      return listViewItem;
    }

    protected override void AddListViewItem(ListViewItem listViewItem) {
      base.AddListViewItem(listViewItem);
      IParameter parameter = listViewItem.Tag as IParameter;
      if ((parameter != null) && parameter.Hidden && !showHiddenParametersCheckBox.Checked) {
        itemsListView.Items.Remove(listViewItem);
        RebuildImageList();
      }
    }

    protected virtual void UpdateParameterVisibility(IParameter parameter) {
      foreach (ListViewItem listViewItem in GetListViewItemsForItem(parameter)) {
        if (parameter.Hidden) {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Italic);
          listViewItem.ForeColor = Color.LightGray;
          if (!showHiddenParametersCheckBox.Checked)
            itemsListView.Items.Remove(listViewItem);
        } else {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
          listViewItem.ForeColor = itemsListView.ForeColor;
          if (!showHiddenParametersCheckBox.Checked)
            itemsListView.Items.Add(listViewItem);
        }
      }
      RebuildImageList();
      AdjustListViewColumnSizes();
      if (!Content.Any(x => x.Hidden)) showHiddenParametersCheckBox.Checked = false;
      showHiddenParametersCheckBox.Enabled = (Content != null) && Content.Any(x => x.Hidden);
    }

    #region Control Events
    protected virtual void showHiddenParametersCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        foreach (IParameter parameter in itemListViewItemMapping.Keys.Where(x => x.Hidden).OrderBy(x => x.ToString())) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(parameter)) {
            if (showHiddenParametersCheckBox.Checked)
              itemsListView.Items.Add(listViewItem);
            else
              itemsListView.Items.Remove(listViewItem);
          }
        }
        RebuildImageList();
        AdjustListViewColumnSizes();
      }
    }
    protected override void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      base.itemsListView_SelectedIndexChanged(sender, e);
      SetEnabledStateOfControls();
    }
    protected override void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        IParameter item = itemsListView.SelectedItems[0].Tag as IParameter;
        if (item != null) {
          IContentView view = MainFormManager.MainForm.ShowContent(item);
          if (view != null) {
            view.ReadOnly = ReadOnly || (item.Hidden && !AllowEditingOfHiddenParameters);
            view.Locked = Locked;
          }
        }
      }
    }
    protected virtual void itemsListViewContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      showHideParametersToolStripMenuItem.Text = "Show/Hide Parameters";
      showHideParametersToolStripMenuItem.Enabled = false;
      if (itemsListView.SelectedItems.Count == 0) {
        e.Cancel = true;
      } else if (!ReadOnly && !Locked && AllowEditingOfHiddenParameters) {
        List<IParameter> parameters = new List<IParameter>();
        foreach (ListViewItem listViewItem in itemsListView.SelectedItems) {
          IParameter parameter = listViewItem.Tag as IParameter;
          if (parameter != null) parameters.Add(parameter);
        }
        showHideParametersToolStripMenuItem.Enabled = parameters.Count > 0;
        if (parameters.Count == 1) showHideParametersToolStripMenuItem.Text = parameters[0].Hidden ? "Show Parameter" : "Hide Parameter";
        else if ((parameters.Count > 1) && parameters.All(x => x.Hidden == parameters[0].Hidden)) showHideParametersToolStripMenuItem.Text = parameters[0].Hidden ? "Show Parameters" : "Hide Parameters";
        showHideParametersToolStripMenuItem.Tag = parameters;
      }
    }
    protected virtual void showHideParametersToolStripMenuItem_Click(object sender, System.EventArgs e) {
      foreach (IParameter parameter in (IEnumerable<IParameter>)showHideParametersToolStripMenuItem.Tag)
        parameter.Hidden = !parameter.Hidden;
    }
    protected override void showDetailsCheckBox_CheckedChanged(object sender, EventArgs e) {
      base.showDetailsCheckBox_CheckedChanged(sender, e);
      SetEnabledStateOfControls();
    }
    #endregion

    #region Content Events
    protected override void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IParameter>(Content_ItemsAdded), sender, e);
      else {
        base.Content_ItemsAdded(sender, e);
        showHiddenParametersCheckBox.Enabled = Content.Any(x => x.Hidden);
      }
    }
    protected override void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IParameter>(Content_ItemsRemoved), sender, e);
      else {
        base.Content_ItemsRemoved(sender, e);
        if (!Content.Any(x => x.Hidden)) showHiddenParametersCheckBox.Checked = false;
        showHiddenParametersCheckBox.Enabled = Content.Any(x => x.Hidden);
      }
    }
    protected override void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IParameter>(Content_CollectionReset), sender, e);
      else {
        base.Content_CollectionReset(sender, e);
        if (!Content.Any(x => x.Hidden)) showHiddenParametersCheckBox.Checked = false;
        showHiddenParametersCheckBox.Enabled = Content.Any(x => x.Hidden);
      }
    }
    #endregion

    #region Item Events
    protected virtual void Item_HiddenChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Item_HiddenChanged), sender, e);
      else
        UpdateParameterVisibility((IParameter)sender);
    }
    #endregion

    #region Helpers
    protected override void RebuildImageList() {
      base.RebuildImageList();
      if (!showHiddenParametersCheckBox.Checked) {
        // update image of hidden list view items
        foreach (IParameter parameter in itemListViewItemMapping.Keys.Where(x => x.Hidden)) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(parameter)) {
            itemsListView.SmallImageList.Images.Add(parameter.ItemImage);
            listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
          }
        }
      }
    }
    #endregion
  }
}
