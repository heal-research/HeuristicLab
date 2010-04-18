#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("OptimizerList View")]
  [Content(typeof(OptimizerList), true)]
  [Content(typeof(IItemList<IOptimizer>), false)]
  public partial class OptimizerListView : ItemListView<IOptimizer> {
    protected TypeSelectorDialog typeSelectorDialog;

    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public OptimizerListView() {
      InitializeComponent();
      Caption = "OptimizerList";
      itemsGroupBox.Text = "Optimizers";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public OptimizerListView(IItemList<IOptimizer> content)
      : this() {
      Content = content;
    }

    protected override IOptimizer CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.TypeSelector.Caption = "Available Optimizers";
        typeSelectorDialog.TypeSelector.Configure(typeof(IOptimizer), false, false);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK)
        return typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType() as IOptimizer;
      else
        return null;
    }
  }
}
