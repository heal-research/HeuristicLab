#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Parameters.Views {
  /// <summary>
  /// The visual representation of a <see cref="Parameter"/>.
  /// </summary>
  [View("LookupParameter View")]
  [Content(typeof(LookupParameter<>), true)]
  [Content(typeof(ILookupParameter<>), false)]
  public partial class LookupParameterView<T> : ParameterView where T : class, IItem {
    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new ILookupParameter<T> Content {
      get { return (ILookupParameter<T>)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public LookupParameterView() {
      InitializeComponent();
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ActualNameChanged -= new EventHandler(Content_ActualNameChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ActualNameChanged += new EventHandler(Content_ActualNameChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null)
        actualNameTextBox.Text = "-";
      else
        actualNameTextBox.Text = Content.ActualName;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      actualNameTextBox.Enabled = Content != null;
      actualNameTextBox.ReadOnly = ReadOnly;
    }

    protected virtual void Content_ActualNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ActualNameChanged), sender, e);
      else
        actualNameTextBox.Text = Content.ActualName;
    }

    protected virtual void actualNameTextBox_Validated(object sender, EventArgs e) {
      Content.ActualName = actualNameTextBox.Text;
    }
  }
}
