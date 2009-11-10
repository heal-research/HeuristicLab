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
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// The visual representation of the information of the variables of an operator.
  /// </summary>
  public partial class OperatorBaseVariableInfosView : ViewBase {
    /// <summary>
    /// Gets or sets the operator whose variable infos should be represented visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public IOperator Operator {
      get { return (IOperator)Item; }
      set { base.Item = value; }
    }
    /// <summary>
    /// Gets all selected variable infos.
    /// <note type="caution"> Variable infos are returned read-only!</note>
    /// </summary>
    public ICollection<IVariableInfo> SelectedVariableInfos {
      get {
        List<IVariableInfo> selected = new List<IVariableInfo>();
        foreach (ListViewItem item in variableInfosListView.SelectedItems)
          selected.Add((IVariableInfo)item.Tag);
        return selected.AsReadOnly();
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBaseVariableInfosView"/> with caption "Operator".
    /// </summary>
    public OperatorBaseVariableInfosView() {
      InitializeComponent();
      variableInfosListView.Columns[0].Width = Math.Max(0, variableInfosListView.Width - 25);
      Caption = "Operator";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBaseVariableInfosView"/> with the given operator
    /// <paramref name="op"/>.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorBaseVariableInfosView()"/>.</remarks>
    /// <param name="op">The operator whose variable infos should be displayed.</param>
    public OperatorBaseVariableInfosView(IOperator op)
      : this() {
      Operator = op;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IOperator"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      Operator.VariableInfoAdded -= new EventHandler<EventArgs<IVariableInfo>>(OperatorBase_VariableInfoAdded);
      Operator.VariableInfoRemoved -= new EventHandler<EventArgs<IVariableInfo>>(OperatorBase_VariableInfoRemoved);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IOperator"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Operator.VariableInfoAdded += new EventHandler<EventArgs<IVariableInfo>>(OperatorBase_VariableInfoAdded);
      Operator.VariableInfoRemoved += new EventHandler<EventArgs<IVariableInfo>>(OperatorBase_VariableInfoRemoved);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
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

    /// <summary>
    /// Occurs when the variables were changed, whose infos should be displayed.
    /// </summary>
    public event EventHandler SelectedVariableInfosChanged;
    /// <summary>
    /// Fires a new <c>SelectedVariableInfosChanged</c>.
    /// </summary>
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
    private void OperatorBase_VariableInfoAdded(object sender, EventArgs<IVariableInfo> e) {
      ListViewItem item = new ListViewItem();
      item.Text = e.Value.ActualName;
      item.Tag = e.Value;
      variableInfosListView.Items.Add(item);
      e.Value.ActualNameChanged += new EventHandler(VariableInfo_ActualNameChanged);
    }
    private void OperatorBase_VariableInfoRemoved(object sender, EventArgs<IVariableInfo> e) {
      ListViewItem itemToDelete = null;
      foreach (ListViewItem item in variableInfosListView.Items) {
        if (item.Tag == e.Value)
          itemToDelete = item;
      }
      e.Value.ActualNameChanged -= new EventHandler(VariableInfo_ActualNameChanged);
      variableInfosListView.Items.Remove(itemToDelete);
    }
    #endregion
  }
}
