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

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of the description of operators.
  /// </summary>
  public partial class OperatorBaseDescriptionView : ViewBase {
    /// <summary>
    /// Gets or sets the operator whose description should be displayed.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.</remarks>
    public IOperator Operator {
      get { return (IOperator)Item; }
      set { base.Item = value; }
    }
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBaseDescriptionView"/> with caption "Operator".
    /// </summary>
    public OperatorBaseDescriptionView() {
      InitializeComponent();
      Caption = "Operator";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBaseDescriptionView"/> 
    /// with the operator <paramref name="op"/>.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorBaseDescriptionView()"/>.</remarks>
    /// <param name="op">The operator whose description to display.</param>
    public OperatorBaseDescriptionView(IOperator op)
      : this() {
      Operator = op;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (Operator == null) {
        Caption = "Operator";
        descriptionTextBox.Text = "";
        descriptionTextBox.Enabled = false;
      } else {
        Caption = Operator.Name + " (" + Operator.GetType().Name + ")";
        descriptionTextBox.Text = Operator.Description;
        descriptionTextBox.Enabled = true;
      }
    }
  }
}
