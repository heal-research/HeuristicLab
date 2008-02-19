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

namespace HeuristicLab.Core {
  public partial class OperatorBaseVariableInfosView : ViewBase {
    public IOperator Operator {
      get { return (IOperator)Item; }
      set { base.Item = value; }
    }
    public ICollection<IVariableInfo> SelectedVariableInfos {
      get {
        List<IVariableInfo> selected = new List<IVariableInfo>();
        foreach (ListViewItem item in variableInfosListView.SelectedItems)
          selected.Add((IVariableInfo)item.Tag);
        return selected.AsReadOnly();
      }
    }

    public OperatorBaseVariableInfosView() {
      InitializeComponent();
      variableInfosListView.Columns[0].Width = Math.Max(0, variableInfosListView.Width - 25);
      Caption = "Operator";
    }
    public OperatorBaseVariableInfosView(IOperator op)
      : this() {
      Operator = op;
    }

    protected override void RemoveItemEvents() {
      Operator.VariableInfoAdded -= new EventHandler<VariableInfoEventArgs>(OperatorBase_VariableInfoAdded);
      Operator.VariableInfoRemoved -= new EventHandler<VariableInfoEventArgs>(OperatorBase_VariableInfoRemoved);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Operator.VariableInfoAdded += new EventHandler<VariableInfoEventArgs>(OperatorBase_VariableInfoAdded);
      Operator.VariableInfoRemoved += new EventHandler<VariableInfoEventArgs>(OperatorBase_VariableInfoRemoved);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      variableInfoDetailsGroupBox.Controls.Clear();
      if (Operator == null) {
        Caption = "Operator";
        variableInfosListView.Enabled = false;
        variableInfoDetailsGroupBox.Enabled = false;
      } else {
        Caption = Operator.Name + " (" + Operator.GetType().Name + ")";
        variableInfosListView.Enabled = true;
        foreach (ListViewItem item in variableInfosListView.Items) {
          ((IVariableInfo)item.Tag).ActualNameChanged -= new EventHandler(VariableInfo_ActualNameChanged);
        }
        variableInfosListView.Items.Clear();
        foreach (IVariableInfo variableInfo in Operator.VariableInfos) {
          ListViewItem item = new ListViewItem();
          item.Text = variableInfo.ActualName;
          item.Tag = variableInfo;
          variableInfosListView.Items.Add(item);
          variableInfo.ActualNameChanged += new EventHandler(VariableInfo_ActualNameChanged);
        }
        if (variableInfosListView.Items.Count > 0)
          variableInfosListView.SelectedIndices.Add(0);
      }
    }

    private void variableInfosListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (variableInfoDetailsGroupBox.Controls.Count > 0)
        variableInfoDetailsGroupBox.Controls[0].Dispose();
      variableInfoDetailsGroupBox.Controls.Clear();
      variableInfoDetailsGroupBox.Enabled = false;
      if (variableInfosListView.SelectedItems.Count == 1) {
        IVariableInfo variableInfo = (IVariableInfo)variableInfosListView.SelectedItems[0].Tag;
        Control control = (Control)variableInfo.CreateView();
        variableInfoDetailsGroupBox.Controls.Add(control);
        control.Dock = DockStyle.Fill;
        variableInfoDetailsGroupBox.Enabled = true;
      }
      OnSelectedVariableInfosChanged();
    }

    public event EventHandler SelectedVariableInfosChanged;
    protected virtual void OnSelectedVariableInfosChanged() {
      if (SelectedVariableInfosChanged != null)
        SelectedVariableInfosChanged(this, new EventArgs());
    }

    #region Size Changed Events
    private void variableInfosListView_SizeChanged(object sender, EventArgs e) {
      if (variableInfosListView.Columns.Count > 0)
        variableInfosListView.Columns[0].Width = Math.Max(0, variableInfosListView.Width - 25);
    }
    #endregion

    #region VariableInfo Events
    private void VariableInfo_ActualNameChanged(object sender, EventArgs e) {
      IVariableInfo variableInfo = (IVariableInfo)sender;
      foreach (ListViewItem item in variableInfosListView.Items) {
        if (item.Tag == variableInfo)
          item.Text = variableInfo.ActualName;
      }
    }
    #endregion

    #region OperatorBase Events
    private void OperatorBase_VariableInfoAdded(object sender, VariableInfoEventArgs e) {
      ListViewItem item = new ListViewItem();
      item.Text = e.VariableInfo.ActualName;
      item.Tag = e.VariableInfo;
      variableInfosListView.Items.Add(item);
      e.VariableInfo.ActualNameChanged += new EventHandler(VariableInfo_ActualNameChanged);
    }
    private void OperatorBase_VariableInfoRemoved(object sender, VariableInfoEventArgs e) {
      ListViewItem itemToDelete = null;
      foreach (ListViewItem item in variableInfosListView.Items) {
        if (item.Tag == e.VariableInfo)
          itemToDelete = item;
      }
      e.VariableInfo.ActualNameChanged -= new EventHandler(VariableInfo_ActualNameChanged);
      variableInfosListView.Items.Remove(itemToDelete);
    }
    #endregion
  }
}
