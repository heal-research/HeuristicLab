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
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of a <see cref="Parameter"/>.
  /// </summary>
  [Content(typeof(Parameter), true)]
  public partial class ParameterView : NamedItemView {
    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public Parameter Parameter {
      get { return (Parameter)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public ParameterView() {
      InitializeComponent();
      Caption = "Parameter";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with the given <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariableView()"/>.</remarks>
    /// <param name="variable">The variable to represent visually.</param>
    public ParameterView(Parameter parameter)
      : this() {
      Parameter = parameter;
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (Parameter == null) {
        Caption = "Parameter";
        dataTypeTextBox.Text = "-";
        dataTypeTextBox.Enabled = false;
      } else {
        Caption = Parameter.Name + " (" + Parameter.GetType().Name + ")";
        dataTypeTextBox.Text = Parameter.DataType.Name;
        dataTypeTextBox.Enabled = true;
      }
    }
  }
}
