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

namespace HeuristicLab.Operators.Views { 
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [Content(typeof(CombinedOperator), true)]
  public partial class CombinedOperatorView : NamedItemView {
    public CombinedOperator CombinedOperator {
      get { return (CombinedOperator)base.Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public CombinedOperatorView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public CombinedOperatorView(CombinedOperator combinedOperator)
      : this() {
      CombinedOperator = combinedOperator;
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (CombinedOperator == null) {
        parameterCollectionView.NamedItemCollection = null;
        operatorGraphView.OperatorGraph = null;
        tabControl.Enabled = false;
      } else {
        parameterCollectionView.NamedItemCollection = ((IOperator)CombinedOperator).Parameters;
        operatorGraphView.OperatorGraph = CombinedOperator.OperatorGraph;
        tabControl.Enabled = true;
      }
    }
  }
}
