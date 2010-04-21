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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Operators.Views { 
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("MultiOperator View")]
  [Content(typeof(MultiOperator<>), true)]
  public partial class MultiOperatorView<T> : NamedItemView where T : class, IOperator {
    public new MultiOperator<T> Content {
      get { return (MultiOperator<T>)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public MultiOperatorView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public MultiOperatorView(MultiOperator<T> content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IOperatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.BreakpointChanged -= new EventHandler(Content_BreakpointChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IOperatorGraph"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.BreakpointChanged += new EventHandler(Content_BreakpointChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        breakpointCheckBox.Checked = false;
        operatorListView.Content = null;
        parameterCollectionView.Content = null;
      } else {
        breakpointCheckBox.Checked = Content.Breakpoint;
        operatorListView.Content = Content.Operators;
        parameterCollectionView.Content = ((IOperator)Content).Parameters;
      }
      SetEnabledStateOfControls();
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }

    private void SetEnabledStateOfControls() {
      breakpointCheckBox.Enabled = Content != null && !ReadOnly;
      operatorListView.Enabled = Content != null;
      operatorListView.ReadOnly = ReadOnly;
      parameterCollectionView.Enabled = Content != null;
      parameterCollectionView.ReadOnly = ReadOnly;
    }

    protected void Content_BreakpointChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_DescriptionChanged), sender, e);
      else
        breakpointCheckBox.Checked = Content.Breakpoint;
    }

    protected void breakpointCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      if (Content != null) Content.Breakpoint = breakpointCheckBox.Checked;
    }
  }
}
