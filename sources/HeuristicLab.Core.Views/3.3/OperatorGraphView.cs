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
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Collections;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of an <see cref="OperatorGraph"/>.
  /// </summary>
  [Content(typeof(OperatorGraph), true)]
  public partial class OperatorGraphView : ItemView {
    /// <summary>
    /// Gets or sets the operator graph to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public OperatorGraph OperatorGraph {
      get { return (OperatorGraph)Item; }
      set { base.Item = value; }
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
    public OperatorGraphView(OperatorGraph operatorGraph)
      : this() {
      OperatorGraph = operatorGraph;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IOperatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterObjectEvents() {
      OperatorGraph.InitialOperatorChanged -= new EventHandler(OperatorGraph_InitialOperatorChanged);
      base.DeregisterObjectEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IOperatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      OperatorGraph.InitialOperatorChanged += new EventHandler(OperatorGraph_InitialOperatorChanged);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      operatorsView.ItemSet = null;
      operatorsView.Enabled = false;
      graphView.Operator = null;
      graphView.Enabled = false;
      if (OperatorGraph == null) {
        Caption = "Operator Graph";
        operatorsView.ItemSet = null;
        operatorsView.Enabled = false;
        graphView.Operator = null;
        graphView.Enabled = false;
      } else {
        Caption = OperatorGraph.ItemName + " (" + OperatorGraph.GetType().Name + ")";
        operatorsView.ItemSet = OperatorGraph.Operators;
        operatorsView.Enabled = true;
        MarkInitialOperator();
        graphView.Operator = OperatorGraph.InitialOperator;
        graphView.Enabled = true;
      }
    }

    private void MarkInitialOperator() {
      foreach (ListViewItem item in operatorsView.ItemsListView.Items) {
        if ((OperatorGraph.InitialOperator != null) && (((IOperator)item.Tag) == OperatorGraph.InitialOperator))
          item.Font = new Font(operatorsView.ItemsListView.Font, FontStyle.Bold);
        else
          item.Font = operatorsView.ItemsListView.Font;
      }
    }

    #region Context Menu Events
    private void operatorsContextMenuStrip_Opening(object sender, CancelEventArgs e) {
      initialOperatorToolStripMenuItem.Enabled = false;
      initialOperatorToolStripMenuItem.Checked = false;
      if (operatorsView.ItemsListView.SelectedItems.Count == 1) {
        IOperator op = (IOperator)operatorsView.ItemsListView.SelectedItems[0].Tag;
        initialOperatorToolStripMenuItem.Enabled = true;
        initialOperatorToolStripMenuItem.Tag = op;
        if (op == OperatorGraph.InitialOperator)
          initialOperatorToolStripMenuItem.Checked = true;
      }
    }
    private void initialOperatorToolStripMenuItem_Click(object sender, EventArgs e) {
      if (initialOperatorToolStripMenuItem.Checked)
        OperatorGraph.InitialOperator = (IOperator)initialOperatorToolStripMenuItem.Tag;
      else
        OperatorGraph.InitialOperator = null;
    }
    #endregion

    #region OperatorGraph Events
    private void OperatorGraph_InitialOperatorChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(OperatorGraph_InitialOperatorChanged), sender, e);
      else {
        MarkInitialOperator();
        graphView.Operator = OperatorGraph.InitialOperator;
      }
    }
    #endregion

    private void operatorsView_Load(object sender, EventArgs e) {
      operatorsView.ItemsListView.ContextMenuStrip = operatorsContextMenuStrip;
    }
  }
}
