#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core.Views {
  [View("OperatorSet View")]
  [Content(typeof(OperatorSet), true)]
  [Content(typeof(IItemSet<IOperator>), false)]
  public partial class OperatorSetView : ItemSetView<IOperator> {
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public OperatorSetView() {
      InitializeComponent();
      itemsGroupBox.Text = "Operators";
    }

    protected override IOperator CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Operator";
        typeSelectorDialog.TypeSelector.Caption = "Available Operators";
        typeSelectorDialog.TypeSelector.Configure(typeof(IOperator), false, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          return (IOperator)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
      return null;
    }
  }
}
