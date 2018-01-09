#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [View("ValuesCollector View")]
  [Content(typeof(ValuesCollector), true)]
  public partial class ValuesCollectorView : NamedItemView {
    public new ValuesCollector Content {
      get { return (ValuesCollector)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public ValuesCollectorView() {
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
        collectedValuesView.Content = null;
        parameterCollectionView.Content = null;
      } else {
        breakpointCheckBox.Checked = Content.Breakpoint;
        collectedValuesView.Content = Content.CollectedValues;
        parameterCollectionView.Content = ((IOperator)Content).Parameters;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      breakpointCheckBox.Enabled = Content != null && !Locked;
      collectedValuesView.Enabled = Content != null;
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
