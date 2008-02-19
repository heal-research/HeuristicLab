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

namespace HeuristicLab.Operators {
  public partial class CombinedOperatorView : ViewBase {
    public CombinedOperator CombinedOperator {
      get { return (CombinedOperator)Item; }
      set { base.Item = value; }
    }

    protected override void RemoveItemEvents() {
      operatorGraphView.OperatorGraph = null;
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseVariablesView.Operator = null;
      constrainedItemBaseView.ConstrainedItem = null;
      CombinedOperator.DescriptionChanged -= new EventHandler(CombinedOperator_DescriptionChanged);
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorGraphView.OperatorGraph = CombinedOperator.OperatorGraph;
      operatorBaseVariableInfosView.Operator = CombinedOperator;
      operatorBaseVariablesView.Operator = CombinedOperator;
      constrainedItemBaseView.ConstrainedItem = CombinedOperator;
      CombinedOperator.DescriptionChanged += new EventHandler(CombinedOperator_DescriptionChanged);
    }

    public CombinedOperatorView() {
      InitializeComponent();
    }
    public CombinedOperatorView(CombinedOperator combinedOperator)
      : this() {
      CombinedOperator = combinedOperator;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (CombinedOperator == null) {
        descriptionTextBox.Text = "";
        descriptionTextBox.Enabled = false;
      } else {
        descriptionTextBox.Text = CombinedOperator.Description;
        descriptionTextBox.Enabled = true;
      }
    }

    private void CombinedOperator_DescriptionChanged(object sender, EventArgs e) {
      descriptionTextBox.Text = CombinedOperator.Description;
    }
    private void descriptionTextBox_Validated(object sender, EventArgs e) {
      CombinedOperator.SetDescription(descriptionTextBox.Text);
    }
  }
}
