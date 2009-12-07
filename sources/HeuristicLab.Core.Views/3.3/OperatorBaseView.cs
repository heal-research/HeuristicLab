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
  /// The base class for visual representation of operators (contains description view, variable view, 
  /// variable info view,...).
  /// </summary>
  [Content(typeof(OperatorBase), true)]
  public partial class OperatorBaseView : ItemViewBase {
    /// <summary>
    /// Gets or sets the operator to represent visually. 
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public IOperator Operator {
      get { return (IOperator)Item; }
      set {base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBaseView"/> with caption "Operator".
    /// </summary>
    public OperatorBaseView() {
      InitializeComponent();
      Caption = "Operator";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBaseView"/> 
    /// with the given operator <paramref name="op"/>. 
    /// </summary>
    /// <remarks>Calls <see cref="OperatorBaseView()"/>.</remarks>
    /// <param name="op">The operator to represent visually.</param>
    public OperatorBaseView(IOperator op)
      : this() {
      Operator = op;
    }

    /// <summary>
    /// Removes event handlers in all children.
    /// </summary>
    protected override void RemoveItemEvents() {
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseVariablesView.Operator = null;
      operatorBaseDescriptionView.Operator = null;
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds event handlers in all children.
    /// </summary>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorBaseVariableInfosView.Operator = Operator;
      operatorBaseVariablesView.Operator = Operator;
      operatorBaseDescriptionView.Operator = Operator;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (Operator == null) {
        Caption = "Operator";
        tabControl.Enabled = false;
      } else {
        Caption = Operator.Name + " (" + Operator.GetType().Name + ")";
        tabControl.Enabled = true;
      }
    }
  }
}
