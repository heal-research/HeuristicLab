#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [View("CheckedMultiOperator View")]
  [Content(typeof(CheckedMultiOperator<>), true)]
  public partial class CheckedMultiOperatorView<T> : NamedItemView where T : class, IOperator {
    public new CheckedMultiOperator<T> Content {
      get { return (CheckedMultiOperator<T>)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CheckedMultiOperatorView"/>.
    /// </summary>
    public CheckedMultiOperatorView() {
      InitializeComponent();
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="CheckedMultiOperator"/>.
    /// </summary>
    /// <remarks>Calls <see cref="NamedItemView.DeregisterContentEvents"/> of base class <see cref="NamedItemView"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.BreakpointChanged -= new EventHandler(Content_BreakpointChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="CheckedMultiOperator"/>.
    /// </summary>
    /// <remarks>Calls <see cref="NamedItemView.RegisterContentEvents"/> of base class <see cref="NamedItemView"/>.</remarks>
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
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      breakpointCheckBox.Enabled = Content != null && !Locked;
      parameterCollectionView.Enabled = Content != null;
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
