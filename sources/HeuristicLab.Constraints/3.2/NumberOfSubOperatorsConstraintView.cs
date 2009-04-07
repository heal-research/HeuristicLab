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

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Visual representation of a <see cref="NumberOfSubOperatorsConstraint"/>.
  /// </summary>
  public partial class NumberOfSubOperatorsConstraintView : ViewBase {
    private NumberOfSubOperatorsConstraint constraint;

    /// <summary>
    /// Initializes a new instance of <see cref="NumberOfSubOperatorsConstraintView"/>.
    /// </summary>
    public NumberOfSubOperatorsConstraintView() {
      InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumberOfSubOperatorsConstraintView"/>
    /// with the given <paramref name="constraint"/> to display.
    /// </summary>
    /// <param name="constraint">The constraint to represent visually.</param>
    public NumberOfSubOperatorsConstraintView(NumberOfSubOperatorsConstraint constraint) {
      InitializeComponent();
      this.constraint = constraint;

      minDataView.IntData = constraint.MinOperators;
      maxDataView.IntData = constraint.MaxOperators;
    }
  }
}
