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
  /// The visual representation of a <see cref="CombinedOperator"/>.
  /// </summary>
  [Content(typeof(CombinedOperator), true)]
  public partial class CombinedOperatorView : ViewBase {
    /// <summary>
    /// Gets or sets the combined operator to display.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public CombinedOperator CombinedOperator {
      get { return (CombinedOperator)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Removes the eventhandlers in all children and from the underlying <see cref="CombinedOperator"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      operatorGraphView.OperatorGraph = null;
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseVariablesView.Operator = null;
      CombinedOperator.DescriptionChanged -= new EventHandler(CombinedOperator_DescriptionChanged);
      base.RemoveItemEvents();
    }
    
    /// <summary>
    /// Adds event handlers in all children and to the underlying <see cref="CombinedOperator"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorGraphView.OperatorGraph = CombinedOperator.OperatorGraph;
      operatorBaseVariableInfosView.Operator = CombinedOperator;
      operatorBaseVariablesView.Operator = CombinedOperator;
      CombinedOperator.DescriptionChanged += new EventHandler(CombinedOperator_DescriptionChanged);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CombinedOperatorView"/>.
    /// </summary>
    public CombinedOperatorView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="CombinedOperatorView"/> with the specified 
    /// <paramref name="combinedOperator"/>.
    /// </summary>
    /// <param name="combinedOperator">The combined operator to display.</param>
    public CombinedOperatorView(CombinedOperator combinedOperator)
      : this() {
      CombinedOperator = combinedOperator;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
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

    private void operatorBaseVariableInfosView_SelectedVariableInfosChanged(object sender, EventArgs e) {
      removeVariableInfoButton.Enabled = operatorBaseVariableInfosView.SelectedVariableInfos.Count > 0;
    }

    #region Click Events
    private void addVariableInfoButton_Click(object sender, EventArgs e) {
      AddVariableInfoDialog dialog = new AddVariableInfoDialog();
      if (dialog.ShowDialog(this) == DialogResult.OK) {
        if (CombinedOperator.GetVariableInfo(dialog.VariableInfo.FormalName) != null)
          HeuristicLab.Core.Views.Auxiliary.ShowErrorMessageBox("A variable info with the same formal name already exists.");
        else
          CombinedOperator.AddVariableInfo(dialog.VariableInfo);
      }
      dialog.Dispose();
    }
    private void removeVariableInfoButton_Click(object sender, EventArgs e) {
      IVariableInfo[] selected = new IVariableInfo[operatorBaseVariableInfosView.SelectedVariableInfos.Count];
      operatorBaseVariableInfosView.SelectedVariableInfos.CopyTo(selected, 0);
      for (int i = 0; i < selected.Length; i++)
        CombinedOperator.RemoveVariableInfo(selected[i].FormalName);
    }
    #endregion
  }
}
