#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Operators.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("AlgorithmOperator View")]
  [Content(typeof(AlgorithmOperator), true)]
  public partial class AlgorithmOperatorView : NamedItemView {
    public new AlgorithmOperator Content {
      get { return (AlgorithmOperator)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public AlgorithmOperatorView() {
      InitializeComponent();
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
        parameterCollectionView.Content = null;
        operatorGraphViewHost.Content = null;
      } else {
        breakpointCheckBox.Checked = Content.Breakpoint;
        parameterCollectionView.Content = ((IOperator)Content).Parameters;
        operatorGraphViewHost.ViewType = null;
        operatorGraphViewHost.Content = Content.OperatorGraph;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      breakpointCheckBox.Enabled = Content != null && !Locked;
      parameterCollectionView.Enabled = Content != null;
      operatorGraphViewHost.Enabled = Content != null;
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
