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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of an <see cref="OperatorGraph"/>.
  /// </summary>
  [View("OperatorGraph View (Tree)")]
  [Content(typeof(OperatorGraph), true)]
  public partial class OperatorGraphView : ItemView {
    /// <summary>
    /// Gets or sets the operator graph to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new OperatorGraph Content {
      get { return (OperatorGraph)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphView"/> with caption "Operator Graph".
    /// </summary>
    public OperatorGraphView() {
      InitializeComponent();
      Caption = "Operator Graph";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraphView"/> 
    /// with the given <paramref name="operatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorGraphView()"/>.</remarks>
    /// <param name="operatorGraph">The operator graph to represent visually.</param>
    public OperatorGraphView(OperatorGraph content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IOperatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.InitialOperatorChanged -= new EventHandler(Content_InitialOperatorChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IOperatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.InitialOperatorChanged += new EventHandler(Content_InitialOperatorChanged);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void OnContentChanged() {
      base.OnContentChanged();
      Caption = "Operator Graph";
      operatorsView.Content = null;
      operatorsView.Enabled = false;
      operatorTreeView.Content = null;
      operatorTreeView.Enabled = false;

      if (Content != null) {
        Caption = Content.ItemName + " (" + Content.GetType().Name + ")";
        operatorsView.Content = Content.Operators;
        operatorsView.Enabled = true;
        MarkInitialOperator();
        operatorTreeView.Content = Content.InitialOperator;
        operatorTreeView.Enabled = true;
      }
    }

    protected virtual void MarkInitialOperator() {
      foreach (ListViewItem item in operatorsView.ItemsListView.Items) {
        if ((Content.InitialOperator != null) && (((IOperator)item.Tag) == Content.InitialOperator))
          item.Font = new Font(operatorsView.ItemsListView.Font, FontStyle.Bold);
        else
          item.Font = operatorsView.ItemsListView.Font;
      }
    }

    #region Context Menu Events
    protected virtual void operatorsView_Load(object sender, EventArgs e) {
      operatorsView.ItemsListView.ContextMenuStrip = operatorsContextMenuStrip;
    }
    protected virtual void operatorsContextMenuStrip_Opening(object sender, CancelEventArgs e) {
      initialOperatorToolStripMenuItem.Enabled = false;
      initialOperatorToolStripMenuItem.Checked = false;
      if (operatorsView.ItemsListView.SelectedItems.Count == 1) {
        IOperator op = (IOperator)operatorsView.ItemsListView.SelectedItems[0].Tag;
        initialOperatorToolStripMenuItem.Enabled = true;
        initialOperatorToolStripMenuItem.Tag = op;
        if (op == Content.InitialOperator)
          initialOperatorToolStripMenuItem.Checked = true;
      }
    }
    protected virtual void initialOperatorToolStripMenuItem_Click(object sender, EventArgs e) {
      if (initialOperatorToolStripMenuItem.Checked)
        Content.InitialOperator = (IOperator)initialOperatorToolStripMenuItem.Tag;
      else
        Content.InitialOperator = null;
    }
    #endregion

    #region Content Events
    protected virtual void Content_InitialOperatorChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_InitialOperatorChanged), sender, e);
      else {
        MarkInitialOperator();
        operatorTreeView.Content = Content.InitialOperator;
      }
    }
    #endregion
  }
}
