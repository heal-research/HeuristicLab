#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core.Views {
  [View("ValueParameterCollection View")]
  [Content(typeof(ValueParameterCollection), true)]
  [Content(typeof(IKeyedItemCollection<string, IValueParameter>), false)]
  public partial class ValueParameterCollectionView : NamedItemCollectionView<IValueParameter> {
    protected CreateParameterDialog createParameterDialog;
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public ValueParameterCollectionView() {
      InitializeComponent();
      itemsGroupBox.Text = "Parameters";
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (createParameterDialog != null) createParameterDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override IValueParameter CreateItem() {
      if (createParameterDialog == null) {
        createParameterDialog = new CreateParameterDialog();
        createParameterDialog.ParameterTypeSelector.Configure(typeof(IValueParameter), false, true);
      }

      if (createParameterDialog.ShowDialog(this) == DialogResult.OK) {
        IValueParameter param = (IValueParameter)createParameterDialog.Parameter;
        if ((param != null) && Content.ContainsKey(param.Name))
          param = (IValueParameter)Activator.CreateInstance(param.GetType(), GetUniqueName(param.Name), param.Description);
        return param;
      }
      return null;
    }
  }
}
