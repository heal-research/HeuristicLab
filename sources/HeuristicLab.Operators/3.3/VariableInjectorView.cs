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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Visual representation of <see cref="VariableInjector"/>.
  /// </summary>
  [Content(typeof(VariableInjector), true)]
  public partial class VariableInjectorView : OperatorBaseView {
    /// <summary>
    /// Gets or sets the <see cref="HeuristicLab.Operators.VariableInjector"/> to display.
    /// </summary>
    /// <remarks>Uses property <see cref="OperatorBaseView.Operator"/> of base class
    /// <see cref="OperatorBaseView"/>. No own data storage present.</remarks>
    public VariableInjector VariableInjector {
      get { return (VariableInjector)Operator; }
      set { base.Operator = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableInjectorView"/>.
    /// </summary>
    public VariableInjectorView() {
      InitializeComponent();
      tabControl.SelectedTab = variablesTabPage;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableInjectorView"/> with the specified 
    /// <paramref name="variableInjector"/>.
    /// </summary>
    /// <param name="variableInjector">The variable injector to display.</param>
    public VariableInjectorView(VariableInjector variableInjector)
      : this() {
      VariableInjector = variableInjector;
    }
  }
}
