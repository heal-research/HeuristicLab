#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Parameters.Views {
  /// <summary>
  /// The visual representation of a <see cref="Parameter"/>.
  /// </summary>
  [View("ScopeTreeLookupParameter View")]
  [Content(typeof(ScopeTreeLookupParameter<>), true)]
  public partial class ScopeTreeLookupParameterView<T> : ParameterView where T : class, IItem {
    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new ScopeTreeLookupParameter<T> Content {
      get { return (ScopeTreeLookupParameter<T>)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public ScopeTreeLookupParameterView() {
      InitializeComponent();
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ActualNameChanged -= new EventHandler(Content_ActualNameChanged);
      Content.DepthChanged -= new EventHandler(Content_DepthChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IVariable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ActualNameChanged += new EventHandler(Content_ActualNameChanged);
      Content.DepthChanged += new EventHandler(Content_DepthChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        actualNameTextBox.Text = string.Empty;
        depthTextBox.Text = string.Empty;
      } else {
        actualNameTextBox.Text = Content.ActualName;
        depthTextBox.Text = Content.Depth.ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      actualNameTextBox.Enabled = Content != null;
      actualNameTextBox.ReadOnly = ReadOnly;
      depthTextBox.Enabled = Content != null;
      depthTextBox.ReadOnly = ReadOnly;
    }

    protected virtual void Content_ActualNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ActualNameChanged), sender, e);
      else
        actualNameTextBox.Text = Content.ActualName;
    }
    protected virtual void Content_DepthChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_DepthChanged), sender, e);
      else
        depthTextBox.Text = Content.Depth.ToString();
    }

    protected virtual void actualNameTextBox_Validated(object sender, EventArgs e) {
      Content.ActualName = actualNameTextBox.Text;
    }
    protected virtual void depthTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        depthLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        depthTextBox.Text = Content.Depth.ToString();
        depthLabel.Focus();  // set focus on label to validate data
      }
    }
    protected virtual void depthTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      int val;
      if (!int.TryParse(depthTextBox.Text, out val) || (val < 0)) {
        e.Cancel = true;
        errorProvider.SetError(depthTextBox, "Invalid Value (depth must be an integer number greater or equal to zero)");
        depthTextBox.SelectAll();
      }
    }
    protected virtual void depthTextBox_Validated(object sender, EventArgs e) {
      Content.Depth = int.Parse(depthTextBox.Text);
      errorProvider.SetError(depthTextBox, string.Empty);
    }
  }
}
