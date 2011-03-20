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
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("ParameterCollection View")]
  [Content(typeof(ParameterCollection), true)]
  [Content(typeof(IKeyedItemCollection<string, IParameter>), false)]
  public partial class ParameterCollectionView : NamedItemCollectionView<IParameter> {
    protected CreateParameterDialog createParameterDialog;
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public ParameterCollectionView() {
      InitializeComponent();
      itemsGroupBox.Text = "Parameters";
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

    protected override void AddListViewItem(ListViewItem listViewItem) {
      IParameter parameter = listViewItem.Tag as IParameter;
      if ((parameter != null) && (parameter.Hidden) && (!showHiddenParametersCheckBox.Checked)) {
        return; // skip parameter
      }
      if ((parameter != null) && (parameter.Hidden) && (showHiddenParametersCheckBox.Checked)) {
        listViewItem.Font = new Font(listViewItem.Font, FontStyle.Italic);
        listViewItem.ForeColor = Color.LightGray;
      }
      base.AddListViewItem(listViewItem);
    }

    protected virtual void UpdateParameterVisibility(IParameter parameter) {
      if (parameter.Hidden) {
        if (showHiddenParametersCheckBox.Checked) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(parameter)) {
            listViewItem.Font = new Font(listViewItem.Font, FontStyle.Italic);
            listViewItem.ForeColor = Color.LightGray;
          }
        } else {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(parameter).ToArray())
            RemoveListViewItem(listViewItem);
          RebuildImageList();
        }
      } else {
        if (showHiddenParametersCheckBox.Checked) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(parameter)) {
            listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
            listViewItem.ForeColor = itemsListView.ForeColor;
          }
        } else {
          for (int i = 0; i < Content.Count(x => x == parameter); i++)
            AddListViewItem(CreateListViewItem(parameter));
        }
      }
      AdjustListViewColumnSizes();
    }

    #region Control Events
    protected virtual void showHiddenParametersCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      if (showHiddenParametersCheckBox.Checked) {
        foreach (IParameter parameter in Content.Where(x => x.Hidden))
          AddListViewItem(CreateListViewItem(parameter));
        AdjustListViewColumnSizes();
      } else {
        foreach (IParameter parameter in Content.Where(x => x.Hidden)) {
          foreach (ListViewItem listViewItem in GetListViewItemsForItem(parameter).ToArray())
            RemoveListViewItem(listViewItem);
        }
        RebuildImageList();
      }
    }
    protected virtual void itemsListViewContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      if ((itemsListView.SelectedItems.Count == 0) || ReadOnly || Locked) {
        showHideParametersToolStripMenuItem.Enabled = false;
      } else {
        List<IParameter> parameters = new List<IParameter>();
        foreach (ListViewItem listViewItem in itemsListView.SelectedItems) {
          IParameter parameter = listViewItem.Tag as IParameter;
          if (parameter != null) parameters.Add(parameter);
        }
        showHideParametersToolStripMenuItem.Enabled = (parameters.Count > 0) && (parameters.All(x => x.Hidden == parameters[0].Hidden));
        if (parameters.Count == 1) showHideParametersToolStripMenuItem.Text = parameters[0].Hidden ? "Show Parameter" : "Hide Parameter";
        else showHideParametersToolStripMenuItem.Text = parameters[0].Hidden ? "Show Parameters" : "Hide Parameters";
        showHideParametersToolStripMenuItem.Tag = parameters;
      }
    }
    protected virtual void showHideParametersToolStripMenuItem_Click(object sender, System.EventArgs e) {
      foreach (IParameter parameter in (IEnumerable<IParameter>)showHideParametersToolStripMenuItem.Tag)
        parameter.Hidden = !parameter.Hidden;
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
  }
}
