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
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The visual representation of a <see cref="Variable"/>.
  /// </summary>
  [View("Result View")]
  [Content(typeof(Result), true)]
  [Content(typeof(IResult), false)]
  public sealed partial class ResultView : NamedItemView {
    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new IResult Content {
      get { return (IResult)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get { return base.ReadOnly; }
      set { /*not needed because results are always readonly */}
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public ResultView() {
      InitializeComponent();
      Caption = "Result";
      base.ReadOnly = true;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with the given <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariableView()"/>.</remarks>
    /// <param name="variable">The variable to represent visually.</param>
    public ResultView(IResult content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="Variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="Variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "Result";
        dataTypeTextBox.Text = "-";
        viewHost.Content = null;
      } else {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        dataTypeTextBox.Text = Content.DataType.GetPrettyName();
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
      SetEnableStateOfControls();
    }
    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnableStateOfControls();
    }
    private void SetEnableStateOfControls() {
      if (Content == null) {
        dataTypeTextBox.Enabled = false;
        valueGroupBox.Enabled = false;
        viewHost.Enabled = false;
      } else {
        dataTypeTextBox.Enabled = true;
        valueGroupBox.Enabled = true;
        viewHost.Enabled = true;
        viewHost.ReadOnly = ReadOnly;
      }
    }

    private void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
    }
  }
}
