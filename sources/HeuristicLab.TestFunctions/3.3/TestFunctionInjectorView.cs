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

using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.TestFunctions {
  /// <summary>
  /// Visual representation of a <see cref="TestFunctionInjector"/>.
  /// </summary>
  public partial class TestFunctionInjectorView : ViewBase {
    /// <summary>
    /// Gets or sets the TestFunctionInjector to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public TestFunctionInjector TestFunctionInjector {
      get { return (TestFunctionInjector)base.Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TestFunctionInjectorView"/>.
    /// </summary>
    public TestFunctionInjectorView() {
      InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TestFunctionInjectorView"/> with the given
    /// <paramref name="testFunctionInjector"/> to display.
    /// </summary>
    /// <param name="testFunctionInjector">The injector to represent visually.</param>
    public TestFunctionInjectorView(TestFunctionInjector testFunctionInjector)
      : this() {
      TestFunctionInjector = testFunctionInjector;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying controls.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      maximizationCheckBox.DataBindings.Clear();
      dimensionTextBox.DataBindings.Clear();
      lowerBoundTextBox.DataBindings.Clear();
      upperBoundTextBox.DataBindings.Clear();
      base.RemoveItemEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying controls.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      maximizationCheckBox.DataBindings.Add("Checked", TestFunctionInjector, "Maximization");
      dimensionTextBox.DataBindings.Add("Text", TestFunctionInjector, "Dimension");
      lowerBoundTextBox.DataBindings.Add("Text", TestFunctionInjector, "LowerBound");
      upperBoundTextBox.DataBindings.Add("Text", TestFunctionInjector, "UpperBound");
    }

    /// <summary>
    /// Updates all controls with the latest values.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (TestFunctionInjector == null) {
        maximizationCheckBox.Enabled = false;
        maximizationCheckBox.Checked = false;
        dimensionTextBox.Enabled = false;
        dimensionTextBox.Text = "-";
        lowerBoundTextBox.Enabled = false;
        lowerBoundTextBox.Text = "-";
        upperBoundTextBox.Enabled = false;
        upperBoundTextBox.Text = "-";
      } else {
        maximizationCheckBox.Enabled = true;
        dimensionTextBox.Enabled = true;
        lowerBoundTextBox.Enabled = true;
        upperBoundTextBox.Enabled = true;
      }
    }
  }
}
