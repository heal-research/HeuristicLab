#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Operators;

namespace HeuristicLab.DebugEngine {

  [View("Operator Trace View")]
  [Content(typeof(OperatorTrace), IsDefaultView = true)]
  public partial class OperatorTraceView : AsynchronousContentView {

    public new OperatorTrace Content {
      get { return (OperatorTrace)base.Content; }
      set { base.Content = value; }
    }

    public OperatorTraceView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.CollectionReset -= Content_CollectionChanged;
      Content.ItemsAdded -= Content_CollectionChanged;
      Content.ItemsMoved -= Content_CollectionChanged;
      Content.ItemsRemoved -= Content_CollectionChanged;
      Content.ItemsReplaced -= Content_CollectionChanged;
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CollectionReset += Content_CollectionChanged;
      Content.ItemsAdded += Content_CollectionChanged;
      Content.ItemsMoved += Content_CollectionChanged;
      Content.ItemsRemoved += Content_CollectionChanged;
      Content.ItemsReplaced += Content_CollectionChanged;
    }

    #region Event Handlers (Content)
    void Content_CollectionChanged(object sender, CollectionItemsChangedEventArgs<Collections.IndexedItem<IOperator>> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<CollectionItemsChangedEventArgs<IndexedItem<IOperator>>>(Content_CollectionChanged), sender, e);
      else
        UpdateOperatorTrace();
    }
    #endregion

    private void UpdateOperatorTrace() {
      listView.BeginUpdate();
      listView.Items.Clear();
      listView.SmallImageList.Images.Clear();
      listView.SmallImageList.Images.Add(VS2008ImageLibrary.Method);
      listView.SmallImageList.Images.Add(VS2008ImageLibrary.Module);
      listView.SmallImageList.Images.Add(VS2008ImageLibrary.BreakpointActive);
      foreach (var item in Content) {
        var viewItem = listView.Items.Add(item.Name ?? item.ItemName);
        viewItem.ToolTipText = string.Format("{0}{1}{1}{2}",
          Utils.TypeName(item), Environment.NewLine,
          Utils.Wrap(item.Description, 60));
        viewItem.Tag = item;
        if (item.Breakpoint) {
          viewItem.ForeColor = Color.Red;
          viewItem.ImageIndex = 2;
        } else {
          viewItem.ImageIndex = item is CombinedOperator ? 1 : 0;
        }
      }
      if (listView.Items.Count > 0) {
        for (int i = 0; i < listView.Columns.Count; i++)
          listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
      listView.EndUpdate();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        listView.Items.Clear();
      } else {
        UpdateOperatorTrace();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      isEnabledCheckbox.Enabled = Content != null;
    }

    #region Event Handlers (child controls)
    private void listView_ItemActivate(object sender, EventArgs e) {
      if (listView.SelectedItems.Count > 0) {
        IOperator op = listView.SelectedItems[0].Tag as IOperator;
        if (op != null) {
          MainFormManager.MainForm.ShowContent(op);
        }
      }
    }
    #endregion

    private void isEnabledCheckbox_CheckedChanged(object sender, EventArgs e) {
      Content.IsEnabled = isEnabledCheckbox.Checked;
    }
  }

}
